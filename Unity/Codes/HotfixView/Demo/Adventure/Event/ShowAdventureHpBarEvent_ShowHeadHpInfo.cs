using System;

namespace ET
{
    public class ShowAdventureHpBarEvent_ShowHeadHpInfo: AEventClass<EventType.ShowAdventureHpBar>
    {
        protected override void Run(object a)
        {
            try
            {
                EventType.ShowAdventureHpBar args = a as EventType.ShowAdventureHpBar;
                args.Unit.GetComponent<HeadHpViewComponent>().SetVisible(args.isShow);
                args.Unit.GetComponent<HeadHpViewComponent>().SetHp().Coroutine();
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
            }
        }
    }
}