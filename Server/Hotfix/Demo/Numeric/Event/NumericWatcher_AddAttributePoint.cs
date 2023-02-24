using ET.EventType;

namespace ET
{

    [NumericWatcher(NumericType.Atk)]
    [NumericWatcher(NumericType.Def)]
    [NumericWatcher(NumericType.Hp)]
    [NumericWatcher(NumericType.Dmg)]
    public class NumericWatcher_AddAttributePoint : INumericWatcher
    {
        public void Run(EventType.NumbericChange args)
        {
            if (!(args.Parent is Unit unit))
            {
                return;
            }
            
            //攻击防御 一点5战斗力 生命 一点1战斗力 伤害 一点30战斗力
            if (args.NumericType == NumericType.Atk || args.NumericType == NumericType.Def)
            {
                
                unit.GetComponent<NumericComponent>()[NumericType.CE] += (args.New -args.Old) * 5;
            }
            if (args.NumericType == NumericType.Hp)
            {
                unit.GetComponent<NumericComponent>()[NumericType.CE] += (args.New -args.Old) * 1;
            }
            if (args.NumericType == NumericType.Dmg)
            {
                unit.GetComponent<NumericComponent>()[NumericType.CE] += (args.New -args.Old) * 30;
            }

            

        }
    }
}