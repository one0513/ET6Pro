using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 特效显示封装
/// </summary>
public class CEffect : MaskableGraphic
{
    private int _sortingOrder = -1;
    private bool _asParticleGraphic = false;

    public RectTransform _maskTrans;
    public RectTransform customMaskTrans;//支持自定义mask矩形
    private bool _maskChanged = false;
    private Vector3 _lossScale;
    private Vector3 _pos;
    private Rect _rect;
    private bool isDirty = false;
    private bool hadShadermodify = false;

    private CanvasGroup canvasGroup;
    private Renderer[] renderers;
    private EffectPropertyItem[] propertyItems;
    private bool isDestroy = false;

    private float lastCanvasGroupAlpha = 0;
    private bool synCanvasGroupAlpha = false;

    public bool asParticleGraphic
    {
        get { return _asParticleGraphic; }
    }
    public int sortingOrder
    {
        get { return _sortingOrder; }
        set { _sortingOrder = value; }
    }
    public bool SynCanvasGroupAlpha
    {
        get { return synCanvasGroupAlpha; }
        set {
            synCanvasGroupAlpha = value;
            CheckCanvasGroup();
        }
    }

    public bool IsDestroy
    {
        get { return isDestroy; }
    }

    public void Show(bool asParticleGraphic = false)
    {
        asParticleGraphic = false;
    }

    public void SetEffectAlpha(float alpha) {
        InitRenderers();
        if (propertyItems != null) {
            for (int i = 0; i < propertyItems.Length; i++) {
                propertyItems[i].SetAlpha(alpha);
            }
        }
    }

    private void OnEnable()
    {
        base.OnEnable();
        FindParentMask();
        CheckCanvasGroup();
    }

    protected override void OnDestroy() {
        base.OnDestroy();
        isDestroy = true;
    }

    public void SetDirty() {
        isDirty = true;
    }

    private void InitRenderers() {
        if (renderers == null) {
            renderers = this.GetComponentsInChildren<Renderer>();
        }
        if(renderers != null && propertyItems == null) {
            propertyItems = new EffectPropertyItem[renderers.Length];
            for (int i = 0; i < renderers.Length; i++) {
                var item = renderers[i].gameObject.GetComponent<EffectPropertyItem>();
                if(item == null) {
                    item = renderers[i].gameObject.AddComponent<EffectPropertyItem>();
                }
                propertyItems[i] = item;
            }
        }
    }

    public void OnEffectPropertyItemDestroy() {
        if (propertyItems != null) {
            propertyItems = null;
        }
        if(renderers != null) {
            renderers = null;
        }
    }

    void CullMask()
    {
        if (_maskTrans != null)
        {
            _lossScale = _maskTrans.lossyScale;
            _pos = _maskTrans.position;
            _rect = _maskTrans.rect;

            Vector3[] corners = new Vector3[4];
            _maskTrans.GetWorldCorners(corners);
            DoCull(corners[0].x, corners[0].y, corners[2].x, corners[2].y);
        }
        else
        {
            CancelCull();
        }
        _maskChanged = false;
    }
    private void CancelCull() {
        InitRenderers();
        if (propertyItems != null) {
            for (int i = 0; i < propertyItems.Length; i++) {
                propertyItems[i].SetClipable(false);
                propertyItems[i].SetClipRect(new Vector4(-10, 10, -10, 10));
            }
        }
    }
    private void DoCull(float minX, float minY, float maxX, float maxY)
    {
        InitRenderers();
        if (propertyItems != null) {
            for (int i = 0; i < propertyItems.Length; i++) {

                propertyItems[i].SetClipable(true);
                propertyItems[i].SetClipRect(new Vector4(minX, maxX, minY, maxY));

                hadShadermodify = true;
            }
        }
        if (hadShadermodify) {
            CheckCanvasGroup();
        }
    }
    private void OnDisable()
    {
        base.OnDisable();
        _maskTrans = null;
        _maskChanged = false;
        _lossScale = Vector3.zero;
        _pos = Vector3.zero;
        _rect = Rect.zero;
        canvasGroup = null;
        lastCanvasGroupAlpha = 0;
        hadShadermodify = false;
    }
    protected override void OnTransformParentChanged()
    {
        base.OnTransformParentChanged();
        FindParentMask();
    }

    protected override void OnCanvasGroupChanged() {
        if (synCanvasGroupAlpha) {
            CheckCanvasGroup();
        }
    }

    public void CheckCanvasGroup() {
        if (synCanvasGroupAlpha && hadShadermodify) {
            if (canvasGroup == null) {
                canvasGroup = GetComponentInParent<CanvasGroup>();
                if (canvasGroup) {
                    lastCanvasGroupAlpha = canvasGroup.alpha;
                    InitRenderers();
                    if (propertyItems != null) {
                        for (int n = 0; n < propertyItems.Length; n++) {
                            propertyItems[n].SetAlpha(lastCanvasGroupAlpha);
                        }
                    }
                }
            } else {
                if (synCanvasGroupAlpha && canvasGroup != null) {
                    if (lastCanvasGroupAlpha != canvasGroup.alpha) {
                        lastCanvasGroupAlpha = canvasGroup.alpha;
                        InitRenderers();
                        if (propertyItems != null) {
                            for (int n = 0; n < propertyItems.Length; n++) {
                                propertyItems[n].SetAlpha(lastCanvasGroupAlpha);
                            }
                        }
                    }
                }
            }
        } else {
            if (canvasGroup != null) {
                canvasGroup = null;
                lastCanvasGroupAlpha = 0;
            }
        }
    }

    public void FindParentMask()
    {
        if (customMaskTrans != null && _maskTrans != customMaskTrans) {
            _maskTrans = customMaskTrans;
            _maskChanged = true;
            return;
        }
        CContainer container = this.GetComponentInParent<CContainer>();
        if (container == null)
        {
            if (_maskTrans != null)
            {
                _maskTrans = null;
                _maskChanged = true;
            } else {
                _maskChanged = true;
            }
        }
        else if (_maskTrans != container.SelfTransform)
        {
            _maskTrans = container.SelfTransform;
            _maskChanged = true;
        }
    }
    private void Update()
    {
        //同步sortingOrder
        if (isDirty || (canvas != null && canvas.sortingOrder != _sortingOrder))
        {
            if (canvas != null)
            {
                isDirty = false;
                _sortingOrder = canvas.sortingOrder;
                InitRenderers();
                if (renderers != null) {
                    foreach (Renderer render in renderers) {
                        if (render.sharedMaterial != null) {
                            render.sharedMaterial.renderQueue = 3000;//3000是默认的Transparent
                        }
                        render.sortingOrder = _sortingOrder + 1;
                    }
                }
            }
        }
        //同步裁剪区域
        if (_maskChanged == false && _maskTrans != null)
        {
            _maskChanged = (_lossScale != _maskTrans.lossyScale || _pos != _maskTrans.position || _rect != _maskTrans.rect);
        }

        if(_maskChanged)
        {
            CullMask();
        }
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
    }
}

