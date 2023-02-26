using System.Linq;
using ET.EventType;

namespace ET
{
    public class TasksComponentDestroySystem: DestroySystem<TasksComponent>
    {
        public override void Destroy(TasksComponent self)
        {
            foreach(TaskInfo taskInfo in self.TaskInfoDict.Values)
            {
                taskInfo?.Dispose();
            }
            self.TaskInfoDict.Clear();
        }
    }

    [FriendClassAttribute(typeof(ET.TasksComponent))]
    [FriendClassAttribute(typeof(ET.TaskInfo))]
    public static class TasksComponentSystem
    {
        public static void AddOrUpdateTaskInfo(this TasksComponent self, TaskInfoProto taskInfoProto)
        {
            TaskInfo taskInfo = self.GetTaskInfoByConfigId(taskInfoProto.ConfigId);
            if ( taskInfo == null )
            {
                taskInfo = self.AddChild<TaskInfo>();
                self.TaskInfoDict.Add(taskInfoProto.ConfigId, taskInfo);
            }
            taskInfo.FromMessage(taskInfoProto);
            
            Game.EventSystem.Publish(new UpdateTaskInfo(){ ZoneScene = self.ZoneScene()});
        }


        public static TaskInfo GetTaskInfoByConfigId(this TasksComponent self, int configId)
        {
            self.TaskInfoDict.TryGetValue(configId, out TaskInfo taskInfo);
            return taskInfo;
        }

        public static int GetTaskInfoCount(this TasksComponent self)
        {
            self.TaskInfoList.Clear();
            self.TaskInfoList = self.TaskInfoDict.Values.Where(a => !a.IsTaskState(TaskState.Received)).ToList();
            self.TaskInfoList.Sort(( a,  b) => { return b.TaskState - a.TaskState;});
            return self.TaskInfoList.Count;
        }
        
        public static TaskInfo GetTaskInfoByIndex(this TasksComponent self,int index)
        {
            if (index < 0 || index >= self.TaskInfoList.Count)
            {
                return null;
            }
            return self.TaskInfoList[index];
        }
        
        public static bool IsExistTaskComplete(this TasksComponent self)
        {
            foreach (var taskInfo in self.TaskInfoDict.Values)
            {
                if ( taskInfo.IsTaskState(TaskState.Complete) )
                {
                    return true;
                }
            }

            return false;
        }
    }
}