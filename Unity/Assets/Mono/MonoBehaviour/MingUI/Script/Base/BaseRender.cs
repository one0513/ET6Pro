
using UnityEngine;

/// <summary>
/// c#层list的BaseRender基类
/// 同时也是lua层的BaseRender的代理类
/// </summary>
public class BaseRender
{
    private object _data; //数据源
    private CBaseRender _render; //预设
    private RectTransform _selfTransfrom;



    public string prefabName; // 生成 CBaseRender 的GameObject 名字

    public object data
    {
        get { return _data; }
    }

    public RectTransform SelfTransform
    {
        get { return _selfTransfrom; }
    }
    /// <summary>
    /// 脏读情况
    /// </summary>
    public bool IsDirty { get; set; }

    /// <summary>
    /// 绑定lua类
    /// </summary>
   

    /// <summary>
    /// 1.设置render实例
    /// </summary>
    public CBaseRender Render
    {
        get;
        set;
    }

    /// <summary>
    /// 2.初始化UI
    /// </summary>
    public virtual void InitUI()
    {

    }

    /// <summary>
    /// 3.设置数据
    /// </summary>
    /// <param name="value"></param>
    public virtual void SetData(object value)
    {
       
    }

    public virtual void SetDataKv(string funName, object data) {
       
    }
    
    /// <summary>
    /// 数据回调
    /// </summary>
    /// <param name="value"></param>
    public virtual void OnGUI(object value)
    {

    }
    /// <summary>
    /// 销毁
    /// </summary>
    public virtual void OnDestroy()
    {
     
    }


    public void Recycle()
    {
        
        Render.Recycle();
    }
}