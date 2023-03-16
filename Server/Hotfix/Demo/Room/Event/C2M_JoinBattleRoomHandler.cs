using System;

namespace ET
{
    [FriendClass(typeof(RoomInfo))]
    public class C2M_JoinBattleRoomHandler: AMActorLocationRpcHandler<Unit, C2M_JoinBattleRoom, M2C_JoinBattleRoom>
    {
        protected override async ETTask Run(Unit unit, C2M_JoinBattleRoom request, M2C_JoinBattleRoom response, Action reply)
        {
            await ETTask.CompletedTask;
            
            NumericComponent num = unit.GetComponent<NumericComponent>();
            long roomId = num.GetAsLong(NumericType.RoomID);
            if (roomId != 0)
            {
                response.Error = ErrorCode.ERR_AreadyHasRoom;
                reply();
                return;
            }
            
            var infos =  await DBManagerComponent.Instance.GetZoneDB(unit.DomainZone()).Query<RoomInfo>(d => d.RoomId == request.RoomId);
            if (infos.Count <= 0)
            {
                response.Error = ErrorCode.ERR_CurRoomError;
                reply();
                return;
            }
            if (infos[0].RoomPlayerNum >= 3)
            {
                response.Error = ErrorCode.ERR_CurRoomError;
                reply();
                return;
            }
            infos[0].playerList.Add(unit.Id);
            infos[0].RoomPlayerNum += 1;
            num.Set(NumericType.RoomID,request.RoomId);

            reply();
            if (unit.DomainScene().GetComponent<RoomInfoComponent>().Get(request.RoomId) != null)
            {
                unit.DomainScene().GetComponent<RoomInfoComponent>().Get(request.RoomId).playerList.Add(unit.Id);
                unit.DomainScene().GetComponent<RoomInfoComponent>().Get(request.RoomId).RoomPlayerNum += 1;

            }
            await DBManagerComponent.Instance.GetZoneDB(unit.DomainZone()).Save<RoomInfo>(infos[0]);
            
        }
    }
}