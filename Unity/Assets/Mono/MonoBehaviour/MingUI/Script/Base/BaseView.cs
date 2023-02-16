using System;
using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// c#层panel里的基本视图类
/// 同时也是lua层的BaseView的代理类
/// </summary>
public class BaseView
{
    private GameObject _myObj;
    private RectTransform _selfTransfrom;
    private object _data; //数据源
    private bool _isLoading;

  

    public object data
    {
        get { return _data; }
    }

    public RectTransform SelfTransform
    {
        get { return _selfTransfrom; }
    }

    public Transform ParentTransfrom { get; set; }

    public string LuaViewName
    {
        get;
    }


    /// <summary>
    /// 是否正在加载预设
    /// </summary>
    public bool IsLoadingPrefab
    {
        get;

    }
    /// <summary>
    /// 动态加载
    /// </summary>
    /// <param name="assetName"></param>
    public virtual void LoadPrefab(string assetName)
    {
        _isLoading = true;
        MingUIAgent.LoadUIPrefab(assetName, LoadComplete);//c#层加载
    }

    protected virtual void LoadComplete(ItemVo vo)
    {
        _isLoading = false;
        GameObject go = vo.getInstance() as GameObject;
        if (go)
        {
            go.transform.SetParent(ParentTransfrom);
            go.transform.localEulerAngles = Vector3.zero;
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;
            MyObj = go;
        }
    }
    /// <summary>
    /// 1.设置实例
    /// </summary>
    public GameObject MyObj
    {
        get;
        set;
    }

    /// <summary>
    /// 2.初始化UI
    /// </summary>
    protected virtual void InitUI()
    {

    }

    /// <summary>
    /// 3.设置数据
    /// </summary>
    /// <param name="value"></param>
    public void SetData(object value)
    {
      
    }
    /// <summary>
    /// 数据回调
    /// </summary>
    /// <param name="value"></param>
    public virtual void OnGUI(object value)
    {
        
    }
    /// <summary>
    /// 设置可见
    /// </summary>
    /// <param name="value"></param>
    public void SetActive(bool value)
    {
        
    }
    /// <summary>
    /// 销毁
    /// </summary>
    public virtual void OnDestroy()
    {
       
    }
}