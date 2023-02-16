using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 滚动条
/// </summary>
[AddComponentMenu("MingUI/CScrollBar", 30)]
public class CScrollBar : Scrollbar
{
    private UIEventListener.FloatDelegate _onChange;

    public void AddChange(UIEventListener.FloatDelegate fun)
    {
        _onChange = fun;
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