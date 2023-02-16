using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class COutline : Outline
{
    public int pxNum = 1;
    public override void ModifyMesh(VertexHelper vh)
    {
        if (!IsActive())
            return;

        for (int i = 0; i < pxNum; i++)
        {
            var verts = LPool<UIVertex>.Get();
            vh.GetUIVertexStream(verts);

            var neededCpacity = verts.Count * 5;
            if (verts.Capacity < neededCpacity)
                verts.Capacity = neededCpacity;

            var start = 0;
            var end = verts.Count;
            ApplyShadowZeroAlloc(verts, effectColor, start, verts.Count, effectDistance.x, effectDistance.y);

            start = end;
            end = verts.Count;
            ApplyShadowZeroAlloc(verts, effectColor, start, verts.Count, effectDistance.x, -effectDistance.y);

            start = end;
            end = verts.Count;
            ApplyShadowZeroAlloc(verts, effectColor, start, verts.Count, -effectDistance.x, effectDistance.y);

            start = end;
            end = verts.Count;
            ApplyShadowZeroAlloc(verts, effectColor, start, verts.Count, -effectDistance.x, -effectDistance.y);

            vh.Clear();
            vh.AddUIVertexTriangleStream(verts);
            LPool<UIVertex>.Release(verts);
        }
    }
}

public class OPool<T> where T : new() {
    private readonly Stack<T> m_Stack = new Stack<T>();
    private readonly UnityAction<T> m_ActionOnGet;
    private readonly UnityAction<T> m_ActionOnRelease;

    public int countAll { get; private set; }
    public int countActive { get { return countAll - countInactive; } }
    public int countInactive { get { return m_Stack.Count; } }

    public OPool(UnityAction<T> actionOnGet, UnityAction<T> actionOnRelease) {
        m_ActionOnGet = actionOnGet;
        m_ActionOnRelease = actionOnRelease;
    }

    public T Get() {
        T element;
        if (m_Stack.Count == 0) {
            element = new T();
            countAll++;
        } else {
            element = m_Stack.Pop();
        }
        if (m_ActionOnGet != null)
            m_ActionOnGet(element);
        return element;
    }

    public void Release(T element) {
        if (m_Stack.Count > 0 && ReferenceEquals(m_Stack.Peek(), element))
            Debug.LogError("Internal error. Trying to destroy object that is already released to pool.");
        if (m_ActionOnRelease != null)
            m_ActionOnRelease(element);
        m_Stack.Push(element);
    }
}

public static class LPool<T> {
    // Object pool to avoid allocations.
    private static readonly OPool<List<T>> s_ListPool = new OPool<List<T>>(null, l => l.Clear());

    public static List<T> Get() {
        return s_ListPool.Get();
    }

    public static void Release(List<T> toRelease) {
        s_ListPool.Release(toRelease);
    }
}
