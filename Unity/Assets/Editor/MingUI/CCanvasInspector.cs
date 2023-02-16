using UnityEditor;

[CustomEditor(typeof (CCanvas))]
public class CCanvasInspector : CContainerInspector
{
    protected override void OnPreInspectorGUI()
    {
        base.OnPreInspectorGUI();
        MingUIEditorTools.DrawProperty("HorizontalLimit", serializedObject, "horizontalLimit");
        MingUIEditorTools.DrawProperty("VerticalLimit", serializedObject, "verticalLimit");
        MingUIEditorTools.DrawProperty("AutoAlphaScrollBar", serializedObject, "autoAlphaScrollBar");
        MingUIEditorTools.DrawProperty("Ease", serializedObject, "ease");
        MingUIEditorTools.DrawProperty("EaseTime", serializedObject, "easeTime");
    }
}