using ET.EventType;

namespace ET
{
    [NumericWatcher(NumericType.Lv)]
    [NumericWatcher(NumericType.AttributePoint)]
    [NumericWatcher(NumericType.Atk)]
    [NumericWatcher(NumericType.Def)]
    [NumericWatcher(NumericType.Hp)]
    [NumericWatcher(NumericType.Dmg)]
    [NumericWatcher(NumericType.CE)]
    public class NumericWatcher_UpdateUIRoleView: INumericWatcher
    {
        public void Run(NumbericChange args)
        {
            UIManagerComponent.Instance.GetWindow<UIRoleView>()?.UpdateView();
        }
    }
}