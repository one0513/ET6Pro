using DG.Tweening;

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class CIrregularBaseRender : CBaseRender
{
    private System.Action<CIrregularBaseRender,int> onPrefabSizeUpdateCallBack;
    public void SetPrefabSizeUpdateCallBack(System.Action<CIrregularBaseRender,int> callBack)
    {
        onPrefabSizeUpdateCallBack = callBack;
    }
    public void UpdatePrefabSize()
    {
        if (onPrefabSizeUpdateCallBack != null)
        {
            onPrefabSizeUpdateCallBack.Invoke(this,Index);
        }
    }
}
public class CIrregularLuaItemRenderInfo
{
    public string itemName;
   


}

public class CIrregularBaseRenderItemInfo
{
    private float itemScale;
    public CIrregularBaseRender renderPrefab;
    public bool isCanUse = false;
    public CIrregularBaseRenderItemInfo() { }
    public CIrregularBaseRenderItemInfo(CIrregularBaseRender _renderPrefab, float _itemScale = 1f)
    {
        renderPrefab = _renderPrefab;
        itemScale = _itemScale;
        UpdateInfo();
    }

    public void UpdateScale(float _itemScale)
    {
        itemScale = _itemScale;
        UpdateInfo();
    }

    private void UpdateInfo()
    {
        UpdateInfo(renderPrefab);
    }
    public void UpdateInfo(CIrregularBaseRender _renderPrefab)
    {
        itemWidth = _renderPrefab.Width * itemScale;
        itemHeight = _renderPrefab.Height * itemScale;
        itemWidthOffset = itemWidth * _renderPrefab.rectTransform.pivot.x;
        itemHeightOffset = itemHeight * (1 - _renderPrefab.rectTransform.pivot.y);
        itemOffset = new Vector2(itemWidthOffset, -itemHeightOffset);
    }
    //子项宽度
    public float itemWidth;
    //子项高度
    public float itemHeight;
    //子项锚点偏移量
    public Vector2 itemOffset;
    //子项锚点宽度偏移量
    public float itemWidthOffset;
    //子项锚点高度偏移量
    public float itemHeightOffset;
}
/// <summary>
/// 线性列表(线性显示线性增加,自身锚点在左上，render锚点自定义)
/// 增强功能：
/// 1.多行多列左上角对齐排列+自动居中排列
/// 2.索引定位及增量滚动
/// 3.定尺寸（宽高）Render
/// 4.单选+多选选择模式+选中/取消选中回调
/// 5.数据源数量限制功能
/// 6.多种拖拽模式+拖拽尾部数据请求回调
/// 7.动态调整List长宽
/// </summary>
public class CIrregularList : CCanvas
{
    public enum DragMode
    {
        Follow, //拖拽-跟随（内容跟着鼠标拖拽移动）
        Adhere, //拖拽-吸附(内容不跟着鼠标拖拽移动,当拖拽发生并移动的阈值达到吸附阈值时，则直接吸附)
        FollowAndAdhere, //拖拽-跟随后吸附(内容跟着鼠标拖拽移动,当拖拽结束后发生判断移动的阈值，再决定吸附)
    }
    [SerializeField]
    private int columns; //列数
    public Vector2 pad; //间距
    public Vector2 leftTop;//左
    public float itemScale = 1; //render的缩放
    public int dataCountLimit = 0; //数据项长度限制（超出则会被裁减），0为不限制
    public int maxSelectNum = 1; //最大选中数量 0：不限  n:n个
    public DragMode dragMode = DragMode.Follow; //是否开启整render移动，若是，则移动结束后会自动吸附到该块（仅仅针对一行/一列）
    public float dragThreshold = 0.25f; //拖拽阈值（用于DragMode.Adhere或者FollowAndAdhere模式），Adhere：移动像素量达成则开启吸附|FollowAndAdhere：0.25是指当前移动超出该render25%区域时则自动向右/下吸附，否则还原
    [SerializeField]
    private List<CIrregularBaseRender> renderPrefabList = new List<CIrregularBaseRender>(); //子对象渲染预设
    private Dictionary<string, CIrregularBaseRenderItemInfo> renderItemInfoDic;
    private List<CIrregularBaseRenderItemInfo> renderItemInfoList;
    public bool managerRenderSelect = true;
    public bool itemTweenAlpha = true;
    public static float itemTweenAlphaSpeed = 0.05f;
    public static float itemTweenAlphaTime = 0.2f;

    [NonSerialized]
    public Type itemRender = typeof(BaseRender); //c#层的render,必须是ListItemRender或者其子类


    protected bool dataChanged = true; //数据源是否改变？
    protected bool prefabSizeChanged = false;
    protected int maxShowNum; //当前最多可显示多少个render
    protected int startIndex = -1; //当前渲染的首索引
    protected int endIndex = -1;//当前渲染的尾索引
    protected List<int> selectList; //多选索引列表
    protected Vector2 beginDragPos; //开始拖拽时的content位置

    protected Dictionary<string, CIrregularLuaItemRenderInfo> luaItemRenderInfoDic = new Dictionary<string, CIrregularLuaItemRenderInfo>();

    protected List<object> dataProvider; //c#层数据源
    protected List<string> luaDataPrefabList;
    protected UIEventListener.IntDelegate onSelect; //选中回调（返回索引）
    protected UIEventListener.IntDelegate onDeSelect; //取消选中回调（返回索引）
    protected UIEventListener.IntDelegate onDataRequest; //数据请求回调
    protected UIEventListener.IntDelegate onAdhere;//吸附回调
    protected UIEventListener.VoidDelegate onDragBegin;
    protected UIEventListener.VoidDelegate onDragEnd;
    protected UIEventListener.VoidDelegate onValueChange;

    protected List<BaseRender> drawingRenders = new List<BaseRender>(); //正在渲染的render
    protected Dictionary<string, List<BaseRender>> availableRenderDic = new Dictionary<string, List<BaseRender>>(); //缓存的render
    protected Dictionary<int, bool> selectIndexDic = new Dictionary<int, bool>(); //选中的索引字典
    protected Dictionary<int, BaseRender> renderIndexDic = new Dictionary<int, BaseRender>(); //正在渲染的render的索引字典


    protected bool duringDeleteAnimation = false;


    public List<CIrregularBaseRender> RenderPrefabList
    {
        get { return renderPrefabList; }
        set
        {
            renderPrefabList = value;
        }
    }

    public Dictionary<string, CIrregularBaseRenderItemInfo> RenderItemInfoDic
    {
        get
        {
            if (renderItemInfoDic == null) UpdateCIrregularBaseRenderItemInfoDic();
            return renderItemInfoDic;
        }
    }
    private void UpdateCIrregularBaseRenderItemInfoDic()
    {
        if (renderItemInfoDic == null) renderItemInfoDic = new Dictionary<string, CIrregularBaseRenderItemInfo>();
        renderItemInfoDic.Clear();
        for (int i = 0; i < renderPrefabList.Count; i++)
        {
            renderItemInfoDic[renderPrefabList[i].name] = new CIrregularBaseRenderItemInfo(renderPrefabList[i], itemScale);
        }
    }

   


    public int Columns
    {
        get => columns;
        set
        {
            columns = value == 0 ? 0 : 1;
            OnDimensionsChange();
            ContentChanged(true);
            RefreshCurrentRenderPos();
        }
    }
    //数据总项
    public int DataCount
    {
        get { return dataProvider == null ? 0 : dataProvider.Count; }
    }

    #region 外部接口
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
        get { return selectList ?? (selectList = new List<int>()); }
        set
        {
            //1.清除选中
            if (selectList != null)
            {
                for (int i = 0; i < selectList.Count; i++)
                {
                    SetRenderSelect(selectList[i], false);
                }
            }
            //2.更新选中
            selectList = value;
            //3.设置选中
            if (selectList != null)
            {
                if (maxSelectNum > 0) //不能超出数量
                {
                    while (selectList.Count > maxSelectNum)
                    {
                        selectList.RemoveAt(selectList.Count - 1);
                    }
                }

                for (int i = 0; i < selectList.Count; i++)
                {
                    SetRenderSelect(selectList[i], true);
                }
            }
        }
    }

    public void SelectIndexWithoutFunc(int index)
    {
        //1.清除选中
        if (selectList != null)
        {
            for (int i = 0; i < selectList.Count; i++)
            {
                SetRenderSelectWithoutFunc(selectList[i], false);
            }
        }
        //2.更新选中
        selectList = new List<int>() { index };
        //3.设置选中
        if (selectList != null)
        {
            if (maxSelectNum > 0) //不能超出数量
            {
                while (selectList.Count > maxSelectNum)
                {
                    selectList.RemoveAt(selectList.Count - 1);
                }
            }

            for (int i = 0; i < selectList.Count; i++)
            {
                SetRenderSelectWithoutFunc(selectList[i], true);
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
        if (maxSelectNum == 1) //单选
        {
            SelectIndex = index;
        }
        else if (maxSelectNum == 0 || SelectList.Count < maxSelectNum) //多选
        {
            if (IsSelected(index) == false)
            {
                SelectList.Add(index);
                SetRenderSelect(index, true);
            }
        }
        else //达到上限
        {
            Debug.LogWarning("可选择数量已达到上限");
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
    /// 添加选中的回调
    /// </summary>
    /// <param name="callback"></param>
    public void AddSelect(UIEventListener.IntDelegate callback)
    {
        onSelect = callback;
    }

    /// <summary>
    /// 删除选中回调
    /// </summary>
    public void RemoveSelect()
    {
        onSelect = null;
    }

    /// <summary>
    /// 添加取消选中的回调
    /// </summary>
    /// <param name="callback"></param>
    public void AddDeSelect(UIEventListener.IntDelegate callback)
    {
        onDeSelect = callback;
    }

    /// <summary>
    /// 删除取消选中回调
    /// </summary>
    public void RemoveDeSelect()
    {
        onDeSelect = null;
    }

    /// <summary>
    /// 添加数据请求回调
    /// </summary>
    /// <param name="callback"></param>
    public void AddDataRequest(UIEventListener.IntDelegate callback)
    {
        onDataRequest = callback;
    }

    /// <summary>
    /// 删除数据请求回调
    /// </summary>
    public void RemoveDataRequest()
    {
        onDataRequest = null;
    }
    /// <summary>
    /// 添加吸附回调
    /// </summary>
    /// <param name="callback"></param>
    public void AddAdhere(UIEventListener.IntDelegate callback)
    {
        onAdhere = callback;
    }

    /// <summary>
    /// 删除吸附回调
    /// </summary>
    public void RemoveAdhere()
    {
        onAdhere = null;
    }

    public void AddDragBegin(UIEventListener.VoidDelegate callback)
    {
        onDragBegin = callback;
    }

    public void RemoveDragBegin()
    {
        onDragBegin = null;
    }

    public void AddDragEnd(UIEventListener.VoidDelegate callback)
    {
        onDragEnd = callback;
    }
    public void RemoveDragEnd()
    {
        onDragEnd = null;
    }

    public void AddValueChange(UIEventListener.VoidDelegate callback)
    {
        onValueChange = callback;
    }
    public void RemoveValueChange()
    {
        onValueChange = null;
    }

  
    public void ResetAllRenderPool()
    {
        ResetRenderPool(DataCount);
    }

    public void SetDataPrivide(object obj)
    {

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
            ClampDataProvider(); //裁减数据源
            ClearSelect(); //清除选择
            DataChanged(); //设置脏读
        }
    }

    private void UpdateLuaDataPrefabList()
    {
       
    }
    private void InitRenderItemInfoList()
    {
     
    }

    /// <summary>
    /// 补充一个数据源(lua / c# 均可)
    /// </summary>
    /// <param name="vo"></param>
    /// <param name="goTo">是否定位到这个item</param>
    /// <param name="anim"></param>
    public void AppendData(object vo, bool goTo = true, bool anim = true)
    {
        if (dataProvider == null) dataProvider = new List<object>();
        dataProvider.Add(vo);
        ClampDataProvider(); //裁减数据源
        DataChanged(); //设置脏读
        if (goTo)
        {
            GotoIndex(dataProvider.Count - 1, anim);
        }
    }
   
    /// <summary>
    /// 补充一组数据源（c#）
    /// </summary>
    /// <param name="list"></param>
    /// <param name="goTo">是否定位到补充的第一个数据的index</param>
    /// <param name="anim"></param>
    public void AppendDataList(List<object> list, bool goTo = true, bool anim = true)
    {
        if (dataProvider == null) dataProvider = new List<object>();
        int length = dataProvider.Count;
        for (int i = 0; i < list.Count; i++)
        {
            dataProvider.Add(list[i]);
        }
        ClampDataProvider(); //裁减数据源
        DataChanged(); //设置脏读
        if (goTo)
        {
            GotoIndex(length, anim);
        }
    }

    /// <summary>
    /// 通过索引获取数据
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public object GetItemData(int index)
    {
        if (index >= 0 && index < dataProvider.Count)
        {
            return dataProvider[index];
        }
        return null;
    }

    /// <summary>
    /// 数据源改变标记（整个数据源改变或者单个数据改变了可调用一次）
    /// </summary>
    public void DataChanged(bool updateNow = false)
    {
        dataChanged = true;
        OnDataChange();
        if (updateNow)
        {
            DrawList();
        }
    }


    /// <summary>
    /// 定位到索引位置
    /// </summary>
    /// <param name="index"></param>
    /// <param name="anim"></param>
    /// <param name="callback"></param>
    public virtual void GotoIndex(int index, bool anim = false, TweenCallback callback = null)
    {
        if (dataChanged)
        {
            DataChanged(true);
        }
        Vector2 pos = GetRenderPos(index, false);
        pos *= -1;
        ScrollTo(pos, anim, () => {
            if (!anim)
            {
                DrawList();
            }
            if (callback != null)
            {
                callback();
            }
        });
    }

    

    /// <summary>
    /// 增量索引移动
    /// </summary>
    /// <param name="step"> 大于0：右/下  小于0：左上</param>
    /// <param name="anim"></param>
    /// <param name="callback"></param>
    public virtual void MoveStep(int step, bool anim = false, TweenCallback callback = null)
    {
        GotoIndex(ClampIndex(startIndex + step), anim, callback);
    }

    public void ResetStartIndex()
    {
        startIndex = -1;
    }

    public void SetAllRenderDataKv(string funName, object data)
    {
        foreach (var render in drawingRenders)
        {
            render.SetDataKv(funName, data);
        }
    }

    /// <summary>
    /// 请自行保证Lua里面的数据也同步删除了
    /// </summary>
    /// <param name="index"></param>
    /// <param name="Anim"></param>
    /// <param name="duration"></param>
    /// <param name="callBack"></param>
    public void DeleteItemByIndex(int index, bool Anim, float duration, TweenCallback callBack)
    {
        if (Anim && duration > 0)
        {
            if (duringDeleteAnimation) return;

            //防止drawList被调用
            duringDeleteAnimation = true;

            int i = index;
            int start = startIndex;
            int end = Math.Min(DataCount, start + maxShowNum + (Columns > 1 ? Columns : 1)); //多加一个1是为了缓存一个，增加效率
            CIrregularBaseRenderItemInfo renderItemInfo = renderItemInfoList[index];//RenderItemInfoDic[luaDataPrefabList[index]];
            Vector2 offset = renderItemInfo.itemOffset;
            Tweener tweener = null;
            BaseRender delRender = null;
            BaseRender cellRender;

            //首先隐藏需要删除的Item
            renderIndexDic.TryGetValue(i, out delRender);
            if (delRender != null)
            {
                tweener = delRender.Render.rectTransform.DOScale(Vector3.zero, 0.2f);
            }
            i++;
            //移动后面的Item
            while (i < end)
            {
                renderIndexDic.TryGetValue(i, out cellRender);
                if (cellRender != null)
                {
                    cellRender.Render.Index = i - 1;
                    //tweener = cellRender.Render.rectTransform.DOAnchorPos(GetRenderPos(i - 1), duration, false);
                }
                i++;
            }
            //执行回调
            if (tweener != null)
            {
                tweener.OnComplete(() => {
                    delRender.Render.rectTransform.localScale = Vector3.one;
                    if (horizontal)
                    {
                        delRender.Render.rectTransform.localPosition = new Vector3(0, 99999, 0);
                    }
                    else
                    {
                        delRender.Render.rectTransform.localPosition = new Vector3(99999, 0);
                    }

                    for (int j = index; j < end; j++)
                    {
                        if (j == end - 1)
                        {
                            drawingRenders[j] = delRender;
                            renderIndexDic[j] = delRender;
                        }
                        else
                        {
                            renderIndexDic.TryGetValue(j, out cellRender);
                            if (cellRender != null)
                            {
                                drawingRenders[j] = drawingRenders[j + 1];
                                renderIndexDic[j] = renderIndexDic[j + 1];
                            }
                        }
                    }

                    //还原drawList被调用
                    duringDeleteAnimation = false;
                    //刷新CList
                    dataProvider.RemoveAt(index);
                    ClearSelect();
                    DataChanged(true);
                    if (callBack != null)
                    {
                        callBack();
                    }
                });
            }
        }
        else
        {
            //刷新CList
            dataProvider.RemoveAt(index);
            ClearSelect();
            DataChanged(true);
            if (callBack != null)
            {
                callBack();
            }
        }
    }

    #endregion

    #region 编辑器刷新相关
    [SerializeField]
    private int _eItemCount = 0;
    private List<object> tempList = new List<object>();


    public int EItemCount
    {
        get => _eItemCount;
        set
        {
            if (_eItemCount != value) _eItemCount = value;
            RefreshLayout();
        }
    }



    private void UpdateTempList()
    {
        tempList.Clear();
        for (int i = 0; i < _eItemCount; i++)
        {
            tempList.Add(1);
        }
        DataProvider = tempList;
    }

    private void CreatEditorPrefab()
    {
        for (int i = 0; i < renderPrefabList.Count; i++)
        {
            GameObject go = GameObject.Instantiate(renderPrefabList[i].gameObject, content.transform);
            RectTransform rectTrans = go.GetComponent<RectTransform>();
            rectTrans.anchorMin = rectTrans.anchorMax = new Vector2(0, 1); //约定左上角为锚点
            rectTrans.localScale = Vector3.one;
            rectTrans.localEulerAngles = Vector3.zero;
            ContentChanged();
            go.name = renderPrefabList[i].gameObject.name;
        }
    }
    public void RefreshLayout()
    {
        if (Application.isPlaying)
        {
            UpdateTempList();
        }
        else
        {
            if (renderPrefabList.Count == 0) return;
            int c = content.childCount / renderPrefabList.Count;
            if (c < _eItemCount)
            {
                for (int i = 0; i < _eItemCount - c; i++)
                {
                    CreatEditorPrefab();
                }
            }
            else
            {
                for (int i = c - 1; i >= _eItemCount; i--)
                {
                    GameObject.Destroy(content.GetChild(i).gameObject);
                }
            }
        }
        RefreshLayoutOnEditor();
    }
    private void ClearContentChilds()
    {
        for (int i = content.childCount - 1; i >= 0; i--)
        {
            Destroy(content.GetChild(i).gameObject);
        }
    }
    public void RefreshLayoutOnEditor()
    {
        if (renderPrefabList.Count == 0)
        {
            Debug.LogError("缺少--renderPrefab-->", this.gameObject);
            return;
        }
        for (int i = 0; i < content.childCount; i++)
        {
            RectTransform rectTransform = content.GetChild(i).GetComponent<RectTransform>();
            if (Application.isPlaying) rectTransform.anchoredPosition = GetRenderPos(i);
            else
                rectTransform.anchoredPosition = GetRenderPosOnEditor(i); //设置位置
        }
    }
    protected virtual Vector2 GetRenderPosOnEditor(int index)
    {
        int quotient = Mathf.FloorToInt(index / renderPrefabList.Count);
        int remainder = index % renderPrefabList.Count;
        float allWidth = 0;
        float allHeight = 0;
        for (int i = 0; i < renderPrefabList.Count; i++)
        {
            allWidth += renderPrefabList[i].Width + pad.x;
            allHeight += renderPrefabList[i].Height + pad.y;
        }
        CIrregularBaseRenderItemInfo renderItemInfo = renderItemInfoList[remainder];//RenderItemInfoDic[renderPrefabList[remainder].name];
        Vector2 offset = renderItemInfo.itemOffset;
        Vector2 pos = offset;
        pos.x += leftTop.x;
        pos.y -= leftTop.y;
        if (Columns == 0) //一行
        {
            float remainderWidth = 0;
            for (int i = 0; i < remainder; i++)
            {
                remainderWidth += renderPrefabList[i].Width + pad.x;
            }
            pos.x += quotient * allWidth + remainderWidth;
        }
        else //(Columns == 1) //一列
        {
            float remainderHeight = 0;
            for (int i = 0; i < remainder; i++)
            {
                remainderHeight += renderPrefabList[i].Height + pad.y;
            }
            pos.y -= quotient * allHeight + remainderHeight;

        }
        return pos;
    }
    public void DeleCreatItem()
    {
        if (Application.isPlaying) return;
        for (int i = content.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(content.GetChild(i).gameObject);

        }
    }
    #endregion

    #region 内部接口

    /// <summary>
    /// 初始化
    /// </summary>
    protected override void OnInit()
    {
        ClearContentChilds();
        base.OnInit();
        AddPositionChange(OnPositionChange);
    }
    /// <summary>
    /// 位置改变了刷新下渲染
    /// </summary>
    /// <param name="pos"></param>
    protected virtual void OnPositionChange(Vector2 pos)
    {
        DrawList();
        if (onValueChange != null)
        {
            onValueChange.Invoke();
        }
    }

    protected virtual void OnDataChange()
    {

    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        if (dataChanged)
        {
            DrawList();
        }
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        if (dragMode == DragMode.Adhere)
        {
            beginDragPos = eventData.position;
        }
        else //其他情况都需要
        {
            base.OnBeginDrag(eventData);
            beginDragPos = ClampPosition(content.anchoredPosition);
        }

        if (onDragBegin != null)
        {
            onDragBegin.Invoke();
        }
    }

    public override void OnDrag(PointerEventData eventData)
    {
        if (dragMode != DragMode.Adhere) //吸附时，不跟随拖拽
        {
            base.OnDrag(eventData);
        }
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        if (dragMode == DragMode.Follow) //跟随
        {
            base.OnEndDrag(eventData);
        }
        else if (dragMode == DragMode.Adhere) //吸附
        {
            if (Columns <= 1 && DataCount > 0) //仅仅支持单行单列
            {
                Vector2 delta = eventData.position - beginDragPos;
                if (Columns == 0 && Math.Abs(delta.x) > dragThreshold)
                { //一行
                    int toIndex = delta.x > 0 ? startIndex - 1 : startIndex + 1;
                    toIndex = ClampIndex(toIndex);
                    GotoIndex(toIndex, true, () => {
                        OnAdhere(toIndex);
                    });
                }
                else if (Columns == 1 && Math.Abs(delta.y) > dragThreshold)
                { //一列
                    int toIndex = delta.y > 0 ? startIndex + 1 : startIndex - 1;
                    toIndex = ClampIndex(toIndex);
                    GotoIndex(toIndex, true, () => {
                        OnAdhere(toIndex);
                    });
                }
            }
        }
        else if (dragMode == DragMode.FollowAndAdhere) //先跟随再吸附
        {
            base.OnEndDrag(eventData);

            if (Columns <= 1 && DataCount > 0) //仅仅支持单行单列
            {
                Vector2 nowPos = ClampPosition(content.anchoredPosition); //x:[-x,0],y:[0.y]
                int toIndex; //当前滑动到的索引
                float rate;
                if (Columns == 0) //一行
                {
                    // float xRate = -nowPos.x / (ItemWidth + pad.x + leftTop.x);
                    // toIndex = Mathf.FloorToInt(xRate); //向下取整
                    // xRate -= toIndex; //剩下的小数部分是多少？
                    getRateAndToIndex(-nowPos.x, true, out rate, out toIndex);
                    if (nowPos.x <= beginDragPos.x) //向左滑动了
                    {
                        if (rate >= dragThreshold)
                        {
                            toIndex += 1;
                        }
                    }
                    else //向右滑动了
                    {
                        if (rate >= 1 - dragThreshold)
                        {
                            toIndex += 1;
                        }
                    }
                }
                else //一列
                {
                    getRateAndToIndex(nowPos.y, false, out rate, out toIndex);
                    if (nowPos.y >= beginDragPos.y) //向上滑动了
                    {
                        if (rate >= dragThreshold)
                        {
                            toIndex += 1;
                        }
                    }
                    else //向下滑动了
                    {
                        if (rate >= 1 - dragThreshold)
                        {
                            toIndex += 1;
                        }
                    }
                }
                toIndex = ClampIndex(toIndex);
                GotoIndex(toIndex, true, () => {
                    OnAdhere(toIndex);
                });
            }
        }
        //拖拽到最右或者最下，用于异步请求数据
        if (onDataRequest != null)
        {
            if ((horizontal && horizontalNormalizedPosition > 1) || (vertical && verticalNormalizedPosition < 0))
            {
                onDataRequest(DataCount); //返回当前数据总数
            }
        }
        if (onDragEnd != null)
        {
            onDragEnd.Invoke();
        }
    }

    private void getRateAndToIndex(float nowPos, bool isX, out float rate, out int toIndex)
    {
        if (nowPos == 0)
        {
            rate = 0;
            toIndex = 0;
            return;
        }
        float temp = 0;
        for (int i = 0; i < luaDataPrefabList.Count; i++)
        {
            temp += isX
                ? (/*RenderItemInfoDic[luaDataPrefabList[i]]*/renderItemInfoList[i].itemWidth + pad.x)
                : (/*RenderItemInfoDic[luaDataPrefabList[i]]*/renderItemInfoList[i].itemHeight + pad.y);
            if (temp > nowPos)
            {
                toIndex = i - 1;
                rate = 1 - (temp - nowPos) /
                    (isX
                        ? /*RenderItemInfoDic[luaDataPrefabList[i]]*/renderItemInfoList[i].itemWidth
                        : /*RenderItemInfoDic[luaDataPrefabList[i]]*/renderItemInfoList[i].itemHeight);
                return;
            }
        }
        rate = 0;
        toIndex = 0;
        Debug.LogError($"getRateAndToIndex异常，nowPos->{nowPos},isX->{isX},luaDataPrefabList.Count-->{luaDataPrefabList.Count}");
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();
        if (prefabSizeChanged)
        {
            OnDimensionsChange();
            ContentChanged(true);
            prefabSizeChanged = false;
        }
    }

    private int _preDataCount = 0;
    /// <summary>
    /// 渲染列表
    /// </summary>
    protected virtual void DrawList()
    {
        //if (!Application.isPlaying) return;
        if (renderPrefabList.Count == 0) return;
        if (duringDeleteAnimation) return;
        int start = GetDrawRendererStartIndex();
        //如果仅仅是位置改变了而且渲染的第一个没变则不用再管了
        if (!dataChanged && start == startIndex) return;


        //循环利用Item的模式
        var isDoTween = start > startIndex || dataChanged;

        int off = start - startIndex;
        if (off > 0)
        {
            //往上拉了，原先的stat位置已经隐藏，差值大于1表示拖得太快了
            while (off > 0)
            {
                ClipRenderByIndex(startIndex + off - 1);
                off--;
            }
        }
        else if (off < 0)
        {
            //往下拉，原先的last位置已经隐藏，差值绝对值大于1表示拖得太快了
            while (off < 0)
            {
                ClipRenderByIndex(endIndex + off + 1);
                off++;
            }
        }
        if (dataChanged)//数据源发生了改变
        {
            //1.设置脏读
            foreach (BaseRender render in renderIndexDic.Values)
            {
                render.Render.Select = false;
                render.IsDirty = true;
            }
            //2.回收冗余
            int nowDataCount = DataCount;
            int gapNum = _preDataCount - nowDataCount;
            if (gapNum != 0)//数量有变化
            {
                if (gapNum > 0)//回收这些冗余的
                {
                    ResetRenderPool(gapNum);
                }
                _preDataCount = nowDataCount;
                ContentChanged(true);
            }
            dataChanged = false;
        }
        int i = start;
        int end = Math.Min(DataCount, start + maxShowNum + (Columns > 1 ? Columns : 1)); //多加一个1是为了缓存一个，增加效率
        BaseRender cellRender;
        var tAlphaIndex = 0;
        while (i < end)
        {
            renderIndexDic.TryGetValue(i, out cellRender);
            endIndex = i;
            var isNotRender = cellRender == null;
            if (cellRender == null)//当前这个索引的render还未生成
            {
                cellRender = GetOneRender(luaDataPrefabList[i]);

                renderIndexDic[i] = cellRender;
                drawingRenders.Add(cellRender);
                CBaseRender cbRender = cellRender.Render;
                cbRender.Index = i; //设置索引
                cbRender.gameObject.name = $"{luaDataPrefabList[i]}{i}"; //设置名称
                cbRender.rectTransform.localPosition = Vector3.zero;
                cbRender.rectTransform.anchoredPosition = GetRenderPos(i); //设置位置
                cbRender.gameObject.SetActive(true); //显示
                cellRender.IsDirty = false;
                cellRender.SetData(GetItemData(i)); //设置数据
                if (managerRenderSelect) cellRender.Render.Select = IsSelected(i); //设置选中
            }
            else if (cellRender.IsDirty)//已经生成了并且脏读了，则要重新设置数据
            {
                cellRender.IsDirty = false;
                cellRender.SetData(GetItemData(i));
                if (managerRenderSelect) cellRender.Render.Select = IsSelected(i); //设置选中
            }
            if (isDoTween && (startIndex == -1 || isNotRender))
            {
                tAlphaIndex++;
                DoTweenAlpha(cellRender.Render, tAlphaIndex);
            }
            i++;
        }
        startIndex = start;
    }

    public void RefreshCurrentRenderPos()
    {
        int i = 0;
        int end = Math.Min(DataCount, maxShowNum + (Columns > 1 ? Columns : 1));
        BaseRender cellRender;
        while (i < end)
        {
            renderIndexDic.TryGetValue(i, out cellRender);
            endIndex = i;
            var isNotRender = cellRender == null;
            if (cellRender == null)//当前这个索引的render还未生成
            {
                cellRender = GetOneRender(luaDataPrefabList[i]);

                renderIndexDic[i] = cellRender;
                drawingRenders.Add(cellRender);
                CBaseRender cbRender = cellRender.Render;
                cbRender.Index = i; //设置索引
                cbRender.gameObject.name = $"{luaDataPrefabList[i]}{i}"; //设置名称
                cbRender.rectTransform.localPosition = Vector3.zero;
                cbRender.rectTransform.anchoredPosition = GetRenderPos(i); //设置位置
                cbRender.gameObject.SetActive(true); //显示
                cellRender.IsDirty = false;
                cellRender.SetData(GetItemData(i)); //设置数据
                if (managerRenderSelect) cellRender.Render.Select = IsSelected(i); //设置选中
            }
            else
            {
                cellRender.Render.rectTransform.anchoredPosition = GetRenderPos(i); //设置位置
            }
            i++;
        }
        //2.回收冗余
        int gapNum = _preDataCount - end;
        if (gapNum != 0)//数量有变化
        {
            if (gapNum > 0)//回收这些冗余的
            {
                ResetRenderPool(gapNum);
            }
            _preDataCount = end;
            ContentChanged(true);
        }
    }



    private void DoTweenAlpha(CBaseRender render, int tIndex)
    {
        if (itemTweenAlpha == false) return;
        MingUIUtil.SetGroupAlpha(render.gameObject, 0);
        render.tweenAlpha = MingUIUtil.SetGroupAlphaInTime(render.gameObject, 1, itemTweenAlphaTime, Ease.OutCubic).SetDelay(tIndex * itemTweenAlphaSpeed);
    }


    protected BaseRender GetOneRender(string prefabName)
    {
        BaseRender cellRender;
        if (availableRenderDic.TryGetValue(prefabName, out List<BaseRender> l))
        {
            if (l.Count <= 0) cellRender = CreateItemRender(prefabName);
            else
            {
                int last = l.Count - 1;
                cellRender = l[last];
                l.RemoveAt(last);
            }
        }
        else
        {
            availableRenderDic[prefabName] = new List<BaseRender>();
            cellRender = CreateItemRender(prefabName);
        }
        return cellRender;
    }


    /// <summary>
    /// 定位render（考虑render的锚点的偏移）
    /// </summary>
    /// <param name="index">数据项索引</param>
    /// <param name="offset">偏移量</param>
    /// <returns></returns>
    protected virtual Vector2 GetRenderPos(int index, bool isAddOffset = true)
    {
        CIrregularBaseRenderItemInfo renderItemInfo = renderItemInfoList[index]/*RenderItemInfoDic[luaDataPrefabList[index]]*/;
        Vector2 offset = renderItemInfo.itemOffset;
        Vector2 pos = isAddOffset ? offset : Vector2.zero;
        pos.x += leftTop.x;
        pos.y -= leftTop.y;
        if (Columns == 0) //一行
        {
            pos.x += getPos(true, index);
        }
        else //(Columns == 1) //一列
        {
            pos.y -= getPos(false, index);
        }
        return pos;
    }
    private float getPos(bool isX, int index)
    {
        if (luaDataPrefabList == null || luaDataPrefabList.Count == 0) return 0;
        float temp = 0;
        index = index <= luaDataPrefabList.Count ? index : luaDataPrefabList.Count;
        for (int i = 0; i < index; i++)
        {
            if (isX)
            {
                temp += renderItemInfoList[i].itemWidth + pad.x;
            }
            else
            {
                temp += renderItemInfoList[i].itemHeight + pad.y;
            }
        }
        return temp;
    }
    /// <summary>
    /// 按数量回收池
    /// </summary>
    /// <param name="num"></param>
    protected virtual void ResetRenderPool(int num)
    {
        int i = _preDataCount - 1;//从尾部开始回收
        while (num > 0) //回收当前渲染的render
        {
            BaseRender render;
            renderIndexDic.TryGetValue(i, out render);
            if (render != null)//有可能此时这项还没生成
            {
                render.Render.Recycle();
                if (horizontal)
                {
                    render.Render.rectTransform.localPosition = new Vector3(0, 99999, 0);
                }
                else
                {
                    render.Render.rectTransform.localPosition = new Vector3(99999, 0);
                }
                //render.Render.gameObject.SetActive(false);
                drawingRenders.Remove(render);
                renderIndexDic.Remove(i);
                if (availableRenderDic.TryGetValue(render.prefabName, out List<BaseRender> l)) l.Add(render);
                else
                {
                    availableRenderDic[render.prefabName] = new List<BaseRender>();
                    availableRenderDic[render.prefabName].Add(render);
                }
            }
            i--;
            num--;
        }
    }


    protected virtual void ClipRenderByIndex(int index)
    {
        BaseRender render;
        renderIndexDic.TryGetValue(index, out render);
        if (render != null)//有可能此时这项还没生成
        {
            render.Recycle();
            if (horizontal)
            {
                render.Render.rectTransform.localPosition = new Vector3(0, 99999, 0);
            }
            else
            {
                render.Render.rectTransform.localPosition = new Vector3(99999, 0);
            }
            //render.Render.gameObject.SetActive(false);
            drawingRenders.Remove(render);
            renderIndexDic.Remove(index);
            if (availableRenderDic.TryGetValue(render.prefabName, out List<BaseRender> l)) l.Add(render);
            else
            {
                availableRenderDic[render.prefabName] = new List<BaseRender>();
                availableRenderDic[render.prefabName].Add(render);
            }
        }
    }

    /// <summary>
    /// 创建ItemRender(c#层)
    /// </summary>
    /// <returns></returns>
    protected virtual BaseRender CreateItemRender(string prefabName)
    {
        CIrregularBaseRender renderPrefab = RenderItemInfoDic[prefabName].renderPrefab;
        BaseRender render = Activator.CreateInstance(itemRender) as BaseRender;
        if (render != null)
        {
            GameObject renderObj = Instantiate(renderPrefab.gameObject);
            CIrregularBaseRender cbRender = renderObj.GetComponent<CIrregularBaseRender>();
            AddChild(cbRender.gameObject, Vector3.zero);
            cbRender.SetPrefabSizeUpdateCallBack(UpdateRenderItemInfoList);
            if (luaItemRenderInfoDic[prefabName] != null)
            {
               
            }
            render.prefabName = prefabName;
            render.Render = cbRender;
            if (managerRenderSelect)
            {
                cbRender.AddClick(OnRenderClick);
            }
            renderObj.transform.localScale = Vector3.one * itemScale;
            return render;
        }
        return null;
    }
    /// <summary>
    /// 更新Lua数据 index 位置 对应的预设的大小
    /// </summary>
    /// <param name="index"></param>
    protected void UpdateRenderItemInfoList(CIrregularBaseRender cirregularBaseRender, int index)
    {
        prefabSizeChanged = true;
        CIrregularBaseRenderItemInfo cirregularBaseRenderItemInfo = renderItemInfoList[index];
        cirregularBaseRenderItemInfo.UpdateInfo(cirregularBaseRender);
        
        //OnDimensionsChange();
        //ContentChanged(true);
        //RefreshCurrentRenderPos();
        //prefabSizeChanged = false;
    }
    /// <summary>
    /// 点击了BaseRender
    /// </summary>
    /// <param name="eventData"></param>
    protected virtual void OnRenderClick(PointerEventData eventData)
    {
        if (eventData.pointerPress == null)
        {
            return;
        }
        CIrregularBaseRender cbRender = eventData.pointerPress.GetComponent<CIrregularBaseRender>();
        int index = cbRender.Index;
        if (IsSelected(index)) //反选
        {
            if (maxSelectNum == 1)
            {
                SetRenderSelect(index, true); //还是要派发回调的
            }
            else
            {
                DeSelect(index);
            }
        }
        else //选中
        {
            AppendSelect(index);
        }
    }

    /// <summary>
    /// 获取当前渲染的[开始Index].
    /// </summary>
    /// <returns></returns>
    protected virtual int GetDrawRendererStartIndex()
    {
        int start;
        Vector2 nowPos = content.anchoredPosition;
        nowPos.x = Mathf.Clamp(nowPos.x, nowPos.x, 0); //X最大为0
        nowPos.y = Mathf.Clamp(nowPos.y, 0, nowPos.y); //Y最小为0
        if (Columns == 0) //一行
        {
            start = getIndex(true, -nowPos.x);
        }
        else //一列 Columns == 1
        {
            start = getIndex(false, nowPos.y);
        }
        return ClampIndex(start);
    }

    private int getIndex(bool isX, float pos)
    {
        if (luaDataPrefabList == null || luaDataPrefabList.Count == 0) return 0;
        float temp = 0;
        for (int i = 0; i < luaDataPrefabList.Count; i++)
        {
            temp += isX
                ? (/*RenderItemInfoDic[luaDataPrefabList[i]]*/renderItemInfoList[i].itemWidth + pad.x)
                : (/*RenderItemInfoDic[luaDataPrefabList[i]]*/renderItemInfoList[i].itemHeight + pad.y);
            if (temp > pos)
            {
                return i;
            }
        }
        return 0;
    }

    /// <summary>
    /// 获取内容的边框盒大小（子项定长）
    /// </summary>
    /// <returns></returns>
    protected override Bounds GetBounds()
    {
        if (dataChanged||prefabSizeChanged)
        {
            int count = DataCount;
            Vector3 center = Vector3.zero;
            Vector3 size = Vector3.zero;
            size.x += leftTop.x;
            size.y += leftTop.y;
            Vector2 viewSize = viewRect.rect.size;
            if (Columns == 0) //一行
            {
                size.x += getPos(true, count);
            }
            else  //一列
            {
                size.y += getPos(false, count);
            }
            size.x = Mathf.Max(size.x, viewSize.x);
            size.y = Mathf.Max(size.y, viewSize.y);
            center.x = size.x / 2;
            center.y = -size.y / 2;
            mBounds = new Bounds(center, size);
        }
        return mBounds;
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
        //if (!Application.isPlaying) return;
        int preShowNum = maxShowNum;
        Vector2 size = mask.rect.size;
        if (Columns == 0) //一行
        {
            maxShowNum = getMaxShow(true, size.x - leftTop.x);
        }
        else //一列
        {
            maxShowNum = getMaxShow(false, (size.y - leftTop.y));
        }
        if (preShowNum != maxShowNum)
        {
            DataChanged();
        }
    }

    private int getMaxShow(bool isX, float size)
    {
        //这里取最小的尺寸来算可以显示多少个。
        float prefabSize = 0;
        foreach (var item in RenderItemInfoDic)
        {
            if (prefabSize == 0) prefabSize = isX ? item.Value.itemWidth : item.Value.itemHeight;
            else
            {
                float tempS = isX ? item.Value.itemWidth : item.Value.itemHeight;
                if (tempS < prefabSize) prefabSize = tempS;
            }
        }
        return (int)Math.Ceiling(size / (prefabSize + (isX ? pad.x : pad.y)));
    }
    /// <summary>
    /// 设置render选中状态
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    private void SetRenderSelect(int index, bool value)
    {
        selectIndexDic[index] = value;
        //前提是该render已经生成
        if (renderIndexDic.ContainsKey(index))
        {
            renderIndexDic[index].Render.Select = value;
        }

        if (value && onSelect != null) //选中回调
        {
            onSelect(index);
        }
        else if (!value && onDeSelect != null) //取消选中回调
        {
            onDeSelect(index);
        }
    }

    /// <summary>
    /// 设置render选中状态，并且不触发事件
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    private void SetRenderSelectWithoutFunc(int index, bool value)
    {
        selectIndexDic[index] = value;
        //前提是该render已经生成
        if (renderIndexDic.ContainsKey(index))
        {
            renderIndexDic[index].Render.Select = value;
        }
    }

    /// <summary>
    /// 清空选中
    /// </summary>
    private void ClearSelect()
    {
        SelectList.Clear();
        selectIndexDic.Clear();
    }


    private int GetMaxEmptyIndex()
    {
        int maxIndex = maxShowNum;
        int oriDataCount = DataCount;
        if (oriDataCount > maxShowNum)
        {
            int maxCol = Mathf.Max(1, Columns);
            int overMaxShowNum = oriDataCount - maxShowNum;
            int emptyNum = overMaxShowNum % maxCol;
            maxIndex = oriDataCount + maxCol - emptyNum;
        }
        return maxIndex;
    }

    /// <summary>
    /// 索引限制
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    protected int ClampIndex(int index)
    {
        return Mathf.Clamp(index, 0, DataCount > 0 ? DataCount - 1 : 0);
    }

    /// <summary>
    /// 限制数据源数量
    /// </summary>
    private void ClampDataProvider()
    {
        if (dataProvider != null && dataCountLimit > 0 && dataProvider.Count > 0)
        {
            int cut = dataProvider.Count - dataCountLimit;
            while (cut > 0)
            {
                dataProvider.RemoveAt(0); //从前往后删除
                cut--;
            }
        }
    }

    private void OnAdhere(int index)
    {
        if (onAdhere != null)
        {
            onAdhere(index);
        }
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        itemRender = null;
        selectList = null;
        dataProvider = null;
        onSelect = null;
        onDeSelect = null;
        onDataRequest = null;
        onAdhere = null;
       
        ClearRenderPrefab();

        foreach (BaseRender baseRender in drawingRenders)
        {
            baseRender.OnDestroy();
        }
        drawingRenders.Clear();
        drawingRenders = null;

        foreach (var item in availableRenderDic)
        {
            for (int i = 0; i < item.Value.Count; i++)
            {
                item.Value[i].OnDestroy();
            }

            item.Value.Clear();
        }
        availableRenderDic.Clear();
        availableRenderDic = null;



        selectIndexDic.Clear();
        renderIndexDic.Clear();
        selectIndexDic = null;
        renderIndexDic = null;
    }

    private void ClearRenderPrefab()
    {
        foreach (var item in RenderItemInfoDic)
        {
            item.Value.renderPrefab = null;
        }
        RenderItemInfoDic.Clear();
        // for (int i = 0; i < renderPrefabList.Count; i++)
        // {
        //     Destroy(renderPrefabList[i]);
        //     renderPrefabList[i] = null;
        // }
        renderItemInfoDic = null;
        renderPrefabList = null;
    }

    #endregion
}