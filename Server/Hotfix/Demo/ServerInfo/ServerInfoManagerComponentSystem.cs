using System;
using System.Collections.Generic;

namespace ET
{
    public class ServerInfoManagerComponentAwakeSystem : AwakeSystem<ServerInfoManagerComponent>
    {
        public override void Awake(ServerInfoManagerComponent self)
        {
            self.Awake().Coroutine();
        }
    }

    public class ServerInfoManagerComponentDestroySystem : DestroySystem<ServerInfoManagerComponent>
    {
        public override void Destroy(ServerInfoManagerComponent self)
        {
            foreach (var serverInfo in self.ServerInfos)
            {
                serverInfo?.Dispose();
            }
            self.ServerInfos.Clear();
        }
    }
    
    public class ServerInfoManagerComponentLoadSyetem : LoadSystem<ServerInfoManagerComponent>
    {
        public override void Load(ServerInfoManagerComponent self)
        {
            self.Awake().Coroutine();
        }
    }

    [FriendClass(typeof(ServerInfoManagerComponent))]
    [FriendClass(typeof(ServerInfo))]
    public static class ServerInfoManagerComponentSystem
    {
        public static async ETTask Awake(this ServerInfoManagerComponent self)
        {
            // var serverInfoList = await DBManagerComponent.Instance.GetZoneDB(self.DomainZone()).Query<ServerInfo>(d => true);
            // var severInfoConfigs = ServerInfoConfigCategory.Instance.GetAll();
            //
            // if (serverInfoList.Count < severInfoConfigs.Count )
            // {
            //     self.ServerInfos.Clear();
            //     foreach (var serverInfo in severInfoConfigs.Values)
            //     {
            //         ServerInfo newServerInfo = self.AddChildWithId<ServerInfo>(serverInfo.Id);
            //         newServerInfo.serverName = serverInfo.ServerName;
            //         newServerInfo.status = serverInfo.ServerStatus;
            //         self.ServerInfos.Add(newServerInfo);
            //         await DBManagerComponent.Instance.GetZoneDB(self.DomainZone()).Save(newServerInfo);
            //     }
            //     return;
            // }
            // self.ServerInfos.Clear();
            //
            // foreach (var serverInfo in serverInfoList)
            // {
            //     self.AddChild(serverInfo);
            //     self.ServerInfos.Add(serverInfo);
            // }
            
            var severInfoConfigs = ServerInfoConfigCategory.Instance.GetAll();
            self.ServerInfos.Clear();
            foreach (var serverInfo in severInfoConfigs.Values)
            {
                ServerInfo newServerInfo = self.AddChildWithId<ServerInfo>(serverInfo.Id);
                newServerInfo.serverName = serverInfo.ServerName;
                newServerInfo.status = serverInfo.ServerStatus;
                self.ServerInfos.Add(newServerInfo);
                await DBManagerComponent.Instance.GetZoneDB((int)newServerInfo.Id).Save(newServerInfo);
            }
            return;
        }
    }
}