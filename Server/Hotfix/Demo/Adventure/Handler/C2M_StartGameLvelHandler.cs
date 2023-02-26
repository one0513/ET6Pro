using System;

namespace ET
{
    public class C2M_StartGameLvelHandler : AMActorLocationRpcHandler<Unit,C2M_StartGameLevel,M2C_StartGameLevel>
    {
        protected override async ETTask Run(Unit unit, C2M_StartGameLevel request, M2C_StartGameLevel response, Action reply)
        {
            NumericComponent numericComponent = unit.GetComponent<NumericComponent>();

            if ( numericComponent.GetAsInt(NumericType.AdventureState) != 0 )
            {
                response.Error = ErrorCode.ERR_AlreadyAdventureState;
                reply();
                return;
            }
            
            numericComponent.Set(NumericType.AdventureState, 1);
            numericComponent.Set(NumericType.CurLevel, request.LevelId);
            numericComponent.Set(NumericType.AdventureStartTime,TimeHelper.ServerNow());
            //设置本次战斗的随机种子，保证客户端的战斗中的每次随机产生的数能在服务器端复现
            numericComponent.Set(NumericType.BattleRandomSeed,RandomHelper.RandUInt32());
            
            reply();
            await ETTask.CompletedTask;
        }
    }
}