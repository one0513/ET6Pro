using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 艺术字库
/// </summary>
public class CFont : MonoBehaviour
{
    public CAtlas atlas;
    //public List<BMSymbol> symbolList = new List<BMSymbol>();
    public List<string> keyList = new List<string>();
    public List<string> valueList = new List<string>();
    #region 新的方式（不知道为什么使用自定义的序列化类在手机上不能反序列化）
    public void AddSymbol(string key, string spriteName)
    {
        int index = GetSymbol(key);
        if (index == -1)
        {
            keyList.Add(key);
            valueList.Add(spriteName);
            MarkAsChanged();
        }
    }

    public void RemoveSymbol(string key)
    {
        int index = GetSymbol(key);
        if (index != -1)
        {
            keyList.RemoveAt(index);
            valueList.RemoveAt(index);
            MarkAsChanged();
        }
    }

    public void RenameSymbol(string before, string after)
    {
        int index = GetSymbol(before);
        if (index != -1)
        {
            keyList[index] = after;
            MarkAsChanged();
        }
    }

    public void ChangeSymbol(string key, string spriteName)
    {
        int index = GetSymbol(key);
        if (index != -1)
        {
            valueList[index] = spriteName;
            MarkAsChanged();
        }
    }

    public string MatchSymbol(string text, int offset, int textLength)
    {
        int count = keyList.Count;
        if (count == 0) return null;
        textLength -= offset;

        for (int i = 0; i < count; ++i)
        {
            string key = keyList[i];
            int symbolLength = key.Length;
            if (symbolLength == 0 || textLength < symbolLength) continue;

            bool match = true;

            for (int c = 0; c < symbolLength; ++c)
            {
                if (text[offset + c] != key[c])
                {
                    match = false;
                    break;
                }
            }
            if (match) return valueList[i];
        }
        return null;
    }

    private int GetSymbol(string key)
    {
        for (int i = 0, imax = keyList.Count; i < imax; ++i)
        {
            if (keyList[i] == key) return i;
        }
        return -1;
    }
    #endregion
//    #region 老的方式（不知道为什么使用自定义的序列化类在手机上不能反序列化）
//    public void AddSymbol(string key, string spriteName)
//    {
//        BMSymbol bmSymbol = GetSymbol(key, true);
//        bmSymbol.spriteName = spriteName;
//        MarkAsChanged();
//    }

//    public void RemoveSymbol(string key)
//    {
//        BMSymbol bmSymbol = GetSymbol(key, false);
//        if (bmSymbol != null) symbolList.Remove(bmSymbol);
//        MarkAsChanged();
//    }

//    public void RenameSymbol(string before, string after)
//    {
//        BMSymbol bmSymbol = GetSymbol(before, false);
//        if (bmSymbol != null) bmSymbol.key = after;
//        MarkAsChanged();
//    }

//    public void ChangeSymbol(string key, string spriteName)
//    {
//        BMSymbol bmSymbol = GetSymbol(key, false);
//        if (bmSymbol != null)
//        {
//            bmSymbol.spriteName = spriteName;
//            MarkAsChanged();
//        }
//    }

//    public BMSymbol MatchSymbol(string text, int offset, int textLength)
//    {
//        int count = symbolList.Count;
//        if (count == 0) return null;
//        textLength -= offset;

//        for (int i = 0; i < count; ++i)
//        {
//            BMSymbol sym = symbolList[i];
//            int symbolLength = sym.Length;
//            if (symbolLength == 0 || textLength < symbolLength) continue;

//            bool match = true;

//            for (int c = 0; c < symbolLength; ++c)
//            {
//                if (text[offset + c] != sym.key[c])
//                {
//                    match = false;
//                    break;
//                }
//            }
//            if (match) return sym;
//        }
//        return null;
//    }

//    private BMSymbol GetSymbol(string key, bool createIfMissing)
//    {
//        for (int i = 0, imax = symbolList.Count; i < imax; ++i)
//        {
//            BMSymbol sym = symbolList[i];
//            if (sym.key == key) return sym;
//        }
//        if (createIfMissing)
//        {
//            BMSymbol sym = new BMSymbol();
//            sym.key = key;
//            symbolList.Add(sym);
//            return sym;
//        }
//        return null;
//    }
//#endregion
    public void MarkAsChanged()
    {
        if (MingUIAgent.IsEditorMode)
        {
            MingUIAgent.EditorSetDirty(gameObject);
        }
    }
    public void OnDestroy()
    {
        atlas = null;
        //symbolList.Clear();
        //symbolList = null;
        keyList.Clear();
        valueList.Clear();
        keyList = null;
        valueList = null;
    }
}