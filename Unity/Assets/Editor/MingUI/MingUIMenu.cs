using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class MingUIMenu
{
    public const string UI_PATH = "Assets/Bundles/UI/";
    public const string MING_UI_PATH = UI_PATH + "MingUI/";
    public const string COMMON_UI_PATH = UI_PATH + "Common/";

    #region Create

    

    //[MenuItem("GameObject/AefUI/TipContainer", false, 10)]
    //public static void AddBaseTipContainer()
    //{
    //    AddChild("TipContainer", "TipContainer");
    //}

    //[MenuItem("GameObject/AefUI/CScrollBar", false, 10)]
    //public static void AddCScrollBar()
    //{
    //    AddChild("CScrollBar", "ScrBar");
    //}

    //[MenuItem("GameObject/AefUI/CSlider", false, 11)]
    //public static void AddCSlider()
    //{
    //    AddChild("CSlider", "Slider");
    //}

  
    private static List<string> mEntries = new List<string>();
    private static GenericMenu mMenu;

    public static void InitSceneMenu() {
        var type = typeof (MingUIMenu);
        var methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public);
        foreach (var methodInfo in methods) {
            var attrs = methodInfo.GetCustomAttributes(typeof(MenuItem), false);
            foreach (var attr in attrs) {
                var menu = attr as MenuItem;
                if (menu.menuItem.StartsWith("MingUI")) {
                    AddItem(menu.menuItem, false, (GenericMenu.MenuFunction) Delegate.CreateDelegate(typeof(GenericMenu.MenuFunction),methodInfo));
                }
            }
        }


        //AddItem("MingUI/CostComponent", false, AddCostComponent);
        //AddItem("MingUI/CProgressBar &#p", false, AddCostComponent);
        //AddItem("MingUI/ItemsShow", false, AddItemsShow);
    }

    public static void AddItem(string item, bool isChecked, GenericMenu.MenuFunction callback) {
        if (callback != null) {
            if (mMenu == null) mMenu = new GenericMenu();
            int count = 0;

            for (int i = 0; i < mEntries.Count; ++i) {
                string str = mEntries[i];
                if (str == item) ++count;
            }
            mEntries.Add(item);

            if (count > 0) item += " [" + count + "]";
            mMenu.AddItem(new GUIContent(item), isChecked, callback);
        }
    }

    public static void Show() {
        if (mMenu != null) {
            mMenu.ShowAsContext();
            mMenu = null;
            mEntries.Clear();
        }
    }

    #endregion

    public static Vector3 mousePosition;
    public static GameObject AddChild(string componentName, string childName)
    {
        //ALog.Info("AddChild" + componentName + " " + childName);
        GameObject go;
        GameObject parent = SelectedRoot();

        if (parent == null)
        {
            parent = CreateRoot();
        }
        if (parent != null)
        {
            go = InstanceMingUI(componentName,parent.transform);
            go.name = childName;
            Transform t = go.transform;
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
            t.localScale = Vector3.one;

            if (componentName.Contains("PopupPanel")) {
                (t as RectTransform).offsetMax = Vector2.zero;
                (t as RectTransform).offsetMin = Vector2.zero;
            }

            go.layer = parent.layer;

            var rect = t as RectTransform;
            rect.anchoredPosition = Vector2.zero;
            Selection.activeGameObject = go;
            return go;
        }
        return null;
    }

    public static GameObject InstanceMingUI(string componentName,Transform parent)
    {
        return Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(MING_UI_PATH + "Prefabs/" + componentName + ".prefab"), parent);
    }

    public static GameObject SelectedRoot()
    {
        GameObject go = Selection.activeGameObject;
        if (go == null)
        {
            go = GameObject.Find("UIRoot");
        }
        return go;
    }

    public static GameObject CreateRoot()
    {
        GameObject go = new GameObject("UIRoot");
        go.transform.localPosition = Vector3.zero;
        GameObject camGo = new GameObject("Camera");
        camGo.transform.SetParent(go.transform);
        Camera camera = camGo.AddComponent<Camera>();
        camera.depth = 1;
        camera.orthographic = true;
        camera.orthographicSize = 1;
        camera.clearFlags = CameraClearFlags.Depth;
        camera.cullingMask = 1 << 5;
        camera.farClipPlane = 10;
        camera.nearClipPlane = -10;

        Canvas canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = camera;
        canvas.pixelPerfect = false;
        canvas.sortingOrder = 0;

        CanvasScaler scaler = go.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1334, 750);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0;
        scaler.referencePixelsPerUnit = 100;

        GraphicRaycaster raycaster = go.AddComponent<GraphicRaycaster>();
        raycaster.ignoreReversedGraphics = false;
        raycaster.blockingObjects = GraphicRaycaster.BlockingObjects.None;

        go.AddComponent<UIRoot>();

        MingUIUtil.SetLayer(go, 5);

        EventSystem system = Object.FindObjectOfType<EventSystem>();
        if (system == null)
        {
            GameObject eventGo = new GameObject("EventSystem");
            eventGo.AddComponent<EventSystem>();
            eventGo.AddComponent<StandaloneInputModule>();
        }
        return go;
    }
}