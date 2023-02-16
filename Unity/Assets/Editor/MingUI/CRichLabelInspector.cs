using UnityEditor;
using UnityEngine;
using TextEditor = UnityEditor.UI.TextEditor;

[CustomEditor(typeof (CRichLabel))]
public class CRichLabelInspector : TextEditor
{
    public override void OnInspectorGUI()
    {
        CRichLabel label = target as CRichLabel;
        if (label == null) return;
        serializedObject.Update();
        MingUIEditorTools.DrawProperty("Algin", serializedObject, "_align", GUILayout.MinWidth(20f));
        if (Application.isPlaying)
        {
            label.HtmlText = EditorGUILayout.TextField(label.HtmlText, new GUIStyle("textarea"), GUILayout.MinHeight(100), GUILayout.ExpandHeight(true));
        }
        serializedObject.ApplyModifiedProperties();
        base.OnInspectorGUI();
    }
}