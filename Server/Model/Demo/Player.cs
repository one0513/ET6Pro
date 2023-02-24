namespace ET
{
    public enum PlayerState
    {
        Disconnect,
        Gate,
        Game,
    }
	
    [ChildOf(typeof(PlayerComponent))]
    public sealed class Player : Entity,IAwake<string>, IAwake<long,long>
    {
        public string Accout { get; set; }
        public long AccountId { get;  set; }
		
        public long UnitId { get; set; }

        public PlayerState PlayerState { get; set; }

        public Session ClientSession { get; set; }

        public long ChatInfoInstanceId { get; set; }

    }
}