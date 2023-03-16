using System;

namespace ET
{
    [FriendClass(typeof(RoomInfo))]
    [FriendClass(typeof(RoleInfo))]
    public class C2M_GetBattleRoomPlayerInfoHandler: AMActorLocationRpcHandler<Unit, C2M_GetBattleRoomPlayerInfo, M2C_GetBattleRoomPlayerInfo>
    {
        protected override async ETTask Run(Unit unit, C2M_GetBattleRoomPlayerInfo request, M2C_GetBattleRoomPlayerInfo response, Action reply)
        {
            await ETTask.CompletedTask;
            NumericComponent num = unit.GetComponent<NumericComponent>();
            if (num.GetAsLong(NumericType.RoomID) == 0)
            {
                response.Error = ErrorCode.ERR_NotInRoom;
                reply();
                return;
            }

            var infos =  await DBManagerComponent.Instance.GetZoneDB(unit.DomainZone()).Query<RoomInfo>(d => d.RoomId == num.GetAsLong(NumericType.RoomID));
            
            if (infos.Count <= 0)
            {
                response.Error = ErrorCode.ERR_CurRoomError;
                reply();
                return;
            }

            for (int i = 0; i < infos[0].playerList.Count; i++)
            {
               Unit roomPlayer =  unit.DomainScene().GetComponent<UnitComponent>().Get(infos[0].playerList[i]);
               if (roomPlayer != null)
               {
                   response.PlayerInfoList.Add(new PlayerInfoProto(){Name =  roomPlayer.GetComponent<RoleInfo>().Name,
                       Level = roomPlayer.GetComponent<NumericComponent>().GetAsInt(NumericType.Lv),IsOnline = 0});//isOnline 0在线1离线
               }
               else
               {
                   var roleinfos =  await DBManagerComponent.Instance.GetZoneDB(unit.DomainZone()).Query<RoleInfo>(d => d.Id == infos[0].playerList[i]);
                   var numInfo = await DBManagerComponent.Instance.GetZoneDB(unit.DomainZone()).Query<NumericComponent>(d => d.Id == infos[0].playerList[i]);
                   response.PlayerInfoList.Add(new PlayerInfoProto(){Name =  roleinfos[0].Name,
                       Level = numInfo[0].GetAsInt(NumericType.Lv),IsOnline = 1});
               }
               
            }
            
            reply();
        }
    }
}