using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CToggle : UIBehaviour,IPointerClickHandler {
    public Graphic offBg;
    public Graphic offFront;
    public Graphic onBg;
    public Graphic onFront;
    public float duration = 0.3f;


    private bool _isOn = false;
    private float _width;
    private float _height;

    private Vector3 _offSrcPosition;
    private Vector3 _onSrcPosition;

    private UIEventListener.BoolDelegate _changeFunc;

    protected override void Start() {
        var rt = GetComponent<RectTransform>();
        _width = rt.rect.size.x;
        _height = rt.rect.size.y;

        offFront.rectTransform.pivot =new Vector2(1,0.5f);
        _offSrcPosition = offFront.rectTransform.localPosition;
        onFront.rectTransform.pivot =new Vector2(1,0.5f);
        _onSrcPosition = onFront.rectTransform.localPosition;
    }

    public void OnPointerClick(PointerEventData eventData) {
        IsOn = !IsOn;
    }

    private void DoTween() {
        if (IsOn) {
            DoOnTween();
        } else {
            DoOffTween();
        }
    }

    private void DoOnTween() {
        DoKill();
        offFront.rectTransform.DOLocalMoveX(_width / 2, duration).SetEase(Ease.OutSine);
        offFront.DOFade(0, duration);
        offBg.DOFade(0, duration);

        onFront.rectTransform.localPosition = _onSrcPosition;
        onFront.DOFade(1, duration*0.8f).SetEase(Ease.InQuad);
    }

    private void DoOffTween() {
        DoKill();
        onFront.rectTransform.localPosition = _onSrcPosition;
        onFront.rectTransform.DOLocalMoveX(_offSrcPosition.x, duration).SetEase(Ease.OutSine);
        onFront.DOFade(0, duration*0.7f);

        offBg.DOFade(1, duration);
        offFront.rectTransform.localPosition = _offSrcPosition;
        offFront.DOFade(1, duration * 0.8f).SetEase(Ease.OutSine);
    }

    private void DoKill() {
        onFront.rectTransform.DOKill(true);
        onFront.DOKill(true);
        offFront.rectTransform.DOKill(true);
        offFront.DOKill(true);
        offBg.DOKill(true);
    }

    public bool IsOn {
        get { return _isOn; }
        set {
            if (_isOn != value) {
                _isOn = value;
                if (_changeFunc != null) {
                    _changeFunc(_isOn);
                }
                DoTween();
            }
        }
    }

    public void SetSelect(bool isOn) {
        if (isOn == _isOn) return;
        _isOn = isOn;
        var last = duration;
        duration = 0;
        DoTween();
        duration = last;
    }

    public void AddChange(UIEventListener.BoolDelegate func) {
        _changeFunc = func;
    }


}
