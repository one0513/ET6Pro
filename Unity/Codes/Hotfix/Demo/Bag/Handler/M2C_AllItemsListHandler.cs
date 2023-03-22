namespace ET
{
    public class M2C_AllItemsListHandler  : AMHandler<M2C_AllItemsList>
    {
        protected override void Run(Session session, M2C_AllItemsList message)
        {
            ItemHelper.Clear(session.ZoneScene(),(ItemContainerType)message.ContainerType);

            for (int i = 0; i < message.ItemInfoList.Count; i++)
            {
                Item item = ItemFactory.Create(session.ZoneScene(),message.ItemInfoList[i]);
                ItemHelper.AddItem(session.ZoneScene(),item,(ItemContainerType)message.ContainerType);
            }
        }
    }
}