namespace ET
{
    [FriendClass(typeof(ServerInfo))]
    public static class ServerInfoSystem
    {
        public static void FromMessage(this ServerInfo self, ServerInfoProto serverInfoProto)
        {
            self.status = serverInfoProto.Status;
            self.serverName = serverInfoProto.ServerName;
            self.Id = serverInfoProto.Id;
        }


        public static ServerInfoProto ToMessage(this ServerInfo self)
        {
            return new ServerInfoProto() {Id = (int)self.Id,ServerName = self.serverName,Status = self.status};
        }
        
    }
}