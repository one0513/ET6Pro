using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CNumStepper : UIBehaviour {
    public CButton btnDown;
    public CButton btnDown10;
    public CButton btnUp;
    public CButton btnUp10;
    public CTextInput inputNum;
    public bool alwaysCallBack = false;
    public int _max = 99;
    public int Min = 1;
    public int step = 1;

    private UIEventListener.IntDelegate _callBack;
    private int _value = -1;
    private int _stepFactor = 0;
    private float _btnDownTime = 0;
    private bool _isEnabled = true;

    protected override void Start() {
        inputNum.inputType = TMPro.TMP_InputField.InputType.Standard;
        inputNum.contentType = TMPro.TMP_InputField.ContentType.IntegerNumber;
        inputNum.AddEndEdit(OnInputSumit);
        inputNum.AddChange(OnInputChange);

        if (btnDown) {
            btnDown.AddMouseDown(value => { _stepFactor = -1; _btnDownTime = Time.time; });
            btnDown.AddMouseUp(value => { _stepFactor = 0; });
            btnDown.AddClick(OnDownClick);
        }
        if (btnUp) {
            btnUp.AddClick(OnUpClick);
            btnUp.AddMouseDown(value => { _stepFactor = 1; _btnDownTime = Time.time; });
            btnUp.AddMouseUp(value => { _stepFactor = 0; });
        }

        if (btnUp10) {
            btnUp10.AddClick(p => ChangeValue(Value + 10));
        }

        if (btnDown10) {
            btnDown10.AddClick(p => ChangeValue(Value - 10));
        }
        if (_value == -1) {
            Value = Min;
        }
    }

    private void OnInputChange(string value) {
        var num = 0;
        int.TryParse(value,out num);
        if (num > Max) {
            num = Max;
            inputNum.text = num.ToString();
        }
    }

    private void OnInputSumit(string value) {
        ChangeValue(int.Parse(inputNum.text));
    }

    private void OnUpClick(PointerEventData value) {
        ChangeValue(Value + 1);
    }

    private void OnDownClick(PointerEventData value) {
        ChangeValue(Value - 1);
    }

    private void ChangeValue(int newValue) {
        //Debug.Log(string.Format("max={0},min={1},newValue={2}",Max,Min,newValue));
        newValue = Mathf.Clamp(newValue, Min, Max);
        if (newValue != Value) {
            inputNum.text = newValue.ToString();
            _value = newValue;
            DoCallBack(_value);
        } else {
            if (alwaysCallBack) {
                DoCallBack(_value);
            }
        }
    }

    public int Max {
        get { return _max; } 
        set {
            _max = value;
            IsEnabled = value > 1;
        }
    }

    public bool IsEnabled {
        get { return _isEnabled; }
        set {
            if (value != _isEnabled) {
                _isEnabled = value;
                btnDown.IsEnabled = value;
                btnUp.IsEnabled = value;
                btnDown10.IsEnabled = value;
                btnUp10.IsEnabled = value;
            }
        }
    }

    public int Value {
        set { ChangeValue(value); }
        get { return _value; }
    }

    public void AddChange(UIEventListener.IntDelegate callback) {
        _callBack = callback;
    }

    private void DoCallBack(int num = 0) {
        if (_callBack != null) _callBack(num);
    }

    void Update() {
        if (_stepFactor != 0 && Time.time - _btnDownTime > 0.3) {
            ChangeValue(Value + step * _stepFactor);
        }
    }
}
