using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CMeshProLabel))]
public class CMeshProEffectUGUI : MonoBehaviour {
    public CMeshProLabel text;

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
        if (text == null) text = GetComponent<CMeshProLabel>();
    }

    private void OnValidate() {
        if (text == null) {
            return;
        }

        if (!this.enabled) {
            text.outlineWidth = 0;
            text.underlayOffsetX = 0;
            text.underlayOffsetY = 0;
            text.underlayDilate = 0;
            text.outlineColor = Color.white;
        } else {
            RefreshValue();
        }
    }

    private void OnEnable() {
        if (Application.isPlaying) {
            if (text == null) {
                text = transform.GetComponent<CMeshProLabel>();
            }

            RefreshValue();
            text.UpdatePaddind();
            text.SetAllDirty();
        }
    }

    public void RefreshValue() {
        if (text != null) {
            text.faceDilate = faceDilate;
            text.outlineWidth = outlineWidth * (0.711f / 3);
            text.underlayOffsetX = shadowWidth * (0.405f / 3);
            text.underlayOffsetY = shadowOffsetY * (1.71f / 3);
            text.underlayDilate = underlayDilate;
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
