using System.Reflection;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace CompleteQuestsToGainSkills
{
    public class SubModule : MBSubModuleBase
    {
        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
        }

        protected override void OnSubModuleUnloaded()
        {
            base.OnSubModuleUnloaded();
        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            base.OnBeforeInitialModuleScreenSetAsRoot();
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            base.OnGameStart(game, gameStarterObject);
            if (Campaign.Current is Campaign campaign && campaign.GameMode == CampaignGameMode.Campaign)
            {
                CampaignGameStarter campaignGameStarter = (CampaignGameStarter)gameStarterObject;
                campaignGameStarter.AddBehavior(new CompleteQuestsToGainSkillsBehavior());
            }
        }

        public override void OnGameEnd(Game game)
        {
            var eventField = typeof(CampaignEvents).GetField("OnQuestCompletedEvent", BindingFlags.Static | BindingFlags.NonPublic);
            MulticastDelegate? eventDelegate = eventField?.GetValue(null) as MulticastDelegate;
            if (eventDelegate != null && eventDelegate.GetInvocationList().Length > 0)
            {
                CampaignEvents.OnQuestCompletedEvent.ClearListeners(this);
            }
            base.OnGameEnd(game);
        }
    }
}