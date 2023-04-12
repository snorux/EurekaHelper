﻿using System;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;

namespace EurekaHelper.XIV.Zones
{
    public class EurekaHydatos : IEurekaTracker
    {
        private readonly List<EurekaFate> Fates = new();

        private static readonly (int, EurekaWeather)[] Weathers = new (int, EurekaWeather)[]
        {
            (12, EurekaWeather.FairSkies),
            (34, EurekaWeather.Showers),
            (56, EurekaWeather.Gloom),
            (78, EurekaWeather.Thunderstorms),
            (100, EurekaWeather.Snow)
        };

        public static readonly List<Vector3> ElementalPositions = new()
        {
            new(-116.452f, 501.6467f, -331.4255f),
            new(-312.15518f, 502.0328f, -228.2824f),
            new(-360.7999f, 500f, -710.83484f),
            new(-365.2967f, 494f, -68.05076f),
            new(-572.7837f, 509.94162f, -836.63403f),
            new(-579.59796f, 504.07983f, -213.05739f),
            new(-583.8156f, 507.74808f, -468.509f),
            new(-712.05316f, 511.6147f, -583.89923f),
            new(-895.5052f, 507.7101f, -130.0577f),
            new(28.61431f, 496.32492f, -54.40704f),
            new(113.322f, 495.31586f, -191.00049f),
            new(141.9147f, 502.4549f, -386.16882f),
            new(355.92392f, 508.80362f, -491.6639f),
            new(400.4016f, 495.38144f, -33.853767f),
            new(629.0432f, 500.25275f, -472.00372f),
            new(828.46814f, 512.196f, -428.00702f),
            new(867.875f, 512.1614f, -740.5326f)
        };

        public EurekaHydatos(List<EurekaFate> fates) { Fates = fates; }

        public static EurekaHydatos GetTracker()
        {
            List<EurekaFate> HydatosFates = new()
            {
                new(1412, 55, 827, 515,     "I Ink, Therefore I Am", "Khalamari", "Khalamari", new Vector2(11.0f, 25.3f), "Xzomit", new Vector2(11.0f, 25.3f), EurekaWeather.None, EurekaWeather.None, EurekaElement.Water, EurekaElement.Water, false),
                new(1413, 56, 827, 515,     "From Tusk till Dawn", "Stegodon", "Stegodon", new Vector2(10.1f, 17.9f), "Hydatos Primelephas", new Vector2(11.1f, 16.0f), EurekaWeather.None, EurekaWeather.None, EurekaElement.Earth, EurekaElement.Earth, false),
                new(1414, 57, 827, 515,     "Bullheaded Berserker", "Molech", "Molech", new Vector2(7.0f, 21.0f), "Val Nullchu", new Vector2(7.0f, 21.0f), EurekaWeather.None, EurekaWeather.None, EurekaElement.Ice, EurekaElement.Earth, false),
                new(1415, 58, 827, 515,     "Mad, Bad, and Fabulous to Know", "Piasa", "Piasa", new Vector2(7.0f, 14.0f), "Vivid Gastornis", new Vector2(7.0f, 14.0f), EurekaWeather.None, EurekaWeather.None, EurekaElement.Wind, EurekaElement.Wind, false),
                new(1416, 59, 827, 515,     "Fearful Symmetry", "Frostmane", "Frostmane", new Vector2(7.9f, 26.1f), "Northern Tiger", new Vector2(6.4f, 26.5f), EurekaWeather.None, EurekaWeather.None, EurekaElement.Fire, EurekaElement.Earth, false),
                new(1417, 60, 827, 515,     "Crawling Chaos", "Daphne", "Daphne", new Vector2(25.6f, 16.2f), "Dark Void Monk", new Vector2(25.6f, 16.2f), EurekaWeather.None, EurekaWeather.None, EurekaElement.Water, EurekaElement.Water, false),
                new(1418, 61, 827, 515,     "Duty-free", "King Goldemar", "Golde", new Vector2(28.9f, 23.6f), "Hydatos Wraith", new Vector2(28.0f, 23.0f), EurekaWeather.None, EurekaWeather.None, EurekaElement.Lightning, EurekaElement.Lightning, true),
                new(1419, 62, 827, 515,     "Leukwarm Reception", "Leuke", "Leuke", new Vector2(37.0f, 26.0f), "Tigerhawk", new Vector2(37.2f, 27.8f), EurekaWeather.None, EurekaWeather.None, EurekaElement.Earth, EurekaElement.Wind, false),
                new(1420, 63, 827, 515,     "Robber Barong", "Barong", "Barong", new Vector2(32.0f, 24.0f), "Laboratory Lion", new Vector2(34.6f, 24.9f), EurekaWeather.None, EurekaWeather.None, EurekaElement.Fire, EurekaElement.Earth, false),
                new(1421, 64, 827, 515,     "Stone-cold Killer", "Ceto", "Ceto", new Vector2(36.4f, 13.4f), "Hydatos Delphyne", new Vector2(36.4f, 13.4f), EurekaWeather.None, EurekaWeather.None, EurekaElement.Water, EurekaElement.Fire, false),
                new(1423, 65, 827, 515,     "Crystalline Provenance", "Provenance Watcher", "PW", new Vector2(32.7f, 19.6f), "Crystal Claw", new Vector2(32.5f, 21.6f), EurekaWeather.None, EurekaWeather.None, EurekaElement.Fire, EurekaElement.Fire, false),
                new(1424, null, 827, 515,   "I Don't Want to Believe", "Ovni", "Ovni", new Vector2(27.0f, 29.0f), null, Vector2.Zero, EurekaWeather.None, EurekaWeather.None, EurekaElement.Unknown, EurekaElement.Unknown, false, false),
                new(1425, null, 827, 515,   "Drink Me", "Bunny Fate 1", "Bunny Fate 1", new Vector2(14.0f, 21.5f), null, Vector2.Zero, EurekaWeather.None, EurekaWeather.None, EurekaElement.Unknown, EurekaElement.Unknown, false, false, true),
            };

            Utils.GetFatePositionFromLgb(827, HydatosFates);

            return new EurekaHydatos(HydatosFates);
        }

        public List<EurekaFate> GetFates() => Fates;

        public (EurekaWeather Weather, TimeSpan Timeleft) GetCurrentWeatherInfo() => EorzeaWeather.GetCurrentWeatherInfo(Weathers);

        public static List<DateTime> GetWeatherForecast(EurekaWeather targetWeather, int count)
        {
            var timeNow = EorzeaTime.GetNearestEarthInterval(DateTime.Now);
            int counter = 0;

            List<DateTime> result = new();
            do
            {
                int chance = EorzeaWeather.CalculateTarget(timeNow);
                EurekaWeather weather = EorzeaWeather.Forecast(Weathers, chance);

                if (weather == targetWeather)
                {
                    result.Add(timeNow.ToLocalTime());
                    counter++;
                }

                timeNow += TimeSpan.FromMilliseconds(EorzeaTime.EIGHT_HOURS);
            }
            while (counter < count);

            return result;
        }

        public List<(EurekaWeather Weather, TimeSpan Time)> GetAllNextWeatherTime() => EorzeaWeather.GetAllWeathers(Weathers);

        public void SetPopTimes(Dictionary<ushort, long> keyValuePairs)
        {
            var zoneFates = Fates.Where(x => x.IncludeInTracker).ToList();
            foreach (var fate in zoneFates)
            {
                if (keyValuePairs.ContainsKey((ushort)fate.TrackerId))
                    fate.SetKill(keyValuePairs[(ushort)fate.TrackerId]);
                else
                    fate.ResetKill();
            }
        }
    }
}
