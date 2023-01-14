using System.Collections.Generic;

namespace EurekaHelper.XIV
{
    public class EurekaZone
    {
        public ushort ZoneId { get; set; }
        public string ZoneName { get; set; }
        public List<EurekaFate> Fates { get; set; }

        public EurekaZone(ushort zoneId, string zoneName, List<EurekaFate> fates)
        {
            ZoneId = zoneId;
            ZoneName = zoneName;
            Fates = fates;
        }
    }
}
