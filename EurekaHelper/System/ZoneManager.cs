using Dalamud.Game.Gui.Dtr;
using Dalamud.Hooking;
using Dalamud.Logging;
using Dalamud.Memory;
using Dalamud.Utility.Signatures;
using System;

namespace EurekaHelper.System
{
    public class ZoneManager
    {
        private delegate nint InitZoneDelegate(nint a1, int a2, nint a3);
        private readonly DtrBarEntry _dtrBarEntry;

        public ZoneManager() 
        {
            SignatureHelper.Initialise(this);
            InitZoneHook?.Enable();

            var dtrBarTitle = "Eureka Helper";
            try
            {
                _dtrBarEntry = DalamudApi.DtrBar.Get(dtrBarTitle);
            }
            catch (ArgumentException ex)
            {
                for (var i = 0; i < 5; i++)
                {
                    PluginLog.LogError(ex, $"Failed to acquire DtrBarEntry {dtrBarTitle}, trying {dtrBarTitle}{i}");
                    try
                    {
                        _dtrBarEntry = DalamudApi.DtrBar.Get($"{dtrBarTitle}{i}");
                    }
                    catch (ArgumentException)
                    {
                        continue;
                    }

                    break;
                }
            }
        }

        [Signature("E8 ?? ?? ?? ?? 45 33 C0 48 8D 53 10 8B CE E8 ?? ?? ?? ?? 48 8D 4B 64 ", DetourName = nameof(InitZoneDetour))]
        private readonly Hook<InitZoneDelegate> InitZoneHook = null!;

        private nint InitZoneDetour(nint a1, int a2, nint a3)
        {
            try
            {
                ushort serverId = MemoryHelper.Read<ushort>(a3);
                ushort zoneId = MemoryHelper.Read<ushort>(a3 + 2);
                var zoneName = Utils.GetZoneName(zoneId);

                if (zoneName != null)
                {
                    if (EurekaHelper.Config.DisplayServerId)
                        EurekaHelper.PrintMessage($"{zoneName} Server ID: {serverId}");

                    if (EurekaHelper.Config.DisplayServerIdInServerInfo)
                    {
                        if (_dtrBarEntry != null)
                        {
                            _dtrBarEntry.Text = $"Server ID: {serverId}";
                            _dtrBarEntry.Shown = true;
                        }
                    }
                }
                else
                {
                    if (_dtrBarEntry != null)
                    {
                        _dtrBarEntry.Text = "";
                        _dtrBarEntry.Shown = false;
                    }
                }
            }
            catch (Exception ex)
            {
                PluginLog.Error($"Something went wrong. Please contact the author.\n{ex.Message}");
            }

            return InitZoneHook.Original(a1, a2, a3);
        }

        public void Dispose()
        {
            InitZoneHook?.Dispose();
            _dtrBarEntry?.Dispose();
        }
    }
}
