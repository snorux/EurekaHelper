using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Interface.Windowing;
using Dalamud.Logging;
using Dalamud.Plugin;
using EurekaHelper.System;
using EurekaHelper.Windows;
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

        internal readonly WindowSystem WindowSystem;
        internal readonly PluginWindow PluginWindow;
        internal readonly RelicWindow RelicWindow;
        internal readonly AlarmWindow AlarmWindow;

        internal readonly FateManager FateManager;
        internal readonly ZoneManager ZoneManager;
        internal readonly ElementalManager ElementalManager;
        internal readonly InventoryManager InventoryManager;
        internal readonly AlarmManager AlarmManager;

        public EurekaHelper(DalamudPluginInterface pluginInterface)
        {
            Plugin = this;

            DalamudApi.Initialize(this, pluginInterface);
            Config = (Configuration)DalamudApi.PluginInterface.GetPluginConfig() ?? new();
            Config.Initialize();

            Utils.BuildLgbData();

            FateManager = new(this);
            ZoneManager = new();
            ElementalManager = new();
            InventoryManager = new();
            AlarmManager = new();

            PluginWindow = new(this);
            RelicWindow = new(this);
            AlarmWindow = new(this);

            WindowSystem = new("Eureka Helper");
            WindowSystem.AddWindow(PluginWindow);
            WindowSystem.AddWindow(RelicWindow);
            WindowSystem.AddWindow(AlarmWindow);

            DalamudApi.PluginInterface.UiBuilder.Draw += DrawUI;
            DalamudApi.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
        }

        [Command("/eurekahelper")]
        [Aliases("/ehelper", "/eh")]
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
            var crabTime1 = crabWeatherTimes[0];
            var crabTime2 = crabWeatherTimes[1];
            var crabBuiltString = Utils.ArisuStringbuilder("Crab/KA", "Fog", crabTime1, crabTime2);
            PrintMessage(crabBuiltString.BuiltString);
            PluginLog.Information(crabBuiltString.ToString());
            #endregion

            #region Cassie
            var cassieTime1 = cassieWeatherTimes[0];
            var cassieTime2 = cassieWeatherTimes[1];
            var cassieBuiltString = Utils.ArisuStringbuilder("Cassie", "Blizzards", cassieTime1, cassieTime2);
            PrintMessage(cassieBuiltString.BuiltString);
            PluginLog.Information(cassieBuiltString.ToString());
            #endregion

            #region Skoll
            var skollTime1 = skollWeatherTimes[0];
            var skollTime2 = skollWeatherTimes[1];
            var skollBuildString = Utils.ArisuStringbuilder("Skoll", "Blizzards", skollTime1, skollTime2);
            PrintMessage(skollBuildString.BuiltString);
            PluginLog.Information(skollBuildString.ToString());
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

            await connectionManager.Send(JArray.Parse(@$"[ ""1"", ""1"", ""datacenter:{datacenterId}"", ""phx_join"", {{}} ]").ToString());
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

        [Command("/erelic")]
        [HelpMessage("Opens / Closes the Eureka Relic helper window")]
        private void RelicHelper(string command, string argument) => RelicWindow.IsOpen ^= true;

        [Command("/ealarms")]
        [HelpMessage("Opens / Closes the Eureka Alarms window")]
        private void AlarmCommand(string command, string argument) => AlarmWindow.IsOpen ^= true;

#if DEBUG
        [Command("/edebug")]
        [DoNotShowInHelp]
        private void Debug(string command, string argument)
        {

        }
#endif

        private void DrawUI() => WindowSystem.Draw();
        private void DrawConfigUI() => PluginWindow.IsOpen ^= true;

        public static void PrintMessage(SeString message)
        {
            var sb = new SeStringBuilder()
                .AddUiForeground(60)
                .AddText($"[{Plugin.Name}] ")
                .AddUiForegroundOff()
                .Append(message);

            DalamudApi.ChatGui.Print(new XivChatEntry()
            {
                Type = Config.ChatChannel,
                Message = sb.BuiltString
            });
        }

        public void Dispose()
        {
            WindowSystem.RemoveAllWindows();
            DalamudApi.Dispose();
            FateManager.Dispose();
            ZoneManager.Dispose();
            ElementalManager.Dispose();
            InventoryManager.Dispose();
            AlarmManager.Dispose();
            PluginWindow.GetConnection().Dispose();
            DalamudApi.PluginInterface.RemoveChatLinkHandler();
        }
    }
}