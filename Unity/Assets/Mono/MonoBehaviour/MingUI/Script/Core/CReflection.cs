using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class CReflection : BaseMeshEffect {
    [SerializeField]
    private Color m_EffectColor = new Color(1f, 1f, 1f, 0.5f);

    [SerializeField]
    private Vector2 m_EffectDistance = new Vector2(1f, -1f);

    [SerializeField]
    private bool m_UseGraphicAlpha = true;

    private const float kMaxEffectDistance = 600f;

    protected CReflection() { }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            effectDistance = m_EffectDistance;
            base.OnValidate();
        }

#endif

    public Color effectColor {
        get { return m_EffectColor; }
        set {
            m_EffectColor = value;
            if (graphic != null)
                graphic.SetVerticesDirty();
        }
    }

    public Vector2 effectDistance {
        get { return m_EffectDistance; }
        set {
            if (value.x > kMaxEffectDistance)
                value.x = kMaxEffectDistance;
            if (value.x < -kMaxEffectDistance)
                value.x = -kMaxEffectDistance;

            if (value.y > kMaxEffectDistance)
                value.y = kMaxEffectDistance;
            if (value.y < -kMaxEffectDistance)
                value.y = -kMaxEffectDistance;

            if (m_EffectDistance == value)
                return;

            m_EffectDistance = value;

            if (graphic != null)
                graphic.SetVerticesDirty();
        }
    }

    public bool useGraphicAlpha {
        get { return m_UseGraphicAlpha; }
        set {
            m_UseGraphicAlpha = value;
            if (graphic != null)
                graphic.SetVerticesDirty();
        }
    }

    protected void ApplyRelflectionZeroAlloc(List<UIVertex> verts, Color32 color, int start, int end, float x, float y) {
        UIVertex vt;

        var neededCapacity = verts.Count + end - start;
        if (verts.Capacity < neededCapacity)
            verts.Capacity = neededCapacity;

        for (int i = start; i < end; ++i) {
            vt = verts[i];
            verts.Add(vt);

            Vector3 v = vt.position;
            v.x += x;
            v.y = v.y - 2 * v.y - 2 * y;
            vt.position = v;
            var newColor = color;
            if (m_UseGraphicAlpha)
                newColor.a = (byte)((newColor.a * verts[i].color.a) / 255);
            vt.color = newColor;
            verts[i] = vt;
        }
    }

    protected void ApplyRelflection(List<UIVertex> verts, Color32 color, int start, int end, float x, float y) {
        ApplyRelflectionZeroAlloc(verts, color, start, end, x, y);
    }

    public override void ModifyMesh(VertexHelper vh) {
        if (!IsActive())
            return;

        var output = LPool<UIVertex>.Get();
        vh.GetUIVertexStream(output);

        ApplyRelflection(output, effectColor, 0, output.Count, effectDistance.x, effectDistance.y);
        vh.Clear();
        vh.AddUIVertexTriangleStream(output);
        LPool<UIVertex>.Release(output);
    }
}
