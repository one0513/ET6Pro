using System;

namespace ET
{
    [FriendClass(typeof(RoomInfo))]
    [FriendClass(typeof(RoleInfo))]
    public class C2M_OutBattleRoomHandler: AMActorLocationRpcHandler<Unit, C2M_OutBattleRoom, M2C_OutBattleRoom>
    {
        protected override async ETTask Run(Unit unit, C2M_OutBattleRoom request, M2C_OutBattleRoom response, Action reply)
        {
            await ETTask.CompletedTask;

            NumericComponent num = unit.GetComponent<NumericComponent>();
            long roomId = num.GetAsLong(NumericType.RoomID);
            if (roomId == 0)
            {
                response.Error = ErrorCode.ERR_AreadyHasRoom;
                reply();
                return;
            }

            RoomInfo info =  unit.DomainScene().GetComponent<RoomInfoComponent>().Get(roomId);
            if (info != null)
            {
                if (info.RoomPlayerNum > 1)
                {
                    reply();
                    info.RoomPlayerNum -= 1;
                    info.playerList.Remove(unit.Id);
                    num.Set(NumericType.RoomID,0);
                    await DBManagerComponent.Instance.GetZoneDB(unit.DomainZone()).Save<RoomInfo>(info);
                    return;
                }
                else
                {
                    reply();
                    unit.DomainScene().GetComponent<RoomInfoComponent>().Remove(roomId);
                    num.Set(NumericType.RoomID,0);
                    await DBManagerComponent.Instance.GetZoneDB(unit.DomainZone()).Remove<RoomInfo>(d => d.RoomId == roomId);
                    return;
                }
            }
            else
            {
                var infos =  await DBManagerComponent.Instance.GetZoneDB(unit.DomainZone()).Query<RoomInfo>(d => d.RoomId == roomId);
                if (infos.Count <= 0)
                {
                    response.Error = ErrorCode.ERR_NotInRoom;
                    reply();
                    return;
                }

                if (infos[0].playerList.Count <= 1 )
                {
                    reply();
                    num.Set(NumericType.RoomID,0);
                    await DBManagerComponent.Instance.GetZoneDB(unit.DomainZone()).Remove<RoomInfo>(d => d.RoomId == roomId);
                    return;
                }
            
                if (infos[0].playerList.Count > 1 )
                {
                    reply();
                    infos[0].RoomPlayerNum -= 1;
                    infos[0].playerList.Remove(unit.Id);
                    num.Set(NumericType.RoomID,0);
                    await DBManagerComponent.Instance.GetZoneDB(unit.DomainZone()).Save<RoomInfo>(infos[0]);
                    return;
                }
            }
            reply();
        }
    }
}