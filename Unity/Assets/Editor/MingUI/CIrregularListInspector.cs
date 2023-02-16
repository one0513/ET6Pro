using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof (CIrregularList))]
public class CIrregularListInspector : CCanvasInspector
{
    private CIrregularList _CIrregularList
    {
        get { return target as CIrregularList; }
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
        if (_columns != _CIrregularList.Columns || _pad != _CIrregularList.pad || _leftTop != _CIrregularList.leftTop)
        {
            _CIrregularList.RefreshLayoutOnEditor();
        }
    }
    protected override void OnPreInspectorGUI()
    {
        base.OnPreInspectorGUI();
        OnOwnInspectorGUI();
    }
    private void UpdateValue()
    {
        _columns = _CIrregularList.Columns;
        _pad = _CIrregularList.pad;
        _leftTop = _CIrregularList.leftTop;
    }

    protected virtual void OnOwnInspectorGUI()
    {
        MingUIEditorTools.DrawProperty("Columns", serializedObject, "columns");
        MingUIEditorTools.DrawProperty("LeftTop", serializedObject, "leftTop");
        MingUIEditorTools.DrawProperty("Pad", serializedObject, "pad");
        MingUIEditorTools.DrawProperty("ItemScale", serializedObject, "itemScale");
        MingUIEditorTools.DrawProperty("MaxSelectNum", serializedObject, "maxSelectNum");

        MingUIEditorTools.DrawProperty("生成个数", serializedObject, "_eItemCount");
        if (UnityEngine.GUILayout.Button(m_RefreshButtonContent, EditorStyles.miniButton)) _CIrregularList.RefreshLayout();
        if (UnityEngine.GUILayout.Button(m_DeleButtonContent, EditorStyles.miniButton)) _CIrregularList.DeleCreatItem();
        MingUIEditorTools.DrawProperty("DataCountLimit", serializedObject, "dataCountLimit");
        MingUIEditorTools.DrawProperty("AutoCenter", serializedObject, "autoCenter");
        SerializedProperty sp = MingUIEditorTools.DrawProperty("DragMode", serializedObject, "dragMode");
        if (sp.enumValueIndex > 0)
        {
            MingUIEditorTools.DrawProperty("DragThreshold", serializedObject, "dragThreshold");
        }
        MingUIEditorTools.DrawProperty("RenderPrefabList", serializedObject, "renderPrefabList");
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
                UnityEngine.GUILayout.Space(EditorGUIUtility.labelWidth);
                
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndFadeGroup();



    }
}