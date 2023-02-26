namespace ET
{
    public class AdventureRoundResetEvent_ResetAnimation: AEventAsync<EventType.AdventureRoundReset>
    {
        protected override async ETTask Run(EventType.AdventureRoundReset args)
        {
            Unit unit = UnitHelper.GetMyUnitFromCurrentScene(args.ZoneScene.CurrentScene());
            unit?.GetComponent<AnimatorComponent>()?.Play(MotionType.Idle);
            await ETTask.CompletedTask;
        }
    }
}