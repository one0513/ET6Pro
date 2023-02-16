using System;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 循环列表（循环显示数量固定，content锚点在中心，render锚点也在中心）
/// </summary>
public class CLoopList : CList
{
    public enum Mode
    {
        Horizontal, //水平排列
        Vertical, //垂直排列
    }

    public Mode mode = Mode.Horizontal; //水平/垂直排列
    public float lerpScale = 0.9f; //如果不要缩放效果则设置为1即可

    private int _expandNum;
    private int _scrollNum; //当前滚动的数量
    private int _centerIndex = -1;

    protected UIEventListener.IntDelegate onChange; //中心索引改变回调add
    public int CenterIndex
    {
        get { return _centerIndex; }
        set
        {
            if (value != _centerIndex && value >= 0 && value < DataCount)
            {
                Vector2 pos = content.anchoredPosition;
                if (mode == Mode.Horizontal)
                {
                    pos.x = -value * (ItemWidth + pad.x);
                }
                else
                {
                    pos.y = value * (ItemHeight + pad.y);
                }
                StopMovement();
                content.anchoredPosition = pos;
                DataChanged(true);
            }
        }
    }
    /// <summary>
    /// 添加索引变换事件
    /// </summary>
    /// <param name="callback"></param>
    public void AddChange(UIEventListener.IntDelegate callback)
    {
        onChange = callback;
    }

    /// <summary>
    /// 移除索引变换事件
    /// </summary>
    public void RemoveChange()
    {
        onChange = null;
    }

    /// <summary>
    /// 当前实际渲染的render的总数,分别向两边拓展1个
    /// </summary>
    protected int DrawCount
    {
        get { return Mathf.Min(DataCount, maxShowNum + 2); }
    }

    protected Vector2 GapPos
    {
        get
        {
            Vector2 pos = Vector2.zero;
            if (mode == Mode.Horizontal)
            {
                pos.x = ItemWidth + pad.x;
            }
            else
            {
                pos.y = ItemHeight + pad.y;
            }
            return pos;
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
            Vector2 delta = eventData.position - beginDragPos;
            if (mode == Mode.Horizontal && Math.Abs(delta.x) > dragThreshold)
            {
                int add = delta.x > 0 ? -1 : 1;
                Scroll(add * GapPos, true);
            }
            else if (mode == Mode.Vertical && Math.Abs(delta.y) > dragThreshold)
            {
                int toIndex = delta.y > 0 ? startIndex + 1 : startIndex - 1;
                GotoIndex(ClampIndex(toIndex), true);
            }
        }
        else if (dragMode == DragMode.FollowAndAdhere) //先跟随再吸附
        {
            //暂未实现
        }
    }

    /// <summary>
    /// 位置改变了刷新下渲染
    /// </summary>
    /// <param name="pos"></param>
    protected override void OnPositionChange(Vector2 pos)
    {
        base.OnPositionChange(pos);
        if (lerpScale < 1)
        {
            for (int i = 0; i < drawingRenders.Count; i++)
            {
                BaseRender render = drawingRenders[i];
                render.SelfTransform.localScale = GetRenderScale(render);
            }
        }
    }

    protected override void DrawList()
    {
        int start = GetDrawRendererStartIndex();
        //如果仅仅是位置改变了而且渲染的第一个没变则不用再管了
        if (!dataChanged && start == startIndex) return;

        if (onChange != null) onChange(_centerIndex); //派发事件

        int gap = start - startIndex;
        if (Mathf.Abs(gap) != 1) //跨度了
        {
            gap = gap > 0 ? -1 : 1;
        }

        startIndex = start; //当前第一个渲染的数据索引
        //print("start:" + start);
        //print("center:" + _centerIndex);
        //print("_scrollNum:" + _scrollNum);
        int drawTotal = DrawCount;
        if (dataChanged) //全部回收
        {
            gap = 1;
            _expandNum = 0;
            ContentChanged(true);
            ResetRenderPool(drawingRenders.Count);
            dataChanged = false;
        }
        else if (DataCount >= maxShowNum) //查看当前回收哪个,前提是显示的数据超出内容
        {
            BaseRender render;

            if (gap > 0) //向左/向上滑动了,删除最前面的，往后面补充数据
            {
                render = drawingRenders[0];
            }
            else //向右/向下滑动了,删除最后面的，往前面补充数据
            {
                render = drawingRenders[drawingRenders.Count - 1];
            }
            //print("delete:" + render.Render.Index);
            drawingRenders.Remove(render);
            renderIndexDic.Remove(render.Render.Index);
            availableRenders.Add(render); //回收
        }
        else
        {
            return;
        }
        int dataIndex = start;
        int drawIndex = 0;
        BaseRender cellRender;
        while (drawIndex < drawTotal) //渲染当前显示的
        {
            if (renderIndexDic.ContainsKey(dataIndex) == false) //新的
            {
                cellRender = GetOneRender();
                cellRender.SetData(GetItemData(dataIndex)); //设置数据
                renderIndexDic[dataIndex] = cellRender;
                if (gap > 0) //往后面添加
                {
                    drawingRenders.Add(cellRender);
                }
                else
                {
                    drawingRenders.Insert(0, cellRender); //往前面添加
                }
                CBaseRender cbRender = cellRender.Render;
                cbRender.Index = dataIndex; //设置索引
                cbRender.Select = IsSelected(dataIndex); //设置选中
                cbRender.gameObject.name = renderPrefab.gameObject.name + dataIndex; //设置名称
                cbRender.rectTransform.localPosition = Vector3.zero;
                cbRender.rectTransform.anchoredPosition = GetRenderPos(dataIndex, Vector2.zero); //设置位置
                cbRender.rectTransform.localScale = GetRenderScale(cellRender); //设置缩放
                cbRender.gameObject.SetActive(true); //显示
            }
            drawIndex++;
            dataIndex++;
            if (dataIndex >= DataCount)
            {
                dataIndex = 0;
            }
        }
    }

    protected override void OnInit()
    {
        base.OnInit();
        Columns = mode == Mode.Horizontal ? 0 : 1;
        Vector2 maskSize = mask.rect.size;

        content.pivot = Vector2.one * 0.5f; //中心点
        content.anchorMin = Vector2.zero;
        content.anchorMax = Vector2.one;
        content.anchoredPosition = Vector2.zero; //在中心点
        content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maskSize.x);
        content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, maskSize.y);

        viewport.pivot = viewport.anchorMin = viewport.anchorMax = Vector2.one * 0.5f; //中心点
        viewport.anchoredPosition = Vector2.zero; //在中心点
        viewport.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maskSize.x);
        viewport.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, maskSize.y);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        onChange = null;
    }

    protected override Vector2 GetRenderPos(int index, Vector2 offset)
    {
        Vector2 pos = Vector2.zero;
        if (mode == Mode.Horizontal)
        {
            float singleLength = ItemWidth + pad.x;
            int gap = GetRenderGap(index);
            if (content.anchoredPosition.x >= 0)
            {
                pos.x = (-_scrollNum + gap) * singleLength; //当前中心坐标
            }
            else
            {
                pos.x = (_scrollNum + gap) * singleLength; //当前中心坐标
            }
        }
        else
        {
            float singleLength = ItemHeight + pad.y;
            int gap = GetRenderGap(index);
            if (content.anchoredPosition.y <= 0)
            {
                pos.y = (-_scrollNum + gap) * singleLength; //当前中心坐标
            }
            else
            {
                pos.y = (_scrollNum + gap) * singleLength; //当前中心坐标
            }
        }
        return pos;
    }

    protected virtual Vector3 GetRenderScale(BaseRender render)
    {
        if (lerpScale < 1)
        {
            Vector2 pos = render.SelfTransform.position;
            pos = mask.transform.InverseTransformPoint(pos);
            float t;
            if (mode == Mode.Horizontal)
            {
                float halfWidth = (ItemWidth + pad.x) * 0.5f;
                pos.x = Math.Abs(Mathf.Clamp(pos.x, -halfWidth, halfWidth));
                t = 1 - pos.x / halfWidth;
            }
            else
            {
                float halfHeight = (ItemHeight + pad.y) * 0.5f;
                pos.y = Math.Abs(Mathf.Clamp(pos.y, -halfHeight, halfHeight));
                t = 1 - pos.y / halfHeight;
            }
            return Vector3.Lerp(Vector3.one * lerpScale, Vector3.one, t);
        }
        return Vector3.one;
    }

    private int GetRenderGap(int index)
    {
        if (index == _centerIndex) return 0;
        int leftGap = int.MaxValue;
        int rightGap = int.MaxValue;
        int half = DataCount / 2;
        for (int i = 1; i <= half; i++)
        {
            if (leftGap == int.MaxValue) //向左找
            {
                int nowLeft = _centerIndex - i;
                if (nowLeft < 0) nowLeft = DataCount + nowLeft;
                if (nowLeft == index)
                {
                    leftGap = i;
                }
            }
            if (rightGap == int.MaxValue) //向右找
            {
                int nowRight = _centerIndex + i;
                if (nowRight >= DataCount) nowRight = nowRight - DataCount;
                if (nowRight == index)
                {
                    rightGap = i;
                }
            }
            if (leftGap != int.MaxValue && rightGap != int.MaxValue) break;
        }
        if (leftGap != int.MaxValue || rightGap != int.MaxValue)
        {
            if (leftGap != int.MaxValue)
            {
                if (rightGap != int.MaxValue)
                {
                    if (leftGap < rightGap)
                    {
                        return -leftGap;
                    }
                    else
                    {
                        return rightGap;
                    }
                }
                else
                {
                    return -leftGap;
                }
            }
            else if (rightGap != int.MaxValue)
            {
                return rightGap;
            }
        }
        return int.MaxValue;
    }

    /// <summary>
    /// 获取当前渲染的[左边那个Index].
    /// </summary>
    /// <returns></returns>
    protected override int GetDrawRendererStartIndex()
    {
        //根据content当前位置计算
        Vector2 pos = content.anchoredPosition;

        int index;
        int halfCount = Mathf.Max(Mathf.FloorToInt((DrawCount - 1) * 0.5f), 0); //上/左半边有多少个？

        if (mode == Mode.Horizontal)
        {
            float length = ItemWidth + pad.x;
            _scrollNum = Mathf.FloorToInt((Math.Abs(pos.x) + length * 0.5f) / length); //向左/右滑动的数量
            _centerIndex = _scrollNum % DataCount; //求出当前处于中间index的索引

            if (_centerIndex > 0 && pos.x > 0) //反向取索引
            {
                _centerIndex = DataCount - _centerIndex;
            }
        }
        else
        {
            float length = ItemHeight + pad.y;
            _scrollNum = Mathf.FloorToInt((Math.Abs(pos.y) + length * 0.5f) / length); //向左/右滑动的数量
            _centerIndex = _scrollNum % DataCount; //求出当前处于中间index的索引

            if (_centerIndex > 0 && pos.y < 0) //反向取索引
            {
                _centerIndex = DataCount - _centerIndex;
            }
        }
        //求出最上/左的那个索引
        index = _centerIndex - halfCount;
        if (index < 0)
        {
            index = DataCount + index;
        }
        return ClampIndex(index);
    }

    /// <summary>
    /// 面积发生改变
    /// </summary>
    public override void OnDimensionsChange()
    {
        base.OnDimensionsChange();
        ContentChanged(true);
    }

    protected override void OnContentChange()
    {
        contentChanged = false;
        //限定滚动区域
        Vector2 maskSize = mask.rect.size;

        content.pivot = Vector2.one * 0.5f; //中心点
        content.anchorMin = Vector2.zero;
        content.anchorMax = Vector2.one;

        if (DataCount == 0 || DataCount >= maxShowNum) //开启循环模式
        {
            movementType = MovementType.Unrestricted;
            content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maskSize.x);
            content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, maskSize.y);

            viewport.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, ItemWidth + pad.x);
            viewport.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ItemHeight + pad.y);
        }
        else //限制模式
        {
            movementType = MovementType.Clamped;
            int half = DrawCount / 2;

            if (mode == Mode.Horizontal)
            {
                content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, ItemWidth + half * (ItemWidth + pad.x) * 2);

                float gapXmin = -(DataCount - 1) * (ItemWidth + pad.x) - ItemWidth * 0.5f;
                viewport.offsetMin = new Vector2(gapXmin, -maskSize.y * 0.5f);
                float gapXmax = ItemWidth * 0.5f;
                viewport.offsetMax = new Vector2(gapXmax, maskSize.y * 0.5f);
            }
            else
            {
                content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ItemHeight + half * (ItemHeight + pad.y) * 2);

                float gapYmin = (DataCount - 1) * (ItemHeight + pad.y) + ItemHeight * 0.5f;
                viewport.offsetMin = new Vector2(-maskSize.x * 0.5f, gapYmin);
                float gapYmax = -ItemHeight * 0.5f;
                viewport.offsetMax = new Vector2(maskSize.x * 0.5f, gapYmax);
            }
        }
    }

    /// <summary>
    /// 计算最大显示数量
    /// </summary>
    protected override void CalculateMaxShowNum()
    {
        int preShowNum = maxShowNum;
        Vector2 size = mask.rect.size;
        if (mode == Mode.Horizontal) //一行
        {
            maxShowNum = Mathf.CeilToInt((size.x - ItemWidth) * 0.5f / (ItemWidth + pad.x)) * 2 + 1;
        }
        else //一列
        {
            maxShowNum = Mathf.CeilToInt((size.x - ItemHeight) * 0.5f / (ItemHeight + pad.y)) * 2 + 1;
        }
        if (preShowNum != maxShowNum)
        {
            DataChanged();
        }
    }

    /// <summary>
    /// 获取实际内容的边框盒大小
    /// </summary>
    /// <returns></returns>
    protected override Bounds GetBounds()
    {
        if (contentChanged)
        {
            Vector2 center = Vector2.zero;
            Vector2 maskSize = mask.rect.size;
            int half = DrawCount / 2;

            if (mode == Mode.Horizontal)
            {
                maskSize.x = ItemWidth + half * (ItemWidth + pad.x) * 2;
            }
            else
            {
                maskSize.y = ItemHeight + half * (ItemHeight + pad.y) * 2;
            }
            //计算滚动
            int add = _scrollNum - _expandNum;
            if (add != 0) //每增加两个则边界拓展
            {
                maskSize += GapPos * 2 * add;
                _expandNum = _scrollNum;
            }
            mBounds = new Bounds(center, maskSize);
        }
        return mBounds;
    }
}