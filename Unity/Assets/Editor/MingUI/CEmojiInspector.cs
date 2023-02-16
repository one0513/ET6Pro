using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

[CustomEditor(typeof (CEmoji))]
public class CEmojiInspector : Editor
{
    private CEmoji _emoji;

    private string _tempName;
    private string _tempPrefix;
    private int _tempFps = 10;

    private string _selectKey;
    private int _selectFps;
    public override void OnInspectorGUI()
    {
        _emoji = target as CEmoji;
        serializedObject.Update();
        EditorGUIUtility.labelWidth = 60;
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

        if (atlas != null && _emoji != null)
        {
            if (MingUIEditorTools.DrawHeader("表情库"))
            {
                MingUIEditorTools.BeginContents();
                List<string> keyList = _emoji.keyList;
                List<string> valueList = _emoji.valueList;

                for (int i = 0; i < keyList.Count; i++)
                {
                    string key = keyList[i];
                    string value = valueList[i];
                    string[] arr = value.Split(CEmoji.SPLIT, 2);
                    string prefix = arr[0];
                    int fps = Convert.ToInt32(arr[1]);

                    GUILayout.BeginHorizontal();
                    GUILayout.Label(key, GUILayout.Width(40f));

                    prefix = EditorGUILayout.TextField("Prefix", prefix, GUILayout.MinWidth(40f));
                    if (GUILayout.Button("Sprite", "DropDown", GUILayout.Height(18f), GUILayout.MinWidth(20f)))
                    {
                        _selectKey = key;
                        _selectFps = fps;
                        SpriteSelector.Show(atlas, "", ChangeSprite);
                    }
                    fps = EditorGUILayout.IntField("Fps", fps, GUILayout.MinWidth(80));

                    valueList[i] = prefix + "#" + fps;

                    GUI.backgroundColor = Color.red;
                    if (GUILayout.Button("X", GUILayout.Width(22f)))
                    {
                        _emoji.RemoveEmoji(key);
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
                _tempName = EditorGUILayout.TextField(_tempName, GUILayout.Width(40f));
                _tempPrefix = EditorGUILayout.TextField("Prefix", _tempPrefix, GUILayout.MinWidth(40f));
                if (GUILayout.Button("Sprite", "DropDown", GUILayout.Height(18f), GUILayout.MinWidth(20f)))
                {
                    SpriteSelector.Show(atlas, "", SelectSprite);
                }
                _tempFps = EditorGUILayout.IntField("Fps", _tempFps, GUILayout.MinWidth(80));

                bool isValid = !string.IsNullOrEmpty(_tempName) && !string.IsNullOrEmpty(_tempPrefix);
                GUI.backgroundColor = isValid ? Color.green : Color.grey;
                if (GUILayout.Button("Add", GUILayout.Width(40f)) && isValid)
                {
                    _emoji.AddEmoji(_tempName, _tempPrefix, _tempFps);
                    _tempName = "";
                    _tempPrefix = "";
                    _tempFps = 10;
                    AssetDatabase.SaveAssets();
                }
                _emoji.MarkAsChanged();
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
        _emoji.atlas = obj as CAtlas;
        EditorUtility.SetDirty(_emoji);
    }

    private void ChangeSprite(string spriteName)
    {
        if (!string.IsNullOrEmpty(_selectKey) && _emoji != null)
        {
            _emoji.ChangeEmoji(_selectKey, spriteName, _selectFps);
            Repaint();
        }
    }

    private void SelectSprite(string spriteName)
    {
        _tempPrefix = spriteName;
        Repaint();
    }
}