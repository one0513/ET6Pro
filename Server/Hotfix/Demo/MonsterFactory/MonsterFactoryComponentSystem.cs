using System;
using System.Collections.Generic;
using UnityEngine;

namespace ET
{
    public class MonsterFactoryComponentDestroySystem: DestroySystem<MonsterFactoryComponent>
    {
        public override void Destroy(MonsterFactoryComponent self)
        {
            foreach (var unitIds in self.roomIdAndEnemyId.Values)
            {
                foreach (var unitId in unitIds)
                {
                    self.DomainScene().GetComponent<UnitComponent>().Remove(unitId);
                }
                
            }
            self.roomIdAndEnemyId.Clear();
        }
    }
    
    public class MonsterFactoryComponentAwakeSystem: AwakeSystem<MonsterFactoryComponent>
    {
        public override void Awake(MonsterFactoryComponent self)
        {
            
        }
    }
    
    
    [FriendClass(typeof(MonsterFactoryComponent))]
    [FriendClass(typeof(Unit))]
    public static class MonsterFactoryComponentSystem
    {
        
        //创建关卡怪物
        public static async ETTask CreateLevelMonster(this MonsterFactoryComponent self,long roomId,int levelId = 1)
        {
            //通知移除老关卡怪物
            M2C_RemoveUnits m2CRemoveUnits = new M2C_RemoveUnits();
            if (self.Get(roomId) != null)
            {
                foreach (var monsetrId in self.roomIdAndEnemyId[roomId])
                {
                    m2CRemoveUnits.Units.Add(monsetrId);
                }

                MessageHelper.RoomBroadcastAll(self.DomainScene().GetComponent<UnitComponent>().Get(self.roomIdAndEnemyId[roomId][0]),
                    m2CRemoveUnits);
                for (int i = 0; i < self.roomIdAndEnemyId[roomId].Count; i++)
                {
                    self.DomainScene().GetComponent<UnitComponent>().Remove(self.roomIdAndEnemyId[roomId][i]);
                }
                self.Remove(roomId);
            }
            
            //通知创建关卡新怪物
            BattleLevelConfig cfg = BattleLevelConfigCategory.Instance.Get(levelId);
            M2C_CreateUnits m2CCreateUnits = new M2C_CreateUnits();
            foreach (var cfgId in cfg.MonsterIds)
            {
                Unit monsterUnit = UnitFactory.CreateMonster(self.DomainScene(),roomId,cfgId);
                int r = RandomHelper.RandomNumber(0, self.randomPos.Count);
                monsterUnit.Position = self.randomPos[r];
                self.Add(roomId,monsterUnit.Id);
                monsterUnit.AddComponent<BattleUnitFindComponent, long>(roomId);
                m2CCreateUnits.Units.Add(UnitHelper.CreateUnitInfo(monsterUnit));
            }
            MessageHelper.RoomBroadcastAll(self.DomainScene().GetComponent<UnitComponent>().Get(self.roomIdAndEnemyId[roomId][0]),m2CCreateUnits);
            self.DomainScene().GetComponent<RoomInfoComponent>().SaveRoomCurLevel(roomId,levelId);
            
            await ETTask.CompletedTask;
        }
        
        public static async ETTask InitRoomMonster(this MonsterFactoryComponent self,long roomId,Unit unit)
        {
            if (roomId != 0)
            {
                if (self.Get(roomId) != null &&self.Get(roomId).Count >0)
                {
                    List<UnitInfo> unitInfos = new List<UnitInfo>();
                    M2C_CreateUnits m2CCreateUnits = new M2C_CreateUnits();
                    for (int i = 0; i < self.roomIdAndEnemyId[roomId].Count; i++)
                    {
                        unitInfos.Add(UnitHelper.CreateUnitInfo(self.DomainScene().GetComponent<UnitComponent>().Get(self.roomIdAndEnemyId[roomId][i])));
                    }
                    m2CCreateUnits.Units = unitInfos;
                    MessageHelper.SendToClient(unit,m2CCreateUnits);
                    await ETTask.CompletedTask;
                }
                else
                {
                    self.CreateLevelMonster(roomId,self.DomainScene().GetComponent<RoomInfoComponent>().GetRoomCurLevel(roomId)).Coroutine();
                }
            }
        }
        
        public static Unit CreateMonsterUnit(this MonsterFactoryComponent self,long roomId,int config = 1002)
        {
            Unit monsterUnit = UnitFactory.CreateMonster(self.DomainScene(),roomId,config);
            
            self.Add(roomId,monsterUnit.Id);
            return monsterUnit;
        }

        public static void Add(this MonsterFactoryComponent self, long roomId,long unitId)
        {
            if (self.roomIdAndEnemyId.ContainsKey(roomId))
            {
                self.roomIdAndEnemyId[roomId].Add(unitId);
            }
            else
            {
                self.roomIdAndEnemyId[roomId] = new List<long>(){unitId};
            }
        }
        
        public static void Remove(this MonsterFactoryComponent self, long roomId)
        {
            self.roomIdAndEnemyId.Remove(roomId);
        }
        
        public static void RemoveOneUnit(this MonsterFactoryComponent self, long roomId,long unitId)
        {
            if (self.roomIdAndEnemyId.ContainsKey(roomId))
            {
                self.roomIdAndEnemyId[roomId].Remove(unitId);
                self.DomainScene().GetComponent<UnitComponent>().Remove(unitId);
            }
        }
        
        public static List<long> Get(this MonsterFactoryComponent self, long roomId)
        {
            if (self.roomIdAndEnemyId.ContainsKey(roomId) && self.roomIdAndEnemyId[roomId].Count > 0)
            {
                return self.roomIdAndEnemyId[roomId];
            }

            return null;
        }
        
        public static async ETTask OnUnitDie(this MonsterFactoryComponent self, Unit unit)
        {
            long roomId = unit.GetComponent<NumericComponent>().GetAsLong(NumericType.RoomID);
            if (unit.Type == UnitType.Monster)
            {
                if (self.roomIdAndEnemyId[roomId].Count > 1)//还有怪物存活 不需要重新生成关卡
                {
                    
                    M2C_MonsterDie m2CRemoveUnits = new M2C_MonsterDie();
                    m2CRemoveUnits.UnitIds.Add(unit.Id);
                    MessageHelper.RoomBroadcastAll(self.DomainScene().GetComponent<UnitComponent>().Get(self.roomIdAndEnemyId[roomId][0]),
                        m2CRemoveUnits);
                    
                    //给房间内玩家分发经验
                    var list = self.DomainScene().GetComponent<RoomInfoComponent>().GetOnlinePlayerId(roomId);
                    for (int i = 0; i < list.Count; i++)
                    {
                        var num = self.DomainScene().GetComponent<UnitComponent>().Get(list[i]).GetComponent<NumericComponent>();
                        num[NumericType.Exp] += UnitConfigCategory.Instance.Get(unit.ConfigId).Drop / list.Count;
                    }
                    
                    self.DomainScene().GetComponent<UnitComponent>().Remove(unit.Id);
                    self.RemoveOneUnit(roomId,unit.Id);
                    
                    
                }
                else
                {
                    var list = self.DomainScene().GetComponent<RoomInfoComponent>().GetOnlinePlayerId(roomId);
                    for (int i = 0; i < list.Count; i++)
                    {
                        var num = self.DomainScene().GetComponent<UnitComponent>().Get(list[i]).GetComponent<NumericComponent>();
                        num[NumericType.Exp] += UnitConfigCategory.Instance.Get(unit.ConfigId).Drop / list.Count;
                    }
                    M2C_MonsterDie m2CRemoveUnits = new M2C_MonsterDie();
                    m2CRemoveUnits.UnitIds.Add(unit.Id);
                    MessageHelper.RoomBroadcastAll(self.DomainScene().GetComponent<UnitComponent>().Get(self.roomIdAndEnemyId[roomId][0]),
                        m2CRemoveUnits);
                    
                    self.DomainScene().GetComponent<UnitComponent>().Remove(unit.Id);
                    self.RemoveOneUnit(roomId,unit.Id);
                    
                    self.CreateLevelMonster(roomId,self.DomainScene().GetComponent<RoomInfoComponent>().GetRoomCurLevel(roomId)).Coroutine();
                }
            }
            else
            {
                var num = unit.GetComponent<NumericComponent>();
                if (num[NumericType.IsAlive] ==0)
                {
                    return;
                }
                num[NumericType.IsAlive] = 0;
                M2C_UpdatePlayerDieTime m2CUpdatePlayerDieTime = new M2C_UpdatePlayerDieTime();
                m2CUpdatePlayerDieTime.UnitId = unit.Id;
                
                int time = num.GetAsInt(NumericType.RestartTime);
                for (int i = time; i > -1; i--)
                {
                    m2CUpdatePlayerDieTime.relifeTime = i;
                    if (i ==0)
                    {
                        num[NumericType.IsAlive] = 1;
                        num.Set(NumericType.Hp,num[NumericType.MaxHp]);

                    }
                    MessageHelper.RoomBroadcastAll(unit, m2CUpdatePlayerDieTime);
                    await TimerComponent.Instance.WaitAsync(1000);
                }
            }

        }
        
    }
}