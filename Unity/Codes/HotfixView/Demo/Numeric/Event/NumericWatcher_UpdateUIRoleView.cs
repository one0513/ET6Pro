using ET.EventType;

namespace ET
{
    [NumericWatcher(NumericType.Level)]
    [NumericWatcher(NumericType.AttributePoint)]
    [NumericWatcher(NumericType.Atk)]
    [NumericWatcher(NumericType.Def)]
    [NumericWatcher(NumericType.NewHp)]
    [NumericWatcher(NumericType.Dmg)]
    public class NumericWatcher_UpdateUIRoleView: INumericWatcher
    {
        public void Run(NumbericChange args)
        {
            UIManagerComponent.Instance.GetWindow<UIRoleView>()?.UpdateView();
        }
    }
}