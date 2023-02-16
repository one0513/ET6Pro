using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof (CButton))]
public class CButtonInspector : ButtonEditor
{
    public override void OnInspectorGUI()
    {
        CButton button = target as CButton;
        if (button == null) return;
        serializedObject.Update();
        MingUIEditorTools.DrawProperty("Label", serializedObject, "label");
        MingUIEditorTools.DrawProperty("Icon", serializedObject, "icon");
        MingUIEditorTools.DrawProperty("RedTip", serializedObject, "redTip");
        MingUIEditorTools.DrawProperty("IsCanSetGray", serializedObject, "isCanSetGray");
        MingUIEditorTools.DrawProperty("clickTimeProtect", serializedObject, "clickTimeProtect");
        MingUIEditorTools.DrawProperty("longClickThreshold", serializedObject, "longClickThreshold");
        
        bool isEnabled = button.IsEnabled;
        SerializedProperty sp = MingUIEditorTools.DrawProperty("IsEnabled", serializedObject, "_isEnabled");
        if (sp.boolValue != isEnabled)
        {
            button.IsEnabled = sp.boolValue;
            EditorUtility.SetDirty(button);
            serializedObject.Update();
        }

        bool openEffect = button.OpenButtonEffect;
        sp = MingUIEditorTools.DrawProperty("OpenButtonEffect", serializedObject, "_openButtonEffect");
        if (sp.boolValue != openEffect)
        {
            button.OpenButtonEffect = sp.boolValue;
        }

        serializedObject.ApplyModifiedProperties();

        base.OnInspectorGUI();
    }
}