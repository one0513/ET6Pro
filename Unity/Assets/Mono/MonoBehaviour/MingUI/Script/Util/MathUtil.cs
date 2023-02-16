using System;
using UnityEngine;

public class MathUtil
{
    /// <summary>
    /// 通过在自身的锚点获取对应锚点在自身的坐标
    /// </summary>
    /// <param name="trans"></param>
    /// <param name="anchor"></param>
    /// <returns></returns>
    public static Vector2 GetLocalByAnchor(RectTransform trans,Vector2 anchor)
    {
        Rect rect = trans.rect;
        Vector2 pos = Vector2.zero;
        pos.x += rect.xMin + anchor.x * rect.width;
        pos.y += rect.yMin + anchor.y * rect.height;
        return pos;
    }
    /// <summary>
    /// 通过在自身的锚点获取对应锚点在世界的坐标
    /// </summary>
    /// <param name="trans"></param>
    /// <param name="anchor"></param>
    /// <returns></returns>
    public static Vector3 GetWorldByAnchor(RectTransform trans, Vector2 anchor)
    {
        Vector2 pos = GetLocalByAnchor(trans,anchor);
        return trans.TransformPoint(new Vector3(pos.x,pos.y,0));
    }

    /// <summary>
    /// 获取当前时间戳
    /// </summary>
    /// <returns></returns>
    public static long GetTime()
    {
        TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        return Convert.ToInt64(ts.TotalSeconds);
    }
}