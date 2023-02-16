using UnityEditor;
using UnityEngine;

[CustomEditor(typeof (CImageLabel))]
public class CImageLabelInspector : Editor
{
    private CImageLabel _label;

    public override void OnInspectorGUI()
    {
        _label = target as CImageLabel;
        if (_label == null) return;

        serializedObject.Update();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Font", "DropDown", GUILayout.Width(55f)))
        {
            ComponentSelector.Show<CFont>(OnSelectFont);
        }
        SerializedProperty sp = MingUIEditorTools.DrawProperty("", serializedObject, "_font", GUILayout.MinWidth(20f));
        CFont nowFont = sp.objectReferenceValue as CFont;
        _label.Font = nowFont;

        if (GUILayout.Button("Edit", GUILayout.Width(40f)))
        {
            if (nowFont != null)
            {
                Selection.activeGameObject = nowFont.gameObject;
            }
        }

        GUILayout.EndHorizontal();

        sp = MingUIEditorTools.DrawProperty("Algin", serializedObject, "_align", GUILayout.MinWidth(20f));
        _label.Align = (CImageLabel.AlignType)sp.enumValueIndex;

        sp = MingUIEditorTools.DrawProperty("Space", serializedObject, "_space", GUILayout.MinWidth(20f));
        _label.Space = sp.floatValue;

        if (Application.isPlaying||Application.isEditor)
        {
            _label.Text = EditorGUILayout.TextField("Text", _label.Text);
        }
        serializedObject.ApplyModifiedProperties();
    }

    private void OnSelectFont(Object obj)
    {
        serializedObject.Update();
        SerializedProperty sp = serializedObject.FindProperty("_font");
        sp.objectReferenceValue = obj;
        _label.Font = obj as CFont;
        serializedObject.ApplyModifiedProperties();
    }
}