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
        public static Unit GetNearestMonster(this BattleUnitFindComponent self)
        {
            return null;
        }
        
#if SERVER
        public static async ETTask<bool> MonsterHasTarget(this BattleUnitFindComponent self)
        {

            RoomInfo info = await self.Parent.DomainScene().GetComponent<RoomInfoComponent>().Get(self.RoomId);
            foreach (var unitId in info.playerList)
            {
                if (self.Parent.DomainScene().GetComponent<UnitComponent>().Get(unitId) != null)
                {
                    return true;
                }
            }

            return false;
        }
#endif
        
#if SERVER
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
                    if (player != null)
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