using Dalamud.Logging;
using EurekaHelper.XIV;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EurekaHelper.System
{
    public class EurekaConnectionManager : IDisposable
    {
        private const string TrackerUrl = "wss://ffxiv-eureka.com/socket/websocket?vsn=2.0.0";
        private const string TrackerAPIUrl = "https://ffxiv-eureka.com/api/instances";
        private static HttpClient HttpClient = new();
        private ClientWebSocket ClientWebSocket;
        private CancellationTokenSource CancellationTokenSource;
        private List<JToken> TrackerList;

        private int MessageId;
        private int LastHeartbeatId;

        private bool Connected = false;
        private bool Invalid = false;
        private bool Public = false;
        private string TrackerId;
        private string TrackerPassword;
        private int Viewers;
        private IEurekaTracker Tracker;

        public EurekaConnectionManager()
        {
            ClientWebSocket = new();
            CancellationTokenSource = new();
            TrackerList = new();

            MessageId = 0;
            LastHeartbeatId = -1;

            TrackerId = String.Empty;
            TrackerPassword = String.Empty;
            Viewers = 0;
        }

        public static async Task<EurekaConnectionManager> Connect()
        {
            EurekaConnectionManager connection = new();
            try
            {
                await connection.ClientWebSocket.ConnectAsync(new Uri(TrackerUrl), connection.CancellationTokenSource.Token);
                _ = connection.Receive();
                PluginLog.Information("Successfully connected to websocket");
            }
            catch (Exception ex)
            {
                PluginLog.Information($"Failed to connect to websocket: {ex.Message}");
                connection.Connected = false;
            }

            return connection;
        }

        public async Task Receive()
        {
            ArraySegment<byte> buffer = new(new byte[2048]);
            do
            {
                WebSocketReceiveResult result;
                using MemoryStream memoryStream = new();
                do
                {
                    result = await ClientWebSocket.ReceiveAsync(buffer, CancellationTokenSource.Token);
                    memoryStream.Write(buffer.Array, buffer.Offset, result.Count);
                } while (!result.EndOfMessage);

                if (result.MessageType == WebSocketMessageType.Close)
                    break;

                memoryStream.Seek(0, SeekOrigin.Begin);
                using StreamReader streamReader = new(memoryStream, Encoding.UTF8);

                string data = await streamReader.ReadToEndAsync();
                JArray messageArray = JArray.Parse(data);
                EurekaTrackerMessage message = new(
                    messageArray[0].Type != JTokenType.Null,
                    messageArray[1].Type == JTokenType.Null ? -1 : (int)messageArray[1],
                    (string)messageArray[2],
                    (string)messageArray[3],
                    (JObject)messageArray[4]
                );

                switch (message.Event)
                {
                    case "presence_state":
                        Viewers = message.Payload.Count;
                        break;

                    case "presence_diff":
                        Viewers = Viewers + ((JObject)message.Payload["joins"]).Count - ((JObject)message.Payload["leaves"]).Count;
                        break;

                    case "password_set":
                        if ((bool)message.Payload["success"])
                        {
                            TrackerPassword = (string)message.Payload["password"];
                            PluginLog.Information("Successfully set password for tracker");
                        }
                        else
                        {
                            TrackerPassword = String.Empty;
                            PluginLog.Information("Failed to set password for tracker");
                        }
                        break;

                    case "phx_reply":
                        Console.WriteLine("Receiving phx_reply");
                        if (!((string)message.Payload["status"]).Equals("ok"))
                        {
                            if (message.Payload["response"].Type == JTokenType.String)
                            {
                                if (((string)message.Payload["response"]).Equals("Instance does not exist"))
                                {
                                    PluginLog.Information("Invalid instance. Closing connection");

                                    Invalid = true;
                                    await Close();
                                    break;
                                }
                            }

                            PluginLog.Information($"Received status: \"{message.Payload["status"]}\" and response: \"{message.Payload["response"]["reason"]}\". Closing connection");
                            await Close();
                            break;
                        }

                        if (message.MessageId == LastHeartbeatId)
                        {
                            LastHeartbeatId = -1;
                            break;
                        }

                        break;

                    case "initial_payload":
                        if (message.Payload["data"] is JArray)
                        {
                            foreach (var token in message.Payload["data"])
                                TrackerList.Add(token);

                            break;
                        }

                        int zoneId = (int)message.Payload["data"]["relationships"]["zone"]["data"]["id"];
                        Tracker = Utils.GetEurekaTracker((ushort)zoneId);

                        TrackerId = (string)message.Payload["data"]["id"];
                        TrackerPassword = message.Payload["data"]["attributes"]["password"] != null ? (string)message.Payload["data"]["attributes"]["password"] : String.Empty;
                        Public = message.Payload["data"]["attributes"]["data-center-id"].Type != JTokenType.Null;

                        if (!String.IsNullOrWhiteSpace(TrackerPassword))
                            PluginLog.Information("Connected to tracker with password");
                        else
                            PluginLog.Information("Connected to tracker");

                        {
                            var notoriousMonsters = JObject.Parse((string)message.Payload["data"]["attributes"]["notorious-monsters"]);
                            Dictionary<ushort, long> keyValuePairs = new();
                            foreach (var monster in notoriousMonsters)
                                keyValuePairs.Add(ushort.Parse(monster.Key), (long)monster.Value);

                            Tracker.SetPopTimes(keyValuePairs);
                        }

                        Invalid = false;
                        Connected = true;

                        break;

                    case "payload":
                        Console.WriteLine("Receiving payload");

                        {
                            if (message.Payload["data"]["attributes"]?["notorious-monsters"] != null)
                            {
                                var notoriousMonsters = JObject.Parse((string)message.Payload["data"]["attributes"]["notorious-monsters"]);
                                Dictionary<ushort, long> keyValuePairs = new();
                                foreach (var monster in notoriousMonsters)
                                    keyValuePairs.Add(ushort.Parse(monster.Key), (long)monster.Value);

                                Tracker.SetPopTimes(keyValuePairs);
                            }
                        }

                        if (message.Payload["data"]["attributes"]?["data-center-id"] != null)
                        {
                            if (message.Payload["data"]["attributes"]["data-center-id"].Type != JTokenType.Null)
                            {
                                Public = true;
                                PluginLog.Information("Set tracker visibility to public");
                            }
                            else
                            {
                                Public = false;
                                PluginLog.Information("Set tracker visibility to private");
                            }
                        }

                        break;
                }
            } while (!CancellationTokenSource.Token.IsCancellationRequested);
        }

        public async Task Send(string data) => await ClientWebSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(data)), WebSocketMessageType.Text, true, CancellationTokenSource.Token);

        public async Task Join(string trackerId, string password = null)
        {
            EurekaTrackerMessage eurekaTrackerMessage = new(
                true,
                ++MessageId,
                $"instance:{trackerId}",
                "phx_join",
                String.IsNullOrWhiteSpace(password) ? new JObject() : JObject.Parse(@$"{{ 'password': '{password}' }}"));

            await Send(eurekaTrackerMessage.ToMessage());

            // Send heartbeat every 30s after joining
            _ = Task.Run(async () =>
            {
                while (!CancellationTokenSource.Token.IsCancellationRequested)
                {
                    await Task.Delay(TimeSpan.FromSeconds(30), CancellationTokenSource.Token);
                    await SendHeartbeat();
                    PluginLog.Information("Sending Heartbeat");
                }
            }, CancellationTokenSource.Token);
        }

        public static async Task<EurekaConnectionManager> JoinTracker(string trackerId, string password = null)
        {
            var connection = await Connect();
            await connection.Join(trackerId, password);
            return connection;
        }

        public async Task SendHeartbeat()
        {
            if (LastHeartbeatId != -1)
            {
                PluginLog.Information("No response to heartbeat message received within 30seconds. Closing connection");
                await Close();
                return;
            }

            EurekaTrackerMessage eurekaTrackerMessage = new(
                false,
                ++MessageId,
                "phoenix",
                "heartbeat",
                new JObject());

            LastHeartbeatId = MessageId;
            await Send(eurekaTrackerMessage.ToMessage());
        }

        public async Task SetPassword(string password)
        {
            EurekaTrackerMessage eurekaTrackerMessage = new(
                true,
                ++MessageId,
                $"instance:{TrackerId}",
                "set_password",
                JObject.Parse(@$"{{ 'password': '{password}' }}"));

            await Send(eurekaTrackerMessage.ToMessage());
        }

        public async Task SetTrackerVisiblity(int dataCenterId = -1)
        {
            EurekaTrackerMessage eurekaTrackerMessage = new(
                true,
                ++MessageId,
                $"instance:{TrackerId}",
                "set_instance_information",
                JObject.Parse($"{{ instance_id: null, data_center_id: {(dataCenterId == -1 ? "null" : dataCenterId)} }}"));

            await Send(eurekaTrackerMessage.ToMessage());
        }

        public async Task SetPopTime(ushort trackerId, long killTime)
        {
            EurekaTrackerMessage eurekaTrackerMessage = new(
                true,
                ++MessageId,
                $"instance:{TrackerId}",
                "set_kill_time",
                JObject.Parse($"{{ id: {trackerId}, time: {killTime} }}"));

            await Send(eurekaTrackerMessage.ToMessage());
        }

        public async Task Reset(ushort trackerId)
        {
            EurekaTrackerMessage eurekaTrackerMessage = new(
                true,
                ++MessageId,
                $"instance:{TrackerId}",
                "reset_kill",
                JObject.Parse($"{{ id: {trackerId} }}"));

            await Send(eurekaTrackerMessage.ToMessage());
        }

        public async Task ResetAll()
        {
            EurekaTrackerMessage eurekaTrackerMessage = new(
                true,
                ++MessageId,
                $"instance:{TrackerId}",
                "reset_all",
                null
                );

            await Send(eurekaTrackerMessage.ToMessage());
        }

        public async Task Close()
        {
            Connected = false;

            await ClientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "closing", CancellationToken.None);
            CancellationTokenSource.Cancel();
            PluginLog.Information("Successfully closed the socket connection");

            Public = false;
            TrackerId = String.Empty;
            TrackerPassword = String.Empty;
            Tracker = null;
        }

        public static async Task<(string trackerId, string password)> CreateTracker(int zoneId)
        {
            string jsonContent = JObject.Parse(@$"{{ 'data': {{ 'attributes': {{ 'zone-id': {zoneId} }}, 'type': 'instances' }} }}").ToString();

            var httpResponseMessage = await HttpClient.PostAsync(
                TrackerAPIUrl,
                new StringContent(jsonContent, Encoding.UTF8, "application/json"));

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                string response = await httpResponseMessage.Content.ReadAsStringAsync();
                var json = JObject.Parse(response);
                string trackerId = (string)json["data"]["id"];
                string password = (string)json["data"]["attributes"]["password"];

                return (trackerId, password);
            }

            return (String.Empty, String.Empty);
        }

        public static async Task<(string trackerId, string password)> ExportTracker(string oldTrackerId)
        {
            string jsonContent = JObject.Parse(@$"{{ 'data': {{ 'attributes': {{ 'copy-from': '{oldTrackerId}' }}, 'type': 'instances' }} }}").ToString();

            var httpResponseMessage = await HttpClient.PostAsync(
                TrackerAPIUrl,
                new StringContent(jsonContent, Encoding.UTF8, "application/json"));

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                var response = await httpResponseMessage.Content.ReadAsStringAsync();
                var json = JObject.Parse(response);
                string trackerId = (string)json["data"]["id"];
                string password = (string)json["data"]["attributes"]["password"];

                return (trackerId, password);
            }

            return (String.Empty, String.Empty);
        }

        public List<JToken> GetCurrentTrackers() => TrackerList;

        public bool IsConnected() => this.Connected;

        public int GetViewers() => this.Viewers;

        public string GetTrackerId() => this.TrackerId;

        public string GetTrackerPassword() => this.TrackerPassword;

        public bool IsInvalid() => this.Invalid;

        public bool CanModify() => !String.IsNullOrWhiteSpace(this.TrackerPassword);

        public bool IsPublic() => this.Public;

        public IEurekaTracker GetTracker() => this.Tracker;

        public async void Dispose()
        {
            if (this.IsConnected())
                await Close();
        }
    }
}
