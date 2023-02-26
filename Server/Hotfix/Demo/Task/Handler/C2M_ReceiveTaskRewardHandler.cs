using System;

namespace ET
{
    public class C2M_ReceiveTaskRewardHandler : AMActorLocationRpcHandler<Unit,C2M_ReceiveTaskReward,M2C_ReceiveTaskReward>
    {
        protected override async ETTask Run(Unit unit, C2M_ReceiveTaskReward request, M2C_ReceiveTaskReward response, Action reply)
        {
            TasksComponent tasksComponent = unit.GetComponent<TasksComponent>();
            
            int errorCode = tasksComponent.TryReceiveTaskReward(request.TaskConfigId);
            if (errorCode != ErrorCode.ERR_Success)
            {
                response.Error = errorCode;
                reply();
                return;
            }
        
            tasksComponent.ReceiveTaskRewardState(unit,request.TaskConfigId);

            unit.GetComponent<NumericComponent>()[NumericType.Exp] += TaskConfigCategory.Instance.Get(request.TaskConfigId).RewardExpCount;
            
            reply();
            
            await ETTask.CompletedTask;
        }
    }
}