using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class LayoutElementContainer:UIBehaviour,ILayoutElement
{
    public enum Type
    {
        Original,
        GoodsImage,
        HeadImage,
        EquipImage,
    }

    public Type layoutType;

    private RectTransform _selfTransform;

    public RectTransform SelfTransform
    {
        get
        {
            if (_selfTransform == null)
            {
                _selfTransform = GetComponent<RectTransform>();
            }
            return _selfTransform;
        }
    }
    /// <summary>
    /// 宽度
    /// </summary>
    public float Width
    {
        get { return Size.x; }
        set { Size = new Vector2(value, Size.y); }
    }

    /// <summary>
    /// 高度
    /// </summary>
    public float Height
    {
        get { return Size.y; }
        set { Size = new Vector2(Size.x, value); }
    }

    /// <summary>
    /// 大小
    /// </summary>
    public Vector2 Size
    {
        get { return SelfTransform.rect.size; }
        set
        {
            if (value != SelfTransform.rect.size)
            {
                SelfTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, value.x);
                SelfTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, value.y);
            }
        }
    }

    public virtual void CalculateLayoutInputHorizontal() { }
    public virtual void CalculateLayoutInputVertical() { }

    public virtual float minWidth { get { return 0; } }
    public virtual float minHeight { get { return 0; } }
    public virtual float preferredWidth { get { return Width; } }
    public virtual float preferredHeight { get { return Height; } }
    public virtual float flexibleWidth { get { return -1; } }
    public virtual float flexibleHeight { get { return -1; } }
    public virtual int layoutPriority { get { return 0; } }
}

