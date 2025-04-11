using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Attributes;
using MCM.Abstractions.Base.Global;
using TaleWorlds.Localization;

namespace CompleteQuestsToGainSkills
{
    internal class MCMSettings : AttributeGlobalSettings<MCMSettings>
    {
        public override string Id
        { get { return "CompleteQuestsToGainSkillsSettings"; } }

        public override string DisplayName
        { get { return new TextObject("{=CQTGS_THXRbXba}Complete Quests To Gain Skills").ToString(); } }

        public override string FolderName
        { get { return "CompleteQuestsToGainSkills"; } }

        public override string FormatType
        { get { return "json2"; } }

        [SettingPropertyBool("{=CQTGS_gz6KzX0w}Exclude Skills Above Limit", Order = 0, RequireRestart = false, HintText = "{=CQTGS_7ITa6UqH}Toggle excluding skills above a certain level from random selection using slider value below. [Default: disabled]")]
        [SettingPropertyGroup("{=CQTGS_CWJ2Qq7o}Main settings", GroupOrder = 0)]
        public bool ExcludeSkillsAboveLimit { get; set; } = false;

        [SettingPropertyInteger("{=CQTGS_DjNHBZmC}Skill Level Limit", 50, 1000, "0", Order = 1, RequireRestart = false, HintText = "{=CQTGS_tJVNwaoT}Set the maximum skill level for skills to be included in random selection. [Default: 300]")]
        [SettingPropertyGroup("{=CQTGS_CWJ2Qq7o}Main settings", GroupOrder = 0)]
        public int SkillLevelLimit { get; set; } = 300;

        [SettingPropertyFloatingInteger("{=CQTGS_NK5OQ7Tl}Weight Exponent", 0.0f, 3.0f, "0.0", Order = 2, RequireRestart = false, HintText = "{=CQTGS_JD4o4eBb}Controls how much more likely lower-level skills are to be increased. Higher values make low-level skills more likely to increase, while a value of 0 gives all skills an equal chance of being increased. [Default: 1.0]")]
        [SettingPropertyGroup("{=CQTGS_CWJ2Qq7o}Main settings", GroupOrder = 0)]
        public float WeightExponent { get; set; } = 1.0f;

        [SettingPropertyBool("{=CQTGS_EkjJT9pW}Skill Increases Notifications", Order = 3, RequireRestart = false, HintText = "{=CQTGS_9gBjEs7r}Notifications for skill increases after a quest is completed. [Default: enabled]")]
        [SettingPropertyGroup("{=CQTGS_CWJ2Qq7o}Main settings", GroupOrder = 0)]
        public bool NotificationsEnabled { get; set; } = true;


        [SettingPropertyBool("{=CQTGS_hCh6K70k}Logging for debugging", Order = 0, RequireRestart = false, HintText = "{=CQTGS_KcYHRAqu}Logging for debugging (English only). [Default: disabled]")]
        [SettingPropertyGroup("{=CQTGS_OwsWuWum}Technical settings", GroupOrder = 1)]
        public bool LoggingEnabled { get; set; } = false;
    }
}