using System;
using System.IO;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

[CustomEditor(typeof(CTexture))]
public class CTextureInspector : RawImageEditor{
    private CTexture _texture;
    SerializedProperty m_alpha_Texture;

    protected override void OnEnable() {
        base.OnEnable();
        m_alpha_Texture = serializedObject.FindProperty("m_Texture_alpha");
    }

    public override void OnInspectorGUI(){
        _texture = target as CTexture;
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
        _texture.hasBorder = EditorGUILayout.Toggle("HasBorder", _texture.hasBorder);
        if (_texture.hasBorder){
            _texture.vBorder = EditorGUILayout.Vector2Field("vBorder", _texture.vBorder);
            _texture.hBorder = EditorGUILayout.Vector2Field("hBorder", _texture.hBorder);
        }
        //MingUIEditorTools.DrawProperty("Original Path", serializedObject, "oriPath");
        serializedObject.ApplyModifiedProperties();

        //通道分离
        serializedObject.Update();
        MingUIEditorTools.DrawProperty("透明通道分离", serializedObject, "useAlphaDepart");
        _texture.maskable = EditorGUILayout.Toggle("Maskable", _texture.maskable);
        if (_texture.useAlphaDepart) {
            EditorGUILayout.PropertyField(m_alpha_Texture);
        }
        serializedObject.ApplyModifiedProperties();
    }
}