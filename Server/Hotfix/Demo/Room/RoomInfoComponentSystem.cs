using System.Collections.Generic;
using System.Linq;

namespace ET
{
    
    public class RoomInfoComponentAwakeSystem : AwakeSystem<RoomInfoComponent>
    {
        public override void Awake(RoomInfoComponent self)
        {
            
        }
    }


    public class RoomInfoComponentDestroySystem: DestroySystem<RoomInfoComponent>
    {
        public override void Destroy(RoomInfoComponent self)
        {
            
        }
    }
    [FriendClass(typeof(RoomInfoComponent))]
    [FriendClass(typeof(RoomInfo))]
    public static class RoomInfoComponentSystem
    {
        public static void Add(this RoomInfoComponent self, RoomInfo info)
        {
            self.RoomInfos.Add(info.Id, info);
            DBManagerComponent.Instance.GetZoneDB(self.DomainZone()).Save<RoomInfo>(info).Coroutine();
        }

        public static async ETTask<RoomInfo> Get(this RoomInfoComponent self,long roomId)
        {
            self.RoomInfos.TryGetValue(roomId, out RoomInfo info);
            if (info == null)
            {
                var infos = await DBManagerComponent.Instance.GetZoneDB(self.DomainZone()).Query<RoomInfo>(d => d.RoomId == roomId);
                if (infos.Count > 0)
                {
                    self.RoomInfos.Add(infos[0].Id, infos[0]);
                    return infos[0];
                }
            }
            return info;
        }
        
        public static  List<long> GetOnlinePlayerId(this RoomInfoComponent self,long roomId)
        {
            self.RoomInfos.TryGetValue(roomId, out RoomInfo info);
            if (info != null)
            {
                self.onlinePlayerIds.Clear();
                foreach (var playerId in info.playerList)
                {
                    if (self.DomainScene().GetComponent<UnitComponent>().Get(playerId) != null)
                    {
                        self.onlinePlayerIds.Add(playerId);
                    }
                }
            }
            return self.onlinePlayerIds;
        }

        

        public static void Remove(this RoomInfoComponent self,long id)
        {
            self.RoomInfos.Remove(id);
        }

        public static RoomInfo[] GetAll(this RoomInfoComponent self)
        {
            return self.RoomInfos.Values.ToArray();
        }

        public static void SaveRoomCurLevel(this RoomInfoComponent self, long roomId, int level)
        {
            self.RoomInfos.TryGetValue(roomId, out RoomInfo info);
            if (info != null && info.curLevel != level)
            {
                info.curLevel = level;
                DBManagerComponent.Instance.GetZoneDB(self.DomainZone()).Save<RoomInfo>(info).Coroutine();
            }
        }

        public static int GetRoomCurLevel(this RoomInfoComponent self, long roomId)
        {
            self.RoomInfos.TryGetValue(roomId, out RoomInfo info);
            if (info != null)
            {
                return info.curLevel;
            }
            return 1;
        }
        
    }
}