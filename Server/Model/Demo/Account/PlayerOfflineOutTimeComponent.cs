namespace ET
{
    [ComponentOf(typeof(Player))]
    public class PlayerOfflineOutTimeComponent : Entity,IAwake,IDestroy
    {
        public long Timer;
    }
}