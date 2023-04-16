using System.Collections.Generic;

namespace EurekaHelper.XIV.Relic
{
    public class PyrosRelic
    {
        #region Elemental +1 Weapon
        private static readonly List<RelicItem> ElementalPlusOneRelic = new()
        {
            new RelicItem(24039),
            new RelicItem(24054),
            new RelicItem(24041),
            new RelicItem(24045),
            new RelicItem(24042),
            new RelicItem(24040),
            new RelicItem(24052),
            new RelicItem(24044),
            new RelicItem(24043),
            new RelicItem(24046),
            new RelicItem(24048),
            new RelicItem(24049),
            new RelicItem(24053),
            new RelicItem(24047),
            new RelicItem(24050),
            new RelicItem(24051),
        };

        private static readonly List<CompletionRequirement> ElementalPlusOneRelicCompletionRequirements = new()
        {
            new CompletionRequirement(24124, 150),
            new CompletionRequirement(0, 10, "Logos Actions Unlocked")
        };
        #endregion

        #region Elemental +2 Weapon
        private static readonly List<RelicItem> ElementalPlusTwoRelic = new()
        {
            new RelicItem(24055),
            new RelicItem(24070),
            new RelicItem(24057),
            new RelicItem(24061),
            new RelicItem(24058),
            new RelicItem(24056),
            new RelicItem(24068),
            new RelicItem(24060),
            new RelicItem(24059),
            new RelicItem(24062),
            new RelicItem(24064),
            new RelicItem(24065),
            new RelicItem(24069),
            new RelicItem(24063),
            new RelicItem(24066),
            new RelicItem(24067),
        };

        private static readonly List<CompletionRequirement> ElementalPlusTwoRelicCompletionRequirements = new()
        {
            new CompletionRequirement(24124, 200),
            new CompletionRequirement(0, 20, "Logos Actions Unlocked")
        };
        #endregion

        #region Pyros Weapon
        private static readonly List<RelicItem> PyrosWeaponRelic = new()
        {
            new RelicItem(24071),
            new RelicItem(24086),
            new RelicItem(24073),
            new RelicItem(24077),
            new RelicItem(24074),
            new RelicItem(24072),
            new RelicItem(24084),
            new RelicItem(24076),
            new RelicItem(24075),
            new RelicItem(24078),
            new RelicItem(24080),
            new RelicItem(24081),
            new RelicItem(24085),
            new RelicItem(24079),
            new RelicItem(24082),
            new RelicItem(24083),
        };

        private static readonly List<CompletionRequirement> PyrosWeaponRelicCompletionRequirements = new()
        {
            new CompletionRequirement(24124, 300),
            new CompletionRequirement(24123, 5),
            new CompletionRequirement(0, 30, "Logos Actions Unlocked")
        };
        #endregion

        #region Elemental Armor
        private static readonly List<RelicItem> ElementalArmorRelic = new()
        {
            new RelicItem(24087),
            new RelicItem(24092),
            new RelicItem(24097),
            new RelicItem(24107),
            new RelicItem(24102),
            new RelicItem(24117),
            new RelicItem(24112),
            new RelicItem(24088),
            new RelicItem(24093),
            new RelicItem(24098),
            new RelicItem(24108),
            new RelicItem(24103),
            new RelicItem(24118),
            new RelicItem(24113),
            new RelicItem(24089),
            new RelicItem(24094),
            new RelicItem(24099),
            new RelicItem(24109),
            new RelicItem(24104),
            new RelicItem(24119),
            new RelicItem(24114),
            new RelicItem(24090),
            new RelicItem(24095),
            new RelicItem(24100),
            new RelicItem(24110),
            new RelicItem(24105),
            new RelicItem(24120),
            new RelicItem(24115),
            new RelicItem(24091),
            new RelicItem(24096),
            new RelicItem(24101),
            new RelicItem(24111),
            new RelicItem(24106),
            new RelicItem(24121),
            new RelicItem(24116),
        };

        private static readonly List<CompletionRequirement> ElementalArmorRelicCompletionRequirements = new()
        {
            new CompletionRequirement(24124, 40),
            new CompletionRequirement(0, 50, "Logos Actions Unlocked")
        };
        #endregion

        public static readonly Dictionary<string, EurekaRelic> PyrosRelicStages = new()
        {
            ["Elemental +1"] = new EurekaRelic(ElementalPlusOneRelic, ElementalPlusOneRelicCompletionRequirements),
            ["Elemental +2"] = new EurekaRelic(ElementalPlusTwoRelic, ElementalPlusTwoRelicCompletionRequirements),
            ["Pyros Weapon"] = new EurekaRelic(PyrosWeaponRelic, PyrosWeaponRelicCompletionRequirements),
            ["Elemental Armor"] = new EurekaRelic(ElementalArmorRelic, ElementalArmorRelicCompletionRequirements),
        };
    }
}
