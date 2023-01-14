namespace EurekaHelper.XIV
{
    public enum EurekaElement
    {
        Wind,
        Water,
        Earth,
        Lightning,
        Fire,
        Ice,
        Unknown
    }

    public static class EurekaElementExtensions
    {
        public static string ToFriendlyString(this EurekaElement element)
        {
            return element switch
            {
                EurekaElement.Wind => "Wind",
                EurekaElement.Water => "Water",
                EurekaElement.Earth => "Earth",
                EurekaElement.Lightning => "Lightning",
                EurekaElement.Fire => "Fire",
                EurekaElement.Ice => "Ice",
                _ => "Unknown"
            };
        }
    }
}
