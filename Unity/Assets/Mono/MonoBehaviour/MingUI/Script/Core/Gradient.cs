using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gradient1:BaseMeshEffect
{
    [SerializeField]
    private Color32 _topColor = Color.white;//上渐变

    [SerializeField]
    private Color32 _bottomColor = Color.black;//下渐变

    public Color TopColor
    {
        get { return _topColor; }
        set
        {
            _topColor = value;
            if (graphic != null)
                graphic.SetVerticesDirty();
        }
    }

    public Color BottomColor
    {
        get { return _bottomColor; }
        set
        {
            _bottomColor = value;
            if (graphic != null)
                graphic.SetVerticesDirty();
        }
    }

    public override void ModifyMesh(VertexHelper vh)
    {
        if (!IsActive())
        {
            return;
        }

        var vertexList = new List<UIVertex>();
        vh.GetUIVertexStream(vertexList);
        int count = vertexList.Count;
        if (count == 0)
        {
            return;
        }
        ApplyGradient(vertexList, 0, count);
        vh.Clear();
        vh.AddUIVertexTriangleStream(vertexList);
    }

    private void ApplyGradient(List<UIVertex> vertexList, int start, int end)
    {
        float bottomY = vertexList[0].position.y;
        float topY = vertexList[0].position.y;
        for (int i = start; i < end; ++i)
        {
            float y = vertexList[i].position.y;
            if (y > topY)
            {
                topY = y;
            }
            else if (y < bottomY)
            {
                bottomY = y;
            }
        }

        float uiElementHeight = topY - bottomY;
        for (int i = start; i < end; ++i)
        {
            UIVertex uiVertex = vertexList[i];
            uiVertex.color = Color32.Lerp(BottomColor, TopColor, (uiVertex.position.y - bottomY) / uiElementHeight);
            vertexList[i] = uiVertex;
        }
    }
}
