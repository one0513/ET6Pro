//#define USE_SPRITE_PACKER
//#define USE_ETC_1
using Assets.Editor;
using Assets.Editor.ComTool;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

public enum TexturePackerCMDType
{
    Normal,
    BigMap,
    ArtFont,
    SpriteAnim,
}


public class TexturePacker : MonoBehaviour {
    public const string JIANTI = "Jianti";//简体资源
    public const string FANTI = "Fanti";//繁体资源

    public const string MAIN_TEX_POSTFIX = "_Tex.png";
    public const string ALPHA_TEX_POSTFIX = "_Tex_alpha.png";

    ///  "Default","Web","Standalone","iPhone","Android"
    public const string PLAT_FORM = "Android";
    #if UNITY_EDITOR_OSX
        public const string TEXTURE_CMD = "/Applications/TexturePacker.app/Contents/MacOS/TexturePacker";
    #else
        public const string TEXTURE_CMD = "C:/Program Files/CodeAndWeb/TexturePacker/bin/TexturePacker.exe";
    #endif

    private static TexturePackerCMDType cmdType = TexturePackerCMDType.Normal;
    /// <summary>
    /// 选择模块进行打包
    /// </summary>
    /// 
    #if USE_SPRITE_PACKER
    #else
    [MenuItem("Assets/按模块打包图集", false, 1100)]
    #endif
    [Obsolete("Obsolete")]
    public static void Select() {
        Object[] selectobj = Selection.GetFiltered(typeof(object), SelectionMode.Assets);
        Select(selectobj[0].name);
    }
    [Obsolete("Obsolete")]
    public static void Select(string name) {
        cmdType = TexturePackerCMDType.Normal;
        PackSelect("Assets/AssetsPackage/UI/" + name);
    }

    
    
    [Obsolete("Obsolete")]
    public static void PackSelect(string modulePath, string resType = FANTI) {
        if (!ModuleHasTexture(modulePath, "Atlas_" + resType)) {
            EditorUtility.DisplayDialog("温馨提示", "模块无可打包图片" + modulePath, "确定");
            return;
        }
        string moduleName = Path.GetFileNameWithoutExtension(modulePath);
        string prefabPath = modulePath + "/Prefabs/" + moduleName + ".prefab";
        StartPacket(modulePath, moduleName, prefabPath, "Atlas_" + resType);
    }

    [Obsolete("Obsolete")]
    public static void StartPacket(string modulePath, string moduleName, string prefabPath, string extAtlasPath) {
        GameObject go = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject)) as GameObject;
        if (go == null) // 第一次创建，先生成预设体
        {
            go = new GameObject(moduleName);
            go.AddComponent<CAtlas>();
            PrefabUtility.SaveAsPrefabAsset(go, prefabPath);
            DestroyImmediate(go);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        if (ExePackerCmd(modulePath, extAtlasPath, moduleName)) {
            //自动处理先关闭
            // bool postOpen = AssetBuilder.OpenPostprocess;
            // AssetBuilder.OpenPostprocess = false;
            string mainTexPath = prefabPath.Replace(".prefab", MAIN_TEX_POSTFIX);
            //#define USE_ETC_1
            //string alphTexPath = prefabPath.Replace(".prefab", ALPHA_TEX_POSTFIX);
            //if (File.Exists(Application.dataPath.Replace("Assets", alphTexPath))) {
            //    File.Copy(Application.dataPath.Replace("Assets", mainTexPath), Application.dataPath.Replace("Assets", alphTexPath), true);
            //} else {
            //    AssetDatabase.CopyAsset(mainTexPath, alphTexPath);
            //}
            //#endif
            //此时已经把图集打完了
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            //#define USE_ETC_1
            //优先把透明的打包了，然后打包的时候不要刷新sprite，避免报错
            //if (ImportTheTpAlpha(prefabPath)) {
            //    Debug.Log("[Success][Alpha]:" + moduleName);
            //}
            //#endif
            if (ImportTheTp(prefabPath))//解析json转换为spritesheet
            {
                Debug.Log("[Success]:" + moduleName);
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            //#define USE_ETC_1
            //透明通道的再改一下
            //TranAlphaChannelToRedChannel(alphTexPath);
            //还原成不可读
            //SetTextureUnReadable(alphTexPath);
            //#endif
            //自动处理还原
            //AssetBuilder.OpenPostprocess = postOpen;
            AssetDatabase.ImportAsset(mainTexPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        }
    }
    [Obsolete("Obsolete")]
    private static bool ImportTheTp(string prefabPath,bool irregular = false) {
        GameObject go = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject)) as GameObject;
        if (go != null) {
            CAtlas mAtlas = go.GetComponent<CAtlas>();

            string mainTexPath = prefabPath.Replace(".prefab", MAIN_TEX_POSTFIX);
            Texture2D mainTex = AssetDatabase.LoadAssetAtPath(mainTexPath, typeof(Texture)) as Texture2D;

            string jsonTxtPaht = prefabPath.Replace(".prefab", "_Tex.txt");
            TextAsset ta = AssetDatabase.LoadAssetAtPath(jsonTxtPaht, typeof(TextAsset)) as TextAsset;

            if (mAtlas == null || mainTex == null || ta == null) {
                Debug.Log("[Fail]：" + prefabPath);
                return false;
            }

            mAtlas.mainTexture = mainTex;//设置主贴图
            List<SpriteMetaData> sprites = ProcessToSprites(mAtlas, ta.text, mainTexPath, irregular);//解析子图元
            ChangeTextureImportModule(mainTexPath, sprites);//设置贴图格式

            mAtlas.spriteList.Clear();
            Object[] list = AssetDatabase.LoadAllAssetsAtPath(mainTexPath);
            foreach (Object asset in list) {
                Sprite sp = asset as Sprite;
                if (sp != null)//碎图
                {
                    mAtlas.AddSprite(sp);
                }
            }
            mAtlas.spriteList.Sort(CompareSprites);
            //需要重新整理里面dic的内容，避免重启Unity才可以获取新的内容
            mAtlas.ClearSpirteDic();
            mAtlas.MarkAsChanged();
            Selection.activeGameObject = mAtlas.gameObject;
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        return true;
    }

    [Obsolete("Obsolete")]
    private static bool ImportTheTpAlpha(string prefabPath, bool irregular = false) {
        GameObject go = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject)) as GameObject;
        if (go != null) {
            CAtlas mAtlas = go.GetComponent<CAtlas>();

            string mainTexPath = prefabPath.Replace(".prefab", ALPHA_TEX_POSTFIX);
            Texture2D mainTex = AssetDatabase.LoadAssetAtPath(mainTexPath, typeof(Texture)) as Texture2D;

            string jsonTxtPaht = prefabPath.Replace(".prefab", "_Tex.txt");
            TextAsset ta = AssetDatabase.LoadAssetAtPath(jsonTxtPaht, typeof(TextAsset)) as TextAsset;

            if (mAtlas == null || mainTex == null || ta == null) {
                Debug.Log("[Fail]：" + prefabPath);
                return false;
            }

            //#define USE_ETC_1
            //mAtlas.alphaTexture = mainTex;//设置透明贴图
            //#endif
            List<SpriteMetaData> sprites = ProcessToSprites(mAtlas, ta.text, mainTexPath, irregular, true);//解析子图元
            ChangeTextureImportModule(mainTexPath, sprites, true);//设置贴图格式

            //#define USE_ETC_1
            //mAtlas.spriteAlphaList.Clear();
            //#endif
            Object[] list = AssetDatabase.LoadAllAssetsAtPath(mainTexPath);
            foreach (Object asset in list) {
                Sprite sp = asset as Sprite;
                if (sp != null)//碎图
                {
                    mAtlas.AddSprite(sp,true);
                }
            }
            //#define USE_ETC_1
            //mAtlas.spriteAlphaList.Sort(CompareSprites);
            //#endif

            //mAtlas.MarkAsChanged();//rgb贴图的时候才更新，太早更新会报错
            Selection.activeGameObject = mAtlas.gameObject;
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        return true;
    }

    static void TranAlphaChannelToRedChannel(string alphaTexPath) {
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
        File.WriteAllBytes(Application.dataPath.Replace("Assets", alphaTexPath) , newTex.EncodeToPNG());  
    }

    static int CompareSprites(Sprite a, Sprite b) { return String.Compare(a.name, b.name, StringComparison.Ordinal); }
    /// <summary>
    /// 将json解析成spriteMetaData
    /// </summary>
    /// <param name="mAtlas"></param>
    /// <param name="text"></param>
    /// <param name="texturePath"></param>
    /// <returns></returns>
    private static List<SpriteMetaData> ProcessToSprites(CAtlas mAtlas, string text, string texturePath,bool irrgular = false,bool alpha = false) {
        Hashtable decodedHash = text.hashtableFromJson();
        TextureImporter asetImp = AssetImporter.GetAtPath(texturePath) as TextureImporter;
        if (asetImp == null) return null;
        List<SpriteMetaData> oldSprites = new List<SpriteMetaData>(asetImp.spritesheet);//老的
        List<SpriteMetaData> newSprites = new List<SpriteMetaData>();//新的
        //格式：
        //"fulin.png":
        //{
        //  "frame": { "x":106,"y":196,"w":61,"h":32},
        //	"rotated": true,
        //	"trimmed": false,
        //	"spriteSourceSize": { "x":0,"y":0,"w":61,"h":32},
        //	"sourceSize": { "w":61,"h":32}
        //},
        //int spriteMatchNum = 0;
        Hashtable frames = (Hashtable)decodedHash["frames"];
        foreach (DictionaryEntry item in frames) {
            SpriteMetaData newSprite = new SpriteMetaData();
            newSprite.name = item.Key.ToString();

            newSprite.name = newSprite.name.Replace(".jpg", "");
            newSprite.name = newSprite.name.Replace(".png", "");
            newSprite.name = newSprite.name.Replace(".tga", "");

            Hashtable table = (Hashtable)item.Value;
            Hashtable frame = (Hashtable)table["frame"];
            //Boolean rotated = (Boolean)table["rotated"];
            //Boolean trimmed = (bool)table["trimmed"];

            int frameX = int.Parse(frame["x"].ToString());
            int frameY = int.Parse(frame["y"].ToString());
            int frameW = int.Parse(frame["w"].ToString());
            int frameH = int.Parse(frame["h"].ToString());

            newSprite.rect = new Rect(frameX, frameY, frameW, frameH);
            if (alpha) {
                //#define USE_ETC_1
                //newSprite.rect.y = mAtlas.alphaTexture.height - frameY - newSprite.rect.height;//ugui是左下角为0点
                //#endif
            } else {
                newSprite.rect.y = mAtlas.mainTexture.height - frameY - newSprite.rect.height;//ugui是左下角为0点
            }

            if (irrgular) {
                bool trimmed = (bool)table["trimmed"];
                if (trimmed) {
                    Hashtable spSource = (Hashtable)table["spriteSourceSize"];
                    int spSourceX = int.Parse(spSource["x"].ToString());
                    int spSourceY = int.Parse(spSource["y"].ToString());
                    int spSourceW = int.Parse(spSource["w"].ToString());
                    int spSourceH = int.Parse(spSource["h"].ToString());

                    Hashtable source = (Hashtable)table["sourceSize"];
                    int sourceW = int.Parse(source["w"].ToString());
                    int sourceH = int.Parse(source["h"].ToString());

                    Vector2 offset = new Vector2(spSourceX + spSourceW / 2.0f - sourceW / 2.0f, spSourceY + spSourceH / 2.0f - sourceH / 2.0f);
                    newSprite.border.x = offset.x;
                    newSprite.border.y = -offset.y;
                }
            } else {
                foreach (SpriteMetaData oldSprite in oldSprites) {
                    if (oldSprite.name.Equals(newSprite.name, StringComparison.OrdinalIgnoreCase)) {
                        newSprite.border = oldSprite.border;
                        //spriteMatchNum++;
                        break;
                    }
                }
            }
            newSprites.Add(newSprite);
        }
        //第一次打的图集 
        //if (spriteMatchNum == 0) {
        //    Debug.LogError("图集无匹配border，可能是名字不匹配，请确认是否正常");
        //}
        return newSprites;
    }

    private static void CheckSpriteBorder(string prefabPath) {
        GameObject go = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject)) as GameObject;
        if (go != null) {
            CAtlas mAtlas = go.GetComponent<CAtlas>();
            string texturePath = prefabPath.Replace(".prefab", MAIN_TEX_POSTFIX);
            TextureImporter asetImp = AssetImporter.GetAtPath(texturePath) as TextureImporter;
            if (asetImp == null) {
                Debug.LogError("Sprite Border检测失败，找不到图集");
            } else {
                bool isAllEmpty = true;
                List<SpriteMetaData> sprites = new List<SpriteMetaData>(asetImp.spritesheet);
                foreach (SpriteMetaData sprite in sprites) {
                    if (sprite.border != Vector4.zero) {
                        isAllEmpty = false;
                    }
                }
                if (isAllEmpty) {
                    EditorUtility.DisplayDialog("Sprite Border检测", "图集Border全部为空，请确认是否正常", "确定");
                }
            }
        }
    }

    [Obsolete("Obsolete")]
    private static void ChangeTextureImportModule(string fileName, List<SpriteMetaData> sprites,bool alpha = false) {
        TextureImporter ti = (TextureImporter)AssetImporter.GetAtPath(fileName);
        if (ti != null) {
            ti.textureType = TextureImporterType.Sprite;
            ti.spriteImportMode = SpriteImportMode.Multiple;
            ti.spritesheet = sprites.ToArray();
            ti.npotScale = TextureImporterNPOTScale.ToNearest;//打图集就用nearest
            ti.textureShape = TextureImporterShape.Texture2D;
            ti.mipmapEnabled = false;
            ti.alphaIsTransparency = true;
            ti.wrapMode = TextureWrapMode.Clamp;
            ti.filterMode = FilterMode.Bilinear;
            ti.anisoLevel = 1;
            ti.isReadable = false;
            ti.maxTextureSize = 4096;
            if (alpha) {
                ti.isReadable = true;
                ti.SetPlatformTextureSettings(PLAT_FORM, ti.maxTextureSize, TextureImporterFormat.RGBA32);
            } else {
                #if USE_ETC_1
                ti.SetPlatformTextureSettings(PLAT_FORM, ti.maxTextureSize, TextureImporterFormat.ETC_RGB4);
                #else
                ti.SetPlatformTextureSettings(PLAT_FORM, ti.maxTextureSize, TextureImporterFormat.ETC2_RGBA8);
                #endif
            }
            AssetDatabase.ImportAsset(fileName);
        }
    }
    [Obsolete("Obsolete")]
    private static void SetTextureUnReadable(string fileName){
        TextureImporter ti = (TextureImporter)AssetImporter.GetAtPath(fileName);
        if (ti != null) {
            ti.isReadable = false;
            ti.SetPlatformTextureSettings(PLAT_FORM, ti.maxTextureSize, TextureImporterFormat.ETC_RGB4);
            AssetDatabase.ImportAsset(fileName);
        }
    }

    /// <summary>
    /// 该模块是否有图集的图元？
    /// </summary>
    /// <param name="modulePath"></param>
    /// <param name="extAtlasPath"></param>
    /// <returns></returns>
    private static bool ModuleHasTexture(string modulePath, string extAtlasPath) {
        return FileHasTexture(modulePath + "/Atlas") || FileHasTexture(modulePath + "/" + extAtlasPath);
    }
    /// <summary>
    /// 文件夹下是否有图片？
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    private static bool FileHasTexture(string path) {
        if (Directory.Exists(path)) {
            List<string> assets = FileMgr.GetFiles(path, isDeep:true);
            foreach (string asset in assets) {
                if (asset.EndsWith(".png") || asset.EndsWith(".jpg")) {
                    return true;
                }
            }
        }
        return false;
    }

    public static bool ExePackerCmd(string modulePath, string atlasPath, string moduleName) {
        string cmdExe = TEXTURE_CMD;
        string cmdParam = modulePath + "/Atlas/";
        if (Directory.Exists(modulePath + "/" + atlasPath)) {
            cmdParam += " " + modulePath + "/" + atlasPath + "/";
        }
        cmdParam += " --sheet " + modulePath + "/prefabs/" + moduleName + "_Tex.png";
        cmdParam += " --data " + modulePath + "/prefabs/" + moduleName + "_Tex.txt";
        cmdParam += " --format unity";
        if (cmdType == TexturePackerCMDType.Normal)
        {
            cmdParam += " --algorithm MaxRects";
        }
        else if(cmdType == TexturePackerCMDType.BigMap)
        {
            cmdParam += " --algorithm Basic";
            cmdParam += " --basic-sort-by Name";
            cmdParam += " --basic-order Ascending";
            cmdParam += " --shape-padding 0";
            cmdParam += " --border-padding 0";
            cmdParam += " --inner-padding 0";
            cmdParam += " --force-squared";
            
            cmdParam += " --pack-mode Best";
            cmdParam += " --scale-mode Smooth";
            cmdParam += " --disable-auto-alias";
        }
        cmdParam += " --max-size 4096";
        cmdParam += " --trim-mode None";
		cmdParam += " --size-constraints POT";
        cmdParam += " --disable-rotation";
       
        return ExeCmd(cmdExe, cmdParam);
    }

    public static bool ExeCmd(string cmdExe, string cmdParam) {
        ProcessStartInfo start = new ProcessStartInfo(cmdExe);
        start.Arguments = cmdParam;
        start.CreateNoWindow = true;
        start.RedirectStandardError = true;
        start.UseShellExecute = false;
        Process p = Process.Start(start);
        if (p != null) {
            string r = p.StandardError.ReadToEnd();
            if (!string.IsNullOrEmpty(r)) {
                Debug.Log("打包失败：" + r);
            }
            p.Close();
            return string.IsNullOrEmpty(r);
        }
        return false;
    }
    //[MenuItem("Assets/AAA", false, 0)]
    //public static void AAA()
    //{
    //    Texture2D tex = Selection.activeObject as Texture2D;
    //    ExportNoAlphaTexture(tex);
    //    ExportMaskTexture(tex);
    //}
    private static void ExportNoAlphaTexture(Texture2D oldTex) {
        string rootPath = AssetDatabase.GetAssetPath(oldTex);
        rootPath = rootPath.Replace(".png", ".jpg");
        Debug.Log(rootPath);
        Color[] pixels = oldTex.GetPixels();
        Texture2D tex = new Texture2D(oldTex.width, oldTex.height, TextureFormat.RGB24, false);
        int y = 0;
        while (y < tex.height) {
            int x = 0;
            while (x < tex.width) {
                Color sample = pixels[y * tex.width + x];
                if (sample.a < 1) {
                    if (sample.a <= 0) {
                        tex.SetPixel(x, y, Color.black);
                    } else {
                        tex.SetPixel(x, y, new Color(sample.r, sample.g, sample.b, 1));
                    }
                } else {
                    tex.SetPixel(x, y, sample);
                }
                ++x;
            }
            ++y;
        }

        tex.Apply();
        byte[] bytes = tex.EncodeToJPG(100);

        File.WriteAllBytes(rootPath, bytes);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
    }

    private static void ExportMaskTexture(Texture2D oldTex) {
        string rootPath = AssetDatabase.GetAssetPath(oldTex);
        rootPath = rootPath.Replace(".png", "_Alpha_Mask.jpg");
        Debug.Log(rootPath);
        Color[] pixels = oldTex.GetPixels();
        Texture2D tex = new Texture2D(oldTex.width, oldTex.height, TextureFormat.RGB24, false);
        int y = 0;
        while (y < tex.height) {
            int x = 0;
            while (x < tex.width) {
                float a = pixels[y * tex.width + x].a;
                Color sample = new Color(a, a, a);
                tex.SetPixel(x, y, sample);
                ++x;
            }
            ++y;
        }

        tex.Apply();
        byte[] bytes = tex.EncodeToJPG(100);

        File.WriteAllBytes(rootPath, bytes);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
    }


    ////////////////////////////////////////////////////////////////////////////////////
  
    [Obsolete("Obsolete")]
    public static void StartNormalPacker(string prefabPath, string moduleName, string modulePath) {
        GameObject go = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject)) as GameObject;
        if (go == null) // 第一次创建，先生成预设体
        {
            Object prefab = PrefabUtility.CreateEmptyPrefab(prefabPath);
            go = new GameObject(moduleName);
            go.AddComponent<CAtlas>();
            PrefabUtility.ReplacePrefab(go, prefab);
            DestroyImmediate(go);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        } else {
            AssetDatabase.RenameAsset(prefabPath, moduleName);
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
#if USE_SPRITE_PACKER
        if (ExePackerCmdNormal(modulePath, moduleName)) {
            string mainTexPath = prefabPath.Replace(".prefab", MAIN_TEX_POSTFIX);
            //此时已经把图集打完了
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            if (ImportTheTp(prefabPath, true))//解析json转换为spritesheet
            {
                Debug.Log("[Success]:" + moduleName);
            }
            //设置packingTag
            SetTexturePackingTag(prefabPath.Replace(".prefab", MAIN_TEX_POSTFIX), moduleName);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        }
#else
        if (ExePackerCmdNormal(modulePath, moduleName)) {
            //自动处理先关闭
            //AssetBuilder.OpenPostprocess = false;
            string mainTexPath = prefabPath.Replace(".prefab", MAIN_TEX_POSTFIX);
            //#define USE_ETC_1
            //string alphTexPath = prefabPath.Replace(".prefab", ALPHA_TEX_POSTFIX);
            //AssetDatabase.CopyAsset(mainTexPath, alphTexPath);
            //#endif
            //此时已经把图集打完了
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            if (ImportTheTp(prefabPath, true))//解析json转换为spritesheet
            {
                Debug.Log("[Success]:" + moduleName);
            }
            //#define USE_ETC_1
            //if (ImportTheTpAlpha(prefabPath)) {
            //    Debug.Log("[Success][Alpha]:" + moduleName);
            //}
            //#endif
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            //#define USE_ETC_1
            //透明通道的再改一下
            //TranAlphaChannelToRedChannel(alphTexPath);
            //还原成不可读
            //SetTextureUnReadable(alphTexPath);
            //#endif
            //自动处理还原
            //AssetBuilder.OpenPostprocess = true;
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        }
#endif
    }

    public static bool ExePackerCmdNormal(string modulePath, string atlasName) {
        string cmdExe = TEXTURE_CMD;
        string cmdParam = modulePath + "Atlas/" + atlasName + "/";
        cmdParam += " --sheet " + modulePath + "Prefabs/" + atlasName + "_Tex.png";
        cmdParam += " --data " + modulePath + "Prefabs/" + atlasName + "_Tex.txt";
        cmdParam += " --format unity";
        cmdParam += " --algorithm MaxRects";
        //cmdParam += " --algorithm Basic";
        cmdParam += " --basic-sort-by Name";
        cmdParam += " --basic-order Ascending";
        cmdParam += " --max-size 4096";
        if (cmdType == TexturePackerCMDType.SpriteAnim) {
            cmdParam += " --trim-mode Trim";
        } else {
            cmdParam += " --trim-mode None";
        }
        cmdParam += " --size-constraints POT";
        cmdParam += " --disable-rotation";
        return ExeCmd(cmdExe, cmdParam);
    }

    ////////////////////////////////////////////////////////////////////////////////////
  
    private static void CreatArtPrefab(string modulePath, string moduleName)
    {
        string prefabPath = $"{modulePath}Prefabs/{moduleName}Font.prefab";
        Debug.Log(prefabPath);

        GameObject go = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject)) as GameObject;
        CFont cFont = null;
        if (go)cFont = go.GetComponent<CFont>();
        string atlsPrefab = prefabPath.Replace("Font.prefab", ".prefab");
        GameObject atlsObj = AssetDatabase.LoadAssetAtPath(atlsPrefab, typeof(GameObject)) as GameObject;
        CAtlas atlas = atlsObj.GetComponent<CAtlas>();
        bool isCreate = false;
        if (go == null) 
        {
            go = new GameObject(moduleName);
            cFont = go.AddComponent<CFont>();
            isCreate = true;
        }
        if (cFont == null) cFont = go.AddComponent<CFont>();
        if (cFont.atlas == null || cFont.atlas != atlas) cFont.atlas = atlas;
        List<Sprite> sprites = atlas.spriteList;
        Dictionary<string, bool> fontVlueDic = new Dictionary<string, bool>();
        for (int i = 0; i < cFont.valueList.Count; i++)
        {
            fontVlueDic[cFont.valueList[i]] = true;
        }

        for (int i = 0; i < sprites.Count; i++)
        {
            string name = sprites[i].name;
            if (!fontVlueDic.TryGetValue(name, out _))
            {
                cFont.AddSymbol(name, name);
                fontVlueDic[name] = true;
            }
        }
        if (isCreate)
        {
            PrefabUtility.SaveAsPrefabAsset(go, prefabPath);
            DestroyImmediate(go);
        }
        else
        {
            AssetDatabase.RenameAsset(prefabPath, moduleName);
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    [Obsolete("Obsolete")]
#if USE_SPRITE_PACKER
    [MenuItem("Assets/按模块打包图集（使用SpritePacker）", false, 0)]
#endif
    public static void PackSelectWithoutAlphaDepart() {
        Object[] selectobj = Selection.GetFiltered(typeof(object), SelectionMode.Assets);
        string resType = FANTI;
        string modulePath = "Assets/OriginalResource/UI/" + selectobj[0].name;
        if (!ModuleHasTexture(modulePath, "Atlas_" + resType)) {
            Debug.Log("模块无可打包图片" + modulePath);
            return;
        }
        string moduleName = Path.GetFileNameWithoutExtension(modulePath);
        string prefabPath = modulePath + "/Prefabs/" + moduleName + ".prefab";
        StartPacketWithoutAlphaDepart(modulePath, moduleName, prefabPath, "Atlas_" + resType);
        CheckAndClearAlpha(prefabPath);
        CheckSpriteBorder(prefabPath);
    }
    [Obsolete("Obsolete")]
    public static void StartPacketWithoutAlphaDepart(string modulePath, string moduleName, string prefabPath, string extAtlasPath) {
        GameObject go = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject)) as GameObject;
        if (go == null) // 第一次创建，先生成预设体
        {
            Object prefab = PrefabUtility.CreateEmptyPrefab(prefabPath);
            go = new GameObject(moduleName);
            go.AddComponent<CAtlas>();
            PrefabUtility.ReplacePrefab(go, prefab);
            DestroyImmediate(go);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        if (ExePackerCmd(modulePath, extAtlasPath, moduleName)) {
            //此时已经把图集打完了
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            if (ImportTheTp(prefabPath))//解析json转换为spritesheet
            {
                Debug.Log("[Success]:" + moduleName);
            }
            //设置packingTag
            SetTexturePackingTag(prefabPath.Replace(".prefab", MAIN_TEX_POSTFIX), moduleName);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        }
    }

    public static void CheckAndClearAlpha(string prefabPath) {
        GameObject go = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject)) as GameObject;
        if (go != null) {
            CAtlas mAtlas = go.GetComponent<CAtlas>();
            //#define USE_ETC_1
            //mAtlas.spriteAlphaList.Clear();
            //mAtlas.spriteAlphaList = null;
            //mAtlas.spriteAlphaList = new List<Sprite>();
            //mAtlas.alphaTexture = null;
            //#endif
            mAtlas.mainTexture = null;
            mAtlas.MarkAsChanged();
        }
        string alphTexPath = prefabPath.Replace(".prefab", ALPHA_TEX_POSTFIX);
        if (File.Exists(Application.dataPath.Replace("Assets", alphTexPath))) {
            AssetDatabase.DeleteAsset(alphTexPath);
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
    }
    [Obsolete("Obsolete")]
    private static void SetTexturePackingTag(string path, string moduleName) {
        TextureImporter ti = (TextureImporter)AssetImporter.GetAtPath(path);
        if (ti != null) {
            ti.spritePackingTag = moduleName;
            ti.SetPlatformTextureSettings(PLAT_FORM, ti.maxTextureSize, TextureImporterFormat.ETC_RGB4, true);
            AssetDatabase.ImportAsset(path);
        }
    }    

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region 单个sprite图集功能，暂时保留
    [Obsolete("Obsolete")]
    //[MenuItem("Assets/按模块打包图集（单个sprite）", false, 0)]
    public static void PackSelectWithSprites() {
        Object[] selectobj = Selection.GetFiltered(typeof(object), SelectionMode.Assets);
        string resType = FANTI;
        string modulePath = "Assets/OriginalResource/UI/" + selectobj[0].name;
        if (!ModuleHasTexture(modulePath, "Atlas_" + resType)) {
            Debug.Log("模块无可打包图片" + modulePath);
            return;
        }
        string moduleName = Path.GetFileNameWithoutExtension(modulePath);
        string prefabPath = modulePath + "/Prefabs/" + moduleName + ".prefab";
        StartPacketWithSingleSprite(modulePath, moduleName, prefabPath, "Atlas_" + resType);
        CheckAndClearAlpha(prefabPath);
    }
    [Obsolete("Obsolete")]
    public static void StartPacketWithSingleSprite(string modulePath, string moduleName, string prefabPath, string extAtlasPath) {
        GameObject go = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject)) as GameObject;
        if (go == null) // 第一次创建，先生成预设体
        {
            Object prefab = PrefabUtility.CreateEmptyPrefab(prefabPath);
            go = new GameObject(moduleName);
            go.AddComponent<CAtlas>();
            PrefabUtility.ReplacePrefab(go, prefab);
            DestroyImmediate(go);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        }
        string atlasPath = modulePath + "/Atlas";
        if (Directory.Exists(atlasPath)) {
            List<string> assets = FileMgr.GetFiles(atlasPath, isDeep: true);
            foreach (string asset in assets) {
                if (asset.EndsWith(".png") || asset.EndsWith(".jpg")) {
                    TextureFormatToSprite(asset, moduleName);
                }
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        }
        if (FillAtlasWithSprites(prefabPath, atlasPath)) {
            Debug.Log("[Success]:" + moduleName);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        }
    }

    private static bool FillAtlasWithSprites(string prefabPath,string atlasPath, bool irregular = false) {
        GameObject go = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject)) as GameObject;
        if (go != null) {
            CAtlas mAtlas = go.GetComponent<CAtlas>();
            if (mAtlas == null) {
                Debug.Log("[Fail]：" + prefabPath);
                return false;
            }
            List<string> assets = FileMgr.GetFiles(atlasPath, isDeep: true);
            mAtlas.spriteList.Clear();
            foreach (string asset in assets) {
                Sprite sp = AssetDatabase.LoadAssetAtPath<Sprite>(asset);
                if (sp != null)//碎图
                {
                    mAtlas.AddSprite(sp);
                }
            }
            mAtlas.spriteList.Sort(CompareSprites);
            mAtlas.MarkAsChanged();
            Selection.activeGameObject = mAtlas.gameObject;
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        return true;
    }
    [Obsolete("Obsolete")]
    private static void TextureFormatToSprite(string fileName,string moduleName) {
        TextureImporter ti = (TextureImporter)AssetImporter.GetAtPath(fileName);
        if (ti != null) {
            ti.textureType = TextureImporterType.Sprite;
            ti.spriteImportMode = SpriteImportMode.Single;
            ti.npotScale = TextureImporterNPOTScale.None;
            ti.textureShape = TextureImporterShape.Texture2D;
            ti.mipmapEnabled = false;
            ti.alphaIsTransparency = true;
            ti.wrapMode = TextureWrapMode.Clamp;
            ti.filterMode = FilterMode.Bilinear;
            ti.anisoLevel = 1;
            ti.isReadable = false;
            ti.maxTextureSize = 4096;
            ti.spritePackingTag = moduleName;
            ti.SetPlatformTextureSettings(PLAT_FORM, ti.maxTextureSize, TextureImporterFormat.ETC_RGB4, true);
            AssetDatabase.ImportAsset(fileName);
        }
    }
    #endregion
}
