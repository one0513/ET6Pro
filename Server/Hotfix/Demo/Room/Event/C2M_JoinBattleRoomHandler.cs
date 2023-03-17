using System;
using System.Collections.Generic;
using UnityEngine;

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
            //todo: 初始化队友和怪物
            M2C_CreateUnits m2CCreateUnits = new M2C_CreateUnits();
            foreach (var unitId in info.playerList)
            {
                Unit otherPlayer = unit.DomainScene().GetComponent<UnitComponent>().Get(unitId);
                if (unitId != unit.Id && otherPlayer != null)
                {
                    //通知在线队友有人加入队伍
                    M2C_CreateUnits m2CCreateMyUnit = new M2C_CreateUnits();
                    m2CCreateMyUnit.Units.Add(UnitHelper.CreateUnitInfo(unit));
                    MessageHelper.SendToClient(otherPlayer,m2CCreateMyUnit);
                    
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
            
            info.playerList.Add(unit.Id);
            info.RoomPlayerNum += 1;
            await DBManagerComponent.Instance.GetZoneDB(unit.DomainZone()).Save<RoomInfo>(info);
            
        }
    }
}