namespace ET
{
    [ComponentOf(typeof(Scene))]
    public class AccountInfoComponent : Entity ,IAwake,IDestroy
    {
        public string token { get; set; }
        public long accountId{ get; set; }
    }
}