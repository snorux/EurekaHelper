using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Interface.Windowing;
using Dalamud.Logging;
using Dalamud.Plugin;
using EurekaHelper.System;
using EurekaHelper.XIV;
using EurekaHelper.XIV.Zones;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Threading;

namespace EurekaHelper
{
    public class EurekaHelper : IDalamudPlugin
    {
        public string Name => "Eureka Helper";
        public static Configuration Config { get; private set; }
        public static EurekaHelper Plugin { get; private set; }
        public WindowSystem WindowSystem = new("EurekaHelper");
        private PluginWindow PluginWindow { get; init; }

        private readonly FatesManager FatesManager;
        private readonly ZoneManager ZoneManager;

        public EurekaHelper(DalamudPluginInterface pluginInterface)
        {
            Plugin = this;

            DalamudApi.Initialize(this, pluginInterface);
            Config = (Configuration)DalamudApi.PluginInterface.GetPluginConfig() ?? new();
            Config.Initialize();

            FatesManager = new();
            ZoneManager = new();
            PluginWindow = new PluginWindow();

            WindowSystem.AddWindow(PluginWindow);

            DalamudApi.PluginInterface.UiBuilder.Draw += DrawUI;
            DalamudApi.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
        }

        [Command("/eurekahelper")]
        [Aliases("/ehelper")]
        [HelpMessage("Opens / Closes the configuration window")]
        private void ToggleConfig(string command, string argument) => DrawConfigUI();

        [Command("/arisu")]
        [HelpMessage("Display next weather for Crab, Cassie & Skoll.")]
        private void Arisu(string command, string argument)
        {
            var crabWeatherTimes = EurekaPagos.GetWeatherForecast(EurekaWeather.Fog, 2);
            var cassieWeatherTimes = EurekaPagos.GetWeatherForecast(EurekaWeather.Blizzards, 2);
            var skollWeatherTimes = EurekaPyros.GetWeatherForecast(EurekaWeather.Blizzards, 2);

            PrintMessage("Weather timers for important NMs:");
            #region Crab/KA
            var crabTime1 = crabWeatherTimes[0]; var crabTime2 = crabWeatherTimes[1];
            if (crabTime1 < DateTime.Now)
            {
                var currTimeDiff = crabTime1 + TimeSpan.FromMilliseconds(EorzeaTime.EIGHT_HOURS) - DateTime.Now;
                var nextCrabTimeDiff = crabTime2 - DateTime.Now;
                PluginLog.Information($"Crab/KA weather is up now! It ends in {(int)Math.Round(currTimeDiff.TotalMinutes)}m. Next Crab/KA weather (Fog) in {(int)Math.Round(nextCrabTimeDiff.TotalMinutes)}m @ {crabTime2:d MMM yyyy hh:mm tt}");
                var sb = new SeStringBuilder()
                    .AddUiForeground(523)
                    .AddText("Crab/KA weather is up now! It ends in ")
                    .AddUiForegroundOff()
                    .AddUiForeground(508)
                    .AddText($"{(int)Math.Round(currTimeDiff.TotalMinutes)}m. ")
                    .AddUiForegroundOff()
                    .AddUiForeground(523)
                    .AddText("Next Crab/KA weather (Fog) in ")
                    .AddUiForegroundOff()
                    .AddUiForeground(508)
                    .AddText($"{(int)Math.Round(nextCrabTimeDiff.TotalMinutes)}m ")
                    .AddUiForegroundOff()
                    .AddUiForeground(523)
                    .AddText("@ ")
                    .AddUiForegroundOff()
                    .AddUiForeground(508)
                    .AddText($"{crabTime2:d MMM yyyy hh:mm tt}")
                    .AddUiForegroundOff();
                PrintMessage(sb.BuiltString);
            }
            else
            {
                var nextCrabTimeDiff = crabTime1 - DateTime.Now;
                PluginLog.Information($"Next Crab/KA weather (Fog) in {(int)Math.Round(nextCrabTimeDiff.TotalMinutes)}m @ {crabTime1:d MMM yyyy hh:mm tt}");
                var sb = new SeStringBuilder()
                    .AddUiForeground(523)
                    .AddText("Next Crab/KA weather (Fog) in ")
                    .AddUiForegroundOff()
                    .AddUiForeground(508)
                    .AddText($"{(int)Math.Round(nextCrabTimeDiff.TotalMinutes)}m ")
                    .AddUiForegroundOff()
                    .AddUiForeground(523)
                    .AddText("@ ")
                    .AddUiForegroundOff()
                    .AddUiForeground(508)
                    .AddText($"{crabTime1:d MMM yyyy hh:mm tt}")
                    .AddUiForegroundOff();
                PrintMessage(sb.BuiltString);
            }
            #endregion

            #region Cassie
            var cassieTime1 = cassieWeatherTimes[0]; var cassieTime2 = cassieWeatherTimes[1];
            if (cassieTime1 < DateTime.Now)
            {
                var currTimeDiff = cassieTime1 + TimeSpan.FromMilliseconds(EorzeaTime.EIGHT_HOURS) - DateTime.Now;
                var nextCassieTimeDiff = cassieTime2 - DateTime.Now;
                PluginLog.Information($"Cassie weather is up now! It ends in {(int)Math.Round(currTimeDiff.TotalMinutes)}m. Next Cassie weather (Blizzards) in {(int)Math.Round(nextCassieTimeDiff.TotalMinutes)}m @ {cassieTime2:d MMM yyyy hh:mm tt}");
                var sb = new SeStringBuilder()
                    .AddUiForeground(523)
                    .AddText("Cassie weather is up now! It ends in ")
                    .AddUiForegroundOff()
                    .AddUiForeground(508)
                    .AddText($"{(int)Math.Round(currTimeDiff.TotalMinutes)}m. ")
                    .AddUiForegroundOff()
                    .AddUiForeground(523)
                    .AddText("Next Cassie weather (Blizzards) in ")
                    .AddUiForegroundOff()
                    .AddUiForeground(508)
                    .AddText($"{(int)Math.Round(nextCassieTimeDiff.TotalMinutes)}m ")
                    .AddUiForegroundOff()
                    .AddUiForeground(523)
                    .AddText("@ ")
                    .AddUiForegroundOff()
                    .AddUiForeground(508)
                    .AddText($"{cassieTime2:d MMM yyyy hh:mm tt}")
                    .AddUiForegroundOff();
                PrintMessage(sb.BuiltString);
            }
            else
            {
                var nextCassieTimeDiff = cassieTime1 - DateTime.Now;
                PluginLog.Information($"Next Cassie weather (Blizzards) in {(int)Math.Round(nextCassieTimeDiff.TotalMinutes)}m @ {cassieTime1:d MMM yyyy hh:mm tt}");
                var sb = new SeStringBuilder()
                    .AddUiForeground(523)
                    .AddText("Next Cassie weather (Blizzards) in ")
                    .AddUiForegroundOff()
                    .AddUiForeground(508)
                    .AddText($"{(int)Math.Round(nextCassieTimeDiff.TotalMinutes)}m ")
                    .AddUiForegroundOff()
                    .AddUiForeground(523)
                    .AddText("@ ")
                    .AddUiForegroundOff()
                    .AddUiForeground(508)
                    .AddText($"{cassieTime1:d MMM yyyy hh:mm tt}")
                    .AddUiForegroundOff();
                PrintMessage(sb.BuiltString);
            }
            #endregion

            #region Skoll
            var skollTime1 = skollWeatherTimes[0]; var skollTime2 = skollWeatherTimes[1];
            if (skollTime1 < DateTime.Now)
            {
                var currTimeDiff = skollTime1 + TimeSpan.FromMilliseconds(EorzeaTime.EIGHT_HOURS) - DateTime.Now;
                var nextSkollTimeDiff = skollTime2 - DateTime.Now;
                PluginLog.Information($"Skoll weather is up now! It ends in {(int)Math.Round(currTimeDiff.TotalMinutes)}m. Next Skoll weather (Blizzards) in {(int)Math.Round(nextSkollTimeDiff.TotalMinutes)}m @ {skollTime2:d MMM yyyy hh:mm tt}");
                var sb = new SeStringBuilder()
                    .AddUiForeground(523)
                    .AddText("Skoll weather is up now! It ends in ")
                    .AddUiForegroundOff()
                    .AddUiForeground(508)
                    .AddText($"{(int)Math.Round(currTimeDiff.TotalMinutes)}m. ")
                    .AddUiForegroundOff()
                    .AddUiForeground(523)
                    .AddText("Next Skoll weather (Blizzards) in ")
                    .AddUiForegroundOff()
                    .AddUiForeground(508)
                    .AddText($"{(int)Math.Round(nextSkollTimeDiff.TotalMinutes)}m ")
                    .AddUiForegroundOff()
                    .AddUiForeground(523)
                    .AddText("@ ")
                    .AddUiForegroundOff()
                    .AddUiForeground(508)
                    .AddText($"{skollTime2:d MMM yyyy hh:mm tt}")
                    .AddUiForegroundOff();
                PrintMessage(sb.BuiltString);
            }
            else
            {
                var nextSkollTimeDiff = skollTime1 - DateTime.Now;
                PluginLog.Information($"Next Skoll weather (Blizzards) in {(int)Math.Round(nextSkollTimeDiff.TotalMinutes)}m @ {skollTime1:d MMM yyyy hh:mm tt}");
                var sb = new SeStringBuilder()
                    .AddUiForeground(523)
                    .AddText("Next Skoll weather (Blizzards) in ")
                    .AddUiForegroundOff()
                    .AddUiForeground(508)
                    .AddText($"{(int)Math.Round(nextSkollTimeDiff.TotalMinutes)}m ")
                    .AddUiForegroundOff()
                    .AddUiForeground(523)
                    .AddText("@ ")
                    .AddUiForegroundOff()
                    .AddUiForeground(508)
                    .AddText($"{skollTime1:d MMM yyyy hh:mm tt}")
                    .AddUiForegroundOff();
                PrintMessage(sb.BuiltString);
            }
            #endregion
        }

        [Command("/etrackers")]
        [HelpMessage("Attempts to get a tracker for the current instance in the same datacenter.")]
        private async void ETrackers(string command, string argument)
        {
            var connectionManager = await EurekaConnectionManager.Connect();

            var datacenterId = Utils.DatacenterToEurekaDatacenterId(DalamudApi.ClientState.LocalPlayer.CurrentWorld.GameData.DataCenter.Value.Name.RawString);
            if (datacenterId == 0)
            {
                PrintMessage("This datacenter is not supported currently. Please submit an issue if you think this is incorrect.");
                await connectionManager.Close();
                return;
            }

            await connectionManager.Send(JArray.Parse(@$"[ '1', '1', 'datacenter:{datacenterId}', 'phx_join', {{}} ]").ToString());
            Thread.Sleep(500);

            var trackerList = connectionManager.GetCurrentTrackers();
            await connectionManager.Close();

            var filteredList = trackerList.Where(x => (int)x["relationships"]["zone"]["data"]["id"] == Utils.GetIndexOfZone(DalamudApi.ClientState.TerritoryType));
            if (!filteredList.Any())
            {
                PrintMessage("Unable to find any public trackers.");
                return;
            }

            var sb = new SeStringBuilder()
                            .AddText("Found")
                            .AddUiForeground(58)
                            .AddText($" {filteredList.Count()} ")
                            .AddUiForegroundOff()
                            .AddText("public trackers:");
            PrintMessage(sb.BuiltString);

            foreach (var tracker in filteredList)
                PrintMessage(Utils.CombineUrl(Constants.EurekaTrackerLink, tracker["id"].ToString()));
        }

        private void DrawUI() => WindowSystem.Draw();

        private void DrawConfigUI() => PluginWindow.IsOpen = true;

        public static void PrintMessage(SeString message)
        {
            var sb = new SeStringBuilder()
                .AddUiForeground(60)
                .AddText($"[{Plugin.Name}] ")
                .AddUiForegroundOff()
                .Append(message);

            DalamudApi.ChatGui.PrintChat(new XivChatEntry()
            {
                Type = XivChatType.Echo,
                Message = sb.BuiltString
            });
        }

        public void Dispose()
        {
            WindowSystem.RemoveAllWindows();
            DalamudApi.Dispose();
            FatesManager.Dispose();
            ZoneManager.Dispose();
            PluginWindow.GetConnection().Dispose();
        }
    }
}