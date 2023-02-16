using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CPanel : UIBehaviour {
    public CButton btnClose;
    public CMeshProLabel txtTitle;

    private Action _onClose;

    private Canvas _canvas;
    private GraphicRaycaster _graphicRaycaster;
    private UIEventListener.IntDelegate _onSortingOrderChange;

    public void AddSortingOrderChange(UIEventListener.IntDelegate callback) {
        _onSortingOrderChange = callback;
    }

    public void RemoveSortingOrderChange() {
        _onSortingOrderChange = null;
    }

    public Canvas canvas {
        get {
            if (_canvas == null) {
                _canvas = MingUIUtil.AddCanvas(gameObject, 0);
            }
            return _canvas;
        }
    }

    protected GraphicRaycaster GraphicRaycaster {
        get {
            if (_graphicRaycaster == null) {
                _graphicRaycaster = canvas.GetComponent<GraphicRaycaster>();
                _graphicRaycaster.blockingObjects = GraphicRaycaster.BlockingObjects.ThreeD;
            }
            return _graphicRaycaster;
        }
    }

    protected virtual void Awake() {
        if (btnClose != null) {
            btnClose.Sound = "mainTab";
            btnClose.AddClick(OnCloseBtnClick);
        }
    }

    public int SortingOrder {
        get { return canvas.sortingOrder; }
        set {
            canvas.overrideSorting = true;
            canvas.sortingOrder = value;
            if (_onSortingOrderChange != null) {
                _onSortingOrderChange(value);
            }
        }
    }

    public bool IgnoreReversedGraphics {
        get { return GraphicRaycaster.ignoreReversedGraphics; }
        set { GraphicRaycaster.ignoreReversedGraphics = value; }
    }

    public void ShowClose(bool show) {
        if (btnClose != null) {
            btnClose.gameObject.SetActive(show);
        }
    }
    public void AddClose(Action callback) {
        _onClose = callback;
    }

    public void RemoveClose() {
        _onClose = null;
    }

    private void OnCloseBtnClick(PointerEventData value) {
        if (_onClose != null) _onClose();
    }

    protected virtual void OnDestroy() {
        _onClose = null;
    }

    public void SetTitle(string name) {
        if (txtTitle != null) {
            txtTitle.text = name;
        }
    }
}