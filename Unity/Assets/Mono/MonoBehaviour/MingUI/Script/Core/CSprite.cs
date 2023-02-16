//#define USE_GRRY_UNIT
//#define USE_ETC_1
using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 图元
/// </summary>
[AddComponentMenu("MingUI/CSprite", 4)]
public class CSprite : Image, IGray {
    private ItemVo _itemVo;//加载的vo
    private static char[] SPLIT = new[] { '.' };
    private string _path;
    public string moduleName;
    private float _groupAlpha = 1;
    private bool _gray;

    [SerializeField]
    private CAtlas _atlas; //图集
    [SerializeField]
    private string _spriteName; //图元

    [SerializeField]
    private string _atlasName; //原始图集名称

    //#if USE_ETC_1
    //[SerializeField]
    //private Sprite alphaSprite;//透明数据
    //#endif

    public bool autoSnap = true; //动态加载后是否自动适应大小
    public bool showWhiteSource = false; //是否显示白色图源
    public bool raycastConsiderAlpha = false; //自身透明度是否影响事件？默认不影响：即alpha即使为0，也可以接受点击等事件

    private UIEventListener.Vector2Delegate _onNativeSize;
    private UIEventListener.VoidDelegate _loadAtlasCallBack;
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

    public override Material material {
        get {
            if (m_Material != null)
                return m_Material;
            return defaultMaterial;
        }
        set {
            base.material = value;
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
                material = _gray ? GreyMaterialManager.GetGreyMaterial() : null;
                #endif
            }
        }
    }
    /// <summary>
    /// 设置图集
    /// </summary>
    public CAtlas Atlas {
        get { return _atlas; }
        set {
            //更换了图集
            if (_atlas != value) {
                if (Atlas != null) {
                    MingUIAgent.OnEnable(this, false);//上一个图集需要释放计数器
                }
                _atlas = value;
                RefreshAtlasName();
                SpriteName = null;
            }
        }
    }

    public string AtlasName {
        get { return _atlasName; }
    }

    public void RefreshAtlasName() {
        if (_atlas != null && _atlasName != _atlas.name) {
            _atlasName = _atlas.name;
        } else if (_atlas == null) {
            _atlasName = null;
        }
    }

    /// <summary>
    /// 设置图元
    /// </summary>
    public string SpriteName {
        get { return _spriteName; }
        set {
            if ( string.IsNullOrEmpty(value)) {
                //MingUIAgent.Info("SpriteName NULL" + _spriteName);
            }
            if (_spriteName != value) {
                _spriteName = value;
                if (string.IsNullOrEmpty(_spriteName)) {
                    sprite = null;
                    //#if USE_ETC_1
                    //alphaSprite = null;
                    _path = string.Empty;
                } else if (Atlas != null) {
                    sprite = Atlas.GetSprite(_spriteName);
                    //#if USE_ETC_1
                    //alphaSprite = Atlas.GetSprite(_spriteName, true);
                    //#endif
                    if (Application.isPlaying && type == Type.Simple && autoSnap && rectTransform.anchorMin == rectTransform.anchorMax) {
                        SetNativeSize();
                    }
                }
            }
        }
    }

    /// <summary>
    /// 动态设置图集和图元
    /// 参数形式：path = "MingUI.bg"
    /// </summary>
    public string Path {
        get { return _path; }
        set {
            if (_path != value) {
                _path = value;
                if (string.IsNullOrEmpty(_path)) {
                    SpriteName = null;
                } else {
                    string[] arr = _path.Split(SPLIT);
                    string argAtlas = arr[0];
                    string argSprite = arr[1];
                    moduleName = arr.Length > 2 ? arr[2] : argAtlas;
                    SpriteName = argSprite;
                    LoadAtlas(argAtlas); //去加载图集
                }
            }
        }
    }
    /// <summary>
    /// 添加自动大小回调
    /// </summary>
    /// <param name="callback"></param>
    public void AddNativeSize(UIEventListener.Vector2Delegate callback) {
        _onNativeSize = callback;
    }
    /// <summary>
    /// 移除自动大小回调
    /// </summary>
    public void RemoveNativeSize() {
        _onNativeSize = null;
    }

    public void AddAtlasLoadCallBack(UIEventListener.VoidDelegate callback) {
        _loadAtlasCallBack = callback;
    }

    public void RemoveAtlasCallBack(UIEventListener.VoidDelegate callback) {
        if (_loadAtlasCallBack != null && _loadAtlasCallBack == callback) {
            _loadAtlasCallBack = null;
        }
    }

    public void RemoveAtlasCallBack() {
        _loadAtlasCallBack = null;
    }

    /// <summary>
    /// 加载图集
    /// </summary>
    /// <param name="atName"></param>
    public void LoadAtlas(string atName) {
      
        if (Atlas != null) {
            MingUIAgent.OnEnable(this, false);//上一个图集需要释放计数器
        }
        if (Atlas == null || Atlas.name != atName) {

            if (_atlas != null) {
                if (Atlas != null) {
                    MingUIAgent.OnEnable(this, false);//上一个图集需要释放计数器
                }
                _atlas = null;
                RefreshAtlasName();
            }//清图集确保回调的时候能做差异对比hao 

            if (string.IsNullOrEmpty(moduleName) == false)
            {
                MingUIAgent.LoadUIPrefab("UI/" + moduleName + "/Prefabs/" + atName, LoadComplete);
            }
            else
            {
                MingUIAgent.LoadUIPrefab("UI/" + atName + "/Prefabs/" + atName, LoadComplete);

            }
        }
    }

    /// <summary>
    /// 加载完毕
    /// </summary>
    private void LoadComplete(ItemVo vo) {
        if (string.IsNullOrEmpty(_atlasName) == false && System.IO.Path.GetFileNameWithoutExtension(vo.abName) != _atlasName.ToLower()) {
            MingUIAgent.Info("LoadComplete atlas已经换了，不要callback" +vo.abName + (string.IsNullOrEmpty(_atlasName) ? "" : _atlasName ));
            return;//atlas已经换了，不要callback
        }
        _itemVo = vo;
        GameObject atlasGo = vo.getObject() as GameObject;
        if (atlasGo != null) {
            CAtlas at = atlasGo.GetComponent<CAtlas>();
            _atlas = at;
            RefreshSprite();
            RefreshAtlasName();
            if (_loadAtlasCallBack != null) {
                _loadAtlasCallBack.Invoke();
            }
        }
    }

    /// <summary>
    /// 这里重写是因为UGUI在响应事件时没有计算透明度
    /// </summary>
    /// <param name="screenPoint"></param>
    /// <param name="eventCamera"></param>
    /// <returns></returns>
    public override bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera) {
        if (raycastConsiderAlpha && (color.a <= 0f || _groupAlpha <= 0f)) {
            return false;
        }
        return base.IsRaycastLocationValid(screenPoint, eventCamera);
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

    protected override void Start() {
        base.Start();
        CheckAtlasSprite();
    }

    protected override void OnDestroy() {
        base.OnDestroy();
        sprite = null;
        //#if USE_ETC_1
        //alphaSprite = null;
        //#endif
        _atlas = null;
    }

    protected override void OnRectTransformDimensionsChange() {
        base.OnRectTransformDimensionsChange();
        if (_onNativeSize != null) {
            _onNativeSize(Size);
        }
    }
    protected override void OnPopulateMesh(VertexHelper vh) {
        if (overrideSprite != null || showWhiteSource) {
            base.OnPopulateMesh(vh);
        } else {
            vh.Clear();
        }
    }

    //public override Material GetModifiedMaterial(Material baseMaterial)
    //{
    //    Material cModifiedMat = base.GetModifiedMaterial(baseMaterial);
    //    return cModifiedMat;
    //}

    /// <summary>
    /// 保证spriteName与本身的sprite统一
    /// </summary>
    public void CheckAtlasSprite() {
        string temp = sprite != null ? sprite.name : null;
        if (temp != _spriteName) {
            RefreshSprite();
            if (MingUIAgent.IsEditorMode) {
                MingUIAgent.EditorSetDirty(gameObject);
            }
        }
        
    }

    /// <summary>
    /// 强制刷新下当前的sprite
    /// 由于本身的sprite也是序列化属性而在改变了图集后，这种属性就丢失了
    /// 用于在自己的图集发生了改变后，需要根据_spriteName再次关联下，这样就保证了更新图集不会丢失之前的sprite
    /// </summary>
    public void RefreshSprite() {

        string temp = _spriteName;
        _spriteName = null;
        SpriteName = temp;
    }

    protected override void OnEnable() {
        base.OnEnable();
        MingUIAgent.OnEnable(this, true);
    }

    protected override void OnDisable() {
        base.OnDisable();
        MingUIAgent.OnEnable(this, false);
    }

    //#if USE_ETC_1
    //protected override void UpdateMaterial() {
    //    base.UpdateMaterial();
    //    if (sprite != null && sprite.associatedAlphaSplitTexture) {
    //        canvasRenderer.SetAlphaTexture(sprite.associatedAlphaSplitTexture);
    //    } else {
    //        if (alphaSprite == null && Atlas != null) {
    //            alphaSprite = Atlas.GetSprite(SpriteName, true);
    //        }
    //        if (alphaSprite != null) {
    //            canvasRenderer.SetAlphaTexture(alphaSprite.texture);
    //        } else {
    //            canvasRenderer.SetAlphaTexture(Texture2D.whiteTexture);
    //        }
    //    }
    //}
    //#endif
}