using UnityEditor;
using UnityEditor.UI;

namespace Assets.Editor.PublishDelScript.MingUI {
    [CustomEditor(typeof(CSlider))]
    class CSliderInspector : SliderEditor {
        private CSlider slider;
        public override void OnInspectorGUI() {
            serializedObject.Update();
            DoInspectorGUI();
            serializedObject.ApplyModifiedProperties();
            base.OnInspectorGUI();
        }

        private void DoInspectorGUI() {
            slider = target as CSlider;
            if (slider == null) return;
            slider.btnAdd = (CButton)EditorGUILayout.ObjectField("BtnAdd", slider.btnAdd, typeof(CButton), true);
            slider.btnSub = (CButton)EditorGUILayout.ObjectField("BtnSub", slider.btnSub, typeof(CButton), true);
            // slider.inputNum =
            //     (CTextInput)EditorGUILayout.ObjectField("InputNum", slider.inputNum, typeof(CTextInput), true);
        }
    }
}
