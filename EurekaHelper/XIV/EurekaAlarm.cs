using Dalamud.Game.Text.SeStringHandling;
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

        public void Announce((DateTime Start, DateTime End) uptime)
        {
            if (!DalamudApi.Condition.Any())
                return;

            if (!PrintMessage && !ShowToast)
                return;

            var sb = new SeStringBuilder()
                .AddUiForeground(523)
                .Append($"[{Name}] ")
                .AddUiForegroundOff();


            if (Type == AlarmType.Weather)
                sb.AddUiForeground(508)
                    .Append($"{Weather.ToFriendlyString()} ")
                    .AddUiForegroundOff()
                    .AddUiForeground(523)
                    .Append("in ")
                    .AddUiForegroundOff()
                    .AddUiForeground(508)
                    .Append($"{Utils.GetZoneName(ZoneId)} ")
                    .AddUiForegroundOff();
            else
                sb.AddUiForeground(508)
                    .Append($"{TimeType} ")
                    .AddUiForegroundOff();

            if (uptime.Start > DateTime.Now)
            {
                var diff = uptime.Start - DateTime.Now;
                sb.AddUiForeground(523)
                    .Append("will be up in ")
                    .AddUiForegroundOff()
                    .AddUiForeground(559)
                    .Append($"{(diff.ToString(diff.Hours > 0 ? "hh'h 'mm'm 'ss's'" : "mm'm 'ss's'"))} ")
                    .AddUiForegroundOff()
                    .AddUiForeground(523)
                    .Append("@ ")
                    .AddUiForegroundOff()
                    .AddUiForeground(559)
                    .Append($"{uptime.Start:d MMM yyyy hh:mm tt}")
                    .AddUiForegroundOff();
            }
            else
            {
                var diff = uptime.End - DateTime.Now;
                sb.AddUiForeground(523)
                    .Append("will be up for the next ")
                    .AddUiForegroundOff()
                    .AddUiForeground(559)
                    .Append($"{(diff.ToString(diff.Hours > 0 ? "hh'h 'mm'm 'ss's'" : "mm'm 'ss's'"))} ")
                    .AddUiForegroundOff()
                    .AddUiForeground(523)
                    .Append("to ")
                    .AddUiForegroundOff()
                    .AddUiForeground(559)
                    .Append($"{uptime.End:d MMM yyyy hh:mm tt}")
                    .AddUiForegroundOff();

            }

            if (PrintMessage)
                EurekaHelper.PrintMessage(sb.BuiltString);
                // do something

            if (ShowToast)
                DalamudApi.ToastGui.ShowQuest(sb.BuiltString);
                // do something
        }

        public EurekaAlarm Clone()
            => new()
            {
                ID = ID,
                Name = Name,
                SoundEffect = SoundEffect,
                Type = Type,
                TimeType = TimeType,
                Weather = Weather,
                ZoneId = ZoneId,
                MinutesOffset = MinutesOffset,
                Enabled = Enabled,
                PrintMessage = PrintMessage,
                ShowToast = ShowToast,
            };
    }
}
