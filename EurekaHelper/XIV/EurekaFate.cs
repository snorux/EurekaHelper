using System.Numerics;
using System;

namespace EurekaHelper.XIV
{
    public class EurekaFate
    {
        public ushort FateId { get; private set; }
        public ushort? TrackerId { get; private set; }
        public ushort TerritoryId { get; private set; }
        public ushort MapId { get; private set; }
        public string FateName { get; private set; }
        public string BossName { get; private set; }
        public string BossShortName { get; private set; }
        public Vector2 FatePosition { get; set; }
        public string SpawnedBy { get; private set; }
        public Vector2 SpawnByPosition { get; private set; }
        public EurekaWeather SpawnRequiredWeather { get; private set; }
        public EurekaWeather SpawnByRequiredWeather { get; private set; }
        public EurekaElement BossElement { get; private set; }
        public EurekaElement SpawnByElement { get; private set; }
        public bool SpawnByRequiredNight { get; private set; }
        private long KilledAt { get; set; }
        public byte FateProgress { get; set; }
        public bool IncludeInTracker { get; private set; }
        public bool IsBunnyFate { get; private set; }
        public int FateLevel { get; private set; }

        public EurekaFate(ushort fateId, ushort? trackerId, ushort territoryId, ushort mapId, string fateName, string bossName, string bossShortName, Vector2 fatePosition, string spawnedBy, Vector2 spawnedByPosition, EurekaWeather spawnRequiredWeather, EurekaWeather spawnByRequiredWeather, EurekaElement bossElement, EurekaElement spawnByElement, bool spawnByRequiredNight, int fateLevel, bool includeInTracker = true, bool isBunnyFate = false)
        {
            FateId = fateId;
            TrackerId = trackerId;
            TerritoryId = territoryId;
            MapId = mapId;
            FateName = fateName;
            BossName = bossName;
            BossShortName = bossShortName;
            FatePosition = fatePosition;
            SpawnedBy = spawnedBy;
            SpawnByPosition = spawnedByPosition;
            SpawnRequiredWeather = spawnRequiredWeather;
            SpawnByRequiredWeather = spawnByRequiredWeather;
            BossElement = bossElement;
            SpawnByElement = spawnByElement;
            SpawnByRequiredNight = spawnByRequiredNight;
            KilledAt = -1;
            IncludeInTracker = includeInTracker;
            IsBunnyFate = isBunnyFate;
            FateLevel = fateLevel;
        }

        public bool IsPopped() => KilledAt != -1 && (KilledAt + 7200000) > DateTimeOffset.Now.ToUnixTimeMilliseconds();

        public bool IsRespawnTimeWithinRange(TimeSpan timespan) => GetRespawnTimeleft() <= timespan;

        public DateTime GetPoppedTime() => EorzeaTime.Zero.AddMilliseconds(KilledAt).ToLocalTime();

        public TimeSpan GetRespawnTimeleft() => TimeSpan.FromMilliseconds(KilledAt + 7200000 - DateTimeOffset.Now.ToUnixTimeMilliseconds());

        public void ResetKill() => KilledAt = -1;

        public void SetKill(long time) => KilledAt = time;
    }
}
