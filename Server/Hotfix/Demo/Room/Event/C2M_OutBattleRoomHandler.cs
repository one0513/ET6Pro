using System;
using System.Collections.Generic;

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

            RoomInfo info =  await unit.DomainScene().GetComponent<RoomInfoComponent>().Get(roomId);
            if (info != null)
            {
                if (info.RoomPlayerNum > 1)
                {
                    reply();
                    info.RoomPlayerNum -= 1;
                    info.playerList.Remove(unit.Id);
                    num.Set(NumericType.RoomID,0);
                    
                    List<long> monsterIdsList =  unit.DomainScene().GetComponent<MonsterFactoryComponent>().Get(roomId);
                    M2C_RemoveUnits m2CRemoveUnits = new M2C_RemoveUnits();
                    m2CRemoveUnits.Units = monsterIdsList;
                    foreach (var unitId in info.playerList)
                    {
                        if (unitId != unit.Id && unit.DomainScene().GetComponent<UnitComponent>().Get(unitId) != null)
                        {
                            m2CRemoveUnits.Units.Add(unitId);
                        }
                    }
                    MessageHelper.SendToClient(unit,m2CRemoveUnits);
                    
                    
                    await DBManagerComponent.Instance.GetZoneDB(unit.DomainZone()).Save<RoomInfo>(info);
                    return;
                }
                else
                {
                    reply();
                    
                    unit.DomainScene().GetComponent<RoomInfoComponent>().Remove(roomId);
                    num.Set(NumericType.RoomID,0);
                    List<long> monsterIdsList =  unit.DomainScene().GetComponent<MonsterFactoryComponent>().Get(roomId);
                    M2C_RemoveUnits m2CRemoveUnits = new M2C_RemoveUnits();
                    m2CRemoveUnits.Units = monsterIdsList;
                    MessageHelper.SendToClient(unit,m2CRemoveUnits);
                    for (int i = 0; i < monsterIdsList.Count; i++)
                    {
                        unit.DomainScene().GetComponent<UnitComponent>().Remove(monsterIdsList[i]);
                    }
                    unit.DomainScene().GetComponent<MonsterFactoryComponent>().Remove(roomId);
                    await DBManagerComponent.Instance.GetZoneDB(unit.DomainZone()).Remove<RoomInfo>(d => d.RoomId == roomId);
                    return;
                }
            }
            reply();
        }
    }
}