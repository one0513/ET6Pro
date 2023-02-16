//#define USE_ETC_1
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof (CAtlas))]
public class CAtlasInspector : Editor
{
    private CAtlas _atlas;
    private Sprite _sprite;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        _atlas = target as CAtlas;
        MingUIEditorTools.DrawProperty("MainTexture", serializedObject, "mainTexture");

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Sprite", "DropDown"))
        {
            SpriteSelector.Show(_atlas, _sprite != null ? _sprite.name : "", OnSelect);
        }
        if (GUILayout.Button("Edit", GUILayout.Width(40f)))
        {
            if (_atlas != null && _atlas.mainTexture != null)
            {
                SpriteEditor.Open(_atlas.mainTexture);
            }
        }
        GUILayout.EndHorizontal();

        //#define USE_ETC_1
        //MingUIEditorTools.DrawProperty("AlphaTexture", serializedObject, "alphaTexture");
        //GUILayout.BeginHorizontal();
        //if (GUILayout.Button("SpriteAlpha", "DropDown")) {
        //    SpriteSelector.Show(_atlas, _sprite != null ? _sprite.name : "", OnSelectAlpha, true);
        //}
        //if (GUILayout.Button("Edit", GUILayout.Width(40f))) {
        //    if (_atlas != null && _atlas.alphaTexture != null) {
        //        SpriteEditor.Open(_atlas.alphaTexture);
        //    }
        //}
        //GUILayout.EndHorizontal();
        //#endif

        GUILayout.Space(6f);
        if (!MingUIEditorTools.DrawHeader("Sprite Details")) return;
        MingUIEditorTools.BeginContents();

        EditorGUILayout.TextField("Name", _sprite != null ? _sprite.name : "");
        EditorGUILayout.TextField("Width", _sprite != null ? "" + _sprite.rect.width : "");
        EditorGUILayout.TextField("Height", _sprite != null ? "" + _sprite.rect.height : "");

        MingUIEditorTools.EndContents();

        serializedObject.ApplyModifiedProperties();
    }

    public override bool HasPreviewGUI()
    {
        return true;
    }

    public override void OnPreviewGUI(Rect rect, GUIStyle background)
    {
        if (_sprite == null) return;
        MingUIEditorTools.DrawSprite(_sprite, rect, Color.white);
    }

    private void OnSelect(string spriteName)
    {
        _sprite = _atlas.GetSprite(spriteName);
        Repaint();
    }

    //#define USE_ETC_1
    //private void OnSelectAlpha(string spriteName) {
    //    _sprite = _atlas.GetSprite(spriteName,true);
    //    Repaint();
    //}
    //#endif
}