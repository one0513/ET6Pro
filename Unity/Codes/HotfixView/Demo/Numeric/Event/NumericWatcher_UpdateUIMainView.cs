using ET.EventType;

namespace ET
{
    [NumericWatcher(NumericType.Level)]
    public class NumericWatcher_UpdateUIMainView : INumericWatcher
    {
        public void Run(NumbericChange args)
        {
            UIManagerComponent.Instance.GetWindow<UIMainView>()?.UpdateView();
        }
    }
}