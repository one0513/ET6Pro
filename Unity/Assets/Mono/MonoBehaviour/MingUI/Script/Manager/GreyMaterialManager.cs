//#define USE_GRRY_UNIT
using System;
using System.Collections.Generic;
using UnityEngine;

public class GreyMaterialManager {

    private static Material defaultGreyMat;
    private static Material customDefaultMat;
    public static Color greyColor = new Color(1 / 255f, 0,0,1.0f);
    public static Color greyTextColor = new Color(12 / 255f, 12 / 255f, 12 / 255f, 1.0f);

    public static Material GetGreyMaterial() {
        if (defaultGreyMat == null) {
            defaultGreyMat = new Material(Shader.Find("UI/Gray"));
            defaultGreyMat.hideFlags = HideFlags.DontSave | HideFlags.NotEditable;
        }
        return defaultGreyMat;
    }

    public static Material GetCustomDefaultMaterial() {
        #if USE_GRRY_UNIT
        return GetGreyMaterial();
        #else
        if (customDefaultMat == null) {
            customDefaultMat = new Material(Shader.Find("UI/ETC1Default"));
        }
        return customDefaultMat;
        #endif
    }
}