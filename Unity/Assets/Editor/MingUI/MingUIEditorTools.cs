using System;
using UnityEditor;
using UnityEngine;

public class MingUIEditorTools
{
    /// <summary>
    /// 画一条线
    /// </summary>
    public static void DrawSeparator()
    {
        GUILayout.Space(12f);

        if (Event.current.type == EventType.Repaint)
        {
            Texture2D tex = EditorGUIUtility.whiteTexture;
            Rect rect = GUILayoutUtility.GetLastRect();
            GUI.color = new Color(0f, 0f, 0f, 0.25f);
            GUI.DrawTexture(new Rect(0f, rect.yMin + 6f, Screen.width, 4f), tex);
            GUI.DrawTexture(new Rect(0f, rect.yMin + 6f, Screen.width, 1f), tex);
            GUI.DrawTexture(new Rect(0f, rect.yMin + 9f, Screen.width, 1f), tex);
            GUI.color = Color.white;
        }
    }

    /// <summary>
    /// 画边框
    /// </summary>
    /// <param name="rect"></param>
    /// <param name="color"></param>
    public static void DrawOutline(Rect rect, Color color)
    {
        if (Event.current.type == EventType.Repaint)
        {
            Texture2D tex = EditorGUIUtility.whiteTexture;
            GUI.color = color;
            GUI.DrawTexture(new Rect(rect.xMin, rect.yMin, 1f, rect.height), tex);
            GUI.DrawTexture(new Rect(rect.xMax, rect.yMin, 1f, rect.height), tex);
            GUI.DrawTexture(new Rect(rect.xMin, rect.yMin, rect.width, 1f), tex);
            GUI.DrawTexture(new Rect(rect.xMin, rect.yMax, rect.width, 1f), tex);
            GUI.color = Color.white;
        }
    }

    /// <summary>
    /// 画下拉区域
    /// </summary>
    /// <param name="text"></param>
    /// <param name="key"></param>
    /// <param name="forceOn"></param>
    /// <returns></returns>
    public static bool DrawHeader(string text, string key = null, bool forceOn = false)
    {
        key = key == null ? text : key;
        bool state = EditorPrefs.GetBool(key, true);

        GUILayout.Space(3f);
        if (!forceOn && !state) GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f);
        GUILayout.BeginHorizontal();
        GUILayout.Space(3f);

        GUI.changed = false;
#if UNITY_3_5
		if (state) Text = "\u25B2 " + Text;
		else Text = "\u25BC " + Text;
		if (!GUILayout.Toggle(true, Text, "dragtab", GUILayout.MinWidth(20f))) state = !state;
#else
        text = "<b><Size=11>" + text + "</Size></b>";
        if (state) text = "\u25B2 " + text;
        else text = "\u25BC " + text;
        if (!GUILayout.Toggle(true, text, "dragtab", GUILayout.MinWidth(20f))) state = !state;
#endif
        if (GUI.changed) EditorPrefs.SetBool(key, state);

        GUILayout.Space(2f);
        GUILayout.EndHorizontal();
        GUI.backgroundColor = Color.white;
        if (!forceOn && !state) GUILayout.Space(3f);
        return state;
    }

    /// <summary>
    /// 画图元
    /// </summary>
    /// <param name="sprite"></param>
    /// <param name="drawArea"></param>
    /// <param name="color"></param>
    public static void DrawSprite(Sprite sprite, Rect drawArea, Color color)
    {
        if (sprite == null)
            return;

        Texture2D tex = sprite.texture;
        if (tex == null)
            return;

        Rect outer = sprite.rect;
        Rect inner = outer;
        inner.xMin += sprite.border.x;
        inner.yMin += sprite.border.y;
        inner.xMax -= sprite.border.z;
        inner.yMax -= sprite.border.w;

        Vector4 uv4 = UnityEngine.Sprites.DataUtility.GetOuterUV(sprite);
        Rect uv = new Rect(uv4.x, uv4.y, uv4.z - uv4.x, uv4.w - uv4.y);
        Vector4 padding = UnityEngine.Sprites.DataUtility.GetPadding(sprite);
        padding.x /= outer.width;
        padding.y /= outer.height;
        padding.z /= outer.width;
        padding.w /= outer.height;

        DrawSprite(tex, drawArea, padding, outer, inner, uv, color, null);
    }

    /// <summary>
    /// 开始内容
    /// </summary>
    public static void BeginContents()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Space(4f);
        EditorGUILayout.BeginHorizontal( GUILayout.MinHeight(10f));
        GUILayout.BeginVertical();
        GUILayout.Space(2f);
    }

    /// <summary>
    /// 结束内容
    /// </summary>
    public static void EndContents()
    {
        GUILayout.Space(3f);
        GUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(3f);
        GUILayout.EndHorizontal();
        GUILayout.Space(3f);
    }

    public static void HideMoveTool(bool hide)
    {
#if !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_1 && !UNITY_4_2 && !UNITY_4_3
        UnityEditor.Tools.hidden = hide && (UnityEditor.Tools.current == UnityEditor.Tool.Move);
#endif
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


    private static Texture2D _mBackdropTex;
    private static Texture2D _contrastTex;

    public static Texture2D BackdropTexture
    {
        get
        {
            if (_mBackdropTex == null)
                _mBackdropTex = CreateCheckerTex(
                    new Color(0.1f, 0.1f, 0.1f, 0.5f),
                    new Color(0.2f, 0.2f, 0.2f, 0.5f));
            return _mBackdropTex;
        }
    }

    private static Texture2D ContrastTexture
    {
        get
        {
            if (_contrastTex == null)
                _contrastTex = CreateCheckerTex(
                    new Color(0f, 0.0f, 0f, 0.5f),
                    new Color(1f, 1f, 1f, 0.5f));
            return _contrastTex;
        }
    }

    private static Texture2D CreateCheckerTex(Color c0, Color c1)
    {
        Texture2D tex = new Texture2D(16, 16);
        tex.name = "[Generated] Checker Texture";
        tex.hideFlags = HideFlags.DontSave;

        for (int y = 0; y < 8; ++y) for (int x = 0; x < 8; ++x) tex.SetPixel(x, y, c1);
        for (int y = 8; y < 16; ++y) for (int x = 0; x < 8; ++x) tex.SetPixel(x, y, c0);
        for (int y = 0; y < 8; ++y) for (int x = 8; x < 16; ++x) tex.SetPixel(x, y, c0);
        for (int y = 8; y < 16; ++y) for (int x = 8; x < 16; ++x) tex.SetPixel(x, y, c1);

        tex.Apply();
        tex.filterMode = FilterMode.Point;
        return tex;
    }

    private static void DrawSprite(Texture tex, Rect drawArea, Vector4 padding, Rect outer, Rect inner, Rect uv, Color color, Material mat)
    {
        // Create the texture rectangle that is centered inside rect.
        Rect outerRect = drawArea;
        outerRect.width = Mathf.Abs(outer.width);
        outerRect.height = Mathf.Abs(outer.height);

        if (outerRect.width > 0f)
        {
            float f = drawArea.width / outerRect.width;
            outerRect.width *= f;
            outerRect.height *= f;
        }

        if (drawArea.height > outerRect.height)
        {
            outerRect.y += (drawArea.height - outerRect.height) * 0.5f;
        }
        else if (outerRect.height > drawArea.height)
        {
            float f = drawArea.height / outerRect.height;
            outerRect.width *= f;
            outerRect.height *= f;
        }

        if (drawArea.width > outerRect.width)
            outerRect.x += (drawArea.width - outerRect.width) * 0.5f;

        // Draw the background
        EditorGUI.DrawTextureTransparent(outerRect, null, ScaleMode.ScaleToFit, outer.width / outer.height);

        // Draw the Image
        GUI.color = color;

        Rect paddedTexArea = new Rect(
            outerRect.x + outerRect.width * padding.x,
            outerRect.y + outerRect.height * padding.w,
            outerRect.width - outerRect.width * (padding.z + padding.x),
            outerRect.height - outerRect.height * (padding.w + padding.y)
            );

        if (mat == null)
        {
            GUI.DrawTextureWithTexCoords(paddedTexArea, tex, uv, true);
        }
        else
        {
            // NOTE: There is an issue in Unity that prevents it from clipping the drawn preview
            // using BeginGroup/EndGroup, and there is no way to specify a UV rect...
            EditorGUI.DrawPreviewTexture(paddedTexArea, tex, mat);
        }

        // Draw the border indicator lines
        GUI.BeginGroup(outerRect);
        {
            tex = ContrastTexture;
            GUI.color = Color.white;

            if (Math.Abs(inner.xMin - outer.xMin) > 0)
            {
                float x = (inner.xMin - outer.xMin) / outer.width * outerRect.width - 1;
                DrawTiledTexture(new Rect(x, 0f, 1f, outerRect.height), tex);
            }

            if (Math.Abs(inner.xMax - outer.xMax) > 0)
            {
                float x = (inner.xMax - outer.xMin) / outer.width * outerRect.width - 1;
                DrawTiledTexture(new Rect(x, 0f, 1f, outerRect.height), tex);
            }

            if (Math.Abs(inner.yMin - outer.yMin) > 0)
            {
                // GUI.DrawTexture is top-left based rather than bottom-left
                float y = (inner.yMin - outer.yMin) / outer.height * outerRect.height - 1;
                DrawTiledTexture(new Rect(0f, outerRect.height - y, outerRect.width, 1f), tex);
            }

            if (Math.Abs(inner.yMax - outer.yMax) > 0)
            {
                float y = (inner.yMax - outer.yMin) / outer.height * outerRect.height - 1;
                DrawTiledTexture(new Rect(0f, outerRect.height - y, outerRect.width, 1f), tex);
            }
        }

        GUI.EndGroup();
    }

    public static void DrawTiledTexture(Rect rect, Texture tex)
    {
        float u = rect.width / tex.width;
        float v = rect.height / tex.height;

        Rect texCoords = new Rect(0, 0, u, v);
        TextureWrapMode originalMode = tex.wrapMode;
        tex.wrapMode = TextureWrapMode.Repeat;
        GUI.DrawTextureWithTexCoords(rect, tex, texCoords);
        tex.wrapMode = originalMode;
    }
}