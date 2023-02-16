using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class CTree : CCanvas
{
    public CList.DragMode dragMode = CList.DragMode.Follow;//是否开启整render移动，若是，则移动结束后会自动吸附到该块（仅仅针对一行/一列）
    public float dragThreshold = 0.25f;//拖拽阈值（用于DragMode.Adhere或者FollowAndAdhere模式），Adhere：移动像素量达成则开启吸附|FollowAndAdhere：0.25是指当前移动超出该render25%区域时则自动向右/下吸附，否则还原

    public Vector2 pad;//间距
    public CBaseRender renderPrefab;//子对象渲染预设
    public Vector2 childTreeOffset;//子树间距
    public bool autoFold; //是否自动折叠（为true情况下每点击新的render会将之前展开的子树折叠）
    public bool autoSelectChildFirstRender; //展开子树是否自动选择第一个

    [NonSerialized]
    public Type itemRender = typeof(TreeRender);
    private bool _doSelectTag = false;
    protected int maxShowNum;//当前最多可显示多少个render
    private bool _dataChanged = true;//数据源是否改变？
    private int _startIndex = -1;//当前渲染的首索引
   
    protected List<object> dataProvider;//数据源
    private Action<int, int, object> _onSelect;//选中回调（返回索引）
    private UIEventListener.IntDelegate _onDeSelect;//取消选中回调（返回索引）

    protected List<int> hasSelectList;//多选索引列表

    private readonly List<TreeRender> _drawingRenders = new List<TreeRender>();//正在渲染的render
    private readonly List<TreeRender> _availableRenders = new List<TreeRender>();//缓存的render
    protected Dictionary<int, bool> selectIndexDic = new Dictionary<int, bool>();//选中的索引字典
    public Dictionary<int, TreeRender> renderIndexDic = new Dictionary<int, TreeRender>();//正在渲染的render的索引字典
    private readonly List<CTree>  _availableCtree = new List<CTree>(); //当前树的子树池


    public int level = 1; //初始树的层级
    public bool canDrag = true; //是否可拖拽
    public CTree parentTree; //父级数
    public CBaseRender parentRender; //当前树相对于父级树的挂载baserender


    //数据总项
    public int DataCount { get { return dataProvider == null ? 0 : dataProvider.Count; } }
    //子项宽度
    public float ItemWidth { get { return renderPrefab.Width; } }
    //子项高度
    public float ItemHeight { get { return renderPrefab.Height; } }
    //子项锚点偏移量
    public Vector2 ItemOffset { get { return new Vector2(ItemWidthOffset, -ItemHeightOffset); } }
    //子项锚点宽度偏移量
    public float ItemWidthOffset { get { return ItemWidth * renderPrefab.rectTransform.pivot.x; } }
    //子项锚点高度偏移量
    public float ItemHeightOffset { get { return ItemHeight * (1 - renderPrefab.rectTransform.pivot.x); } }


    #region 外部接口

    /// <summary>
    /// 设置树背景图路径
    /// </summary>
    public String TreeBgPath
    {
        set
        {
            CSprite bg = GetComponent<CSprite>();
            if (value == "")
                bg.enabled = false;
            else
                bg.Path = value;
        }
    }

 

    /// <summary>
    /// 设置数据源(c#层)
    /// </summary>
    public List<object> DataProvider
    {
        get { return dataProvider; }
        set
        {
            dataProvider = value;
            ClearSelect();//清除选择
            DataChanged();//设置脏读
        }
    }

    /// <summary>
    /// 通过索引获取数据
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public object GetItemData(int index)
    {
        return dataProvider != null ? dataProvider[index] : null;
    }

    /// <summary>
    /// 添加选中的回调
    /// </summary>
    /// <param name="callback"></param>
    public void AddSelect(Action<int, int, object> callback)
    {
        _onSelect = callback;
    }

    public void CallSelectFunc(int theIndex, int theLv, object theData)
    {
        if (_onSelect!=null)
        {
            _onSelect(theIndex, theLv, theData);
        }
    }

    /// <summary>
    /// 删除选中回调
    /// </summary>
    public void RemoveSelect()
    {
        _onSelect = null;
    }

    /// <summary>
    /// 添加取消选中的回调
    /// </summary>
    /// <param name="callback"></param>
    public void AddDeSelect(UIEventListener.IntDelegate callback)
    {
        _onDeSelect = callback;
    }

    /// <summary>
    /// 删除取消选中回调
    /// </summary>
    public void RemoveDeSelect()
    {
        _onDeSelect = null;
    }

    /// <summary>
    /// 定位到索引位置
    /// </summary>
    /// <param name="index"></param>
    /// <param name="anim"></param>
    /// <param name="callback"></param>
    public virtual void GotoIndex(int index, bool anim = false, TweenCallback callback = null)
    {
        if (index > _drawingRenders.Count - 1)
        {
            return;
        }
        Vector2 pos = GetRenderPos(index, Vector2.zero);
        pos *= -1;
        ScrollTo(pos, anim, callback);
    }

    public virtual void GotoIndex()
    {

    }

    /// <summary>
    /// 设置选中(单选)
    /// 无则返回-1
    /// </summary>
    public virtual int SelectIndex
    {
        get { return SelectList.Count > 0 ? SelectList[0] : -1; }
        set
        {
            if (SelectIndex != value)
            {
                SelectList = new List<int> { value };
            }
        }
    }

    /// <summary>
    /// 设置选中(多选)
    /// 无选中则返回空列表
    /// </summary>
    public List<int> SelectList
    {
        get { return hasSelectList ?? (hasSelectList = new List<int>()); }
        set
        {
            //1.清除选中
            if (hasSelectList != null)
            {
                for (var i = hasSelectList.Count - 1; i >= 0; i--)
                {
                    SetRenderSelect(i, false);
                }
            }
            //2.更新选中
            hasSelectList = value;
            //3.设置选中
            if (hasSelectList != null)
            {
                for (var i = hasSelectList.Count - 1; i >= 0; i--)
                {
                    SetRenderSelect(hasSelectList[i], true);
                }
            }
        }
    }

    /// <summary>
    /// 是否选中了？
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public bool IsSelected(int index)
    {
        bool state;
        selectIndexDic.TryGetValue(index, out state);
        return state;
    }

    /// <summary>
    /// 添加选中
    /// </summary>
    /// <param name="index"></param>
    public void AppendSelect(int index)
    {
        if (IsSelected(index) == false)
        {
            SelectList.Add(index);
            SetRenderSelect(index, true);
        }
    }
    /// <summary>
    /// 取消选中
    /// </summary>
    /// <param name="index"></param>
    public void DeSelect(int index)
    {
        if (IsSelected(index))
        {
            SelectList.Remove(index);
            SetRenderSelect(index, false);
        }
    }

    /// <summary>
    /// 清空选中
    /// </summary>
    public void ClearSelect()
    {
        SelectList.Clear();
        selectIndexDic.Clear();

        for (int i = _drawingRenders.Count - 1; i >= 0; i--)
        {
            if (_drawingRenders[i].IsShowChildTree)
            {
                _drawingRenders[i].ChildTree.parentTree.RemoveChildTree(_drawingRenders[i]);
            }
        }
    }

    #endregion

    #region 内部接口

    /// <summary>
    /// 判断当前是否有子树显示
    /// </summary>
    /// <returns></returns>
    private bool HasChildTreeShow()
    {
        bool hasTag = false;
        for (int i = _drawingRenders.Count - 1; i >= 0; i--)
        {
            TreeRender tempRender = _drawingRenders[i];
            if (tempRender.IsShowChildTree)
            {
                hasTag = true;
                break;
            }
        }
        return hasTag;
    }

    /// <summary>
    /// 设置render选中状态
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    public void SetRenderSelect(int index, bool value)
    {
        selectIndexDic[index] = value;
        //前提是该render已经生成
        if (renderIndexDic.ContainsKey(index))
        {
            renderIndexDic[index].Render.Select = value;
        }
        if (value && _onSelect != null)//选中回调
        {
            _onSelect(index, level, DataProvider[index]);
        }
        else if (!value && _onDeSelect != null)//取消选中回调
        {
            _onDeSelect(index);
        }
    }

    #endregion

    #region 各种状态回调（OnXXX...）

    protected override void OnInit()
    {
        base.OnInit();
        AddPositionChange(OnPositionChange);
        CalculateMaxShowNum();
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        if (Application.isPlaying)
        {
            if (_dataChanged)
            {
                DrawList();
            }
        }
    }

    /// <summary>
    /// 数据源改变标记（整个数据源改变或者单个数据改变了可调用一次）
    /// </summary>
    public void DataChanged(bool updateNow = false)
    {
        _dataChanged = true;

        if (updateNow)
        {
            DrawList();
        }
    }

    /// <summary>
    /// 位置改变了刷新下渲染
    /// </summary>
    /// <param name="pos"></param>
    protected virtual void OnPositionChange(Vector2 pos)
    {
        DrawList();
    }

    /// <summary>
    /// 点击了BaseRender
    /// </summary>
    /// <param name="eventData"></param>
    protected virtual void OnRenderClick(PointerEventData eventData)
    {
        CBaseRender cbRender = eventData.pointerPress.GetComponent<CBaseRender>();
        TreeRender currentRender = renderIndexDic[cbRender.Index];
        if (currentRender.IsShowChildTree && !_doSelectTag)
        {
            RemoveChildTree(currentRender);
        }
        else
        {
            if (autoFold)
            {
                for (int i = _drawingRenders.Count - 1; i >= 0; i--)
                {
                    if (_drawingRenders[i].IsShowChildTree)
                    {
                        _drawingRenders[i].ChildTree.parentTree.RemoveChildTree(_drawingRenders[i]);
                    }
                    DeSelect(_drawingRenders[i].Render.Index);
                }
            }
            DrawChildTree(currentRender, cbRender);   
        }

        if (IsSelected(cbRender.Index))//反选
        {
            DeSelect(cbRender.Index);
        }
        else//选中
        {
            AppendSelect(cbRender.Index);
            if (parentTree && parentRender)
            {
                for (int i = 0; i < parentTree.renderIndexDic.Count; i++)
                {
                    TreeRender parentTreeRender = parentTree.renderIndexDic[i];
                    parentTreeRender.ChildClickStack = i==parentRender.Index ? new List<object> { cbRender.Index } : null;
                }
            }
        }
        _doSelectTag = false;
    }

   
    public void DoSelect(List<object> objList)
    {
        DoSelectPrivate(objList);
    }

    private void DoSelectPrivate(List<object> objList)
    {
        if (objList.Count > 0)
        {
            int index = Convert.ToInt32(objList[0]);
            if (renderIndexDic.ContainsKey(index))
            {
                PointerEventData eventData = new PointerEventData(EventSystem.current) { pointerPress = renderIndexDic[index].Render.gameObject };

                if (objList.Count > 1)
                {
                    objList.RemoveAt(0);
                    renderIndexDic[index].ChildClickStack = objList;
                }

                _doSelectTag = true;
                OnRenderClick(eventData);
            }
        }
    }

    /// <summary>
    /// 面积发生改变
    /// </summary>
    public override void OnDimensionsChange()
    {
        base.OnDimensionsChange();
        CalculateMaxShowNum();
    }

    /// <summary>
    /// 计算最大显示数量
    /// </summary>
    protected virtual void CalculateMaxShowNum()
    {
        //展开子树时会响应父树的DimensionsChange，这个时候可见项向下移动，不需要重新计算最大显示数量
        if (HasChildTreeShow()) return;
        int preShowNum = maxShowNum;
        Vector2 vector2 = mask.rect.size;

        maxShowNum = (int)Math.Ceiling(vector2.y / (ItemHeight + pad.y));

        if (preShowNum != maxShowNum)
        {
            DataChanged();
        }
    }

    public override void OnInitializePotentialDrag(PointerEventData eventData)
    {
        if (!canDrag && parentTree != null)
        {
            parentTree.OnInitializePotentialDrag(eventData);
        }
        else
        {
            base.OnInitializePotentialDrag(eventData);
        }
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
        if (!canDrag && parentTree != null)
        {
            parentTree.OnBeginDrag(eventData);
        }
        else
        {
            base.OnBeginDrag(eventData);
        }
    }

    public override void OnDrag(PointerEventData eventData)
    {
        if (!canDrag && parentTree != null)
        {
            parentTree.OnDrag(eventData);
        }
        else
        {
            base.OnDrag(eventData);
        }
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        if (!canDrag && parentTree != null)
        {
            parentTree.OnEndDrag(eventData);
        }
        else
        {
            base.OnEndDrag(eventData);
        }
    }

    #endregion

    #region 渲染相关

    /// <summary>
    /// 渲染列表
    /// </summary>
    private void DrawList()
    {
        int start = GetDrawRendererStartIndex();
        //如果仅仅是位置改变了而且渲染的第一个没变则不用再管了
        if (!_dataChanged && start == _startIndex) return;
        _startIndex = start;
        if (_dataChanged)
        {
            ResetRenderPool();
            ContentChanged(true);
            _dataChanged = false;
        }
        int i = start;
        int end = Math.Min(DataCount, start + maxShowNum + 1);//多加一个1是为了缓存一个，增加效率
        Vector2 offset = ItemOffset;
        while (i < end)
        {
            if (renderIndexDic.ContainsKey(i) == false)
            {
                TreeRender cellRender = GetOneRender();
                cellRender.Level = level;
                cellRender.SetData(GetItemData(i));//设置数据
                renderIndexDic[i] = cellRender;
                _drawingRenders.Add(cellRender);

                CBaseRender cbRender = cellRender.Render;
                cbRender.Index = i;//设置索引
                cbRender.gameObject.name = renderPrefab.gameObject.name + i;//设置名称
                                                                            //  cbRender.select = IsSelected(i);//设置选中
                cbRender.rectTransform.anchoredPosition = GetRenderPos(i, offset);//设置位置
                Vector3 pos = cbRender.transform.localPosition;
                pos.z = 0;
                cbRender.transform.localPosition = pos;
                cbRender.gameObject.SetActive(true);//显示
            }
            i++;
        }
        for (i = 0; i < _availableRenders.Count; i++)
        {
            _availableRenders[i].Render.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 定位render（考虑render的锚点的偏移）
    /// </summary>
    /// <param name="index">数据项索引</param>
    /// <param name="offset"></param>
    /// <returns></returns>
    protected virtual Vector2 GetRenderPos(int index, Vector2 offset)
    {
        Vector2 pos = offset;
        if (index > 0)
        {
            float yOffset = ItemHeight + pad.y;
            if (renderIndexDic.ContainsKey(index - 1))
            {
                TreeRender lastRender = renderIndexDic[index - 1];
                if (lastRender.IsShowChildTree)
                {
                    yOffset += lastRender.ChildTree.Height;
                }
                pos.y = (lastRender.Render.rectTransform.anchoredPosition.y - yOffset);
            }
        }
        return pos;
    }

    protected TreeRender GetOneRender()
    {
        TreeRender cellRender;
        int last = _availableRenders.Count - 1;
        if (last >= 0) //直接从资源池里拿
        {
            cellRender = _availableRenders[last];
            cellRender.Render.gameObject.SetActive(true);
            _availableRenders.RemoveAt(last);
        }
        else //需要创建了
        {
            cellRender = CreateItemRender();
        }
        return cellRender;
    }

    /// <summary>
    /// 创建itemRender，指定预设名的会去load资源
    /// </summary>
    /// <returns></returns>
    protected virtual TreeRender CreateItemRender()
    {
        TreeRender render = Activator.CreateInstance(itemRender) as TreeRender;

        if (render == null)
        {
            return null;
        }
       
        if (renderPrefab == null)
        {
            return null;
        }
        GameObject renderObj = Instantiate(renderPrefab.gameObject);
        CBaseRender cbRender = renderObj.GetComponent<CBaseRender>();
        render.Render = cbRender;
        cbRender.AddClick(OnRenderClick);
        AddChild(cbRender.gameObject, Vector3.zero);
        return render;
    }

    private void GetOneTree(TreeRender currentRender, Action<GameObject> callBack)
    {
        if (currentRender.HasChildData())
        {
            int last = _availableCtree.Count - 1;
            if (last>=0)
            {
                currentRender.ChildTree = _availableCtree[last];
                currentRender.ChildTree.gameObject.SetActive(true);
                callBack(currentRender.ChildTree.gameObject);
            }
            else
            {
                currentRender.CreatChildTree(callBack);
            }
        }
    }

    private void PushCTreeToPool(CTree noUseTree)
    {
        noUseTree.ClearSelect();
        noUseTree.ResetRenderPool();
        noUseTree.gameObject.SetActive(false);
        _availableCtree.Add(noUseTree);
    }

    /// <summary>
    /// 显示子树
    /// </summary>
    private void DrawChildTree(TreeRender currentRender, CBaseRender cbRender)
    {
       

    }

    /// <summary>
    /// 添加或删除子树后，递归修改所有render位置
    /// </summary>
    /// <param name="childTree">子树</param>
    /// <param name="addState">添加或者删除子树</param>
    /// <param name="offsetY"></param>
    private void AdjustPostion(CTree childTree, bool addState, float offsetY = 0)
    {
        CTree theParentTree = childTree.parentTree;
        if (theParentTree == null) return;

        int index = childTree.parentRender.Index;
        if (offsetY.Equals(0))
        {
            offsetY = addState ? childTree.Height + childTreeOffset.y*2 : -childTree.Height - childTreeOffset.y*2;
        }
        if (!IsTopTree(theParentTree))
        {
            theParentTree.Height = theParentTree.Height + offsetY;
        }
        for (int i = index + 1; i < theParentTree.renderIndexDic.Count; i++)
        {
            CBaseRender theRender = theParentTree.renderIndexDic[i].Render;
            Vector2 newPos = theRender.rectTransform.anchoredPosition;
            newPos.y = newPos.y - offsetY;
            theRender.rectTransform.anchoredPosition = newPos;
            if (theParentTree.renderIndexDic[i].IsShowChildTree)
            {
                CTree followTree = theParentTree.renderIndexDic[i].ChildTree;
                Vector3 treePos = theRender.rectTransform.anchoredPosition;
                treePos.x = treePos.x + childTreeOffset.x - theRender.Width / 2;
                treePos.y = treePos.y - childTreeOffset.y - theRender.Height / 2;
                followTree.SelfTransform.anchoredPosition = treePos;
            }
        }
        theParentTree.ContentChanged();
        AdjustPostion(theParentTree, addState, offsetY);
    }

    private bool IsTopTree(CTree theTree)
    {
        return theTree.parentTree == null;
    }

    /// <summary>
    /// 向上递归改变父树的宽度，最顶级的树不操作，顶级树的宽度在做界面的时候指定
    /// </summary>
    /// <param name="childTree"></param>
    private void ChangeParentTreeWidth(CTree childTree)
    {
        CTree theParentTree = childTree.parentTree;
        if (theParentTree == null) return;
        if (!IsTopTree(theParentTree))
        {
            theParentTree.Width = theParentTree.Width + childTree.childTreeOffset.x;
        }
        ChangeParentTreeWidth(theParentTree);
    }

    /// <summary>
    /// 移除子树 
    /// </summary>
    /// <param name="currentRender">要操作的节点</param>
    private void RemoveChildTree(TreeRender currentRender)
    {
        if (currentRender.ChildTree != null)
        {
            AdjustPostion(currentRender.ChildTree, false);
            PushCTreeToPool(currentRender.ChildTree);
           
        }
    }

    /// <summary>
    /// 根据预设名获取预设脚本
    /// </summary>
    /// <param name="prefabName"></param>
    /// <param name="callBack"></param>
    /// <returns></returns>
    private void GetBaseRenderByName(string prefabName, Action<CBaseRender> callBack)
    {
        MingUIAgent.LoadUIPrefab(prefabName, itemVo =>
        {
            GameObject prefabObj = itemVo.getObject() as GameObject;
            if (prefabObj)
            {
                callBack(prefabObj.GetComponent<CBaseRender>());   
            }
        });
    }

    /// <summary>
    /// 获取当前渲染的[开始Index].
    /// </summary>
    /// <returns></returns>
    protected virtual int GetDrawRendererStartIndex()
    {
        Vector2 nowPos = content.anchoredPosition;
        nowPos.x = Mathf.Clamp(nowPos.x, nowPos.x, 0);//X最大为0
        nowPos.y = Mathf.Clamp(nowPos.y, 0, nowPos.y);//Y最小为0


        var start = (int)Math.Floor(nowPos.y / (ItemHeight + pad.y));

        if (_dataChanged)
        {
            return 0;   
        }
        return ClampIndex(start);
    }

    /// <summary>
    /// 索引限制
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    protected int ClampIndex(int index)
    {
        return Mathf.Clamp(index, 0, DataCount > 0 ? (DataCount - 1) : 0);
    }

    /// <summary>
    /// 全部Render回池
    /// </summary>
    protected virtual void ResetRenderPool()
    {
        int i = 0;
        while (i < _drawingRenders.Count) //回收当前渲染的render
        {
            _drawingRenders[i].ChildClickStack = null;
            _drawingRenders[i].Render.Select = false;
           
            _drawingRenders[i].Render.gameObject.SetActive(false);
            _availableRenders.Add(_drawingRenders[i]);
            i++;
        }
        _drawingRenders.Clear();
        renderIndexDic.Clear();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        ClearParams();
    }

    private void ClearParams()
    {
        foreach (TreeRender baseRender in _drawingRenders)
        {
            baseRender.OnDestroy();
        }
        foreach (TreeRender baseRender in _availableRenders)
        {
            baseRender.OnDestroy();
        }
        foreach (CTree treeInPool in _availableCtree)
        {
            treeInPool.OnDestroy();
        }

        _drawingRenders.Clear();
        _availableRenders.Clear();
        _availableCtree.Clear();
        selectIndexDic.Clear();
        renderIndexDic.Clear();
    }

    #endregion
}

