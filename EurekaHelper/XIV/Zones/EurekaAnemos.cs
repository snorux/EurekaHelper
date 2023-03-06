using System;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;

namespace EurekaHelper.XIV.Zones
{
    public class EurekaAnemos : IEurekaTracker
    {
        private readonly List<EurekaFate> Fates = new();

        private static readonly (int, EurekaWeather)[] Weathers = new (int, EurekaWeather)[]
        {
            (30, EurekaWeather.FairSkies),
            (60, EurekaWeather.Gales),
            (90, EurekaWeather.Showers),
            (100, EurekaWeather.Snow)
        };

        public EurekaAnemos(List<EurekaFate> fates) { Fates = fates; }

        public static EurekaAnemos GetTracker()
        {
            List<EurekaFate> AnemosFates = new()
            {
                new(1332, 1, 732, 414,  "Unsafety Dance", "Sabotender Corrido", "Sabo", new Vector2(14.0f, 22.0f), "Flowering Sabotender", EurekaWeather.None, EurekaWeather.None, EurekaElement.Wind, EurekaElement.Wind, false),
                new(1348, 2, 732, 414,  "The Shadow over Anemos", "The Lord of Anemos", "Lord", new Vector2(30.0f, 27.0f), "Sea Bishop", EurekaWeather.None, EurekaWeather.None, EurekaElement.Water, EurekaElement.Water, false),
                new(1333, 3, 732, 414,  "Teles House", "Teles", "Teles", new Vector2(25.9f, 27.0f), "Anemos Harpeia", EurekaWeather.None, EurekaWeather.None, EurekaElement.Wind, EurekaElement.Wind, false),
                new(1328, 4, 732, 414,  "The Swarm Never Sets", "The Emperor of Anemos", "Emperor", new Vector2(17.0f, 22.0f), "Darner", EurekaWeather.None, EurekaWeather.None, EurekaElement.Wind, EurekaElement.Wind, false),
                new(1344, 5, 732, 414,  "One Missed Callisto", "Callisto", "Callisto", new Vector2(26.0f, 22.0f), "Val Bear", EurekaWeather.None, EurekaWeather.None, EurekaElement.Earth, EurekaElement.Earth, false),
                new(1347, 6, 732, 414,  "By Numbers", "Number", "Number", new Vector2(24.0f, 23.0f), "Pneumaflayer", EurekaWeather.None, EurekaWeather.None, EurekaElement.Lightning, EurekaElement.Lightning, false),
                new(1345, 7, 732, 414,  "Disinherit the Wind", "Jahannam", "Jaha", new Vector2(18.0f, 19.0f), "Typhoon Sprite", EurekaWeather.None, EurekaWeather.Gales, EurekaElement.Wind, EurekaElement.Wind, false),
                new(1334, 8, 732, 414,  "Prove Your Amemettle", "Amemet", "Amemet", new Vector2(15.0f, 16.0f), "Abraxas", EurekaWeather.None, EurekaWeather.None, EurekaElement.Fire, EurekaElement.Fire, false),
                new(1335, 9, 732, 414,  "Caym What May", "Caym", "Caym", new Vector2(14.0f, 13.0f), "Stalker Ziz", EurekaWeather.None, EurekaWeather.None, EurekaElement.Ice, EurekaElement.Ice, false),
                new(1336, 10, 732, 414, "The Killing of a Sacred Bombardier", "Bombadeel", "Bomba", new Vector2(28.0f, 20.0f), "Traveling Gourmand", EurekaWeather.None, EurekaWeather.None, EurekaElement.Earth, EurekaElement.Earth, true),
                new(1339, 11, 732, 414, "Short Serket 2", "Serket", "Serket", new Vector2(25.0f, 18.0f), "Khor Claw", EurekaWeather.None, EurekaWeather.None, EurekaElement.Earth, EurekaElement.Earth, false),
                new(1346, 12, 732, 414, "Don't Judge Me, Morbol", "Judgemental Julika", "Julika", new Vector2(22.0f, 16.0f), "Henbane", EurekaWeather.None, EurekaWeather.None, EurekaElement.Ice, EurekaElement.Ice, false),
                new(1343, 13, 732, 414, "When You Ride Alone", "The White Rider", "Rider", new Vector2(20.0f, 13.0f), "Duskfall Dullahan", EurekaWeather.None, EurekaWeather.None, EurekaElement.Lightning, EurekaElement.Lightning, true),
                new(1337, 14, 732, 414, "Sing, Muse", "Polyphemus", "Poly", new Vector2(26.0f, 14.0f), "Monoeye", EurekaWeather.None, EurekaWeather.None, EurekaElement.Ice, EurekaElement.Ice, false),
                new(1342, 15, 732, 414, "Simurghasbord", "Simurgh's Strider", "Strider", new Vector2(29.0f, 13.0f), "Old World Zu", EurekaWeather.None, EurekaWeather.None, EurekaElement.Wind, EurekaElement.Fire, false),
                new(1341, 16, 732, 414, "To the Mat", "King Hazmat", "Hazmat", new Vector2(35.0f, 18.0f), "Anemos Anala", EurekaWeather.None, EurekaWeather.None, EurekaElement.Fire, EurekaElement.Fire, false),
                new(1331, 17, 732, 414, "Wine and Honey", "Fafnir", "Fafnir", new Vector2(36.0f, 22.0f), "Fossil Dragon", EurekaWeather.None, EurekaWeather.None, EurekaElement.Fire, EurekaElement.Fire, true),
                new(1340, 18, 732, 414, "I Amarok", "Amarok", "Amarok", new Vector2(8.0f, 18.0f), "Voidscale", EurekaWeather.None, EurekaWeather.None, EurekaElement.Ice, EurekaElement.Ice, false),
                new(1338, 19, 732, 414, "Drama Lamashtu", "Lamashtu", "Lamashtu", new Vector2(8.0f, 23.0f), "Val Specter", EurekaWeather.None, EurekaWeather.None, EurekaElement.Wind, EurekaElement.Wind, true),
                new(1329, 20, 732, 414, "Wail in the Willows", "Pazuzu", "Paz", new Vector2(7.0f, 22.0f), "Shadow Wraith", EurekaWeather.Gales, EurekaWeather.None, EurekaElement.Wind, EurekaElement.Fire, true)
            };

            return new EurekaAnemos(AnemosFates);
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
