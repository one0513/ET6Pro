using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(CToggleGroup), true)]
public class CToggleGroupInspector : Editor {
    private CToggleGroup Group {
        get { return target as CToggleGroup; }
    }

    private int _columns;
    private Vector2 _pad;

    public override void OnInspectorGUI() {
        serializedObject.Update();
        _columns = Group.columns;
        _pad = Group.pad;
        MingUIEditorTools.DrawProperty("AllowSwitchOff", serializedObject, "m_AllowSwitchOff");
        SerializedProperty sp = MingUIEditorTools.DrawProperty("AutoLayout", serializedObject, "autoLayout");
        if (sp.boolValue)//自动排列
        {
            MingUIEditorTools.DrawProperty("Columns", serializedObject, "columns");
            MingUIEditorTools.DrawProperty("Pad", serializedObject, "pad");
        }
        SerializedProperty extrudingAffect = MingUIEditorTools.DrawProperty("是否开启点击后布局效果", serializedObject, "isShowExtrudingAffect");
        if (extrudingAffect.boolValue)
        {
            MingUIEditorTools.DrawProperty("运动方向", serializedObject, "axes");
            MingUIEditorTools.DrawProperty("Scale缩放", serializedObject, "tabBarScale");
            MingUIEditorTools.DrawProperty("Scale缩放时间", serializedObject, "scaleDurationTime");
            MingUIEditorTools.DrawProperty("Scale缩放Ease", serializedObject, "scaleEase");
            MingUIEditorTools.DrawProperty("移动时间", serializedObject, "moveDurationTime");
            MingUIEditorTools.DrawProperty("移动Ease", serializedObject, "moveEase");
        }

        MingUIEditorTools.DrawProperty("ToggleRender", serializedObject, "toggleRender");


        OnCustomInpector();
        serializedObject.ApplyModifiedProperties();

        if (_columns != Group.columns || _pad != Group.pad) {
            Group.Reposition();
        }
        if (GUILayout.Button("刷新Toggles")) {
            Group.RefreshToggleList();
            EditorUtility.SetDirty(Group);
        }
    }

    protected virtual void OnCustomInpector() {
        if (MingUIEditorTools.DrawHeader("Toggle")) {
            MingUIEditorTools.BeginContents();
            List<Toggle> toggles = Group.toggleList;
            for (int i = 0; i < toggles.Count; i++) {
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(i.ToString(), GUILayout.Width(20));

                Toggle toggle = toggles[i];
                if (toggle != null) {
                    EditorGUILayout.ObjectField(toggle.gameObject, typeof(GameObject), true);
                }
                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("X", GUILayout.Width(22f))) {
                    Group.Remove(i);
                }
                GUI.backgroundColor = Color.white;
                GUILayout.EndHorizontal();
                GUILayout.Space(4f);
            }

            if (toggles.Count > 0) {
                GUILayout.Space(6f);
            }
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add")) {
                Group.Add(toggles.Count);
            }
            GUILayout.EndHorizontal();
            if (toggles.Count == 0) {
                EditorGUILayout.HelpBox("当前没有Toggle，请自行添加", MessageType.Info);
            } else GUILayout.Space(4f);
            MingUIEditorTools.EndContents();
        }
    }
    //public void OnSceneGUI()
    //{
    //    MingUIEditorTools.HideMoveTool(true);
    //    Event e = Event.current;
    //    if (e.type == EventType.MouseUp && e.button == 1)
    //    {
    //        MingUIContextMenu.AddItem("AddItem", false, AddToggle, null);
    //        MingUIContextMenu.AddItem("RemoveAll", false, RemoveAll, null);
    //        MingUIContextMenu.AddItem("Reposition", false, Reposition, null);
    //        MingUIContextMenu.Show();
    //    }
    //}

    //private void AddToggle(object obj)
    //{
    //    Group.Add();
    //}

    //private void RemoveAll(object obj)
    //{
    //    Group.RemoveAll();
    //}

    //private void Reposition(object obj)
    //{
    //    Group.Reposition();
    //}

    //private void OnDisable()
    //{
    //    //MingUIEditorTools.HideMoveTool(false);
    //}
}