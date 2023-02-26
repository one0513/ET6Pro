using ET.EventType;

namespace ET
{
    [NumericWatcher(NumericType.IsAlive)]
    public class NumericWatcher_IsAliveAnimation : INumericWatcher
    {
        public void Run(NumbericChange args)
        {
            if (!(args.Parent is Unit unit))
            {
                return;
            }

            unit = args.Parent as Unit;
            
            if (args.New == 0)
            {
                unit?.GetComponent<AnimatorComponent>()?.Play(MotionType.Die);
            }
            else
            {
                unit?.GetComponent<AnimatorComponent>()?.Play(MotionType.Idle);
            }
        }
    }
}