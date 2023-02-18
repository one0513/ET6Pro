using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TMPro
{
    public class TMP_AssetsLoad
    {
        private static Action<string, Action<Object>> loadAssets;
        public static void SetLoadAct(Action<string, Action<Object>> _loadAssets)
        {
            loadAssets = _loadAssets;
        }

        public static T GetAssets<T>(string url)where T : UnityEngine.Object
        {
            UnityEngine.Object obj = null ;
            obj = Resources.Load(url);
#if UNITY_EDITOR
            //obj = UnityEditor.AssetDatabase.LoadAssetAtPath<T>($"Assets/AssetsPackage/{url}.asset");
#else
           
            if (loadAssets != null)
            {
                loadAssets.DynamicInvoke(url, (Action<Object>) (tmp =>
                {
                    obj = tmp;
                }));
            }
#endif
            return obj as T;
        }
    }

    public class TMP_ChatReport
    {
        private static Dictionary<int,bool> cacheMissingDic;
        private static System.Action<string> chatReportAct; 
        public static void SetChatReportAct(Action<string> _chatReportAct) {
            chatReportAct = _chatReportAct;
        }

        public static void ReportMissiongChat(int srcGlyph) {
            if (chatReportAct != null) {
                if (cacheMissingDic != null && cacheMissingDic.Count > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (var VARIABLE in cacheMissingDic.Keys)
                    {
                        sb.Append((char) VARIABLE + "|");
                    }

                    if (cacheMissingDic.ContainsKey(srcGlyph))
                    {
                        sb.Remove(sb.Length - 1, 1);
                    }
                    else
                    {
                        sb.Append((char) srcGlyph);
                    }
                    chatReportAct(sb.ToString());
                    sb.Clear();
                    cacheMissingDic.Clear();
                }
                else
                {
                    chatReportAct(((char) srcGlyph).ToString());
                }
                
            }
            else {
                if (cacheMissingDic == null)
                {
                    cacheMissingDic = new Dictionary<int, bool>();
                }
                cacheMissingDic[srcGlyph] = true;
                
            }
        }
        
    }
    
    
    
}