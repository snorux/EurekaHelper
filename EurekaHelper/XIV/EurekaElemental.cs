using FFXIVClientStructs.FFXIV.Common.Math;

namespace EurekaHelper.XIV
{
    public class EurekaElemental
    {
        public Vector3 Position { get; set; }
        public uint ObjectId { get; set; }
        public ushort ZoneId { get; set; }

        public EurekaElemental(ushort zoneId, Vector3 position, uint objectId) 
        {
            ZoneId = zoneId;
            Position = position;
            ObjectId = objectId;
        }
    }
}
