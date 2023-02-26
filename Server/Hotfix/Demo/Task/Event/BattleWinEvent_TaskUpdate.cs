using ET.EventType;

namespace ET
{
    public class BattleWinEvent_TaskUpdate : AEvent<BattleWin>
    {
        protected override void Run(BattleWin args)
        {
            args.Unit.GetComponent<TasksComponent>().TriggerTaskAction(TaskActionType.Adverture,count:1, targetId : args.LevelId);
        }
    }
}