using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CSpriteGhost : CSprite {

    public int maxGhostNum = 3;
    public float sampleTimeGap = 0;
    public float sampleScale = 1;
    public RectTransform parentTransform;

    protected float lastSampleTime = 0;
    protected bool isDuringGhost = false;
    protected List<Vector2> _recordList = new List<Vector2>();


    protected void SetRecordPos() {
        if (parentTransform != null) {
            _recordList.Add(parentTransform.anchoredPosition);
        } else {
            _recordList.Add(this.rectTransform.anchoredPosition);
        }
    }

    public void BeginSample() {
        SetRecordPos();
        isDuringGhost = true;
        lastSampleTime = Time.time;
    }

    public void EndSample() {
        _recordList.Clear();
        isDuringGhost = false;
    }

    public void AddSample() {
        if (Time.time - lastSampleTime > sampleTimeGap) {
            lastSampleTime = Time.time;
            if (_recordList.Count >= maxGhostNum) {
                _recordList.RemoveAt(0);
            }
            SetRecordPos();
            SetAllDirty();
        }
    }

    /// <summary>
    /// Update the UI renderer mesh.
    /// </summary>
    protected override void OnPopulateMesh(VertexHelper toFill) {
        if (overrideSprite == null) {
            base.OnPopulateMesh(toFill);
            return;
        }
        GenerateSimpleSprite(toFill);
    }

    /// <summary>
    /// Generate vertices for a simple Image.
    /// </summary>
    protected void GenerateSimpleSprite(VertexHelper vh) {
        Vector4 v = GetDrawingDimensions();
        var uv = (overrideSprite != null) ? UnityEngine.Sprites.DataUtility.GetOuterUV(overrideSprite) : Vector4.zero;

        var color32 = color;
        vh.Clear();

        if (isDuringGhost) {
            for (int i = 0; i < _recordList.Count; i++) {
                Vector2 offset = parentTransform == null ? _recordList[i] - this.rectTransform.anchoredPosition  : _recordList[i] - parentTransform.anchoredPosition;
                color32.a = 1.0f / (_recordList.Count - i);

                vh.AddVert(new Vector2(v.x + offset.x * sampleScale, v.y + offset.y), color32, new Vector2(uv.x, uv.y));
                vh.AddVert(new Vector2(v.x + offset.x * sampleScale, v.w + offset.y), color32, new Vector2(uv.x, uv.w));
                vh.AddVert(new Vector2(v.z + offset.x * sampleScale, v.w + offset.y), color32, new Vector2(uv.z, uv.w));
                vh.AddVert(new Vector2(v.z + offset.x * sampleScale, v.y + offset.y), color32, new Vector2(uv.z, uv.y));

                vh.AddTriangle(4 * i + 0, 4 * i + 1, 4 * i + 2);
                vh.AddTriangle(4 * i + 2, 4 * i + 3, 4 * i + 0);
            }
        } else {
            vh.AddVert(new Vector2(v.x, v.y), color32, new Vector2(uv.x, uv.y));
            vh.AddVert(new Vector2(v.x, v.w), color32, new Vector2(uv.x, uv.w));
            vh.AddVert(new Vector2(v.z, v.w), color32, new Vector2(uv.z, uv.w));
            vh.AddVert(new Vector2(v.z, v.y), color32, new Vector2(uv.z, uv.y));

            vh.AddTriangle(0, 1, 2);
            vh.AddTriangle(2, 3, 0);
        }
    }

    /// Image's dimensions used for drawing. X = left, Y = bottom, Z = right, W = top.
    private Vector4 GetDrawingDimensions() {
        var padding = overrideSprite == null ? Vector4.zero : UnityEngine.Sprites.DataUtility.GetPadding(overrideSprite);
        var size = overrideSprite == null ? Vector2.zero : new Vector2(overrideSprite.rect.width, overrideSprite.rect.height);

        int spriteW = Mathf.RoundToInt(size.x);
        int spriteH = Mathf.RoundToInt(size.y);

        Rect r = GetPixelAdjustedRect();
        // Debug.Log(string.Format("r:{2}, size:{0}, padding:{1}", size, padding, r));
        r.x += (rectTransform.sizeDelta.x - spriteW) / 2;
        r.y += (rectTransform.sizeDelta.y - spriteH) / 2;

        var v = new Vector4(
                padding.x / spriteW,
                padding.y / spriteH,
                (spriteW - padding.z) / spriteW,
                (spriteH - padding.w) / spriteH);

        v = new Vector4(
                r.x + spriteW * v.x,
                r.y + spriteH * v.y,
                r.x + spriteW * v.z,
                r.y + spriteH * v.w
                );

        return v;
    }
}