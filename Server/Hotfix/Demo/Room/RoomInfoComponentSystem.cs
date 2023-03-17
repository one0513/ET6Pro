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

        public static void Remove(this RoomInfoComponent self,long id)
        {
            self.RoomInfos.Remove(id);
        }

        public static RoomInfo[] GetAll(this RoomInfoComponent self)
        {
            return self.RoomInfos.Values.ToArray();
        }
    }
}