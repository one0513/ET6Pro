namespace ET
{
    [ComponentOf(typeof(Unit))]
    public class BattleUnitFindComponent :Entity,IAwake,IDestroy
    {
        public Unit Target;
    }
}