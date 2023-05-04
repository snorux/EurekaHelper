using System.Collections.Generic;

namespace EurekaHelper.XIV.Relic
{
    public static class AnemosRelic
    {
        #region Base Weapon
        private static readonly List<RelicItem> BaseRelic = new()
        {
            new RelicItem(21942),
            new RelicItem(21957),
            new RelicItem(21944),
            new RelicItem(21948),
            new RelicItem(21945),
            new RelicItem(21943),
            new RelicItem(21955),
            new RelicItem(21947),
            new RelicItem(21946),
            new RelicItem(21949),
            new RelicItem(21951),
            new RelicItem(21952),
            new RelicItem(21956),
            new RelicItem(21950),
            new RelicItem(21953),
            new RelicItem(21954),
        };

        private static readonly List<CompletionRequirement> BaseRelicCompletionRequirements = new()
        {
            new CompletionRequirement(21801, 100)
        };
        #endregion

        #region Base +1 Weapon
        private static readonly List<RelicItem> BasePlusOneRelic = new()
        {
            new RelicItem(21958),
            new RelicItem(21973),
            new RelicItem(21960),
            new RelicItem(21964),
            new RelicItem(21961),
            new RelicItem(21959),
            new RelicItem(21971),
            new RelicItem(21963),
            new RelicItem(21962),
            new RelicItem(21965),
            new RelicItem(21967),
            new RelicItem(21968),
            new RelicItem(21972),
            new RelicItem(21966),
            new RelicItem(21969),
            new RelicItem(21970),
        };

        private static readonly List<CompletionRequirement> BasePlusOneCompletionRequirements = new()
        {
            new CompletionRequirement(21801, 400)
        };
        #endregion

        #region Base +2 Weapon
        private static readonly List<RelicItem> BasePlusTwoRelic = new()
        {
            new RelicItem(21974),
            new RelicItem(21989),
            new RelicItem(21976),
            new RelicItem(21980),
            new RelicItem(21977),
            new RelicItem(21975),
            new RelicItem(21987),
            new RelicItem(21979),
            new RelicItem(21978),
            new RelicItem(21981),
            new RelicItem(21983),
            new RelicItem(21984),
            new RelicItem(21988),
            new RelicItem(21982),
            new RelicItem(21985),
            new RelicItem(21986)
        };

        private static readonly List<CompletionRequirement> BasePlusTwoCompletionRequirements = new()
        {
            new CompletionRequirement(21801, 800)
        };
        #endregion

        #region Anemos Weapon
        private static readonly List<RelicItem> AnemosWeaponRelic = new()
        {
            new RelicItem(21990),
            new RelicItem(22005),
            new RelicItem(21992),
            new RelicItem(21996),
            new RelicItem(21993),
            new RelicItem(21991),
            new RelicItem(22003),
            new RelicItem(21995),
            new RelicItem(21994),
            new RelicItem(21997),
            new RelicItem(21999),
            new RelicItem(22000),
            new RelicItem(22004),
            new RelicItem(21998),
            new RelicItem(22001),
            new RelicItem(22002),
        };  

        private static readonly List<CompletionRequirement> AnemosWeaponCompletionRequirements = new()
        {
            new CompletionRequirement(21802, 3)
        };
        #endregion

        #region Base Armor
        private static readonly List<RelicItem> BaseArmorRelic = new()
        {
            new RelicItem(22006),
            new RelicItem(22016),
            new RelicItem(22056),
            new RelicItem(22021),
            new RelicItem(22011),
            new RelicItem(22071),
            new RelicItem(22051),
            new RelicItem(22026),
            new RelicItem(22061),
            new RelicItem(22036),
            new RelicItem(22041),
            new RelicItem(22076),
            new RelicItem(22031),
            new RelicItem(22046),
            new RelicItem(22066),
            new RelicItem(22007),
            new RelicItem(22017),
            new RelicItem(22057),
            new RelicItem(22022),
            new RelicItem(22012),
            new RelicItem(22072),
            new RelicItem(22052),
            new RelicItem(22027),
            new RelicItem(22062),
            new RelicItem(22037),
            new RelicItem(22042),
            new RelicItem(22077),
            new RelicItem(22032),
            new RelicItem(22047),
            new RelicItem(22067),
            new RelicItem(22008),
            new RelicItem(22018),
            new RelicItem(22058),
            new RelicItem(22023),
            new RelicItem(22013),
            new RelicItem(22073),
            new RelicItem(22053),
            new RelicItem(22028),
            new RelicItem(22063),
            new RelicItem(22038),
            new RelicItem(22043),
            new RelicItem(22078),
            new RelicItem(22033),
            new RelicItem(22048),
            new RelicItem(22068),
            new RelicItem(22009),
            new RelicItem(22019),
            new RelicItem(22059),
            new RelicItem(22024),
            new RelicItem(22014),
            new RelicItem(22074),
            new RelicItem(22054),
            new RelicItem(22029),
            new RelicItem(22064),
            new RelicItem(22039),
            new RelicItem(22044),
            new RelicItem(22079),
            new RelicItem(22034),
            new RelicItem(22049),
            new RelicItem(22069),
            new RelicItem(22010),
            new RelicItem(22060),
            new RelicItem(22020),
            new RelicItem(22025),
            new RelicItem(22015),
            new RelicItem(22075),
            new RelicItem(22055),
            new RelicItem(22030),
            new RelicItem(22065),
            new RelicItem(22040),
            new RelicItem(22045),
            new RelicItem(22080),
            new RelicItem(22035),
            new RelicItem(22050),
            new RelicItem(22070),
        };

        private static readonly List<CompletionRequirement> BaseArmorCompletionRequirements = new()
        {
            new CompletionRequirement(21801, 50)
        };
        #endregion

        #region Base +1 Armor
        private static readonly List<RelicItem> BasePlusOneArmorRelic = new()
        {
            new RelicItem(22081),
            new RelicItem(22091),
            new RelicItem(22131),
            new RelicItem(22096),
            new RelicItem(22086),
            new RelicItem(22146),
            new RelicItem(22126),
            new RelicItem(22101),
            new RelicItem(22136),
            new RelicItem(22111),
            new RelicItem(22116),
            new RelicItem(22151),
            new RelicItem(22106),
            new RelicItem(22121),
            new RelicItem(22141),
            new RelicItem(22082),
            new RelicItem(22092),
            new RelicItem(22132),
            new RelicItem(22097),
            new RelicItem(22087),
            new RelicItem(22147),
            new RelicItem(22127),
            new RelicItem(22102),
            new RelicItem(22137),
            new RelicItem(22112),
            new RelicItem(22117),
            new RelicItem(22152),
            new RelicItem(22107),
            new RelicItem(22122),
            new RelicItem(22142),
            new RelicItem(22083),
            new RelicItem(22093),
            new RelicItem(22133),
            new RelicItem(22098),
            new RelicItem(22088),
            new RelicItem(22148),
            new RelicItem(22128),
            new RelicItem(22103),
            new RelicItem(22138),
            new RelicItem(22113),
            new RelicItem(22118),
            new RelicItem(22153),
            new RelicItem(22108),
            new RelicItem(22123),
            new RelicItem(22143),
            new RelicItem(22084),
            new RelicItem(22094),
            new RelicItem(22134),
            new RelicItem(22099),
            new RelicItem(22089),
            new RelicItem(22149),
            new RelicItem(22129),
            new RelicItem(22104),
            new RelicItem(22139),
            new RelicItem(22114),
            new RelicItem(22119),
            new RelicItem(22154),
            new RelicItem(22109),
            new RelicItem(22124),
            new RelicItem(22144),
            new RelicItem(22085),
            new RelicItem(22135),
            new RelicItem(22095),
            new RelicItem(22100),
            new RelicItem(22090),
            new RelicItem(22150),
            new RelicItem(22130),
            new RelicItem(22105),
            new RelicItem(22140),
            new RelicItem(22115),
            new RelicItem(22120),
            new RelicItem(22155),
            new RelicItem(22110),
            new RelicItem(22125),
            new RelicItem(22145),
        };

        private static readonly List<CompletionRequirement> BasePlusOneArmorCompletionRequirements = new()
        {
            new CompletionRequirement(21801, 150)
        };
        #endregion

        #region Base +2 Armor
        private static readonly List<RelicItem> BasePlusTwoArmorRelic = new()
        {
            new RelicItem(22156),
            new RelicItem(22166),
            new RelicItem(22206),
            new RelicItem(22171),
            new RelicItem(22161),
            new RelicItem(22221),
            new RelicItem(22201),
            new RelicItem(22176),
            new RelicItem(22211),
            new RelicItem(22186),
            new RelicItem(22191),
            new RelicItem(22226),
            new RelicItem(22181),
            new RelicItem(22196),
            new RelicItem(22216),
            new RelicItem(22157),
            new RelicItem(22167),
            new RelicItem(22207),
            new RelicItem(22172),
            new RelicItem(22162),
            new RelicItem(22222),
            new RelicItem(22202),
            new RelicItem(22177),
            new RelicItem(22212),
            new RelicItem(22187),
            new RelicItem(22192),
            new RelicItem(22227),
            new RelicItem(22182),
            new RelicItem(22197),
            new RelicItem(22217),
            new RelicItem(22158),
            new RelicItem(22168),
            new RelicItem(22208),
            new RelicItem(22173),
            new RelicItem(22163),
            new RelicItem(22223),
            new RelicItem(22203),
            new RelicItem(22178),
            new RelicItem(22213),
            new RelicItem(22188),
            new RelicItem(22193),
            new RelicItem(22228),
            new RelicItem(22183),
            new RelicItem(22198),
            new RelicItem(22218),
            new RelicItem(22159),
            new RelicItem(22169),
            new RelicItem(22209),
            new RelicItem(22174),
            new RelicItem(22164),
            new RelicItem(22224),
            new RelicItem(22204),
            new RelicItem(22179),
            new RelicItem(22214),
            new RelicItem(22189),
            new RelicItem(22194),
            new RelicItem(22229),
            new RelicItem(22184),
            new RelicItem(22199),
            new RelicItem(22219),
            new RelicItem(22160),
            new RelicItem(22210),
            new RelicItem(22170),
            new RelicItem(22175),
            new RelicItem(22165),
            new RelicItem(22225),
            new RelicItem(22205),
            new RelicItem(22180),
            new RelicItem(22215),
            new RelicItem(22190),
            new RelicItem(22195),
            new RelicItem(22230),
            new RelicItem(22185),
            new RelicItem(22200),
            new RelicItem(22220),
        };

        private static readonly List<CompletionRequirement> BasePlusTwoArmorCompletionRequirements = new()
        {
            new CompletionRequirement(21801, 400)
        };
        #endregion

        #region Anemos Armor
        private static readonly List<RelicItem> AnemosArmorRelic = new()
        {
            new RelicItem(22231),
            new RelicItem(22241),
            new RelicItem(22281),
            new RelicItem(22246),
            new RelicItem(22236),
            new RelicItem(22296),
            new RelicItem(22276),
            new RelicItem(22251),
            new RelicItem(22286),
            new RelicItem(22261),
            new RelicItem(22266),
            new RelicItem(22301),
            new RelicItem(22256),
            new RelicItem(22271),
            new RelicItem(22291),
            new RelicItem(22232),
            new RelicItem(22242),
            new RelicItem(22282),
            new RelicItem(22247),
            new RelicItem(22237),
            new RelicItem(22297),
            new RelicItem(22277),
            new RelicItem(22252),
            new RelicItem(22287),
            new RelicItem(22262),
            new RelicItem(22267),
            new RelicItem(22302),
            new RelicItem(22257),
            new RelicItem(22272),
            new RelicItem(22292),
            new RelicItem(22233),
            new RelicItem(22243),
            new RelicItem(22283),
            new RelicItem(22248),
            new RelicItem(22238),
            new RelicItem(22298),
            new RelicItem(22278),
            new RelicItem(22253),
            new RelicItem(22288),
            new RelicItem(22263),
            new RelicItem(22268),
            new RelicItem(22303),
            new RelicItem(22258),
            new RelicItem(22273),
            new RelicItem(22293),
            new RelicItem(22234),
            new RelicItem(22244),
            new RelicItem(22284),
            new RelicItem(22249),
            new RelicItem(22239),
            new RelicItem(22299),
            new RelicItem(22279),
            new RelicItem(22254),
            new RelicItem(22289),
            new RelicItem(22264),
            new RelicItem(22269),
            new RelicItem(22304),
            new RelicItem(22259),
            new RelicItem(22274),
            new RelicItem(22294),
            new RelicItem(22235),
            new RelicItem(22285),
            new RelicItem(22245),
            new RelicItem(22250),
            new RelicItem(22240),
            new RelicItem(22300),
            new RelicItem(22280),
            new RelicItem(22255),
            new RelicItem(22290),
            new RelicItem(22265),
            new RelicItem(22270),
            new RelicItem(22305),
            new RelicItem(22260),
            new RelicItem(22275),
            new RelicItem(22295),
        };

        private static readonly List<CompletionRequirement> AnemosArmorCompletionRequirements = new()
        {
            new CompletionRequirement(21803, 150)
        };
        #endregion

        public static readonly Dictionary<string, EurekaRelic> AnemosRelicStages = new()
        {
            ["Base"] = new EurekaRelic(BaseRelic, BaseRelicCompletionRequirements),
            ["Base +1"] = new EurekaRelic(BasePlusOneRelic, BasePlusOneCompletionRequirements),
            ["Base +2"] = new EurekaRelic(BasePlusTwoRelic, BasePlusTwoCompletionRequirements),
            ["Anemos Weapon"] = new EurekaRelic(AnemosWeaponRelic, AnemosWeaponCompletionRequirements),
            ["Base Armor"] = new EurekaRelic(BaseArmorRelic, BaseArmorCompletionRequirements),
            ["Base +1 Armor"] = new EurekaRelic(BasePlusOneArmorRelic, BasePlusOneArmorCompletionRequirements),
            ["Base +2 Armor"] = new EurekaRelic(BasePlusTwoArmorRelic, BasePlusTwoCompletionRequirements),
            ["Anemos Armor"] = new EurekaRelic(AnemosArmorRelic, AnemosArmorCompletionRequirements),
        };
    }
}
