using System;

namespace ET
{
    [FriendClass(typeof(Item))]
    [FriendClass(typeof(BagComponent))]
    public static class ItemFactory
    {
        public static Item Create(Entity parent, int configId, int count = 1)
        {
            if ( !ItemConfigCategory.Instance.Contain(configId))
            {
                Log.Error($"当前所创建的物品id 不存在: {configId}");
                return null;
            }
            Item item = parent.AddChild<Item, int>(configId);
            //item.RandomQuality();
            AddComponentByItemType(item);
            item.Count = count;
            if (ItemConfigCategory.Instance.Get(configId).Type == (int)ItemType.Item && !((BagComponent)parent).CfgAndItemIDDict.ContainsKey(configId))
            {
                ((BagComponent)parent).CfgAndItemIDDict.Add(configId,item.Id);
            }
            return item;
        }

        public static void AddComponentByItemType(Item item)
        {
            switch ((ItemType)item.Config.Type)
            {
     
                case ItemType.Equipment:
                {
                    item.AddComponent<EquipInfoComponent>();
                }
                    break;
                case ItemType.Item:
                {
                    
                }
                    break;
            }
        }

      
        
    }
}