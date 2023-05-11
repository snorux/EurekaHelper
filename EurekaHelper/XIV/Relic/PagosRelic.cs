using System.Collections.Generic;

namespace EurekaHelper.XIV.Relic
{
    public class PagosRelic
    {
        #region Pagos Weapon
        private static readonly List<RelicItem> PagosWeaponRelic = new()
        {
            new RelicItem(22925),
            new RelicItem(22940),
            new RelicItem(22927),
            new RelicItem(22931),
            new RelicItem(22928),
            new RelicItem(22926),
            new RelicItem(22938),
            new RelicItem(22930),
            new RelicItem(22929),
            new RelicItem(22932),
            new RelicItem(22934),
            new RelicItem(22935),
            new RelicItem(22939),
            new RelicItem(22933),
            new RelicItem(22936),
            new RelicItem(22937),
        };

        private static readonly List<CompletionRequirement> PagosWeaponRelicCompletionRequirements = new()
        {
            new CompletionRequirement(23309, 5)
        };
        #endregion

        #region Pagos +1 Weapon
        private static readonly List<RelicItem> PagosPlusOneWeaponRelic = new()
        {
            new RelicItem(22941),
            new RelicItem(22956),
            new RelicItem(22943),
            new RelicItem(22947),
            new RelicItem(22944),
            new RelicItem(22942),
            new RelicItem(22954),
            new RelicItem(22946),
            new RelicItem(22945),
            new RelicItem(22948),
            new RelicItem(22950),
            new RelicItem(22951),
            new RelicItem(22955),
            new RelicItem(22949),
            new RelicItem(22952),
            new RelicItem(22953),
        };

        private static readonly List<CompletionRequirement> PagosPlusOneRelicCompletionRequirements = new()
        {
            new CompletionRequirement(23309, 10),
            new CompletionRequirement(22976, 500)
        };
        #endregion

        #region Elemental Weapon
        private static readonly List<RelicItem> ElementalWeaponRelic = new()
        {
            new RelicItem(22957),
            new RelicItem(22972),
            new RelicItem(22959),
            new RelicItem(22963),
            new RelicItem(22960),
            new RelicItem(22958),
            new RelicItem(22970),
            new RelicItem(22962),
            new RelicItem(22961),
            new RelicItem(22964),
            new RelicItem(22966),
            new RelicItem(22967),
            new RelicItem(22971),
            new RelicItem(22965),
            new RelicItem(22968),
            new RelicItem(22969),
        };

        private static readonly List<CompletionRequirement> ElementalRelicCompletionRequirements = new()
        {
            new CompletionRequirement(23309, 16),
            new CompletionRequirement(22975, 5)
        };
        #endregion

        public static readonly Dictionary<string, EurekaRelic> AnemosRelicStages = new()
        {
            ["Pagos Weapon"] = new EurekaRelic(PagosWeaponRelic, PagosWeaponRelicCompletionRequirements),
            ["Pagos +1"] = new EurekaRelic(PagosPlusOneWeaponRelic, PagosPlusOneRelicCompletionRequirements),
            ["Elemental"] = new EurekaRelic(ElementalWeaponRelic, ElementalRelicCompletionRequirements)
        };
    }
}
