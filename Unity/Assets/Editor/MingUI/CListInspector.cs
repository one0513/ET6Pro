using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof (CList))]
public class CListInspector : CCanvasInspector
{
    private CList _CList
    {
        get { return target as CList; }
    }
    private int _columns;
    private Vector2 _pad, _leftTop;
    //private UnityEditor.AnimatedValues.AnimBool m_ShowRefreshLayout;
    protected override void OnEnable()
    {
        base.OnEnable();
        m_RefreshButtonContent = EditorGUIUtility.TrTextContent("刷新", "刷新布局");
        m_DeleButtonContent = EditorGUIUtility.TrTextContent("删除", "删除手动生成的Render");
    }
    public override void OnInspectorGUI()
    {
        UpdateValue();
        base.OnInspectorGUI();
        if (_columns != _CList.Columns || _pad != _CList.pad || _leftTop != _CList.leftTop)
        {
            _CList.RefreshLayoutOnEditor();
        }
    }
    protected override void OnPreInspectorGUI()
    {
        base.OnPreInspectorGUI();
        OnOwnInspectorGUI();
    }
    private void UpdateValue()
    {
        _columns = _CList.Columns;
        _pad = _CList.pad;
        _leftTop = _CList.leftTop;
    }

    protected virtual void OnOwnInspectorGUI()
    {
        MingUIEditorTools.DrawProperty("正向有序排列", serializedObject, "orderedArrangement");
        MingUIEditorTools.DrawProperty("不定高度", serializedObject, "unequalHeight");
        if(_CList.UnequalHeight)
        {
            _CList.Columns = 1;
        }
        else
        {
            MingUIEditorTools.DrawProperty("Columns", serializedObject, "columns");
        }
        MingUIEditorTools.DrawProperty("LeftTop", serializedObject, "leftTop");
        MingUIEditorTools.DrawProperty("Pad", serializedObject, "pad");
        MingUIEditorTools.DrawProperty("ItemScale", serializedObject, "itemScale");
        MingUIEditorTools.DrawProperty("MaxSelectNum", serializedObject, "maxSelectNum");

        MingUIEditorTools.DrawProperty("生成个数", serializedObject, "_eItemCount");
        if (UnityEngine.GUILayout.Button(m_RefreshButtonContent, EditorStyles.miniButton)) _CList.RefreshLayout();
        if (UnityEngine.GUILayout.Button(m_DeleButtonContent, EditorStyles.miniButton)) _CList.DeleCreatItem();
        MingUIEditorTools.DrawProperty("DataCountLimit", serializedObject, "dataCountLimit");
        MingUIEditorTools.DrawProperty("AutoCenter", serializedObject, "autoCenter");
        SerializedProperty sp = MingUIEditorTools.DrawProperty("DragMode", serializedObject, "dragMode");
        if (sp.enumValueIndex > 0)
        {
            MingUIEditorTools.DrawProperty("DragThreshold", serializedObject, "dragThreshold");
        }
        MingUIEditorTools.DrawProperty("DragStartDir", serializedObject, "dragStartDir");
        MingUIEditorTools.DrawProperty("RenderPrefab", serializedObject, "renderPrefab");
        MingUIEditorTools.DrawProperty("EmptyPrefab", serializedObject, "emptyRenderPrefab");
        MingUIEditorTools.DrawProperty("ManagerRenderSelect", serializedObject, "managerRenderSelect");
        MingUIEditorTools.DrawProperty("LoopUseItems", serializedObject, "loopItem");
        MingUIEditorTools.DrawProperty("ItemTweenAplha", serializedObject, "itemTweenAplha");
        
    }
    private UnityEngine.GUIContent m_RefreshButtonContent;
    private UnityEngine.GUIContent m_DeleButtonContent;
    protected void RefreshLayoutButtonGUI()
    {
        if (EditorGUILayout.BeginFadeGroup(1))
        {
            EditorGUILayout.BeginHorizontal();
            {
                //UnityEngine.GUILayout.Space(EditorGUIUtility.labelWidth);
                
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndFadeGroup();



    }
}