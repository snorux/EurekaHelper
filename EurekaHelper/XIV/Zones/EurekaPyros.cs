using System;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;

namespace EurekaHelper.XIV.Zones
{
    public class EurekaPyros : IEurekaTracker
    {
        private readonly List<EurekaFate> Fates = new();

        private static readonly (int, EurekaWeather)[] Weathers = new (int, EurekaWeather)[]
        {
            (10, EurekaWeather.FairSkies),
            (28, EurekaWeather.HeatWaves),
            (46, EurekaWeather.Thunder),
            (64, EurekaWeather.Blizzards),
            (82, EurekaWeather.UmbralWind),
            (100, EurekaWeather.Snow)
        };

        public EurekaPyros(List<EurekaFate> fates) { Fates = fates; }

        public static EurekaPyros GetTracker()
        {
            List<EurekaFate> PyrosFates = new()
            {
                new(1388, 38, 795, 484,     "Medias Res", "Leucosia", "Leucosia", new Vector2(27.0f, 26.0f), "Pyros Bhoot", EurekaWeather.None, EurekaWeather.None, EurekaElement.Water, EurekaElement.Ice, true),
                new(1389, 39, 795, 484,     "High Voltage","Flauros", "Flauros", new Vector2(29.0f, 29.0f), "Thunderstorm Sprite", EurekaWeather.None, EurekaWeather.Thunder, EurekaElement.Lightning, EurekaElement.Lightning, false),
                new(1390, 40, 795, 484,     "On the Nonexistent", "The Sophist", "Sophist", new Vector2(32.1f, 31.5f), "Pyros Apanda", EurekaWeather.None, EurekaWeather.None, EurekaElement.Wind, EurekaElement.Earth, false),
                new(1391, 41, 795, 484,     "Creepy Doll", "Graffiacane", "Doll", new Vector2(23.0f, 37.0f), "Valking", EurekaWeather.None, EurekaWeather.None, EurekaElement.Ice, EurekaElement.Lightning, false),
                new(1392, 42, 795, 484,     "Quiet, Please", "Askalaphos", "Owl", new Vector2(19.2f, 29.2f), "Overdue Tome", EurekaWeather.UmbralWind, EurekaWeather.None, EurekaElement.Wind, EurekaElement.Earth, false),
                new(1393, 43, 795, 484,     "Up and Batym", "Grand Duke Batym", "Batym", new Vector2(18.0f, 14.0f), "Dark Troubadour", EurekaWeather.None, EurekaWeather.None, EurekaElement.Earth, EurekaElement.Earth, true),
                new(1394, 44, 795, 484,     "Rondo Aetolus", "Aetolus", "Aetolus", new Vector2(10.0f, 14.0f), "Islandhander", EurekaWeather.None, EurekaWeather.None, EurekaElement.Lightning, EurekaElement.Wind, false),
                new(1395, 45, 795, 484,     "Scorchpion King", "Lesath", "Lesath", new Vector2(13.0f, 11.0f), "Bird Eater", EurekaWeather.None, EurekaWeather.None, EurekaElement.Fire, EurekaElement.Wind, false),
                new(1396, 46, 795, 484,     "Burning Hunger", "Eldthurs", "Eldthurs", new Vector2(15.2f, 6.4f), "Eldthurs", EurekaWeather.None, EurekaWeather.None, EurekaElement.Fire, EurekaElement.Fire, false),
                new(1397, 47, 795, 484,     "Dry Iris", "Iris", "Iris", new Vector2(21.3f, 12.2f), "Northern Swallow", EurekaWeather.None, EurekaWeather.None, EurekaElement.Water, EurekaElement.Water, false),
                new(1398, 48, 795, 484,     "Thirty Whacks", "Lamebrix Strikebocks", "Lamebrix", new Vector2(21.9f, 8.4f), "Illuminati Escapee", EurekaWeather.None, EurekaWeather.None, EurekaElement.Earth, EurekaElement.Lightning, false),
                new(1399, 49, 795, 484,     "Put Up Your Dux", "Dux", "Dux", new Vector2(27.0f, 9.0f), "Matanga Castaway", EurekaWeather.Thunder, EurekaWeather.None, EurekaElement.Lightning, EurekaElement.Fire, false),
                new(1400, 50, 795, 484,     "You Do Know Jack", "Lumber Jack", "Jack", new Vector2(30.2f, 11.4f), "Pyros Treant", EurekaWeather.None, EurekaWeather.None, EurekaElement.Earth, EurekaElement.Lightning, false),
                new(1401, 51, 795, 484,     "Mister Bright-eyes", "Glaukopis", "Glaukopis", new Vector2(32.0f, 15.2f), "Val Skatene", EurekaWeather.None, EurekaWeather.None, EurekaElement.Fire, EurekaElement.Wind, false),
                new(1402, 52, 795, 484,     "Haunter of the Dark", "Ying-Yang", "YY", new Vector2(11.5f, 34.3f), "Pyros Hecteyes", EurekaWeather.None, EurekaWeather.None, EurekaElement.Water, EurekaElement.Water, false),
                new(1403, 53, 795, 484,     "Heavens' Warg", "Skoll", "Skoll", new Vector2(24.0f, 30.0f), "Pyros Shuck", EurekaWeather.Blizzards, EurekaWeather.None, EurekaElement.Ice, EurekaElement.Earth, false),
                new(1404, 54, 795, 484,     "Lost Epic", "Penthesilea", "Penny", new Vector2(35.9f, 5.9f), "Val Bloodglider", EurekaWeather.HeatWaves, EurekaWeather.None, EurekaElement.Fire, EurekaElement.Fire, false),
                new(1407, null, 795, 484,   "We're All Mad Here", "Bunny Fate 1", "Bunny Fate 1", new Vector2(24.2f, 26.3f), null, EurekaWeather.None, EurekaWeather.None, EurekaElement.Unknown, EurekaElement.Unknown, false, false, true),
                new(1408, null, 795, 484,   "Uncommon Nonsense", "Bunny Fate 2", "Bunny Fate 2", new Vector2(24.9f, 11.1f), null, EurekaWeather.None, EurekaWeather.None, EurekaElement.Unknown, EurekaElement.Unknown, false, false, true)
            };

            return new EurekaPyros(PyrosFates);
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
