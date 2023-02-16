using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 按钮
/// </summary>
[AddComponentMenu("MingUI/CButton", 1)]
public class CButton : Button {
    private UIEventListener.PointerDelegate _onDown;
    private UIEventListener.PointerDelegate _onUp;
    private UIEventListener.PointerDelegate _onClick;
    private UIEventListener.PointerDelegate _onDoubleClick;
    private Action<object> _onPointExit;
    private Action _onLongClick;
    private Action<object> _onFirstLongClick;
    private bool _isFirstLongClick;
    private Action<object> _timeProtectCallBack;

    private bool _pointDownTag;
    private int _frameCount;
    private float _frameAdd;
    private Color _normalTxtColor;//默认的文本颜色
    private readonly Color disableTxtColor = new Color(225f / 255f, 225f / 255f, 225f / 255f);//变灰文本颜色
    private readonly Color disableTxtColor2 = new Color(255f / 255f, 255f / 255f, 255f / 255f);//变灰文本颜色
    private long _lastClickTime = 0;//上次点击时间

    public CMeshProLabel label; //文本
    public CSprite icon; //图标
    public CSprite redTip; //红点
    public bool isCanSetGray = true;//true则变灰调用SetGray，false则用disableSprite(如果有)
    public int clickTimeProtect = 0; // 点击保护
    public int longClickThreshold = 30;//触发长按事件的阈值帧数

    [SerializeField]
    private bool _openButtonEffect = true; //是否开启按钮点击效果
    [SerializeField]
    private bool _isEnabled = true; //是否可点击
    private string normalName = "";
    private string _sound = "audio_effect_ui_click";
    private static int _tweenID;
    private string GetTweenID() {
        return "CButton_" + _tweenID++;
    }

    private string tweenID;
    private bool isShowRedTip;
    private bool isInitShowRedTip;

    public string Sound {
        set { _sound = value; }
    }

    protected override void Awake() {
        if (label) {
            _normalTxtColor = label.color;
        }
        //暂时屏蔽点击效果
        base.Awake();
        CButtonEffect effect = gameObject.GetComponent<CButtonEffect>();
        if (effect != null) {
            effect.enabled = false;
        }
        _frameCount = 0;
        _frameAdd = longClickThreshold;
        if (targetGraphic as CSprite) {
            normalName = (targetGraphic as CSprite).SpriteName;
        }
    }

    public bool IsEnabled {
        get { return _isEnabled; }
        set {
            if (value != _isEnabled) {
                _isEnabled = value;
                //interactable = _isEnabled;
                //OpenButtonEffect = _isEnabled;
                //transition = Transition.None;
                if (!isCanSetGray) {
                    if (label != null) {
                        label.color = _isEnabled ? _normalTxtColor : disableTxtColor;
                    }
                } else {
                    if (spriteState.disabledSprite) {
                        (targetGraphic as CSprite).SpriteName = _isEnabled ? normalName : spriteState.disabledSprite.name;
                        if (label != null) {
                            label.color = _isEnabled ? _normalTxtColor : disableTxtColor2;
                        }
                    } else {
                        MingUIUtil.SetGray(gameObject, !_isEnabled);
                    }
                }
            }
        }
    }

    public bool IsEnableAndInteractable {
        get { return _isEnabled && interactable; }
        set {
            IsEnabled = value;
            interactable = value;
        }
    }

    public bool OpenButtonEffect {
        get { return _openButtonEffect; }
        set {
            _openButtonEffect = value;
            if (Application.isPlaying) {
                if (_openButtonEffect) {
                    MingUIUtil.AddButtonEffect(gameObject);
                } else {
                    CButtonEffect effect = gameObject.GetComponent<CButtonEffect>();
                    if (effect != null) {
                        effect.enabled = false;
                    }
                }
            }
        }
    }

    private RectTransform _selfTransform;

    public RectTransform SelfTransform {
        get {
            if (_selfTransform == null) {
                _selfTransform = GetComponent<RectTransform>();
            }
            return _selfTransform;
        }
    }

    public string Text {
        get {
            if (label == null) return null;
            return label.text; }
        set { label.text = value; }
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
    /// 容器大小
    /// </summary>
    public Vector2 Size {
        get { return SelfTransform.rect.size; }
        set {
            if (value != SelfTransform.rect.size) {
                SelfTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, value.x);
                SelfTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, value.y);
            }
        }
    }

    #region 添加事件

    /// <summary>
    /// 添加在保护时间时点击按钮时的回调
    /// </summary>
    /// <param name="callback"></param>
    public void AddTimeProtectCallBack(Action<object> callback) {
        _timeProtectCallBack = callback;
    }

    /// <summary>
    /// 移除在保护时间时点击按钮时的回调
    /// </summary>
    public void RemoveTimeProtectCallBack() {
        _timeProtectCallBack = null;
    }

    /// <summary>
    /// 添加单击
    /// </summary>
    /// <param name="callback"></param>
    public void AddClick(UIEventListener.PointerDelegate callback) {
        _onClick = callback;
    }

    /// <summary>
    /// 移除单击
    /// </summary>
    public void RemoveClick() {
        _onClick = null;
    }

    /// <summary>
    /// 添加双击
    /// </summary>
    /// <param name="callback"></param>
    public void AddDoubleClick(UIEventListener.PointerDelegate callback) {
        _onDoubleClick = callback;
    }

    /// <summary>
    /// 移除双击
    /// </summary>
    public void RemoveDoubleClick() {
        _onDoubleClick = null;
    }

    public void AddLongClick(Action callback) {
        _onLongClick = callback;
    }

    public void RemoveLongClick() {
        _onLongClick = null;
    }

    public void AddFirstLongClick(Action<object> callback) {
        _onFirstLongClick = callback;
    }

    public void RemoveFirstLongClick() {
        _onFirstLongClick = null;
    }

    /// <summary>
    /// 添加鼠标按下
    /// </summary>
    /// <param name="callback"></param>
    public void AddMouseDown(UIEventListener.PointerDelegate callback) {
        _onDown = callback;
    }

    /// <summary>
    /// 移除鼠标按下
    /// </summary>
    public void RemoveMouseDown() {
        _onDown = null;
    }

    /// <summary>
    /// 添加鼠标弹起
    /// </summary>
    /// <param name="callback"></param>
    public void AddMouseUp(UIEventListener.PointerDelegate callback) {
        _onUp = callback;
    }

    /// <summary>
    /// 移除鼠标弹起
    /// </summary>
    public void RemoveMouseUp() {
        _onUp = null;
    }

    public void AddPointExitCallBack(Action<object> callback) {
        _onPointExit = callback;
    }
    public void RemovePointExitCallBack() {
        _onPointExit = null;
    }

    #endregion

    #region 内部接口

    private bool CanReceiveEvent(PointerEventData eventData) {
        return eventData.button == PointerEventData.InputButton.Left && IsActive() && IsInteractable();
    }


    public void ClearCD() {
        _lastClickTime = 0;
    }
    public override void OnPointerClick(PointerEventData eventData) {
        base.OnPointerClick(eventData);
        _pointDownTag = false;
        _isFirstLongClick = false;
        if (CanReceiveEvent(eventData)) {
            //UIEventListener.Print("Click", gameObject, eventData);
            if (_onClick != null) {
                if (clickTimeProtect == 0) {
                    MingUIAgent.PlaySound(_sound);
                    _onClick(eventData);
                } else {
                    long now = MathUtil.GetTime();
                    long useTime = now - _lastClickTime;
                    if (useTime >= clickTimeProtect) {
                        _lastClickTime = now;
                        _onClick(eventData);
                    } else if (_timeProtectCallBack != null) {

                        _timeProtectCallBack(clickTimeProtect - useTime);
                    }
                }
            }
            if (eventData.clickCount == 2) //双击
            {
                //UIEventListener.Print("DoubleClick", gameObject, eventData);
                if (_onDoubleClick != null) {
                    _onDoubleClick(eventData);
                }
            }
        }
    }

    public void OnGuideClick() 
    {
        if (_onClick != null) 
        {
            _onClick(null);
        }
    }

    public override void OnPointerDown(PointerEventData eventData) {
        base.OnPointerDown(eventData);
        if (CanReceiveEvent(eventData)) {
            _pointDownTag = true;
            _isFirstLongClick = true;
            //UIEventListener.Print("Down", gameObject, eventData);
            if (_onDown != null) {
                _onDown(eventData);
            }
        }
    }

    public override void OnPointerUp(PointerEventData eventData) {
        base.OnPointerUp(eventData);
        _pointDownTag = false;
        _isFirstLongClick = false;
        _frameCount = 0;
        _frameAdd = longClickThreshold;
        if (CanReceiveEvent(eventData)) {
            //UIEventListener.Print("Up", gameObject, eventData);
            if (_onUp != null) {
                _onUp(eventData);
            }
        }
    }

    public override void OnPointerExit(PointerEventData ecData) {
        base.OnPointerExit(ecData);
        if (_onPointExit != null) {
            _onPointExit(null);
        }
    }

    protected override void Start() {
        base.Start();
        if (Application.isPlaying) {
            OpenButtonEffect = _openButtonEffect;
        }
        if (label != null) label.raycastTarget = false;
        if (icon != null) icon.raycastTarget = false;
        if (redTip != null) redTip.raycastTarget = false;
    }

    void Update() {
        if (_pointDownTag) {
            _frameCount += 1;
            if (_frameCount > longClickThreshold) {
                //首次长按事件（触发一次）
                if (_isFirstLongClick) {
                    _isFirstLongClick = false;
                    if (_onFirstLongClick != null) {
                        _onFirstLongClick(this);
                    }
                }

                _frameAdd -= 0.25f;
                if (_frameAdd <= 5) {
                    _frameAdd = 5;
                }
                if (_frameCount % Convert.ToInt32(_frameAdd) == 0) {
                    //长按事件（每帧触发）
                    if (_onLongClick != null) {
                        _onLongClick();
                    }
                }
            }
        }
    }

    protected override void OnDestroy() {
        base.OnDestroy();
        _onDown = null;
        _onUp = null;
        _onClick = null;
        _onDoubleClick = null;
        _onLongClick = null;
        _onFirstLongClick = null;
        _timeProtectCallBack = null;
        _onPointExit = null;
    }

    #endregion
}