using System;
using System.Reflection;
using UnityEditor;

/// <summary>
/// 通过反射打开Unity自身的Sprite
/// </summary>
public class SpriteEditor
{
    private static EditorWindow Window
    {
        get
        {
            Init();
            return (EditorWindow)_instanceField.GetValue(null);
        }
    }

    private static MethodInfo _openHandler;
    //private static MethodInfo _selectHandler;
    private static FieldInfo _instanceField; //

    private static bool _hasInit;

    private static void Init()
    {
        if (!_hasInit)
        {
            Type type = Type.GetType("UnityEditor.SpriteUtilityWindow,UnityEditor");
            if (type == null) return;
            _openHandler = type.GetMethod("ShowSpriteEditorWindow", BindingFlags.NonPublic | BindingFlags.Static);
            //_selectHandler = type.GetMethod("SelectSpriteIndex", BindingFlags.NonPublic | BindingFlags.Instance);
            _instanceField = type.GetField("s_Instance", BindingFlags.Public | BindingFlags.Static);
            _hasInit = true;
        }
    }

    public static void Open(UnityEngine.Object obj)
    {
        Selection.activeObject = obj;
        Init();
        _openHandler.Invoke(null, new object[] { null});
    }
}