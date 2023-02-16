using UnityEditor;

[CustomEditor(typeof (CSprite), true)]
public class CBaseRenderInspector : CSpriteInspector
{
    protected override void OnPreInspectorGUI()
    {
        base.OnPreInspectorGUI();
        MingUIEditorTools.DrawProperty("SelectHide", serializedObject, "selectHide");
        MingUIEditorTools.DrawProperty("SourceBg", serializedObject, "sourceBg");
        MingUIEditorTools.DrawProperty("SelectBg", serializedObject, "selectBg");
        MingUIEditorTools.DrawProperty("DefaultLabel", serializedObject, "defaultLabel");
        MingUIEditorTools.DrawProperty("RedTip", serializedObject, "redTip");
        MingUIEditorTools.DrawProperty("背景排列模式", serializedObject, "showBgAlternateMode");
    }
}