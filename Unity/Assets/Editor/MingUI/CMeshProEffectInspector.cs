using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

[CustomEditor(typeof (CMeshProEffect))]
public class CMeshProEffectInspector : Editor
{
    public CMeshProEffect _effect;
    public CMeshProEffect _Effect
    {
        get
        {
            if (_effect == null)
                _effect = target as CMeshProEffect;
            return _effect;
        }
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        //if (_Effect.text == null) _Effect.text = _Effect.transform.GetComponent<TextMeshPro>();
    }
}
