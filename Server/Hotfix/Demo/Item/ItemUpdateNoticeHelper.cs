
namespace ET
{
    [FriendClass(typeof(EquipmentsComponent))]
    [FriendClass(typeof(BagComponent))]
    [FriendClass(typeof(Item))]
    public static class ItemUpdateNoticeHelper
    {
        public static void SyncAddItem(Unit unit, Item item,  M2C_ItemUpdateOpInfo message)
        {
            message.ItemInfo = item.ToMessage();
            message.Op =(int)ItemOp.Add;
            MessageHelper.SendToClient(unit, message);
        }
        
        public static void SyncRemoveItem(Unit unit, Item item,M2C_ItemUpdateOpInfo message)
        {
            message.ItemInfo = item.ToMessage(false);
            message.Op = (int)ItemOp.Remove;
            MessageHelper.SendToClient(unit, message);
        }


        public static void SyncAllBagItems(Unit unit)
        {
            M2C_AllItemsList m2CAllItemsList = new M2C_AllItemsList(){ContainerType = (int)ItemContainerType.Bag};
            BagComponent bagComponent = unit.GetComponent<BagComponent>();
            foreach (var item in bagComponent.ItemsDict.Values)
            {
                m2CAllItemsList.ItemInfoList.Add(item.ToMessage());
            }

            MessageHelper.SendToClient(unit, m2CAllItemsList);
        }
        
        public static void SyncAllEquipItems(Unit unit)
        {
            M2C_AllItemsList m2CAllItemsList = new M2C_AllItemsList(){ContainerType = (int)ItemContainerType.RoleInfo};;
            EquipmentsComponent equipmentsComponent = unit.GetComponent<EquipmentsComponent>();
            foreach (var item in equipmentsComponent.EquipItems.Values)
            {
                m2CAllItemsList.ItemInfoList.Add(item.ToMessage());
            }
            MessageHelper.SendToClient(unit, m2CAllItemsList);
        }
        
        
    }
}