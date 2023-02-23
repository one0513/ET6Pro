namespace ET
{
    [FriendClass(typeof(Player))]
    public static class PlayerSystem
    {
        [ObjectSystem]
        public class PlayerAwakeSystem : AwakeSystem<Player, long,long>
        {
            public override void Awake(Player self, long a,long roleId)
            {
                self.AccountId = a;
                self.UnitId = roleId;
            }
        }
        
        
        [ObjectSystem]
        public class PlayerDestroySystem : DestroySystem<Player>
        {
            public override void Destroy(Player self)
            {
                self.Parent.GetChild<Player>(self.AccountId)?.Dispose();
                self.AccountId = 0;
                self.UnitId    = 0;
                self.ChatInfoInstanceId = 0;
                self.PlayerState = PlayerState.Disconnect;
                self.ClientSession?.Dispose();
                
            }
        }
        
    }
}