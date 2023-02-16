using UnityEngine.EventSystems;

public class CEnterExit:UIBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private UIEventListener.PointerDelegate _onEnter;
    private UIEventListener.PointerDelegate _onExit;

    /// <summary>
    /// 添加进入
    /// </summary>
    /// <param name="callback"></param>
    public void AddEnter(UIEventListener.PointerDelegate callback)
    {
        _onEnter = callback;
    }

    /// <summary>
    /// 移除进入
    /// </summary>
    public void RemoveEnter()
    {
        _onEnter = null;
    }

    /// <summary>
    /// 添加退出
    /// </summary>
    /// <param name="callback"></param>
    public void AddExit(UIEventListener.PointerDelegate callback)
    {
        _onExit = callback;
    }

    /// <summary>
    /// 移除退出
    /// </summary>
    public void RemoveExit()
    {
        _onExit = null;
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        if (_onEnter != null)
        {
            _onEnter(eventData);
        }
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        if (_onExit != null)
        {
            _onExit(eventData);
        }
    }
}

