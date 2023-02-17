namespace ET
{
    public enum AccountType
    {
        Genral = 0,
        
        BlackList = 1,
        
    }
    
    [ChildOf(typeof(Session))]
    public class Account : Entity,IAwake
    {
        public string accountName { get; set; }

        public string password{ get; set; }

        public long createTime{ get; set; }

        public int accountType{ get; set; }
    }
}