using Dalamud.Game;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Logging;
using EurekaHelper.XIV;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EurekaHelper.System
{
    public class ElementalManager : IDisposable
    {
        public List<EurekaElemental> Elementals = new();

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
            var elementals = DalamudApi.ObjectTable.Where(x => x is BattleNpc bnpc && Constants.EurekaElementals.Contains(bnpc.NameId));
            foreach (var elemental in elementals)
            {
                if (Elementals.Exists(x => x.ObjectId == elemental.ObjectId))
                    continue;

                Elementals.Add(new(DalamudApi.ClientState.TerritoryType, elemental.Position, elemental.ObjectId));
            }
        }

        public void Dispose()
        {
            DalamudApi.ClientState.TerritoryChanged -= OnTerritoryChanged;
            DalamudApi.Framework.Update -= OnUpdate;
        }
    }
}
