using ET.EventType;

namespace ET
{

    [NumericWatcher(NumericType.Lv)]
    public class NumericWatcher_UpLevel : INumericWatcher
    {
        public void Run(NumbericChange args)
        {
            if (!(args.Parent is Unit unit))
            {
                return;
            }
            unit = args.Parent as Unit;
            unit.GetComponent<TasksComponent>().TriggerTaskAction(TaskActionType.UpLevel,(int)args.New);
            
            //RankHelper.AddOrUpdateLevelRank(unit);
        }
    }
}