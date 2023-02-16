using System;
using System.IO;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

[CustomEditor(typeof(CTextureUV))]
public class CTextureUVInspector : RawImageEditor {
    private CTextureUV _texture;
    public override void OnInspectorGUI() {
        _texture = target as CTextureUV;
        if (_texture == null) return;
        serializedObject.Update();
        MingUIEditorTools.DrawProperty("AutoSnap", serializedObject, "autoSnap");
        MingUIEditorTools.DrawProperty("ShowWhiteSource", serializedObject, "showWhiteSource");
        serializedObject.ApplyModifiedProperties();
        if (Application.isEditor) {
            if (_texture.texture != null) {
                var texPath = AssetDatabase.GetAssetPath(_texture.texture);
                if (string.IsNullOrEmpty(texPath) == false && texPath.IndexOf("Assets/OriginalResource/", StringComparison.Ordinal) != -1) {
                    var path = texPath.Replace("Assets/OriginalResource/", "");
                    _texture.Path = path.Replace(Path.GetExtension(path), "");
                }
                EditorGUILayout.TextField("Path", _texture.Path);
            } else {
                _texture.Path = string.Empty;
            }
        }
        base.OnInspectorGUI();

        serializedObject.Update();
        MingUIEditorTools.DrawProperty("RaycastConsiderAlpha", serializedObject, "raycastConsiderAlpha");

        _texture.Gray = EditorGUILayout.Toggle("Gray", _texture.Gray);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_isLoopScaleMode"), new GUIContent("IsLoopScaleMode"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_isMove"), new GUIContent("IsMove"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_isUseTexSize"), new GUIContent("IsUseTexSize"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_moveSpeed"), new GUIContent("MoveSpeed"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_tileSize"), new GUIContent("TileSize"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_updateFps"), new GUIContent("UpdateFps"));
        if (GUILayout.Button("SetDefaultTile")) {
            _texture.SetTexSizeAsTileSize();
        }
        serializedObject.ApplyModifiedProperties();
    }
}