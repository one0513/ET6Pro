using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor.ComTool {
    public static class DevTool {
       
        public static string GetSelectedDir() {
            string path = "Assets";
            foreach (Object obj in Selection.GetFiltered(typeof(Object), SelectionMode.Assets)) {
                path = AssetDatabase.GetAssetPath(obj);
                if (File.Exists(path)) {
                    path = Path.GetDirectoryName(path);
                }
                break;
            }
            return path;
        }
    }
}
