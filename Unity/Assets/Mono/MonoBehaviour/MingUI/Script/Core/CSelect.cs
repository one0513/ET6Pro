using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class CSelect:UIBehaviour,ISelectHandler,IDeselectHandler
{
    public bool ignoreChildSelect = true;

    private UIEventListener.BoolDelegate _onSelect;

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
    public virtual void OnSelect(BaseEventData eventData)
    {
        if (_onSelect != null)
        {
            _onSelect(true);
        }
    }

    public virtual void OnDeselect(BaseEventData eventData)
    {
        if (ignoreChildSelect)
        {
            PointerEventData pEData = eventData as PointerEventData;

            if (pEData!=null && pEData.pointerCurrentRaycast.gameObject != null)
            {
                Transform target = pEData.pointerCurrentRaycast.gameObject.transform;

                if (target.IsChildOf(this.transform))
                {
                    StartCoroutine(ReturnSelect());
                    return;
                }
            }
        }
        if (_onSelect != null) {
            _onSelect(false);
        }
    }

    public void HandleSelectCallBack(bool isSelect) {
        if (_onSelect != null) {
            _onSelect(isSelect);
        }
    }

    public IEnumerator ReturnSelect()
    {
        yield return 0;
        MingUIUtil.SetSelectedGameObject(gameObject);
        yield return 0;
    }
}

