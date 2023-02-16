using UnityEngine;
using UnityEngine.UI;

public class CSpriteQueue : CSprite {

    public bool isFlipX = false;
    public int queueNum = 3;
    public int queueCount = 1;
    public float queueRotation = 0;
    public Vector2 queueGap = Vector2.zero;
    public Vector2 queueSize = Vector2.one;

    public void SetFlipX(bool isFilp) {
        if (isFlipX != isFilp) {
            isFlipX = isFilp;
            //SetAllDirty();
        }
    }

    public void SetQueueNum(int num) {
        if (queueNum != num) {
            queueNum = num;
            //SetAllDirty();
        }
    }

    public void SetQueueCount(int count) {
        if (queueCount != count) {
            queueCount = count;
            //SetAllDirty();
        }
    }

    public void SetQueueGap(Vector2 gap) {
        if (queueGap != gap) {
            queueGap = gap;
            //SetAllDirty();
        }
    }

    public void SetQueueSize(Vector2 size) {
        if (queueSize != size) {
            queueSize = size;
            //SetAllDirty();
        }
    }

    public void SetQueueRotation(float rotation) {
        queueRotation = rotation;
        //SetAllDirty();
    }

    /// <summary>
    /// Update the UI renderer mesh.
    /// </summary>
    protected override void OnPopulateMesh(VertexHelper toFill) {
        if (overrideSprite == null) {
            base.OnPopulateMesh(toFill);
            return;
        }

        GenerateSimpleSprite(toFill, preserveAspect);
    }

    /// <summary>
    /// Generate vertices for a simple Image.
    /// </summary>
    void GenerateSimpleSprite(VertexHelper vh, bool lPreserveAspect) {
        var uv = (overrideSprite != null) ? UnityEngine.Sprites.DataUtility.GetOuterUV(overrideSprite) : Vector4.zero;
        var color32 = color;
        vh.Clear();

        float maxWidthLine = (queueNum - 1) * queueSize.x + (queueNum - 1) * queueGap.x;
        float maxHeightLine = (queueNum - 1) * queueSize.y + (queueNum - 1) * queueGap.y;
        float maxWidthCol = (queueCount - 1) * queueSize.x + (queueCount - 1) * queueGap.x;
        float maxHeightCol = (queueCount - 1) * queueSize.y + (queueCount - 1) * queueGap.y;

        int startIndex = 0;
        for (int j = 0; j < queueCount; j++) {

            float dx = (maxWidthCol / -2.0f + j * queueSize.x + j * queueGap.x) * Mathf.Cos((queueRotation + 90) * Mathf.Deg2Rad);
            float dy = (maxHeightCol / -2.0f + j * queueSize.y + j * queueGap.y) * Mathf.Sin((queueRotation + 90) * Mathf.Deg2Rad);

            for (int i = 0; i < queueNum; i++) {
                float lx = (maxWidthLine / -2.0f + i * queueSize.x + i * queueGap.x) * Mathf.Cos(queueRotation * Mathf.Deg2Rad) + dx;
                float by = (maxHeightLine / -2.0f + i * queueSize.y + i * queueGap.y) * Mathf.Sin(queueRotation * Mathf.Deg2Rad) + dy;

                if (isFlipX == false) {
                    vh.AddVert(new Vector2(lx - this.Width / 2, by - this.Height / 2), color32, new Vector2(uv.x, uv.y));
                    vh.AddVert(new Vector2(lx - this.Width / 2, by + this.Height / 2), color32, new Vector2(uv.x, uv.w));
                    vh.AddVert(new Vector2(lx + this.Width / 2, by + this.Height / 2), color32, new Vector2(uv.z, uv.w));
                    vh.AddVert(new Vector2(lx + this.Width / 2, by - this.Height / 2), color32, new Vector2(uv.z, uv.y));
                } else {
                    vh.AddVert(new Vector2(lx - this.Width / 2, by - this.Height / 2), color32, new Vector2(uv.z, uv.y));
                    vh.AddVert(new Vector2(lx - this.Width / 2, by + this.Height / 2), color32, new Vector2(uv.z, uv.w));
                    vh.AddVert(new Vector2(lx + this.Width / 2, by + this.Height / 2), color32, new Vector2(uv.x, uv.w));
                    vh.AddVert(new Vector2(lx + this.Width / 2, by - this.Height / 2), color32, new Vector2(uv.x, uv.y));
                }

                startIndex = 4 * (i + j * queueNum);
                vh.AddTriangle(startIndex + 0, startIndex + 1, startIndex + 2);
                vh.AddTriangle(startIndex + 2, startIndex + 3, startIndex + 0);
            }
        }
    }
}
