
namespace ET
{
    public class TaskInfoAwakeSystem : AwakeSystem<TaskInfo,int>
    {
        public override void Awake(TaskInfo self,int configId)
        {
            self.ConfigId    = configId;
            self.TaskPogress = 0;
            self.TaskState   = (int)TaskState.Doing;
        }
    }
    
    public class  TaskInfoDestroySystem : DestroySystem<TaskInfo>
    {
        public override void Destroy(TaskInfo self)
        {
            self.ConfigId    = 0;
            self.TaskPogress = 0;
            self.TaskState   = (int)TaskState.None;
        }
    }
    

    [FriendClassAttribute(typeof(ET.TaskInfo))]
    public static class TaskInfoSystem
    {
        public static void FromMessage(this TaskInfo self, TaskInfoProto taskInfoProto)
        {
            self.ConfigId = taskInfoProto.ConfigId;
            self.TaskPogress = taskInfoProto.TaskPogress;
            self.TaskState = taskInfoProto.TaskState;
        }
        
        public static TaskInfoProto ToMessage(this TaskInfo self)
        {
            TaskInfoProto TaskInfoProto = new TaskInfoProto();
            TaskInfoProto.ConfigId      = self.ConfigId;
            TaskInfoProto.TaskPogress   = self.TaskPogress;
            TaskInfoProto.TaskState     = self.TaskState;
            return TaskInfoProto;
        }


        public static void SetTaskState(this TaskInfo self, TaskState taskState)
        {
            self.TaskState = (int)taskState;
        }
        
        public static bool IsTaskState(this TaskInfo self, TaskState taskState)
        {
            return self.TaskState == (int)taskState;
        }

        public static void UpdateProgress(this TaskInfo self, int count)
        {
            var taskActionType = TaskConfigCategory.Instance.Get(self.ConfigId).TaskActionType;
            var config         = TaskActionConfigCategory.Instance.Get(taskActionType);
            if (config.TaskProgressType == (int)TaskProgressType.Add)
            {
                self.TaskPogress += count;
            }
            else if (config.TaskProgressType == (int)TaskProgressType.Sub)
            {
                self.TaskPogress -= count;
            }
            else if (config.TaskProgressType == (int)TaskProgressType.Update)
            {
                self.TaskPogress = count;
            }
        }        
        
        /// <summary>
        /// 是否可以被完成
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static void TryCompleteTask(this TaskInfo self)
        {
            if ( !self.IsCompleteProgress() || !self.IsTaskState(TaskState.Doing) )
            {
                return;
            }
            
            self.TaskState = (int)TaskState.Complete;
        }

        
        /// <summary>
        /// 是否达到任务目标数量
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static bool IsCompleteProgress(this TaskInfo self)
        {
            return self.TaskPogress >= TaskConfigCategory.Instance.Get(self.ConfigId).TaskTargetCount;
        }
    }
}