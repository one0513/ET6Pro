//#define USE_GRRY_UNIT
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 文本
/// </summary>
[AddComponentMenu("MingUI/CLabel", 6)]
public class CLabel : Text, ICanvasRaycastFilter, IGray {
    private float _groupAlpha = 1;
    private bool _gray;
    private int _maxLineGapCount = 0;

    [SerializeField]
    protected bool _boldFont;

    public bool raycastConsiderAlpha = false; //自身透明度是否影响事件？默认不影响：即alpha即使为0，也可以接受点击等事件
    public float textGap = 0.0f;

    private UIEventListener.Vector2Delegate _onSizeChange;

    /// <summary>
    /// 实际渲染的宽度，包括额外添加的字间距
    /// </summary>
    public float RendWidth {
        get {
            return preferredWidth + textGap * Mathf.Max(0, _maxLineGapCount - 1);
        }
    }

    public bool BoldFont {
        get { return _boldFont; }
        set {
            if (value != _boldFont) {
                _boldFont = value;
                SetLayoutDirty();
            }
        }
    }

    /// <summary>
    /// 宽度
    /// </summary>
    public float Width {
        get { return Size.x; }
        set { Size = new Vector2(value, Size.y); }
    }

    /// <summary>
    /// 高度
    /// </summary>
    public float Height {
        get { return Size.y; }
        set { Size = new Vector2(Size.x, value); }
    }

    public void ValidSize() {
        LayoutRebuilder.ForceRebuildLayoutImmediate(this.rectTransform);
    }

    /// <summary>
    /// 大小
    /// </summary>
    public Vector2 Size {
        get { return rectTransform.rect.size; }
        set {
            if (value != rectTransform.rect.size) {
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, value.x);
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, value.y);
            }
        }
    }

    /// <summary>
    /// 透明度
    /// </summary>
    public float Alpha {
        get { return color.a; }
        set {
            if (Math.Abs(color.a - value) > 0) {
                Color temp = color;
                temp.a = value;
                color = temp;
            }
        }
    }

    #if USE_GRRY_UNIT
    protected Color unGreyColor;
    protected string unGreyTex;
    #endif
    /// <summary>
    /// 灰度化？
    /// </summary>
    public bool Gray {
        get { return _gray; }
        set {
            if (value != _gray) {
                _gray = value;
                #if USE_GRRY_UNIT
                if (_gray) {
                    unGreyColor = color;
                    color = GreyMaterialManager.greyTextColor;
                    unGreyTex = text;
                    base.text = HtmlUtil_MingUI.ClearColor(text);
                } else {
                    color = unGreyColor;
                    base.text = unGreyTex;
                }
#else
                if (_gray) {
                    material = GreyMaterialManager.GetGreyMaterial();
                } else {
                    material = null;
                }
#endif

            }
        }
    }

    #if USE_GRRY_UNIT
    public override string text {
        get {
            return base.text;
        }
        set {
            if (_gray) {
                unGreyTex = value;
                base.text = HtmlUtil_MingUI.ClearColor(value);
            } else {
                base.text = value;
            }
        }
    }
    #endif

    /// <summary>
    /// 添加面积改变
    /// </summary>
    /// <param name="callback"></param>
    public void AddSizeChange(UIEventListener.Vector2Delegate callback) {
        this.UpdateGeometry();
        _onSizeChange = callback;
    }
    /// <summary>
    /// 移除面积改变
    /// </summary>
    public void RemoveSizeChange() {
        _onSizeChange = null;
    }

    protected override void OnDestroy() {
        base.OnDestroy();
        _onSizeChange = null;
    }
    protected override void Awake() {
        base.Awake();
        this.RegisterDirtyVerticesCallback(OnVerticesChange);
    }

    private void OnVerticesChange() {
        if (_onSizeChange != null) _onSizeChange(Size);
    }
    /// <summary>
    /// 这里重写是因为UGUI在响应事件时没有计算透明度
    /// </summary>
    /// <param name="screenPoint"></param>
    /// <param name="eventCamera"></param>
    /// <returns></returns>
    public virtual bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera) {
        if (raycastConsiderAlpha && (color.a <= 0f || _groupAlpha <= 0f)) {
            return false;
        }
        return true;
    }

    /// <summary>
    /// 当CanvasGroupChange时派发
    /// </summary>
    protected override void OnCanvasGroupChanged() {
        base.OnCanvasGroupChanged();
        if (raycastConsiderAlpha) {
            _groupAlpha = MingUIUtil.GetGroupAlpha(gameObject);
        }
    }

    protected readonly UIVertex[] m_TempVerts = new UIVertex[4];
    protected override void OnPopulateMesh(VertexHelper toFill) {
        if (font == null)
            return;
        if (Math.Abs(textGap) > 0) {
            ProcessTextGap(toFill);
        } else {
            ProcessText(toFill);
        }
    }

    protected override void UpdateMaterial() {
        base.UpdateMaterial();
        canvasRenderer.SetAlphaTexture(Texture2D.whiteTexture);
    }

    #region 顶点网格处理
    private void ProcessText(VertexHelper toFill) {
        if (font == null)
            return;

        // We don't care if we the font Texture changes while we are doing our Update.
        // The end result of cachedTextGenerator will be valid for this instance.
        // Otherwise we can get issues like Case 619238.
        m_DisableFontTextureRebuiltCallback = true;

        Vector2 extents = rectTransform.rect.size;

        var settings = GetGenerationSettings(extents);
        cachedTextGenerator.PopulateWithErrors(text, settings, gameObject);

        // Apply the offset to the vertices
        IList<UIVertex> verts = cachedTextGenerator.verts;
        float unitsPerPixel = 1 / pixelsPerUnit;
        int vertCount = verts.Count;

        // We have no verts to process just return (case 1037923)
        if (vertCount <= 0) {
            toFill.Clear();
            return;
        }

        Vector2 roundingOffset = new Vector2(verts[0].position.x, verts[0].position.y) * unitsPerPixel;
        roundingOffset = PixelAdjustPoint(roundingOffset) - roundingOffset;
        toFill.Clear();
        if (roundingOffset != Vector2.zero) {
            for (int i = 0; i < vertCount; ++i) {
                int tempVertsIndex = i & 3;
                m_TempVerts[tempVertsIndex] = verts[i];
                m_TempVerts[tempVertsIndex].position *= unitsPerPixel;
                m_TempVerts[tempVertsIndex].position.x += roundingOffset.x;
                m_TempVerts[tempVertsIndex].position.y += roundingOffset.y;
                if (tempVertsIndex == 3)
                    toFill.AddUIVertexQuad(m_TempVerts);
            }
        } else {
            for (int i = 0; i < vertCount; ++i) {
                int tempVertsIndex = i & 3;
                m_TempVerts[tempVertsIndex] = verts[i];
                m_TempVerts[tempVertsIndex].position *= unitsPerPixel;
                if (tempVertsIndex == 3)
                    toFill.AddUIVertexQuad(m_TempVerts);
            }
        }

        m_DisableFontTextureRebuiltCallback = false;
    }

    private static Dictionary<int, int> fontWidthDict = new Dictionary<int, int>(); 
    public float GetCharWidth(int ch) {
        if (!fontWidthDict.ContainsKey(ch)) {
            var c = (char)ch;
            CharacterInfo info;
            if (font.GetCharacterInfo(c, out info, fontSize)) {
                fontWidthDict[ch] = info.advance;
            } else {
                fontWidthDict[ch] = fontSize;
            }
        }
        return fontWidthDict[ch];
    }

    private List<float> offset = new List<float>();
    private void ProcessTextGap(VertexHelper toFill) {
        if (font == null)
            return;

        // We don't care if we the font Texture changes while we are doing our Update.
        // The end result of cachedTextGenerator will be valid for this instance.
        // Otherwise we can get issues like Case 619238.
        m_DisableFontTextureRebuiltCallback = true;

        Vector2 extents = rectTransform.rect.size;

        var settings = GetGenerationSettings(extents);
        cachedTextGenerator.PopulateWithErrors(text, settings, gameObject);

        // Apply the offset to the vertices
        IList<UIVertex> verts = cachedTextGenerator.verts;
        float unitsPerPixel = 1 / pixelsPerUnit;
        int vertCount = verts.Count;

        // We have no verts to process just return (case 1037923)
        if (vertCount <= 0) {
            toFill.Clear();
            return;
        }

        Vector2 roundingOffset = new Vector2(verts[0].position.x, verts[0].position.y) * unitsPerPixel;
        roundingOffset = PixelAdjustPoint(roundingOffset) - roundingOffset;
        toFill.Clear();
        if (roundingOffset != Vector2.zero) {
            for (int i = 0; i < vertCount; ++i) {
                int tempVertsIndex = i & 3;
                m_TempVerts[tempVertsIndex] = verts[i];
                m_TempVerts[tempVertsIndex].position *= unitsPerPixel;
                m_TempVerts[tempVertsIndex].position.x += roundingOffset.x;
                m_TempVerts[tempVertsIndex].position.y += roundingOffset.y;
                if (tempVertsIndex == 3)
                    toFill.AddUIVertexQuad(m_TempVerts);
            }
        } else {
            for (int i = 0; i < vertCount; ++i) {
                int tempVertsIndex = i & 3;
                m_TempVerts[tempVertsIndex] = verts[i];
                m_TempVerts[tempVertsIndex].position *= unitsPerPixel;
                if (tempVertsIndex == 3)
                    toFill.AddUIVertexQuad(m_TempVerts);
            }
        }

        m_DisableFontTextureRebuiltCallback = false;
    }
    #endregion
}