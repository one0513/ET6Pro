using UnityEditor;

[CustomEditor(typeof (CLoopList))]
public class CLoopListInspector : CListInspector
{
    protected override void OnOwnInspectorGUI()
    {
        MingUIEditorTools.DrawProperty("Mode", serializedObject, "mode");
        MingUIEditorTools.DrawProperty("LerpScale", serializedObject, "lerpScale");
        MingUIEditorTools.DrawProperty("Pad", serializedObject, "pad");
        MingUIEditorTools.DrawProperty("ItemScale", serializedObject, "itemScale");
        MingUIEditorTools.DrawProperty("MaxSelectNum", serializedObject, "maxSelectNum");
        SerializedProperty sp = MingUIEditorTools.DrawProperty("DragMode", serializedObject, "dragMode");
        if (sp.enumValueIndex > 0)
        {
            MingUIEditorTools.DrawProperty("DragThreshold", serializedObject, "dragThreshold");
        }
        MingUIEditorTools.DrawProperty("RenderPrefab", serializedObject, "renderPrefab");
    }
}