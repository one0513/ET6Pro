using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

[CustomEditor(typeof(CBezierCurve), false)]
public class CBezierCurveInspector : ImageEditor {

    private CBezierCurve _sprite;
    private bool showAnchord = true;
    private bool anchoredActive = false;

    public Color Col_Ctrl = new Color(0, 1, 1, 0.3f);
    public Color Col_Anchor = new Color(0, 0, 1, 0.3f);

    public override void OnInspectorGUI() {
        serializedObject.Update();
        OnPreInspectorGUI();
        serializedObject.ApplyModifiedProperties();

        base.OnInspectorGUI();

        serializedObject.Update();
        OnAfterInspectorGUI();
        serializedObject.ApplyModifiedProperties();
    }

    public void OnSceneGUI() {
        _sprite = target as CBezierCurve;
        if (_sprite == null) return;

        RectTransform gui = _sprite.rectTransform;
        Transform parentSpace = gui.transform;
        if (showAnchord) {
            Handles.color = Color.yellow;
            CurveAnchoredGui(_sprite.anchorPos1, _sprite.controlPos1);
            CurveAnchoredGui(_sprite.anchorPos2, _sprite.controlPos2);

            Handles.color = Col_Anchor;
            AnchorSceneGUI(gui, parentSpace, _sprite.anchorPos1, anchoredActive, 0, 0, GUIUtility.GetControlID(FocusType.Passive), "anchorPos1", 2);
            AnchorSceneGUI(gui, parentSpace, _sprite.anchorPos2, anchoredActive, 0, 1, GUIUtility.GetControlID(FocusType.Passive), "anchorPos2", 2);
        }
        Handles.color = Col_Ctrl;
        AnchorSceneGUI(gui, parentSpace, _sprite.controlPos1, true, 0, 0, GUIUtility.GetControlID(FocusType.Passive), "controlPos1", 4);
        AnchorSceneGUI(gui, parentSpace, _sprite.controlPos2, true, 0, 1, GUIUtility.GetControlID(FocusType.Passive), "controlPos2", 4);
    }

    void AnchorSceneGUI(RectTransform gui, Transform parentSpace, Vector2 offset, bool interactive, int minmaxX, int minmaxY, int id, string property, int scale) {
        Vector3 curPos = _sprite.rectTransform.position;
        curPos.x += offset.x * _sprite.canvas.rootCanvas.transform.localScale.x;
        curPos.y += offset.y * _sprite.canvas.rootCanvas.transform.localScale.y;
        float size = 0.05f * HandleUtility.GetHandleSize(curPos) * scale;
        Handles.DotHandleCap(id, curPos, _sprite.rectTransform.rotation, size, EventType.Repaint);

        if (!interactive)
            return;

        Event evtCopy = new Event(Event.current);

        EditorGUI.BeginChangeCheck();
        Vector3 newPos = Handles.Slider2D(id, curPos, parentSpace.forward, parentSpace.right, parentSpace.up, size * 2, (Handles.CapFunction)null, Vector2.zero);

        if (evtCopy.type == EventType.MouseDown && GUIUtility.hotControl == id) {

            //Debug.Log("mouse down");
        }

        if (EditorGUI.EndChangeCheck()) {
            //Debug.Log("EndChangeCheck down");
            Vector2 newOffset = newPos - curPos;
            serializedObject.FindProperty(property).vector2Value = offset + new Vector2(newOffset.x / _sprite.canvas.rootCanvas.transform.localScale.x, newOffset.y / _sprite.canvas.rootCanvas.transform.localScale.y);
            serializedObject.ApplyModifiedProperties();
        }
    }

    void CurveAnchoredGui(Vector2 begin, Vector2 end) {
        Vector3 pos1 = _sprite.rectTransform.position;
        pos1.x += begin.x * _sprite.canvas.rootCanvas.transform.localScale.x;
        pos1.y += begin.y * _sprite.canvas.rootCanvas.transform.localScale.y;

        Vector3 pos2 = _sprite.rectTransform.position;
        pos2.x += end.x * _sprite.canvas.rootCanvas.transform.localScale.x;
        pos2.y += end.y * _sprite.canvas.rootCanvas.transform.localScale.y;

        Handles.DrawLine(pos1, pos2);
    }

    protected virtual void OnAfterInspectorGUI() {
        MingUIEditorTools.DrawProperty("RaycastConsiderAlpha", serializedObject, "raycastConsiderAlpha");
        _sprite.Gray = EditorGUILayout.Toggle("Gray", _sprite.Gray);
        _sprite.CheckAtlasSprite();
        MingUIEditorTools.DrawProperty("anchorPos1", serializedObject, "anchorPos1");
        MingUIEditorTools.DrawProperty("anchorPos2", serializedObject, "anchorPos2");
        MingUIEditorTools.DrawProperty("controlPos1", serializedObject, "controlPos1");
        MingUIEditorTools.DrawProperty("controlPos2", serializedObject, "controlPos2");
        MingUIEditorTools.DrawProperty("curSegments", serializedObject, "curSegments");
        MingUIEditorTools.DrawProperty("width", serializedObject, "width");
        MingUIEditorTools.DrawProperty("showBeginIndex", serializedObject, "showBeginIndex");
        MingUIEditorTools.DrawProperty("useFast", serializedObject, "useFast");
        MingUIEditorTools.DrawProperty("useWeld", serializedObject, "useWeld");
        MingUIEditorTools.DrawProperty("useHeadBorder", serializedObject, "useHeadBorder");
        MingUIEditorTools.DrawProperty("useReverse", serializedObject, "useReverse");
        if (showAnchord) {
            anchoredActive = GUILayout.Toggle(anchoredActive, new GUIContent("anchored active"));
            if (GUILayout.Button("Hide Anchored")) {
                showAnchord = false;
                SceneView.RepaintAll();
            }
        } else {
            if (GUILayout.Button("Show Anchored")) {
                showAnchord = true;
                SceneView.RepaintAll();
            }
        }
    }

    protected virtual void OnPreInspectorGUI() {
        _sprite = target as CBezierCurve;
        if (_sprite == null) return;
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Atlas", "DropDown", GUILayout.Width(55f))) {
            ComponentSelector.Show<CAtlas>(OnSelectAtlas);
        }

        SerializedProperty sp = MingUIEditorTools.DrawProperty("", serializedObject, "_atlas", GUILayout.MinWidth(20f));
        CAtlas nowAtlas = sp.objectReferenceValue as CAtlas;
        _sprite.Atlas = nowAtlas;
        _sprite.RefreshAtlasName();
        if (GUILayout.Button("Edit", GUILayout.Width(40f))) {
            if (nowAtlas != null) {
                SpriteEditor.Open(nowAtlas.mainTexture);
            }
        }
        GUILayout.EndHorizontal();
        ////////////////////////////////////////////////////////////////////////////////
        if (nowAtlas == null) //无图集
        {
            OnSelectAtlas(null);
            SelectSprite(null);
        } else {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Sprite", "DropDown", GUILayout.Width(55f))) {
                SpriteSelector.Show(nowAtlas, _sprite.SpriteName, SelectSprite);
            }
            
            GUILayout.Label(_sprite.SpriteName, "HelpBox", GUILayout.Height(18f), GUILayout.MinWidth(20f));
            
            if (GUILayout.Button("Edit", GUILayout.Width(40f))) {
                if (_sprite.sprite != null) {
                    SpriteEditor.Open(_sprite.sprite);
                }
            }
            GUILayout.EndHorizontal();
        }
        
        MingUIEditorTools.DrawProperty("AutoSnap", serializedObject, "autoSnap");
        MingUIEditorTools.DrawProperty("ShowWhiteSource", serializedObject, "showWhiteSource");
    }

    private void OnSelectAtlas(Object obj) {
        if (_sprite.Atlas != obj) {
            _sprite.Atlas = obj as CAtlas;
            EditorUtility.SetDirty(_sprite);
        }
    }

    private void SelectSprite(string spriteName) {
        if (_sprite.SpriteName != spriteName) {
            _sprite.SpriteName = spriteName;
            EditorUtility.SetDirty(_sprite);
        }

    }
}
