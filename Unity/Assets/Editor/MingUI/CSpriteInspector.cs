using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

[CustomEditor(typeof (CSprite), true)]
public class CSpriteInspector : ImageEditor
{
    private CSprite _sprite;

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
        MingUIEditorTools.DrawProperty("RaycastConsiderAlpha", serializedObject, "raycastConsiderAlpha");
        _sprite.Gray = EditorGUILayout.Toggle("Gray", _sprite.Gray);
        _sprite.CheckAtlasSprite();
    }

    protected virtual void OnPreInspectorGUI()
    {
        _sprite = target as CSprite;
        if (_sprite == null) return;
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Atlas", "DropDown", GUILayout.Width(55f)))
        {
            ComponentSelector.Show<CAtlas>(OnSelectAtlas);
        }

        SerializedProperty sp = MingUIEditorTools.DrawProperty("", serializedObject, "_atlas", GUILayout.MinWidth(20f));
        CAtlas nowAtlas = sp.objectReferenceValue as CAtlas;
        _sprite.Atlas = nowAtlas;
        _sprite.RefreshAtlasName();
        if (_sprite.sprite == null){
            _sprite.SpriteName = null;
        }

        if (GUILayout.Button("Edit", GUILayout.Width(40f)))
        {
            if (nowAtlas != null)
            {
                SpriteEditor.Open(nowAtlas.mainTexture);
            }
        }
        GUILayout.EndHorizontal();
        ////////////////////////////////////////////////////////////////////////////////
        if (nowAtlas == null) //无图集
        {
            if (Application.isPlaying == false){
                OnSelectAtlas(null);
                SelectSprite(null);
            }
            else{
                GUILayout.BeginHorizontal();
                GUILayout.Label("运行状态下，不给设spritenamenull，资源管理相关" + _sprite.SpriteName, "HelpBox", GUILayout.Height(18f), GUILayout.MinWidth(20f));
                GUILayout.EndHorizontal();
            }
        }
        else
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Sprite", "DropDown", GUILayout.Width(55f)))
            {
                SpriteSelector.Show(nowAtlas, _sprite.SpriteName, SelectSprite);
            }

            GUILayout.Label(_sprite.SpriteName, "HelpBox", GUILayout.Height(18f), GUILayout.MinWidth(20f));

            if (GUILayout.Button("Edit", GUILayout.Width(40f)))
            {
                if (_sprite.sprite != null)
                {
                    SpriteEditor.Open(_sprite.sprite);
                }
            }
            GUILayout.EndHorizontal();
        }

        MingUIEditorTools.DrawProperty("AutoSnap", serializedObject, "autoSnap");
        MingUIEditorTools.DrawProperty("ShowWhiteSource", serializedObject, "showWhiteSource");
    }

    private void OnSelectAtlas(Object obj)
    {
        if(_sprite.Atlas != obj){
            _sprite.Atlas = obj as CAtlas;
            EditorUtility.SetDirty(_sprite);
        }
    }

    private void SelectSprite(string spriteName)
    {
        if(_sprite.SpriteName != spriteName){
            _sprite.SpriteName = spriteName;
            EditorUtility.SetDirty(_sprite);
        }

    }
}