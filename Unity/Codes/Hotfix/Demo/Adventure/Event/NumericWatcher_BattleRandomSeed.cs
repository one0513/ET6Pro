using ET.EventType;

namespace ET
{
    [NumericWatcher(NumericType.BattleRandomSeed)]
    public class NumericWatcher_BattleRandomSeed : INumericWatcher
    {
        public void Run(NumbericChange args)
        {
            if (!(args.Parent is Unit unit))
            {
                return;
            }
            unit = args.Parent as Unit;
       
            unit.ZoneScene().CurrentScene().GetComponent<AdventureComponent>()?.SetBattleRandomSeed();
        }
    }
}