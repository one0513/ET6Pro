using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 事件阻挡器，用来阻挡事件流
/// </summary>
[AddComponentMenu("MingUI/CBoxCollider", 7)]
public class CBoxCollider :
    MaskableGraphic,
    ICanvasRaycastFilter,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerDownHandler,
    IPointerUpHandler,
    IPointerClickHandler,
    IInitializePotentialDragHandler,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler,
    IDropHandler,
    IScrollHandler,
    IUpdateSelectedHandler,
    ISelectHandler,
    IDeselectHandler,
    IMoveHandler,
    ISubmitHandler,
    ICancelHandler
{
    public bool showWhiteSource = true; //是否显示白色图源

    //这里暂时只添加这几种事件回调，届时根据需要再进行添加即可
    private UIEventListener.PointerDelegate _onDown;
    private UIEventListener.PointerDelegate _onUp;
    private UIEventListener.PointerDelegate _onClick;
    private UIEventListener.BoolDelegate _onSelect;
    private float _groupAlpha = 1;

    #region 添加事件

    /// <summary>
    /// 添加单击
    /// </summary>
    /// <param name="callback"></param>
    public void AddClick(UIEventListener.PointerDelegate callback)
    {
        _onClick = callback;
    }

    /// <summary>
    /// 移除单击
    /// </summary>
    public void RemoveClick()
    {
        _onClick = null;
    }

    /// <summary>
    /// 添加鼠标按下
    /// </summary>
    /// <param name="callback"></param>
    public void AddMouseDown(UIEventListener.PointerDelegate callback)
    {
        _onDown = callback;
    }

    /// <summary>
    /// 移除鼠标按下
    /// </summary>
    public void RemoveMouseDown()
    {
        _onDown = null;
    }

    /// <summary>
    /// 添加鼠标弹起
    /// </summary>
    /// <param name="callback"></param>
    public void AddMouseUp(UIEventListener.PointerDelegate callback)
    {
        _onUp = callback;
    }

    /// <summary>
    /// 移除鼠标弹起
    /// </summary>
    public void RemoveMouseUp()
    {
        _onUp = null;
    }
    /// <summary>
    /// 添加选中
    /// </summary>
    /// <param name="callback"></param>
    public void AddSelect(UIEventListener.BoolDelegate callback)
    {
        _onSelect = callback;
    }
    /// <summary>
    /// 取消选中
    /// </summary>
    public void RemoveSelect()
    {
        _onSelect = null;
    }
    #endregion

    #region 内部接口
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        if (showWhiteSource)
        {
            base.OnPopulateMesh(vh);
        }
        else
        {
            vh.Clear();
        }
    }
    public virtual void OnPointerEnter(PointerEventData eventData)
    {
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
    }

    public virtual void OnDrop(PointerEventData eventData)
    {
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (_onDown != null)
        {
            _onDown(eventData);
        }
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        if (_onUp != null)
        {
            _onUp(eventData);
        }
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (_onClick != null)
        {
            _onClick(eventData);
        }
    }

    public virtual void OnSelect(BaseEventData eventData)
    {
        if (_onSelect != null)
        {
            _onSelect(true);
        }
    }

    public virtual void OnDeselect(BaseEventData eventData)
    {
        if (_onSelect != null)
        {
            _onSelect(false);
        }
    }

    public virtual void OnScroll(PointerEventData eventData)
    {
    }

    public virtual void OnMove(AxisEventData eventData)
    {
    }

    public virtual void OnUpdateSelected(BaseEventData eventData)
    {
    }

    public virtual void OnInitializePotentialDrag(PointerEventData eventData)
    {
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
    }

    public virtual void OnSubmit(BaseEventData eventData)
    {
    }

    public virtual void OnCancel(BaseEventData eventData)
    {
    }

    /// <summary>
    /// 这里重写是因为UGUI在响应事件时没有计算透明度
    /// </summary>
    /// <param name="screenPoint"></param>
    /// <param name="eventCamera"></param>
    /// <returns></returns>
    public virtual bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
    {
        return _groupAlpha > 0f;
    }

    protected override void OnCanvasGroupChanged()
    {
        base.OnCanvasGroupChanged();
        _groupAlpha = MingUIUtil.GetGroupAlpha(gameObject);
    }

    protected override void UpdateMaterial() {
        base.UpdateMaterial();
        canvasRenderer.SetAlphaTexture(Texture2D.whiteTexture);
    }
    #endregion
}