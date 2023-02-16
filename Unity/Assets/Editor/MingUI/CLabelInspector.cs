using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof (CLabel))]
public class CLabelInspector :TextEditor
{
    private CLabel _label;

    public override void OnInspectorGUI()
    {
        _label = target as CLabel;
        if (_label == null) return;
        base.OnInspectorGUI();

        serializedObject.Update();
        MingUIEditorTools.DrawProperty("RaycastConsiderAlpha", serializedObject, "raycastConsiderAlpha");
        MingUIEditorTools.DrawProperty("TextGap", serializedObject, "textGap");

        _label.Gray = EditorGUILayout.Toggle("Gray", _label.Gray);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_boldFont"), new UnityEngine.GUIContent("BoldFont"));
        serializedObject.ApplyModifiedProperties();
    }
}
