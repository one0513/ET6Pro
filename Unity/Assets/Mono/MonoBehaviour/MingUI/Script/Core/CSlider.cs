using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 滑动条
/// </summary>
[AddComponentMenu("MingUI/CSlider", 29)]
public class CSlider : Slider {

    public class CSliderGroupVO {
        public int dataKey;
        public float dataValue;
        public float dataScale = 1;//默认是1

        public void ResetData() {
            dataValue = 0;
            dataScale = 1;
        }
    }

    //设置最大值组（所有拖动条的值之和）
    //ID设置需要避免重复，目前是使用linkID限定
    private static Dictionary<int, float> _groupMaxValueDic;
    private static Dictionary<int, float> _groupCurValueDic;
    private static Dictionary<int, Dictionary<int, CSliderGroupVO>> _groupVoDic = new Dictionary<int, Dictionary<int, CSliderGroupVO>>();

    private int maxGroupId = 0;
    private int groupKey = -1;
    private float groupScale = 1;//最大组的值的倍率

    private UIEventListener.FloatDelegate _onChange;
    public CButton btnSub;
    public CButton btnAdd;
    public CTextInput inputNum;

    public static void SetGroupMaxValue(int groupId, float maxGroupValue) {
        if (_groupMaxValueDic == null) _groupMaxValueDic = new Dictionary<int, float>();
        if (_groupCurValueDic == null) _groupCurValueDic = new Dictionary<int, float>();
        if (_groupVoDic == null) _groupVoDic = new Dictionary<int, Dictionary<int, CSliderGroupVO>>();
        if (_groupVoDic.ContainsKey(groupId) == false) _groupVoDic[groupId] = new Dictionary<int, CSliderGroupVO>();
        _groupMaxValueDic[groupId] = maxGroupValue;
        _groupCurValueDic[groupId] = maxGroupValue;
    }

    public static float GetCurGroupValue(int groupId) {
        if (_groupMaxValueDic != null && _groupCurValueDic != null && _groupMaxValueDic.ContainsKey(groupId) && _groupCurValueDic.ContainsKey(groupId)) {
            return _groupMaxValueDic[groupId] - _groupCurValueDic[groupId];
        }
        return 0;
    }

    //不知道为啥，Start执行总是比Lua里面setData慢，所以要这样执行
    public static void ForceUpdateGroupValue(int groupId) {
        _groupCurValueDic[groupId] = _groupMaxValueDic[groupId];
        var voEnum = _groupVoDic[groupId].GetEnumerator();
        while (voEnum.MoveNext()) {
            _groupCurValueDic[groupId] -= voEnum.Current.Value.dataValue * voEnum.Current.Value.dataScale;
        }
    }

    public static void ForceClearGroupData(int groupId) {
        if (_groupVoDic != null && _groupVoDic.ContainsKey(groupId)) {
            var voEnum = _groupVoDic[groupId].GetEnumerator();
            while (voEnum.MoveNext()) {
                voEnum.Current.Value.ResetData();
            }
        }
    }

    public static void SetCustomGroupData(int id, int key, float val,float scale = 1) {
        if (_groupVoDic == null) _groupVoDic = new Dictionary<int, Dictionary<int, CSliderGroupVO>>();
        if (_groupVoDic.ContainsKey(id) == false) _groupVoDic[id] = new Dictionary<int, CSliderGroupVO>();
        if (_groupVoDic[id].ContainsKey(key) == false) {
            _groupVoDic[id][key] = new CSliderGroupVO();
        }
        _groupVoDic[id][key].dataKey = key;
        _groupVoDic[id][key].dataValue = val;
        _groupVoDic[id][key].dataScale = scale;
    }

    public void SetGroupId(int id, int key, float val, float maxVal, float scale = 1) {
        if (id > 0) {
            maxGroupId = id;
            groupKey = key;
            groupScale = scale;
            if (_groupMaxValueDic == null || _groupMaxValueDic.ContainsKey(id) == false) {
                SetGroupMaxValue(id, maxValue);//如果事先没设置最大值，则使用首次设置者的最大值
            }
            if (_groupVoDic == null) _groupVoDic = new Dictionary<int, Dictionary<int, CSliderGroupVO>>();
            if (_groupVoDic.ContainsKey(id) == false) _groupVoDic[id] = new Dictionary<int, CSliderGroupVO>();
            if (_groupVoDic[id].ContainsKey(groupKey) == false) {
                _groupVoDic[id][groupKey] = new CSliderGroupVO();
            }
            _groupVoDic[id][groupKey].dataKey = groupKey;
            _groupVoDic[id][groupKey].dataScale = groupScale;
            _groupVoDic[id][groupKey].dataValue = val;
            maxValue = maxVal;
            base.Set(val, true);
        } else {
            //重置最大组
            RemoveGroup();
        }
    }

    private void UpdateGroupValue() {
        if (_groupVoDic != null && _groupVoDic.ContainsKey(maxGroupId) && _groupVoDic[maxGroupId].ContainsKey(groupKey)) {
            _groupVoDic[maxGroupId][groupKey].dataValue = this.value;
        }
    }

    private void RemoveGroup() {
        if (maxGroupId > 0 && _groupVoDic != null && _groupVoDic.ContainsKey(maxGroupId)) {
            if (_groupVoDic[maxGroupId].ContainsKey(groupKey)) {
                _groupVoDic[maxGroupId][groupKey] = null;
                _groupVoDic[maxGroupId].Remove(groupKey);
            }
            if (_groupVoDic[maxGroupId].Count == 0) {
                _groupVoDic.Remove(maxGroupId);
            }
            if (_groupVoDic.Count == 0) {
                _groupVoDic = null;
            }
        }
        maxGroupId = 0;
    }

    public void AddChange(UIEventListener.FloatDelegate fun) {
        _onChange = fun;
    }

    public void RemoveChange(UIEventListener.FloatDelegate fun) {
        _onChange = null;
    }

    public void SetInteractable(bool able) {
        if (interactable != able) {
            interactable = able;
            btnAdd.interactable = able;
            btnSub.interactable = able;
            inputNum.interactable = able;
        }
    }

    protected override void Start() {
        base.Start();
        onValueChanged.AddListener(OnChange);
        if (btnSub != null) {
            btnSub.AddClick(OnSub);
            btnSub.AddLongClick(() => OnSub(null));
        }
        if (btnAdd != null) {
            btnAdd.AddClick(OnAdd);
            btnAdd.AddLongClick(() => OnAdd(null));
        }
        if (inputNum != null) {
            inputNum.AddChange(OnChangeInput);
            inputNum.AddEndEdit(CheckInputValid);
            inputNum.keyboardType = TouchScreenKeyboardType.NumberPad;
        }
        //默认不让点击滑动条
        targetGraphic.raycastTarget = false;
        foreach (Transform child in targetGraphic.rectTransform)
        {
            var obj = child.gameObject.GetComponent<CSprite>();
            obj.raycastTarget = false;
        }
    }

    protected override void Set(float input, bool sendCallback) {
        //重写输入值，防止超过最大值组
        if (maxGroupId > 0 && input > MaxValue) {
            input = MaxValue;
        }
        base.Set(input, sendCallback);
    }

    protected float MaxValue {
        get {
            if (maxGroupId > 0) {
                return Mathf.Min(Mathf.FloorToInt((_groupCurValueDic[maxGroupId] + value * groupScale) / groupScale), maxValue);
            } else {
                return maxValue;
            }
        }
    }

    private void CheckInputValid(string input) {
        int num;
        if (!int.TryParse(input, out num)) {
            value = MaxValue;
            ChangeInputTxtOnly((int)MaxValue + "");
        }
    }

    private void OnChangeInput(string input) {
        int num = -1;
        if (int.TryParse(input, out num)) {
            if (num <= MaxValue && num >= minValue) {
                value = num;
                return;
            }
        }
        if (num > -1) {
            if (num > MaxValue) {
                value = MaxValue;
                ChangeInputTxtOnly((int)value + "");
            } else if (num < minValue) {
                value = minValue;
                ChangeInputTxtOnly("1");
            }
        }
    }

    private void OnSub(PointerEventData p) {
        if (interactable && value > minValue) {
            value--;
        }
    }

    private void OnAdd(PointerEventData p) {
        if (interactable && value < MaxValue) {
            value++;
        }
    }

    protected override void OnDestroy() {
        RemoveGroup();
        base.OnDestroy();
        _onChange = null;
    }

    private void OnChange(float val) {
        if (maxGroupId > 0) {
            UpdateGroupValue();
            ForceUpdateGroupValue(maxGroupId);
        }
        if (_onChange != null) {
            _onChange(val);
        }
        ChangeInputTxtOnly((int)val + "");
    }

    public void ChangeInputTxtOnly(string str) {
        if (!string.IsNullOrEmpty(str)) {
            inputNum.text = str;
        }
    }
}