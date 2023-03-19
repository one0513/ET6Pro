using System;
using System.Collections.Generic;

namespace ET
{
    [FriendClass(typeof(RoomInfo))]
    [FriendClass(typeof(RoleInfo))]
    [FriendClass(typeof(AIComponent))]
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
            unit.RemoveComponent<BattleUnitFindComponent>();
            unit.GetComponent<AIComponent>().Current = 0;
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
                    for (int i = 0; i < monsterIdsList.Count; i++)
                    {
                        m2CRemoveUnits.Units.Add(monsterIdsList[i]);
                    }
                    foreach (var unitId in info.playerList)
                    {
                        Unit player = unit.DomainScene().GetComponent<UnitComponent>().Get(unitId);
                        if (unitId != unit.Id && player != null)
                        {
                            m2CRemoveUnits.Units.Add(unitId);
                            
                            M2C_RemoveUnits m2CRemoveMyUnits = new M2C_RemoveUnits();
                            m2CRemoveMyUnits.Units = new List<long>(){unit.Id};
                            MessageHelper.SendToClient(player,m2CRemoveMyUnits);
                            
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
                    for (int i = 0; i < monsterIdsList.Count; i++)
                    {
                        m2CRemoveUnits.Units.Add(monsterIdsList[i]);
                    }
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