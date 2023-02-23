using ET.EventType;

namespace ET
{
    [NumericWatcher(NumericType.Spirit)]
    [NumericWatcher(NumericType.Agile)]
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
            
            //敏捷+1点  闪避概率加0.1%
            if (args.NumericType == NumericType.Agile)
            {
                unit.GetComponent<NumericComponent>()[NumericType.DodgeFinalAdd] += 1 * 1000;
            }
            
            //精神+1点 最大法力值 +1%
            if (args.NumericType == NumericType.Spirit)
            {
                unit.GetComponent<NumericComponent>()[NumericType.MaxMpFinalPct] += 1 * 10000;
            }
        }
    }
}