using UnityEditor;

[CustomEditor(typeof (CComboBox), true)]
public class CComboBoxInspector : Editor
{
    public override void OnInspectorGUI()
    {
        CComboBox box = target as CComboBox;
        if (box == null) return;
        serializedObject.Update();
        MingUIEditorTools.DrawProperty("Button", serializedObject, "button");
        MingUIEditorTools.DrawProperty("ArrowSprite", serializedObject, "arrowSprite");
        
        MingUIEditorTools.DrawProperty("List", serializedObject, "list");
        MingUIEditorTools.DrawProperty("isFixListHeigth", serializedObject, "isFixListHeigth");
        MingUIEditorTools.DrawProperty("autoChooseIndex", serializedObject, "autoChooseIndex");
        MingUIEditorTools.DrawProperty("Style", serializedObject, "style");
        int index = (int)box.Pos;
        SerializedProperty sp = MingUIEditorTools.DrawProperty("Position", serializedObject, "_pos");
        if (sp.enumValueIndex != index)
        {
            box.Pos = (CComboBox.Position)sp.enumValueIndex;
        }
        MingUIEditorTools.DrawProperty("MaxHeight", serializedObject, "maxHeight");
        MingUIEditorTools.DrawProperty("Duration", serializedObject, "duration");
        serializedObject.ApplyModifiedProperties();
    }
}