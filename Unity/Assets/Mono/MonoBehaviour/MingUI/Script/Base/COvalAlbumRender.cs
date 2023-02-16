using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class COvalAlbumRender : BaseRender {

    public void SetPos(Vector2 pos) {
        if (SelfTransform.anchoredPosition.x != pos.x || SelfTransform.anchoredPosition.y != pos.y) {
            SelfTransform.anchoredPosition = pos;
        }
    }
}
