using UnityEngine;

/// <summary>
/// 根节点
/// </summary>
[ExecuteInEditMode]
[RequireComponent(typeof(Canvas))]
public class UIRoot:MonoBehaviour
{
    private static UIRoot _instance;
    public static GameObject root;
    public static RectTransform rootTransform;

    public const float STANDARD_WIDTH = 750;//标准宽度
    public const float STANDARD_HEIGHT = 1334;//标准高度
    public const float STANDARD_RATE = STANDARD_WIDTH / STANDARD_HEIGHT;//标准分辨率比
    
    /// <summary>
    /// 当前分辨率宽度
    /// </summary>
    public static float Width
    {
        get { return rootTransform.rect.width; }
    }

    /// <summary>
    /// 当前分辨率高度
    /// </summary>
    public static float Height
    {
        get { return rootTransform.rect.height; }
    }

    /// <summary>
    /// 当前整体的缩放
    /// </summary>
    public static float Scale
    {
        get { return rootTransform.localScale.x; }
    }

    /// <summary>
    /// 当前分辨率比
    /// </summary>
    public static float CurrentRate
    {
        get { return (float)Screen.width / Screen.height; }
    }

    /// <summary>
    /// 标准缩放比
    /// </summary>
    public static float StandardScale
    {
        get { return 2 / STANDARD_HEIGHT; }
    }
    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            root = gameObject;
            rootTransform = GetComponent<RectTransform>();
            //Screen.orientation = ScreenOrientation.Landscape;
        }
        else
        {
            Destroy(this);
            Debug.LogWarning("Only one UIRoot in the scene");
        }
    }

    void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
            root = null;
            rootTransform = null;
        }
    }
}