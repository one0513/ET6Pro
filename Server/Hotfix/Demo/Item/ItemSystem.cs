namespace ET
{
    public class ItemAwakeSystem: AwakeSystem<Item,int>
    {
        public override void Awake(Item self,int configID)
        {
            self.ConfigId = configID;
        }
    }
    
    
    public class ItemDestorySystem: DestroySystem<Item>
    {
        public override void Destroy(Item self)
        {
            self.Quality = 0;
            self.ConfigId = 0;
        }
    }
    
    
    [FriendClass(typeof(Item))]
    public static class ItemSystem
    {
        public static ItemInfo ToMessage(this Item self,bool isAllInfo = true)
        {
            ItemInfo itemInfo = new ItemInfo();
            itemInfo.ItemUid = self.Id;
            itemInfo.ItemConfigId = self.ConfigId;
            itemInfo.ItemQuality  = self.Quality;
            itemInfo.ItemAccount = self.Count;
            
            if (!isAllInfo)
            {
                return itemInfo;
            }
            
            EquipInfoComponent equipInfoComponent = self.GetComponent<EquipInfoComponent>();
            if (equipInfoComponent != null)
            {
                itemInfo.EquipInfo = equipInfoComponent.ToMessage();
            }

            return itemInfo;
        }
        
    }
}