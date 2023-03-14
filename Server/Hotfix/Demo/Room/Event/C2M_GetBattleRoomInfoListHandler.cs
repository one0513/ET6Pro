using System;
using ET.Room;

namespace ET
{
    [FriendClass(typeof(RoomInfo))]
    public class C2M_GetBattleRoomInfoListHandler: AMActorLocationRpcHandler<Unit, C2M_GetBattleRoomInfoList, M2C_GetBattleRoomInfoList>
    {
        protected override async ETTask Run(Unit unit, C2M_GetBattleRoomInfoList request, M2C_GetBattleRoomInfoList response, Action reply)
        {
            var infos =  await DBManagerComponent.Instance.GetZoneDB(unit.DomainZone()).Query<RoomInfo>(d => d.RoomPlayerNum <3);

            foreach (var info in infos)
            {
                response.BattleRoomInfoList.Add(info.ToMessage());
            }
            reply();
            await ETTask.CompletedTask;
        }
    }
}