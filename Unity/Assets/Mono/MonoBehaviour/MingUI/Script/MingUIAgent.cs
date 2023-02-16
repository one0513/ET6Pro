using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// 资源子项(支持Texture2D/GameObject)
/// </summary>
public class ItemVo {
    public string abName;//资源名
    public bool isComplete;
    public string error_msg;
    public Func<Object> getObject;//资源:Texture2D/GameObject
    public Func<Object> getInstance;  //获取实例对象
    public Action onRelease;//资源回收接口
}
/// <summary>
/// MingUI抛给外面的代理列
/// </summary>
public class MingUIAgent {
    public static bool IsEditorMode { get; set; }

    public delegate bool CheckOpenFun(string key, CTabNavigation tabNav);
    public delegate Shader ShaderFun(string name);

    private static Action<Object> _editorSetDirty;
    private static Action<string, Action<ItemVo>> _loadTextureFun;
    private static Action<string, Action<ItemVo>> _loadPrefabFun;
    private static Action<object, bool> _infoFun;
    private static CheckOpenFun _checkOpenFun;
    private static Action<string> _playSoundFun;
    private static Dictionary<MonoBehaviour, Action<bool>> _onEnableList = new Dictionary<MonoBehaviour, Action<bool>>();
    private static Action<MonoBehaviour, bool> _onEnableFun;
    private static ShaderFun _shaderFindFun;

    public static void SetEditorSetDirty(Action<Object> function) {
        _editorSetDirty = function;
    }

    public static void PlaySound(string key) {
        if (_playSoundFun != null) {
            _playSoundFun(key);
        }
    }

    public static void SetPlaySoundFun(Action<string> function) {
        _playSoundFun = function;
    }

    public static void EditorSetDirty(Object target) {
        _editorSetDirty(target);
    }

    public static void SetLogInfo(Action<object, bool> function) {
        _infoFun = function;
    }

    public static void SetShaderFind(ShaderFun function) {
        _shaderFindFun = function;
    }

    public static Shader ShaderFind(string name) {
        if (_shaderFindFun == null) {
            return Shader.Find(name);
        } else {
            return _shaderFindFun.Invoke(name);
        }
    }

    /// <summary>
    /// 设置功能开启（这种上层业务逻辑本不应该放在这里，但由于项目后期不想在上层逐个处理，只好在底层做统一处理了）
    /// </summary>
    /// <param name="fun"></param>
    public static void SetCheckOpenFun(CheckOpenFun fun) {
        _checkOpenFun = fun;
    }

    /// <summary>
    /// 功能开启检测（这种上层业务逻辑本不应该放在这里，但由于项目后期不想在上层逐个处理，只好在底层做统一处理了）
    /// </summary>
    /// <param name="key"></param>
    /// <param name="tabNav"></param>
    /// <returns></returns>
    public static bool CheckOpen(string key, CTabNavigation tabNav) {
        if (_checkOpenFun != null) {
            return _checkOpenFun(key, tabNav);
        }
        return true;
    }

    /// <summary>
    /// 注册加载回调
    /// </summary>
    /// <param name="loadTextureFun">注册加载贴图</param>
    /// <param name="loadPrefabFun">注册加载预设</param>
    public static void InitLoadProxy(Action<string, Action<ItemVo>> loadTextureFun, Action<string, Action<ItemVo>> loadPrefabFun) {
        _loadTextureFun = loadTextureFun;
        _loadPrefabFun = loadPrefabFun;
    }
    /// <summary>
    /// load 独立贴图
    /// </summary>
    /// <param name="path"></param>
    /// <param name="callBack"></param>
    public static void LoadUITexture(string path, Action<ItemVo> callBack) {
        if (_loadTextureFun != null) {
            _loadTextureFun(path, callBack);
        }
    }
    /// <summary>
    /// load 预设
    /// </summary>
    /// <param name="path"></param>
    /// <param name="callBack"></param>
    public static void LoadUIPrefab(string path, Action<ItemVo> callBack) {
        if (_loadPrefabFun != null) {
            _loadPrefabFun(path, callBack);
        }
    }
    /// <summary>
    /// 打印日志
    /// </summary>
    /// <param name="message"></param>
    /// <param name="writeStack"></param>
    public static void Info(object message, bool writeStack = false) {
        if (_infoFun != null) {
            _infoFun(message, writeStack);
        }
    }

    public static void SetOnEnableFun(Action<MonoBehaviour, bool> func) {
        _onEnableFun = func;
    }

    public static void AddOnEnable(MonoBehaviour mono, Action<bool> fun) {
        _onEnableList[mono] = fun;
    }

    public static void RemoveOnEnable(MonoBehaviour mono, Action<bool> fun) {
        if (_onEnableList.ContainsKey(mono)) {
            _onEnableList.Remove(mono);
        }
    }

    public static void OnEnable(MonoBehaviour mono, bool b) {
        if (_onEnableList.ContainsKey(mono)) {
            _onEnableList[mono].Invoke(b);
        }
        if (_onEnableFun != null) {
            _onEnableFun.Invoke(mono, b);
        }
    }

    public static void SetUIDefaultMaterial(bool custom) {
        if (custom) {
            GraphicGrey.UseCustomDefaultMat();
        } else {
            GraphicGrey.UseDefaultMat();
        }
    }

    public static void Dispose() {
        _loadTextureFun = null;
        _loadPrefabFun = null;
        _infoFun = null;
        _checkOpenFun = null;
        _playSoundFun = null;
        _onEnableList.Clear();
        _onEnableFun = null;
    }
}

