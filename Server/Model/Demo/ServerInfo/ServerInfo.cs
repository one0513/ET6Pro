namespace ET
{
    public enum ServerStatus
    {
        Normal  = 0,
        Stop = 1,
    }
    
    [ChildOf(typeof(ServerInfoManagerComponent))]
    public class ServerInfo : Entity ,IAwake
    {
        public int status;
        public string serverName;
        
    }
}