using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

[CustomEditor(typeof (CMeshProColor))]
public class CMeshProColorInspector : Editor
{
    public CMeshProColor _color;
    public CMeshProColor _Color
    {
        get
        {
            if (_color == null)
                _color = target as CMeshProColor;
            return _color;
        }
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        MingUIEditorTools.DrawProperty("BColor", serializedObject, "bColors");
        MingUIEditorTools.DrawProperty("TColor", serializedObject, "tColors");
        MingUIEditorTools.DrawProperty("开启", serializedObject, "isEnable");
        MingUIEditorTools.DrawProperty("连续性", serializedObject, "isContinuity");
        _Color.Refresh();
        serializedObject.ApplyModifiedProperties();
    }
}
