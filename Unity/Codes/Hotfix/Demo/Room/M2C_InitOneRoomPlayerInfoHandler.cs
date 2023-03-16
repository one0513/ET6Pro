namespace ET.Room
{
    public class M2C_InitOneRoomPlayerInfoHandler: AMHandler<M2C_InitOneRoomPlayerInfo>
    {
        protected override void Run(Session session, M2C_InitOneRoomPlayerInfo message)
        {
            Unit otherUnit = UnitFactory.CreatePlayer(session.DomainScene().CurrentScene(), message.UnitInfo);
            
        }
    }
}