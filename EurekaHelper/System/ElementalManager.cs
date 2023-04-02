using Dalamud.Game;
using System;

namespace EurekaHelper.System
{
    public class ElementalManager : IDisposable
    {
        public ElementalManager()
        {
            DalamudApi.ClientState.TerritoryChanged += OnTerritoryChanged;

            if (Utils.IsPlayerInEurekaZone(DalamudApi.ClientState.TerritoryType))
                DalamudApi.Framework.Update += OnUpdate;
        }

        private void OnTerritoryChanged(object? sender, ushort territoryId)
        {
            if (Utils.IsPlayerInEurekaZone(territoryId))
                DalamudApi.Framework.Update += OnUpdate;
            else
                DalamudApi.Framework.Update -= OnUpdate;
        }

        private void OnUpdate(Framework framework)
        {

        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
