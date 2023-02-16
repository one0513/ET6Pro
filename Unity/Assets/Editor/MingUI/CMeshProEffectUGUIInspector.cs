using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof (CMeshProEffectUGUI))]
public class CMeshProEffectUGUIInspector : Editor
{
    public CMeshProEffectUGUI _effect;
    public CMeshProEffectUGUI _Effect
    {
        get
        {
            if (_effect == null)
                _effect = target as CMeshProEffectUGUI;
            return _effect;
        }
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        //if (_Effect.text == null) _Effect.text = _Effect.transform.GetComponent<CMeshProLabel>();
    }
}
