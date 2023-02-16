
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;
//using DG.Tweening.Plugins.Core.PathCore;
using Path = System.IO.Path;

namespace Assets.Editor.ComTool{
	public class FindProject{

#if FMD
		[MenuItem("Assets/Find References In Project", false, 2000)]
		private static void FindProjectReferences(){
			string appDataPath = Application.dataPath;
			string output = "";
			string selectedAssetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
			List<string> references = new List<string>();

			string guid = AssetDatabase.AssetPathToGUID(selectedAssetPath);

			var psi = new System.Diagnostics.ProcessStartInfo();
			psi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Maximized;
			psi.FileName = "/usr/bin/mdfind";
			psi.Arguments = "-onlyin " + Application.dataPath + " " + guid;
			psi.UseShellExecute = false;
			psi.RedirectStandardOutput = true;
			psi.RedirectStandardError = true;

			System.Diagnostics.Process process = new System.Diagnostics.Process();
			process.StartInfo = psi;

			process.OutputDataReceived += (sender, e) => {
				if (string.IsNullOrEmpty(e.Data))
					return;

				string relativePath = "Assets" + e.Data.Replace(appDataPath, "");

				// skip the meta file of whatever we have selected
				if (relativePath == selectedAssetPath + ".meta")
					return;

				references.Add(relativePath);

			};
			process.ErrorDataReceived += (sender, e) => {
				if (string.IsNullOrEmpty(e.Data))
					return;

				output += "Error: " + e.Data + "\n";
			};
			process.Start();
			process.BeginOutputReadLine();
			process.BeginErrorReadLine();

			process.WaitForExit(2000);

			foreach (var file in references){
				output += file + "\n";
				Debug.Log(file, AssetDatabase.LoadMainAssetAtPath(file));
			}

			Debug.LogWarning(references.Count + " references found for object " + Selection.activeObject.name + "\n\n" + output);
		}

		[MenuItem("Assets/Find Sprite References In Project", false, 2001)]
		private static void FindProjectReferencesSprite(){
			string output = "";
			var msg = "";
			string selectedAssetPath = AssetDatabase.GetAssetPath(Selection.activeObject);

			string guid = AssetDatabase.AssetPathToGUID(selectedAssetPath);
			TextureImporter t = TextureImporter.GetAtPath(selectedAssetPath) as TextureImporter;
			var texDict = GetPropList(t, "m_FileIDToRecycleName");
			var fileID = 0l;
			foreach (var id in texDict.Keys){
				if (texDict[id] == Selection.activeObject.name){
					fileID = id;
				}
			}
			var searchKey = string.Format("fileID: {0}, guid: {1}", fileID, guid);
			var refList = GetRefList(selectedAssetPath, Application.dataPath + "/OriginalResource/UI/", searchKey);
			for (int i = 0; i < refList.Count; i++){
				msg += string.Format("图集：{0}的Sprite：{1}引用被 {2}引用了\n",Path.GetFileNameWithoutExtension(selectedAssetPath), texDict[fileID],refList[i]);
			}
			if (string.IsNullOrEmpty(msg)){
				Debug.Log("检查UI图集引用合理性完成,暂未发现有无引用情况");
			}
			else{
				Debug.Log("检查UI图集引用合理性完成\n" + msg);
			}
			
		}

//		[MenuItem("MC/UI Tools/检查UI图集引用合理性")]
		private static void FindProjectReferencesTrue(){
			var fileList = FileMgr.GetFiles("Assets/OriginalResource/UI/", new string[]{".prefab"},true);
			var msg = "";
			var checkList = new string[3]{"MingUI", "Widget",""};
			foreach (string filePathFull in fileList){
				var filePath = filePathFull.Replace(Application.dataPath, "Assets/");
				var sortFilePath = filePath.Replace("Assets/OriginalResource/UI/", "");
				var headStr = sortFilePath.Split('/')[0];
				if (checkList.Contains(headStr) == false && headStr == (Path.GetFileNameWithoutExtension(sortFilePath))){
					checkList[2] = headStr;
					var refList = GetRefList(filePath);
					for (int i = 0; i < refList.Count; i++){
						var sonSortPath = refList[i].Replace("Assets/OriginalResource/UI/", "");
						var sonHeadStr = sonSortPath.Split('/')[0];
						if (checkList.Contains(sonHeadStr) == false){
							msg += string.Format("图集：{0}，有非本模块的文件引用了：{1}\n", headStr, refList[i]);
							
						}
					}
					checkList[2] = "";
				}
			}
			Debug.Log("检查UI图集引用合理性完成\n" + msg);
		}
		
//		[MenuItem("MC/UI Tools/检查无引用的UI图集图元")]
		private static void FindProjectReferencesNo(){
			var fileList = FileMgr.GetFiles("Assets/OriginalResource/UI/", new string[]{".prefab"},true);
			var msg = "";
			var checkList = new string[3]{"MingUI", "Widget",""};
			var dirList = Directory.GetDirectories("Assets/OriginalResource/UI/");
			var curDir = 0f;
			foreach (string filePathFull in fileList){
				var filePath = filePathFull.Replace(Application.dataPath, "Assets/");
				var sortFilePath = filePath.Replace("Assets/OriginalResource/UI/", "");
				var headStr = sortFilePath.Split('/')[0];
				if (checkList.Contains(headStr) == false && headStr == (Path.GetFileNameWithoutExtension(sortFilePath))){
					checkList[2] = headStr;
					var texPath = filePath.Replace(Path.GetFileName(filePath), headStr + "_Tex.png");
					TextureImporter t = TextureImporter.GetAtPath(texPath) as TextureImporter;
					var guid = AssetDatabase.AssetPathToGUID(texPath);
					var texDict = GetPropList(t, "m_FileIDToRecycleName");
					var spList = AssetDatabase.LoadAssetAtPath<CAtlas>(filePath).spriteList;
					foreach (var fileID in texDict.Keys){
						var isInSp = false;
						for (int i = 0; i < spList.Count; i++){
							if (spList[i].name == texDict[fileID]){
								isInSp = true;
							}
						}
						if (isInSp){
							var searchKey = string.Format("fileID: {0}, guid: {1}", fileID, guid);
							var refList = GetRefList(texPath, Application.dataPath + "/OriginalResource/UI/", searchKey);
							if (refList.Count == 0){
								msg += string.Format("图集：{0}，Sprite：{1}没有预设引用\n", headStr, texDict[fileID]);
							}
						}
						EditorUtility.DisplayProgressBar("正在检查UI图集引用合理性...",string.Format("正在检查图集{0}引用合理性:" + (curDir / dirList.Length),headStr),
							curDir / dirList.Length);
					}
					checkList[2] = "";
					curDir++;
					if (curDir == 10){
						EditorUtility.ClearProgressBar();
						return;
					}
				}
			}
			Debug.Log("检查UI图集引用合理性完成\n" + msg);
		}

		private static Dictionary<long, string> GetPropList(Object obj, string key){
			var dict = new Dictionary<long,string>();
			SerializedObject sObj = new SerializedObject(obj);
			SerializedProperty recycleMap = sObj.FindProperty(key);
			var e = recycleMap.GetEnumerator();
			while (e.MoveNext()){
				var element = e.Current as SerializedProperty;
				SerializedProperty first = element.FindPropertyRelative("first");
				var firstV = first.longValue;
				SerializedProperty second = element.FindPropertyRelative("second");
				var secV = second.stringValue;
				dict[firstV] = secV;
			}
			return dict;
		}

		private static List<string> GetRefList(string selectedAssetPath,string dir = null,string key = null){
			string appDataPath = Application.dataPath;
			string output = "";
			List<string> references = new List<string>();
			
			string guid = AssetDatabase.AssetPathToGUID(selectedAssetPath);
			key = string.IsNullOrEmpty(key) ? guid : key;
			dir = string.IsNullOrEmpty(dir) ? Application.dataPath : dir;
				
			var psi = new System.Diagnostics.ProcessStartInfo();
			psi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Maximized;
			psi.FileName = "/usr/bin/mdfind";
			psi.Arguments = "-onlyin " + dir + " " + key;
			psi.UseShellExecute = false;
			psi.RedirectStandardOutput = true;
			psi.RedirectStandardError = true;

			System.Diagnostics.Process process = new System.Diagnostics.Process();
			process.StartInfo = psi;

			process.OutputDataReceived += (sender, e) => {
				if (string.IsNullOrEmpty(e.Data))
					return;

				string relativePath = "Assets" + e.Data.Replace(appDataPath, "");

				// skip the meta file of whatever we have selected
				if (relativePath == selectedAssetPath + ".meta")
					return;

				references.Add(relativePath);

			};
			process.ErrorDataReceived += (sender, e) => {
				if (string.IsNullOrEmpty(e.Data))
					return;

				output += "Error: " + e.Data + "\n";
			};
			process.Start();
			process.BeginOutputReadLine();
			process.BeginErrorReadLine();

			process.WaitForExit(2000);
			return references;
		}
#endif
	}
}