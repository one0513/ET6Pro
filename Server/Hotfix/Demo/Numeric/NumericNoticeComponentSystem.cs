using ET.EventType;

namespace ET
{
    [FriendClass(typeof(NumericNoticeComponent))]
    public static class NumericNoticeComponentSystem
    {
        public static void NoticeImmediately(this NumericNoticeComponent self,NumbericChange args)
        {
            Unit unit = self.GetParent<Unit>();
            self.NoticeUnitNumericMessage.UnitId      = unit.Id;
            self.NoticeUnitNumericMessage.NumericType = args.NumericType;
            self.NoticeUnitNumericMessage.NewValue    = args.New;
            MessageHelper.SendToClient(unit,self.NoticeUnitNumericMessage);
        }
    }
}      
        
    
    
    
    
    
    
    
    
    



















        // public long Timer;
        //
        // public long LastNoticeTime;
        //         
        // public long NoticeTime;
        //         
        // public Queue<M2C_NoticeUnitNumeric> messageQueue = new Queue<M2C_NoticeUnitNumeric>();
    
        // public static void Notice(this NumericNoticeComponent self, NumbericChange args)
        // {
        //     if ( !ServerGlobalValue.NoticeImmediatelySet.Contains(args.NumericType) &&  
        //         self.LastNoticeTime > 0 && TimeHelper.ServerNow() - self.LastNoticeTime < 100)
        //     {
        //         self.EnqueueNoticeMessage(args);
        //         self.CheckNoticeTimer();
        //     }
        //     else
        //     {
        //         self.NoticeImmediately(args);
        //     }
        // }
        //
        // public static void EnqueueNoticeMessage(this NumericNoticeComponent self, NumbericChange args)
        // {
        //     M2C_NoticeUnitNumeric message = new M2C_NoticeUnitNumeric()
        //     { 
        //         UnitId = self.GetParent<Unit>().Id, NumericType = args.NumericType, NewValue = args.New
        //     };
        //     self.messageQueue.Enqueue(message);
        // }
        //
        // public static void CheckNoticeTimer(this NumericNoticeComponent self)
        // {
        //     if ( self.NoticeTime >= TimeHelper.ServerNow() )
        //     {
        //       return;
        //     }
        //     
        //     if ( self.Timer != 0 )
        //     {
        //         TimerComponent.Instance.Remove(ref self.Timer);
        //     }
        //     
        //     self.NoticeTime = TimeHelper.ServerNow() + 100;
        //     self.Timer = TimerComponent.Instance.NewOnceTimer(self.NoticeTime, TimerType.NoticeUnitNumericTime, self);
        // }
        //
        //
        //
        // public static void NoticePackMessage(this NumericNoticeComponent self)
        // {
        //     M2C_NoticeUnitNumericPack m2CNoticeUnitNumericPack = new M2C_NoticeUnitNumericPack();
        //     
        //     Unit unit = self.GetParent<Unit>();
        //     m2CNoticeUnitNumericPack.UnitId = unit.Id;
        //     
        //     int queueCount = self.messageQueue.Count;
        //     for ( int i = 0; i < queueCount; ++i )
        //     {
        //         M2C_NoticeUnitNumeric queueMsg = self.messageQueue.Dequeue();
        //         m2CNoticeUnitNumericPack.NoticMessageList.Add(queueMsg);
        //     }
        //
        //     if ( m2CNoticeUnitNumericPack.NoticMessageList.Count > 0 )
        //     {
        //         self.LastNoticeTime = TimeHelper.ServerNow();
        //         MessageHelper.SendToClient(unit,m2CNoticeUnitNumericPack);
        //     }
        // }
        
    // [Timer(TimerType.NoticeUnitNumericTime)]
    // public class UnitNumericNoticeTimerHandler: ATimer<NumericNoticeComponent>
    // {
    //     public override void Run(NumericNoticeComponent numericNoticeComponent)
    //     {
    //         numericNoticeComponent.NoticePackMessage();
    //     }
    // }
