using System;

namespace ET
{
    public static class ItemHelper
    {
        public static void Clear(Scene ZoneScene, ItemContainerType itemContainerType)
        {
            if (itemContainerType == ItemContainerType.Bag)
            {
                ZoneScene?.GetComponent<BagComponent>()?.Clear();
            }
            else if (itemContainerType == ItemContainerType.RoleInfo)
            {
                ZoneScene?.GetComponent<EquipmentsComponent>()?.Clear();
            }
        }
        
        public static Item GetItem(Scene ZoneScene, long itemId, ItemContainerType itemContainerType)
        {
            if (itemContainerType == ItemContainerType.Bag)
            {
                return ZoneScene.GetComponent<BagComponent>().GetItemById(itemId);
            }
            else if (itemContainerType == ItemContainerType.RoleInfo)
            {
                return ZoneScene.GetComponent<EquipmentsComponent>().GetItemById(itemId);
            }

            return null;
        }
        
        public static void AddItem(Scene ZoneScene, Item item, ItemContainerType itemContainerType)
        {
            if (itemContainerType == ItemContainerType.Bag)
            {
                ZoneScene.GetComponent<BagComponent>().AddItem(item);
            }
            else if (itemContainerType == ItemContainerType.RoleInfo)
            {
                 ZoneScene.GetComponent<EquipmentsComponent>().AddEquipItem(item);
            }
        }
        
        public static void RemoveItemById(Scene ZoneScene,  long itemId, ItemContainerType itemContainerType)
        {
            Item item = GetItem(ZoneScene,itemId, itemContainerType);
            if (itemContainerType == ItemContainerType.Bag)
            {
                ZoneScene.GetComponent<BagComponent>().RemoveItem(item);
            }
            else if (itemContainerType == ItemContainerType.RoleInfo)
            {
                ZoneScene.GetComponent<EquipmentsComponent>().UnloadEquipItem(item);
            }
        }
        
        public static void RemoveItem(Scene ZoneScene,  Item item, ItemContainerType itemContainerType)
        {
            if (itemContainerType == ItemContainerType.Bag)
            {
                ZoneScene.GetComponent<BagComponent>().RemoveItem(item);
            }
            else if (itemContainerType == ItemContainerType.RoleInfo)
            {
                ZoneScene.GetComponent<EquipmentsComponent>().UnloadEquipItem(item);
            }
        }
        
        

        
        
        
    }
}