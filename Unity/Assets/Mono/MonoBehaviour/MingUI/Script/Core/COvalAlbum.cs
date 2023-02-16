using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;


public class COvalAlbum : UIBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
    public class COvalAlbumVO {
        public int index;
        public float angle;
    }

    //Lua和渲染相关
    [NonSerialized]
    public Type itemRender = typeof(CardRender); //c#层的render,必须是ListItemRender或者其子类
    [NonSerialized]
    public Type selItemRender = typeof(COvalAlbumRender); //c#层的render,必须是ListItemRender或者其子类
    public CBaseRender renderPrefab; //子对象渲染预设
    public CBaseRender selRenderPrefab;//Sel子对象渲染预设


    //UI操作相关
    public bool isAutoMove = false;
    public bool isAutoForward = true;
    public float autoMoveTimeScale = 50.0f;
    public float dragMoveSpeed = 2.0f;

    //拖动数据
    private Vector2 touchDragDelta;

    //预设相关
    public RectTransform parent;

    //列表结构    
    public float autoSelTime = 1.0f;
    public string[] testData;

    private int firstIndex;
    private int lastIndex;
    private int selIndex = -1;
    private bool isDuringAutoSel;
    private float autoSelBeginTime;
    private float autoSelSpeedScale;
    private COvalAlbumVO selVO;
    private COvalAlbumRender selItem;
    private List<object> dataProvider;
    private Dictionary<CardRender, COvalAlbumVO> itemDic;

    //临时变量，用于计算
    private float limitAngleForward;
    private float limitAngleBackward;
    private float limitAngleHalfward;
    private float limitAngleTransfer;
    private float limitAngleUnitfactor;
    private float limitAngleForwardFactor;
    private float limitAngleBackwardFactor;
    private float deltaAngle;
    private float curAngle;
    private float factor;
    private float moveDelta;
    private Vector2 nextPos;

    //运动轨迹相关
    public float gapAngle = 20.0f;
    public float ovalHalfWidth = 800.0f;
    public float ovalHalfHeigth = 400.0f;

    protected override void Start() {
        dataProvider = new List<object>();
        itemDic = new Dictionary<CardRender, COvalAlbumVO>();
        touchDragDelta = Vector2.zero;

        InitTestData();
        InitSelItem();
        InitShowItem();
        UpdateShowPos();
        UpdateShowDatas();
    }

    void Update() {
        UpdateShowPos();
    }

    public void OnBeginDrag(PointerEventData eventData) {

    }

    public void OnDrag(PointerEventData data) {
        touchDragDelta = data.delta;
    }

    public void OnEndDrag(PointerEventData eventData) {
        touchDragDelta = Vector2.zero;
    }

   
    //重新设置数据
    public void SetDataProvider(List<object> datas) {
        //dataprovider
        dataProvider.Clear();
        for (int i = 0; i > datas.Count; i++) {
            dataProvider.Add(datas[i]);
        }
        //items
        InitShowItem();
        int index = 0;
        var dic = itemDic.GetEnumerator();
        while (dic.MoveNext()) {
            CardRender item = dic.Current.Key;
            item.SelfTransform.gameObject.SetActive(true);
            item.SelfTransform.anchoredPosition = Vector3.zero;
            item.SelfTransform.SetSiblingIndex(itemDic.Count - 1 - index);
            dic.Current.Value.index = index;
            dic.Current.Value.angle = 90 + gapAngle - gapAngle * index;
            index++;
        }
        firstIndex = 0;
        lastIndex = itemDic.Count - 1;
        //SelItem
        InitSelItem();
        selItem.SelfTransform.SetAsLastSibling();
        selVO.index = -1;
        selVO.angle = 0;
        //update
        UpdateShowPos();
        UpdateShowDatas();
    }


    #region //内部接口

    private int SelIndex {
        set {
            if (value != selIndex && isDuringAutoSel == false) {
                selIndex = value;
                autoSelBeginTime = Time.time;
                isDuringAutoSel = true;
            }
        }
    }

    private void InitTestData() {
        foreach (string str in testData) {
            dataProvider.Add(str);
        }
    }

    private void InitShowItem() {
        if (itemDic == null || itemDic.Count == 0) {
            float showingAngle = 90 + gapAngle * 2;
            int floorCount = Mathf.FloorToInt(showingAngle / gapAngle);
            int count = floorCount + 1;
            for (int i = 0; i < count; i++) {
                CardRender item = CreateItemRender(true);
                item.SelfTransform.gameObject.SetActive(true);
                item.SelfTransform.anchoredPosition = Vector3.zero;
                item.SelfTransform.SetSiblingIndex(count - 1 - i);
                COvalAlbumVO vo = new COvalAlbumVO();
                vo.index = i;
                vo.angle = 90 + gapAngle - gapAngle * i;
                itemDic.Add(item, vo);
            }
            firstIndex = 0;
            lastIndex = count - 1;
            limitAngleForward = 90 + gapAngle * 2;
            limitAngleBackward = 360 + 90 - gapAngle * floorCount;
            limitAngleHalfward = (limitAngleForward + limitAngleBackward) / 2.0f;
            limitAngleTransfer = limitAngleBackward - (90 + gapAngle);
            limitAngleUnitfactor = gapAngle / 360.0f;
            limitAngleForwardFactor = 0.25f + limitAngleUnitfactor;
            limitAngleBackwardFactor = 1.0f - limitAngleUnitfactor;
        }
    }

    private void InitSelItem() {
        if (selItem == null) {
            selItem = CreateSelItemRender();
            selItem.SelfTransform.gameObject.SetActive(true);
            selItem.SelfTransform.anchoredPosition = new Vector2(-603, 0);
            selItem.SelfTransform.SetAsLastSibling();
            selVO = new COvalAlbumVO();
            selVO.index = -1;
            selVO.angle = 0;
        }
    }

    private void UpdateShowPos() {
        if (isDuringAutoSel) {
            if (Time.time - autoSelBeginTime >= autoSelTime) {
                isDuringAutoSel = false;
                UpdateSelItem(selIndex);
            } else {
                deltaAngle = Time.deltaTime * autoSelSpeedScale;
            }
        } else {
            if (isAutoMove) {
                deltaAngle = Time.deltaTime * autoMoveTimeScale * (isAutoForward ? 1 : -1);
            } else {
                //TODO 限制delta的大小，避免卡帧的时候出现越界
                moveDelta = -touchDragDelta.x;
                deltaAngle = moveDelta * Mathf.Deg2Rad * dragMoveSpeed;
            }
        }
        //TODO 限制deltaAngle的增量，这里也就是deltaAngle的大小

        //TODO 越界有拖动不了的问题，在于firstIndex和最接近的一个的显示alpha不完全同步
        if (deltaAngle < 0 && firstIndex == 0) {
            //正向越界
        } else if (deltaAngle > 0 && firstIndex == dataProvider.Count - 1) {
            //反向越界
        } else {
            if (itemDic != null) {
                var dic = itemDic.GetEnumerator();
                while (dic.MoveNext()) {
                    //数据更新
                    dic.Current.Value.angle += deltaAngle;
                    ClipAngle(ref dic.Current.Value.angle);
                    curAngle = dic.Current.Value.angle;
                    factor = dic.Current.Value.angle / 360.0f;

                    //循环更新item的angle
                    if (curAngle >= limitAngleForward && curAngle < limitAngleHalfward) {
                        dic.Current.Value.angle += limitAngleTransfer;
                        dic.Current.Key.SelfTransform.SetAsFirstSibling();
                        //序列更新
                        lastIndex += 1;
                        firstIndex += 1;
                        dic.Current.Value.index = lastIndex;
                        if (lastIndex < dataProvider.Count && lastIndex >= 0) {
                            dic.Current.Key.SetData(dataProvider[lastIndex]);
                        }
                    } else if (curAngle <= limitAngleBackward && curAngle > limitAngleHalfward) {
                        dic.Current.Value.angle -= limitAngleTransfer;
                        dic.Current.Key.SelfTransform.SetAsLastSibling();
                        //序列更新
                        lastIndex -= 1;
                        firstIndex -= 1;
                        dic.Current.Value.index = firstIndex;
                        if (firstIndex >= 0 && firstIndex < dataProvider.Count) {
                            dic.Current.Key.SetData(dataProvider[firstIndex]);
                        }
                    }

                    //限制显示，无数据的就不显示了
                    if (dic.Current.Value.index < 0 || dic.Current.Value.index >= dataProvider.Count) {
                        dic.Current.Key.SetAlpha(0.0f);
                    } else {
                        UpdateItem(dic.Current.Key, factor, curAngle);
                    }
                }
                //更新当前选中
                //TODO 调整更新流程
                if (firstIndex >= 0 && firstIndex < dataProvider.Count) {
                    UpdateSelItem(firstIndex);
                }
            }
        }
        moveDelta = 0.0f;
    }

    private void UpdateShowDatas() {
        if (itemDic != null) {
            var dic = itemDic.GetEnumerator();
            while (dic.MoveNext()) {
                if (dataProvider.Count > dic.Current.Value.index)
                    dic.Current.Key.SetData(dataProvider[dic.Current.Value.index]);
            }
        }
    }

    private void UpdateSelItem(int index) {
        if (selItem != null) {
            if (selVO.index != index) {
                selVO.index = index;
                selItem.SelfTransform.SetAsLastSibling();
                if (dataProvider.Count > index)
                    selItem.SetData(dataProvider[index]);
            }
        }
    }

    private void UpdateItem(CardRender item, float factor, float angle) {
        nextPos.x = ovalHalfWidth * Mathf.Cos(angle * Mathf.Deg2Rad);
        nextPos.y = -ovalHalfHeigth * Mathf.Sin(angle * Mathf.Deg2Rad);
        item.SetPos(nextPos);
        if (factor > 0 && factor < 0.8) {
            item.SetScale(0.2f + 0.8f * angle / 360);
        } else {
            item.SetScale(0.2f);
        }
        if (factor >= 0 && factor <= 0.25f) {
            item.SetAlpha(1.0f);
        } else if (factor > 0.25f && factor < limitAngleForwardFactor) {
            item.SetAlpha((limitAngleForwardFactor - factor) / limitAngleUnitfactor);
        } else if (factor > limitAngleBackwardFactor) {
            item.SetAlpha((factor - limitAngleBackwardFactor) / limitAngleUnitfactor);
        } else {
            item.SetAlpha(0.0f);
        }
    }

    public void OnClickItem(object obj) {
        //旧一套的数据识别
        //int index = dataProvider.IndexOf(obj);
        //if (index != -1) {
        //    var dic = itemDic.GetEnumerator();
        //    while (dic.MoveNext()) {
        //        if (dic.Current.Value.index == index) {
        //            SelIndex = index;
        //            autoSelSpeedScale = 90 + gapAngle - dic.Current.Value.angle % 360;
        //            autoSelSpeedScale *= 1 / autoSelTime;
        //            break;
        //        }
        //    }
        //}

        //新一套使用事件
        if (obj != null && obj is PointerEventData) {
            GameObject selObj = (obj as PointerEventData).pointerPress;
            if (selObj != null) {
                var dic = itemDic.GetEnumerator();
                while (dic.MoveNext()) {
                    if (dic.Current.Key.SelfTransform == selObj.transform) {
                        SelIndex = dic.Current.Value.index;
                        autoSelSpeedScale = 90 + gapAngle - dic.Current.Value.angle % 360;
                        autoSelSpeedScale *= 1 / autoSelTime;
                        break;
                    }
                }
            }
        }
    }

    //保持区间，方便计算
    private void ClipAngle(ref float angle) {
        if (angle < 0) {
            angle = 360 + angle;
        }
        angle = angle % 360;
    }

    /// <summary>
    /// 创建ItemRender(c#层)
    /// </summary>
    /// <returns></returns>
    protected virtual CardRender CreateItemRender(bool isAddClick = false) {
        CardRender render = Activator.CreateInstance(itemRender) as CardRender;
        if (render != null) {
            GameObject renderObj = Instantiate(renderPrefab.gameObject);
            CBaseRender cbRender = renderObj.GetComponent<CBaseRender>();

            if (isAddClick) {
                cbRender.AddClick(OnClickItem);
            }

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

    protected virtual COvalAlbumRender CreateSelItemRender() {
        COvalAlbumRender render = Activator.CreateInstance(selItemRender) as COvalAlbumRender;
        if (render != null) {
            GameObject renderObj = Instantiate(selRenderPrefab.gameObject);
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
    #endregion
}
