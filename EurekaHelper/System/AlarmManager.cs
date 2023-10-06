using Dalamud.Game;
using Dalamud.Game.ClientState.Conditions;
using EurekaHelper.XIV;
using EurekaHelper.XIV.Zones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Dalamud.Plugin.Services;

namespace EurekaHelper.System
{
    // Badly inspired from:
    // https://github.com/Ottermandias/GatherBuddy/blob/main/GatherBuddy/Alarms/AlarmManager.cs
    public class AlarmManager : IDisposable
    {
        public List<(EurekaAlarm, DateTime)> ActiveAlarms { get; init; } = new();

        private bool Dirty = true;

        public AlarmManager()
        {
            GetActiveAlarms();
            DalamudApi.Framework.Update += OnUpdate;
            DalamudApi.ClientState.Login += OnLogin;
        }

        public void OnLogin()
            => GetActiveAlarms();

        public void OnUpdate(IFramework framework)
        {
            if (ActiveAlarms.Count == 0)
                return;

            if (DalamudApi.Condition[ConditionFlag.BetweenAreas] || DalamudApi.Condition[ConditionFlag.BetweenAreas51])
                return;

            if (DalamudApi.ClientState.LocalPlayer == null)
                return;

            SortActiveAlarms();
            var (alarm, dateTime) = ActiveAlarms[0];

            var dt = DateTime.Now;
            if (dateTime >= dt)
                return;

            var uptime = GetUptime(alarm);
            alarm.Announce(uptime);
            SoundManager.PlaySoundEffect(alarm.SoundEffect);

            var newUptime = uptime;
            var newStart = DateTime.MinValue;
            while (newStart <= dt)
            {
                newUptime = GetUptime(alarm, newUptime.End + TimeSpan.FromMinutes(1));
                newStart = newUptime.Start.AddMinutes(-alarm.MinutesOffset);
            }

            ActiveAlarms[0] = (alarm, newStart);
            Dirty = true;
        }

        public void AddAlarm(EurekaAlarm alarm)
        {
            alarm.ID = GenerateAlarmCode();
            EurekaHelper.Config.Alarms.Add(alarm);
            EurekaHelper.Config.Save();
        }

        public void DeleteAlarm(EurekaAlarm alarm, bool deleteAll = false)
        {
            if (deleteAll)
            {
                EurekaHelper.Config.Alarms.Clear();
                EurekaHelper.Config.Save();

                ActiveAlarms.Clear();
                return;
            }

            EurekaHelper.Config.Alarms.RemoveAll(x => x.ID == alarm.ID);
            EurekaHelper.Config.Save();

            ActiveAlarms.RemoveAll(x => x.Item1.ID == alarm.ID);
        }

        public void UpdateAlarm(EurekaAlarm alarm)
        {
            EurekaHelper.Config.Save();

            var index = ActiveAlarms.FindIndex(x => x.Item1.ID == alarm.ID);
            if (index < 0)
                return;

            var existingAlarm = ActiveAlarms[index];

            RemoveActiveAlarm(existingAlarm.Item1);
            if (alarm.Enabled)
                AddActiveAlarm(existingAlarm.Item1);
        }

        public void ToggleAlarm(EurekaAlarm alarm)
        {
            if (alarm.Enabled)
            {
                alarm.Enabled = false;
                ActiveAlarms.RemoveAll(x => x.Item1.ID == alarm.ID);
            }
            else
            {
                alarm.Enabled = true;
                AddActiveAlarm(alarm);
            }

            EurekaHelper.Config.Save();
        }

        public void SetAlarmPrintMessage(EurekaAlarm alarm, bool printMessage)
        {
            alarm.PrintMessage = printMessage;
            EurekaHelper.Config.Save();
        }

        public void SetAlarmShowToast(EurekaAlarm alarm, bool showToast)
        {
            alarm.ShowToast = showToast;
            EurekaHelper.Config.Save();
        }

        private string GenerateAlarmCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            const int size = 8;

            byte[] data = new byte[4 * size];
            using var crypto = RandomNumberGenerator.Create();
            crypto.GetBytes(data);

            StringBuilder result = new(size);
            for (int i = 0; i < size; i++)
            {
                var rnd = BitConverter.ToUInt32(data, i * 4);
                var idx = rnd % chars.Length;

                result.Append(chars[(int)idx]);
            }

            return result.ToString();
        }

        private void AddActiveAlarm(EurekaAlarm alarm)
        {
            if (ActiveAlarms.Exists(x => x.Item1.ID == alarm.ID))
                return;

            var (Start, End) = GetUptime(alarm);
            var startTime = Start.AddMinutes(-alarm.MinutesOffset);
            ActiveAlarms.Add((alarm, startTime));
            Dirty = true;
        }

        private void RemoveActiveAlarm(EurekaAlarm alarm)
        {
            if (!ActiveAlarms.Exists(x => x.Item1.ID == alarm.ID))
                return;

            ActiveAlarms.RemoveAll(x => x.Item1.ID == alarm.ID);
        }

        public static (DateTime Start, DateTime End) GetUptime(EurekaAlarm alarm)
            => GetUptime(alarm, DateTime.Now);

        public static (DateTime Start, DateTime End) GetUptime(EurekaAlarm alarm, DateTime start)
        {
            if (alarm.Type == AlarmType.Time)
            {
                return alarm.TimeType switch
                {
                    TimeType.Night => EorzeaTime.GetTimeUptime(start, TimeType.Night),
                    TimeType.Day => EorzeaTime.GetTimeUptime(start, TimeType.Day),
                    _ => throw new NotImplementedException()
                };
            }
            else
            {
                return alarm.ZoneId switch
                {
                    732 => EurekaAnemos.GetWeatherUptime(alarm.Weather, start),
                    763 => EurekaPagos.GetWeatherUptime(alarm.Weather, start),
                    795 => EurekaPyros.GetWeatherUptime(alarm.Weather, start),
                    827 => EurekaHydatos.GetWeatherUptime(alarm.Weather, start),
                    _ => throw new NotImplementedException()
                };
            }
        }

        private void GetActiveAlarms()
        {
            ActiveAlarms.Clear();

            foreach (var alarm in EurekaHelper.Config.Alarms.Where(x => x.Enabled))
                AddActiveAlarm(alarm);
        }

        private void SortActiveAlarms()
        {
            if (!Dirty)
                return;

            ActiveAlarms.Sort((x, y) => x.Item2.CompareTo(y.Item2));
            Dirty = false;
        }

        public void Dispose()
        {
            DalamudApi.Framework.Update -= OnUpdate;
            DalamudApi.ClientState.Login -= OnLogin;
        }
    }
}
