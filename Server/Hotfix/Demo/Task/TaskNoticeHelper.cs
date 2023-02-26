namespace ET
{
    [FriendClassAttribute(typeof(ET.TasksComponent))]
    public static class TaskNoticeHelper
    {
        public static void SyncTaskInfo(Unit unit, TaskInfo taskInfo, M2C_UpdateTaskInfo updateTaskInfo)
        {
            updateTaskInfo.TaskInfoProto = taskInfo.ToMessage();
            MessageHelper.SendToClient(unit, updateTaskInfo);
        }
        
        public static void SyncAllTaskInfo(Unit unit)
        {
            TasksComponent tasksComponent = unit.GetComponent<TasksComponent>();
            
            M2C_AllTaskInfoList m2CAllTaskInfoList = new M2C_AllTaskInfoList();

            foreach (var taskInfo in tasksComponent.TaskInfoDict.Values)
            {
                m2CAllTaskInfoList.TaskInfoProtoList.Add(taskInfo.ToMessage());
            }
            MessageHelper.SendToClient(unit, m2CAllTaskInfoList);
        }
        
    }
}