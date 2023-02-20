using System;
using System.Collections.Generic;

namespace ET
{
    public class ServerInfosComponentDestroySystem : DestroySystem<ServerInfosComponent>
    {
        public override void Destroy(ServerInfosComponent self)
        {
            foreach (var info in self.serverInfoList)
            {
                info?.Dispose();
            }
            self.serverInfoList.Clear();
        }
    }

    [FriendClass(typeof(ServerInfosComponent))]
    [FriendClass(typeof(ServerInfo))]
    public static class ServerInfosComponentSystem
    {
        public static void Add(this ServerInfosComponent self, ServerInfo serverInfo)
        {
            self.serverInfoList.Add(serverInfo);
        }
        
        public static List<string> GetAllServerName(this ServerInfosComponent self)
        {
            List<string> serverNames = new List<string>();
            foreach (var info in self.serverInfoList)
            {
                serverNames.Add(info.serverName);
            }
            return serverNames;
        }
        
    }
}