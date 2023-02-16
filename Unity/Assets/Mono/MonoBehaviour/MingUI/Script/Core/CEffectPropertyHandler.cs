using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 1. 非UI的特效，用来替代CEffect管理粒子系统的EffectPropertyItem
/// 2. 用于所有特效进行排序处理
/// </summary>
public class CEffectPropertyHandler : MonoBehaviour {

    private static Dictionary<EffectSortingUnit.SortinType, EffectSortingUnit> sortingHandleDic;

    private List<EffectPropertyItem> propertyItemList;
    private EffectSortingUnit.SortinType sortingType = EffectSortingUnit.SortinType.SortingOrder;
    private bool isInit = false;

    protected System.Action _onDesotryCallBack;
    private bool isDestroy = false;

    public bool IsDestroy
    {
        get { return isDestroy; }
    }

    // Use this for initialization
    void Start() {
        InitRenderers();
    }

    public void RegisterDestroyCallBack(System.Action action) {
        _onDesotryCallBack = action;
    }

    private void OnDestroy() {
        isDestroy = true;
        ClearEffectSorting(gameObject.name, sortingType);
        if(_onDesotryCallBack != null) {
            _onDesotryCallBack.Invoke();
        }
    }

    private void InitRenderers() {
        if (!isInit) {
            isInit = true;
            var renderers = this.GetComponentsInChildren<Renderer>();
            propertyItemList = new List<EffectPropertyItem>();
            for (int i = 0; i < renderers.Length; i++) {
                var item = renderers[i].gameObject.GetComponent<EffectPropertyItem>();
                if (item == null) {
                    item = renderers[i].gameObject.AddComponent<EffectPropertyItem>();
                }
                propertyItemList.Add(item);
            }
        }
    }

    private void SetEffectSorting(string effName, EffectSortingUnit.SortinType sortType) {
        InitRenderers();
        //排序
        if(sortingHandleDic == null) {
            sortingHandleDic = new Dictionary<EffectSortingUnit.SortinType, EffectSortingUnit>();
        }
        if (!sortingHandleDic.ContainsKey(sortType)) {
            sortingHandleDic.Add(sortType, new EffectSortingUnit(sortType));
        }
        if (sortingHandleDic.ContainsKey(sortType)) {
            sortingHandleDic[sortType].SetGlobalSorting(effName, propertyItemList);
        }
    }

    private void ClearEffectSorting(string effName, EffectSortingUnit.SortinType sortType) {
        //清除排序信息
        if (sortingHandleDic != null && sortingHandleDic.ContainsKey(sortType)) {
            sortingHandleDic[sortType].ClearGlobalSorting(effName);
        }
    }

    public void SetEffectAlpha(float alpha) {
        if (propertyItemList != null) {
            for (int i = 0; i < propertyItemList.Count; i++) {
                propertyItemList[i].SetAlpha(alpha);
            }
        }
    }

    public void SetEffectSorting(bool isUIEffect) {
        if (isUIEffect) {
            sortingType = EffectSortingUnit.SortinType.SortingFudge;
        } else {
            sortingType = EffectSortingUnit.SortinType.SortingOrder;
        }
        SetEffectSorting(gameObject.name, sortingType);
    }

    
}
