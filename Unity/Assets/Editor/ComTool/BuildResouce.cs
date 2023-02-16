using Assets.Editor.ComTool;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Editor {
    public struct SFileInfo {
        public string index;
        public string abName;
        public string[] depends;
        public Object[] objects;
        public bool writeCfg;
        public string savePos;
    }

    public class BuildResouce : MonoBehaviour {
//         public const string RESPATH = "OriginalResource/";
//
//         public static BuildTarget target;
//         private static readonly BuildAssetBundleOptions _buildOptions;
//
//         public static string inputPath = Application.dataPath + "/" + RESPATH;
//         private static string _outPath = BuildCom.RES_ROOT_PATH;
//         private static string _platform = "android";
//         public static bool isCMDBuild = false;
//
//         private static readonly Dictionary<string, string[]> _comDirDict = new Dictionary<string, string[]>() {
// //                { "Shaders", new[] {".shader" } },
//                 { "Prefabs", new[]{ ".prefab" } },
//                 { "Textures", new[] { ".png",".jpg",".tga" } },
//                 { "Materials", new[] { ".mat" } }
//             };
//         private static readonly Dictionary<string, string[]> _effDirDict = new Dictionary<string, string[]>() {
//                 { "Prefabs", new[]{ ".prefab" } },
//                 { "Texture", new[] { ".png",".tga", ".jpg" } }
//             };
//         private static readonly Dictionary<string, string[]> _uiDirDict = new Dictionary<string, string[]>() {
//                 { "Prefabs", new[]{ ".prefab",".png"} }
//             };
//
//         private static readonly Dictionary<string, string[]> _modelDirDict = new Dictionary<string, string[]>() {
// //                { "Shaders", new[] {".shader" } },
//                 { "Prefabs", new[]{ ".prefab" } },
//                 { "Textures", new[] { ".tga",".png",".jpg", ".dds" } }  //dds??
//             };
//         private static readonly Dictionary<string, string[]> _audioDirDict = new Dictionary<string, string[]>() {
//                 { "", new[]{ ".wav",".mp3" } }
//             };
//
//         private static readonly Dictionary<string, string[]> _sceneDirDict = new Dictionary<string, string[]> {
//             {"Prefabs", new[] {".prefab"}},
//             {"Lightmaps", new []{".exr"}}
//         };
//
//         private static readonly Dictionary<string, Dictionary<string, string[]>> type2DirDict = new Dictionary
//             <string, Dictionary<string, string[]>> {
//             {"Effect", _effDirDict},
//             {"Actor", _modelDirDict},
//             {"Scenes", _sceneDirDict},
//             {"Audio", _audioDirDict}
//         };
//
//         static BuildResouce() {
//             _buildOptions = BuildCom.BuildOptions;
//         }
//
//        
//         public static void BuildCmd() {
//             isCMDBuild = true;
//             _outPath = Environment.GetEnvironmentVariable("RES_PATH_ENV");
//             _platform = Environment.GetEnvironmentVariable("PLATFORM");
//             target = BuildCom.GetBuildTarget(_platform);
//             BuildCom.Init();
//             
//             BuildAllAB(target, _outPath);
//            
//         }
//         
//         private static void BuildAllAB(BuildTarget buildTarget, string resOutPath, bool refreshABName = false) {
//             //AssetBuilder.OpenPostprocess = false;
//             inputPath = Application.dataPath + "/" + RESPATH;
//             AssetDatabase.Refresh();
//             if (refreshABName) { CleanAbNames(); }
//             AssetDatabase.Refresh();
//             ModifyShaderABName();
//             ModifyFontABName();
//             List<string> dirs = FileMgr.GetCurDirNames(inputPath);
//             if (refreshABName) {
//                 foreach (string dir in dirs) {
//                     ModifyResAssetName(dir, inputPath);
//                 }
//             }
//             AssetDatabase.Refresh();
//             BuildAssetBundles(resOutPath, buildTarget);
//             AssetDatabase.Refresh();
//         }
//
//          public static void BuildFirstJson() {
//             isCMDBuild = true;
//             _outPath = Environment.GetEnvironmentVariable("RES_PATH_ENV");
//             _platform = Environment.GetEnvironmentVariable("PLATFORM");
//             target = BuildCom.GetBuildTarget(_platform);
//             BuildCom.Init();
//            
//         }
//
//         private static void ModifyShaderABName() {
//            
//         }
//
//         private static void ModifyFontABName() {
//             var fontArr = new[] { "kaiti", "yuanti" };
//             for (var i = 0; i < 2; i++) {
//                 var metaFilePath = string.Format("Assets/Resources/Font/{0}.ttf", fontArr[i]);
//                 var abName = string.Format("font/{0}.ab", fontArr[i]);
//                 if (File.Exists(metaFilePath)) {
//                     TrueTypeFontImporter assetImport = (TrueTypeFontImporter)AssetImporter.GetAtPath(metaFilePath);
//                     assetImport.includeFontData = true;
//                     assetImport.assetBundleName = abName;
//                     assetImport.assetBundleVariant = "";
//                 }
//             }
//         }
//
//         private static void ModifyResAssetName(string resType, string dirPath) {
//             dirPath = dirPath + resType;
//             if (resType == "UI") {
//                 UIModifyResAssetName(dirPath);
//             } else if (type2DirDict.ContainsKey(resType)) {
//                 CommonModifyResAssetName(dirPath, type2DirDict[resType]);
//             } else if (resType == "Tile") {
//                 AllModifyResAssetName(dirPath);
//             } else {
//                 CommonModifyResAssetName(dirPath);
//             }
//         }
//
//         private static void CommonModifyResAssetName(string dirPath) {
//             CommonModifyResAssetName(dirPath, _comDirDict);
//         }
//         private static void UIModifyResAssetName(string dirPath, Dictionary<string, string[]> comDirDict) {
//             foreach (KeyValuePair<string, string[]> kvp in comDirDict) {
//                 string curPath = dirPath + "/" + kvp.Key;
//                 if (Directory.Exists(curPath)) {
//                     List<string> files = FileMgr.GetFiles(curPath, kvp.Value);
//                     foreach (string filePath in files) {
//                         string extName = Path.GetExtension(filePath);
//                         if (kvp.Key == "Prefabs" && extName == ".png") {
//                             string abName = GetUITexAbName(filePath);
//                             ModifyMetaAttr(filePath, abName);
//                         } else {
//                             ModifyMetaAttr(filePath);
//                         }
//                     }
//                 } else {
//                     ELog.Warring("目录" + curPath + "不存在");
//                 }
//             }
//         }
//
//         private static void CommonModifyResAssetName(string dirPath, Dictionary<string, string[]> comDirDict) {
//             foreach (KeyValuePair<string,string[]> kvp in comDirDict) {
//                 string curPath = dirPath + "/" + kvp.Key;
//                 if (Directory.Exists(curPath)) {
//                     List<string> files = FileMgr.GetFiles(curPath, kvp.Value);
//                     foreach (string filePath in files) {
//                         ModifyMetaAttr(filePath);
//                     }
//                 } else {
//                     ELog.Warring("目录" + curPath + "不存在");
//                 }
//             }
//         }
//
//         private static void AllModifyResAssetName(string dirPath) {
//             var files = FileMgr.GetFiles(dirPath);
//             foreach (var filePath in files) {
//                 ModifyMetaAttr(filePath);
//             }
//         }
//
//         private static string GetUITexAbName(string filePath) {
//             string relativePath = filePath.Replace(inputPath, "");
//             if (relativePath.IndexOf(RESPATH, StringComparison.Ordinal) != -1) {
//                 relativePath = relativePath.Replace("Assets/" + RESPATH, "");
//             }
//             string[] name = Path.GetFileNameWithoutExtension(relativePath).Split('_');
//             string abName = Path.GetDirectoryName(relativePath) + "/" + name[0] + ".ab";
//             return abName;
//         }
//
//         // 修改资源的ab Name
//         private static void ModifyMetaAttr(string filePath, string abName = "", string abExtName = "") {
//             string relativePath = filePath.Replace(inputPath, "");
//             string mateFilePath;
//             if (relativePath.IndexOf(RESPATH, StringComparison.Ordinal) != -1) {
//                 relativePath = relativePath.Replace("Assets/" + RESPATH, "");
//             }
//             mateFilePath = "Assets/" + RESPATH + relativePath;
//             if (abName == "") {
//                 abName = Path.GetDirectoryName(relativePath) + "/" + Path.GetFileNameWithoutExtension(relativePath) + ".ab";
//             }
//             if (File.Exists(mateFilePath)) {
//                 AssetImporter assetImport = AssetImporter.GetAtPath(mateFilePath);
//                 assetImport.assetBundleName = abName;
//                 assetImport.assetBundleVariant = abExtName;
//             } else {
//                 ELog.Warring(mateFilePath + "不存在");
//             }
//         }
//
//         //设置UI url
//         private static void UIModifyResAssetName(string uiDirPath, string lang = "Fanti") {
//             List<string> curDirs = FileMgr.GetCurDirs(uiDirPath);
//             foreach (string dirPath in curDirs) {
//                 string baseName = Path.GetFileNameWithoutExtension(dirPath);
//                 if (baseName == "Textures") {
//                     ModifyUITextures(dirPath, lang);
//                 } else {
//                     UIModifyResAssetName(dirPath, _uiDirDict);
//                 }
//             }
//         }
//
//         private static void ModifyUITextures(string dirPath, string lang) {
//             List<string> textureLs = FileMgr.GetFiles(dirPath, new[] { ".png", ".jpg" });
//
//             List<string> iconLs = FileMgr.GetFiles(dirPath + "/Icon", new[] { ".png", ".jpg" }, true);
//             textureLs.AddRange(iconLs);
//             foreach (string filePath in textureLs) {
//                 ModifyMetaAttr(filePath);
//             }
//             ModifyUILangTextures(dirPath, lang);
//         }
//
//         private static void ModifyUILangTextures(string dirPath, string lang = "Fanti") {
//             List<string> langLs = FileMgr.GetFiles(dirPath + "/Texture_" + lang, new[] { ".png", ".jpg" });
//             foreach (string filePath in langLs) {
//                 string relativePath = filePath.Replace(inputPath, "");
//                 if (relativePath.IndexOf(RESPATH, StringComparison.Ordinal) != -1) {
//                     relativePath = relativePath.Replace("Assets/" + RESPATH, "");
//                 }
//                 relativePath = relativePath.Replace("/Texture_" + lang, "");
//                 string abName = Path.GetDirectoryName(relativePath) + "/" + Path.GetFileNameWithoutExtension(relativePath) + ".ab";
//                 ModifyMetaAttr(filePath, abName);
//             }
//         }
//
//         public static void BuildAssetBundles(string outPutPath, BuildTarget buildTarget) {
//             if (!Enum.IsDefined(typeof(BuildTarget), buildTarget)) throw new InvalidEnumArgumentException("buildTarget", (int)buildTarget, typeof(BuildTarget));
//             ELog.Print("BuildAssetBundles start " + outPutPath + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss fff"));
//             BuildPipeline.BuildAssetBundles(outPutPath, _buildOptions, buildTarget);
//             ELog.Print("BuildAssetBundles end " + outPutPath +  DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss fff"));
//             AssetDatabase.Refresh();
//             string name = Path.GetFileName(outPutPath);
//             string srcFile = outPutPath + "/" + name;
//             string destFile = srcFile + ".ab";
//             if (File.Exists(destFile)) {
//                 File.Delete(destFile);
//             }
//             if (File.Exists(srcFile)) {
//                 ELog.Print("move");
//                 File.Move(outPutPath + "/" + name, destFile);
//             }
//             ELog.Print("^_^打包完成^_^");
//         }
//
//         private static void CleanAbNames() {
//             string[] abNames = AssetDatabase.GetAllAssetBundleNames();
//             for (int i=0; i < abNames.Length; i++) {
//                 AssetDatabase.RemoveAssetBundleName(abNames[i], true);
//             }
//             ELog.Print("资源数：" + abNames.Length);
//         }
//
//         //[MenuItem("Assets/打包UI模块资源", false, -20)]
//         public static void BuildSelectUIAB() {
//             var osDirPath = "";
//             foreach (Object obj in Selection.GetFiltered(typeof(Object), SelectionMode.Assets)) {
//                 osDirPath = AssetDatabase.GetAssetPath(obj);
//                 if (File.Exists(osDirPath)) {
//                     osDirPath = Path.GetDirectoryName(osDirPath);
//                 }
//                 break;
//             }
//             if (osDirPath == "") return;
//             List<string> assetPaths = new List<string>();
//             assetPaths.AddRange(
//                 Array.ConvertAll(
//                     FileMgr.GetFiles(osDirPath, null, true, new[] { ".meta" })
//                         .ToArray(),
//                     s => s.Substring(s.IndexOf("Assets"))));
//             Dictionary<string, AssetBundleBuild> dict = new Dictionary<string, AssetBundleBuild>();
//             if (osDirPath.Contains("OriginalResource/UI")) {
//                 AssetBundleBuild assetBuild = new AssetBundleBuild();
//                 foreach (var path in assetPaths) {
//                     if (path.Contains("Prefabs")) {
//                         var osPath = Application.dataPath.Replace("Assets", path);
//                         if (path.EndsWith("png")) { //todo 这里单干有问题
//                             assetBuild.assetBundleName = GetUITexAbName(osPath);
//                             assetBuild.assetNames = new[] { path.Substring(path.IndexOf("_Tex")) };
//                         } else {
//                             assetBuild.assetBundleName = GetAssetAbName(path);
//                             assetBuild.assetNames = new[] { path };
//                         }
//                         dict[assetBuild.assetBundleName] = assetBuild;
//                     } else if (path.Contains("Textures")) {
//                         assetBuild.assetBundleName = GetAssetAbName(path);
//                         assetBuild.assetNames = GetAssetNames(path);
//                         dict[assetBuild.assetBundleName] = assetBuild;
//                     }
//                 }
//                 AssetBundleBuild[] buildArr = dict.Values.ToArray();
//                 BuildPipeline.BuildAssetBundles(_outPath, buildArr, BuildCom.BuildOptions, BuildTarget.Android);
//             }
//         }
//
//         //[MenuItem("Assets/打包所选资源[非UI]", false, -20)]
//         public static void BuildSelectAb() {
//             Object[] selectObj = Selection.GetFiltered(typeof(object), SelectionMode.Assets);
//             var assetPaths = new List<string>();
//             foreach (var obj in selectObj) {
//                 var selAstPath = AssetDatabase.GetAssetPath(obj);
//                 if (Directory.Exists(selAstPath)) {
//                     assetPaths.AddRange(
//                         Array.ConvertAll(
//                             FileMgr.GetFiles(Application.dataPath.Replace("Assets", selAstPath), null, true, new[] { ".meta" }).ToArray(),
//                             s => s.Substring(s.IndexOf("Assets"))));
//                 } else {
//                     assetPaths.Add(selAstPath);
//                 }
//             }
//             Dictionary<string, AssetBundleBuild> dict = new Dictionary<string, AssetBundleBuild>();
//             foreach (var path in assetPaths) {
//                 var dirArr = path.Split('/');
//                 var typeDir = dirArr[2];
//                 var subDir = dirArr[3];
//                 var ext = "." + path.Split('.')[1];
//                 if (type2DirDict.ContainsKey(typeDir)) {
//                     var subDict = type2DirDict[typeDir];
//                     AssetBundleBuild assetBuild = new AssetBundleBuild();
//                     if (subDict.ContainsKey(subDir) && subDict[subDir].Contains(ext)) {
//                         assetBuild.assetBundleName = GetAssetAbName(path);
//                         assetBuild.assetNames = GetAssetNames(path);
//                         dict[assetBuild.assetBundleName] = assetBuild;
//                     }
//                 }
//             }
//             AssetBundleBuild[] buildArr = dict.Values.ToArray();
//             BuildPipeline.BuildAssetBundles(_outPath, buildArr, BuildCom.BuildOptions, BuildTarget.Android);
//         }
//
//        
//
//         private static string[] GetAssetNames(string assetPath) {
//             string[] arr = new[] { assetPath };
//             return arr;
//         }
//
//         private static string GetAssetAbName(string assetPath) {
//             assetPath = assetPath.Replace("Assets/" + RESPATH, "");
//             string abName = Path.GetDirectoryName(assetPath) + "/" + Path.GetFileNameWithoutExtension(assetPath) + ".ab";
//             return abName;
//         }
     }
}
