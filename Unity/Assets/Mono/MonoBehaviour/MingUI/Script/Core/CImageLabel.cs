using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 艺术字文本
/// 仅限于一行文本
/// 此类为运行类
/// </summary>
public class CImageLabel : MonoBehaviour
{
    public enum AlignType
    {
        Left,
        Center,
        Right
    }

    [SerializeField] private CFont _font;
    [SerializeField] private float _space;
    [SerializeField] private AlignType _align = AlignType.Left;
    private string _text = string.Empty;
    private List<CSprite> _sprites = new List<CSprite>();
    private List<CSprite> _spritePool = new List<CSprite>();

    private List<CSprite> Sprites
    {
        get
        {
            if (_sprites == null)
            {
                _sprites = new List<CSprite>();
            }
            return _sprites;
        }
    }

    public void ChangeColor(Color color)
    {
        for (var i = 0; i < Sprites.Count; i++)
        {
            var sp = Sprites[i];
            sp.color = color;
        }
    }

    private List<CSprite> SpritePool
    {
        get
        {
            if (_spritePool == null)
            {
                _spritePool = new List<CSprite>();
            }
            return _spritePool;
        }
    }

    private RectTransform _selfTransform;
    private bool _dataChanged;
    private float _totalLength; //总长度
    private float _firstWidth; //第一个的宽度
    private float _lastWidth;

    public CFont Font
    {
        get { return _font; }
        set
        {
            if (value != _font)
            {
                _font = value;

                string temp = _text;
                _text = string.Empty;
                Text = temp;
            }
        }
    }

    public float Space
    {
        get { return _space; }
        set
        {
            if (Math.Abs(value - _space) > 0)
            {
                _space = value;
                Reposition();
            }
        }
    }

    public AlignType Align
    {
        get { return _align; }
        set
        {
            if (value != _align)
            {
                _align = value;
                Reposition();
            }
        }
    }

    public RectTransform SelfTransform
    {
        get
        {
            if (_selfTransform == null)
            {
                _selfTransform = GetComponent<RectTransform>();
            }
            return _selfTransform;
        }
    }

    public string Text
    {
        get { return _text; }
        set
        {
            if (_text != value)
            {
                _text = value;
                _dataChanged = true;
                if (Application.isPlaying)
                {
                    Update();
                }
            }
        }
    }

    private void Update()
    {
        if (_dataChanged)
        {
            ProcessSymbols();
            _dataChanged = false;
        }
    }

    private void ProcessSymbols()
    {
        if (Font == null)
        {
            return;
        }

        _totalLength = 0;
        for (int i = Sprites.Count - 1; i >= 0; i--)
        {
            CSprite sprite = Sprites[i];
            sprite.enabled = false;
            Sprites.RemoveAt(i);
            SpritePool.Add(sprite);
        }

        if (!string.IsNullOrEmpty(_text))
        {
            int textLength = _text.Length;
            int ch;
            int index = 0;
            CSprite sprite = null;
            for (int i = 0; i < textLength; i++)
            {
                ch = Text[i];
                if (ch < ' ') continue;
                string spriteName = Font.MatchSymbol(_text, i, textLength);
                if (!string.IsNullOrEmpty(spriteName))
                {
                    sprite = GetSprite();
                    Sprites.Add(sprite);
                    sprite.enabled = true;
                    sprite.Atlas = Font.atlas;
                    sprite.SpriteName = spriteName;
                    sprite.SetNativeSize();
                    _totalLength += sprite.Width + _space;
                    if (index == 0)
                    {
                        _firstWidth = sprite.Width; //记录第一个宽度
                    }
                    index++;
                }
            }
            if (sprite != null)
            {
                _lastWidth = sprite.Width; //记录最后一个宽度
            }
            _totalLength -= _space;
            Reposition();
        }
    }

    

    private void Reposition()
    {
        Vector3 offset = Vector3.zero;
        if (Align == AlignType.Left)
        {
            offset.x = _firstWidth * 0.5f;
        }
        else if (Align == AlignType.Center)
        {
            offset.x += SelfTransform.rect.size.x * 0.5f - _totalLength * 0.5f + _firstWidth * 0.5f;
        }
        else if (Align == AlignType.Right)
        {
            offset.x += SelfTransform.rect.size.x - _totalLength + _lastWidth * 0.5f;
        }

        for (int i = 0; i < Sprites.Count; i++)
        {
            Sprites[i].rectTransform.anchoredPosition3D = GetSymPos(i, offset);
        }
    }

    private Vector3 GetSymPos(int index, Vector3 offset)
    {
        CSprite lastSprite = null;
        if (index > 0 && index < Sprites.Count)
        {
            lastSprite = Sprites[index - 1];
        }
        if (lastSprite != null)
        {
            offset.x = lastSprite.rectTransform.anchoredPosition.x + lastSprite.Width * 0.5f + Sprites[index].Width * 0.5f + _space;
        }
        return offset;
    }

    private CSprite GetSprite()
    {
        CSprite sprite;
        int last = SpritePool.Count - 1;
        if (last >= 0)
        {
            sprite = SpritePool[last];
            SpritePool.RemoveAt(last);
        }
        else
        {
            GameObject go = new GameObject("Symbol");
            go.transform.parent = transform;
            go.transform.localScale = Vector3.one;
            go.transform.localEulerAngles = Vector3.zero;
            sprite = go.AddComponent<CSprite>();
            sprite.raycastTarget = false;
            sprite.rectTransform.pivot = Vector2.one * 0.5f; //中心点为原点
            sprite.rectTransform.anchorMin = sprite.rectTransform.anchorMax = new Vector2(0, 0.5f); //左中角为锚点
            sprite.showWhiteSource = false;
        }
        return sprite;
    }

    private void OnDestroy()
    {
        _sprites = null;
        _spritePool = null;
    }

    public void RefreshLabel()
    {
        string temp = _text;
        _text = null;
        Text = temp;
    }

    public float totalLength
    {
        get { return _totalLength; }
    }
}