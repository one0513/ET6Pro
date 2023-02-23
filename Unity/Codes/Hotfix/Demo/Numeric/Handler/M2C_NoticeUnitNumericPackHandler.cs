namespace ET
{
    // public class M2C_NoticeUnitNumericPackHandler: AMHandler<M2C_NoticeUnitNumericPack>
    // {
    //     protected override async ETTask Run(Session session, M2C_NoticeUnitNumericPack message)
    //     {
    //         NumericComponent numericComponent = session.ZoneScene()?.CurrentScene()?.GetComponent<UnitComponent>()?
    //                                                 .Get(message.UnitId)?.GetComponent<NumericComponent>();
    //         if (numericComponent == null)
    //         {
    //             return;
    //         }
    //
    //         foreach (M2C_NoticeUnitNumeric noticeUnitNumericMessage in message.NoticMessageList)
    //         {
    //             numericComponent.Set(noticeUnitNumericMessage.NumericType, noticeUnitNumericMessage.NewValue);
    //         }
    //         message.NoticMessageList.Clear();
    //         await ETTask.CompletedTask;
    //     }
    // }
}