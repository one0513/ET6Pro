using ET.EventType;

namespace ET
{
    public class NumericChangeEvent_NoticeToClient: AEventClass<EventType.NumbericChange>
    {
        protected override void Run(object a)
        {
            EventType.NumbericChange args = a as EventType.NumbericChange;
            if (!(args.Parent is Unit unit))
            {
                return;
            }

            //只允许通知玩家Unit
            if (unit.Type != UnitType.Player)
            {
                return;
            }
            unit.GetComponent<NumericNoticeComponent>()?.NoticeImmediately(args);

        }
    }
}