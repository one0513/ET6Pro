using System;
using System.Numerics;

namespace ET
{
    [ObjectSystem]
    public class BattleUnitFindComponentAwakeSystem:AwakeSystem<BattleUnitFindComponent,long>
    {
        public override void Awake(BattleUnitFindComponent self,long roomId)
        {
            self.RoomId = roomId;
        }
    }
    [ObjectSystem]
    public class BattleUnitFindComponentDestroySystem : DestroySystem<BattleUnitFindComponent>
    {
        public override void Destroy(BattleUnitFindComponent self)
        {
            
        }
    }
    [FriendClass(typeof(BattleUnitFindComponent))]
    [FriendClass(typeof(RoomInfo))]
    public static class BattleUnitFindComponentSystem
    {
        
#if SERVER
        public static async ETTask<bool> MonsterHasTarget(this BattleUnitFindComponent self)
        {
            RoomInfo info = await self.Parent.DomainScene().GetComponent<RoomInfoComponent>().Get(self.RoomId);
            if (((Unit)self.Parent).Type == UnitType.Player)
            {
                var monsterIds = self.Parent.DomainScene().GetComponent<MonsterFactoryComponent>().Get(self.RoomId);
                return monsterIds != null && monsterIds.Count > 0;
            }
            else
            {
                foreach (var unitId in info.playerList)
                {
                    if (self.Parent.DomainScene().GetComponent<UnitComponent>().Get(unitId) != null)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        
        public static bool CheckHasLifePlayer(this BattleUnitFindComponent self)
        {
            var list = self.Parent.DomainScene().GetComponent<RoomInfoComponent>().GetOnlinePlayerId(self.RoomId);
            for (int i = 0; i < list.Count; i++)
            {
                bool state = self.Parent.DomainScene().GetComponent<UnitComponent>().
                        Get(list[i]).GetComponent<NumericComponent>().GetAsInt(NumericType.IsAlive) == 1;
                if (state)
                {
                    return true;
                }
            }

            return false;
        }
        
        public static bool PlayerCanAtk(this BattleUnitFindComponent self)
        {
            var monsterIds = self.Parent.DomainScene().GetComponent<MonsterFactoryComponent>().Get(self.RoomId);
            if (monsterIds == null || monsterIds.Count <= 0)
            {
                return false;
            }

            for (int i = 0; i < monsterIds.Count; i++)
            {
                Unit monsetrUnit = self.Parent.DomainScene().GetComponent<UnitComponent>().Get(monsterIds[i]);
                float rage = self.Parent.GetComponent<NumericComponent>().GetAsFloat(NumericType.ATKRage);
                float dis = Vector2.Distance(new Vector2(monsetrUnit.Position.x, monsetrUnit.Position.z),
                    new Vector2(((Unit)self.Parent).Position.x, ((Unit)self.Parent).Position.z));
                if (rage +1 > dis)
                {
                    return true;
                }
            }
            
            return false;
        }
#endif
        
#if SERVER
        public static Unit GetNearestMonsetr(this BattleUnitFindComponent self)
        {
            Unit nearestUnit = null;
            float nearDis = 10f;

            var monsterIds = self.Parent.DomainScene().GetComponent<MonsterFactoryComponent>().Get(self.RoomId);
            if (monsterIds == null || monsterIds.Count <= 0)
            {
                return null;
            }

            for (int i = 0; i < monsterIds.Count; i++)
            {
                Unit monsetrUnit = self.Parent.DomainScene().GetComponent<UnitComponent>().Get(monsterIds[i]);
                float rage = self.Parent.GetComponent<NumericComponent>().GetAsFloat(NumericType.ATKRage) +1f;//1f 为半径
                float dis = Vector2.Distance(new Vector2(monsetrUnit.Position.x, monsetrUnit.Position.z),
                    new Vector2(((Unit)self.Parent).Position.x, ((Unit)self.Parent).Position.z));
                if (rage > dis)
                {
                    if (nearDis > dis )
                    {
                        nearDis = dis;
                        nearestUnit = monsetrUnit;
                    }
                }
            }
            return nearestUnit;
        }
        
        public static Unit HasLifePlayer(this BattleUnitFindComponent self)
        {
            var list = self.Parent.DomainScene().GetComponent<RoomInfoComponent>().GetOnlinePlayerId(self.RoomId);
            for (int i = 0; i < list.Count; i++)
            {
                Unit playerUnit = self.Parent.DomainScene().GetComponent<UnitComponent>().Get(list[i]);
                bool state = playerUnit.GetComponent<NumericComponent>().GetAsInt(NumericType.IsAlive) == 1;
                if (state)
                {
                    return playerUnit;
                }
            }

            return null;
        }
        
        public static Unit HasLifeMonsetr(this BattleUnitFindComponent self)
        {

            var monsterIds = self.Parent.DomainScene().GetComponent<MonsterFactoryComponent>().Get(self.RoomId);
            if (monsterIds == null || monsterIds.Count <= 0)
            {
                return null;
            }

            for (int i = 0; i < monsterIds.Count; i++)
            {
                Unit monsetrUnit = self.Parent.DomainScene().GetComponent<UnitComponent>().Get(monsterIds[i]);
                bool state = monsetrUnit.GetComponent<NumericComponent>().GetAsInt(NumericType.IsAlive) == 1;
                if (state)
                {
                    return monsetrUnit;
                }

            }
            return null;
        }
        
        
        public static async ETTask<Unit> GetNearestPlayer(this BattleUnitFindComponent self)
        {
            Unit nearestUnit = null;
            float dis = 100f;

            if (self.RoomId != 0)
            {
                RoomInfo info = await self.Parent.DomainScene().GetComponent<RoomInfoComponent>().Get(self.RoomId);
                foreach (var unitId in info.playerList)
                {
                    Unit player = self.Parent.DomainScene().GetComponent<UnitComponent>().Get(unitId);
                    if (player != null && player.GetComponent<NumericComponent>().GetAsInt(NumericType.IsAlive) != 0)
                    {
                        float nowDis = Vector2.Distance(new Vector2(player.Position.x, player.Position.z),
                            new Vector2(((Unit)self.Parent).Position.x, ((Unit)self.Parent).Position.z));
                        if (nowDis <= dis)
                        {
                            nearestUnit = player;
                            dis = nowDis;
                        }
                    }
                }
            }

            return nearestUnit;
        }
#endif
    }

}