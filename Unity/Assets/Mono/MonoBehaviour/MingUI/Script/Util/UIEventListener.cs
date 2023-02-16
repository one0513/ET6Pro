using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIEventListener
{
    //所有的UI事件委托类型
    public delegate void VoidDelegate();

    public delegate void ObjectDelegate(object value);

    public delegate void FloatDelegate(float value);

    public delegate void BoolDelegate(bool value);

    public delegate void IntDelegate(int value);

    public delegate void StringDelegate(string value);

    public delegate void Vector2Delegate(Vector2 value);

    public delegate void Vector3Delegate(Vector3 value);

    public delegate void GameObjectDelegate(GameObject value);

    public delegate void PointerDelegate(PointerEventData value);

    public static void Print(string type, GameObject gameObject, PointerEventData data)
    {
        Debug.Log("[" + type + "]" + gameObject.name + "##" + data);
    }

    #region 按钮行为的事件：[按下、弹起、移动、选择、进出、点击、长按]

    /// <summary>
    /// 添加按下
    /// </summary>
    /// <param name="go"></param>
    /// <param name="handler"></param>
    static public CButton AddMouseDown(GameObject go, PointerDelegate handler)
    {
        CButton button = go.GetComponent<CButton>();
        if (button == null) button = go.AddComponent<CButton>();
        button.AddMouseDown(handler);
        return button;
    }

    /// <summary>
    /// 添加弹起
    /// </summary>
    /// <param name="go"></param>
    /// <param name="handler"></param>
    static public CButton AddMouseUp(GameObject go, PointerDelegate handler)
    {
        CButton button = go.GetComponent<CButton>();
        if (button == null) button = go.AddComponent<CButton>();
        button.AddMouseUp(handler);
        return button;
    }

    /// <summary>
    /// 添加单击
    /// </summary>
    /// <param name="go"></param>
    /// <param name="handler"></param>
    static public CButton AddClick(GameObject go, PointerDelegate handler)
    {
        CButton button = go.GetComponent<CButton>();
        if (button == null) button = go.AddComponent<CButton>();
        button.AddClick(handler);
        return button;
    }

    /// <summary>
    /// 添加双击
    /// </summary>
    /// <param name="go"></param>
    /// <param name="handler"></param>
    static public CButton AddDoubleClick(GameObject go, PointerDelegate handler)
    {
        CButton button = go.GetComponent<CButton>();
        if (button == null) button = go.AddComponent<CButton>();
        button.AddDoubleClick(handler);
        return button;
    }

    public static CButton AddLongClick(GameObject go,  Action handler)
    {
        var button = go.GetComponent<CButton>() ?? go.AddComponent<CButton>();
        button.AddLongClick(handler);
        return button;
    }

    #endregion

    #region 拖拽行为的事件：[拖拽初始化、开始拖拽、拖拽中、拖拽结束]

    /// <summary>
    /// 添加拖拽
    /// </summary>
    /// <param name="go"></param>
    /// <param name="handler"></param>
    static public CDrag AddDrag(GameObject go, PointerDelegate handler)
    {
        CDrag drag = go.GetComponent<CDrag>();
        if (drag == null) drag = go.AddComponent<CDrag>();
        drag.AddDrag(handler);
        return drag;
    }

    static public void ChangeDragMode(GameObject go, int dragMode)
    {
        CDrag drag = go.GetComponent<CDrag>();
        if (drag != null)
        {
            if (dragMode == 0) drag.mode = CDrag.DragMode.None;
            if (dragMode == 1) drag.mode = CDrag.DragMode.Entity;
            if (dragMode == 2) drag.mode = CDrag.DragMode.Clone;
        }
    }

    #endregion

    #region 投放行为的事件：[投放]

    /// <summary>
    /// 添加拖拽
    /// </summary>
    /// <param name="go"></param>
    /// <param name="handler"></param>
    static public CDrop AddDrop(GameObject go, ObjectDelegate handler)
    {
        CDrop drop = go.GetComponent<CDrop>();
        if (drop == null) drop = go.AddComponent<CDrop>();
        drop.AddDrop(handler);
        return drop;
    }

    #endregion

    #region 选择行为
    /// <summary>
    /// 添加选择
    /// </summary>
    /// <param name="go"></param>
    /// <param name="handler"></param>
    static public CSelect AddSelect(GameObject go, BoolDelegate handler)
    {
        CSelect select = go.GetComponent<CSelect>();
        if (select == null) select = go.AddComponent<CSelect>();
        select.AddSelect(handler);
        return select;
    }
    #endregion
    #region 进入、退出
    /// <summary>
    /// 添加进入
    /// </summary>
    /// <param name="go"></param>
    /// <param name="handler"></param>
    static public CEnterExit AddEnter(GameObject go, PointerDelegate handler)
    {
        CEnterExit mono = go.GetComponent<CEnterExit>();
        if (mono == null) mono = go.AddComponent<CEnterExit>();
        mono.AddEnter(handler);
        return mono;
    }
    /// <summary>
    /// 添加退出
    /// </summary>
    /// <param name="go"></param>
    /// <param name="handler"></param>
    static public CEnterExit AddExit(GameObject go, PointerDelegate handler)
    {
        CEnterExit mono = go.GetComponent<CEnterExit>();
        if (mono == null) mono = go.AddComponent<CEnterExit>();
        mono.AddExit(handler);
        return mono;
    }
    #endregion
}