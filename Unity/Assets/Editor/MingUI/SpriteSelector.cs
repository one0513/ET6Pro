//#define USE_ETC_1
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SpriteSelector : ScriptableWizard
{
    public delegate void Callback(string sprite);

    public static SpriteSelector instance;

    private CAtlas _atlas; //当前选择的图集
    private string _selectSprite; //当前选择的图元
    private Callback _callback; //选择回调
    private Vector2 _pos = Vector2.zero;
    private float _clickTime;
    private bool _alpha = false;
    private Texture2D playModeTex;

    void OnEnable() {
        playModeTex = null;
        instance = this; 
    }
    void OnDisable() {
        playModeTex = null;
        instance = null; 
    }

    private static string _searchSprite = ""; //搜索的

    private void OnGUI()
    {
        EditorGUIUtility.labelWidth = 80;

        if (_atlas == null)
        {
            GUILayout.Label("No Atlas selected.", "LODLevelNotifyText");
            return;
        }
        GUILayout.Label(_atlas.name + " Sprites", "LODLevelNotifyText");
        MingUIEditorTools.DrawSeparator();
        GUILayout.BeginHorizontal();
        GUILayout.Space(84f);

        bool close = false;
        string before = _searchSprite;
        string after = EditorGUILayout.TextField("", before, "SearchTextField");
        _searchSprite = after;

        if (GUILayout.Button("", "SearchCancelButton", GUILayout.Width(18f)))
        {
            _searchSprite = "";
            GUIUtility.keyboardControl = 0;
        }
        GUILayout.Space(84f);
        GUILayout.EndHorizontal();

        Texture2D tex = null;
        if (Application.isPlaying) {
            if (playModeTex == null) {
                string oriTexPath = string.Format("Assets/OriginalResource/UI/{0}/Prefabs/{0}_Tex.png", _atlas.name, _atlas.name);
                playModeTex = AssetDatabase.LoadAssetAtPath<Texture2D>(oriTexPath);
            }
            tex = playModeTex;
        } else {
            if (_alpha) {
                //#define USE_ETC_1
                //if (_atlas.spriteAlphaList.Count > 0) {
                //    tex = _atlas.spriteAlphaList[0].texture as Texture2D;
                //}
                //#endif
            } else {
                if (_atlas.spriteList.Count > 0) {
                    tex = _atlas.spriteList[0].texture as Texture2D;
                }
            }
        }
        if (tex == null)
        {
            GUILayout.Label("该图集无关联贴图");
            return;
        }
        List<string> result = new List<string>();
        if (_alpha) {
            //#define USE_ETC_1
            //foreach (Sprite sp in _atlas.spriteAlphaList) {
            //    if (sp != null && sp.name.IndexOf(_searchSprite, StringComparison.Ordinal) != -1) {
            //        result.Add(sp.name);
            //    }
            //}
            //#endif
        } else {
            foreach (Sprite sp in _atlas.spriteList) {
                if (sp != null && sp.name.IndexOf(_searchSprite, StringComparison.Ordinal) != -1) {
                    result.Add(sp.name);
                }
            }
        }
        
        float size = 80f;
        float padded = size + 10f;
        int columns = Mathf.FloorToInt(this.position.width / padded);
        if (columns < 1) columns = 1;

        int offset = 0;
        Rect rect = new Rect(10f, 0, size, size);

        GUILayout.Space(10f);
        _pos = GUILayout.BeginScrollView(_pos);
        int rows = 1;
        while (offset < result.Count)
        {
            GUILayout.BeginHorizontal();
            {
                int col = 0;
                rect.x = 10f;
                for (; offset < result.Count; ++offset)
                {
                    Sprite sprite = _atlas.GetSprite(result[offset], _alpha);
                    if (sprite == null) continue;
                    if (GUI.Button(rect, ""))
                    {
                        if (Event.current.button == 0) //左键点击
                        {
                            float delta = Time.realtimeSinceStartup - _clickTime;
                            _clickTime = Time.realtimeSinceStartup;
                            if (_selectSprite != sprite.name) //点击了其他的sprite
                            {
                                _selectSprite = sprite.name;
                                if (_callback != null) _callback(sprite.name);
                            }
                            else if (delta < 0.5f) close = true;
                        }
                    }
                    if (Event.current.type == EventType.Repaint)
                    {
                        MingUIEditorTools.DrawTiledTexture(rect, MingUIEditorTools.BackdropTexture);
                        Rect uv = sprite.rect;
                        uv = ConvertToTexCoords(uv, tex.width, tex.height);

                        float scaleX = rect.width / uv.width;
                        float scaleY = rect.height / uv.height;
                        float aspect = scaleY / scaleX / ((float)tex.height / tex.width);
                        Rect clipRect = rect;
                        if (Math.Abs(aspect - 1f) > 0)
                        {
                            if (aspect < 1f)
                            {
                                float padding = size * (1f - aspect) * 0.5f;
                                clipRect.xMin += padding;
                                clipRect.xMax -= padding;
                            }
                            else
                            {
                                float padding = size * (1f - 1f / aspect) * 0.5f;
                                clipRect.yMin += padding;
                                clipRect.yMax -= padding;
                            }
                        }
                        GUI.DrawTextureWithTexCoords(clipRect, tex, uv);
                        if (_selectSprite == sprite.name)
                        {
                            MingUIEditorTools.DrawOutline(rect, new Color(0.4f, 1f, 0f, 1f));
                        }
                    }
                    GUI.backgroundColor = new Color(1f, 1f, 1f, 0.5f);
                    GUI.contentColor = new Color(1f, 1f, 1f, 0.7f);
                    GUI.Label(new Rect(rect.x, rect.y + rect.height, rect.width, 48f), sprite.name, "ProgressBarBack");
                    GUI.contentColor = Color.white;
                    GUI.backgroundColor = Color.white;

                    if (++col >= columns)
                    {
                        ++offset;
                        break;
                    }
                    rect.x += padded;
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(padded);
            rect.y += padded + 42;
            ++rows;
        }
        GUILayout.Space(rows * 42);
        GUILayout.EndScrollView();
        if (close) Close();
    }

    public static Rect ConvertToTexCoords(Rect rect, int width, int height)
    {
        Rect final = rect;

        if (width > 0f && height > 0f)
        {
            final.xMin = rect.xMin / width;
            final.xMax = rect.xMax / width;
            final.yMin = rect.yMin / height;
            final.yMax = rect.yMax / height;
        }
        return final;
    }

    public static void Show(CAtlas atlas, string spriteName, Callback callback,bool alpha = false)
    {
        if (instance != null)
        {
            instance.Close();
            instance = null;
        }

        SpriteSelector comp = DisplayWizard<SpriteSelector>("Select a Sprite");
        instance = comp;
        comp._selectSprite = spriteName;
        comp._callback = callback;
        comp._atlas = atlas;
        comp._alpha = alpha;
    }
}