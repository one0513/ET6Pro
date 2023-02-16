using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CBezierCurve : CSprite {

    public Vector2 anchorPos1 = new Vector2(-10, 0);
    public Vector2 controlPos1 = new Vector2(-8, 2);
    public Vector2 anchorPos2 = new Vector2(10, 0);
    public Vector2 controlPos2 = new Vector2(8, -2);
    public float width = 12;
    public int curSegments = 3;

    public bool useFast = false;
    public bool useWeld = true;
    public bool useInnerUv = true;
    public bool useHeadBorder = false;
    public bool useReverse = false;

    public int showBeginIndex = 0;

    private bool hasInit = false;
    private bool curvePointsDirty = true;
    private Vector2[] m_lineVertices;
    private Vector2[] m_points2;

    private void SetLineVertices() {
        if (Application.isEditor) {
            m_lineVertices = new Vector2[curSegments * 4];
        } else {
            if (m_lineVertices == null) {
                m_lineVertices = new Vector2[curSegments * 4];
            } else {
                if (m_lineVertices.Length < curSegments * 4) {
                    m_lineVertices = new Vector2[curSegments * 4];
                }
            }
        }
    }

    private void CheckCurvePoints(int count) {
        if (Application.isEditor) {
            m_points2 = new Vector2[count];
        } else {
            if (m_points2 == null) {
                m_points2 = new Vector2[count];
            } else {
                if (m_points2.Length < count) {
                    m_points2 = new Vector2[count];
                }
            }
        }
    } 

    private void SetCurvePoints() {
        if (curvePointsDirty || Application.isEditor) {
            curvePointsDirty = false;
            int count = curSegments + 1;
            Vector2 anchor1a = anchorPos1; Vector2 anchor2a = anchorPos2;
            Vector2 control1a = controlPos1; Vector2 control2a = controlPos2;
            CheckCurvePoints(count);
            if (useReverse) {
                for (int i = 0; i < count; i++) {
                    m_points2[i] = GetBezierPoint(ref anchor1a, ref control1a, ref anchor2a, ref control2a, (float)(count - i - 1) / curSegments);
                }
            } else {
                for (int i = 0; i < count; i++) {
                    m_points2[i] = GetBezierPoint(ref anchor1a, ref control1a, ref anchor2a, ref control2a, (float)i / curSegments);
                }
            }
        }
    }

    /// <summary>
    /// 传值的时候，注意传的是绝对坐标，也就是基于视图的一个位置坐标
    /// </summary>
    /// <param name="centerPos">item的坐标</param>
    /// <param name="anchor1">左边的顶点</param>
    /// <param name="control1">左边的控制点</param>
    /// <param name="anchor2">左边的顶点</param>
    /// <param name="control2">右边的控制点</param>
    /// <param name="segments">段数</param>
    public void DrawCurve(Vector2 centerPos, Vector2 anchor1, Vector2 control1, Vector2 anchor2, Vector2 control2, int segments, float lineWidth) {
        if (anchorPos1 != anchor1 || anchorPos2 != anchor2 || controlPos1 != control1 || controlPos2 != control2 || curSegments != segments) {
            anchorPos1 = anchor1 - centerPos;
            anchorPos2 = anchor2 - centerPos;
            controlPos1 = control1 - centerPos;
            controlPos2 = control2 - centerPos;
            curSegments = segments;
            width = lineWidth;
            hasInit = true;
            curvePointsDirty = true;
            SetLineVertices();
            SetAllDirty();
        }
    }

    /// <summary>
    /// 传值的时候，注意传的是相对坐标，也就是基于本Graphic的中心坐标的差值
    /// </summary>
    /// <param name="anchor1">左边的顶点</param>
    /// <param name="control1">左边的控制点</param>
    /// <param name="anchor2">左边的顶点</param>
    /// <param name="control2">右边的控制点</param>
    /// <param name="segments">段数</param>
    public void DrawRelativeCurve(Vector2 anchor1, Vector2 control1, Vector2 anchor2, Vector2 control2, int segments, float lineWidth) {
        if (anchorPos1 != anchor1 || anchorPos2 != anchor2 || controlPos1 != control1 || controlPos2 != control2 || curSegments != segments) {
            anchorPos1 = anchor1;
            anchorPos2 = anchor2;
            controlPos1 = control1;
            controlPos2 = control2;
            curSegments = segments;
            width = lineWidth;
            hasInit = true;
            curvePointsDirty = true;
            SetLineVertices();
            SetAllDirty();
        }
    }

    public void SetShowBeginIndex(int index) {
        if (showBeginIndex != index) {
            showBeginIndex = index;
        }
    }

    public void SetReverse(bool isReverse) {
        if (useReverse != isReverse) {
            useReverse = isReverse;
            curvePointsDirty = true;
        }
    }

    /// <summary>
    /// Update the UI renderer mesh.
    /// </summary>
    protected override void OnPopulateMesh(VertexHelper toFill) {
        if (overrideSprite == null || (!Application.isEditor && !hasInit)) {
            base.OnPopulateMesh(toFill);
            return;
        }
        GenerateSimpleSprite(toFill, preserveAspect);
    }

    /// <summary>
    /// Generate vertices for a simple Image.
    /// </summary>
    void GenerateSimpleSprite(VertexHelper vh, bool lPreserveAspect) {
        vh.Clear();
        var color32 = color;
        Vector4 uv;
        Vector4 outer = (overrideSprite != null) ? UnityEngine.Sprites.DataUtility.GetOuterUV(overrideSprite) : Vector4.zero;
        if (useHeadBorder || useInnerUv) {
            uv = (overrideSprite != null) ? UnityEngine.Sprites.DataUtility.GetInnerUV(overrideSprite) : Vector4.zero;
        } else {
            uv = outer;
        }
        Vector4 headerUv = outer;
        Vector4 headerUv2 = uv;
        int start = 0;
        int end = curSegments;
        int curIndex = 0;
        float halfWidth = width / 2.0f;
        float temDis = 0;
        float outerWidth = 0;
        bool hasChangeHead = false;
        bool endChange = true;
        showBeginIndex = Mathf.Max(0, showBeginIndex);
        curSegments = Mathf.Max(1, curSegments);
        SetCurvePoints();
        if (useFast) {
            for (int i = start; i < end; i++) {
                if (i >= showBeginIndex) {
                    curIndex = i - showBeginIndex;

                    if (useHeadBorder && hasChangeHead == false) {
                        temDis = Vector2.Distance(m_points2[i], m_points2[i + 1]);
                        outerWidth = (uv.z - headerUv.x) * this.mainTexture.width;
                        endChange = temDis > outerWidth;
                        if (endChange) {
                            hasChangeHead = true;
                            headerUv2 = uv;
                        } else {
                            headerUv2.z = headerUv.x + temDis / this.mainTexture.width;
                        }
                        vh.AddVert(new Vector2(m_points2[i].x, m_points2[i].y - halfWidth), color32, new Vector2(headerUv.x, headerUv.y));
                        vh.AddVert(new Vector2(m_points2[i].x, m_points2[i].y + halfWidth), color32, new Vector2(headerUv.x, headerUv2.w));
                        vh.AddVert(new Vector2(m_points2[i + 1].x, m_points2[i + 1].y + halfWidth), color32, new Vector2(headerUv2.z, headerUv2.w));
                        vh.AddVert(new Vector2(m_points2[i + 1].x, m_points2[i + 1].y - halfWidth), color32, new Vector2(headerUv2.z, headerUv.y));
                        if (endChange == false) {
                            headerUv.x = headerUv2.z;
                        }
                    } else {
                        vh.AddVert(new Vector2(m_points2[i].x, m_points2[i].y - halfWidth), color32, new Vector2(uv.x, uv.y));
                        vh.AddVert(new Vector2(m_points2[i].x, m_points2[i].y + halfWidth), color32, new Vector2(uv.x, uv.w));
                        vh.AddVert(new Vector2(m_points2[i + 1].x, m_points2[i + 1].y + halfWidth), color32, new Vector2(uv.z, uv.w));
                        vh.AddVert(new Vector2(m_points2[i + 1].x, m_points2[i + 1].y - halfWidth), color32, new Vector2(uv.z, uv.y));
                    }

                    vh.AddTriangle(4 * curIndex + 0, 4 * curIndex + 1, 4 * curIndex + 2);
                    vh.AddTriangle(4 * curIndex + 2, 4 * curIndex + 3, 4 * curIndex + 0);
                }
            }
        } else {
            SetLineVertices();
            Vector2 p1 = Vector2.zero, p2 = Vector2.zero, v1 = Vector2.zero, px = Vector2.zero;
            int idx = 0, widthIdx = 0;
            int widthIdxAdd = 0;
            float normalizedDistance = 0.0f;
            for (int i = start; i < end; i++) {
                p1.x = m_points2[i].x; p1.y = m_points2[i].y;
                p2.x = m_points2[i + 1].x; p2.y = m_points2[i + 1].y;
                if (p1.x == p2.x && p1.y == p2.y) {
                    m_lineVertices[idx] = Vector2.zero;
                    m_lineVertices[idx + 1] = Vector2.zero;
                    m_lineVertices[idx + 2] = Vector2.zero;
                    m_lineVertices[idx + 3] = Vector2.zero;

                    idx += 4;
                    widthIdx += widthIdxAdd;
                    continue;
                }

                px.x = p2.y - p1.y; px.y = p1.x - p2.x;
                normalizedDistance = (1.0f / (float)System.Math.Sqrt((px.x * px.x) + (px.y * px.y)));
                px *= normalizedDistance * halfWidth;
                m_lineVertices[idx].x = p1.x - px.x; m_lineVertices[idx].y = p1.y - px.y;
                m_lineVertices[idx + 3].x = p1.x + px.x; m_lineVertices[idx + 3].y = p1.y + px.y;
                m_lineVertices[idx + 2].x = p2.x + px.x; m_lineVertices[idx + 2].y = p2.y + px.y;
                m_lineVertices[idx + 1].x = p2.x - px.x; m_lineVertices[idx + 1].y = p2.y - px.y;
                idx += 4;
                widthIdx += widthIdxAdd;
            }
            if (useWeld) {
                for (int i = 4; i < end * 4; i += 4) {
                    SetIntersectionPoint(i - 4, i - 3, i, i + 1);
                    SetIntersectionPoint(i - 1, i - 2, i + 3, i + 2);
                }
            }
            
            for (int i = start; i < end; i++) {
                if (i >= showBeginIndex) {
                    curIndex = i - showBeginIndex;
                    if (useHeadBorder && hasChangeHead == false) {
                        temDis = Vector2.Distance(m_points2[i], m_points2[i + 1]);
                        outerWidth = (uv.z - headerUv.x) * this.mainTexture.width;
                        endChange = temDis > outerWidth;
                        if (endChange) {
                            hasChangeHead = true;
                            headerUv2 = uv;
                        } else {
                            headerUv2.z = headerUv.x + temDis / this.mainTexture.width;
                        }
                        vh.AddVert(m_lineVertices[4 * i + 3], color32, new Vector2(headerUv.x, headerUv.y));
                        vh.AddVert(m_lineVertices[4 * i + 0], color32, new Vector2(headerUv.x, headerUv2.w));
                        vh.AddVert(m_lineVertices[4 * i + 1], color32, new Vector2(headerUv2.z, headerUv2.w));
                        vh.AddVert(m_lineVertices[4 * i + 2], color32, new Vector2(headerUv2.z, headerUv.y));
                        if (endChange == false) {
                            headerUv.x = headerUv2.z;
                        }
                    } else {
                        vh.AddVert(m_lineVertices[4 * i + 3], color32, new Vector2(uv.x, uv.y));
                        vh.AddVert(m_lineVertices[4 * i + 0], color32, new Vector2(uv.x, uv.w));
                        vh.AddVert(m_lineVertices[4 * i + 1], color32, new Vector2(uv.z, uv.w));
                        vh.AddVert(m_lineVertices[4 * i + 2], color32, new Vector2(uv.z, uv.y));
                    }
                    vh.AddTriangle(4 * curIndex + 0, 4 * curIndex + 1, 4 * curIndex + 2);
                    vh.AddTriangle(4 * curIndex + 2, 4 * curIndex + 3, 4 * curIndex + 0);
                }
            }
        }
    }

    private static Vector2 GetBezierPoint(ref Vector2 anchor1, ref Vector2 control1, ref Vector2 anchor2, ref Vector2 control2, float t) {
        float cx = 3 * (control1.x - anchor1.x);
        float bx = 3 * (control2.x - control1.x) - cx;
        float ax = anchor2.x - anchor1.x - cx - bx;
        float cy = 3 * (control1.y - anchor1.y);
        float by = 3 * (control2.y - control1.y) - cy;
        float ay = anchor2.y - anchor1.y - cy - by;

        return new Vector2((ax * (t * t * t)) + (bx * (t * t)) + (cx * t) + anchor1.x,
                            (ay * (t * t * t)) + (by * (t * t)) + (cy * t) + anchor1.y);
    }

    private void SetIntersectionPoint(int p1, int p2, int p3, int p4) {
        var l1a = m_lineVertices[p1]; var l1b = m_lineVertices[p2];
        var l2a = m_lineVertices[p3]; var l2b = m_lineVertices[p4];
        if ((l1a.x == l1b.x && l1a.y == l1b.y) || (l2a.x == l2b.x && l2a.y == l2b.y)) return;

        float d = (l2b.y - l2a.y) * (l1b.x - l1a.x) - (l2b.x - l2a.x) * (l1b.y - l1a.y);
        if (d > -0.005f && d < 0.005f) {	// Sometimes nearly parallel lines have errors, so just average the points together
            if (Mathf.Abs(l1b.x - l2a.x) < .005f && Mathf.Abs(l1b.y - l2a.y) < .005f) {	// But only if the points are mostly the same
                m_lineVertices[p2] = (l1b + l2a) * 0.5f;
                m_lineVertices[p3] = m_lineVertices[p2];
            }
            return;	// Otherwise that means the line is going back on itself, so do nothing
        }
        float n = ((l2b.x - l2a.x) * (l1a.y - l2a.y) - (l2b.y - l2a.y) * (l1a.x - l2a.x)) / d;

        var v3 = new Vector2(l1a.x + (n * (l1b.x - l1a.x)), l1a.y + (n * (l1b.y - l1a.y)));
        if ((v3 - l1b).sqrMagnitude > (width * width)) return;
        m_lineVertices[p2] = v3;
        m_lineVertices[p3] = v3;
    }
}
