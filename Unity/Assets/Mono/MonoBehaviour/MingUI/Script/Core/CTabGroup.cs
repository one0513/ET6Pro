
using System.Collections.Generic;

/// <summary>
/// 标签页组
/// </summary>
public class CTabGroup : CToggleGroup {
    /// <summary>
    /// 显示红点
    /// </summary>
    /// <param name="index"></param>
    /// <param name="visible"></param>
    public void ShowRedTip(int index, bool visible) {
        if (index >= 0 && index < toggleList.Count) {
            CTabBar bar = toggleList[index] as CTabBar;
            if (bar != null) {
                bar.ShowRedTip(visible);
            }
        }
    }

    public void SetTabBarInteractable(int index, bool interactable) {
        if (index >= 0 && index < toggleList.Count) {
            CTabBar bar = toggleList[index] as CTabBar;
            if (bar != null) {
                bar.interactable = interactable;
            }
        }
    }

    /// <summary>
    /// 设置标签栏文字 
    /// </summary>
    /// <param name="index"></param>
    /// <param name="tabLabel"></param>
    public void SetTabBarLabel(int index, string tabLabel) {
        if (index >= 0 && index < toggleList.Count) {
            CTabBar bar = toggleList[index] as CTabBar;
            if (bar != null) {
                bar.SetTabBarLabel(tabLabel);
            }
        }
    }

    /// <summary>
    /// 设置标签栏文字，列表
    /// </summary>
    /// <param name="paramList"></param>
    internal void SetTabBarLabelList(List<object> paramList) {
        if (paramList.Count > 0) {
            int tabBarCount = toggleList.Count;
            for (int index = 0; index < tabBarCount; index++) {
                if (index < paramList.Count) {
                    toggleList[index].gameObject.SetActive(true);
                    string newTabLabel = paramList[index] as string;
                    SetTabBarLabel(index, newTabLabel);
                } else {
                    toggleList[index].gameObject.SetActive(false);
                }
            }
        }
    }

    
}