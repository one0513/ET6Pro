namespace ET
{
    [FriendClass(typeof(CombatUnitComponent))]
    public class AfterCombatUnitGetDamage_PlayAnim:AEvent<EventType.AfterCombatUnitGetDamage>
    {
        protected override void Run(EventType.AfterCombatUnitGetDamage args)
        {
            var hpViewComponent = args.Unit.unit.GetComponent<HeadHpViewComponent>();
            if (hpViewComponent != null)
            {
                hpViewComponent.SetHp();
                args.Unit.unit.ZoneScene().GetComponent<FlyDamageValueViewComponent>().SpawnFlyDamage(args.Unit.unit.Position, args.DamageValue).Coroutine();
                args.Unit.unit.ZoneScene().GetComponent<FlyDamageValueViewComponent>().SpawnAtkFx(args.Unit.unit).Coroutine();
            }
            // else if(args.Unit.unit.GetComponent<NumericComponent>().GetAsInt(NumericType.Hp)<=0)//直接死了
            // {
            //     args.Unit.unit.Dispose();
            // }
            
        }
        
    }
}