using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

[CustomEditor(typeof(CSpriteTile), false)]
public class CSpriteTileInspector : ImageEditor {

    private CSprite _sprite;

    public override void OnInspectorGUI() {
        serializedObject.Update();
        OnPreInspectorGUI();
        serializedObject.ApplyModifiedProperties();

        base.OnInspectorGUI();

        serializedObject.Update();
        OnAfterInspectorGUI();
        serializedObject.ApplyModifiedProperties();
    }

    protected virtual void OnAfterInspectorGUI() {
        MingUIEditorTools.DrawProperty("MAX_TILE_X", serializedObject, "MAX_TILE_X");
        MingUIEditorTools.DrawProperty("MAX_TILE_Z", serializedObject, "MAX_TILE_Z");
        MingUIEditorTools.DrawProperty("TILE_SIZE", serializedObject, "TILE_SIZE");
        MingUIEditorTools.DrawProperty("defaultIndex", serializedObject, "defaultIndex");
    }

    protected virtual void OnPreInspectorGUI() {
        _sprite = target as CSprite;
        if (_sprite == null) return;
    }
}
