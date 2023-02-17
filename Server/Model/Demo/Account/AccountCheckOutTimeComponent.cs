namespace ET
{
    [ComponentOf(typeof(Session))]
    public class AccountCheckOutTimeComponent : Entity,IAwake<long>,IDestroy
    {
        public long timer = 0;

        public long accountId = 0;
    }
}