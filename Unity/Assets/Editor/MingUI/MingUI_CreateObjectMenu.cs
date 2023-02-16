using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public  static class MingUI_CreateObjectMenu
{
   
    [MenuItem("GameObject/UI/lable", false, 2001)]
    static void AddCLable(MenuCommand menuCommand)
    {
        GameObject go = new GameObject();
        GameObject parent = Selection.activeGameObject;
        go.transform.SetParent(parent.transform,false);
        go.transform.SetAsLastSibling();
        go.AddComponent<RectTransform>();
        go.AddComponent<CanvasRenderer>();
        CMeshProLabel label = go.AddComponent<CMeshProLabel>();
        label.alignment = TextAlignmentOptions.Center;
        CMeshProEffectUGUI eff = go.AddComponent<CMeshProEffectUGUI>();
        eff.text = label;
        label.text = "New lable";
        go.name = "lbl";
    }
    
    [MenuItem("GameObject/UI/sprite", false, 2001)]
    static void AddCSprite(MenuCommand menuCommand)
    {
        GameObject go = new GameObject();
        GameObject parent = Selection.activeGameObject;
        go.transform.SetParent(parent.transform,false);
        go.transform.SetAsLastSibling();
        go.AddComponent<RectTransform>();
        go.AddComponent<CanvasRenderer>();
        CSprite label = go.AddComponent<CSprite>();

        go.name = "sp";
    }
    
    [MenuItem("GameObject/UI/button", false, 2001)]
    static void AddCButton(MenuCommand menuCommand)
    {
        GameObject go = new GameObject();
        GameObject parent = Selection.activeGameObject;
        go.transform.SetParent(parent.transform,false);
        go.transform.SetAsLastSibling();
        go.AddComponent<RectTransform>();
        go.AddComponent<CanvasRenderer>();
        CButton btn = go.AddComponent<CButton>();
        btn.targetGraphic = go.AddComponent<CSprite>();
        go.name = "btn";
        
        GameObject go3 = new GameObject();
        go3.transform.SetParent(go.transform,false);
        go3.transform.SetAsLastSibling();
        RectTransform go3Rect = go3.AddComponent<RectTransform>();
        go3.AddComponent<CanvasRenderer>();
        SetTrctAnchorCenter(go3Rect);
        CSprite sp  = go3.AddComponent<CSprite>();
        go3.name = "sp";
        sp.Atlas = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/AssetsPackage/UI/UICommon/Prefabs/UICommon.prefab").GetComponent<CAtlas>();
        sp.SpriteName = "Blue_gradient";
                
        GameObject go2 = new GameObject();
        go2.transform.SetParent(go.transform,false);
        go2.transform.SetAsLastSibling();
        RectTransform go2Rect = go2.AddComponent<RectTransform>();
        go2.AddComponent<CanvasRenderer>();
        SetTrctAnchorCenter(go2Rect);
        CMeshProLabel label = go2.AddComponent<CMeshProLabel>();
        label.alignment = TextAlignmentOptions.Center;
        CMeshProEffectUGUI eff = go2.AddComponent<CMeshProEffectUGUI>();
        eff.text = label;
        label.text = "button";
        go2.name = "lbl";
        
        

        
        btn.label = label;
        btn.icon = sp;

    }
    
    
    
    static void SetTrctAnchorCenter(RectTransform rectTransform)
    {
        rectTransform.anchorMax = Vector2.one;
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.sizeDelta = Vector2.zero;
    }
    
}
