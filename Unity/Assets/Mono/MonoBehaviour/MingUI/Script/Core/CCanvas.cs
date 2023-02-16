using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 画布容器
/// 增强功能：
/// 1.水平/垂直按量、方向、定位滚动
/// 2.边界判断及回调
/// 3.滚动条自动淡化效果
/// </summary>
public class CCanvas : CContainer
{
    public bool autoAlphaScrollBar = true; //滚动条自动淡化
    public Ease ease;
    public float easeTime = 0.25f;
    public Vector2 horizontalLimit = Vector2.zero;
    public Vector2 verticalLimit = Vector2.zero;

    private float _tempAlpha = 1f; //滚动条的淡化值
    private float _hPreValue = -1f; //上一帧水平位置
    private float _vPreValue = -1f; //上一帧垂直位置
    private Action<Vector2> _onPositionChange; //滑动值触发
    private Action<bool> _onPositionBound; //触发边界时的回调,true 进入边界 false 退出边界

    private Action<object> _onDragCallBack; //拖拽回调

    #region 外部接口

    /// <summary>
    /// 是否到最上
    /// </summary>
    public virtual bool IsTop
    {
        get { return verticalNormalizedPosition >= 1; }
    }

    /// <summary>
    /// 是否到最下
    /// </summary>
    public virtual bool IsBottom
    {
        get { return verticalNormalizedPosition <= 0; }
    }

    /// <summary>
    /// 是否到最左
    /// </summary>
    public virtual bool IsLeft
    {
        get { return horizontalNormalizedPosition <= 0; }
    }

    /// <summary>
    /// 是否到最右
    /// </summary>
    public virtual bool IsRight
    {
        get { return horizontalNormalizedPosition >= 1; }
    }

    /// <summary>
    /// 滚动到最左
    /// </summary>
    public virtual void ScrollToLeft(bool anim = false, TweenCallback callback = null)
    {
        StopMovement();
        if (anim)
        {
            // Tweener tweener = this.DOHorizontalNormalizedPos(0, easeTime);
            // tweener.SetEase(ease);
            // tweener.OnComplete(callback);
        }
        else
        {
            horizontalNormalizedPosition = 0;
            if (callback != null)
            {
                callback();
            }
        }
    }

    public virtual void ScrollHorizontalNormalized(float normalizedVal, bool anim = false, TweenCallback callback = null) {
        StopMovement();
        if (anim) {
            // Tweener tweener = this.DOHorizontalNormalizedPos(normalizedVal, easeTime);
            // tweener.SetEase(ease);
            // tweener.OnComplete(callback);
        } else {
            horizontalNormalizedPosition = 0;
            if (callback != null) {
                callback();
            }
        }
    }

    /// <summary>
    /// 滚动到最右
    /// </summary>
    public virtual void ScrollToRight(bool anim = false, TweenCallback callback = null)
    {
        StopMovement();
        if (anim)
        {
            // Tweener tweener = this.DOHorizontalNormalizedPos(1, easeTime);
            // tweener.SetEase(ease);
            // tweener.OnComplete(callback);
        }
        else
        {
            horizontalNormalizedPosition = 1;
            if (callback != null)
            {
                callback();
            }
        }
    }

    /// <summary>
    /// 滚动到顶部
    /// </summary>
    public virtual void ScrollToTop(bool anim = false, TweenCallback callback = null)
    {
        StopMovement();
        if (anim)
        {
            // Tweener tweener = this.DOVerticalNormalizedPos(1, easeTime);
            // tweener.SetEase(ease);
            // tweener.OnComplete(callback);
        }
        else
        {
            verticalNormalizedPosition = 1;
            if (callback != null)
            {
                callback();
            }
        }
    }

    /// <summary>
    /// 滚动到底部
    /// </summary>
    public virtual void ScrollToBottom(bool anim = false, TweenCallback callback = null)
    {
        StopMovement();
        if (anim)
        {
            // Tweener tweener = this.DOVerticalNormalizedPos(0, easeTime);
            // tweener.SetEase(ease);
            // tweener.OnComplete(callback);
        }
        else
        {
            verticalNormalizedPosition = 0;
            if (callback != null)
            {
                callback();
            }
        }
    }

    /// <summary>
    /// 增量滚动
    /// </summary>
    /// <param name="delta"></param>
    /// <param name="anim"></param>
    /// <param name="callback"></param>
    public virtual void Scroll(Vector2 delta, bool anim = false, TweenCallback callback = null)
    {
        StopMovement();
        Vector2 newPos = content.anchoredPosition - delta; //向右滚，就是向左移动，向上滚，就是向下移动
        if (movementType != MovementType.Unrestricted)
        {
            newPos = ClampPosition(newPos);
        }

        if (anim)
        {
            // Tweener tweener = content.DOAnchorPos(newPos, easeTime);
            // tweener.SetEase(ease);
            // tweener.OnComplete(callback);
        }
        else
        {
            SetContentAnchoredPosition(newPos);
            if (callback != null)
            {
                callback();
            }
        }
    }

    public void Clear()
    {
        for (int i = 0; i < content.transform.childCount; i++)
        {
            GameObject go = content.transform.GetChild(i).gameObject;
            go.SetActive(false);
            Destroy(go);
        }
        ContentChanged();
    }

    public int GetChildCount()
    {
        return content.transform.childCount;
    }

    public void ClearTopChild(bool needResetContent)
    {
        for (int i = 0; i < content.transform.childCount; i++)
        {
            if (i==0)
            {
                GameObject go = content.transform.GetChild(i).gameObject;
                go.SetActive(false);
                Destroy(go);
                break;
            }
        }
        if (needResetContent)
        {
            ContentChanged();   
        }
    }

    /// <summary>
    /// 滚动到具体位置
    /// </summary>
    /// <param name="newPos"></param>
    /// <param name="anim"></param>
    /// <param name="callback"></param>
    public virtual void ScrollTo(Vector2 newPos, bool anim = false, TweenCallback callback = null)
    {
        StopMovement();
        if (movementType != MovementType.Unrestricted)
        {
            newPos = ClampPosition(newPos);
        }
        if (anim)
        {
            // Tweener tweener = content.DOAnchorPos(newPos, easeTime);
            // tweener.SetEase(ease);
            // tweener.OnComplete(callback);
        }
        else
        {
            SetContentAnchoredPosition(newPos);
            if (callback != null)
            {
                callback();
            }
        }
    }

    public override void StopMovement()
    {
        base.StopMovement();
        if (content != null)
        {
            DOTween.Kill(content);
        }
    }

    /// <summary>
    /// 限定content的移动范围
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    protected Vector2 ClampPosition(Vector2 pos)
    {
        Vector2 boundSize = GetBounds().size;
        Vector2 viewSize = viewRect.rect.size;
        float gapX = Math.Abs(viewSize.x - boundSize.x);
        float gapY = Math.Abs(viewSize.y - boundSize.y);
        pos.x = Mathf.Clamp(pos.x, -gapX, 0);
        pos.y = Mathf.Clamp(pos.y, 0, gapY);
        return pos;
    }

    /// <summary>
    /// 添加边界回调
    /// </summary>
    /// <param name="callback"></param>
    public void AddPositionChange(Action<Vector2> callback)
    {
        _onPositionChange = callback;
    }

    /// <summary>
    /// 移除边界回调
    /// </summary>
    public void RemovePositionChange()
    {
        _onPositionChange = null;
    }

    /// <summary>
    /// 添加边界回调
    /// </summary>
    /// <param name="callback"></param>
    public void AddDragChange(Action<object> callback)
    {
        _onDragCallBack = callback;
    }

    /// <summary>
    /// 移除边界回调
    /// </summary>
    public void RemoveDragChange()
    {
        _onDragCallBack = null;
    }

    /// <summary>
    /// 添加边界回调
    /// </summary>
    /// <param name="callback"></param>
    public void AddPositionBound(Action<bool> callback)
    {
        _onPositionBound = callback;
    }

    /// <summary>
    /// 移除边界回调
    /// </summary>
    public void RemovePositionBound()
    {
        _onPositionBound = null;
    }

    #endregion

    #region 内部接口
    public override void OnDrag(PointerEventData eventData)
    {
        if (horizontal && horizontalLimit != Vector2.zero)
        {
            float h = this.horizontalNormalizedPosition;
            if (h <= horizontalLimit.x && eventData.delta.x < 0 || h >= horizontalLimit.y && eventData.delta.x > 0)
            {
                return;
            }
        }

        if (vertical && verticalLimit != Vector2.zero)
        {
            float v = this.verticalNormalizedPosition;
            if (v <= verticalLimit.x && eventData.delta.y > 0 || v >= verticalLimit.y && eventData.delta.y < 0)
            {
                return;
            }
        }
        base.OnDrag(eventData);
        if (_onDragCallBack!=null)
        {
            _onDragCallBack(null);
        }
    }

    protected override void OnValueChanged(Vector2 value)
    {
        if (Application.isPlaying == false) return;
        if (_onPositionChange != null)
        {
            _onPositionChange(value);
        }

        float nowH = Mathf.Clamp01(value.x);
        float nowV = Mathf.Clamp01(value.y);

        if (horizontal && Math.Abs(_hPreValue - nowH) > 0)
        {
            if ((_hPreValue > 0 && nowH <= 0) || (_hPreValue < 1 && nowH >= 1)) //进入水平边界
            {
                OnPositionBound(true);
            }
            else if ((_hPreValue <= 0 && nowH > 0) || (_hPreValue >= 1 && nowH < 1)) //退出水平边界
            {
                OnPositionBound(false);
            }
            _hPreValue = nowH;
        }
        if (vertical && Math.Abs(_vPreValue - nowV) > 0)
        {
            if ((_vPreValue > 0 && nowV <= 0) || (_vPreValue < 1 && nowV >= 1)) //进入垂直边界
            {
                OnPositionBound(true);
            }
            else if ((_vPreValue <= 0 && nowV > 0) || (_vPreValue >= 1 && nowV < 1)) //退出垂直边界
            {
                OnPositionBound(false);
            }
            _vPreValue = nowV;
        }
    }

    protected virtual void OnPositionBound(bool isEnter)
    {
        if (_onPositionBound != null)
        {
            _onPositionBound(isEnter);
        }
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();

        if (Application.isPlaying && autoAlphaScrollBar && (horizontalScrollbar != null || verticalScrollbar != null))
        {
            if (Math.Abs(velocity.x) > 0.01f || Math.Abs(velocity.y) > 0.01f)
            {
                _tempAlpha = 1; //在滑动，则要显示滚动条
            }
            else
            {
                _tempAlpha -= 0.03f;
                _tempAlpha = Math.Max(0, _tempAlpha); //否则渐变
            }
            if (horizontalScrollbar != null)
            {
                MingUIUtil.SetGroupAlpha(horizontalScrollbar.gameObject, _tempAlpha);
            }
            if (verticalScrollbar != null)
            {
                MingUIUtil.SetGroupAlpha(verticalScrollbar.gameObject, _tempAlpha);
            }
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        StopMovement();
        _onPositionChange = null;
        _onPositionBound = null;
    }

    #endregion
}