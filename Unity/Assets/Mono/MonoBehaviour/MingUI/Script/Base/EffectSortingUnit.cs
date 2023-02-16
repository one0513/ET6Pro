using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EffectSortingUnit {

    public enum SortinType {
        SortingOrder = 0,
        SortingFudge = 1
    }

    public class EffectSortingInfo {

        private string effName;
        private int instanceCount;
        private int useRangeIndex;
        private Dictionary<int, int> effSortDic = new Dictionary<int, int>();

        public void SetEffectName(string name) {
            effName = name;
        }

        public bool HasSortInfo(int code) {
            return effSortDic.ContainsKey(code);
        }

        public void SetSortInfo(int code, int index) {
            effSortDic[code] = index;
        }

        public int GetSortOrder(int code) {
            return effSortDic[code];
        }

        public void AddInstance() {
            instanceCount++;
        }

        public bool DelInstance() {
            instanceCount--;
            return instanceCount == 0;
        }

        public void SetRangeIndex(int rangeIndex) {
            useRangeIndex = rangeIndex;
        }

        public int GetUseRangeIndex() {
            return useRangeIndex;
        }

        public void Clear() {
            effSortDic.Clear();
            effSortDic = null;
        }
    }

    /// <summary>
    /// 对Renderer进行排序设置，根据不同的指定排序类型，[0 - SortingOrder] | [1 - SortingFudge]
    /// </summary>
    /// <param name="render"></param>
    /// <param name="sortType"></param>
    public static void SetRenderSortByType(EffectPropertyItem item, int sortVal, SortinType type) {
        sortVal += 100;   //层级从100开始，防止被2D场景覆盖
        switch (type) {
            case SortinType.SortingOrder:
                item.SetSortingOrder(sortVal);
                break;
            case SortinType.SortingFudge:
                item.SetSortingFudge(sortVal);
                break;
            default:
                item.SetSortingOrder(sortVal);
                break;
        }
    }

    private static void SortingRenderListByType(List<EffectPropertyItem> propertyItemList, SortinType type) {
        switch (type) {
            case SortinType.SortingOrder:
                propertyItemList.Sort(CompareRenderBySortingFudge0);
                break;
            case SortinType.SortingFudge:
                propertyItemList.Sort(CompareRenderBySortingFudge1);
                break;
            default:
                propertyItemList.Sort(CompareRenderBySortingFudge0);
                break;
        }
    }

    private static int CompareRenderBySortingFudge0(EffectPropertyItem itemA, EffectPropertyItem itemB) {
        if (itemA.customSortingFudge > itemB.customSortingFudge) {
            return -1;
        } else if (itemA.customSortingFudge == itemB.customSortingFudge) {
            return 0;
        } else {
            return 1;
        }
    }

    private static int CompareRenderBySortingFudge1(EffectPropertyItem itemA, EffectPropertyItem itemB) {
        if (itemA.customSortingFudge < itemB.customSortingFudge) {
            return -1;
        } else if (itemA.customSortingFudge == itemB.customSortingFudge) {
            return 0;
        } else {
            return 1;
        }
    }

    private int gloabalRangeIndex = 0;
    private List<int> bigRangeListTemp = new List<int>();
    private Dictionary<int, bool> freeRangeIndex = new Dictionary<int, bool>();
    private Dictionary<string, EffectSortingInfo> globalSortInfoDic = new Dictionary<string, EffectSortingInfo>();

    private SortinType sortingType = SortinType.SortingOrder;

    public EffectSortingUnit(SortinType type) {
        sortingType = type;
    }

    public void SetGlobalSorting(string effName, List<EffectPropertyItem> propertyItemList) {
        //增加处理，如果总数超过200的时候，不予处理
        if (propertyItemList == null || propertyItemList.Count == 0 || propertyItemList.Count > 200) return;
        ToSetGlobalSorting(effName, propertyItemList);
    }

    public void ClearGlobalSorting(string effName) {
        ToClearGlobalSorting(effName);
    }

    private void ToSetGlobalSorting(string effectName, List<EffectPropertyItem> propertyItemList) {
        if (globalSortInfoDic.ContainsKey(effectName) == false) {
            SortingRenderListByType(propertyItemList, sortingType);
            EffectSortingInfo info = new EffectSortingInfo();
            info.SetEffectName(effectName);
            info.SetRangeIndex(GetCurRangeByCount(propertyItemList.Count));
            globalSortInfoDic.Add(effectName, info);
            //排序的初始值设置
            int rangeFirstIndex = (info.GetUseRangeIndex() % 1000) * 100;
            int rangeFirstMax = rangeFirstIndex + 100;
            int rangeSecondIndex = (Mathf.FloorToInt(info.GetUseRangeIndex() / 1000)) * 100;
            int orderIndex = rangeFirstIndex;
            if (sortingType == SortinType.SortingOrder) {
                foreach (EffectPropertyItem item in propertyItemList) {
                    //没有材质球的不修改sortingOrder
                    if (item.CanSorting()) {
                        int matCode = item.GetSortingCode();
                        if (info.HasSortInfo(matCode) == false) {
                            orderIndex = orderIndex + 1;
                            info.SetSortInfo(matCode, orderIndex);
                            if (orderIndex >= rangeFirstMax) {
                                orderIndex = rangeSecondIndex;
                            }
                        }
                        SetRenderSortByType(item, info.GetSortOrder(matCode), sortingType);
                    }
                }
            } else {
                int rangeSecondMax = rangeSecondIndex + 100;
                foreach (EffectPropertyItem item in propertyItemList) {
                    //没有材质球的不修改sortingOrder
                    if (item.CanSorting()) {
                        int matCode = item.GetSortingCode();
                        if (info.HasSortInfo(matCode) == false) {
                            orderIndex = orderIndex + 1;
                            if (item.customSortingFudge >= 0) {
                                info.SetSortInfo(matCode, orderIndex);
                            } else {
                                //SortingFudge是负数的话，顺序又要倒置
                                if (orderIndex > rangeFirstMax) {
                                    info.SetSortInfo(matCode, orderIndex - rangeSecondMax - 100);
                                } else {
                                    info.SetSortInfo(matCode, orderIndex - rangeFirstMax - 100);
                                }
                            }
                            if (orderIndex >= rangeFirstMax) {
                                orderIndex = rangeSecondIndex;
                            }
                        }
                        SetRenderSortByType(item, info.GetSortOrder(matCode), sortingType);
                    }
                }
            }
            //Debug.LogWarning("add " + effectName);
        } else {
            EffectSortingInfo info = globalSortInfoDic[effectName];
            foreach (EffectPropertyItem item in propertyItemList) {
                if (item.CanSorting()) {
                    int matCode = item.GetSortingCode();
                    if (info.HasSortInfo(matCode)) {
                        SetRenderSortByType(item, info.GetSortOrder(matCode), sortingType);
                    }
                }
            }
        }
        globalSortInfoDic[effectName].AddInstance();
    }

    private void ToClearGlobalSorting(string effectName) {
        if (globalSortInfoDic.ContainsKey(effectName)) {
            if (globalSortInfoDic[effectName].DelInstance()) {
                //清除缓存
                globalSortInfoDic[effectName].Clear();
                //释放占用的range
                RelaseRangeIndex(globalSortInfoDic[effectName].GetUseRangeIndex());
                globalSortInfoDic.Remove(effectName);

                //Debug.LogWarning("release " + effectName);
            }
        }
    }

    private int GetCurRangeByCount(int renderCount) {
        //如果超过了300，这里开始一轮强制复用
        if (gloabalRangeIndex > 300) {
            gloabalRangeIndex = 0;
            Debug.LogError("特效动态排序，缓存序列超出范围，已经重置");
        }
        int rangeIndex = 0;
        if (renderCount <= 100) {
            if (freeRangeIndex.Count < 1) {
                rangeIndex = gloabalRangeIndex++;
            } else {
                foreach (int key in freeRangeIndex.Keys) {
                    rangeIndex = key;
                    break;
                }
                freeRangeIndex.Remove(rangeIndex);
            }
        } else {
            //可以扩展到处理多个，目前限制在2个以内
            if (freeRangeIndex.Count < 2) {
                int addCount = 2 - freeRangeIndex.Count;
                for (int i = 0; i < addCount; i++) {
                    freeRangeIndex.Add(gloabalRangeIndex, true);
                    gloabalRangeIndex++;
                }
            }
            bigRangeListTemp.Clear();
            int rangeCount = 2;//直接需用2，后续扩展处理多个的话，这里也要修改
            foreach (int key in freeRangeIndex.Keys) {
                bigRangeListTemp.Add(key);
                rangeCount--;
                if (rangeCount == 0) {
                    break;
                }
            }
            rangeIndex = bigRangeListTemp[1] * 1000 + bigRangeListTemp[0];// %1000求余的数是用来充当第一个Index的。
            foreach (int range in bigRangeListTemp) {
                freeRangeIndex.Remove(range);
            }
        }
        return rangeIndex;
    }

    private void RelaseRangeIndex(int rangeIndex) {
        int beginRange = rangeIndex % 1000;
        if (freeRangeIndex.ContainsKey(beginRange) == false) {
            freeRangeIndex[beginRange] = true;
        }
        if (rangeIndex > 1000) {
            int nextRange = Mathf.FloorToInt(rangeIndex / 1000);
            if (freeRangeIndex.ContainsKey(nextRange) == false) {
                freeRangeIndex[nextRange] = true;
            }
        }
    }
}
