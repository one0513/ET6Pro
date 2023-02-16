//#define USE_GRRY_UNIT
//#define USE_ETC_1
using System;
using UnityEngine;
using UnityEngine.UI;

public class GraphicGrey : Graphic {

    protected override void Awake() {
        base.Awake();
        UseCustomDefaultMat();
    }

    public static void UseDefaultMat() {
        s_DefaultUI = Canvas.GetDefaultCanvasMaterial();
    }

    public static void UseCustomDefaultMat() {
        #if USE_GRRY_UNIT
        s_DefaultUI = GreyMaterialManager.GetGreyMaterial();
        #elif USE_ETC_1
        s_DefaultUI = GreyMaterialManager.GetCustomDefaultMaterial();
        #endif
    }
}