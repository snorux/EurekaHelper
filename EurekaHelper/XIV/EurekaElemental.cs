using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Utility;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Numerics;

namespace EurekaHelper.XIV
{
    public class EurekaElemental
    {
        public string Name { get; set; }
        public Vector3 RawPosition { get; init; }
        public Vector2 Position { get; init; }
        public uint ObjectId { get; set; }
        public ushort TerritoryId { get; set; }
        public ushort MapId { get; set; }
        public long LastSeen { get; set; }


        public EurekaElemental(string name, ushort territoryId, Vector3 position, uint objectId) 
        {
            Name = name;
            TerritoryId = territoryId;
            RawPosition = position;
            ObjectId = objectId;

            MapId = territoryId switch
            {
                732 => 414,
                763 => 467,
                795 => 484,
                827 => 515,
                _ => throw new NotImplementedException()
            };

            var territoryType = DalamudApi.DataManager.GetExcelSheet<TerritoryType>()!.GetRow(territoryId);
            Position = MapUtil.WorldToMap(new Vector2(RawPosition.X, RawPosition.Z), territoryType.Map.Value);

            LastSeen = DateTimeOffset.Now.ToUnixTimeSeconds();
        }

        public SeString GetMapLink() => Utils.MapLink(TerritoryId, MapId, Position);

        public MapLinkPayload GetMapLinkPayload() => new(TerritoryId, MapId, Position.X, Position.Y);
    }
}
