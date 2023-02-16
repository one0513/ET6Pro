using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MingUIContextMenu
{
    private static GenericMenu _mMenu;
    private static readonly List<string> _mEntries = new List<string>();

    public static void AddItem(string item, bool isChecked, GenericMenu.MenuFunction2 callback, object param)
    {
        if (callback != null)
        {
            if (_mMenu == null) _mMenu = new GenericMenu();
            int count = 0;

            for (int i = 0; i < _mEntries.Count; ++i)
            {
                string str = _mEntries[i];
                if (str == item) ++count;
            }
            _mEntries.Add(item);

            if (count > 0) item += " [" + count + "]";
            _mMenu.AddItem(new GUIContent(item), isChecked, callback, param);
        }
        else AddDisabledItem(item);
    }

    public static void AddDisabledItem(string item)
    {
        if (_mMenu == null) _mMenu = new GenericMenu();
        _mMenu.AddDisabledItem(new GUIContent(item));
    }

    public static void Show()
    {
        if (_mMenu != null)
        {
            _mMenu.ShowAsContext();
            _mMenu = null;
            _mEntries.Clear();
        }
    }
}