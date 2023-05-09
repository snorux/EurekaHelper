using System;

namespace EurekaHelper.XIV
{
    public class EorzeaTime
    {
        public const long EIGHT_HOURS = 8 * 175 * 1000;
        public const double MULTIPLIER = 144D / 7D;
        public static readonly DateTime Zero = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public DateTime EorzeaDateTime { get; set; }

        public EorzeaTime(DateTime dateTime)
        {
            EorzeaDateTime = dateTime;
        }

        public static EorzeaTime Now => ToEorzeaTime(DateTime.Now);

        public static EorzeaTime ToEorzeaTime(DateTime dateTime)
        {
            long epochTicks = dateTime.ToUniversalTime().Ticks - Zero.Ticks;
            long eorzeaTicks = (long)Math.Round(epochTicks * MULTIPLIER);
            return new EorzeaTime(new DateTime(eorzeaTicks));
        }

        public DateTime ToEarthTime()
        {
            var epochTicks = (long)Math.Round(EorzeaDateTime.Ticks / MULTIPLIER);
            var earthTicks = epochTicks + Zero.Ticks;
            return new DateTime(earthTicks, DateTimeKind.Utc);
        }

        public DateTime ToLocalEarthTime() => ToEarthTime().ToLocalTime();

        public static DateTime GetNearestEarthInterval(DateTime dateTime)
        {
            long epochTicks = new DateTimeOffset(dateTime).ToUnixTimeMilliseconds();
            var result = epochTicks - epochTicks % EIGHT_HOURS;
            return Zero + TimeSpan.FromSeconds(result / 1000);
        }

        public TimeSpan TimeUntilDay()
        {
            DateTime nextNight;
            if (EorzeaDateTime.Hour < 6)
                nextNight = EorzeaDateTime.Date + new TimeSpan(6, 0, 0);
            else
                nextNight = EorzeaDateTime.Date + new TimeSpan(1, 6, 0, 0);

            return TimeSpan.FromTicks(Convert.ToInt64((nextNight - EorzeaDateTime).Ticks * 7D / 144D));
        }

        public TimeSpan TimeUntilNight()
        {
            DateTime nextDay;
            if (EorzeaDateTime.Hour < 19)
                nextDay = EorzeaDateTime.Date + new TimeSpan(19, 0, 0);
            else
                nextDay = EorzeaDateTime.Date + new TimeSpan(1, 19, 0, 0);

            return TimeSpan.FromTicks(Convert.ToInt64((nextDay - EorzeaDateTime).Ticks * 7D / 144D));
        }

        public static (DateTime Start, DateTime End) GetTimeUptime(DateTime start, TimeType timeType)
        {
            var et = ToEorzeaTime(start);

            if (timeType == TimeType.Night)
            {
                if (et.EorzeaDateTime.Hour >= 19 || et.EorzeaDateTime.Hour < 6)
                {
                    var sevenPmToday = et.EorzeaDateTime.Date.AddDays(et.EorzeaDateTime.Hour < 6 ? -1 : 0).AddHours(19);
                    var ts = TimeSpan.FromTicks(Convert.ToInt64((et.EorzeaDateTime - sevenPmToday).Ticks * 7D / 144D));

                    return (start - ts, start + et.TimeUntilDay());
                }
                else
                {
                    return NextNightTime(start);
                }
            }
            else
            {
                if (et.EorzeaDateTime.Hour >= 6 && et.EorzeaDateTime.Hour < 18)
                {
                    var sixAmToday = et.EorzeaDateTime.Date.AddHours(6);
                    var ts = TimeSpan.FromTicks(Convert.ToInt64((et.EorzeaDateTime - sixAmToday).Ticks * 7D / 144D));

                    return (start - ts, start + et.TimeUntilNight());
                }
                else
                {
                    return NextDayTime(start);
                }
            }
        }

        public static (DateTime Start, DateTime End) NextDayTime()
            => NextDayTime(DateTime.Now);

        public static (DateTime Start, DateTime End) NextDayTime(DateTime start)
        {
            var nextDayTime = start + ToEorzeaTime(start).TimeUntilDay();
            var nextNightTime = ToEorzeaTime(nextDayTime).TimeUntilNight();

            return (nextDayTime, nextDayTime + nextNightTime);
        }

        public static (DateTime Start, DateTime End) NextNightTime()
            => NextNightTime(DateTime.Now);

        public static (DateTime Start, DateTime End) NextNightTime(DateTime start)
        {
            var nextNightTime = start + ToEorzeaTime(start).TimeUntilNight();
            var nextDayTime = ToEorzeaTime(nextNightTime).TimeUntilDay();

            return (nextNightTime, nextNightTime + nextDayTime);
        }
    }
}
