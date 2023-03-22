//#define USE_ETC_1

using System.Collections.Generic;
using UnityEngine;

public class CAtlas : MonoBehaviour
{
    public Texture mainTexture;
    [HideInInspector]
    public List<Sprite> spriteList = new List<Sprite>();

    //#define USE_ETC_1
    //public Texture alphaTexture;
    //[HideInInspector]
    //public List<Sprite> spriteAlphaList = new List<Sprite>();
    //private Dictionary<string, Sprite> spriteAlphaDic;
    //#endif

    private Dictionary<string, Sprite> spriteDic;
    private Dictionary<string, Dictionary<bool, List<string>>> spriteNameDic;

    private void InitSpriteDic(bool isAlpha) {
        if (isAlpha) {
            //#define USE_ETC_1
            //if (spriteAlphaDic == null) {
            //    spriteAlphaDic = new Dictionary<string, Sprite>();
            //    foreach (Sprite sprite in spriteAlphaList) {
            //        spriteAlphaDic[sprite.name] = sprite;
            //    }
            //}
            //#endif
        } else {
            if (spriteDic == null) {
                spriteDic = new Dictionary<string, Sprite>();
                foreach (Sprite sprite in spriteList) {
                    spriteDic[sprite.name] = sprite;
                }
            }  
        }
    }

    public List<string> GetSpriteNameList(string prefixName, bool isReverse) {
        if (spriteNameDic == null) {
            spriteNameDic = new Dictionary<string, Dictionary<bool, List<string>>>();
        }
        if (spriteNameDic.ContainsKey(prefixName) == false) {
            spriteNameDic[prefixName] = new Dictionary<bool, List<string>>();
        }
        if (spriteNameDic[prefixName].ContainsKey(isReverse) == false) {
            var list = new List<string>();
            for (int i = 0, imax = spriteList.Count; i < imax; ++i) {
                Sprite sp = spriteList[i];
                if (string.IsNullOrEmpty(prefixName) || sp.name.StartsWith(prefixName)) {
                    list.Add(sp.name);
                }
            }
            list.Sort();
            if (isReverse) {
                list.Reverse();
            }
            spriteNameDic[prefixName][isReverse] = list;
        }
        return spriteNameDic[prefixName][isReverse];
    }

    public void AddSprite(Sprite sprite,bool alpha = false)
    {
        if (alpha) {
            //#define USE_ETC_1
            //spriteAlphaList.Add(sprite);
            //#endif
        } else {
            spriteList.Add(sprite);
        }
    }

    public Sprite GetSprite(string theName,bool alpha = false)
    {
        if (string.IsNullOrEmpty(theName)) return null;
        InitSpriteDic(alpha);
        if (alpha) {
            //#define USE_ETC_1
            //if (spriteAlphaDic != null && spriteAlphaDic.ContainsKey(theName)) {
            //    return spriteAlphaDic[theName];
            //}
            //#endif
        } else {
            if (spriteDic != null && spriteDic.ContainsKey(theName)) {
                return spriteDic[theName];
            }
        }
        if (alpha) {
            // foreach (Sprite sprite in spriteAlphaList) {
            //     if (sprite.name == theName) {
            //         return sprite;
            //     }
            // }
        } else {
            foreach (Sprite sprite in spriteList) {
                if (sprite.name == theName) {
                    return sprite;
                }
            }
        }
        return null;
    }

    public void MarkAsChanged()
    {
        if (MingUIAgent.IsEditorMode)
        {
            MingUIAgent.EditorSetDirty(gameObject); //一定要调用，要通知磁盘持久化该物体
        }
        //更新下当前场景中持有该图集的sprite
        CSprite[] list = FindObjectsOfType(typeof (CSprite)) as CSprite[];
        if (list == null) return;
        for (int i = 0; i < list.Length; i++)
        {
            CSprite sprite = list[i];
            if (sprite.Atlas == this)
            {
                sprite.RefreshSprite();
                if (MingUIAgent.IsEditorMode)
                {
                    MingUIAgent.EditorSetDirty(sprite);
                }
            }
        }
    }

    public void ClearSpirteDic() {
        if (spriteDic != null) {
            spriteDic.Clear();
            spriteDic = null;
        }
        if (spriteNameDic != null) {
            spriteNameDic.Clear();
            spriteNameDic = null;
        }
    }

    /// <summary>
    /// 资源释放时需要手动调用
    /// </summary>
    public void OnDestroy()
    {
        if (mainTexture != null) {
            Resources.UnloadAsset(mainTexture);
            mainTexture = null;
        }
        spriteList.Clear();
        spriteList = null;
        //#define USE_ETC_1
        //if (alphaTexture != null) {
        //    Resources.UnloadAsset(alphaTexture);
        //    alphaTexture = null;
        //}
        //spriteAlphaList.Clear();
        //spriteAlphaList = null;
        //if (spriteAlphaDic != null) {
        //    spriteAlphaDic.Clear();
        //    spriteAlphaDic = null;
        //}
        //endif
        if (spriteDic != null) {
            spriteDic.Clear();
            spriteDic = null;
        }
        if (spriteNameDic != null) {
            spriteNameDic.Clear();
            spriteNameDic = null;
        }
    }
}