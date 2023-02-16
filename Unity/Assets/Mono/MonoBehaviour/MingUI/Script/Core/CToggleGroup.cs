using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 开关组(只控制排版和开关组，不要掺杂其他逻辑！)
/// </summary>
[AddComponentMenu("MingUI/CToggleGroup", 28)]
[RequireComponent(typeof(RectTransform)), DisallowMultipleComponent()]
public class CToggleGroup : ToggleGroup {
    public int columns = 0; //0为不限列数,即一行
    public Vector2 pad = Vector2.zero; //间距
    public List<Toggle> toggleList = new List<Toggle>(); //列表
    public bool autoLayout = true;//自动对齐
    [SerializeField]
    private Toggle toggleRender; //预设

    private Vector2 pivot;
    private int _selectIndex = -1;
    private bool _ignoreCallBack;
    private UIEventListener.IntDelegate _onSelect;
    private UIEventListener.IntDelegate _onDoubleSelect;

    private RectTransform _toggleRenderTransfrm;

    /// <summary>
    /// 设置开关点击音效
    /// </summary>
    public string TabSound {
        set {
            int count = toggleList.Count;
            for (int i = 0; i < count; i++) {
                CTabBar bar = toggleList[i] as CTabBar;
                if (bar != null) {
                    bar.Sound = value;
                }
            }
        }
    }

    public void RefreshToggleList() {
        toggleList.Clear();
        var len = transform.childCount;
        for (var i = 0; i < len; i++) {
            var toggle = transform.GetChild(i).GetComponent<Toggle>();
            toggle.group = this;
            toggleList.Add(toggle);
        }
    }

    protected RectTransform ToggleRenderTransfrm {
        get {
            if (_toggleRenderTransfrm == null) {
                _toggleRenderTransfrm = toggleRender.GetComponent<RectTransform>();
            }
            return _toggleRenderTransfrm;
        }
    }

    public Toggle ToggleRender {
        get { return toggleRender; }
        set
        {
            if (toggleRender != null && toggleRender == value)return;
            toggleRender = value;
            UpdateToggleRenderTransfrm();
        }
    }

    private void UpdateToggleRenderTransfrm() {
        _toggleRenderTransfrm = toggleRender.GetComponent<RectTransform>();
        pivot = ToggleRenderTransfrm.pivot;
    }
    /// <summary>
    /// 设置选中：改变后会派发选中的事件
    /// </summary>
    public int SelectIndex {
        get { return _selectIndex; }
        set {
            if (allowSwitchOff || _selectIndex != value) {
                var changed = _selectIndex != value;
                if (changed && _selectIndex >= 0 && _selectIndex < toggleList.Count) {
                    toggleList[_selectIndex].isOn = false;
                }
                _selectIndex = value;
                if (_selectIndex >= 0 && _selectIndex < toggleList.Count) {
                    if (changed) {
                        toggleList[_selectIndex].isOn = true;
                    }
                    if (_ignoreCallBack == false) {
                        OnSelectChange();
                    }
                }
            } else {
                if (_onDoubleSelect != null && _ignoreCallBack == false) {
                    _onDoubleSelect(_selectIndex);
                }
            }
        }
    }

    public void UnSelectAll() {
        foreach (var tog in toggleList) {
            tog.isOn = false;
        }
    }

    /// <summary>
    /// 设置选中，但不派发事件
    /// </summary>
    /// <param name="index"></param>
    public void SetSelect(int index) {
        _ignoreCallBack = true;
        SelectIndex = index;
        _ignoreCallBack = false;
    }

    /// <summary>
    /// 添加选中的回调
    /// </summary>
    /// <param name="selectFun"></param>
    public void AddSelect(UIEventListener.IntDelegate selectFun) {
        _onSelect = selectFun;
    }

    public void RemoveSelect() {
        _onSelect = null;
    }

    /// <summary>
    /// 添加选中的回调
    /// </summary>
    /// <param name="selectFun"></param>
    public void AddDoubleSelect(UIEventListener.IntDelegate selectFun) {
        _onDoubleSelect = selectFun;
    }

    /// <summary>
    /// 删除选中回调
    /// </summary>
    public void RemoveDoubleSelect() {
        _onDoubleSelect = null;
    }

    /// <summary>
    /// 获取toggle
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public Toggle this[int index] {
        get { return toggleList[index]; }
    }

    public virtual Toggle Add() {
        return Add(-1);
    }

    public virtual Toggle Add(int index, string label) {
        Toggle toggle = Add(index);
        if (toggle is CCheckBox) {
            (toggle as CCheckBox).Text = label;
        }
        return toggle;
    }

    public virtual void SetLabel(int index, string label) {
        if (index >= 0 && index < toggleList.Count) {
            Toggle toggle = this[index];
            if (toggle is CCheckBox) {
                (toggle as CCheckBox).Text = label;
            }
        }
    }

    /// <summary>
    /// 插入toggle
    /// </summary>
    /// <param name="index">要插入的索引</param>
    /// <returns></returns>
    public virtual Toggle Add(int index) {
        GameObject go = Instantiate(toggleRender.gameObject);

        Toggle toggle = go.GetComponent<Toggle>();
        toggle.group = this;
        if (Application.isPlaying) {
            toggle.onValueChanged.AddListener(OnToggleChange);
        }
        if (index > 0) {
            toggleList.Insert(index, toggle);
        } else //往后面补充
        {
            toggleList.Add(toggle);
        }
        go.transform.SetParent(transform);
        go.transform.localScale = Vector3.one;
        go.transform.localPosition = Vector3.zero;
        go.transform.localEulerAngles = Vector3.zero;
        go.name = toggleRender.name + toggleList.IndexOf(toggle);
        RectTransform rt = toggle.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0, 1); //调整为左上角对齐,为了好对齐
        rt.pivot = new Vector2(0.5f, 0.5f);
        Reposition();
        if (Application.isPlaying)AddBarInfoList(index, toggle);
        return toggle;
    }

    public void RemoveAll() {
        foreach (Toggle toggle in toggleList) {
            if (toggle) {
                if (MingUIAgent.IsEditorMode) {
                    DestroyImmediate(toggle.gameObject);
                } else {
                    Destroy(toggle.gameObject);
                }
            }
        }
        toggleList.Clear();
    }

    /// <summary>
    /// 移除toggle
    /// </summary>
    /// <param name="index">要移除的索引</param>
    public virtual void Remove(int index) {
        if (index >= 0 && index < toggleList.Count) {
            Toggle toggle = toggleList[index];
            toggleList.RemoveAt(index);
            if (toggle != null) {
                toggle.onValueChanged.RemoveAllListeners();
                if (MingUIAgent.IsEditorMode) {
                    DestroyImmediate(toggle.gameObject);
                } else {
                    Destroy(toggle.gameObject);
                }
                Reposition();
            }
        }
    }

    /// <summary>
    /// 设置可见
    /// </summary>
    /// <param name="index"></param>
    /// <param name="visible"></param>
    public virtual void SetVisible(int index, bool visible) {
        Toggle toggle = toggleList[index];
        if (toggle != null && toggle.gameObject.activeSelf != visible) {
            toggle.gameObject.SetActive(visible);
            Reposition();
        }
    }

    /// <summary>
    /// 校准位置
    /// </summary>
    [ContextMenu("Reposition")]
    public virtual void Reposition() {
        if (autoLayout == false) return;
        pivot = ToggleRenderTransfrm.pivot;
        Vector2 size = ToggleRenderTransfrm.rect.size;
        
        float startX = pivot.x * size.x;
        float startY = (pivot.y - 1) * size.y;
        float orginX = startX;
        int count = toggleList.Count;
        for (int i = 0; i < count; i++) {
            if (!toggleList[i].gameObject.activeSelf) continue;

            toggleList[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(startX, startY);
            if (columns == 0)//一行
            {
                startX += size.x + pad.x;
            } else if (columns == 1)//一列
            {
                startY -= size.y + pad.y;
            } else//多列
            {
                if (i % columns == columns - 1) //换行
                {
                    startX = orginX;
                    startY -= size.y + pad.y;
                } else {
                    startX += size.x + pad.x;
                }
            }
        }
        if (Application.isPlaying)UpdateStartPos();
    }

    protected override void Awake() {
        base.Awake();

        base.SetAllTogglesOff();
        pivot = ToggleRenderTransfrm.pivot;
        int i = 0;
        foreach (Toggle toggle in toggleList) {
            toggle.onValueChanged.AddListener(OnToggleChange);
            int tempIndex = i;
            toggle.onValueChanged.AddListener((value) => { OnToggleChange(tempIndex, value); });
            i++;
        }
    }

    protected override void OnDestroy() {
        base.OnDestroy();
        toggleRender = null;
        _toggleRenderTransfrm = null;
        toggleList.Clear();
        toggleList = null;
        _onSelect = null;
        _onDoubleSelect = null;
    }

    private void OnToggleChange(bool value) {
        if (!_ignoreCallBack && value) {
            for (int i = 0; i < toggleList.Count; i++) {
                if (toggleList[i].isOn) {
                    SelectIndex = i;
                    break;
                }
            }
        }
    }

    protected virtual void OnSelectChange() {
        if (_onSelect != null) {
            _onSelect(_selectIndex);
        }
    }

    #region CToggleGroup点击时挤压效果
    public enum Axes
    {
        Horizontal,
        Vertical
    }
    //是否开启挤压效果
    public bool isShowExtrudingAffect = false;
    /// <summary>
    /// CTabBar选中时放大倍数
    /// </summary>
    public float tabBarScale = 1.1f;
    /// <summary>
    /// CTabBar缩放动画时间
    /// </summary>
    public float scaleDurationTime = 0.5f;
    /// <summary>
    /// CTabBar移动动画时间
    /// </summary>
    public float moveDurationTime = 0.5f;
    /// <summary>
    /// 缩放动效方式
    /// </summary>
    public DG.Tweening.Ease scaleEase = DG.Tweening.Ease.OutSine;
    /// <summary>
    /// 移动动效方式
    /// </summary>
    public DG.Tweening.Ease moveEase = DG.Tweening.Ease.OutSine;
    public Axes axes = Axes.Horizontal;
    private Vector2 prefabSize;
    private class BarInfo
    {
        public CTabBar tabBar;
        public Vector2 startPos;
        public Vector2 endPos;
        public float leftValue;
        public float topValue;
        public float scale;
        private Tweener tweener;

        public void DoExtrudingAffectHorizontal(float moveDurationTime, DG.Tweening.Ease scaleEase)
        {
            if (tweener != null) tweener.Kill();
            if (tabBar == null) return;
            tweener = tabBar.rectTransform.DOAnchorPosX(endPos.x, moveDurationTime).SetEase(scaleEase).SetId(tabBar.rectTransform.GetInstanceID());
        }
        public void DoExtrudingAffectVertical(float moveDurationTime, DG.Tweening.Ease scaleEase)
        {
            if (tweener != null) tweener.Kill();
            if (tabBar == null) return;
            tweener = tabBar.rectTransform.DOAnchorPosY(endPos.y, moveDurationTime).SetEase(scaleEase).SetId(tabBar.rectTransform.GetInstanceID());
        }
        public void DOCustomizeTweenScal( float scaleDurationTime, Ease scaleEase)
        {
            if (tabBar == null) return;
            if (tabBar.Effect != null && tabBar.Effect.enabled == true) tabBar.Effect.DoCustomizeTweenScal(scale, scaleDurationTime, scaleEase);
            else tabBar.rectTransform.localScale = Vector3.one*scale;
        }

        public void UpdateStartPos()
        {
            
            startPos = tabBar.rectTransform.anchoredPosition;
        }

        public void SetAnchor()
        {
            if (tabBar == null) return;
            tabBar.rectTransform.anchorMin = tabBar.rectTransform.anchorMax = new Vector2(0, 1); 
        }
    }
    private List<BarInfo> barInfoList = null;
    private int onIndex = -1;
    private BarInfo CreateBarInfo(Toggle toggle)
    {
        BarInfo barInfo = new BarInfo();
        barInfo.tabBar = toggle.transform.GetComponent<CTabBar>();
        barInfo.startPos = barInfo.tabBar.rectTransform.anchoredPosition;
        barInfo.scale = 1;
        return barInfo;
    }

    private void InitBarInfoList()
    {
        if (!isShowExtrudingAffect || barInfoList!= null) return;
        prefabSize = ToggleRenderTransfrm.rect.size;
        barInfoList = new List<BarInfo>();
        for (int i = 0; i < toggleList.Count; i++)
        {
            barInfoList.Add(CreateBarInfo(toggleList[i]));
            barInfoList[i].SetAnchor();
        }
    }
    private void UpdateStartPos()
    {
        if (barInfoList == null) return;
        for (int i = 0; i < barInfoList.Count; i++)
        {
            barInfoList[i].UpdateStartPos();
        }
    }
    private void AddBarInfoList(int index, Toggle toggle)
    {
        if (index > 0) barInfoList.Insert(index, CreateBarInfo(toggle));
        else barInfoList.Add(CreateBarInfo(toggle));
    }

    private void OnToggleChange(int index, bool value)
    {
        if (!isShowExtrudingAffect) return;
        InitBarInfoList();
        UpdateAffectData(index, value);
        UpdateEndPos();
        DoExtrudingAffect();
    }
    private void UpdateAffectData(int index, bool value)
    {
        if (value && onIndex == index)
        {
            if (barInfoList[index].tabBar.Effect != null && barInfoList[index].tabBar.Effect.enabled == true)
            {
                barInfoList[index].tabBar.Effect.DoCustomizeTweenScal(barInfoList[index].scale, scaleDurationTime, scaleEase);
            }
            else barInfoList[index].tabBar.rectTransform.localScale = Vector3.one * barInfoList[index].scale;
            return;
        }
        if (value) onIndex = index;
        else
        {
            if (onIndex == index) onIndex = -1;
        }
        barInfoList[index].scale = value ? tabBarScale : 1;

        for (int i = 0; i < barInfoList.Count; i++)
        {
            if (onIndex == -1)
            {
                if (axes == Axes.Horizontal) barInfoList[i].leftValue = 0;
                else barInfoList[i].topValue = 0;
                continue;
            }
            if (axes == Axes.Horizontal)
            {
                if (i < onIndex) barInfoList[i].leftValue = (tabBarScale - 1) * prefabSize.x * (pivot.x - 1);
                else if (i == onIndex) barInfoList[i].leftValue = 0;
                else barInfoList[i].leftValue = (tabBarScale - 1) * prefabSize.x * (1-pivot.x);
            }
            else
            {
                if (i < onIndex) barInfoList[i].topValue = (tabBarScale - 1) * prefabSize.y * pivot.x ;
                else if (i == onIndex) barInfoList[i].topValue = 0;
                else barInfoList[i].topValue = (tabBarScale - 1) * prefabSize.y * pivot.x * -1;
            }
            
        }

    }

    private float tempX , tempY;
    private int tempRow , tempIndex;
    private void UpdateEndPos()
    {
        tempX = 0;
        tempY = 0;
        tempRow = 0;
        tempIndex = 0;
        float startX = pivot.x * prefabSize.x;
        float startY = (pivot.y - 1) * prefabSize.y;
        for (int i = 0; i < barInfoList.Count; i++)
        {
            if (columns <= 0)
            {
                tempRow = 1;
                tempIndex = i + 1;
            }
            else
            {
                tempRow = Mathf.CeilToInt((float)(i + 1) / (float)columns);
                tempIndex = (i + 1) % columns == 0 ? columns : (i % columns) + 1;
            }
            if (axes == Axes.Horizontal)
            {
                tempX = startX + barInfoList[i].leftValue + (prefabSize.x + pad.x) * (tempIndex - 1);
                tempY = startY + -1 * (prefabSize.y + pad.y) * (tempRow - 1);
                barInfoList[i].endPos = new Vector2(tempX, tempY);
            }
            else
            {
                tempX = startX + (prefabSize.x + pad.x) * (tempIndex - 1);
            tempY = startY + barInfoList[i].topValue + (prefabSize.y + pad.y) * (tempRow - 1)* -1 ;
            barInfoList[i].endPos = new Vector2(tempX, tempY);
            }
            
        }
    }
    private void DoExtrudingAffect()
    {
        for (int i = 0; i < barInfoList.Count; i++)
        {
            barInfoList[i].tabBar.rectTransform.anchoredPosition = barInfoList[i].startPos;
            if (barInfoList[i].scale <= 1)
            {
                if (axes == Axes.Horizontal)barInfoList[i].DoExtrudingAffectHorizontal(moveDurationTime, scaleEase);
                else barInfoList[i].DoExtrudingAffectVertical(moveDurationTime, scaleEase);
            }
            barInfoList[i].DOCustomizeTweenScal(scaleDurationTime, scaleEase);
        }
    }
    #endregion

}