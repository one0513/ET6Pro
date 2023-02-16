using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//仅支持，单行或者单列
//默认的itemHeight/itemWidth是最小的尺寸，也就是动态变化只能加大不能变小（影响item数量，所以要这样限制）
//item的Anchors和Pivot都会被设置成左上角，方便计算高度
//item高度改变，必须要同步到item的sizeDelta；会按照这种方式来计算实际高度，不使用lua回调
//ForceRefreshIndex方法可用于实现类似item展开的情况（暂时不支持动画，根据情况可以item内部实现动画....）
public class CFlexibleList : CCanvas {

    public Vector2 pad; //间距
    public Vector2 leftTop;//左
    public float itemScale = 1; //render的缩放
    public int dataCountLimit = 0; //数据项长度限制（超出则会被裁减），0为不限制
    public int maxSelectNum = 1; //最大选中数量 0：不限  n:n个
    public CBaseRender renderPrefab; //子对象渲染预设
    public bool managerRenderSelect = true;
    public bool loopItem = true;
    public bool itemTweenAlpha = true;
    public static float itemTweenAlphaSpeed = 0.05f;
    public static float itemTweenAlphaTime = 0.2f;

    [NonSerialized]
    public Type itemRender = typeof(BaseRender); //c#层的render,必须是ListItemRender或者其子类

    protected bool dataChanged = true; //数据源是否改变？
    protected int maxShowNum; //当前最多可显示多少个render
    protected int startIndex = -1; //当前渲染的首索引
    protected int endIndex = -1;//当前渲染的尾索引
    protected List<int> selectList; //多选索引列表
    protected Vector2 beginDragPos; //开始拖拽时的content位置
  
    protected List<object> dataProvider; //c#层数据源
    protected UIEventListener.IntDelegate onSelect; //选中回调（返回索引）
    protected UIEventListener.IntDelegate onDeSelect; //取消选中回调（返回索引）
    protected UIEventListener.IntDelegate onDataRequest; //数据请求回调

    protected List<BaseRender> drawingRenders = new List<BaseRender>(); //正在渲染的render
    protected List<BaseRender> availableRenders = new List<BaseRender>(); //缓存的render
    protected Dictionary<int, bool> selectIndexDic = new Dictionary<int, bool>(); //选中的索引字典
    protected Dictionary<int, BaseRender> renderIndexDic = new Dictionary<int, BaseRender>(); //正在渲染的render的索引字典

    protected int curFixIndex = 0;
    protected bool contentChange = false;
    protected bool forceDrawListOnce = false;
    protected Vector2 curMaxBoundSize = Vector2.zero;
    protected List<Vector2> itemBeginPos = new List<Vector2>();
    protected List<int> itemFixRecord = new List<int>();

    //数据总项
    public int DataCount {
        get { return dataProvider == null ? 0 : dataProvider.Count; }
    }

    //子项宽度
    public float ItemWidth {
        get { return renderPrefab == null ? 0 : renderPrefab.Width * itemScale; }
    }

    //子项高度
    public float ItemHeight {
        get { return renderPrefab == null ? 0 : renderPrefab.Height * itemScale; }
    }

    //子项锚点偏移量
    public Vector2 ItemOffset {
        get { return new Vector2(ItemWidthOffset, -ItemHeightOffset); }
    }

    //子项锚点宽度偏移量
    public float ItemWidthOffset {
        get { return renderPrefab == null ? 0 : ItemWidth * renderPrefab.rectTransform.pivot.x; }
    }

    //子项锚点高度偏移量
    public float ItemHeightOffset {
        get { return renderPrefab == null ? 0 : ItemHeight * (1 - renderPrefab.rectTransform.pivot.y); }
    }
   
    #region 外部接口
    /// <summary>
    /// 设置选中(单选)
    /// 无则返回-1
    /// </summary>
    public virtual int SelectIndex {
        get { return SelectList.Count > 0 ? SelectList[0] : -1; }
        set {
            if (SelectIndex != value) {
                SelectList = new List<int> { value };
            }
        }
    }

   
    /// <summary>
    /// 设置选中(多选)
    /// 无选中则返回空列表
    /// </summary>
    public List<int> SelectList {
        get { return selectList ?? (selectList = new List<int>()); }
        set {
            //1.清除选中
            if (selectList != null) {
                for (int i = 0; i < selectList.Count; i++) {
                    SetRenderSelect(selectList[i], false);
                }
            }
            //2.更新选中
            selectList = value;
            //3.设置选中
            if (selectList != null) {
                if (maxSelectNum > 0) //不能超出数量
                {
                    while (selectList.Count > maxSelectNum) {
                        selectList.RemoveAt(selectList.Count - 1);
                    }
                }

                for (int i = 0; i < selectList.Count; i++) {
                    SetRenderSelect(selectList[i], true);
                }
            }
        }
    }

    public void SelectIndexWithoutFunc(int index) {
        //1.清除选中
        if (selectList != null) {
            for (int i = 0; i < selectList.Count; i++) {
                SetRenderSelectWithoutFunc(selectList[i], false);
            }
        }
        //2.更新选中
        selectList = new List<int>() { index };
        //3.设置选中
        if (selectList != null) {
            if (maxSelectNum > 0) //不能超出数量
                {
                while (selectList.Count > maxSelectNum) {
                    selectList.RemoveAt(selectList.Count - 1);
                }
            }

            for (int i = 0; i < selectList.Count; i++) {
                SetRenderSelectWithoutFunc(selectList[i], true);
            }
        }
    }

    /// <summary>
    /// 是否选中了？
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public bool IsSelected(int index) {
        bool state;
        selectIndexDic.TryGetValue(index, out state);
        return state;
    }

    /// <summary>
    /// 添加选中
    /// </summary>
    /// <param name="index"></param>
    public void AppendSelect(int index) {
        if (maxSelectNum == 1) {
            //单选
            SelectIndex = index;
        } else if (maxSelectNum == 0 || SelectList.Count < maxSelectNum) {
            //多选
            if (IsSelected(index) == false) {
                SelectList.Add(index);
                SetRenderSelect(index, true);
            }
        } else {
            //达到上限
            Debug.LogWarning("可选择数量已达到上限");
        }
    }

    /// <summary>
    /// 取消选中
    /// </summary>
    /// <param name="index"></param>
    public void DeSelect(int index) {
        if (IsSelected(index)) {
            SelectList.Remove(index);
            SetRenderSelect(index, false);
        }
    }

    /// <summary>
    /// 添加选中的回调
    /// </summary>
    /// <param name="callback"></param>
    public void AddSelect(UIEventListener.IntDelegate callback) {
        onSelect = callback;
    }

    /// <summary>
    /// 删除选中回调
    /// </summary>
    public void RemoveSelect() {
        onSelect = null;
    }

    /// <summary>
    /// 添加取消选中的回调
    /// </summary>
    /// <param name="callback"></param>
    public void AddDeSelect(UIEventListener.IntDelegate callback) {
        onDeSelect = callback;
    }

    /// <summary>
    /// 删除取消选中回调
    /// </summary>
    public void RemoveDeSelect() {
        onDeSelect = null;
    }

    /// <summary>
    /// 添加数据请求回调
    /// </summary>
    /// <param name="callback"></param>
    public void AddDataRequest(UIEventListener.IntDelegate callback) {
        onDataRequest = callback;
    }

    /// <summary>
    /// 删除数据请求回调
    /// </summary>
    public void RemoveDataRequest() {
        onDataRequest = null;
    }

 

    public void SetDataPrivide(object obj) {

    }
    /// <summary>
    /// 设置数据源(c#层)
    /// </summary>
    public List<object> DataProvider {
        get { return dataProvider; }
        set {
            dataProvider = value;
            ClampDataProvider(); //裁减数据源
            ClearSelect(); //清除选择
            DataChanged(); //设置脏读
        }
    }

    /// <summary>
    /// 补充一个数据源(lua / c# 均可)
    /// </summary>
    /// <param name="vo"></param>
    /// <param name="goTo">是否定位到这个item</param>
    /// <param name="anim"></param>
    public void AppendData(object vo, bool goTo = true, bool anim = true) {
        AppendDataList(new List<object>() { vo }, goTo, anim);
    }

   
    /// <summary>
    /// 补充一组数据源（c#）
    /// </summary>
    /// <param name="list"></param>
    /// <param name="goTo">是否定位到补充的第一个数据的index</param>
    /// <param name="anim"></param>
    public void AppendDataList(List<object> list, bool goTo = true, bool anim = true) {
        if (dataProvider == null) dataProvider = new List<object>();
        int length = dataProvider.Count;
        for (int i = 0; i < list.Count; i++) {
            dataProvider.Add(list[i]);
        }
        ClampDataProvider(true); //裁减数据源
        DataChanged(); //设置脏读
        if (goTo) {
            GotoIndex(length, anim);
        }
    }

    /// <summary>
    /// 通过索引获取数据
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public object GetItemData(int index) {
        if (index >= 0 && index < dataProvider.Count) {
            return dataProvider[index];
        }
        return null;
    }

    /// <summary>
    /// 数据源改变标记（整个数据源改变或者单个数据改变了可调用一次）
    /// </summary>
    public void DataChanged(bool updateNow = false) {
        dataChanged = true;
        OnDataChange();
        if (updateNow) {
            DrawList();
        }
    }

    /// <summary>
    /// 定位到索引位置
    /// </summary>
    /// <param name="index"></param>
    /// <param name="anim"></param>
    /// <param name="callback"></param>
    public virtual void GotoIndex(int index, bool anim = false, TweenCallback callback = null) {
        if (dataChanged) {
            DataChanged(true);
        }
        Vector2 pos = getItemBeginPosAsync(index);//todo 异步处理，因为某些未实例化的index需要进行预处理（否则会跳到错误的位置）
        pos *= -1;

        //todo 这里是直接把ScrollTo的部分代码复制进来，重写方法导致发布机toLua编译报错
        StopMovement();
        //pos = ClampPosition(pos);//todo 这里要屏蔽，因为异步处理的时候，bound的值不一定是正确的，会导致提前裁剪结束位置
        if (anim) {
            // Tweener tweener = content.DOAnchorPos(pos, easeTime);
            // tweener.SetEase(ease);
            // tweener.OnComplete(callback);
        } else {
            SetContentAnchoredPosition(pos);
            DrawList();
            if (callback != null) {
                callback();
            }
        }
    }

   

    /// <summary>
    /// 增量索引移动
    /// </summary>
    /// <param name="step"> 大于0：右/下  小于0：左上</param>
    /// <param name="anim"></param>
    /// <param name="callback"></param>
    public virtual void MoveStep(int step, bool anim = false, TweenCallback callback = null) {
        GotoIndex(ClampIndex(startIndex + step), anim, callback);
    }

    public void ResetStartIndex() {
        startIndex = -1;
    }

    public void ForceRefreshIndex() {
        forceDrawListOnce = true;
        DrawList();
    }

    #endregion

    #region 内部接口

    /// <summary>
    /// 初始化
    /// </summary>
    protected override void OnInit() {
        base.OnInit();
        AddPositionChange(OnPositionChange);
        CalculateMaxShowNum();
    }

    /// <summary>
    /// 位置改变了刷新下渲染
    /// </summary>
    /// <param name="pos"></param>
    protected virtual void OnPositionChange(Vector2 pos) {
        DrawList();
    }

    protected virtual void OnDataChange() {

    }

    protected override void OnUpdate() {
        base.OnUpdate();

        if (dataChanged) {
            DrawList();
        }
    }

    public override void OnBeginDrag(PointerEventData eventData) {
        base.OnBeginDrag(eventData);
        beginDragPos = ClampPosition(content.anchoredPosition);
    }

    public override void OnDrag(PointerEventData eventData) {
        base.OnDrag(eventData);
    }

    public override void OnEndDrag(PointerEventData eventData) {
        base.OnEndDrag(eventData);
        //拖拽到最右或者最下，用于异步请求数据
        if (onDataRequest != null) {
            if ((horizontal && horizontalNormalizedPosition > 1) || (vertical && verticalNormalizedPosition < 0)) {
                onDataRequest(DataCount); //返回当前数据总数
            }
        }
    }

    private int _preDataCount = 0;
    /// <summary>
    /// 渲染列表
    /// </summary>
    protected virtual void DrawList() {
        if (!Application.isPlaying) return;
        if (renderPrefab == null) return;
        int start = GetDrawRendererStartIndex();
        //如果仅仅是位置改变了而且渲染的第一个没变则不用再管了
        if (!dataChanged && start == startIndex && !forceDrawListOnce) return;

        //循环利用Item的模式
        var isDoTween = start > startIndex || dataChanged;
        if (loopItem) {
            int off = start - startIndex;
            if (off > 0) {
                //往上拉了，原先的stat位置已经隐藏，差值大于1表示拖得太快了
                while (off > 0) {
                    ClipRenderByIndex(startIndex + off - 1);
                    off--;
                }
            } else if (off < 0) {
                //往下拉，原先的last位置已经隐藏，差值绝对值大于1表示拖得太快了
                while (off < 0) {
                    ClipRenderByIndex(endIndex + off + 1);
                    off++;
                }
            }
        }

        //数据源发生了改变
        if (dataChanged) {
            //1.设置脏读
            foreach (BaseRender render in renderIndexDic.Values) {
                render.Render.Select = false;
                render.IsDirty = true;
            }
            //2.回收冗余
            int nowDataCount = DataCount;
            int gapNum = _preDataCount - nowDataCount;
            //数量有变化
            if (gapNum != 0) {
                //回收这些冗余的
                if (gapNum > 0) {
                    ResetRenderPool(gapNum);
                }
                _preDataCount = nowDataCount;
                contentChange = true;
            }
            dataChanged = false;
        }
        Vector2 endPos = content.anchoredPosition + SelfTransform.sizeDelta;
        BaseRender cellRender;
        Vector2 offset = ItemOffset;
        var tAlphaIndex = 0;
        var isHeightChange = forceDrawListOnce;
        var jumpPos = Vector2.zero;
        //todo 重新指定一个maxNum用于防止每帧生成的item数量过大【Done】
        //for (int i = start; i < itemBeginPos.Count - 1; i++) {
        for (int i = start; i < maxShowNum + start; i++) {
            if (i >= DataCount) break;
            renderIndexDic.TryGetValue(i, out cellRender);
            endIndex = i;
            var isNotRender = cellRender == null;
            if (cellRender == null) {
                //当前这个索引的render还未生成
                cellRender = GetOneRender();

                renderIndexDic[i] = cellRender;
                drawingRenders.Add(cellRender);
                CBaseRender cbRender = cellRender.Render;
                cbRender.Index = i; //设置索引
                cbRender.gameObject.name = renderPrefab.gameObject.name + i; //设置名称
                cbRender.rectTransform.pivot = new Vector2(0, 1);
                cbRender.rectTransform.localPosition = Vector3.zero;
                cbRender.gameObject.SetActive(true); //显示
                cellRender.IsDirty = false;
                cellRender.SetData(GetItemData(i)); //设置数据

                cbRender.rectTransform.anchoredPosition = getItemBeginPos(i); //设置位置
                if (UpdateItemBeginPos(i + 1, getItemFlexiblePos(i, cbRender.rectTransform.sizeDelta, offset))) {
                    contentChange = true;
                    curFixIndex = i;
                    //todo 完善补充逻辑
                    if (start - startIndex < 0) {
                        isHeightChange = true;
                    }
                }

                if (managerRenderSelect) cellRender.Render.Select = IsSelected(i); //设置选中
            } else if (cellRender.IsDirty) {
                //已经生成了并且脏读了，则要重新设置数据
                cellRender.IsDirty = false;
                cellRender.SetData(GetItemData(i));

                cellRender.SelfTransform.anchoredPosition = getItemBeginPos(i); //设置位置
                if (UpdateItemBeginPos(i + 1, getItemFlexiblePos(i, cellRender.SelfTransform.sizeDelta, offset))) {
                    contentChange = true;
                    curFixIndex = i;
                    //todo 完善补充逻辑
                    if (start - startIndex < 0) {
                        isHeightChange = true;
                    }
                }

                if (managerRenderSelect) cellRender.Render.Select = IsSelected(i); //设置选中
            } else if (isHeightChange) {
                var curPos = cellRender.SelfTransform.anchoredPosition;
                cellRender.SelfTransform.anchoredPosition = getItemBeginPos(i); //设置位置
                if (UpdateItemBeginPos(i + 1, getItemFlexiblePos(i, cellRender.SelfTransform.sizeDelta, offset))) {
                    contentChange = true;
                    curFixIndex = i;
                    if (cellRender.SelfTransform.anchoredPosition != curPos) {
                        jumpPos = cellRender.SelfTransform.anchoredPosition - curPos;
                    }
                }
            }
            if (isDoTween && (startIndex == -1 || isNotRender)) {
                tAlphaIndex++;
                DoTweenAlpha(cellRender.Render, tAlphaIndex);
            }
            //防止多余绘制（同时为了防止绘制失败，特地加上一个阈值，暂时用ItemWidth/ItemHeight作为阈值）
            Vector2 checkPos = getItemBeginPos(i);
            if (horizontal) {
                if (checkPos.x > (endPos.x + ItemWidth)) {
                    break;
                }
            } else {
                if (Mathf.Abs(checkPos.y) > (endPos.y + ItemHeight)) {
                    break;
                }
            }
        }

        startIndex = start;
        //由于更新content会重置部分bound数据，所以在这里执行
        if (contentChange) {
            ContentChanged(true);
            contentChange = false;
        }
        //强制重绘之后，需要还原
        if (forceDrawListOnce) {
            forceDrawListOnce = false;
        } else {
            //检测到高度改变之后，为了拖动的效果，需要改变位移
            if (isHeightChange) {
                //Debug.Log("jump pos before" + content.anchoredPosition);
                content.anchoredPosition -= jumpPos;
                m_ContentStartPosition -= jumpPos;
                //Debug.Log("jump pos " + jumpPos);
                //Debug.Log("jump pos after" + content.anchoredPosition);
            }
        }
    }

    //todo 这里是位置检测log，不需要的时候屏蔽【Done】
    //protected override void SetContentAnchoredPosition(Vector2 position){
    //    base.SetContentAnchoredPosition(position);
    //    Debug.Log("set position" + position);
    //}

    private void DoTweenAlpha(CBaseRender render, int tIndex) {
        if (itemTweenAlpha == false) return;
        MingUIUtil.SetGroupAlpha(render.gameObject, 0);
        render.tweenAlpha = MingUIUtil.SetGroupAlphaInTime(render.gameObject, 1, itemTweenAlphaTime, Ease.OutCubic).SetDelay(tIndex * itemTweenAlphaSpeed);
    }

    protected BaseRender GetOneRender() {
        BaseRender cellRender;
        int last = availableRenders.Count - 1;
        if (last >= 0) {
            //直接从资源池里拿
            cellRender = availableRenders[last];
            availableRenders.RemoveAt(last);
        } else {
            //需要创建了
            cellRender = CreateItemRender();
        }
        return cellRender;
    }

    /// <summary>
    /// 定位render（考虑render的锚点的偏移）
    /// </summary>
    /// <param name="index">数据项索引</param>
    /// <param name="offset">偏移量</param>
    /// <returns></returns>
    protected virtual Vector2 GetRenderPos(int index, Vector2 offset) {
        Vector2 pos = offset;
        pos.x += leftTop.x;
        pos.y -= leftTop.y;
        if (horizontal) {
            pos.x += index * (ItemWidth + pad.x);
        } else {
            pos.y -= index * (ItemHeight + pad.y);
        }
        return pos;
    }

    /// <summary>
    /// 按数量回收池
    /// </summary>
    /// <param name="num"></param>
    protected virtual void ResetRenderPool(int num) {
        int i = _preDataCount - 1;//从尾部开始回收
        //回收当前渲染的render
        while (num > 0) {
            BaseRender render;
            renderIndexDic.TryGetValue(i, out render);
            //有可能此时这项还没生成
            if (render != null) {
                render.Render.Recycle();
                if (horizontal) {
                    render.Render.rectTransform.localPosition = new Vector3(0, 99999, 0);
                } else {
                    render.Render.rectTransform.localPosition = new Vector3(99999, 0);
                }
                //render.Render.gameObject.SetActive(false);
                drawingRenders.Remove(render);
                renderIndexDic.Remove(i);
                availableRenders.Add(render);
            }
            i--;
            num--;
        }
    }

    protected virtual void ClipRenderByIndex(int index) {
        BaseRender render;
        renderIndexDic.TryGetValue(index, out render);
        //有可能此时这项还没生成
        if (render != null) {
            render.Render.Recycle();
            if (horizontal) {
                render.Render.rectTransform.localPosition = new Vector3(0, 99999, 0);
            } else {
                render.Render.rectTransform.localPosition = new Vector3(99999, 0);
            }
            //render.Render.gameObject.SetActive(false);
            drawingRenders.Remove(render);
            renderIndexDic.Remove(index);
            availableRenders.Add(render);
        }
    }

    /// <summary>
    /// 创建ItemRender(c#层)
    /// </summary>
    /// <returns></returns>
    protected virtual BaseRender CreateItemRender() {
        BaseRender render = Activator.CreateInstance(itemRender) as BaseRender;
        if (render != null) {
            GameObject renderObj = Instantiate(renderPrefab.gameObject);
            CBaseRender cbRender = renderObj.GetComponent<CBaseRender>();
            AddChild(cbRender.gameObject, Vector3.zero);

           
            render.Render = cbRender;
            if (managerRenderSelect) {
                cbRender.AddClick(OnRenderClick);
            }
            renderObj.transform.localScale = Vector3.one * itemScale;
            return render;
        }
        return null;
    }

    /// <summary>
    /// 点击了BaseRender
    /// </summary>
    /// <param name="eventData"></param>
    protected virtual void OnRenderClick(PointerEventData eventData) {
        if (eventData.pointerPress == null) {
            return;
        }
        CBaseRender cbRender = eventData.pointerPress.GetComponent<CBaseRender>();
        int index = cbRender.Index;
        if (IsSelected(index)) {
            //反选
            if (maxSelectNum == 1) {
                SetRenderSelect(index, true); //还是要派发回调的
            } else {
                DeSelect(index);
            }
        } else {
            //选中
            AppendSelect(index);
        }
    }

    /// <summary>
    /// 获取当前渲染的[开始Index].
    /// </summary>
    /// <returns></returns>
    protected virtual int GetDrawRendererStartIndex() {
        int start = -1;
        Vector2 nowPos = content.anchoredPosition;
        if (horizontal) {
            for (int i = 0; i < itemBeginPos.Count; i++) {
                if (itemBeginPos[i].x < nowPos.x) {
                    if (i >= itemBeginPos.Count - 1) {
                        start = i;
                        break;
                    } else {
                        if (itemBeginPos[i + 1].x >= nowPos.x) {
                            start = i;
                            break;
                        }
                    }
                }
            }
        } else {
            for (int i = 0; i < itemBeginPos.Count; i++) {
                if (Mathf.Abs(itemBeginPos[i].y) < nowPos.y) {
                    if (i >= itemBeginPos.Count - 1) {
                        start = i;
                        break;
                    } else {
                        if (Mathf.Abs(itemBeginPos[i + 1].y) >= nowPos.y) {
                            start = i;
                            break;
                        }
                    }
                }
            }
        }

        return ClampIndex(start);
    }

    /// <summary>
    /// 获取内容的边框盒大小（子项定长）
    /// </summary>
    /// <returns></returns>
    protected override Bounds GetBounds() {
        if (contentChange || dataChanged) {
            int count = DataCount;
            Vector3 center = Vector3.zero;
            Vector3 size = Vector3.zero;
            size.x += leftTop.x;
            size.y += leftTop.y;
            Vector2 viewSize = viewRect.rect.size;
            int checkIndex = Mathf.Min(curFixIndex + 1, count);
            if (horizontal) {
                if (curMaxBoundSize.x == 0) {
                    curMaxBoundSize.x = size.x + count * (ItemWidth + pad.x) - (count > 0 ? pad.x : 0);
                }
                if (itemBeginPos.Count > checkIndex) {
                    size.x += Mathf.Abs(itemBeginPos[checkIndex].x);
                }
                if (size.x > curMaxBoundSize.x) {
                    curMaxBoundSize.x = size.x;
                }
            } else {
                if (curMaxBoundSize.y == 0) {
                    curMaxBoundSize.y = size.y + count * (ItemHeight + pad.y) - (count > 0 ? pad.y : 0);
                }
                if (itemBeginPos.Count > checkIndex) {
                    size.y += Mathf.Abs(itemBeginPos[checkIndex].y);
                }
                if (size.y > curMaxBoundSize.y) {
                    curMaxBoundSize.y = size.y;
                }
            }
            size.x = Mathf.Max(curMaxBoundSize.x, viewSize.x);
            size.y = Mathf.Max(curMaxBoundSize.y, viewSize.y);
            center.x = size.x / 2;
            center.y = -size.y / 2;
            mBounds = new Bounds(center, size);
            //Debug.Log("bound size is " + size);
        }
        return mBounds;
    }

    /// <summary>
    /// 面积发生改变
    /// </summary>
    public override void OnDimensionsChange() {
        base.OnDimensionsChange();
        CalculateMaxShowNum();
    }

    /// <summary>
    /// 计算最大显示数量
    /// </summary>
    protected virtual void CalculateMaxShowNum() {
        if (!Application.isPlaying) return;
        int preShowNum = maxShowNum;
        Vector2 size = mask.rect.size;
        if (horizontal) {
            maxShowNum = (int)Math.Ceiling((size.x - leftTop.x) / (ItemWidth + pad.x));
        } else {
            maxShowNum = (int)Math.Ceiling((size.y - leftTop.y) / (ItemHeight + pad.y));
        }
        if (preShowNum != maxShowNum) {
            DataChanged();
        }
    }

    /// <summary>
    /// 设置render选中状态
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    private void SetRenderSelect(int index, bool value) {
        selectIndexDic[index] = value;
        //前提是该render已经生成
        if (renderIndexDic.ContainsKey(index)) {
            renderIndexDic[index].Render.Select = value;
        }

        if (value && onSelect != null) {
            //选中回调
            onSelect(index);
        } else if (!value && onDeSelect != null) {
            //取消选中回调
            onDeSelect(index);
        }
    }

    /// <summary>
    /// 设置render选中状态，并且不触发事件
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    private void SetRenderSelectWithoutFunc(int index, bool value) {
        selectIndexDic[index] = value;
        //前提是该render已经生成
        if (renderIndexDic.ContainsKey(index)) {
            renderIndexDic[index].Render.Select = value;
        }
    }

    /// <summary>
    /// 清空选中
    /// </summary>
    private void ClearSelect() {
        SelectList.Clear();
        selectIndexDic.Clear();
    }

    /// <summary>
    /// 索引限制
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    protected int ClampIndex(int index) {
        return Mathf.Clamp(index, 0, DataCount > 0 ? DataCount - 1 : 0);
    }

    /// <summary>
    /// 限制数据源数量
    /// </summary>
    private void ClampDataProvider(bool isAppend = false) {
        if (dataProvider != null && dataCountLimit > 0 && dataProvider.Count > 0) {
            int cut = dataProvider.Count - dataCountLimit;
            while (cut > 0) {
                dataProvider.RemoveAt(0); //从前往后删除
                cut--;
            }
        }
        if (isAppend) {
            AppendItemBeginPos();
        } else {
            InitItemBeginPos();
        }
    }

    private void InitItemBeginPos() {
        Vector2 offset = ItemOffset;
        itemBeginPos.Clear();
        itemFixRecord.Clear();
        //这里多计算1位
        for (int i = 0; i <= dataProvider.Count; i++) {
            itemBeginPos.Add(GetRenderPos(i, offset));
            itemFixRecord.Add(i == 0 ? 1 : 0);
        }
        //重置一些参数
        RestFlexibleParam();
    }

    private void AppendItemBeginPos() {
        Vector2 offset = ItemOffset;
        //按照旧数据来设置新数据的偏移
        //if (itemBeginPos.Count > 0) {
        //    offset += getItemBeginPos(itemBeginPos.Count);
        //}
        //新数据添加偏移记录（同步处理：多计算1位）
        for (int i = itemBeginPos.Count; i <= dataProvider.Count; i++) {
            itemBeginPos.Add(GetRenderPos(i, offset));
            itemFixRecord.Add(0);
        }
    }

    private void RestFlexibleParam() {
        curMaxBoundSize = Vector2.zero;
        //清空旧的内容
        foreach (BaseRender render in renderIndexDic.Values) {
            if (render != null) {
                render.Render.Select = false;
                render.IsDirty = true;
                render.Render.Recycle();
                if (horizontal) {
                    render.Render.rectTransform.localPosition = new Vector3(0, 99999, 0);
                } else {
                    render.Render.rectTransform.localPosition = new Vector3(99999, 0);
                }
                availableRenders.Add(render);
            }
        }
        drawingRenders.Clear();
        renderIndexDic.Clear();
    }

    private Vector2 getItemBeginPos(int index) {
        if (index >= 0 && itemBeginPos.Count > index) {
            return itemBeginPos[index];
        } else {
            return Vector2.zero;
        }
    }

    private Vector2 getItemBeginPosAsync(int index) {
        if (index >= 0 && itemBeginPos.Count > index) {
            if (itemFixRecord[index] == 1) {
                return itemBeginPos[index];
            } else {
                if (index > curFixIndex) {
                    //todo 这里可能导致过大的数据计算（但是需要兼容gotoIndex之后，获取了错误的beginIndex的情况）【Cost Attendtion】
                    for (int i = curFixIndex + 1; i < index + 1; i++) {
                        if (itemFixRecord[i] == 0) {
                            itemBeginPos[i] = new Vector2(itemBeginPos[i].x, itemBeginPos[i - 1].y - ItemHeight - leftTop.y);
                        }
                    }
                }
                return itemBeginPos[index];
            }
        } else {
            return Vector2.zero;
        }
    }

    private Vector2 getItemFlexiblePos(int index, Vector2 sizeDelta, Vector2 offset) {
        Vector2 pos = offset + getItemBeginPos(index);
        //pos.x += leftTop.x;//begin的时候已经增加了，所以这里不再增加
        //pos.y -= leftTop.y;//begin的时候已经增加了，所以这里不再增加
        if (horizontal) {
            pos.x += (sizeDelta.x + pad.x);
        } else {
            pos.y -= (sizeDelta.y + pad.y);
        }
        return pos;
    }

    private bool UpdateItemBeginPos(int index, Vector2 pos) {
        if (itemBeginPos.Count > index && itemBeginPos[index] != pos) {
            itemBeginPos[index] = pos;
            itemFixRecord[index] = 1;//计算过一次的，需要打上正确的标记
            //Debug.Log(string.Format("update item pos[{0}, {1}]", index, pos));
            return true;
        }
        return false;
    }

    protected override void OnDestroy() {
        base.OnDestroy();
        itemRender = null;
        renderPrefab = null;
        selectList = null;
        dataProvider = null;
        onSelect = null;
        onDeSelect = null;
        onDataRequest = null;
       

        foreach (BaseRender baseRender in drawingRenders) {
            baseRender.OnDestroy();
        }
        foreach (BaseRender baseRender in availableRenders) {
            baseRender.OnDestroy();
        }

        drawingRenders.Clear();
        availableRenders.Clear();

        drawingRenders = null;
        availableRenders = null;

        selectIndexDic.Clear();
        renderIndexDic.Clear();
        selectIndexDic = null;
        renderIndexDic = null;

        itemBeginPos.Clear();
        itemFixRecord.Clear();
        itemBeginPos = null;
        itemFixRecord = null;
    }

    #endregion
}