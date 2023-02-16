using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class CBaseRender : CSprite, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{

    public enum BgAlternateMode {
        none = 0,
        firstBegin = 1,
        secondBegin = 2,
    }

    private UIEventListener.BoolDelegate _onSelect;
    private UIEventListener.PointerDelegate _onClick;
    private UIEventListener.PointerDelegate _onLogicClick;
    private Action<int,float> _onSetHeight;
    private bool _currLogicClickIsValid = true;//经过_onLogicClick事件后（期间会调用SetCurrentLogicClickIsValid），是否触发后续的选中效果。

    private int _index;
    private bool _select;
    private bool _isForceSelect = true; //第一次设置Select值时，强制set一次.

    public bool selectHide; //是否需要选中隐藏
    public BgAlternateMode showBgAlternateMode = BgAlternateMode.none;
    public CSprite sourceBg; //选中时的背景
    public CSprite selectBg; //选中时的背景
    public CMeshProLabel defaultLabel; //默认的文本框
    public CSprite redTip; //红点
    private static int _tweenID;
    private string GetTweenID() {
        return "CBaseRender_" + _tweenID++;
    }
    private string tweenID;
    private bool isShowRedTip;
    private bool isInitShowRedTip;

    public Tween tweenAlpha;

    private string _sound = "subTab";

    public string Sound
    {
        set { _sound = value; }
    }

    //当前render所在的数据索引
    public int Index {
        get {
            return _index;
        }
        set {
            _index = value;
            if (sourceBg && showBgAlternateMode != BgAlternateMode.none) {
                switch (showBgAlternateMode) {
                    case BgAlternateMode.firstBegin:
                        sourceBg.gameObject.SetActive(_index % 2 == 0);
                        break;
                    case BgAlternateMode.secondBegin:
                        sourceBg.gameObject.SetActive(_index % 2 != 0);
                        break;
                }
            }
        }
    } 

    public bool Select
    {
        get { return _select; }
        set
        {
            if (_isForceSelect || _select != value)
            {
                _select = value;
                if (selectHide)
                {
                    if (sourceBg != null)
                    {
                        sourceBg.Alpha = _select ? 0 : 1;
                    }
                    else
                    {
                        Alpha = _select ? 0 : 1;
                    }
                }
                if (selectBg != null)
                {
                    selectBg.Alpha = 1;
                    selectBg.gameObject.SetActive(_select);
                }
                if (_onSelect != null)
                {
                    _onSelect(_select);
                }
            }
            _isForceSelect = false;
        }
    }

    /// <summary>
    /// 添加单击
    /// </summary>
    /// <param name="callback"></param>
    public void AddSelect(UIEventListener.BoolDelegate callback)
    {
        _onSelect = callback;
    }

    /// <summary>
    /// 逻辑层的最先触发的点击事件，走完自己的逻辑后，可以调用SetCurrentLogicClickIsValid()方法，来设置是否继续走后续的OnPointerClick方法.
    /// </summary>
    public void AddLogicClick(UIEventListener.PointerDelegate callback)
    {
        _onLogicClick = callback;
    }

    public void RemoveLogicClick(UIEventListener.PointerDelegate callback)
    {
        _onLogicClick = null;
    }

    public void SetCurrentLogicClickIsValid(bool value)
    {
        _currLogicClickIsValid = value;
    }

    public void AddHeightChangeHandle(Action<int, float> callback)
    {
        _onSetHeight = callback;
    }

    /// <summary>
    /// 移除单击
    /// </summary>
    public void RemoveSelect()
    {
        _onSelect = null;
    }
    public void SetDefultLabel(object data)
    {
       
    }

    public string DefultLabel
    {
        get
        {
            return defaultLabel != null ? defaultLabel.text : string.Empty;
        }
        set
        {
            if (defaultLabel != null) defaultLabel.text = value;
        }
    }

    /// <summary>
    /// 添加单击（一般只提供级父层管理类来调用，如果上层需要监控，则选用：AddSelect）
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

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_onLogicClick != null)
        {
            SetCurrentLogicClickIsValid(true);
            _onLogicClick(eventData);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (_currLogicClickIsValid == false)
        {
            return;
        }
        if (_onClick != null)
        {
            MingUIAgent.PlaySound(_sound);
            _onClick(eventData);
        }
    }

    /// <summary>
    /// 红点显示
    /// </summary>
    /// <param name="visible"></param>
    public string ShowRedTip(bool visible) {
        if (redTip) {
            if (visible && tweenID == null) tweenID = GetTweenID();
            if (isShowRedTip != visible || isInitShowRedTip == false) {
                CRedDotHelper.ShowRedDot(redTip, visible, ref tweenID);
            }
            isShowRedTip = visible;
            isInitShowRedTip = true;
        }
        return tweenID;
    }

    public void SetHeight(int height)
    {
        if(_onSetHeight!=null)
        {
            _onSetHeight(Index,height);
        }
    }

    public void Recycle() {
        if (tweenAlpha != null) {
            DOTween.Kill(gameObject);
            tweenAlpha = null;
        }

        if (tweenID != null) {
            CRedDotHelper.ShowRedDot(redTip, false, ref tweenID);
            tweenID = null;
        }
        isShowRedTip = false;
        isInitShowRedTip = false;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        _onClick = null;
        selectBg = null;
        sourceBg = null;
    }
}