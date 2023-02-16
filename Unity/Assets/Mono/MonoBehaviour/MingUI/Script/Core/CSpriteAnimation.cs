using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 序列帧播放器
/// </summary>
[RequireComponent(typeof (CSprite))]
public class CSpriteAnimation : MonoBehaviour
{
    //////////////////////////////////////////////////////////    SpriteAnimation管理逻辑     /////////////////////////////////////////////////////////////////////////////////////////
    //管理序列帧在每秒内开始播放逻辑的帧序列，降低大量序列帧组件在同一帧执行sprite替换的概率

    private static List<CSpriteAnimation> managerList = new List<CSpriteAnimation>();
    private static Dictionary<CSpriteAnimation, int> managerDic = new Dictionary<CSpriteAnimation, int>();

    private static void RegisterManager(CSpriteAnimation spAnim) {
        if (managerDic.ContainsKey(spAnim) == false) {
            managerList.Add(spAnim);
            managerDic.Add(spAnim, managerList.Count - 1);
            spAnim.IntExcuteParam();
        }
    }

    private static void UnRegisterManager(CSpriteAnimation spAnim) {
        int index = -1;
        if (managerDic.TryGetValue(spAnim, out index)) {
            CSpriteAnimation item = managerList[index];
            managerDic.Remove(item);
            if (index == managerList.Count - 1) {
                managerList.RemoveAt(index);
            } else {
                int replaceItemIndex = managerList.Count - 1;
                CSpriteAnimation replaceItem = managerList[replaceItemIndex];
                managerList[index] = replaceItem;
                managerDic[replaceItem] = index;
                managerList.RemoveAt(replaceItemIndex);
            }
            item.ResetExcuteParam();
        }
    }

    public static void HandleManager() {
        for (int i = 0; i < managerList.Count; i++) {
            managerList[i].ExcuteAnimation();
        }
    }

    /////////////////////// 简易分配动画开始循环的帧

    private static int curMaxAssing = 0;
    private static Stack<int> assginStack = new Stack<int>();

    private static int getCurAssginIndex() {
        if (assginStack.Count > 0) {
            return assginStack.Pop();
        } else {
            curMaxAssing = curMaxAssing + 1;
            if (curMaxAssing >= 60) {
                curMaxAssing = 0;
            }
            return curMaxAssing;
        }
    }

    ////////////////////// 批量同时出现的时候，会按照出现的顺序安排动画开始的帧

    private int assignFrameIndex = 1;
    private bool isAssignPlaying = false;
    private bool canBeExcute = false;

    public void ForceExcuteImmediate(bool playing) {
        if (isAssignPlaying == true) {
            canBeExcute = playing;
        }
    }

    private void IntExcuteParam() {
        isAssignPlaying = true;
        canBeExcute = false;
        assignFrameIndex = getCurAssginIndex();
    }

    private void ResetExcuteParam() {
        assginStack.Push(assignFrameIndex);
        canBeExcute = false;
        isAssignPlaying = false;
        assignFrameIndex = 0;
    }

    private void ExcuteAnimation() {
        if (canBeExcute) {
            OnHandleAnimation();
        } else {
            if ((Time.frameCount % 60) == assignFrameIndex) {
                canBeExcute = true;
            }
        }
    }

    //////////////////////////////////////////////////////////    SpriteAnimation实例逻辑     /////////////////////////////////////////////////////////////////////////////////////////

    private static List<string> spriteNameEmptyList = new List<string>();

    [HideInInspector] [SerializeField] private int _mFps = 30;
    [HideInInspector] [SerializeField] private string _mPrefix = "";
    [HideInInspector] [SerializeField] private bool _mLoop = true;

    private CSprite _sprite;
    private float _delta;
    private int _index;
    private bool _active = true;
    private bool _isPlaying = true;
    private bool _isFlipX = false;
    private List<string> _spriteNames = spriteNameEmptyList;

    private bool _duringMachineState = false;
    private bool _irregularSprite = false;
    private bool _stopWhenEnd = false;
    private bool _sortReverse = false;
    private string _recoverPrefix = "";
    private int _recoverFps = 30;
    private int _randomBeginIndex = 0;

    public int FramesPerSecond
    {
        get { return _mFps; }
        set { _mFps = value; }
    }

    public string NamePrefix
    {
        get { return _mPrefix; }
        set
        {
            if (_mPrefix != value)
            {
                _mPrefix = value;
                RebuildSpriteList();
            }
        }
    }

    public bool IsIrregularSprite {
        get {
            return _irregularSprite;
        }
        set {
            _irregularSprite = value;
        }
    }

    public bool Loop
    {
        get { return _mLoop; }
        set { _mLoop = value; }
    }

    public bool IsPlaying
    {
        get { return _isPlaying; }
        set {
            if (_isPlaying != value) {
                _isPlaying = value;
                if (_isPlaying) {
                    RegisterManager(this);
                } else {
                    UnRegisterManager(this);
                }
            }
        }
    }

    public bool IsFlipX {
        get { return _isFlipX; }
        set {
            _isFlipX = value;
        }
    }

    public int GetPrefixNameCount(string name) {
        int count = 0;
        CheckSprite();
        if (_spriteNames != null && _spriteNames.Count > 0) {
            count = _spriteNames.Count;
        }
        return count;
    }

    public void SetStateMachine(string playName, string recoverName, int fps, int recoverFps, bool stopWhenEnd, bool isReverse = false) {
        _duringMachineState = true;
        _stopWhenEnd = stopWhenEnd;
        _recoverPrefix = recoverName;
        _sortReverse = isReverse;

        _delta = 0.0f;
        _mFps = fps;
        _recoverFps = recoverFps;

        _mPrefix = playName;
        _active = true;
        Loop = true;
        RebuildSpriteList();
    }

    public void JumpToLast(string playName, bool isReverse = false) {
        _sortReverse = isReverse;
        _mPrefix = playName;
        _active = false;
        Loop = false;
        RebuildSpriteList();
    }

    public void SetRandomBegin() {
        if (_spriteNames != null && _spriteNames.Count > 1) {
            _randomBeginIndex = Random.Range(0, _spriteNames.Count - 1);
        }
    }

    private void Start()
    {
        RebuildSpriteList();
    }

    private void OnHandleAnimation()
    {
        if (_isPlaying && _spriteNames != null && _spriteNames.Count > 1 && _mFps > 0f)
        {
            _delta += Time.deltaTime;
            float rate = 1f / _mFps;

            if (rate < _delta)
            {
                _delta = rate > 0f ? _delta - rate : 0f;
                if (_index >= _spriteNames.Count)
                {
                    DoStateMachine();
                    _active = Loop;
                }

                if (_active)
                {
                    ChangeSprite(_index);
                }

                _index++;
            }
        }
    }

    private void RebuildSpriteList()
    {
        CheckSprite();;
        if (_sprite != null && _sprite.Atlas != null)
        {
            _spriteNames = _sprite.Atlas.GetSpriteNameList(_mPrefix, _sortReverse);

            if (_spriteNames.Count >= 1)
            {
                ChangeSprite(0);
            }
        } else {
            _spriteNames = spriteNameEmptyList;
        }

        _index = 0;
    }

    private void DoStateMachine() {
        //简单状态机逻辑
        if (_duringMachineState) {
            _duringMachineState = false;
            if (_stopWhenEnd) {
                Loop = false;
            } else {
                _delta = 0.0f;
                _mFps = _recoverFps;
                NamePrefix = _recoverPrefix;
            }
            if (_randomBeginIndex != 0) {
                _index = _randomBeginIndex;
                _randomBeginIndex = 0;
            } else {
                _index = 0;
            }
        } else {
            _index = 0;
        }
    }

    private void ChangeSprite(int index) {
        _sprite.SpriteName = _spriteNames[index];
        if (_irregularSprite && _sprite.sprite != null) {
            if (_isFlipX) {
                _sprite.rectTransform.anchoredPosition = new Vector2(-_sprite.sprite.border.x, _sprite.sprite.border.y);
            } else {
                _sprite.rectTransform.anchoredPosition = new Vector2(_sprite.sprite.border.x, _sprite.sprite.border.y);
            }
        }
    }

    private void CheckSprite() {
        if (_sprite == null) {
            _sprite = GetComponent<CSprite>();
            _sprite.AddAtlasLoadCallBack(RebuildSpriteList);
        }
    }

    private void OnDestroy() {
        if (_sprite != null) {
            _sprite.RemoveAtlasCallBack(RebuildSpriteList);
            _sprite = null;
        }
        UnRegisterManager(this);
    }

    private void OnEnable() {
        if (_isPlaying) {
            RegisterManager(this);
        }
    }

    private void OnDisable() {
        UnRegisterManager(this);
    }
}