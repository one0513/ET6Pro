using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof (CFont))]
public class CFontInspector : Editor
{
    private CFont _font;

    private string _tempKey;
    private string _tempSprite;

    private string _selectKey;

    public override void OnInspectorGUI()
    {
        _font = target as CFont;
        if (_font == null) return;
        serializedObject.Update();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Atlas", "DropDown", GUILayout.Width(55f)))
        {
            ComponentSelector.Show<CAtlas>(OnSelectAtlas);
        }

        SerializedProperty sp = MingUIEditorTools.DrawProperty("", serializedObject, "atlas", GUILayout.MinWidth(20f));

        CAtlas atlas = sp.objectReferenceValue as CAtlas;
        if (GUILayout.Button("Edit", GUILayout.Width(40f)))
        {
            if (atlas != null)
            {
                SpriteEditor.Open(atlas.mainTexture);
            }
        }
        GUILayout.EndHorizontal();
        serializedObject.ApplyModifiedProperties();

        if (atlas != null)
        {
            if (MingUIEditorTools.DrawHeader("艺术字库"))
            {
                MingUIEditorTools.BeginContents();

                List<string> keyList = _font.keyList;
                List<string> valueList = _font.valueList;
                for (int i = 0; i < keyList.Count;i++)
                {
                    string key = keyList[i];
                    string spriteName = valueList[i];
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(key, GUILayout.Width(100f));

                    if (GUILayout.Button(spriteName, "DropDown", GUILayout.Height(18f), GUILayout.MinWidth(20f)))
                    {
                        _selectKey = key;
                        SpriteSelector.Show(atlas, spriteName, ChangeSprite);
                    }

                    GUI.backgroundColor = Color.red;
                    if (GUILayout.Button("X", GUILayout.Width(22f)))
                    {
                        _font.RemoveSymbol(key);
                    }
                    GUI.backgroundColor = Color.white;
                    GUILayout.EndHorizontal();
                    GUILayout.Space(4f);
                }
                if (keyList.Count > 0)
                {
                    GUILayout.Space(6f);
                }
                GUILayout.BeginHorizontal();
                _tempKey = EditorGUILayout.TextField(_tempKey, GUILayout.Width(100f));

                if (GUILayout.Button(_tempSprite, "DropDown", GUILayout.Height(18f), GUILayout.MinWidth(20f)))
                {
                    SpriteSelector.Show(atlas, _tempSprite, SelectSprite);
                }

                bool isValid = !string.IsNullOrEmpty(_tempKey) && !string.IsNullOrEmpty(_tempSprite);
                GUI.backgroundColor = isValid ? Color.green : Color.grey;

                if (GUILayout.Button("Add", GUILayout.Width(40f)) && isValid)
                {
                    _font.AddSymbol(_tempKey, _tempSprite);
                    _tempKey = "";
                    _tempSprite = "";
                    AssetDatabase.SaveAssets();
                }
                GUI.backgroundColor = Color.white;
                GUILayout.EndHorizontal();

                if (keyList.Count == 0)
                {
                    EditorGUILayout.HelpBox("当前还没有艺术字，请自行添加", MessageType.Info);
                }
                else GUILayout.Space(4f);

                MingUIEditorTools.EndContents();
            }
        }
    }

    private void OnSelectAtlas(Object obj)
    {
        _font.atlas = obj as CAtlas;
        EditorUtility.SetDirty(_font);
    }

    private void ChangeSprite(string spriteName)
    {
        if (!string.IsNullOrEmpty(_selectKey) && _font != null)
        {
            _font.ChangeSymbol(_selectKey, spriteName);
            Repaint();
        }
    }

    private void SelectSprite(string spriteName)
    {
        _tempSprite = spriteName;
        Repaint();
    }
}