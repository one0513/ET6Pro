using System;
using Assets.Editor;
using System.IO;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

public class TextureAlphaDepartor : MonoBehaviour {

    public const string PLAT_FORM = "Android";
    public const string ALPHA_TEX_POSTFIX = "_alpha.png";

    [Obsolete("Obsolete")]
    //[MenuItem("Assets/贴图通道分离（CTexture）", false, 1)]
    public static void OnDepartTextrue() {
        GUI_ShowProgress("开始处理", 0);
        Object[] selectobj = Selection.GetFiltered(typeof(Texture), SelectionMode.Assets);
        for (int i = 0; i < selectobj.Length; i++) {
            string path = AssetDatabase.GetAssetPath(selectobj[i]);
            if (path.EndsWith(".png")) {
                DepartTextureAlphaChannel(path);
            } else {
                Debug.LogError(string.Format("仅支持 .png 文件后缀 [{0}]", path));
            }
            GUI_ShowProgress(string.Format("处理中...({0}/{1})", i + 1, selectobj.Length), ((i + 1.0f) / selectobj.Length));
        }
        GUI_ShowProgress("处理完毕", 1);
    }

    [Obsolete("Obsolete")]
    public static void DepartTextureAlphaChannel(string mainTexPath) {
        //自动处理先关闭
        // bool postOpen = AssetBuilder.OpenPostprocess;
        // AssetBuilder.OpenPostprocess = false;
        string alphTexPath = mainTexPath.Replace(".png", ALPHA_TEX_POSTFIX);
        if (File.Exists(Application.dataPath.Replace("Assets", alphTexPath))) {
            File.Copy(Application.dataPath.Replace("Assets", mainTexPath), Application.dataPath.Replace("Assets", alphTexPath), true);
        } else {
            AssetDatabase.CopyAsset(mainTexPath, alphTexPath);
        }
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        //把格式给搞一搞
        ChangeTextureImportModule(mainTexPath);
        ChangeTextureImportModule(alphTexPath, true);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        //透明通道的再改一下
        TranAlphaChannelToRedChannel(alphTexPath);
        //还原成不可读
        SetTextureUnReadable(alphTexPath);
        //自动处理还原
        //AssetBuilder.OpenPostprocess = postOpen;
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
    }

    public static void TranAlphaChannelToRedChannel(string alphaTexPath) {
        Texture2D mainTex = AssetDatabase.LoadAssetAtPath(alphaTexPath, typeof(Texture)) as Texture2D;
        Texture2D newTex = new Texture2D(mainTex.width, mainTex.height);
        Color[] tempCol = mainTex.GetPixels();
        for (int i = 0; i < tempCol.Length; i++) {
            tempCol[i].r = tempCol[i].a;
            tempCol[i].g = tempCol[i].a;
            tempCol[i].b = tempCol[i].a;
        }
        newTex.SetPixels(tempCol);
        newTex.Apply();
        File.WriteAllBytes(Application.dataPath.Replace("Assets", alphaTexPath), newTex.EncodeToPNG());
    }

    [Obsolete("Obsolete")]
    private static void ChangeTextureImportModule(string fileName, bool alpha = false) {
        TextureImporter ti = (TextureImporter)AssetImporter.GetAtPath(fileName);
        if (ti != null) {
            ti.textureShape = TextureImporterShape.Texture2D;
            ti.mipmapEnabled = false;
            ti.alphaIsTransparency = true;
            ti.wrapMode = TextureWrapMode.Clamp;
            ti.filterMode = FilterMode.Bilinear;
            ti.maxTextureSize = 2048;
            if (alpha) {
                ti.isReadable = true;
                ti.npotScale = TextureImporterNPOTScale.None;
                ti.SetPlatformTextureSettings(PLAT_FORM, ti.maxTextureSize, TextureImporterFormat.RGBA32);
            } else {
                ti.isReadable = false;
                ti.npotScale = TextureImporterNPOTScale.ToNearest;
                ti.SetPlatformTextureSettings(PLAT_FORM, ti.maxTextureSize, TextureImporterFormat.ETC_RGB4);
            }
            AssetDatabase.ImportAsset(fileName);
        }
    }
    [Obsolete("Obsolete")]
    public static void SetTextureUnReadable(string fileName) {
        TextureImporter ti = (TextureImporter)AssetImporter.GetAtPath(fileName);
        if (ti != null) {
            ti.isReadable = false;
            ti.npotScale = TextureImporterNPOTScale.ToNearest;
            ti.SetPlatformTextureSettings(PLAT_FORM, ti.maxTextureSize, TextureImporterFormat.ETC_RGB4);
            AssetDatabase.ImportAsset(fileName);
        }
    }

    private static void GUI_ShowProgress(string msg, float progress) {
        EditorUtility.DisplayProgressBar("处理进度", msg, progress);
        if (progress >= 1) {
            EditorUtility.ClearProgressBar();
        }
    }
}
