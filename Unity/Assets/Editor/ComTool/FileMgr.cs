using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;


namespace Assets.Editor.ComTool {
    public class FileMgr {
        public static List<string> GetFiles(string dirPath, string[] extNames = null, bool isDeep = false, string[] excludeExts = null, bool firstReturn = false) {
            List<string> ls = new List<string>();
            if (Directory.Exists(dirPath)) {
                string[] files = Directory.GetFiles(dirPath);
                foreach (string filePath in files) {
                    string extName = Path.GetExtension(filePath);
                    if (excludeExts != null && excludeExts.Contains(extName)) continue;
                    if (extNames == null || extNames.Contains(extName)) {
                        string newfile = filePath.Replace("\\", "/");
                        ls.Add(newfile);
                        if (firstReturn) {
                            break;
                        }
                    }
                }
                if ((!firstReturn || ls.Count < 1) && isDeep) {
                    List<string> curDirs = GetCurDirs(dirPath);
                    for (int i = 0; i < curDirs.Count; i++) {
                        List<string> newLs = GetFiles(curDirs[i], extNames, true, excludeExts);
                        ls.AddRange(newLs);
                    }
                }
            }
            return ls;
        }

        public static List<string> GetCurDirNames(string dirPath) {
            List<string> ls = GetCurDirs(dirPath);
            for (int i=0; i < ls.Count; i++) {
                ls[i] = Path.GetFileName(ls[i]);
            }
            return ls;
        }

        public static List<string> GetCurDirs(string dirPath) {
            List<string> ls = new List<string>();
            if (Directory.Exists(dirPath)) {
                string[] files = Directory.GetDirectories(dirPath);
                ls = files.ToList();
            }
            return ls;
        }

        public static void CreateDirectory(string path) {
            path = path.Replace("\\", "/");
            if (Directory.Exists(path) == false) {
                path = path.Replace("/", "//");
                Directory.CreateDirectory(path);
            }
        }

        public static void Remove(string srcDir, string[] filterExts = null, bool isEditor = true) {
            if (isEditor) {
                if (Directory.Exists(srcDir)) {
                    AssetDatabase.DeleteAsset(srcDir);
                }
            } else {
                if (Directory.Exists(srcDir)) {
                    if (filterExts == null) {
                        Directory.Delete(srcDir, true);
                    } else {
                        string[] names = Directory.GetFiles(srcDir);
                        foreach (string fullFileName in names) {
                            string ext = Path.GetExtension(fullFileName);
                            if (filterExts.Contains(ext)) {
                                File.Delete(fullFileName);
                            }
                        }
                        string[] dirs = Directory.GetDirectories(srcDir);
                        foreach (string fullDir in dirs) {
                            Remove(fullDir, filterExts);
                        }
                    }
                }
            }
        }

        public static void Copy(string srcDir, string destDir, string[] filterExts = null,string destExt = null) {
            if (!Directory.Exists(destDir)) {
                Directory.CreateDirectory(destDir);
            }
            string[] names = Directory.GetFiles(srcDir);
            foreach (string fullFileName in names) {
                string fileName = Path.GetFileName(fullFileName);
                string extName = Path.GetExtension(fileName);
                if (filterExts == null || filterExts.Contains(extName)) {
                    if (string.IsNullOrEmpty(destExt) == false) {
                        fileName = fileName.Replace(extName, destExt);
                    }
                    string destFileName = destDir + "/" + fileName;
                    File.Copy(fullFileName, destFileName, true);
                }
            }
            string[] dirs = Directory.GetDirectories(srcDir);
            foreach (string fullDir in dirs) {
                string dir = Path.GetFileName(fullDir);
                string newDestDir = destDir + "/" + dir;
                if (!Directory.Exists(newDestDir)) {
                    Directory.CreateDirectory(newDestDir);
                }
                Copy(fullDir, newDestDir, filterExts,destExt);
            }
        }


        public static List<string> Recursive(string path, string[] exts, bool checkChild = true, List<string> files = null) {
            CreateDirectory(path);
            string[] names = Directory.GetFiles(path);
            string[] dirs = Directory.GetDirectories(path);
            foreach (string sourceName in names) {
                string ext = Path.GetExtension(sourceName);
                if (ext != null) {
                    ext = ext.ToLower();
                    bool checkType = false;
                    for (int i = 0; i < exts.Length; i++) {
                        if (ext.Equals(exts[i].ToLower())) {
                            checkType = true;
                        }
                    }
                    if (checkType == false) {
                        continue;
                    }
                }
                var filename = sourceName.Replace('\\', '/');
                filename = filename.Replace(Application.dataPath + "/Resources/", "");
                if (files == null) {
                    files = new List<string>();
                }
                files.Add(filename);

            }
            if (checkChild) {
                foreach (string dir in dirs) {
                    Recursive(dir, exts, true, files);
                }
            }
            return files;
        }
    }
}
