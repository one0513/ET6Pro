namespace ET
{
    [ComponentOf(typeof(Unit))]
    public class BattleUnitFindComponent :Entity,IAwake<long>,IDestroy
    {
        public long RoomId;
    }
}