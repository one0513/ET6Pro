using UnityEditor;

[CustomEditor(typeof(CFlexibleList))]
public class CListFlexibleInspector : CCanvasInspector
{
    protected override void OnPreInspectorGUI()
    {
        base.OnPreInspectorGUI();
        OnOwnInspectorGUI();
    }

    protected virtual void OnOwnInspectorGUI()
    {
        MingUIEditorTools.DrawProperty("LeftTop", serializedObject, "leftTop");
        MingUIEditorTools.DrawProperty("Pad", serializedObject, "pad");
        MingUIEditorTools.DrawProperty("ItemScale", serializedObject, "itemScale");
        MingUIEditorTools.DrawProperty("MaxSelectNum", serializedObject, "maxSelectNum");
        MingUIEditorTools.DrawProperty("DataCountLimit", serializedObject, "dataCountLimit");
        MingUIEditorTools.DrawProperty("RenderPrefab", serializedObject, "renderPrefab");
        MingUIEditorTools.DrawProperty("ManagerRenderSelect", serializedObject, "managerRenderSelect");
        MingUIEditorTools.DrawProperty("LoopUseItems", serializedObject, "loopItem");
        MingUIEditorTools.DrawProperty("ItemTweenAplha", serializedObject, "itemTweenAplha");
    }
}