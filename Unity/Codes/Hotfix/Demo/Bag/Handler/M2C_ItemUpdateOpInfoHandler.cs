namespace ET
{
    public class M2C_ItemUpdateOpInfoHandler: AMHandler<M2C_ItemUpdateOpInfo>
    {
        protected override void Run(Session session, M2C_ItemUpdateOpInfo message)
        {
            if (message.Op ==(int)ItemOp.Add)
            {
                Item item = ItemFactory.GetOrCreate(session.ZoneScene(),message.ItemInfo);
                ItemHelper.AddItem(session.ZoneScene(),item,(ItemContainerType)message.ContainerType);
            }
            else if (message.Op == (int) ItemOp.Remove)
            {
                ItemHelper.RemoveItemById(session.ZoneScene(),message.ItemInfo.ItemUid,(ItemContainerType)message.ContainerType);
            }
        }
    }
}