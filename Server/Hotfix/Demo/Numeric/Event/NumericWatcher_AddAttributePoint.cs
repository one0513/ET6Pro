using ET.EventType;

namespace ET
{

    [NumericWatcher(NumericType.PhysicalStrength)]
    [NumericWatcher(NumericType.Power)]
    public class NumericWatcher_AddAttributePoint : INumericWatcher
    {
        public void Run(EventType.NumbericChange args)
        {
            if (!(args.Parent is Unit unit))
            {
                return;
            }
            
            //力量+1点 伤害值+5
            if (args.NumericType == NumericType.Power)
            {
                unit.GetComponent<NumericComponent>()[NumericType.DamageValueAdd] += 5;
            }
            
            //体力+1点 最大生命值 +1%
            if (args.NumericType == NumericType.PhysicalStrength)
            {
                unit.GetComponent<NumericComponent>()[NumericType.MaxHpPct] += 1*10000;
            }
            

        }
    }
}