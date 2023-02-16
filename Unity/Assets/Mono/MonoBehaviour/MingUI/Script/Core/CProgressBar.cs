using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 进度条
/// </summary>
[AddComponentMenu("MingUI/CProgressBar", 28)]
public class CProgressBar : Slider
{
    private UIEventListener.FloatDelegate _onChange;
    public CMeshProLabel label;
    private bool _gray;
    private CSprite current;
    private CSprite fill;
    private CMeshProLabel cLabel;
    public string Text
    {
        get { return label.text; }
        set { label.text = value; }
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
    /// <summary>
    /// 灰度化？
    /// </summary>
    public bool Gray
    {
        get { return _gray; }
        set
        {
            if (value != _gray)
            {
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
                SetGray(_gray);
#endif

            }
        }
    }

    private void SetGray(bool _gray)
    {
        if (current == null) current = transform.GetComponent<CSprite>();
        if (fill == null) fill = fillRect.GetComponent<CSprite>();
        if (label && cLabel == null) cLabel = label.transform.GetComponent<CMeshProLabel>();
        current.Gray = _gray;
        fill.Gray = _gray;
        if (cLabel) cLabel.Gray = _gray;
    }

    public void AddChange(UIEventListener.FloatDelegate fun)
    {
        _onChange += fun;
    }

    /// <summary>
    /// 设置Value，并播放动画
    /// </summary>
    /// <param name="endValue">目标值</param>
    /// <param name="nextLv">是否下一等级，true则先滚到1再滚到目标值</param>
    public void SetValueByTween(float endValue, bool nextLv)
    {
        StopTween();
        if (nextLv)
        {
            Tween tweener = SetEndTweenValue(1, 0.25f);
            tweener.OnComplete(() =>
            {
                value = 0;
                SetEndTweenValue(endValue, 0.25f);
            });
        }
        else
        {
            SetEndTweenValue(endValue, 0.5f);
        }
    }

    //蛋疼，valu的方法在底层，所以等写一个停止tween的方法
    public void StopTween()
    {
        if (DOTween.IsTweening(this) && this != null)
        {
            DOTween.Kill(this);
        }
    }

    private Tween SetEndTweenValue(float value, float time)
    {
        Tween tweener = this.DOValue(value, time);
        return tweener;
    }

   

    public void RemoveChange()
    {
        _onChange = null;
    }

    protected override void Awake()
    {
        base.Awake();
        onValueChanged.AddListener(OnChange);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        _onChange = null;
    }

    private void OnChange(float val)
    {
        if (_onChange != null)
        {
            _onChange(val);
        }
    }
}