using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof (CContainer), true)]
public class CContainerInspector : ScrollRectEditor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        OnPreInspectorGUI();
        serializedObject.ApplyModifiedProperties();
        base.OnInspectorGUI();
    }

    protected virtual void OnPreInspectorGUI()
    {
        MingUIEditorTools.DrawProperty("Mask", serializedObject, "mask");
    }
}