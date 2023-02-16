using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof (CCheckBox))]
public class CCheckBoxInspector : ToggleEditor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        OnPreDrawProperty();
        serializedObject.ApplyModifiedProperties();
        base.OnInspectorGUI();
    }

    protected virtual void OnPreDrawProperty()
    {
        MingUIEditorTools.DrawProperty("Label", serializedObject, "label");
    }
}