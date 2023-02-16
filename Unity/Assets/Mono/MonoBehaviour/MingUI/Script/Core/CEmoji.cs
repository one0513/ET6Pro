using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Emoji
{
    public string name; //命名
    public string prefix; //表情前缀
    public int fps; //帧频
}

/// <summary>
/// 表情索引表
/// </summary>
public class CEmoji : MonoBehaviour
{
    public static char[] SPLIT = new[] { '#' };
    public CAtlas atlas; //对应的图集
    public List<string> keyList = new List<string>();
    public List<string> valueList = new List<string>();

    private Dictionary<string, Emoji> _emojiDic = new Dictionary<string, Emoji>();
    #region 新的方式（不知道为什么使用自定义的序列化类在手机上不能反序列化）
    public void AddEmoji(string newName, string newPrefix, int newFps)
    {
        int index = GetEmojiIndex(newName);
        if (index == -1)
        {
            keyList.Add(newName);
            valueList.Add(newPrefix+"#"+ newFps);
            MarkAsChanged();
        }
    }

    public void RemoveEmoji(string key)
    {
        int index = GetEmojiIndex(key);
        if (index != -1)
        {
            keyList.RemoveAt(index);
            valueList.RemoveAt(index);
            MarkAsChanged();
        }
    }

    public void ChangeEmoji(string key, string newPrefix, int newFps)
    {
        int index = GetEmojiIndex(key);
        if (index != -1)
        {
            valueList[index] = newPrefix + "#" + newFps;
            MarkAsChanged();
        }
    }

    public Emoji GetEmoji(string newName)
    {
        if (_emojiDic.ContainsKey(newName))
        {
            return _emojiDic[newName];
        }
        for (int i = 0, imax = keyList.Count; i < imax; ++i)
        {
            if (keyList[i] == newName)
            {
                Emoji emo = new Emoji();
                emo.name = newName;
                string value = valueList[i];

                string[] arr = value.Split(SPLIT, 2);
                emo.prefix = arr[0];
                emo.fps = Convert.ToInt32(arr[1]);
                _emojiDic[newName] = emo;
                return emo;
            }
        }
        return null;
    }
    private int GetEmojiIndex(string key)
    {
        for (int i = 0, imax = keyList.Count; i < imax; ++i)
        {
            if (keyList[i] == key) return i;
        }
        return -1;
    }

    #endregion
//    #region 老的方式（不知道为什么使用自定义的序列化类在手机上不能反序列化）
//    public void AddEmoji(string newName, string newPrefix, int newFps)
//    {
//        Emoji emoji = GetEmoji(newName, true);
//        emoji.prefix = newPrefix;
//        emoji.fps = newFps;
//        MarkAsChanged();
//    }

//    public void RemoveEmoji(string theName)
//    {
//        Emoji emoji = GetEmoji(theName, false);
//        if (emoji != null) emojiList.Remove(emoji);
//        MarkAsChanged();
//    }

//    public void ChangeEmoji(string theName, string newPrefix, int newFps)
//    {
//        Emoji emoji = GetEmoji(theName, false);
//        if (emoji != null)
//        {
//            emoji.prefix = newPrefix;
//            emoji.fps = newFps;
//        }
//        MarkAsChanged();
//    }

//    public Emoji GetEmoji(string newName, bool createIfMissing)
//    {
//        for (int i = 0, imax = emojiList.Count; i < imax; ++i)
//        {
//            Emoji emo = emojiList[i];
//            if (emo.name == newName) return emo;
//        }
//        if (createIfMissing)
//        {
//            Emoji emo = new Emoji();
//            emo.name = newName;
//            emojiList.Add(emo);
//            return emo;
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
        keyList.Clear();
        valueList.Clear();
        _emojiDic.Clear();
        _emojiDic = null;
        keyList = null;
        valueList = null;
    }
}