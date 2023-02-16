using UnityEditor;

namespace Assets.Editor.MingUI
{
    [CustomEditor(typeof(CTree))]
    public class CTreeInspector : CCanvasInspector
    {
        protected override void OnPreInspectorGUI()
        {
            base.OnPreInspectorGUI();
            OnOwnInspectorGUI();
        }

        protected virtual void OnOwnInspectorGUI()
        {
            MingUIEditorTools.DrawProperty("pad", serializedObject, "pad");
            MingUIEditorTools.DrawProperty("childTreeOffset", serializedObject, "childTreeOffset");
            SerializedProperty sp = MingUIEditorTools.DrawProperty("dragMode", serializedObject, "dragMode");
            if (sp.enumValueIndex > 0)
            {
                MingUIEditorTools.DrawProperty("dragThreshold", serializedObject, "dragThreshold");
            }
            MingUIEditorTools.DrawProperty("renderPrefab", serializedObject, "renderPrefab");
            MingUIEditorTools.DrawProperty("autoFold", serializedObject, "autoFold");
            MingUIEditorTools.DrawProperty("autoSelectChildFirstRender", serializedObject, "autoSelectChildFirstRender");
        }
    }
}
