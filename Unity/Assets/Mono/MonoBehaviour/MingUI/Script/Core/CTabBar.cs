using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// 标签页(分横向：pivot在中下  ，纵向：pivot在左中)
/// </summary>
[AddComponentMenu("MingUI/CTabBar", 26)]
public class CTabBar : CCheckBox {
    //targetGraphic:背景图
    //graphic：勾选图
    public CSprite redTip;
    protected RectTransform redTipTransfrom;

    public Vector2 normalLabelPos; //正常时标签文本位置
    public Vector2 selectLabelPos; //选中时标签文本位置
    public Vector2 normaltTipPos; //正常时红点提示位置
    public Vector2 selectTipPos; //选中时红点提示位置

    public Color nomalLabelColor = Color.white;//正常时标签文本颜色
    public Color selectLabelColor = Color.white;//选中时标签文本颜色

    public Color nomalLabelOutLineColor = Color.white;//正常时标签文本颜色
    public Color selectLabeOutLinelColor = Color.white;//选中时标签文本颜色

    public float nomalLabelOutLineWidth = 2;//正常时标签文本描边宽度
    public float selectLabeOutLinelWidth = 2;//选中时标签文本描边宽度
    
    public GameObject backgroundObject;//背景
    public GameObject checkMarkObject;//选中时的物体
    public RectTransform rectTransform { private set; get; }

    private CButtonEffect effect;
    public CButtonEffect Effect { get => effect; private set => effect = value; }
    [SerializeField]
    private bool _openButtonEffect = true; //是否开启按钮点击效果
    [SerializeField]
    private bool _isHideBgOnSelected = false;//被选中时是否隐藏背景

    private bool isBackgroundShow;
    private static int _tweenID;

    private string GetTweenID() {
        return "CTabBar_" + _tweenID++;
    }
    private string tweenID;
    private bool isShowRedTip;
    private bool isInitShowRedTip;

    public bool OpenButtonEffect {
        get { return _openButtonEffect; }
        set {
            _openButtonEffect = value;
            if (Application.isPlaying) {
                if (_openButtonEffect) {
                    Effect = MingUIUtil.AddButtonEffect(gameObject);
                } else {
                    Effect = gameObject.GetComponent<CButtonEffect>();
                    if (Effect != null) {
                        Effect.enabled = false;
                    }
                }
            }
        }
    }

    public bool IsHideBgOnSelected
    {
        get { return _isHideBgOnSelected;}
        set {
            _isHideBgOnSelected = value;
        }
    }
    
    /// <summary>
    /// 设置标签栏文字 
    /// </summary>
    /// <param name="tabLabel"></param>
    public void SetTabBarLabel(string tabLabel) {
        label.text = tabLabel;
    }

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

    protected override void OnInit() {
        base.OnInit();
        if (redTip) {
            redTipTransfrom = redTip.GetComponent<RectTransform>();
        }
        rectTransform = GetComponent<RectTransform>();
        if (checkMarkObject != null) {
            checkMarkObject.SetActive(false);
        }
        if (backgroundObject != null) {
            backgroundObject.SetActive(true);
            isBackgroundShow = true;
        }
    }

    protected override void Start() {
        base.Start();
        base.Sound = "subTab";
        if (Application.isPlaying) {
            OpenButtonEffect = _openButtonEffect;
        }
        if (isOn)
        {
            OnChange(true);
        }
    }

    protected override void OnChange(bool value) {
        base.OnChange(value);
        if (Application.isPlaying) {
            if (value) { //选中 
                if (label) {
                    label.color = selectLabelColor;
                    labelTransform.anchoredPosition = selectLabelPos;
                }
                if (outLine)
                {
                    outLine.effectColor = selectLabeOutLinelColor;
                    outLine.OutlineWidth = selectLabeOutLinelWidth;
                }
                if (redTipTransfrom) {
                    redTipTransfrom.anchoredPosition = selectTipPos;
                }
            } else {
                if (label) {
                    label.color = nomalLabelColor;
                    labelTransform.anchoredPosition = normalLabelPos;
                }
                if (outLine)
                {
                    outLine.effectColor = nomalLabelOutLineColor;
                    outLine.OutlineWidth = nomalLabelOutLineWidth;
                }
                if (redTipTransfrom) {
                    redTipTransfrom.anchoredPosition = normaltTipPos;
                }
            }
            if (checkMarkObject != null) {
                checkMarkObject.SetActive(value);
            }
            if (_isHideBgOnSelected) BackgroundActive(!value);
            else BackgroundActive(true);
        }
    }

    private void BackgroundActive(bool active) {
        if (backgroundObject == null) return;
        if (isBackgroundShow == active) return;
        isBackgroundShow = active;
        backgroundObject.SetActive(active);
    }
    
}