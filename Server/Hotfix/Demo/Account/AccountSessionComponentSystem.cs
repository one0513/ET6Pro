namespace ET
{
    public class AccountSessionComponentDestorySystem : DestroySystem<AccountSessionComponent>
    {
        public override void Destroy(AccountSessionComponent self)
        {
            self.accountSessionDict.Clear();
        }
    }

    [FriendClass(typeof(AccountSessionComponent))]
    public static class AccountSessionComponentSystem
    {
        public static long Get(this AccountSessionComponent self, long accountId)
        {
            if (!self.accountSessionDict.TryGetValue(accountId,out long instanceId))
            {
                return 0;
            }

            return instanceId;
        }

        public static void Add(this AccountSessionComponent self, long accountId, long sessionInstanceId)
        {
            if (self.accountSessionDict.ContainsKey(accountId))
            {
                self.accountSessionDict[accountId] = sessionInstanceId;
                return;
            }
            self.accountSessionDict.Add(accountId,sessionInstanceId);
        }
        
        public static void Remove(this AccountSessionComponent self, long accountId)
        {
            if (self.accountSessionDict.ContainsKey(accountId))
            {
                self.accountSessionDict.Remove(accountId);
               
            }
        }
        
        
    }
}