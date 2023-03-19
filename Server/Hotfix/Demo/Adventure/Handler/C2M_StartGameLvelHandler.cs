using System;

namespace ET
{
    [FriendClass(typeof(RoomInfo))]
    [FriendClass(typeof(AIComponent))]
    public class C2M_StartGameLvelHandler : AMActorLocationRpcHandler<Unit,C2M_StartGameLevel,M2C_StartGameLevel>
    {
        protected override async ETTask Run(Unit unit, C2M_StartGameLevel request, M2C_StartGameLevel response, Action reply)
        {
            NumericComponent numericComponent = unit.GetComponent<NumericComponent>();

            long roomId = numericComponent.GetAsLong(NumericType.RoomID);
            if (roomId == 0)
            {
                response.Error = ErrorCode.ERR_NotInRoom;
                reply();
                return;
            }
            reply();
            unit.DomainScene().GetComponent<MonsterFactoryComponent>().CreateLevelMonster(roomId, request.LevelId).Coroutine();
            var roomInfo =  await unit.DomainScene().GetComponent<RoomInfoComponent>().Get(roomId);
            foreach (var playerId in roomInfo.playerList)
            {
                if (unit.DomainScene().GetComponent<UnitComponent>().Get(playerId) != null)
                {
                    unit.DomainScene().GetComponent<UnitComponent>().Get(playerId).GetComponent<AIComponent>().Current = 0;
                }
                
            }
            
            await ETTask.CompletedTask;
        }
    }
}