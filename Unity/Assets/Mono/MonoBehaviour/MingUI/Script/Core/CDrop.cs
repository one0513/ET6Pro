using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 可投放脚本
/// </summary>
[AddComponentMenu("MingUI/CDrop", 3)]
public class CDrop : MonoBehaviour, IDropHandler
{
    private UIEventListener.ObjectDelegate _onDrop;

    #region 添加事件

    public void AddDrop(UIEventListener.ObjectDelegate callback)
    {
        _onDrop = callback;
    }

    #endregion

    #region 内部接口

    public void OnDrop(PointerEventData eventData)
    {
        if (DragManager.IsDraging())
        {
            //UIEventListener.Print("Drop", gameObject, eventData);
            if (_onDrop != null)
            {
                _onDrop(DragManager.DragData);
            }
        }
    }

    protected void OnDestroy()
    {
        _onDrop = null;
    }

    #endregion
}