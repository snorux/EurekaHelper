using System;
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
            new(-123.5271f, 501.02158f, -559.1261f),
            new(-253.41269f, 499.13086f, -518.068f),
            new(-256.50482f, 514.3237f, -903.4661f),
            new(-312.15518f, 502.0328f, -228.2824f),
            new(-360.7999f, 500f, -710.83484f),
            new(-365.2967f, 494f, -68.05076f),
            new(-572.7837f, 509.94162f, -836.63403f),
            new(-579.59796f, 504.07983f, -213.05739f),
            new(-583.8156f, 507.74808f, -468.509f),
            new(-611.1752f, 507.422f, -59.846287f),
            new(-712.05316f, 511.6147f, -583.89923f),
            new(-716.3769f, 504.4299f, -370.0967f),
            new(-798.5133f, 514.8021f, -754.0926f),
            new(-801.7904f, 505.8443f, -49.631298f),
            new(-852.24817f, 508.86594f, -353.7695f),
            new(-895.5052f, 507.7101f, -130.0577f),
            new(28.61431f, 496.32492f, -54.40704f),
            new(105.545f, 499.1306f, -590.0835f),
            new(113.322f, 495.31586f, -191.00049f),
            new(141.9147f, 502.4549f, -386.16882f),
            new(155.7582f, 513.09204f, -810.1353f),
            new(355.92392f, 508.80362f, -491.6639f),
            new(400.4016f, 495.38144f, -33.853767f),
            new(466.7553f, 506.69672f, -243.09f),
            new(471.341f, 506.4836f, -742.8676f),
            new(629.0432f, 500.25275f, -472.00372f),
            new(651.8131f, 500.32214f, -713.3341f),
            new(726.0835f, 514.47864f, -203.8521f),
            new(773.1991f, 495.9364f, -60.565678f),
            new(828.46814f, 512.196f, -428.00702f),
            new(873.14496f, 512.10065f, -739.3626f)
        };

        public EurekaHydatos(List<EurekaFate> fates) { Fates = fates; }

        public static EurekaHydatos GetTracker()
        {
            List<EurekaFate> HydatosFates = new()
            {
                new(1412, 55, 827, 515,     "I Ink, Therefore I Am", "Khalamari", "Khalamari", new Vector2(11.0f, 25.3f), "Xzomit", new Vector2(11.0f, 25.3f), EurekaWeather.None, EurekaWeather.None, EurekaElement.Water, EurekaElement.Water, false, 50),
                new(1413, 56, 827, 515,     "From Tusk till Dawn", "Stegodon", "Stegodon", new Vector2(10.1f, 17.9f), "Hydatos Primelephas", new Vector2(11.1f, 16.0f), EurekaWeather.None, EurekaWeather.None, EurekaElement.Earth, EurekaElement.Earth, false, 51),
                new(1414, 57, 827, 515,     "Bullheaded Berserker", "Molech", "Molech", new Vector2(7.0f, 21.0f), "Val Nullchu", new Vector2(7.0f, 21.0f), EurekaWeather.None, EurekaWeather.None, EurekaElement.Ice, EurekaElement.Earth, false, 52),
                new(1415, 58, 827, 515,     "Mad, Bad, and Fabulous to Know", "Piasa", "Piasa", new Vector2(7.0f, 14.0f), "Vivid Gastornis", new Vector2(7.0f, 14.0f), EurekaWeather.None, EurekaWeather.None, EurekaElement.Wind, EurekaElement.Wind, false, 53),
                new(1416, 59, 827, 515,     "Fearful Symmetry", "Frostmane", "Frostmane", new Vector2(7.9f, 26.1f), "Northern Tiger", new Vector2(6.4f, 26.5f), EurekaWeather.None, EurekaWeather.None, EurekaElement.Fire, EurekaElement.Earth, false, 54),
                new(1417, 60, 827, 515,     "Crawling Chaos", "Daphne", "Daphne", new Vector2(25.6f, 16.2f), "Dark Void Monk", new Vector2(25.6f, 16.2f), EurekaWeather.None, EurekaWeather.None, EurekaElement.Water, EurekaElement.Water, false, 55),
                new(1418, 61, 827, 515,     "Duty-free", "King Goldemar", "Golde", new Vector2(28.9f, 23.6f), "Hydatos Wraith", new Vector2(28.0f, 23.0f), EurekaWeather.None, EurekaWeather.None, EurekaElement.Lightning, EurekaElement.Lightning, true, 56),
                new(1419, 62, 827, 515,     "Leukwarm Reception", "Leuke", "Leuke", new Vector2(37.0f, 26.0f), "Tigerhawk", new Vector2(37.2f, 27.8f), EurekaWeather.None, EurekaWeather.None, EurekaElement.Earth, EurekaElement.Wind, false, 57),
                new(1420, 63, 827, 515,     "Robber Barong", "Barong", "Barong", new Vector2(32.0f, 24.0f), "Laboratory Lion", new Vector2(34.6f, 24.9f), EurekaWeather.None, EurekaWeather.None, EurekaElement.Fire, EurekaElement.Earth, false, 58),
                new(1421, 64, 827, 515,     "Stone-cold Killer", "Ceto", "Ceto", new Vector2(36.4f, 13.4f), "Hydatos Delphyne", new Vector2(36.4f, 13.4f), EurekaWeather.None, EurekaWeather.None, EurekaElement.Water, EurekaElement.Fire, false, 59),
                new(1423, 65, 827, 515,     "Crystalline Provenance", "Provenance Watcher", "PW", new Vector2(32.7f, 19.6f), "Crystal Claw", new Vector2(32.5f, 21.6f), EurekaWeather.None, EurekaWeather.None, EurekaElement.Fire, EurekaElement.Fire, false, 60),
                new(1424, null, 827, 515,   "I Don't Want to Believe", "Ovni", "Ovni", new Vector2(27.0f, 29.0f), null, Vector2.Zero, EurekaWeather.None, EurekaWeather.None, EurekaElement.Unknown, EurekaElement.Unknown, false, 60, false),
                new(1422, null, 827, 515,   "The Baldesion Arsenal: Expedition Support", "Tristitia", "Support Fate", new Vector2(18.7f, 28.4f), null, Vector2.Zero, EurekaWeather.None, EurekaWeather.None, EurekaElement.Unknown, EurekaElement.Unknown, false, 60, false),
                new(1425, null, 827, 515,   "Drink Me", "Bunny Fate 1", "Bunny Fate 1", new Vector2(14.0f, 21.5f), null, Vector2.Zero, EurekaWeather.None, EurekaWeather.None, EurekaElement.Unknown, EurekaElement.Unknown, false, 50, false, true),
            };

            Utils.GetFatePositionFromLgb(827, HydatosFates);

            return new EurekaHydatos(HydatosFates);
        }

        public List<EurekaFate> GetFates() => Fates;

        public static List<EurekaWeather> GetZoneWeathers() => Weathers.Select(x => x.Item2).ToList();

        public (EurekaWeather Weather, TimeSpan Timeleft) GetCurrentWeatherInfo() => EorzeaWeather.GetCurrentWeatherInfo(Weathers);

        public static List<DateTime> GetWeatherForecast(EurekaWeather targetWeather, int count) =>
            EorzeaWeather.GetCountWeatherForecasts(targetWeather, count, Weathers);

        public List<(EurekaWeather Weather, TimeSpan Time)> GetAllNextWeatherTime() => EorzeaWeather.GetAllWeathers(Weathers);

        public static (DateTime Start, DateTime End) GetWeatherUptime(EurekaWeather targetWeather, DateTime start)
            => EorzeaWeather.GetWeatherUptime(targetWeather, Weathers, start);

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
