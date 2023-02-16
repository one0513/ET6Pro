using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof (CProgressBar))]
public class CProgressBarInspector : SliderEditor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        MingUIEditorTools.DrawProperty("Label", serializedObject, "label");
        serializedObject.ApplyModifiedProperties();

        base.OnInspectorGUI();
    }
}