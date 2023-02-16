using UnityEngine.EventSystems;

public class CTextInput : TMPro.TMP_InputField
{
    private UIEventListener.StringDelegate _onChange;
    private UIEventListener.StringDelegate _onSubmit;
    private UIEventListener.StringDelegate _onEndEdit;
    private UIEventListener.ObjectDelegate _onFocus;
    private UIEventListener.BoolDelegate _onSelect;
    /// <summary>
    /// 清除
    /// </summary>
    public void Clear() {
        text = string.Empty;
    }

    /// <summary>
    /// 添加改变回调
    /// </summary>
    /// <param name="callback"></param>
    public void AddChange(UIEventListener.StringDelegate callback) {
        _onChange = callback;
    }

    /// <summary>
    /// 删除改变回调
    /// </summary>
    public void RemoveChange() {
        _onChange = null;
    }

    /// <summary>
    /// 添加焦点回调
    /// </summary>
    /// <param name="callback"></param>
    public void AddFocus(UIEventListener.ObjectDelegate callback) {
        _onFocus = callback;
    }

    /// <summary>
    /// 删除焦点回调
    /// </summary>
    public void RemoveFocus() {
        _onFocus = null;
    }

    /// <summary>
    /// 添加提交回调
    /// </summary>
    /// <param name="callback"></param>
    public void AddSubmit(UIEventListener.StringDelegate callback) {
        _onSubmit = callback;
    }

    /// <summary>
    /// 删除提交回调
    /// </summary>
    public void RemoveSubmit() {
        _onSubmit = null;
    }

    /// <summary>
    /// 添加结束编辑回调
    /// </summary>
    /// <param name="callback"></param>
    public void AddEndEdit(UIEventListener.StringDelegate callback) {
        _onEndEdit = callback;
    }

    /// <summary>
    /// 删除结束编辑回调
    /// </summary>
    public void RemoveEndEdit() {
        _onEndEdit = null;
    }

    public void AddSelect(UIEventListener.BoolDelegate callBack) {
        _onSelect = callBack;
    }

    public void RemoveSelect() {
        _onSelect = null;
    }

    public override void OnSubmit(BaseEventData eventData) {
        base.OnSubmit(eventData);
        if (_onSubmit != null) _onSubmit("提交了");
    }

    private void OnChange(string str) {
        if (_onChange != null) _onChange(str);
    }

    private void OnEndEdit(string str) {
        if (_onEndEdit != null) {
            _onEndEdit(str);
        }
    }

    protected override void Awake() {
        base.Awake();
        onValueChanged.AddListener(OnChange);
        onEndEdit.AddListener(OnEndEdit);
    }

    protected override void OnDestroy() {
        base.OnDestroy();
        _onChange = null;
        _onSubmit = null;
        _onEndEdit = null;
        _onFocus = null;
    }

    public override void OnPointerClick(PointerEventData data) {
        base.OnPointerClick(data);
        if (_onFocus != null) {
            _onFocus(null);
        }
    }

    public override void OnSelect(BaseEventData eventData) {
        base.OnSelect(eventData);
        if(_onSelect != null) {
            _onSelect(true);
        }
    }

    public override void OnDeselect(BaseEventData eventData) {
        base.OnDeselect(eventData);
        if (_onSelect != null) {
            _onSelect(false);
        }
    }
}