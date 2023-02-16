using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MingUIUtil {
    /// <summary>
    /// 创建自带RectTransform的GameObject
    /// </summary>
    /// <param name="parentObj"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    static public RectTransform CreateGameObject(GameObject parentObj, string name = "GameObject") {
        GameObject go = new GameObject(name);
        go.layer = 5;//UI层
        RectTransform trans = go.AddComponent<RectTransform>();
        trans.SetParent(parentObj.transform);
        trans.localEulerAngles = Vector3.zero;
        trans.localPosition = Vector3.zero;
        trans.localScale = Vector3.one;
        return trans;
    }

    /// <summary>
    /// 自动全铺自适应大小
    /// </summary>
    /// <param name="trans"></param>
    static public void AutoSize(RectTransform trans) {
        trans.pivot = Vector2.one * 0.5f;//居中
        trans.anchorMin = Vector2.zero;//自适应
        trans.anchorMax = Vector2.one;//自适应
        trans.offsetMin = trans.offsetMax = Vector2.zero;//自适应
    }

    /// <summary>
    /// 添加孩子
    /// </summary>
    /// <param name="child"></param>
    /// <param name="parent"></param>
    static public void AddChild(GameObject child, GameObject parent) {
        child.transform.SetParent(parent.transform);
        child.transform.localEulerAngles = Vector3.zero;
        child.transform.localPosition = Vector3.zero;
        child.transform.localScale = Vector3.one;
    }
    /// <summary>
    /// 交换孩子深度
    /// </summary>
    /// <param name="childA"></param>
    /// <param name="childB"></param>
    static public void SwapChildIndex(GameObject childA, GameObject childB) {
        int indexA = childA.transform.GetSiblingIndex();
        int indexB = childB.transform.GetSiblingIndex();
        childB.transform.SetSiblingIndex(indexA);
        childA.transform.SetSiblingIndex(indexB);
    }
    /// <summary>
    /// 设置layer
    /// </summary>
    /// <param name="go"></param>
    /// <param name="layer"></param>
    static public void SetLayer(GameObject go, int layer) {
        go.layer = layer;
        Transform t = go.transform;

        for (int i = 0, imax = t.childCount; i < imax; ++i) {
            Transform child = t.GetChild(i);
            SetLayer(child.gameObject, layer);
        }
    }
    /// <summary>
    /// 对象响应\关闭事件（会同步到子对象）
    /// </summary>
    /// <param name="go">对象</param>
    /// <param name="enable">能否响应事件</param>
    /// <param name="interactable">能否交互</param>
    static public void EnableEvent(GameObject go, bool enable, bool interactable = true) {
        CanvasGroup group = go.GetComponent<CanvasGroup>();
        if (group == null) {
            group = go.AddComponent<CanvasGroup>();
        }
        group.blocksRaycasts = enable;
        group.interactable = interactable;
    }

    /// <summary>
    /// 设置群组透明度
    /// </summary>
    /// <param name="go"></param>
    /// <param name="alpha"></param>
    static public void SetGroupAlpha(GameObject go, float alpha) {
        CanvasGroup group = go.GetComponent<CanvasGroup>();
        if (group == null) {
            group = go.AddComponent<CanvasGroup>();
        }
        if (group != null) {
            DOTween.Kill(group);
        }
        group.alpha = alpha;
    }

    /// <summary>
    /// 设置群组渐显渐隐
    /// </summary>
    /// <param name="go"></param>
    /// <param name="alpha"></param>
    /// <param name="time"></param>
    /// <param name="type"></param>
    /// <param name="callBack"></param>
    static public Tween SetGroupAlphaInTime(GameObject go, float alpha, float time, Ease type = Ease.Linear, TweenCallback callBack = null) {
        CanvasGroup group = go.GetComponent<CanvasGroup>();
        if (group == null) {
            group = go.AddComponent<CanvasGroup>();
        }
        if (group != null) {
            DOTween.Kill(group);
        }
        Tween tween = group.DOFade(alpha, time).SetEase(type);
        if (callBack != null) {
            tween.OnComplete(callBack);
        }
        return tween;
    }

    /// <summary>
    /// 获取群组透明度
    /// </summary>
    /// <param name="go"></param>
    /// <returns></returns>
    static public float GetGroupAlpha(GameObject go) {
        float alpha = 1;
        Transform t = go.transform;
        CanvasGroup group;
        while (t != null) {
            group = t.GetComponent<CanvasGroup>();
            if (group != null) {
                alpha *= group.alpha;
                if (group.ignoreParentGroups) {
                    return alpha;
                }
            }
            t = t.parent;
        }
        return alpha;
    }
    /// <summary>
    /// 灰度化
    /// </summary>
    /// <param name="go"></param>
    /// <param name="value"></param>
    /// <param name="includeChildren">是否同步子对象</param>
    public static void SetGray(GameObject go, bool value, bool includeChildren = true) {
        if (includeChildren) {
            IGray[] units = go.GetComponentsInChildren<IGray>(true);
            for (int i = 0; i < units.Length; i++) {
                units[i].Gray = value;
            }
        } else {
            IGray unit = go.GetComponent<IGray>();
            if (unit != null) {
                unit.Gray = value;
            }
        }
    }

    /// <summary>
    /// 添加缩放按钮样式
    /// </summary>
    /// <param name="gameObject"></param>
    /// <returns></returns>
    public static CButtonEffect AddButtonEffect(GameObject gameObject) {
        CButtonEffect effect = gameObject.GetComponent<CButtonEffect>();
        if (effect == null) {
            effect = gameObject.AddComponent<CButtonEffect>();
        }
        effect.enabled = true;
        return effect;
    }
    /// <summary>
    /// 动态添加Canvas
    /// </summary>
    /// <param name="go"></param>
    /// <param name="sortingOrder"></param>
    public static Canvas AddCanvas(GameObject go, int sortingOrder, int sortingLayer = 0) {
        Canvas canvas = go.GetComponent<Canvas>();
        if (canvas == null) canvas = go.AddComponent<Canvas>();
        canvas.enabled = true;
        //isRootCanvas若为true，则此时overrideSorting设置是无效的
        //导致isRootCanvas为true的原因有两个：1.该物体无父容器 2.该物体的activeInHierarchy==false
        if (canvas.isRootCanvas) {
            Transform t = go.transform;
            while (t != null) {
                if (t.parent == UIRoot.rootTransform) {
                    break;
                } else if (t.parent == null) {
                    AddChild(t.gameObject, UIRoot.root);
                    break;
                }
                t = t.parent;
            }
        }
        canvas.overrideSorting = true;
        canvas.sortingOrder = sortingOrder;
        canvas.sortingLayerID = sortingLayer;
        canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.TexCoord1 |
                                          AdditionalCanvasShaderChannels.TexCoord2 |
                                          AdditionalCanvasShaderChannels.TexCoord3 |
                                          AdditionalCanvasShaderChannels.Normal |
                                          AdditionalCanvasShaderChannels.Tangent;
        GraphicRaycaster raycaster = go.GetComponent<GraphicRaycaster>();
        if (raycaster == null) raycaster = go.AddComponent<GraphicRaycaster>();
        raycaster.enabled = true;
        raycaster.blockingObjects = GraphicRaycaster.BlockingObjects.ThreeD;
        return canvas;
    }

    /// <summary>
    ///  动态获取对象所在的父面板的CPanel
    /// </summary>
    /// <param name="go"></param>
    /// <returns></returns>
    public static CPanel GetCPanel(GameObject go) {
        CPanel panel;
        Transform t = go.transform;
        while (t != null) {
            panel = t.GetComponent<CPanel>();
            if (panel != null) {
                return panel;
            }
            t = t.parent;
        }
        return null;
    }

    /// <summary>
    /// 动态获取对象所在的父面板的sortorder
    /// </summary>
    /// <param name="go"></param>
    /// <returns></returns>
    public static int GetCPanelOrder(GameObject go) {
        CPanel panel = GetCPanel(go);
        if (panel != null) {
            return panel.SortingOrder;
        }
        return 0;
    }
    /// <summary>
    /// 设置选中的物体
    /// </summary>
    /// <param name="go"></param>
    public static void SetSelectedGameObject(GameObject go) {
        EventSystem.current.SetSelectedGameObject(go);
    }

    /// <summary>
    /// 模拟点击
    /// </summary>
    /// <param name="go"></param>
    public static void ApplayClickEvent(GameObject go) {
        if (go == null) {
            return;
        }
        IPointerClickHandler[] clikHandlers = go.GetComponentsInChildren<IPointerClickHandler>();
        if (clikHandlers != null) {
            for (int i = 0; i < clikHandlers.Length; i++) {
                clikHandlers[i].OnPointerClick(new PointerEventData(EventSystem.current));
            }
            //ExecuteEvents.Execute(btn.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
        }
    }

    public static void SetDynamicRectMask2D(CRectMaskDynamic2D dynamicMask2D, GameObject target, bool includeChildren, bool isUseMask) {
        if (dynamicMask2D != null) {
            if (includeChildren) {
                IClippable[] units = target.GetComponentsInChildren<IClippable>(true);
                if (isUseMask) {
                    for (int i = 0; i < units.Length; i++) {
                        dynamicMask2D.AddClippable(units[i]);
                    }
                } else {
                    for (int i = 0; i < units.Length; i++) {
                        dynamicMask2D.RemoveClippable(units[i]);
                    }
                }
            } else {
                IClippable unit = target.GetComponent<IClippable>();
                if (isUseMask) {
                    dynamicMask2D.AddClippable(unit);
                } else {
                    dynamicMask2D.RemoveClippable(unit);
                }
            }
        }
    }

    public static Tweener DOWidth(RectTransform target, float endValue, float duration) {
        var t = DOTween.To(() => target.rect.width,
            x => target.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, x)
            , endValue, duration);
        return TweenSettingsExtensions.SetOptions(t, true).SetTarget<Tweener>(target);
    }

    public static Tweener DOHeight(RectTransform target, float endValue, float duration) {
        var t = DOTween.To(() => target.rect.height,
            y => target.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, y)
            , endValue, duration);
        return TweenSettingsExtensions.SetOptions(t, true).SetTarget<Tweener>(target);
    }

    public static void initFakeTouch() {
        if (CFakeInputModule.instance == null) {
            EventSystem.current.currentInputModule.enabled = false;
            CFakeInputModule.instance = EventSystem.current.currentInputModule.gameObject.AddComponent<CFakeInputModule>();
        }
    }

    public static void FadeTouch(Vector2 srcPos) {
        if (CFakeInputModule.instance != null && EventSystem.current.currentInputModule == CFakeInputModule.instance) {
            CFakeInputModule.instance.FakeTouch(srcPos);
        }
    }
}