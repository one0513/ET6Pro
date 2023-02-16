using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardRender : BaseRender {

    private Graphic[] alphaItems;
    private Color tempCol;

    public float PosY {
        get {
            return SelfTransform.anchoredPosition.y;
        }
    }

    public void SetScale(float scale) {
        if (SelfTransform.localScale.x != scale) {
            SelfTransform.localScale = Vector3.one * scale;
        }
    }

    public void SetPos(Vector2 pos) {
        if (SelfTransform.anchoredPosition.x != pos.x || SelfTransform.anchoredPosition.y != pos.y) {
            SelfTransform.anchoredPosition = pos;
        }
    }

    public void AddPos(Vector2 pos) {
        if (pos.y != 0f) {
            SelfTransform.anchoredPosition += pos;
        }
    }

    public void SetAlpha(float alpha) {
        if (alphaItems == null) {
            alphaItems = SelfTransform.GetComponentsInChildren<Graphic>();
        }
        for (int i = 0; i < alphaItems.Length; i++) {
            tempCol = alphaItems[i].color;
            tempCol.a = alpha;
            alphaItems[i].color = tempCol;
        }
    }
}
