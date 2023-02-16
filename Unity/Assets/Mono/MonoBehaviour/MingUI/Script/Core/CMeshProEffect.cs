using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(TMPro.TextMeshPro))]
public class CMeshProEffect : MonoBehaviour
{
    public TMPro.TextMeshPro text;

    [SerializeField]
    private Color _effectColor = Color.white;

    // [Range(-1, 1)]
    [HideInInspector]
    public float faceDilate;

    [SerializeField]
    [Range(0, 3)]
    private float outlineWidth;

    [SerializeField]
    [Range(0, 3)]
    private float shadowWidth;

    [HideInInspector]
    [Range(-3, 3)]
    public float shadowOffsetX = 0f;

    [Range(-3, 3)]
    public float shadowOffsetY = 0f;

    [HideInInspector]
    public float underlayDilate = 0f;


    public float OutlineWidth {
        get { return outlineWidth; }
        set {
            if (outlineWidth != value) {
                outlineWidth = value;
                RefreshValue();
            }
        }
    }

    public float ShadowWidth {
        get { return shadowWidth; }
        set {
            if (shadowWidth != value) {
                shadowWidth = value;
                RefreshValue();
            }
        }
    }

    private void Awake() {
        if (text == null) text = GetComponent<TMPro.TextMeshPro>();
    }

    private void OnValidate() {
        if (text == null) {
            return;
        }

        if (!this.enabled) {
            text.outlineWidth = 0;
 
            text.outlineColor = Color.white;
        } else {
            RefreshValue();
        }
    }

    private void OnEnable() {
        if (Application.isPlaying) {
            if (text == null) {
                text = transform.GetComponent<TMPro.TextMeshPro>();
            }

            RefreshValue();

            text.SetAllDirty();
        }
    }

    public void RefreshValue() {
        if (text != null) {
            
            text.outlineWidth = outlineWidth * (0.711f / 3);
          
            text.outlineColor = effectColor;
        }
    }

    public Color effectColor {
        get => _effectColor;
        set {
            _effectColor = value;
            RefreshValue();
        }
    }
}
