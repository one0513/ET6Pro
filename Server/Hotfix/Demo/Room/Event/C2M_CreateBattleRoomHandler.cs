using System;

namespace ET
{
    [FriendClass(typeof(RoomInfo))]
    [FriendClass(typeof(RoleInfo))]
    public class C2M_CreateBattleRoomHandler: AMActorLocationRpcHandler<Unit, C2M_CreateBattleRoom, M2C_CreateBattleRoom>
    {
        protected override async ETTask Run(Unit unit, C2M_CreateBattleRoom request, M2C_CreateBattleRoom response, Action reply)
        {
            await ETTask.CompletedTask;
            NumericComponent num = unit.GetComponent<NumericComponent>();
            if (num.GetAsLong(NumericType.RoomID) != 0)
            {
                response.Error = ErrorCode.ERR_AreadyHasRoom;
                reply();
                return;
            }

            long roomId = IdGenerater.Instance.GenerateId();

            RoomInfo info = new RoomInfo();
            info.Id = roomId;
            info.RoomId = roomId;
            info.RoomName = $"{unit.GetComponent<RoleInfo>().Name}的小队";
            info.RoomPlayerNum += 1;
            info.playerList.Add(unit.Id);
            unit.DomainScene().GetComponent<RoomInfoComponent>().Add(info);
            num.Set(NumericType.RoomID,roomId);
            
            await DBManagerComponent.Instance.GetZoneDB(unit.DomainZone()).Save<RoomInfo>(info);
            
            reply();
        }
    }
}