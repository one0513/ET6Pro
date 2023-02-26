namespace ET.Event
{
    public class AdventureBattleReportEvent_RequestEndGameLevel : AEventAsync<EventType.AdventureBattleReport>
    {
        protected override async ETTask Run(EventType.AdventureBattleReport args)
        {
            if (args.BattleRoundResult == BattleRoundResult.KeepBattle)
            {
                return;
            }
            
            int errCode = await AdventureHelper.RequestEndGameLevel(args.ZoneScene,args.BattleRoundResult,args.Round);

            if (errCode != ErrorCode.ERR_Success)
            {
                return;
            }


            await TimerComponent.Instance.WaitAsync(2000);
            
            args.ZoneScene?.CurrentScene()?.GetComponent<AdventureComponent>()?.ShowAdventureHpBarInfo(false);
            args.ZoneScene?.CurrentScene()?.GetComponent<AdventureComponent>()?.ResetAdventure();
            args.ZoneScene?.CurrentScene()?.GetComponent<AdventureComponent>()?.SetFightStatue(false);
            
            await ETTask.CompletedTask;
        }
    }
}