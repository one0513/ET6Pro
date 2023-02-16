using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 拖拽
/// 支持功能：
/// 1.实体/克隆拖拽
/// 2.拖拽边界设定
/// 3.中心/任意拖拽
/// </summary>
[AddComponentMenu("MingUI/CDrag", 2)]
public class CDrag : MonoBehaviour,
    IInitializePotentialDragHandler,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler,
    IPointerDownHandler,
    IPointerUpHandler
{
    public enum DragMode
    {
        None,//无实质效果
        Entity, //实体拖拽
        Clone, //克隆拖拽
    }

    public DragMode mode = DragMode.Entity; //拖拽模式
    public bool dragAble = true; //可否拖拽
    public bool dragOnCenter = false; //拖拽时是否自动吸附到对象中心
    public RectTransform boundTransform; //拖拽区域（可无）
    public RectTransform target; //拖拽/克隆目标
    public float cloneScale = 1; //克隆目标大小

    private UIEventListener.PointerDelegate _onInitDrag;
    private UIEventListener.PointerDelegate _onBeginDrag;
    private UIEventListener.PointerDelegate _onDrag;
    private UIEventListener.PointerDelegate _onEndDrag;
    private UIEventListener.PointerDelegate _onDown;
    private UIEventListener.PointerDelegate _onUp;

    private readonly Vector3[] _corners = new Vector3[4];
    private Vector2 _dragOffset;
    private Vector3 _mousePos;
    private GameObject _cloneInstance; //克隆实例
    private RectTransform _dragTargetTrans; //拖拽对象
    private object _data;

    private RectTransform _selfTransform;

    public RectTransform SelfTransform
    {
        get
        {
            if (_selfTransform == null)
            {
                _selfTransform = GetComponent<RectTransform>();
            }
            return _selfTransform;
        }
    }

    /// <summary>
    /// 设置拖拽数据（必须在拖拽前设置）
    /// </summary>
    public object Data
    {
        set { _data = value; }
    }

    #region 添加事件

    public void AddInitDrag(UIEventListener.PointerDelegate callback)
    {
        _onInitDrag = callback;
    }

    public void RemoveInitDrag()
    {
        _onInitDrag = null;
    }

    public void AddBeginDrag(UIEventListener.PointerDelegate callback)
    {
        _onBeginDrag = callback;
    }

    public void RemoveBeginDrag()
    {
        _onBeginDrag = null;
    }

    public void AddDrag(UIEventListener.PointerDelegate callback)
    {
        _onDrag = callback;
    }

    public void RemoveDrag()
    {
        _onDrag = null;
    }

    public void AddEndDrag(UIEventListener.PointerDelegate callback)
    {
        _onEndDrag = callback;
    }

    public void RemoveEndDrag()
    {
        _onEndDrag = null;
    }

    public void AddUpDown(UIEventListener.PointerDelegate upFunc, UIEventListener.PointerDelegate downFunc)
    {
        _onUp = upFunc;
        _onDown = downFunc;
    }

    public void RemoveUpDown()
    {
        _onUp = null;
        _onDown = null;
    }

    #endregion

    #region 更新位置（防止出边界）
    public void UpdateBorder()
    {
        if (mode == DragMode.Entity)
        {
            _dragTargetTrans = target != null ? target : SelfTransform;
        }
        Vector3 currentPos = _dragTargetTrans.anchoredPosition;
        Vector3 fixPos = FixTargetTransPos(_dragTargetTrans.anchoredPosition);
        if (fixPos.x != currentPos.x || fixPos.y != currentPos.y)
        {
            _dragTargetTrans.localPosition = fixPos;
        }
    }
    #endregion
    #region 内部接口

    /// <summary>
    /// 初始化拖拽
    /// </summary>
    /// <param name="eventData"></param>
    public void OnInitializePotentialDrag(PointerEventData eventData)
    {
        if (dragAble == false) return;
        //UIEventListener.Print("InitDrag", MyObj, eventData);
        if (_onInitDrag != null)
        {
            _onInitDrag(eventData);
        }

        if (mode == DragMode.Entity)
        {
            _dragTargetTrans = target != null ? target : SelfTransform;
            InitDragTargetPos(eventData);
        }
    }

    private Transform _dragParent;
    public Transform DragParent
    {
        get { return _dragParent ?? (_dragParent = UIRoot.rootTransform); }
        set { _dragParent = value; }
    }

    /// <summary>
    /// 开始拖拽
    /// </summary>
    /// <param name="eventData"></param>
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (dragAble == false || Input.touchCount > 1) return;
        //UIEventListener.Print("BeginDrag", MyObj, eventData);
        if (_onBeginDrag != null)
        {
            _onBeginDrag(eventData);
        }
        if (mode == DragMode.None) return;

        if (mode == DragMode.Clone)
        {
            if (target == null)
            {
                Debug.LogError("there is no target to clone!");
                return;
            }
            if (_cloneInstance != null) Destroy(_cloneInstance);
            _cloneInstance = Instantiate(target.gameObject);
            _cloneInstance.SetActive(true);
            _cloneInstance.transform.SetParent(DragParent);
            _cloneInstance.transform.localScale = target.localScale * cloneScale;


            Vector3 targetPos = target.position;
            targetPos.z = 0;
            _cloneInstance.transform.position = targetPos;
            targetPos = _cloneInstance.transform.localPosition + (Vector3)eventData.delta;
            _cloneInstance.transform.localPosition = targetPos;

            _dragTargetTrans = _cloneInstance.GetComponent<RectTransform>();
            _dragTargetTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, target.rect.width);
            _dragTargetTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, target.rect.height);
            InitDragTargetPos(eventData);
        }
        MingUIUtil.EnableEvent(_dragTargetTrans.gameObject, false, false);
        DragManager.StartDrag(gameObject, _data);
    }

    /// <summary>
    /// 拖拽中
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {
        if (dragAble == false || Input.touchCount > 1) return;
        //UIEventListener.Print("Drag", MyObj, eventData);
        if (_onDrag != null)
        {
            _onDrag(eventData);
        }
        if (mode == DragMode.None) return;
        if (_dragTargetTrans == null) return;
        //计算鼠标屏幕坐标转换为父容器的相对坐标
        RectTransformUtility.ScreenPointToWorldPointInRectangle(_dragTargetTrans, eventData.position, eventData.pressEventCamera, out _mousePos);
        _mousePos = _dragTargetTrans.parent.InverseTransformPoint(_mousePos);
        _mousePos.x -= _dragOffset.x;
        _mousePos.y -= _dragOffset.y;
        _mousePos.z = 0;
        _dragTargetTrans.localPosition = FixTargetTransPos(_mousePos);
    }

    private Vector3 FixTargetTransPos(Vector3 localPos)
    {
        if (boundTransform != null)
        {
            Rect selfRect = _dragTargetTrans.rect;
            Vector2 pivot = _dragTargetTrans.pivot;

            Matrix4x4 toLocal = _dragTargetTrans.parent.worldToLocalMatrix;
            boundTransform.GetWorldCorners(_corners); //拿到边界的4个世界点

            Vector2 boundMin = toLocal.MultiplyPoint3x4(_corners[0]); //边界转换到父对象坐标系(左下角)
            Vector2 boundMax = toLocal.MultiplyPoint3x4(_corners[2]); //边界转换到父对象坐标系（右上角）

            float leftOffset = selfRect.width * pivot.x;
            float rightOffset = selfRect.width * (1 - pivot.x);
            float bottomOffset = selfRect.height * pivot.y;
            float topOffset = selfRect.height * (1 - pivot.y);

            if (localPos.x - leftOffset < boundMin.x) //最左
            {
                localPos.x = boundMin.x + leftOffset;
            }
            else if (localPos.x + rightOffset > boundMax.x) //最右
            {
                localPos.x = boundMax.x - rightOffset;
            }

            if (localPos.y - bottomOffset < boundMin.y) //最下
            {
                localPos.y = boundMin.y + bottomOffset;
            }
            else if (localPos.y + topOffset > boundMax.y) //最上
            {
                localPos.y = boundMax.y - topOffset;
            }
        }
        return localPos;
    }


    /// <summary>
    /// 结束拖拽
    /// </summary>
    /// <param name="eventData"></param>
    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragAble == false) return;
        //UIEventListener.Print("EndDrag", MyObj, eventData);
        if (_onEndDrag != null)
        {
            _onEndDrag(eventData);
        }
        if (mode == DragMode.None) return;

        DragManager.StopDarg(gameObject);
        if (_cloneInstance != null)
        {
            Destroy(_cloneInstance);
        }
        else
        {
            MingUIUtil.EnableEvent(_dragTargetTrans.gameObject, true, true);
        }
    }

    private void InitDragTargetPos(PointerEventData eventData)
    {
        if (mode == DragMode.None) return;
        if (dragOnCenter)
        {
            //当前点在拖拽父容器的坐标
            RectTransformUtility.ScreenPointToWorldPointInRectangle(_dragTargetTrans, eventData.position, eventData.pressEventCamera, out _mousePos);
            _mousePos = _dragTargetTrans.parent.InverseTransformPoint(_mousePos);
            _dragOffset = _dragTargetTrans.rect.center;
            _mousePos.x -= _dragOffset.x;
            _mousePos.y -= _dragOffset.y;
            _mousePos.z = 0;
            _dragTargetTrans.localPosition = _mousePos;
        }
        else
        {
            //当前点在自己的坐标系位置
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_dragTargetTrans, eventData.position, eventData.pressEventCamera, out _dragOffset);
        }
    }

    public void OnPointerDown(PointerEventData evtData)
    {
        if (_onDown != null)
        {
            _onDown(evtData);
        }
    }

    public void OnPointerUp(PointerEventData evtData)
    {
        if (_onUp != null)
        {
            _onUp(evtData);
        }
    }

    protected void OnDestroy()
    {
        _data = null;
        _onInitDrag = null;
        _onBeginDrag = null;
        _onDrag = null;
        _onEndDrag = null;
        target = null;
        boundTransform = null;
        _dragTargetTrans = null;
        if (_cloneInstance != null) Destroy(_cloneInstance);
        _cloneInstance = null;
    }

    public void SetTargetVisible(bool show)
    {
        if (mode == DragMode.Clone)
        {
            if (show != _cloneInstance.activeSelf)
            {
                _cloneInstance.SetActive(show);
            }
        }
    }

    #endregion
}