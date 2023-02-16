using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Editor.PublishDelScript.MingUI {
    [CustomEditor(typeof(RectTransform))]
    class RectTransformInspector :UnityEditor.Editor {
        private UnityEditor.Editor editor;
        private MethodInfo onSceneGUIMethod;

        void OnEnable() {
            editor = UnityEditor.Editor.CreateEditor(target, Assembly.GetAssembly(typeof(UnityEditor.Editor)).GetType("UnityEditor.RectTransformEditor", true));
            onSceneGUIMethod = editor.GetType().GetMethod("OnSceneGUI", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        public override void OnInspectorGUI() {
            var rect = target as RectTransform;
            if (rect == null) return;
            serializedObject.Update();
            editor.OnInspectorGUI();
            EditorGUILayout.Vector2Field("anchoredPosition", rect.anchoredPosition);
        }

        private void OnSceneGUI() {
            onSceneGUIMethod.Invoke(editor,null);
            var rect = this.target as RectTransform;
            var e = Event.current;

            var box = rect.rect;
            var guipos = HandleUtility.WorldToGUIPoint(rect.TransformPoint(box.position));
            var guipos2 = HandleUtility.WorldToGUIPoint(rect.TransformPoint(box.position+box.size));

            box.size = guipos2 - guipos;
            box.position = guipos;
            if (box.height < 0) {
                box.y = box.position.y + box.height;
                box.height = -box.height;
            }

            if (box.Contains(e.mousePosition)) {
                if (e.type == EventType.MouseDown && e.button == 1) {
                    MingUIMenu.InitSceneMenu();
                    MingUIMenu.Show();
                }
            }
        }

        private void OnDestroy() {
            UnityEditor.Editor.DestroyImmediate(editor);
        }
    }
}
