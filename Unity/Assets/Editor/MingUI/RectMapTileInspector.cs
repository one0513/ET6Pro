using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

//[CustomEditor(typeof(RectMapTile), false)]
public class RectTileInspector : ImageEditor {

    //private RectMapTile _sprite;

    public override void OnInspectorGUI() {
        serializedObject.Update();
        OnPreInspectorGUI();
        serializedObject.ApplyModifiedProperties();

        base.OnInspectorGUI();

        serializedObject.Update();
        OnAfterInspectorGUI();
        serializedObject.ApplyModifiedProperties();
    }

    protected virtual void OnAfterInspectorGUI() {
        DrawProperty("MAX_TILE_X", serializedObject, "MAX_TILE_X");
        DrawProperty("MAX_TILE_Y", serializedObject, "MAX_TILE_Y");
        DrawProperty("WIDTH", serializedObject, "WIDTH");
        DrawProperty("HEIGHT", serializedObject, "HEIGHT");
        DrawProperty("defaultIndex", serializedObject, "defaultIndex");
        DrawProperty("哪种颜色值作为数据", serializedObject, "readPixelColor");
        DrawProperty("assetSprite", serializedObject, "assetSprite");
        DrawProperty("dataTex", serializedObject, "dataTex");
    }

    protected virtual void OnPreInspectorGUI() {
        // _sprite = target as RectMapTile;
        // if (_sprite == null) return;
    }

    public static SerializedProperty DrawProperty(string label, SerializedObject serializedObject, string property, params GUILayoutOption[] options)
    {
        return DrawProperty(label, serializedObject, property, false, options);
    }

    public static SerializedProperty DrawProperty(string label, SerializedObject serializedObject, string property, bool padding, params GUILayoutOption[] options)
    {
        SerializedProperty sp = serializedObject.FindProperty(property);

        if (sp != null)
        {
            if (padding) EditorGUILayout.BeginHorizontal();

            if (label != null) EditorGUILayout.PropertyField(sp, new GUIContent(label), true, options);
            else EditorGUILayout.PropertyField(sp, true, options);

            if (padding)
            {
                GUILayout.Space(18f);
                EditorGUILayout.EndHorizontal();
            }
        }
        return sp;
    }
}

// [CustomEditor(typeof(CFogTile), false)]
public class CFogTileInspector : ImageEditor
{

    //private CFogTile _sprite;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        OnPreInspectorGUI();
        serializedObject.ApplyModifiedProperties();

        base.OnInspectorGUI();

        serializedObject.Update();
        OnAfterInspectorGUI();
        serializedObject.ApplyModifiedProperties();
    }

    protected virtual void OnAfterInspectorGUI()
    {
        MingUIEditorTools.DrawProperty("MAX_TILE_X", serializedObject, "MAX_TILE_X");
        MingUIEditorTools.DrawProperty("MAX_TILE_Y", serializedObject, "MAX_TILE_Y");
        MingUIEditorTools.DrawProperty("USE_TILE_X", serializedObject, "USE_TILE_X");
        MingUIEditorTools.DrawProperty("USE_TILE_Y", serializedObject, "USE_TILE_Y");
        MingUIEditorTools.DrawProperty("WIDTH", serializedObject, "WIDTH");
        MingUIEditorTools.DrawProperty("HEIGHT", serializedObject, "HEIGHT");
        MingUIEditorTools.DrawProperty("fogTex", serializedObject, "fogTex");
        MingUIEditorTools.DrawProperty("offsetTile", serializedObject, "offsetTile");
    }

    protected virtual void OnPreInspectorGUI()
    {
        // _sprite = target as CFogTile;
        // if (_sprite == null) return;
    }
}
