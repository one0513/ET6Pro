using UnityEditor;

[InitializeOnLoad]
public class MingUIAgentEditor
{
    static MingUIAgentEditor()
    {
        MingUIAgent.IsEditorMode = true;
        MingUIAgent.SetEditorSetDirty(EditorUtility.SetDirty);
    }
}
