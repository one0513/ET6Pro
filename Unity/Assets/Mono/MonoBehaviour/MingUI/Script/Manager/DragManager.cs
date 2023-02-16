using UnityEngine;

public class DragManager
{
    private static RectTransform _dragLayer;
    private static GameObject _currentDrag; //当前拖拽目标
    private static object _currentData; //拖拽携带的数据

    public static bool IsDraging()
    {
        return _currentDrag != null;
    }

    public static object DragData
    {
        get { return _currentData; }
    }

    /// <summary>
    /// 实体拖拽
    /// </summary>
    /// <param name="go"></param>
    /// <param name="data"></param>
    public static void StartDrag(GameObject go, object data)
    {
        if (IsDraging() == false)
        {
            _currentDrag = go;
            _currentData = data;
        }
    }

    /// <summary>
    /// 结束拖拽
    /// </summary>
    /// <param name="go"></param>
    public static void StopDarg(GameObject go)
    {
        if (_currentDrag == go)
        {
            _currentDrag = null;
            _currentData = null;
        }
    }
}