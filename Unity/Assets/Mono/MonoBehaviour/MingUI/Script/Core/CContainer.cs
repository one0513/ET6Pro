using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 遮罩拖拽容器类
/// </summary>
public class CContainer : ScrollRect
{
    public RectTransform mask; //遮罩

    [NonSerialized] protected Bounds mBounds;
    [NonSerialized] protected bool contentChanged = true;

    [NonSerialized] private RectTransform _selfTransform;
    private Action _onContentChange;

    private bool isFirstFrame = false;
    private bool isSkipNextHandle = false;

    public RectTransform SelfTransform
    {
        get
        {
            if (_selfTransform == null)
            {
                _selfTransform = GetComponent<RectTransform>();
            }
            return _selfTransform;
        }
    }

    /// <summary>
    /// 子对象数目
    /// </summary>
    public int NumberChildren
    {
        get { return content.childCount; }
    }

    /// <summary>
    /// 包含子对象？
    /// </summary>
    /// <param name="trans"></param>
    /// <returns></returns>
    public bool Contains(RectTransform trans)
    {
        return trans.IsChildOf(content);
    }

    /// <summary>
    /// 宽度
    /// </summary>
    public float Width
    {
        get { return Size.x; }
        set { Size = new Vector2(value, Size.y); }
    }

    /// <summary>
    /// 高度
    /// </summary>
    public float Height
    {
        get { return Size.y; }
        set { Size = new Vector2(Size.x, value); }
    }

    /// <summary>
    /// 容器大小
    /// </summary>
    public Vector2 Size
    {
        get { return SelfTransform.rect.size; }
        set
        {
            if (value != SelfTransform.rect.size)
            {
                SelfTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, value.x);
                SelfTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, value.y);
                OnDimensionsChange();
            }
        }
    }

    /// <summary>
    /// 添加对象
    /// </summary>
    /// <param name="go"></param>
    /// <param name="pos"></param>
    public virtual void AddChild(GameObject go, Vector2 pos)
    {
        RectTransform trans = go.GetComponent<RectTransform>();
        if (trans != null)
        {
            AddChild(trans, pos);
        }
    }

    /// <summary>
    /// 添加对象
    /// </summary>
    /// <param name="trans"></param>
    /// <param name="pos"></param>
    public virtual void AddChild(RectTransform trans, Vector2 pos)
    {
        trans.SetParent(content);
        trans.anchorMin = trans.anchorMax = content.pivot; //约定左上角为锚点
        trans.anchoredPosition = pos;
        trans.localScale = Vector3.one;
        trans.localEulerAngles = Vector3.zero;
        ContentChanged();
    }

    /// <summary>
    /// 移除对象
    /// </summary>
    /// <param name="trans"></param>
    public virtual void RemoveChild(RectTransform trans)
    {
        if (Contains(trans))
        {
            if (MingUIAgent.IsEditorMode)
            {
                DestroyImmediate(trans.gameObject);
            }
            else
            {
                Destroy(trans.gameObject);
            }
            ContentChanged();
        }
    }

    /// <summary>
    /// 内容改变
    /// </summary>
    /// <param name="updateNow">是否立即刷新？否则等待下一个Update中执行</param>
    public virtual void ContentChanged(bool updateNow = false)
    {
        if (updateNow == false && isFirstFrame) {
            isSkipNextHandle = true;
        }
        contentChanged = true;
        if (updateNow)
        {
            OnContentChange();
        }
    }

    /// <summary>
    /// 添加内容改变回调
    /// </summary>
    /// <param name="callback"></param>
    public void AddContentChange(Action callback)
    {
        _onContentChange = callback;
    }

    /// <summary>
    /// 移除内容改变回调
    /// </summary>
    public void RemoveContentChange()
    {
        _onContentChange = null;
    }

    #region 内部接口

    protected override void Awake()
    {
        base.Awake();
        if (Application.isPlaying)
        {
            OnInit();
        }
    }

    protected virtual void OnInit()
    {
        Vector2 maskSize = mask.rect.size;
        content.pivot = content.anchorMin = content.anchorMax = new Vector2(0, 1);
        content.anchoredPosition = Vector2.zero; //在中心点
        content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maskSize.x);
        content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, maskSize.y);
        isFirstFrame = true;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        onValueChanged.AddListener(OnValueChanged);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        onValueChanged.RemoveListener(OnValueChanged);
    }

    protected virtual void OnValueChanged(Vector2 value)
    {
    }

    private void Update()
    {
        OnUpdate();
    }

    protected virtual void OnUpdate()
    {
        if (Application.isPlaying)
        {
            if (isSkipNextHandle) {
                isSkipNextHandle = false;//跳过本次的处理
            } else {
                if (contentChanged) {
                    OnContentChange();
                }
            }
            isFirstFrame = false;
        }
    }

    /// <summary>
    /// 获取实际内容的边框盒大小
    /// </summary>
    /// <returns></returns>
    protected virtual Bounds GetBounds()
    {
        if (contentChanged)
        {
            content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, mask.rect.width);
            content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, mask.rect.height);
            mBounds = RectTransformUtility.CalculateRelativeRectTransformBounds(content);
        }
        return mBounds;
    }

    /// <summary>
    /// 内容发生改变（添加、删除子对象）
    /// 则需要重新设置下content的尺寸
    /// </summary>
    protected virtual void OnContentChange()
    {
        Vector3 size = GetBounds().size;
        content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
        content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);

        contentChanged = false;

        if (_onContentChange != null)
        {
            _onContentChange();
        }
    }

    /// <summary>
    /// 面积发生改变
    /// </summary>
    public virtual void OnDimensionsChange()
    {
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        mask = null;
        _selfTransform = null;
        _onContentChange = null;
    }
    private void OnDrawGizmos()
    {
        if (content != null)
        {
            Bounds b = RectTransformUtility.CalculateRelativeRectTransformBounds(content);
            Gizmos.matrix = content.localToWorldMatrix;
            Gizmos.color = new Color(1f, 0.4f, 0f);
            Gizmos.DrawWireCube(new Vector3(b.center.x, b.center.y, b.min.z), new Vector3(b.size.x, b.size.y, 0f));
        }
    }
#endregion
}