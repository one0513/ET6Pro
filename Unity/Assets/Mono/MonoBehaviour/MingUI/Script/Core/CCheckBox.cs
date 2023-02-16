using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 复选框
/// </summary>
[AddComponentMenu("MingUI/CCheckBox", 27)]
public class CCheckBox : Toggle
{
    public CMeshProLabel label;
    public CMeshProEffectUGUI outLine;
    protected RectTransform labelTransform;
    private UIEventListener.BoolDelegate _onChange;
    private bool _ignoreCallBack;
    private string _sound = "audio_effect_ui_click";

    public string Sound
    {
        set { _sound = value; }
    }
    public string Text
    {
        get { return label.text; }
        set { label.text = value; }
    }
    /// <summary>
    /// 设置选中，但不派发事件
    /// </summary>
    /// <param name="index"></param>
    public void SetSelect(bool value)
    {
        _ignoreCallBack = true;
        isOn = value;
        _ignoreCallBack = false;
    }
    /// <summary>
    /// 监听改变事件
    /// </summary>
    /// <param name="fun"></param>
    public void AddChange(UIEventListener.BoolDelegate fun)
    {
        _onChange += fun;
    }

    /// <summary>
    /// 播放点击声音
    /// </summary>
    /// <param name="eventData"></param>
    public override void OnPointerDown(PointerEventData eventData)
    {
        //MingUIAgent.PlaySound(_sound);
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        MingUIAgent.PlaySound(_sound);
    }

    /// <summary>
    /// 移除改变事件
    /// </summary>
    public void RemoveChange()
    {
        _onChange = null;
    }

    protected override void Awake()
    {
        base.Awake();
        OnInit();
    }

    protected virtual void OnInit()
    {
        if (label != null)
        {
            labelTransform = label.GetComponent<RectTransform>();
            outLine = label.GetComponent<CMeshProEffectUGUI>();
        }
        onValueChanged.AddListener(OnChange);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        _onChange = null;
    }

    protected virtual void OnChange(bool value)
    {
        if (!_ignoreCallBack && _onChange != null)
        {
            _onChange(value);
        }
    }
}