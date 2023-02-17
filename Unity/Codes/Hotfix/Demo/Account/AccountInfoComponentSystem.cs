namespace ET
{
    public class AccountInfoComponentDestroySystem : DestroySystem<AccountInfoComponent>
    {
        public override void Destroy(AccountInfoComponent self)
        {
            self.token = string.Empty;
            self.accountId = 0;
        }
    }
    
    

    public static class AccountInfoComponentSystem
    {
        
    }
}