
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 标签导航类容器
/// 1.实现动态显示实例化视图
/// 2.实现动态显示/隐藏视图及标签
/// </summary>
public class CTabNavigation : MonoBehaviour
{
    /// <summary>
    /// 容器的资源加载方式
    /// </summary>
    public enum BindMode
    {
        Directly,//直接预设关联
        ByLoad,//通过动态加载
    }
    public BindMode mode = BindMode.ByLoad;
    public CTabGroup tabBars; //标签页
    public List<GameObject> prefabList = new List<GameObject>(); //直接预设关联的预设列表
    public GameObject shieldMaskPrefab;//功能屏蔽蒙版(预设)
    public Vector3 shieldMaskPos = new Vector3(0,-16,0);
    public bool isHideLastView = true;
    public bool isLuaControlShow = false;
    private GameObject _shieldMask;//功能屏蔽蒙版实体
    private Dictionary<int,bool> _shieldDic = new Dictionary<int, bool>();//功能屏蔽索引

    private List<string> _assetList;//资源列表
    private List<string> AssetList
    {
        get
        {
            if (_assetList == null) _assetList = new List<string>();
            return _assetList;
        }
    }

    private List<BaseView> _viewList; //实例列表
    public List<BaseView> ViewList
    {
        get
        {
            if (_viewList == null) _viewList = new List<BaseView>();
            return _viewList;
        }
    }

    private int _selectIndex = -1;
    private UIEventListener.IntDelegate _onSelect;
    private UIEventListener.IntDelegate _onPreSelect;
    private UIEventListener.IntDelegate _onLastSelectBeforeHide;
    private bool _ignoreOnSelect;
    private bool _ignoreOnPreSelect;
    private bool _ignoreOnLastSelectBeforeHide;

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
    /// 添加视图（c#层）
    /// </summary>
    public void AddView(BaseView view)
    {
        view.ParentTransfrom = transform;
        ViewList.Add(view);
    }
    /// <summary>
    /// 添加视图（c#层）
    /// </summary>
    /// <param name="view"></param>
    /// <param name="assetName">资源名</param>
    public void AddView(BaseView view, string assetName)
    {
        AddView(view);
        AssetList.Add(assetName);
    }

    /// <summary>
    /// 设置选中，但不派发某些事件
    /// </summary>
    /// <param name="index"></param>
    /// <param name="ignoreOnLastSelecetBeforeHide"></param>
    /// <param name="ignoreOnPreSelect"></param>
    /// <param name="ignoreOnSelect"></param>
    public void SetSelect(int index, bool ignoreOnLastSelecetBeforeHide, bool ignoreOnPreSelect, bool ignoreOnSelect)
    {
        _ignoreOnSelect = ignoreOnSelect;
        _ignoreOnPreSelect = ignoreOnPreSelect;
        _ignoreOnLastSelectBeforeHide = ignoreOnLastSelecetBeforeHide;
        SelectIndex = index;
        _ignoreOnSelect = false;
        _ignoreOnPreSelect = false;
        _ignoreOnLastSelectBeforeHide = false;
    }

    /// <summary>
    /// 当前选中的标签
    /// </summary>
    public int SelectIndex
    {
        get { return _selectIndex; }
        set
        {
            if (_selectIndex != value && value >= 0 )//&& value < ViewList.Count) 
            {
                //0.派发事件（隐藏上次选中标签之前）
                if (_onLastSelectBeforeHide != null && !_ignoreOnLastSelectBeforeHide) _onLastSelectBeforeHide(_selectIndex);

                //1.隐藏之前的
                BaseView view = GetView(_selectIndex, false);
                if (view != null && isHideLastView )
                {
                    view.SetActive(false);
                }

                //2.更新
                _selectIndex = value;
                tabBars.SetSelect(_selectIndex);

                //3.派发事件（前置选择）
                if (_onPreSelect != null && !_ignoreOnPreSelect) _onPreSelect(_selectIndex);

                //4.功能屏蔽判断
                bool canShow = true;
                canShow = !CheckShield(_selectIndex);

                //5.功能开启判断
                if (canShow)
                {
                    view = GetView(_selectIndex, false);
                    if (view != null)
                    {
                        canShow = CheckOpen(view.LuaViewName);
                    }
                }

                //6.显示现在的，由于配合切换标签动效，若不需马上显示，由Lua在onSelect回调中进行控制显示
                if (canShow)
                {
                    view = GetView(_selectIndex, true);
                    if (view != null && !isLuaControlShow)
                    {
                        view.SetActive(canShow);
                    }
                }

                //7.派发事件
                if (_onSelect != null && !_ignoreOnSelect) _onSelect(_selectIndex);
            }
        }
    }

    /// <summary>
    /// 添加选中的回调
    /// </summary>
    /// <param name="selectFun"></param>
    public void AddSelect(UIEventListener.IntDelegate selectFun)
    {
        _onSelect = selectFun;
    }

    /// <summary>
    /// 选中后，先触发上个选中SelectIndex的回调
    /// </summary>
    /// <param name="selectFun"></param>
    public void AddLastSelectBeforeHide(UIEventListener.IntDelegate selectFun)
    {
        _onLastSelectBeforeHide = selectFun;
    }

    /// <summary>
    /// 删除选中回调
    /// </summary>
    public void RemoveSelect()
    {
        _onSelect = null;
    }
    /// <summary>
    /// 添加选中的回调
    /// </summary>
    /// <param name="selectFun"></param>
    public void AddPreSelect(UIEventListener.IntDelegate selectFun)
    {
        _onPreSelect = selectFun;
    }

    /// <summary>
    /// 添加选中的回调
    /// </summary>
    /// <param name="selectFun"></param>
    public void AddDoubleSelect(UIEventListener.IntDelegate selectFun)
    {
        tabBars.AddDoubleSelect(selectFun);
    }

    /// <summary>
    /// 删除选中回调
    /// </summary>
    public void RemoveDoubleSelect()
    {
        tabBars.RemoveDoubleSelect();
    }

    /// <summary>
    /// 设置标签的文字内容，有需求要在标签文字上显示(数字)，单个tab
    /// </summary>
    /// <param name="index"></param>
    /// <param name="tabLabel"></param>
    public void SetTabBarLabel(int index, string tabLabel)
    {
        if (index >= 0 )
        {
            tabBars.SetTabBarLabel(index, tabLabel);
        }
    }

   

    /// <summary>
    /// 动态显示隐藏标签页及内容
    /// </summary>
    /// <param name="index"></param>
    /// <param name="visible"></param>
    public void SetVisible(int index, bool visible)
    {
        if (index >= 0 && index < ViewList.Count)
        {
            tabBars.SetVisible(index, visible);
            if (index == _selectIndex && visible == false)
            {
                BaseView view = GetView(index, false);
                if (view != null)
                {
                    view.SetActive(false);
                }
            }
        }
    }

    /// <summary>
    /// 动态显示隐藏所有标签栏
    /// </summary>
    /// <param name="visible"></param>
    public void SetTabBarsVisible(bool visible)
    {
        tabBars.gameObject.SetActive(visible);
    }

    /// <summary>
    /// 设置数据源
    /// </summary>
    /// <param name="index"></param>
    /// <param name="data"></param>
    public void SetData(int index, object data)
    {
        if (index >= 0 && index < ViewList.Count)
        {
            BaseView view = GetView(index, false);
            if (view != null)
            {
                view.SetData(data);
            }
        }
    }

    /// <summary>
    /// 显示红点
    /// </summary>
    /// <param name="index"></param>
    /// <param name="visible"></param>
    public void ShowRedTip(int index, bool visible)
    {
        tabBars.ShowRedTip(index, visible);
    }

    /// <summary>
    /// 屏蔽标签？（屏蔽的话会显示屏蔽蒙版）
    /// </summary>
    /// <param name="index"></param>
    /// <param name="shield">是否屏蔽</param>
    public void ShieldTab(int index, bool shield)
    {
        bool preBool = _shieldDic.ContainsKey(index) && _shieldDic[index];
        _shieldDic[index] = shield;
        bool nowBool = CheckShield(index);
        if (preBool != nowBool)
        {
            Refresh();
        }
    }
    /// <summary>
    /// 刷新下当前标签页的显示
    /// </summary>
    public void Refresh()
    {
        int nowSelect = _selectIndex;
        _selectIndex = -1;
        SelectIndex = nowSelect;
    }
    /// <summary>
    /// 功能屏蔽检测（这里的逻辑应该像功能开启逻辑一样处理比较好，哎，暂时先这样吧）
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    private bool CheckShield(int index)
    {
        bool shield = _shieldDic.ContainsKey(index) && _shieldDic[index];

        if (index == SelectIndex)
        {
            if (shield && _shieldMask == null && shieldMaskPrefab != null)
            {
                _shieldMask = Instantiate(shieldMaskPrefab);
                _shieldMask.transform.SetParent(transform);
                _shieldMask.transform.localScale = Vector3.one;
                _shieldMask.transform.localEulerAngles = Vector3.zero;
                _shieldMask.transform.localPosition = shieldMaskPos;
            }
            if (_shieldMask != null)
            {
                _shieldMask.SetActive(shield);
                if (shield)
                {
                    Canvas canvas = _shieldMask.GetComponent<Canvas>();
                    canvas.sortingOrder = MingUIUtil.GetCPanelOrder(gameObject) + 501;
                }
            }
            return shield;
        }
        return false;
    }
    /// <summary>
    /// 功能开启检测（抛给上层业务逻辑来处理）
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    private bool CheckOpen(string key)
    {
        return MingUIAgent.CheckOpen(key, this);
    }

    private void Awake()
    {
        tabBars.AddSelect(OnSelect);
    }

    private void OnSelect(int index)
    {
        SelectIndex = index;
    }

    public BaseView GetView(int index, bool loadIfMissing)
    {
        if (index >= 0 && index < ViewList.Count)
        {
            BaseView view = ViewList[index];
            if (loadIfMissing && view != null && view.MyObj == null && !view.IsLoadingPrefab)
            {
                if (mode == BindMode.Directly)//预设直接关联
                {
                    GameObject go = Instantiate(prefabList[index]);
                    go.transform.SetParent(transform);
                    go.transform.localPosition = Vector3.zero;
                    go.transform.localScale = Vector3.one;

                    view.MyObj = go;
                }
                else//动态加载
                {
                    view.LoadPrefab(AssetList[index]);
                }
            }
            return view;
        }
        return null;
    }
    /// <summary>
    /// 销毁
    /// </summary>
    protected virtual void OnDestroy()
    {
        tabBars = null;
        prefabList.Clear();
        prefabList = null;
        if (_assetList != null)
        {
            _assetList.Clear();
            _assetList = null;
        }
        if (_viewList != null)
        {
            foreach (BaseView baseView in _viewList)
            {
                baseView.OnDestroy();
            }
            _viewList.Clear();
            _viewList = null;
        }
    }
}