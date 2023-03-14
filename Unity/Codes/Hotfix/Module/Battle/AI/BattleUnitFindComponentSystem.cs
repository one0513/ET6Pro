namespace ET
{
    [ObjectSystem]
    public class BattleUnitFindComponentAwakeSystem:AwakeSystem<BattleUnitFindComponent>
    {
        public override void Awake(BattleUnitFindComponent self)
        {
            
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
    public static class BattleUnitFindComponentSystem
    {
        public static long FindTargetUnit(BattleUnitFindComponent self)
        {
 
            return 0;
        }
        
    }
}