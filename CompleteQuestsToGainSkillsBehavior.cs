using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.Roster;
using MCM.Abstractions.Base.Global;
using TaleWorlds.CampaignSystem.Issues;
using TaleWorlds.Localization;

namespace CompleteQuestsToGainSkills
{
    public class CompleteQuestsToGainSkillsBehavior : CampaignBehaviorBase
    {
        private static MCMSettings? _fallback;
        private static MCMSettings settings => AttributeGlobalSettings<MCMSettings>.Instance ?? (_fallback ??= new MCMSettings());

        public override void RegisterEvents()
        {
            CampaignEvents.OnQuestCompletedEvent.AddNonSerializedListener(this, OnQuestCompleted);
            CampaignEvents.OnIssueUpdatedEvent.AddNonSerializedListener(this, OnIssueUpdated);
        }

        private void OnQuestCompleted(QuestBase quest, QuestBase.QuestCompleteDetails detail)
        {
            if (detail != QuestBase.QuestCompleteDetails.Success) return;
            Random random = new Random();
            bool any = false;
            foreach (Hero hero in ListOfHeroesInParty(Hero.MainHero))
            {
                SkillObject skill = GetRandomSkillBasedOnLevel(hero, random, settings.WeightExponent);
                if (skill == null) continue;
                if (!any && settings.NotificationsEnabled)
                {
                    InformationManager.DisplayMessage(new InformationMessage(new TextObject("{=CQTGS_edy4tT9Y}After completing the quest, your party improved their skills:").ToString(), Colors.Yellow));
                    any = true;
                }
                IncreaseHeroSkill(hero, skill, settings.NotificationsEnabled);
            }
        }
        
        private void OnIssueUpdated(IssueBase issue, IssueBase.IssueUpdateDetails details, Hero issueSolver)
        {
            if (details != IssueBase.IssueUpdateDetails.IssueFinishedByAILord || issueSolver == null || issueSolver == Hero.MainHero || issueSolver.Clan == Clan.PlayerClan)
                return;

            Random random = new Random();
            List<Hero> heroes = ListOfHeroesInParty(issueSolver);
            for (int i = 0; i < heroes.Count; i++)
            {
                SkillObject skill = GetRandomSkillBasedOnLevel(heroes[i], random, settings.WeightExponent, false);
                if (skill != null)
                    IncreaseHeroSkill(heroes[i], skill, false);
            }
        }

        private SkillObject GetRandomSkillBasedOnLevel(Hero hero, Random random, double exponent = 1.0, bool log = true)
        {
            List<SkillObject> skills = new List<SkillObject>
            {
                DefaultSkills.OneHanded,
                DefaultSkills.TwoHanded,
                DefaultSkills.Polearm,
                DefaultSkills.Bow,
                DefaultSkills.Crossbow,
                DefaultSkills.Throwing,
                DefaultSkills.Riding,
                DefaultSkills.Athletics,
                DefaultSkills.Crafting,
                DefaultSkills.Tactics,
                DefaultSkills.Scouting,
                DefaultSkills.Roguery,
                DefaultSkills.Charm,
                DefaultSkills.Leadership,
                DefaultSkills.Trade,
                DefaultSkills.Steward,
                DefaultSkills.Medicine,
                DefaultSkills.Engineering
            };

            List<(SkillObject skill, double weight)> skillWeights = new List<(SkillObject, double)>();
            double totalWeight = 0;
            foreach (var skill in skills)
            {
                int skillLevel = Math.Max(1, hero.GetSkillValue(skill));

                if (settings.ExcludeSkillsAboveLimit && skillLevel > settings.SkillLevelLimit)
                    continue;

                double weight = Math.Max(1, 200 / Math.Pow(Math.Log(skillLevel + 1), exponent));
                skillWeights.Add((skill, weight));
                totalWeight += weight;
            }

            if (skillWeights.Count == 0)
            {
                if (log && settings.LoggingEnabled)
                    InformationManager.DisplayMessage(new InformationMessage("No eligible skills found for selection."));
                return null;
            }

            skillWeights.Sort((x, y) => y.weight.CompareTo(x.weight));

            if (log && settings.LoggingEnabled)
            {
                foreach (var skillWeight in skillWeights)
                {
                    InformationManager.DisplayMessage(new InformationMessage($"{skillWeight.skill.Name}: Level {hero.GetSkillValue(skillWeight.skill)}, Weight {skillWeight.weight}"));
                }
                InformationManager.DisplayMessage(new InformationMessage($"Total Weight: {totalWeight}"));
            }

            double randomValue = random.NextDouble() * totalWeight;

            if (log && settings.LoggingEnabled)
                InformationManager.DisplayMessage(new InformationMessage($"Random Value: {randomValue}"));

            double cumulativeWeight = 0;
            foreach (var skillWeight in skillWeights)
            {
                cumulativeWeight += skillWeight.weight;
                if (randomValue < cumulativeWeight)
                {
                    if (log && settings.LoggingEnabled)
                        InformationManager.DisplayMessage(new InformationMessage($"Selected Skill: {skillWeight.skill.Name}"));

                    return skillWeight.skill;
                }
            }

            if (log && settings.LoggingEnabled)
                InformationManager.DisplayMessage(new InformationMessage($"Fallback Selected Skill: {skillWeights[skillWeights.Count - 1].skill.Name}"));

            return skillWeights[skillWeights.Count - 1].skill;
        }

        private void IncreaseHeroSkill(Hero hero, SkillObject skill, bool notify)
        {
            if (hero?.HeroDeveloper == null || skill == null)
                return;

            if (hero.PartyBelongedTo == null || hero.IsDead || !hero.IsActive)
                return;

            hero.HeroDeveloper.ChangeSkillLevel(skill, 1, notify);
        }

        public static List<Hero> ListOfHeroesInParty(Hero hero)
        {
            List<Hero> listHeroes = new List<Hero>();

            if (hero?.PartyBelongedTo?.MemberRoster == null)
                return listHeroes;

            MBList<TroopRosterElement> listTroops = hero.PartyBelongedTo.MemberRoster.GetTroopRoster();

            if (listTroops == null)
                return listHeroes;

            foreach (var member in listTroops)
            {
                if (member.Character != null && member.Character.IsHero)
                {
                    var partyHero = member.Character.HeroObject;

                    if (partyHero != null && !listHeroes.Contains(partyHero))
                        listHeroes.Add(partyHero);
                }
            }

            return listHeroes;
        }

        public override void SyncData(IDataStore dataStore) { }
    }
}