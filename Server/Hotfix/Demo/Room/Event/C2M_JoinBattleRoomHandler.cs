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
            RoomInfo info = await unit.DomainScene().GetComponent<RoomInfoComponent>().Get(request.RoomId);
            
            //var infos =  await DBManagerComponent.Instance.GetZoneDB(unit.DomainZone()).Query<RoomInfo>(d => d.RoomId == request.RoomId);
            if (info == null)
            {
                response.Error = ErrorCode.ERR_CurRoomError;
                reply();
                return;
            }
            if (info.RoomPlayerNum >= 3)
            {
                response.Error = ErrorCode.ERR_CurRoomError;
                reply();
                return;
            }
            info.playerList.Add(unit.Id);
            info.RoomPlayerNum += 1;
            num.Set(NumericType.RoomID,request.RoomId);

            reply();

            
            if (info != null)
            {
                info.playerList.Add(unit.Id);
                info.RoomPlayerNum += 1;
                await DBManagerComponent.Instance.GetZoneDB(unit.DomainZone()).Save<RoomInfo>(info);
            }
            
            
        }
    }
}