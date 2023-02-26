using ET.EventType;

namespace ET
{
    public class UpdateTaskInfoEven: AEvent<EventType.UpdateTaskInfo>
    {
        protected override void Run(UpdateTaskInfo args)
        {
            bool isExist = args.ZoneScene.GetComponent<TasksComponent>().IsExistTaskComplete();
            if (isExist)
            {
                RedDotComponent.Instance.RefreshRedDotViewCount("UIMainTask", 1);
            }
            else
            {
                RedDotComponent.Instance.RefreshRedDotViewCount("UIMainTask", 0);
            }
            UIManagerComponent.Instance.GetWindow<UITaskView>()?.UpdateView();
            
        }
    }
}