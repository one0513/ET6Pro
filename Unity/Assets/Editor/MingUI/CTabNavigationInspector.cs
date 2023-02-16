using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof (CTabNavigation), true)]
public class CTabNavigationInspector : Editor
{
    private CTabNavigation Nav
    {
        get { return target as CTabNavigation; }
    }
    private string _label;
    private GameObject _view;
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SerializedProperty sp = MingUIEditorTools.DrawProperty("BindMode", serializedObject, "mode");
        CTabNavigation.BindMode mode = (CTabNavigation.BindMode)sp.enumValueIndex;
        sp =  MingUIEditorTools.DrawProperty("TabBars", serializedObject, "tabBars");
        CTabGroup group = sp.objectReferenceValue as CTabGroup;

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Sheild",GUILayout.Width(80));
        Nav.shieldMaskPrefab = EditorGUILayout.ObjectField(Nav.shieldMaskPrefab, typeof(GameObject), false) as GameObject;
        EditorGUILayout.EndHorizontal();
        MingUIEditorTools.DrawProperty("ShieldMaskPos", serializedObject, "shieldMaskPos");
        if (group == null)
        {
            EditorGUILayout.HelpBox("标签页组不能为空！", MessageType.Error);
        }
        else
        {
            List<Toggle> toggles = group.toggleList;
            if (MingUIEditorTools.DrawHeader("导航栏"))
            {
                MingUIEditorTools.BeginContents();
                List<GameObject> views = Nav.prefabList;

                for (int i = 0; i < views.Count; i++)
                {
                    GameObject view = views[i];

                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(i.ToString(), GUILayout.Width(20));

                    CTabBar tab = toggles[i] as CTabBar;
                    if (tab != null)
                    {
                        EditorGUI.BeginChangeCheck();
                        tab.Text = EditorGUILayout.TextField(tab.Text, mode == CTabNavigation.BindMode.Directly ? GUILayout.Width(100):GUILayout.ExpandWidth(true));
                        if (EditorGUI.EndChangeCheck())
                        {
                            EditorUtility.SetDirty(Nav.gameObject);
                        }
                    }

                    if (mode == CTabNavigation.BindMode.Directly)
                    {
                        views[i] = EditorGUILayout.ObjectField(view, typeof(GameObject), false) as GameObject;
                    }

                    GUI.backgroundColor = Color.red;
                    if (GUILayout.Button("X", GUILayout.Width(22f)))
                    {
                        Nav.prefabList.Remove(view);
                        Nav.tabBars.Remove(i);
                        EditorUtility.SetDirty(Nav.gameObject);
                    }
                    GUI.backgroundColor = Color.white;
                    GUILayout.EndHorizontal();
                    GUILayout.Space(4f);
                }
                if (views.Count > 0)
                {
                    GUILayout.Space(6f);
                }
                GUILayout.BeginHorizontal();
                _label = EditorGUILayout.TextField(_label);
                if (mode == CTabNavigation.BindMode.Directly)
                    _view = EditorGUILayout.ObjectField(_view, typeof(GameObject), false) as GameObject;

                if (GUILayout.Button("Add", GUILayout.Width(40f)))
                {
                    Nav.tabBars.Add(views.Count, _label);
                    views.Add(_view);
                    EditorUtility.SetDirty(Nav.gameObject);
                    _label = "";
                    _view = null;
                }
                GUILayout.EndHorizontal();
                if (views.Count == 0)
                {
                    EditorGUILayout.HelpBox("当前没有导航，请自行添加", MessageType.Info);
                }
                else GUILayout.Space(4f);
                MingUIEditorTools.EndContents();
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
