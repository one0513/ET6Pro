namespace ET
{
    public class M2C_NoticeUnitNumericHandler: AMHandler<M2C_NoticeUnitNumeric>
    {
        protected override  void Run(Session session, M2C_NoticeUnitNumeric message)
        {
            session.ZoneScene()?.CurrentScene()?.GetComponent<UnitComponent>()?
                    .Get(message.UnitId)?.GetComponent<NumericComponent>()?.Set(message.NumericType, message.NewValue);
   
        }
    }
}