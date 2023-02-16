using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.EventSystems;

public class CFakeInputModule : StandaloneInputModule {

    public static CFakeInputModule instance;

    protected Touch fakeTouch;
    private bool isInitFadeTouch = false;

    protected void InitFakeTouch() {
        if (isInitFadeTouch == false) {
            isInitFadeTouch = true;
            fakeTouch = new Touch();
            fakeTouch.deltaPosition = Vector2.zero;
            fakeTouch.phase = TouchPhase.Ended;
            fakeTouch.type = TouchType.Direct;
            fakeTouch.fingerId = 99;
        }
    }

    public void FakeTouch(Vector2 srcPos) {
        InitFakeTouch();
        Touch touch = fakeTouch;
        touch.position = srcPos;
        touch.rawPosition = srcPos;
        bool released;
        bool pressed;
        var pointer = GetTouchPointerEventData(touch, out pressed, out released);
        ProcessTouchPress(pointer, pressed, released);
        if (!released) {
            ProcessMove(pointer);
            ProcessDrag(pointer);
        } else
            RemovePointerData(pointer);
    }
}
