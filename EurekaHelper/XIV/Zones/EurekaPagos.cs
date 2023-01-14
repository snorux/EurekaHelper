using System;
using System.Collections.Generic;
using static FFXIVClientStructs.FFXIV.Client.Game.Character.Character;
using System.Numerics;
using System.Linq;

namespace EurekaHelper.XIV.Zones
{
    public class EurekaPagos : IEurekaTracker
    {
        private readonly List<EurekaFate> Fates = new();

        private static readonly (int, EurekaWeather)[] Weathers = new (int, EurekaWeather)[]
        {
            (10, EurekaWeather.FairSkies),
            (28, EurekaWeather.Fog),
            (46, EurekaWeather.HeatWaves),
            (64, EurekaWeather.Snow),
            (82, EurekaWeather.Thunder),
            (100, EurekaWeather.Blizzards)
        };

        public EurekaPagos(List<EurekaFate> fates) { Fates = fates; }

        public static EurekaPagos GetTracker()
        {
            List<EurekaFate> PagosFates = new()
            {
                new(1351, 21, 763, 467,     "Eternity", "The Snow Queen", new Vector2(21.7f, 26.3f), "Yukinko", EurekaWeather.None, EurekaWeather.None, EurekaElement.Ice, EurekaElement.Ice, false),
                new(1369, 22, 763, 467,     "Cairn Blight 451", "Taxim", new Vector2(25.0f, 28.0f), "Demon of the Incunable", EurekaWeather.None, EurekaWeather.None, EurekaElement.Earth, EurekaElement.Wind, true),
                new(1353, 23, 763, 467,     "Ash the Magic Dragon", "Ash Dragon", new Vector2(29.0f, 30.0f), "Blood Demon", EurekaWeather.None, EurekaWeather.None, EurekaElement.Fire, EurekaElement.Fire, false),
                new(1354, 24, 763, 467,     "Conqueror Worm", "Glavoid", new Vector2(32.0f, 26.0f), "Val Worm", EurekaWeather.None, EurekaWeather.None, EurekaElement.Earth, EurekaElement.Earth, false),
                new(1355, 25, 763, 467,     "Melting Point", "Anapos", new Vector2(32.9f, 21.5f), "Snowmelt Sprite", EurekaWeather.None, EurekaWeather.Fog, EurekaElement.Water, EurekaElement.Water, false),
                new(1366, 26, 763, 467,     "The Wobbler in Darkness", "Hakutaku", new Vector2(29.0f, 22.0f), "Blubber Eyes", EurekaWeather.None, EurekaWeather.None, EurekaElement.Fire, EurekaElement.Fire, false),
                new(1357, 27, 763, 467,     "Does It Have to Be a Snowman", "King Igloo", new Vector2(17.0f, 16.0f), "Huwasi", EurekaWeather.None, EurekaWeather.None, EurekaElement.Ice, EurekaElement.Ice, false),
                new(1356, 28, 763, 467,     "Disorder in the Court", "Asag", new Vector2(10.0f, 10.0f), "Wandering Opken", EurekaWeather.None, EurekaWeather.None, EurekaElement.Lightning, EurekaElement.Lightning, false),
                new(1352, 29, 763, 467,     "Cows for Concern", "Surabhi", new Vector2(10.0f, 20.0f), "Pagos Billygoat", EurekaWeather.None, EurekaWeather.None, EurekaElement.Earth, EurekaElement.Earth, false),
                new(1360, 30, 763, 467,     "Morte Arthro", "King Arthro", new Vector2(8.7f, 15.4f), "Val Snipper", EurekaWeather.Fog, EurekaWeather.None, EurekaElement.Water, EurekaElement.Water, false),
                new(1358, 31, 763, 467,     "Brothers", "Mindertaur/Eldertaur", new Vector2(13.9f, 18.7f), "Lab Minotaur", EurekaWeather.None, EurekaWeather.None, EurekaElement.Earth, EurekaElement.Wind, false),
                new(1361, 32, 763, 467,     "Apocalypse Cow", "Holy Cow", new Vector2(26.0f, 16.0f), "Elder Buffalo", EurekaWeather.None, EurekaWeather.None, EurekaElement.Water, EurekaElement.Wind, false),
                new(1362, 33, 763, 467,     "Third Impact", "Hadhayosh", new Vector2(30.0f, 19.0f), "Lesser Void Dragon", EurekaWeather.Thunder, EurekaWeather.None, EurekaElement.Lightning, EurekaElement.Lightning, false),
                new(1362, 34, 763, 467,     "Eye of Horus", "Horus", new Vector2(25.0f, 19.0f), "Void Vouivre", EurekaWeather.HeatWaves, EurekaWeather.None, EurekaElement.Fire, EurekaElement.Fire, false),
                new(1363, 35, 763, 467,     "Eye Scream for Ice Cream", "Arch Angra Mainyu", new Vector2(24.0f, 25.0f), "Gawper", EurekaWeather.None, EurekaWeather.None, EurekaElement.Wind, EurekaElement.Wind, false),
                new(1365, 36, 763, 467,     "Cassie and the Copycats", "Copycat Cassie", new Vector2(22.3f, 14.3f), "Ameretat", EurekaWeather.Blizzards, EurekaWeather.None, EurekaElement.Ice, EurekaElement.Ice, false),
                new(1362, 37, 763, 467,     "Louhi on Ice", "Louhi", new Vector2(36.0f, 19.0f), "Val Corpse", EurekaWeather.None, EurekaWeather.None, EurekaElement.Ice, EurekaElement.Ice, true),
                new(1367, null, 763, 467,   "Down the Rabbit Hole", "Bunny Fate 1", new Vector2(18.0f, 27.5f), null, EurekaWeather.None, EurekaWeather.None, EurekaElement.Unknown, EurekaElement.Unknown, false, false, true),
                new(1368, null, 763, 467,   "Curiouser and Curiouser", "Bunny Fate 2", new Vector2(20.5f, 21.0f), null, EurekaWeather.None, EurekaWeather.None, EurekaElement.Unknown, EurekaElement.Unknown, false, false, true)
            };

            return new EurekaPagos(PagosFates);
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
