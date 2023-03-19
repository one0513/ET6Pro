using ET.EventType;

namespace ET
{
    public class AfterRelefeTimeChange_ChangeTime:AEvent<EventType.AfterRelefeTimeChange>
    {
        protected override void Run(AfterRelefeTimeChange args)
        {
            Unit unit = args.Scene.CurrentScene().GetComponent<UnitComponent>().Get(args.UnitId);
            var hpViewComponent = unit.GetComponent<HeadHpViewComponent>();
            if (hpViewComponent != null)
            {
                hpViewComponent.SetDieState(args.Time);
                hpViewComponent.SetHp();
                
            }
        }
    }
}