//#define USE_GRRY_UNIT
//#define USE_ETC_1
using System;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

/// <summary>
/// 独立贴图
/// </summary>
[AddComponentMenu("MingUI/CTexture", 5)]
public class CTexture : RawImage, ICanvasRaycastFilter, IGray {
    private ItemVo _itemVo;//加载的vo
    private ItemVo _alphaItemVo;

    //public string oriPath;
    public bool raycastConsiderAlpha = false; //自身透明度是否影响事件？默认不影响：即alpha即使为0，也可以接受点击等事件
    public bool ignoreAlpha = false;
    public bool showWhiteSource = false; //是否显示白色图源
    public bool useAlphaDepart = false;
    private Action<object> _loadCallBack;
    private float _groupAlpha = 1;
    private bool _gray;
    public bool hasBorder;
    public Vector2 vBorder;
    public Vector2 hBorder;
    private Material _modifyMaterial;

    [SerializeField]
    private string _path;
    [SerializeField]
    private Texture m_Texture_alpha;//透明通道贴图
    private string _alphaPath;

    public Texture textureAlpha {
        get {
            return m_Texture_alpha;
        }
        set {
            if (m_Texture_alpha == value)
                return;
            m_Texture_alpha = value;
            SetMaterialDirty();
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
                    color = GreyMaterialManager.greyColor;
                } else {
                    color = unGreyColor;
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

    public string Path {
        get { return _path; }
        set {
            if (_path != value) {
                _path = value;
                if (string.IsNullOrEmpty(value)) {
                    _alphaPath = string.Empty;
                    texture = null;
                    textureAlpha = null;
                    OnReleaseAssetVo();
                    OnReleaseAlphaAssetVo();
                } else {
                    LoadTexture(_path); //加载图集
                    if (useAlphaDepart) {
                        _alphaPath = _path + "_alpha";
                        LoadAlphaTexture(_alphaPath);
                    } else {
                        _alphaPath = string.Empty;
                    }
                }
            }
        }
    }

    public Material ModifyMaterial {
        set {
            if (value != null) {
                material = value;
                GetModifiedMaterial(material);
            }
        }
        get {
            return _modifyMaterial;
        }
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

    protected override void OnPopulateMesh(VertexHelper vh) {
        if (texture != null || showWhiteSource) {
            base.OnPopulateMesh(vh);
            if (hasBorder) {
                var outerUV = new Vector4(0, 0, 1, 1);
                var innerUV = Vector4.zero;
                innerUV.x = hBorder.x / mainTexture.width;
                innerUV.y = vBorder.y / mainTexture.height;
                innerUV.z = (mainTexture.width - hBorder.y) / mainTexture.width;
                innerUV.w = (mainTexture.height - vBorder.x) / mainTexture.height;
                var border = new Vector4(hBorder.x, vBorder.y, hBorder.y, vBorder.x);
                var rect = GetPixelAdjustedRect();
                border = GetAdjustedBorders(border, rect);
                var vertScratches = new Vector2[4];
                vertScratches[0] = Vector2.zero;
                vertScratches[3] = new Vector2(rect.width, rect.height);
                vertScratches[1] = new Vector2(border.x, border.y);
                vertScratches[2] = new Vector2(rect.width - border.z, rect.height - border.w);
                for (var i = 0; i < 4; i++) {
                    vertScratches[i].x += rect.x;
                    vertScratches[i].y += rect.y;
                }
                var uvScratches = new[] { new Vector2(outerUV.x, outerUV.y), new Vector2(innerUV.x, innerUV.y), new Vector2(innerUV.z, innerUV.w), new Vector2(outerUV.z, outerUV.w) };
                vh.Clear();
                for (var j = 0; j < 3; j++) {
                    var num = j + 1;
                    for (var k = 0; k < 3; k++) {
                        var num2 = k + 1;
                        AddQuad(vh, new Vector2(vertScratches[j].x, vertScratches[k].y), new Vector2(vertScratches[num].x, vertScratches[num2].y), color, new Vector2(uvScratches[j].x, uvScratches[k].y), new Vector2(uvScratches[num].x, uvScratches[num2].y));
                    }
                }
            }
        } else {
            vh.Clear();
        }
    }

    public override Material GetModifiedMaterial(Material baseMaterial) {
        _modifyMaterial = base.GetModifiedMaterial(baseMaterial);
        return _modifyMaterial;
    }

    public void ReLoadTextrue(string path) {
        LoadTexture(path); //加载图集
        if (useAlphaDepart) {
            _alphaPath = path + "_alpha";
            LoadAlphaTexture(_alphaPath);
        } else {
            _alphaPath = string.Empty;
        }
    }

    //加载图集
    public void LoadTexture(string path) {
        MingUIAgent.LoadUITexture(path, LoadComplete);
    }

    public void LoadAlphaTexture(string path) {
        MingUIAgent.LoadUITexture(path, LoadAlphaComplete);
    }

    //加载完毕
    protected virtual void LoadComplete(ItemVo vo) {
        if (vo.isComplete == false) {
            Path = null;
            return;
        }
        if (ABNameEqual(_path, vo.abName) == false) {
            return;//存在可能loadcomplete多个资源的情况（加载过程中切换path）
        }
        if (_itemVo != null  ) {
            if (_itemVo.abName != vo.abName) {
                OnReleaseAssetVo();//清除上一个，该操作会减1个引用计数
            } else {
                return;//同个path只执行一次
            }
            
        }
        if (this != null && gameObject != null && !_destroy) {
            _itemVo = vo;
            var tex = vo.getObject() as Texture2D;
            if (tex != null) {
                texture = tex;
            } else {
                MingUIAgent.Info("Ctexture null" + _path);
            }
            if (_loadCallBack != null) {
                _loadCallBack(vo);
            }
        } else {
            OnReleaseAssetVo();
        }
    }

    protected virtual void LoadAlphaComplete(ItemVo vo) {
        if (ABNameEqual(_alphaPath, vo.abName) == false) {
            return;//存在可能loadcomplete多个资源的情况（加载过程中切换_alphaPath）
        }
        if (_alphaItemVo != null) {
            if (_alphaItemVo.abName != vo.abName) {
                OnReleaseAlphaAssetVo();//清除上一个，该操作会减1个引用计数
            } else {
                return;//同个path只执行一次
            }
        }
        _alphaItemVo = vo;
        if (this != null && gameObject != null && !_destroy) {
            var tex = vo.getObject() as Texture2D;
            if (tex != null) {
                textureAlpha = tex;
            } else {
                MingUIAgent.Info("Ctexture alpha null" + _alphaPath);
            }
            if (_loadCallBack != null) {
                _loadCallBack(vo);
            }
        } else {
            OnReleaseAlphaAssetVo();
        }
    }

    public void SetLoadCompleteBack(Action<object> loadCallBack) {
        _loadCallBack = loadCallBack;
    }

    protected override void OnEnable() {
        base.OnEnable();
        MingUIAgent.OnEnable(this, true);
    }

    protected virtual void OnDisable() {
        base.OnDisable();
        MingUIAgent.OnEnable(this, false);
    }

    public void OnReleaseAssetVo() {
        if (_itemVo != null) {
            _itemVo.onRelease();
            _itemVo = null;
        }
    }

    public void OnReleaseAlphaAssetVo() {
        if (_alphaItemVo != null) {
            _alphaItemVo.onRelease();
            _alphaItemVo = null;
        }
    }

    private bool _destroy = false;
    protected override void OnDestroy() {
        base.OnDestroy();
        OnReleaseAssetVo();
        OnReleaseAlphaAssetVo();
        _destroy = true;
    }

    public static bool ABNameEqual(string url, string abName) {
        var isAB = abName.EndsWith(".ab");
        if (System.IO.Path.GetExtension(url) == ".ab") {
            return url.ToLower() == abName;
        }
        string nResName = url + (isAB ? ".ab" : "");
        if (isAB) {
            nResName = nResName.ToLower();
        }
        return nResName == abName;
    }

    //public override void SetNativeSize() {
    //    base.SetNativeSize();
    //    //if (Application.isEditor && PlayerPrefs.HasKey("w_" + texture.name)) {
    //    //    var w = PlayerPrefs.GetInt("w_" + texture.name);
    //    //    var h = PlayerPrefs.GetInt("h_" + texture.name);
    //    //    rectTransform.anchorMax = rectTransform.anchorMin;
    //    //    rectTransform.sizeDelta = new Vector2(w, h);
    //    //}
    //}

    protected static void AddQuad(VertexHelper vertexHelper, Vector2 posMin, Vector2 posMax, Color32 color, Vector2 uvMin, Vector2 uvMax) {
        int currentVertCount = vertexHelper.currentVertCount;
        vertexHelper.AddVert(new Vector3(posMin.x, posMin.y, 0f), color, new Vector2(uvMin.x, uvMin.y));
        vertexHelper.AddVert(new Vector3(posMin.x, posMax.y, 0f), color, new Vector2(uvMin.x, uvMax.y));
        vertexHelper.AddVert(new Vector3(posMax.x, posMax.y, 0f), color, new Vector2(uvMax.x, uvMax.y));
        vertexHelper.AddVert(new Vector3(posMax.x, posMin.y, 0f), color, new Vector2(uvMax.x, uvMin.y));
        vertexHelper.AddTriangle(currentVertCount, currentVertCount + 1, currentVertCount + 2);
        vertexHelper.AddTriangle(currentVertCount + 2, currentVertCount + 3, currentVertCount);
    }

    private Vector4 GetAdjustedBorders(Vector4 border, Rect rect) {
        for (int i = 0; i <= 1; i++) {
            float num = border[i] + border[i + 2];
            if (rect.size[i] < num && num != 0f) {
                float num2 = rect.size[i] / num;
                float num3 = border[i];
                border[i] = num3 * num2;
                num3 = border[i + 2];
                border[i + 2] = num3 * num2;
            }
        }
        return border;
    }

    protected override void UpdateMaterial() {
        base.UpdateMaterial();
        if (useAlphaDepart && textureAlpha != null) {
            canvasRenderer.SetAlphaTexture(textureAlpha);
        }
#if USE_ETC_1
        else {
            canvasRenderer.SetAlphaTexture(Texture2D.whiteTexture);
        }
#endif
    }

    /// <summary>
    /// 增强原Raycast功能，使得贴图透明部分不响应鼠标事件
    /// 前提：贴图Readable为true，ignoreAlpha为true，CTexture的pivot为Vector2.zero
    /// </summary>
    /// <param name="sp"></param>
    /// <param name="eventCamera"></param>
    /// <returns></returns>
    public override bool Raycast(Vector2 sp, Camera eventCamera) {
        var result = base.Raycast(sp, eventCamera);
        if (result && ignoreAlpha) {
            Vector2 local;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, eventCamera,
                out local);
            var pixelAdjustedRect = GetPixelAdjustedRect();
            local.x += rectTransform.pivot.x * pixelAdjustedRect.width;
            local.y += rectTransform.pivot.y * pixelAdjustedRect.height;
            var vector = new Vector2(local.x / Width, local.y / Height);
            float u = Mathf.Lerp(rectTransform.rect.x, rectTransform.rect.xMax, vector.x) / Width;
            float v = Mathf.Lerp(rectTransform.rect.y, rectTransform.rect.yMax, vector.y) / Height;
            var tex2D = texture as Texture2D;
            if (tex2D) {
                result = tex2D.GetPixelBilinear(u, v).a > .01f;
            }
        }
        return result;
    }

}