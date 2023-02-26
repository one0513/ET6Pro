namespace ET
{
    public enum TaskState
    {
        None       = -1,
        Doing      = 0,  //正在进行的状态
        Complete   = 1,  //任务完成的状态
        Received   = 2,  //已领取奖励的状态
    }
    
    public enum TaskActionType
    {
        UpLevel      = 1, //升级
        MakeItem     = 2, //打造物品
        Adverture    = 3, //冒险通关
    }
    
    public enum TaskProgressType
    {
        Add          = 1, //增加
        Sub          = 2, //减少
        Update       = 3, //赋值
    }
    
    [ChildOf(typeof(TasksComponent))]
#if SERVER
    public class TaskInfo : Entity,IAwake,IAwake<int>,IDestroy,ISerializeToEntity
#else
    public class TaskInfo : Entity,IAwake,IAwake<int>,IDestroy
#endif
    {
        public int ConfigId    = 0;

        public int TaskState   = 0;

        public int TaskPogress = 0;
    }
}