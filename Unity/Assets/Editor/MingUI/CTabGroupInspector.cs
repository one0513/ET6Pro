using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(CTabGroup), true)]
public class CTabGroupInspector : CToggleGroupInspector
{
    private CTabGroup Group
    {
        get { return target as CTabGroup; }
    }

    private string _label;

    protected override void OnCustomInpector()
    {
        bool belongNav = false;
        CTabNavigation[] navs = Group.GetComponentsInParent<CTabNavigation>(true);
        foreach (CTabNavigation nav in navs)
        {
            if (nav.tabBars == Group)
            {
                belongNav = true;//有所属的导航栏自己不能操作
                break;
            }
        }
        EditorGUI.BeginDisabledGroup(belongNav);

        if (MingUIEditorTools.DrawHeader("标签页"))
        {
            MingUIEditorTools.BeginContents();
            List<Toggle> toggles = Group.toggleList;
            for (int i = 0; i < toggles.Count; i++)
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(i.ToString(), GUILayout.Width(20));

                CTabBar tab = toggles[i] as CTabBar;
                if (tab != null)
                {
                    EditorGUI.BeginChangeCheck();
                    // if (tab.label != null){
                    //     tab.Text = EditorGUILayout.TextField(tab.Text, GUILayout.Width(100));
                    // }
                    if (EditorGUI.EndChangeCheck())
                    {
                        EditorUtility.SetDirty(tab);
                    }
                    EditorGUILayout.ObjectField(tab.gameObject, typeof(GameObject),true);
                }
                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("X", GUILayout.Width(22f)))
                {
                    Group.Remove(i);
                    EditorUtility.SetDirty(Group);
                }
                GUI.backgroundColor = Color.white;
                GUILayout.EndHorizontal();
                GUILayout.Space(4f);
            }
            
            if (toggles.Count > 0)
            {
                GUILayout.Space(6f);
            }
            GUILayout.BeginHorizontal();
            _label = EditorGUILayout.TextField(_label);
            if (GUILayout.Button("Add", GUILayout.Width(40f)))
            {
                Group.Add(toggles.Count,_label);
                _label = "";
                EditorUtility.SetDirty(Group);
            }
            GUILayout.EndHorizontal();
            if (toggles.Count == 0)
            {
                EditorGUILayout.HelpBox("当前没有标签页，请自行添加", MessageType.Info);
            }
            else GUILayout.Space(4f);
            MingUIEditorTools.EndContents();
        }
        EditorGUI.EndDisabledGroup();

        if (belongNav)
        {
            EditorGUILayout.HelpBox("当前标签页组已经被导航栏控制，不能操作", MessageType.Warning);
        }
    }
}
