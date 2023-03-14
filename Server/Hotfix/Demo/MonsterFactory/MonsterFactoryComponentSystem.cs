using System;
using System.Collections.Generic;

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
    public static class MonsterFactoryComponentSystem
    {

        public static Unit CreateMonsterUnit(this MonsterFactoryComponent self,long roomId)
        {
            Unit monsterUnit = UnitFactory.CreateMonster(self.DomainScene(),1002);
            
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
        
        public static void Remove(this MonsterFactoryComponent self, long roomId,long unitId)
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
    }
}