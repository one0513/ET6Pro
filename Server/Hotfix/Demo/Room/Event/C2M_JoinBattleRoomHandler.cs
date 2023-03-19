using System;
using System.Collections.Generic;
using UnityEngine;

namespace ET
{
    [FriendClass(typeof(RoomInfo))]
    [FriendClass(typeof(AIComponent))]
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

            roomId = request.RoomId;
            unit.AddComponent<BattleUnitFindComponent, long>(roomId);
            unit.GetComponent<AIComponent>().Current = 0;
            info.playerList.Add(unit.Id);
            info.RoomPlayerNum += 1;
            num.Set(NumericType.RoomID,request.RoomId);

            reply();
            //初始化队友和怪物
            M2C_CreateUnits m2CCreateOtherUnits = new M2C_CreateUnits();
            foreach (var unitId in info.playerList)
            {
                Unit otherPlayer = unit.DomainScene().GetComponent<UnitComponent>().Get(unitId);
                if (unitId != unit.Id && otherPlayer != null)
                {
                    //通知在线队友有人加入队伍
                    M2C_CreateUnits m2CCreateMyUnit = new M2C_CreateUnits();
                    m2CCreateMyUnit.Units.Add(UnitHelper.CreateUnitInfo(unit));
                    MessageHelper.SendToClient(otherPlayer,m2CCreateMyUnit);
                    
                    m2CCreateOtherUnits.Units.Add(UnitHelper.CreateUnitInfo(otherPlayer));
                }
            }
            MessageHelper.SendToClient(unit,m2CCreateOtherUnits);
            
            List<long> monsterIds = unit.DomainScene().GetComponent<MonsterFactoryComponent>().Get(info.RoomId);
            if (monsterIds != null && monsterIds.Count > 0)
            {
                await unit.DomainScene().GetComponent<MonsterFactoryComponent>().InitRoomMonster(roomId,unit);
            }
            else //重启服务器 队友和怪物都被移除
            {
                await unit.DomainScene().GetComponent<MonsterFactoryComponent>().CreateLevelMonster(roomId, 1);
            }
            DBManagerComponent.Instance.GetZoneDB(unit.DomainZone()).Save<RoomInfo>(info).Coroutine();
            
        }
    }
}