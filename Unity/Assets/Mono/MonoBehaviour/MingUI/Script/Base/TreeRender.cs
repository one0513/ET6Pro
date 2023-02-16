using System;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// c#层tree的TreeRender基类
/// 同时也是lua层的TreeRender的代理类
/// </summary>
public class TreeRender : BaseRender
{
    private int _level;
    private string _treeBgPath;
    private string _childRenderName;
    private int _childTreeHeight;
    private bool _isShowChildTree;


    private CTree _childTreeComphonet; //孩子节点只保留整颗树

    private List<object> _childTreeData;//子树数据源

    /// <summary>
    /// 所属层级
    /// </summary>
    public int Level
    {
        
        set { _level = value; }
    }

    /// <summary>
    /// 子树的
    /// </summary>
    public string TreeBgPath {
 
        set { _treeBgPath = value; }
    }

    /// <summary>
    /// 子树
    /// </summary>
    public CTree ChildTree
    {
        get { return _childTreeComphonet; }
        set { _childTreeComphonet = value; }
    }

    /// <summary>
    /// 子树数据
    /// </summary>
    /// <returns></returns>
    public List<object> ChildData
    {

        set
        {
            _childTreeData = value;
        }
    }

    /// <summary>
    /// 判断是否有子树数据
    /// </summary>
    /// <returns></returns>
    public bool HasChildData()
    {
        return false;
    }

    /// <summary>
    /// 读写子树的预设名
    /// </summary>
    public string ChildRenderName
    {
       
        set { _childRenderName = value; }
    }

    /// <summary>
    /// 读写子树的渲染类
    /// </summary>
    public Type ChildRenderType { get; set; }

 

    /// <summary>
    /// 读写子树的高度，如果不设置则默认显示全部子项
    /// </summary>
    public int ChildTreeHeight {
       
        set { _childTreeHeight = value; }
    }

    public List<object> ChildClickStack { get; set; }
    public bool HasChildStack { get; set; }


    /// <summary>
    /// 读写是否正在显示子树
    /// </summary>
    public bool IsShowChildTree
    {
        get { return _isShowChildTree; }
    
    }

    public void CreatChildTree(Action<GameObject> callBack)
    {
        if (HasChildData())
        {
            MingUIAgent.LoadUIPrefab("MingUI.CTree", itemVo =>
            {
                GameObject childTreeObj = itemVo.getInstance() as GameObject;
                if (childTreeObj!=null)
                {
                    _childTreeComphonet = childTreeObj.GetComponent<CTree>();
                    callBack(childTreeObj);
                }
            });
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        
        _childTreeComphonet = null;
        _childTreeData = null;
        if (ChildClickStack!=null)
        {
            ChildClickStack.Clear();   
        }
        ChildClickStack = null;
    }
}

