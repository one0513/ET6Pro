using UnityEditor;

[CustomEditor(typeof (CTabBar))]
public class CTabBarInspector : CCheckBoxInspector
{
    protected override void OnPreDrawProperty()
    {
        CTabBar bar = target as CTabBar;
        if (bar == null) return;
        base.OnPreDrawProperty();
        MingUIEditorTools.DrawProperty("RedTip", serializedObject, "redTip");
        MingUIEditorTools.DrawProperty("BackgroundObject", serializedObject, "backgroundObject");
        MingUIEditorTools.DrawProperty("CheckMarkObject", serializedObject, "checkMarkObject");
        MingUIEditorTools.DrawProperty("NormalLabelPos", serializedObject, "normalLabelPos");
        MingUIEditorTools.DrawProperty("SelectLabelPos", serializedObject, "selectLabelPos");
        MingUIEditorTools.DrawProperty("NormaltTipPos", serializedObject, "normaltTipPos");
        MingUIEditorTools.DrawProperty("SelectTipPos", serializedObject, "selectTipPos");
        MingUIEditorTools.DrawProperty("NomalLabelColor", serializedObject, "nomalLabelColor");
        MingUIEditorTools.DrawProperty("SelectLabelColor", serializedObject, "selectLabelColor");
        MingUIEditorTools.DrawProperty("NomalLabelOutLineColor", serializedObject, "nomalLabelOutLineColor");
        MingUIEditorTools.DrawProperty("SelectLabeOutLinelColor", serializedObject, "selectLabeOutLinelColor");
        MingUIEditorTools.DrawProperty("NomalLabelOutLineWidth ", serializedObject, "nomalLabelOutLineWidth");
        MingUIEditorTools.DrawProperty("SelectLabeOutLinelWidth", serializedObject, "selectLabeOutLinelWidth");
        bool openEffect = bar.OpenButtonEffect;
        SerializedProperty sp = MingUIEditorTools.DrawProperty("OpenButtonEffect", serializedObject, "_openButtonEffect");
        if (sp.boolValue != openEffect)
        {
            bar.OpenButtonEffect = sp.boolValue;
        }
        MingUIEditorTools.DrawProperty("IsHideBgOnSelected", serializedObject, "_isHideBgOnSelected");
    }
}