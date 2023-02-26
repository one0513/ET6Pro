using System;

namespace ET
{
    public static class TaskHelper
    {
        public static async ETTask<int>  GetTaskReward(Scene ZoneScene, int taskConfigId)
        {
            TaskInfo taskInfo = ZoneScene.GetComponent<TasksComponent>().GetTaskInfoByConfigId(taskConfigId);

            if ( taskInfo == null || taskInfo.IsDisposed )
            {
                return ErrorCode.ERR_NoTaskInfoExist;
            }

            if ( !taskInfo.IsTaskState(TaskState.Complete) )
            {
                return ErrorCode.ERR_TaskNoCompleted;
            }

            M2C_ReceiveTaskReward m2CReciveTaskReward = null;
            try
            {
                m2CReciveTaskReward = (M2C_ReceiveTaskReward)await ZoneScene.GetComponent<SessionComponent>().Session.Call(new C2M_ReceiveTaskReward() { TaskConfigId = taskConfigId });
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
                return ErrorCode.ERR_NetWorkError;
            }
            
            return m2CReciveTaskReward.Error;
        }
    }
}