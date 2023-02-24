using ET.EventType;

namespace ET
{
    [NumericWatcher(NumericType.Lv)]
    public class NumericWatcher_UpdateUIMainView : INumericWatcher
    {
        public void Run(NumbericChange args)
        {
            UIManagerComponent.Instance.GetWindow<UIMainView>()?.UpdateView();
        }
    }
}