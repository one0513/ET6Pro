using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[ExecuteInEditMode]
[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public class CRectMaskDynamic2D : UIBehaviour, IClipper {

    readonly Vector3[] m_CanvasCorners = new Vector3[4];

    protected bool forceClip;
    protected bool lastValidClipRect;
    protected Canvas targetCanvas;
    protected Rect curRectForMask = new Rect();
    protected Rect lastClipRectCanvasSpace = new Rect();
    protected HashSet<IClippable> clipTarget = new HashSet<IClippable>();

    public void SetCanvas() {
        if(targetCanvas == null) {
            targetCanvas = GetComponent<Canvas>();
            lastValidClipRect = true;
        }
    }

    public void SetRectValid(bool isValid) {
        lastValidClipRect = isValid;
        forceClip = true;
    }

    public void SetCullRect(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4) {
        if (targetCanvas != null) {
            m_CanvasCorners[0] = targetCanvas.transform.InverseTransformPoint(p1);
            m_CanvasCorners[1] = targetCanvas.transform.InverseTransformPoint(p2);
            m_CanvasCorners[2] = targetCanvas.transform.InverseTransformPoint(p3);
            m_CanvasCorners[3] = targetCanvas.transform.InverseTransformPoint(p4);
            curRectForMask = new Rect(m_CanvasCorners[0].x, m_CanvasCorners[0].y, m_CanvasCorners[2].x - m_CanvasCorners[0].x, m_CanvasCorners[2].y - m_CanvasCorners[0].y);
        }
    }

    public virtual void PerformClipping() {
        bool clipRectChanged = curRectForMask != lastClipRectCanvasSpace;
        if (clipRectChanged || forceClip) {
            foreach (IClippable target in clipTarget)
                target.SetClipRect(curRectForMask, lastValidClipRect);
            lastClipRectCanvasSpace = curRectForMask;
        }

        foreach (IClippable target in clipTarget) {
            var maskable = target as MaskableGraphic;
            if (maskable != null && !maskable.canvasRenderer.hasMoved && !clipRectChanged)
                continue;
            target.Cull(curRectForMask, lastValidClipRect);
        }
    }

    public void AddClippable(IClippable clippable) {
        if (clippable == null)
            return;
        if (!clipTarget.Contains(clippable))
            clipTarget.Add(clippable);
        forceClip = true;
    }

    public void RemoveClippable(IClippable clippable) {
        if (clippable == null)
            return;
        clippable.SetClipRect(new Rect(), false);
        clipTarget.Remove(clippable);
        forceClip = true;
    }

    protected override void OnEnable() {
        base.OnEnable();
        ClipperRegistry.Register(this);
    }

    protected override void OnDisable() {
        base.OnDisable();
        ClipperRegistry.Unregister(this);
    }
}
