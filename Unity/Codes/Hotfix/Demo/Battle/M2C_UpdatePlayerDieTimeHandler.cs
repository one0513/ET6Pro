namespace ET
{
    [MessageHandler]
    public class M2C_UpdatePlayerDieTimeHandler: AMHandler<M2C_UpdatePlayerDieTime>
    {
        protected override void Run(Session session, M2C_UpdatePlayerDieTime message)
        {
            EventSystem.Instance.Publish(new EventType.AfterRelefeTimeChange() {Time = message.relifeTime,UnitId = message.UnitId,Scene = session.ZoneScene()});
        }
    }
}