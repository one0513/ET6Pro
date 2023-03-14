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
    public static class RoomInfoComponentSystem
    {
        public static void Add(this RoomInfoComponent self, RoomInfo player)
        {
            self.RoomInfos.Add(player.Id, player);
        }

        public static RoomInfo Get(this RoomInfoComponent self,long id)
        {
            self.RoomInfos.TryGetValue(id, out RoomInfo gamer);
            return gamer;
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