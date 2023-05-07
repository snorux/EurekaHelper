using EurekaHelper.System;
using System;

namespace EurekaHelper.XIV
{
    public enum AlarmType
    {
        Weather,
        Time
    }

    public enum TimeType
    {
        Day,
        Night
    }

    public class EurekaAlarm
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public SoundEffect SoundEffect { get; set; }
        public AlarmType Type { get; set; }
        public TimeType TimeType { get; set; }
        public EurekaWeather Weather { get; set; }
        public ushort ZoneId { get; set; }
        public int MinutesOffset { get; set; }
        public bool Enabled { get; set; } = false;
        public bool PrintMessage { get; set; } = false;
        public bool ShowToast { get; set; } = false;

        public void Announce(DateTime uptime)
        {
            if (!DalamudApi.Condition.Any())
                return;

            if (PrintMessage) { }
                // do something

            if (ShowToast) { }
                // do something
        }
    }
}
