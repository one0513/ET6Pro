namespace ET
{
    public class AdventreBattleOverEvent_PlayWinAnimation: AEventAsync<EventType.AdventureBattleOver>
    {
        protected override async ETTask Run(EventType.AdventureBattleOver args)
        {
            args.WinUnit?.GetComponent<AnimatorComponent>()?.Play(MotionType.Win);
            await ETTask.CompletedTask;
        }
    }
}