using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor.ComTool {
    public static class BuildCom {
        public static readonly BuildAssetBundleOptions BuildOptions = BuildAssetBundleOptions.DeterministicAssetBundle |
                                                                      BuildAssetBundleOptions.ChunkBasedCompression;
        // public static string ResComplieProjectParent {
        //     get {
        //         if (BuildResouce.isCMDBuild) {
        //             return Environment.GetEnvironmentVariable("RES_COMPLIE_PROJECT_PARENT_PATH_ENV");
        //         } else {
        //             return Application.dataPath + "/../..";
        //         }
        //     }
        // }
        //
        // public static string RES_ROOT_PATH {
        //     get {
        //         if (BuildResouce.isCMDBuild) {
        //             var pf = Environment.GetEnvironmentVariable("PLATFORM");
        //             return ResComplieProjectParent.Replace("ResComplie", "game_res_" + pf) + "/res/";
        //         } else {
        //             return ResComplieProjectParent + "/../aef_client/game_res/android/res/";
        //         }
        //     }
        // }
        
        // public static string LUASCRIPT_SRC_PATH {
        //     get {
        //         return ResComplieProjectParent + "/lua/scripts";
        //     }
        // }

        static BuildCom() {
        }

        public static void Init() {
           
        }

        // [Obsolete("Obsolete")]
        // public static string GetOutpathByBuildTarget(BuildTarget target) {
        //     if (!BuildResouce.isCMDBuild) {
        //         var pf = "/android/";
        //         if (target == BuildTarget.StandaloneWindows64) {
        //             pf = "/pc/";
        //         }
        //         if (target == BuildTarget.StandaloneOSXIntel64) {
        //             pf = "/mac/";
        //         }
        //         return RES_ROOT_PATH.Replace("/andorid/", pf);
        //     }
        //     return RES_ROOT_PATH;
        // }


        /// <summary>
        /// 外部直接调用次打包函数，用于配置和lua打包
        /// </summary>
        /// <param name="srcDir"></param>
        /// <param name="outDir"></param>
        /// <param name="buildTarget"></param>
        /// <param name="getAbName"></param>
        /// <param name="filters"></param>
        public static void BuildAssetBundle(string srcDir,
            string outDir,
            BuildTarget buildTarget,
            Func<string, string> getAbName,
            string[] filters = null) {
            AssetDatabase.Refresh();
            Dictionary<string, AssetBundleBuild> dict = new Dictionary<string, AssetBundleBuild>();
            GetAssetBundles(srcDir, dict, getAbName, filters);
            DoBuildAssetBundle(outDir, buildTarget, dict);
        }


        public static BuildTarget GetBuildTarget(string platForm) {
            switch (platForm) {
                case "android":
                    return BuildTarget.Android;
                case "ios":
                    return BuildTarget.iOS;
                case "pc":
                    return BuildTarget.StandaloneWindows64;
                case "linux":
                    return BuildTarget.StandaloneLinux64;
                default:
                    return BuildTarget.Android;
            }
        }
        
        public static string GetBuildTargetName(BuildTarget platForm) {
            switch (platForm) {
                case BuildTarget.Android:
                    return "android";
                case BuildTarget.iOS:
                    return "ios";
                case BuildTarget.StandaloneWindows64:
                    return "pc";
                default:
                    return "android";
            }
        }

        /// <summary>
        /// 修改abName
        /// </summary>
        /// <param name="scrDir"></param>
        /// <param name="getAbName"></param>
        /// <param name="filters"></param>
        public static void ModifyAbName(string scrDir, Func<string, string> getAbName, string[] filters = null) {
            string[] names = Directory.GetFiles(scrDir);
            foreach (string fullFileName in names) {
                string ext = Path.GetExtension(fullFileName);
                if (filters == null || filters.Contains(ext)) {
                    string newfullFileName = fullFileName.Replace("\\", "/");
                    string abName = getAbName(newfullFileName);
                    ModifyMetaAttr(newfullFileName, abName);
                }
            }
            string[] dirs = Directory.GetDirectories(scrDir);
            foreach (string fullDir in dirs) {
                ModifyAbName(fullDir, getAbName, filters);
            }
        }

        /// <summary>
        /// 先修改ab名字后在打包
        /// </summary>
        /// <param name="outPutPath"></param>
        /// <param name="buildTarget"></param>
        public static void BuildAssetBundles(string outPutPath, BuildTarget buildTarget) {
            if (!Enum.IsDefined(typeof(BuildTarget), buildTarget)) throw new InvalidEnumArgumentException("buildTarget", (int)buildTarget, typeof(BuildTarget));
            BuildPipeline.BuildAssetBundles(outPutPath, BuildOptions, buildTarget);
            AssetDatabase.Refresh();
            string name = Path.GetFileName(outPutPath);
            string srcFile = outPutPath + "/" + name;
            string destFile = srcFile + ".ab";
            if (File.Exists(destFile)) {
                File.Delete(destFile);
            }
            if (File.Exists(srcFile)) {
                ELog.Print("move");
                File.Move(outPutPath + "/" + name, destFile);
            }
            ELog.Print("^_^打包完成^_^");
        }

        public static void GetAssetBundles(string srcDir,
            Dictionary<string, AssetBundleBuild> dict,
            Func<string, string> getAbName, string[] filters = null) {
            string[] names = Directory.GetFiles(srcDir);
            foreach (string fullFileName in names) {
                string ext = Path.GetExtension(fullFileName);
                if (filters == null || filters.Contains(ext)) {
                    string newfullFileName = fullFileName.Replace("\\", "/");
                    string abName = getAbName(newfullFileName);
                    AssetBundleBuild assetBuild = new AssetBundleBuild();
                    assetBuild.assetBundleName = abName;
                    assetBuild.assetNames = new[] { fullFileName };
                    dict[newfullFileName] = assetBuild;
                }
            }
            string[] dirs = Directory.GetDirectories(srcDir);
            foreach (string fullDir in dirs) {
                GetAssetBundles(fullDir, dict, getAbName, filters);
            }
        }

        public static void DoBuildAssetBundle(string outputLuaDir, BuildTarget buildTarget,
            Dictionary<string, AssetBundleBuild> dict) {
            AssetBundleBuild[] buildArr = dict.Values.ToArray();
            ELog.Print("===开始打包==="+outputLuaDir);
            BuildPipeline.BuildAssetBundles(outputLuaDir, buildArr, BuildOptions, buildTarget);
            ELog.Print("===打包完成==="+outputLuaDir);

        }

        public static void ModifyMetaAttr(string filePath, string abName, string abExtName = "") {
            if (File.Exists(filePath)) {
                AssetImporter assetImport = AssetImporter.GetAtPath(filePath);
                assetImport.assetBundleName = abName;
                assetImport.assetBundleVariant = abExtName;
            } else {
                ELog.Warring(filePath + "不存在");
            }
        }

        public static void RenameExt(string scrDir, string extName, string[] filters = null) {
            string[] names = Directory.GetFiles(scrDir);
            foreach (string fullFileName in names) {
                string ext = Path.GetExtension(fullFileName);
                if (filters == null || filters.Contains(ext)) {
                    string fileName = Path.GetFileNameWithoutExtension(fullFileName);
                    string newFileName = scrDir + "/" + fileName + extName;
                    if (File.Exists(newFileName)) {
                        File.Delete(newFileName);
                    }
                    File.Move(fullFileName, newFileName);
                }
            }
            string[] dirs = Directory.GetDirectories(scrDir);
            foreach (string fullDir in dirs) {
                RenameExt(fullDir, extName, filters);
            }
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
                    if (r.Contains("program run error")) {
                        ELog.Error($"cmdExe-->{cmdExe}执行出现错误-->{r}");
                    }
                    else {
                        ELog.Print("打包失败：" + r);   
                    }
                }
                p.Close();
                return string.IsNullOrEmpty(r);
            }
            return false;
        }

        public static void CopyFileAndSetABName(string srcDir, string destDir,string destRoot,string extName){
            if (!Directory.Exists(destDir)) {
                Directory.CreateDirectory(destDir);
            }
            string[] names = Directory.GetFiles(srcDir);
            foreach (string fullFileName in names) {
                string fileName = Path.GetFileNameWithoutExtension(fullFileName);
                if(string.IsNullOrEmpty(fileName))continue;
                string destFileName = destDir + "/" + fileName + extName;
                File.Copy(fullFileName, destFileName, true);
            }
            string[] dirs = Directory.GetDirectories(srcDir);
            foreach (string fullDir in dirs) {
                string dir = Path.GetFileName(fullDir);
                string newDestDir = destDir + "/" + dir;
                if (!Directory.Exists(newDestDir)) {
                    Directory.CreateDirectory(newDestDir);
                }
                CopyFileAndSetABName(fullDir, newDestDir,destRoot, extName);
            }
        }
        
        public static void DelOutFile(string srcDir, string destDir,string[] checkExt,string extName){
            if (!Directory.Exists(destDir)) {
                Directory.CreateDirectory(destDir);
            }
            if (Directory.Exists(srcDir)) {
                string[] names = Directory.GetFiles(srcDir);
                foreach (string fullFileName in names) {
                    string fileName = Path.GetFileNameWithoutExtension(fullFileName);
                    if (string.IsNullOrEmpty(fileName) || checkExt.Contains(Path.GetExtension(fullFileName)) == false) continue;
                    string destFileName = destDir + "/" + fileName + extName;
                    if (File.Exists(destFileName) == false) {//目标地方没有，就把源文件删掉
                        File.Delete(fullFileName);
                        ELog.Print("DelOutFile" + fullFileName + " " + destFileName);
                    }
                }
                string[] dirs = Directory.GetDirectories(srcDir);
                foreach (string fullDir in dirs) {
                    string dir = Path.GetFileName(fullDir);
                    string newDestDir = destDir + "/" + dir;
                    if (!Directory.Exists(newDestDir)) {
                        Directory.CreateDirectory(newDestDir);
                    }
                    DelOutFile(fullDir, newDestDir, checkExt, extName);
                }
            }
            
        }
    }
}
