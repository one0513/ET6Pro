namespace ET
{
    
    public class EquipmentsComponentDestrory : DestroySystem<EquipmentsComponent>
    {
        public override void Destroy(EquipmentsComponent self)
        {
           self.Clear();
        }
    }

    [FriendClass(typeof(EquipmentsComponent))]
    public static class EquipmentsComponentSystem
    {
        public static void Clear(this EquipmentsComponent self)
        {
            ForeachHelper.Foreach(self.EquipItems, (index, item) => 
            {
                item?.Dispose();
            });
            self.EquipItems.Clear();
        }
        
        
        public static Item GetItemById(this EquipmentsComponent self, long itemId)
        {
            if (self.Children.TryGetValue(itemId,out Entity entity))
            {
                return entity as Item;
            }

            return null;
        }
        
        
        public static Item GetItemByPosition(this EquipmentsComponent self, EquipPosition equipPosition)
        {
            if (self.EquipItems.TryGetValue((int)equipPosition,out Item item))
            {
                return item;
            }

            return null;
        }

        public static void AddEquipItem(this EquipmentsComponent self, Item item)
        {
            if (self.EquipItems.TryGetValue(item.Config.EquipPosition,out Item equipItem))
            {
                Log.Error($"Already EquipItem in Postion{(EquipPosition) item.Config.EquipPosition}");
                return;
            }

            self.AddChild(item);
            self.EquipItems.Add(item.Config.EquipPosition,item);
        }
        
        public static bool IsEquipItemByPosition(this EquipmentsComponent self, EquipPosition equipPosition)
        {
            return self.EquipItems.ContainsKey((int)equipPosition);
        }
        
        public static bool IsEquipItemByItemId(this EquipmentsComponent self, long itemId)
        {
            foreach (var item in self.EquipItems.Values)
            {
                if (item.Id == itemId)
                {
                    return true;
                }
            }
            return false;
        }
        
        public static bool UnloadEquipItem(this EquipmentsComponent self, Item item)
        {
            self.EquipItems.Remove(item.Config.EquipPosition);
            item?.Dispose();
            return true;
        }
    }
}