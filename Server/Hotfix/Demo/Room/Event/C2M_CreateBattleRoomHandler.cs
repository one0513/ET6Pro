using System;
using System.Collections.Generic;
using UnityEngine;

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
            
            //todo: 初始化队友和怪物
            M2C_CreateUnits m2CCreateUnits = new M2C_CreateUnits();
            foreach (var unitId in info.playerList)
            {
                Unit otherPlayer = unit.DomainScene().GetComponent<UnitComponent>().Get(unitId);
                if (unitId != unit.Id && otherPlayer != null)
                {
                    m2CCreateUnits.Units.Add(UnitHelper.CreateUnitInfo(otherPlayer));
                }
            }
            List<long> monsterIds = unit.DomainScene().GetComponent<MonsterFactoryComponent>().Get(info.RoomId);
            if (monsterIds != null && monsterIds.Count > 0)
            {
                for (int i = 0; i < monsterIds.Count; i++)
                {
                    m2CCreateUnits.Units.Add(UnitHelper.CreateUnitInfo(unit.DomainScene().GetComponent<UnitComponent>().Get(monsterIds[i])));
                }
            }
            else
            {
                for (int i = 0; i < 3; i++)
                {
                    Unit monster = unit.DomainScene().GetComponent<MonsterFactoryComponent>().CreateMonsterUnit(info.RoomId);

                    monster.AddComponent<BattleUnitFindComponent, long>(info.RoomId);
                    monster.Position = new Vector3(i + 1, 0, i + 1);
                    m2CCreateUnits.Units.Add(UnitHelper.CreateUnitInfo(monster));
                }
            }
            MessageHelper.SendToClient(unit,m2CCreateUnits);
            
        }
    }
}