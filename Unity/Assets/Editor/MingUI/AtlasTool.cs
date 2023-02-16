using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor.PublishDelScript.MingUI {
    class AtlasTool : EditorWindow {
        private CAtlas atlas;
        private string selectPath;
        private Dictionary<CSprite, string> moduleSpriteInfo = new Dictionary<CSprite, string>();
        private Dictionary<string, string> newModuleSpriteList = new Dictionary<string, string>();

        //[MenuItem("Assets/阿尔法/工具/转移图集", false, -21)]
        static void Main() {
            if (Selection.assetGUIDs.Length != 1) {
                EditorUtility.DisplayDialog("温馨提示", "请选择一个模块文件夹", "确定");
                return;
            }
            var win = GetWindow<AtlasTool>("转移图集");
            win.selectPath = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0]);
        }

        void OnGUI() {
            atlas = (CAtlas)EditorGUILayout.ObjectField("目标图集", atlas, typeof(CAtlas), false);
            if (GUILayout.Button("转移")) {
                Process();
            }
        }

        private void Process() {
            moduleSpriteInfo.Clear();
            newModuleSpriteList.Clear();
            var moduleName = Path.GetFileName(selectPath);
            var spList = atlas.spriteList;
            foreach (var sp in spList) {
                newModuleSpriteList[sp.name.Split('/').Last()] = sp.name;
            }
            var prefabs = Directory.GetFiles(selectPath + "/Prefabs", "*.prefab");
            foreach (var prefab in prefabs) {
                var obj = EditorGUIUtility.Load(prefab) as GameObject;
                var sps = obj.GetComponentsInChildren<CSprite>();
                foreach (var sp in sps) {
                    var oldAtlas = sp.Atlas;
                    if (oldAtlas && oldAtlas.name == moduleName) {
                        var spLastName = sp.SpriteName.Split('/').Last();
                        if (newModuleSpriteList.ContainsKey(spLastName)) {
                            sp.Atlas = atlas;
                            sp.SpriteName = newModuleSpriteList[spLastName];
                        }
                    }
                    EditorUtility.SetDirty(sp);
                }
            }
            AssetDatabase.SaveAssets();
        }
    }
}
