using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 下拉框
/// 宽度和位置直接修改预设即可
/// 高度已经做了自适应
/// </summary>
public class CComboBox : MonoBehaviour, ISelectHandler,IDeselectHandler
{ 
    public enum Position
    {
        Above, //朝上展开
        Below, //朝下展开
    }

    public enum ComboStyle
    {
        Choose, //多个选择的样式
        Menu, //菜单选择的样式
    }

    public CButton button;
    protected UIEventListener.BoolDelegate onListOpen; //List是否打开回调
    public CSprite arrowSprite;
    public bool isFixListHeigth;//是否固定列表高度，不根据数据源的长度变化
    public CList list;
    private CanvasGroup canvasGroup;
    public ComboStyle style = ComboStyle.Choose;
    public bool autoChooseIndex = true;
    [SerializeField] private Position _pos = Position.Below;

    public Position Pos
    {
        get { return _pos; }
        set
        {
            _pos = value;
            if (Pos == Position.Above) //从上面打开
            {
                list.SelfTransform.pivot = new Vector2(0.5f, 0);
            }
            else //从下面打开
            {
                list.SelfTransform.pivot = new Vector2(0.5f, 1);
            }
        }
    }

    public float duration = 0.15f;
    public float maxHeight = 0f; //0代表自适应高度

    private bool _isOpen; //是否处于打开状态？
    private int _preSelect = -1;
    private UIEventListener.ObjectDelegate _onSelect;
    private List<object> _dataProvider; //数据源
    private Canvas _canvas;

    public string Text
    {
        get { return button.Text; }
        set {
            button.Text = value; 
        }
    }

    public int SelectIndex
    {
        get { return list.SelectIndex; }
        set { list.SelectIndex = value; }
    }
    

    /// <summary>
    /// 设置lua的render
    /// </summary>
    public BaseRender ItemRender
    {
        get { return Activator.CreateInstance(list.itemRender) as BaseRender; }
        set
        {
            list.itemRender = value.GetType();
        }
    }

    /// <summary>
    /// 设置数据源(c#层)
    /// </summary>
    public List<object> DataProvider
    {
        get { return _dataProvider; }
        set
        {
            list.DataProvider = value;
           
            if (!isFixListHeigth)
            {
                RectTransform maskTrans = list.mask;
                float maskBottomOffset = Mathf.Max(maskTrans.offsetMin.y, 0);
                float maskTopOffset = Mathf.Max(-maskTrans.offsetMax.y, 0);
                float nowHeight = value.Count * (list.ItemHeight + list.pad.y) + maskBottomOffset + maskTopOffset - list.pad.y;
                if (maxHeight <= 0 || nowHeight <= maxHeight)
                {
                    list.Height = nowHeight;
                }
                else
                {
                    list.Height = maxHeight;
                }
            }

            if (autoChooseIndex && style == ComboStyle.Choose && value.Count > 0)
            {
                list.SelectIndex = 0; //默认选中第0个
            }
        }
    }

    public void SelectIndexWithoutFunc(int index) {
        list.SelectIndexWithoutFunc(index);
      
    }

    public void AddSelect(UIEventListener.ObjectDelegate callback)
    {
        _onSelect = callback;
    }

    public void RemoveSelect()
    {
        _onSelect = null;
    }

    private void Awake()
    {
        _isOpen = true;
        Close();
        button.AddClick(OnButtonClick);
        list.AddSelect(OnListSelect);
        list.AddDeSelect(OnListDeSelect);
        _canvas = list.GetComponent<Canvas>();
        canvasGroup = list.GetComponent<CanvasGroup>();
    }

    private void OnListDeSelect(int index)
    {
        _preSelect = -1;
    }

    private void OnDestroy()
    {
        _dataProvider = null;
        _onSelect = null;
    }

    public void AddListOpen(UIEventListener.BoolDelegate callback) {
        onListOpen = callback;
    }

    private void OnButtonClick(PointerEventData data)
    {
        _canvas.sortingLayerName = "Top";
        _canvas.sortingOrder = 30000;
        list.gameObject.SetActive(true);
        if (arrowSprite != null)
        {
            arrowSprite.rectTransform.localRotation = new Quaternion(0,0,180,0);
        }
        _isOpen = !_isOpen;
        if (_isOpen)
        {
            Expand();
        }
        else
        {
            Shink();
        }
        EventSystem.current.SetSelectedGameObject(gameObject);
    }

    private void OnListSelect(int index)
    {
     
    }

    private void OnTweenComplete()
    {
        if (_isOpen)
        {
        }
        else
        {
            if (arrowSprite != null)
            {
                arrowSprite.rectTransform.localRotation = new Quaternion(0, 0, 0, 0);
            }
            list.gameObject.SetActive(false);
        }
    }

    private void Expand()
    {
        _isOpen = true;
        if (onListOpen != null) onListOpen(true);
        if (list.SelfTransform != null)
        {
            DOTween.Kill(list.SelfTransform);
        }
        Tweener tweener = list.SelfTransform.DOScaleY(1, duration);
        tweener.OnComplete(OnTweenComplete);
        if (canvasGroup) {
            canvasGroup.alpha = 0;
            //canvasGroup.DOFade(1, duration);
        }
    }

    private void Shink()
    {
        _isOpen = false;
        if (onListOpen != null) onListOpen(false);
        if (list.SelfTransform != null)
        {
            DOTween.Kill(list.SelfTransform);
        }
        Tweener tweener = list.SelfTransform.DOScaleY(0, duration);
        tweener.OnComplete(OnTweenComplete);
        if (canvasGroup) {
            canvasGroup.alpha = 1;
            //canvasGroup.DOFade(0, duration);
        }
    }

    private void OnDisable()
    {
        Close();
    }
    
    /// <summary>
    /// 关闭
    /// </summary>
    private void Close()
    {
        if (_isOpen)
        {
            if (onListOpen != null) onListOpen(false);
            _isOpen = false;
            list.gameObject.SetActive(false);
            list.SelfTransform.localScale = new Vector3(1, 0, 1);
            if (arrowSprite != null)
            {
                arrowSprite.rectTransform.localRotation = new Quaternion(0, 0, 0, 0);
            }
        }
    }

    public virtual void OnSelect(BaseEventData eventData)
    {

    }

    public virtual void OnDeselect(BaseEventData eventData)
    {
        if ((eventData as PointerEventData).pointerCurrentRaycast.gameObject != null)
        {
            Transform target = (eventData as PointerEventData).pointerCurrentRaycast.gameObject.transform;

            if (target.IsChildOf(button.transform))
            {
                return;
            }
            Close();
        }
    }

}