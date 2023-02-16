using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class CSelectForInput : CSelect {

    private CTextInput insideInput;
    private bool hasInsideInput = false;
    private bool isMainSel = false;
    private bool isSubSel = false;

    public void CheckInputComponent() {
        insideInput = GetComponentInChildren<CTextInput>();
        if(insideInput != null) {
            hasInsideInput = true;
            insideInput.AddSelect(OnInputSelect);
        } else {
            hasInsideInput = false;
        }
    }

    public void CheckSelectMain() {
        if (hasInsideInput) {
            isMainSel = true;
        }
    }

    public void OnInputSelect(bool isSelect) {
        if (isSelect) {
            isSubSel = true;
            CheckHandleSelectCallBack(true);
        } else {
            isSubSel = false;
            StartCoroutine(DoSubSelectCallBackNextFrame());
        }
    }

    public override void OnSelect(BaseEventData eventData) {
        if (hasInsideInput) {
            isMainSel = true;
            CheckHandleSelectCallBack(true);
        } else {
            base.OnSelect(eventData);
        }
    }

    public override void OnDeselect(BaseEventData eventData) {
        if (hasInsideInput) {
            isMainSel = false;
            if (ignoreChildSelect) {
                PointerEventData pEData = eventData as PointerEventData;
                if (pEData != null && pEData.pointerCurrentRaycast.gameObject != null) {
                    Transform target = pEData.pointerCurrentRaycast.gameObject.transform;
                    if (target.IsChildOf(this.transform)) {
                        if (target.gameObject == insideInput.gameObject || target.IsChildOf(insideInput.transform)) {
                            CheckHandleSelectCallBack(false);
                        } else {
                            StartCoroutine(ReturnSelect());
                            return;
                        }
                    } else {
                        CheckHandleSelectCallBack(false);
                    }
                } else {
                    if(eventData.selectedObject != null) {
                        if(eventData.selectedObject == gameObject || eventData.selectedObject.transform.IsChildOf(transform)) {
                            CheckHandleSelectCallBack(false);
                        } else {
                            StartCoroutine(ReturnSelect());
                            return;
                        }
                    } else {
                        CheckHandleSelectCallBack(false);
                    }
                }
            } else {
                CheckHandleSelectCallBack(false);
            }
        } else {
            base.OnDeselect(eventData);
        }
    }

    private void CheckHandleSelectCallBack(bool isSelect) {
        if (hasInsideInput) {
            if (isSelect) {
                HandleSelectCallBack(isSelect);
            } else {
                StartCoroutine(DoSelectCallBackNextFrame());
            }
        } else {
            HandleSelectCallBack(isSelect);
        }
    }

    IEnumerator DoSelectCallBackNextFrame() {
        yield return 0;
        if (!isMainSel && !isSubSel) {
            HandleSelectCallBack(false);
        }
        yield return 0;
    }


    IEnumerator DoSubSelectCallBackNextFrame() {
        yield return 0;
        if (EventSystem.current.currentSelectedGameObject) {
            if (EventSystem.current.currentSelectedGameObject == this.gameObject) {

            } else if (EventSystem.current.currentSelectedGameObject.transform.IsChildOf(transform)) {
                EventSystem.current.SetSelectedGameObject(gameObject);
            } else {
                if (!isMainSel && !isSubSel) {
                    HandleSelectCallBack(false);
                }
            }
        } else {
            EventSystem.current.SetSelectedGameObject(gameObject);
        }
        yield return 0;
    }
}

