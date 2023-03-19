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

        public ZoneManager() 
        {
            SignatureHelper.Initialise(this);
            InitZoneHook?.Enable();
        }

        [Signature("E8 ?? ?? ?? ?? 45 33 C0 48 8D 53 10 8B CE E8 ?? ?? ?? ??", DetourName = nameof(InitZoneDetour))]
        private readonly Hook<InitZoneDelegate> InitZoneHook = null!;

        private nint InitZoneDetour(nint a1, int a2, nint a3)
        {
            try
            {
                ushort serverId = MemoryHelper.Read<ushort>(a3);
                ushort zoneId = MemoryHelper.Read<ushort>(a3 + 2);
                var zoneName = Utils.GetZoneName(zoneId);

                if (zoneName != null && EurekaHelper.Config.DisplayServerId)
                    EurekaHelper.PrintMessage($"{zoneName} Server ID: {serverId}");
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
        }
    }
}
