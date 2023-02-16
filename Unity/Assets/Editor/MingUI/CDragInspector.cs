using UnityEditor;
using UnityEngine;

[CustomEditor(typeof (CDrag))]
public class CDragInspector : Editor
{
    public override void OnInspectorGUI()
    {
        CDrag drag = target as CDrag;
        serializedObject.Update();
        MingUIEditorTools.DrawProperty("DragMode", serializedObject, "mode");
        if (drag.mode == CDrag.DragMode.Clone)
        {
            MingUIEditorTools.DrawProperty("CloneScale", serializedObject, "cloneScale");
        }
        MingUIEditorTools.DrawProperty("Target", serializedObject, "target");
        MingUIEditorTools.DrawProperty("DragAble", serializedObject, "dragAble");
        MingUIEditorTools.DrawProperty("DragOnCenter", serializedObject, "dragOnCenter");
        MingUIEditorTools.DrawProperty("BoundTransfrom", serializedObject, "boundTransform");
        serializedObject.ApplyModifiedProperties();
    }

    private readonly Vector3[] _corners = new Vector3[4];

    public void OnSceneGUI()
    {
        CDrag drag = target as CDrag;
        if (drag != null && drag.boundTransform != null)
        {
            drag.boundTransform.GetWorldCorners(_corners);
            Rect rect = new Rect(_corners[0].x, _corners[0].y, _corners[2].x - _corners[0].x, _corners[2].y - _corners[0].y);
            Handles.DrawSolidRectangleWithOutline(rect, new Color(1, 1, 1, 0f), new Color(1, 0, 0, 1));
        }
    }
}