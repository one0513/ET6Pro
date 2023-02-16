using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;


public class CLadderList : UIBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

    [NonSerialized]
    public Type itemRender = typeof(CardRender); //c#层的render,必须是ListItemRender或者其子类
    public CBaseRender renderPrefab; //子对象渲染预设


    //阶梯移动计算数据
    public int itemHeight = 400;
    public float minScale = 0.5f;
    public float facScale = 0.5f;
    public float minSpeed = 0.2f;
    public float facSpeed = 1.8f;
    public float matchPosY = 1080.0f;//注意这个值影响facotr的计算（暂时设定成为默认显示最后一个值，而且这个值的位置恰好在最下面）
    public float deltaSpeed = 1.0f;//和计算精度有关
    private List<float> posData;

    //拖动数据
    private Vector2 touchDragDelta;

    //UI预设
    public RectTransform parent;

    //列表基础结构
    public string[] testData;
    private int firstIndex;
    private int lastIndex;
    private List<int> dataIndexList;
    private List<object> dataProvider;//原始data需要进行倒叙处理
    private List<CardRender> itemList;

    //临时变量，方便计算
    private float factor;
    private float curDelta;
    private float crossFactor;
    private bool isNeedPartMove;
    private bool isLimitMove;
    private Vector2 newPos;
    private Vector2 addPos;
    private Vector2 dragDelta;
    private Vector2 dragPartDelta;

    // Use this for initialization
    protected override void Awake() {
        dataProvider = new List<object>();
        dataIndexList = new List<int>();
        itemList = new List<CardRender>();

        touchDragDelta = Vector2.zero;
        newPos = Vector2.zero;
        addPos = Vector2.zero;
        dragDelta = Vector2.zero;
        dragPartDelta = Vector2.zero;

        InitTestData();
        InitPosData();
        //InitShowItem();
        //InitShowDatas();
    }

    // Update is called once per frame
    void Update() {
        //TODO 限制delta的大小，避免卡帧的时候出现越界
        dragDelta.y = touchDragDelta.y;
        UpdateShowPos();
    }

    protected override void OnDestroy() {
        base.OnDestroy();

    }

    public void OnBeginDrag(PointerEventData eventData) {

    }

    public void OnDrag(PointerEventData data) {
        touchDragDelta = data.delta;
    }

    public void OnEndDrag(PointerEventData eventData) {
        touchDragDelta = Vector2.zero;
    }

    #region 外部接口
    public void SetDataProvider(List<object> datas) {
        dataProvider.Clear();
        for (int i = 0; i < datas.Count; i++) {
            dataProvider.Add(datas[i]);
        }
        dataProvider.Reverse();

        InitShowItem();
        for (int i = 0; i < itemList.Count; i++) {
            itemList[i].SetPos(new Vector2(0, posData[i + 1]));
            itemList[i].SetAlpha(1.0f);
            dataIndexList[i] = i;
        }
        InitShowDatas();
    }

  

    #endregion

    #region 内部接口
    private void InitShowItem() {
        if (itemList == null || itemList.Count == 0) {
            for (int i = 0; i < posData.Count - 1; i++) {
                CardRender item = CreateItemRender();
                item.SelfTransform.gameObject.SetActive(true);
                item.SetPos(new Vector2(0, posData[i + 1]));
                itemList.Add(item);
                dataIndexList.Add(i);
            }
        }
    }

    private void InitShowDatas() {
        if (itemList != null) {
            for (int i = 0; i < itemList.Count; i++) {
                int dataIndex = dataIndexList[i];
                itemList[i].SelfTransform.SetSiblingIndex(itemList.Count - 1 - i);
                if (dataIndex >= 0 && dataIndex < dataProvider.Count) {
                    itemList[i].SetData(dataProvider[dataIndex]);
                }
            }
        }
        firstIndex = 0;
        lastIndex = itemList.Count - 1;
    }

    private void UpdateShowPos() {
        if (itemList != null) {
            isNeedPartMove = false;
            isLimitMove = false;
            if (dragDelta.y != 0f) {
                for (int i = 0; i < itemList.Count; i++) {
                    factor = -itemList[i].PosY / matchPosY;//（暂时设定成为默认显示最后一个值，而且这个值的位置恰好在最下面，也就是factor = 1）
                    addPos.y = (dragDelta * (minSpeed + facSpeed * factor)).y;
                    //限制拖动范围
                    if (dataIndexList[i] == 0) {
                        isLimitMove = itemList[i].PosY + addPos.y >= posData[1] / 2.0;
                    } else if (dataIndexList[i] == dataProvider.Count - 1) {
                        isLimitMove = itemList[i].PosY + addPos.y <= posData[1];
                    }
                    if (addPos.y == 0) {
                        continue;
                    }
                    //判断是否需要分批移动
                    if (itemList[i].PosY + addPos.y <= posData[0]) {
                        crossFactor = (itemList[i].PosY + addPos.y - posData[0]) / addPos.y;
                        dragPartDelta.y = dragDelta.y * crossFactor;
                        dragDelta.y = dragDelta.y * (1 - crossFactor);
                        isNeedPartMove = true;
                        break;
                    } else if (itemList[i].PosY + addPos.y >= posData[posData.Count - 1]) {
                        crossFactor = (itemList[i].PosY + addPos.y - posData[posData.Count - 1]) / addPos.y;
                        dragPartDelta.y = dragDelta.y * crossFactor;
                        dragDelta.y = dragDelta.y * (1 - crossFactor);
                        isNeedPartMove = true;
                        break;
                    }
                }
            }
            if (isLimitMove == false) {
                //使用dragDelta
                for (int j = 0; j < itemList.Count; j++) {
                    UpdateItem(itemList[j], dragDelta, j, isNeedPartMove);
                }
                //如果需要循环，则进行分批移动
                if (isNeedPartMove) {
                    for (int k = 0; k < itemList.Count; k++) {
                        UpdateItem(itemList[k], dragPartDelta, k);
                    }
                }
            }
        }
        dragDelta.y = 0;
    }

    private void UpdateItem(CardRender item, Vector2 drag, int dataIndex, bool isNeedMove = false) {
        factor = -item.PosY / matchPosY;//（暂时设定成为默认显示最后一个值，而且这个值的位置恰好在最下面，也就是factor = 1）
        addPos.y = (drag * (minSpeed + facSpeed * factor)).y;
        if (isNeedMove) {
            if (item.PosY + addPos.y <= posData[0]) {
                newPos.y = posData[posData.Count - 1];
                item.SetPos(newPos);
                factor = -posData[posData.Count - 1] / matchPosY;
                addPos.y = 0;

                item.SelfTransform.SetAsFirstSibling();
                lastIndex++;
                firstIndex++;
                if (lastIndex >= 0 && lastIndex < dataProvider.Count) {
                    item.SetData(dataProvider[lastIndex]);
                    dataIndexList[dataIndex] = lastIndex;
                } else {
                    dataIndexList[dataIndex] = -1;
                }
            } else if (item.PosY + addPos.y >= posData[posData.Count - 1]) {
                newPos.y = posData[0];
                item.SetPos(newPos);
                factor = -posData[0] / matchPosY;
                addPos.y = 0;

                item.SelfTransform.SetAsLastSibling();
                lastIndex--;
                firstIndex--;
                if (firstIndex >= 0 && firstIndex < dataProvider.Count) {
                    item.SetData(dataProvider[firstIndex]);
                    dataIndexList[dataIndex] = firstIndex;
                } else {
                    dataIndexList[dataIndex] = -1;
                }
            }
        }
        item.AddPos(addPos);
        item.SetScale(minScale + facScale * factor);
        item.SetAlpha(dataIndexList[dataIndex] == -1 ? 0.0f : 1.0f);
    }

    /// <summary>
    /// 创建ItemRender(c#层)
    /// </summary>
    /// <returns></returns>
    protected virtual CardRender CreateItemRender() {
        CardRender render = Activator.CreateInstance(itemRender) as CardRender;
        if (render != null) {
            GameObject renderObj = Instantiate(renderPrefab.gameObject);
            CBaseRender cbRender = renderObj.GetComponent<CBaseRender>();

            renderObj.transform.parent = parent;
            //AddChild(cbRender.gameObject, Vector3.zero);

           
            render.Render = cbRender;
            //if (managerRenderSelect) {
            //    cbRender.AddClick(OnRenderClick);
            //}
            //renderObj.transform.localScale = Vector3.one * itemScale;
            return render;
        }
        return null;
    }

    #region 辅助用，后期再考虑处理

    private void InitTestData() {
        foreach (string str in testData) {
            dataProvider.Add(str);
        }
        dataProvider.Reverse();
    }

    //注意UpdateItemPos()的计算公式，根据matchPosY这个特殊位置进行计算；（matchPosY这个位置的Item和下一个Item刚好衔接无重合）
    //Item的pivot在Bottom的位置，方便计算最底边的时候的数值
    public void InitPosData() {
        float deltaVal = 0;
        float nextPosY = (matchPosY * matchPosY + itemHeight * minScale * matchPosY) / (matchPosY - itemHeight * facScale);
        float deltaCount = nextPosY - matchPosY;
        float curPosy = matchPosY;
        while (deltaCount > 0) {
            curDelta = minSpeed + facSpeed * curPosy / matchPosY;
            curPosy = curPosy + curDelta * deltaSpeed;
            deltaCount -= curDelta * deltaSpeed;
            deltaVal += deltaSpeed;
            //deltaVal++;//先注释，可能和精度有关
        }
        Debug.Log("count is " + deltaVal);
        float indexPosy = matchPosY;
        posData = new List<float>();
        posData.Add(nextPosY * -1);
        posData.Add(indexPosy * -1);
        while (indexPosy > 0) {
            indexPosy = MatchIndexPosY(deltaVal, indexPosy);
            posData.Add(indexPosy * -1);
        }
        //foreach (float posy in posData) {
        //    Debug.Log("pos is " + posy);
        //}
    }

    private float MatchIndexPosY(float deltaVal, float curPosY) {
        while (deltaVal > 0) {
            curDelta = minSpeed + facSpeed * curPosY / matchPosY;
            curPosY = curPosY - curDelta * deltaSpeed;
            deltaVal -= deltaSpeed;
            //deltaVal--;
        }
        return curPosY;
    }

    #endregion
    #endregion
}
