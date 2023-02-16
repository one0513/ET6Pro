using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 富文本
/// 增强功能：
/// 1.支持图片标签及点击事件（自定义图片路径、缩放、大小）
/// 2.支持表情播放（自定义表情索引、缩放、大小）
/// 3.支持超链接
/// 4.完美自适应方案
/// </summary>
public class CRichLabel : CLabel, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler 
{
    public enum HtmlType 
    {
        Image, //图片
        Link, //超链接
        Emoji, //表情
        Rect,   //Rect
    }
    public enum AlignType 
    {
        Bottom,
        Center,
        Top
    }
    [SerializeField]
    private AlignType _align = AlignType.Bottom;

    private List<HtmlSymbol> _htmlSymbols = new List<HtmlSymbol>(); //当前正在用的标签
    private List<ImageSymbol> _imagePool; //图像标签池
    private List<EmojiSymbol> _emojiPool; //表情标签池
    private List<LinkSymbol> _linkPool; //超链接标签池

    private List<RectSymbol> _rectPool;//通用位置标签池

    private List<ImageSymbol> ImagePool
    {
        get
        {
            if (_imagePool == null) _imagePool = new List<ImageSymbol>();
            return _imagePool;
        }
    }

    private List<EmojiSymbol> EmojiPool
    {
        get
        {
            if (_emojiPool == null) _emojiPool = new List<EmojiSymbol>();
            return _emojiPool;
        }
    }

    private List<LinkSymbol> LinkPool
    {
        get
        {
            if (_linkPool == null) _linkPool = new List<LinkSymbol>();
            return _linkPool;
        }
    }

    private List<RectSymbol> RectPool
    {
        get
        {
            if (_rectPool == null) _rectPool = new List<RectSymbol>();
            return _rectPool;
        }
    }

    private StringBuilder _builder = new StringBuilder();
    private string _htmlText;
    private bool _hasLink;
    private bool _hasUnderLine;
    private UIEventListener.StringDelegate _onLink;
    private UIEventListener.StringDelegate _onImageClick;

    private List<RectTransform> rectItemList;
    private List<RectTransform> rectInsideItemList;
    private string lastRectStr = string.Empty;

    protected static string[] splitGapStrs = new string[] { "#-#" };

    /// <summary>
    /// 设置HtmlText
    /// </summary>
    public string HtmlText
    {
        get { return _htmlText; }
        set
        {
            if (value != _htmlText)
            {
                _htmlText = value;
                ProcessHtml();
            }
        }
    }

    public void SetRectText(string oriText,string gapTxt, RectTransform[] objects) {
        if (lastRectStr == oriText) return;
        lastRectStr = oriText;
        if (rectItemList == null) {
            rectItemList = new List<RectTransform>();
        } else {
            rectItemList.Clear();
        }
        if(objects != null) {
            for(int i = 0; i < objects.Length; i++) {
                rectItemList.Add(objects[i]);
            }
        }
        string[] gapStrs;
        if (string.IsNullOrEmpty(gapTxt)) {
            gapStrs = splitGapStrs;
        } else {
            gapStrs = new string[] { gapTxt };
        }
        string[] infos = oriText.Split(gapStrs, StringSplitOptions.None);
        int index = 0;
        _builder.Length = 0;
        if (infos != null) {
            for(int i = 0; i < infos.Length; i++) {
                _builder.Append(infos[i]);
                if (rectItemList.Count > index) {
                    _builder.AppendFormat(@"<rct index={0} width={1} height={2} inside=0 />", index, rectItemList[index].sizeDelta.x, rectItemList[index].sizeDelta.y);
                    index++;
                }
            }
        }
        HtmlText = _builder.ToString();
    }

    public void SetRectTextInside(string oriText, string gapTxt, RectTransform[] objects) {
        if (lastRectStr == oriText) return;
        lastRectStr = oriText;
        if (rectInsideItemList == null) {
            rectInsideItemList = new List<RectTransform>();
        } else {
            rectInsideItemList.Clear();
        }
        if (objects != null) {
            for (int i = 0; i < objects.Length; i++) {
                rectInsideItemList.Add(objects[i]);
            }
        }
        string[] gapStrs;
        if (string.IsNullOrEmpty(gapTxt)) {
            gapStrs = splitGapStrs;
        } else {
            gapStrs = new string[] { gapTxt };
        }
        string[] infos = oriText.Split(gapStrs, StringSplitOptions.None);
        int index = 0;
        _builder.Length = 0;
        if (infos != null) {
            for (int i = 0; i < infos.Length; i++) {
                _builder.Append(infos[i]);
                if (rectInsideItemList.Count > index) {
                    _builder.AppendFormat(@"<rct index={0} width={1} height={2} inside=1 />", index, rectInsideItemList[index].sizeDelta.x, rectInsideItemList[index].sizeDelta.y);
                    index++;
                }
            }
        }
        HtmlText = _builder.ToString();
    }

    /// <summary>
    /// 添加链接事件
    /// </summary>
    /// <param name="callback"></param>
    public void AddLink(UIEventListener.StringDelegate callback)
    {
        _onLink = callback;
    }

    /// <summary>
    /// 移除链接事件
    /// </summary>
    public void RemoveLink()
    {
        _onLink = null;
    }

    /// <summary>
    /// 添加图片点击
    /// </summary>
    /// <param name="callback"></param>
    public void AddImageClick(UIEventListener.StringDelegate callback)
    {
        _onImageClick = callback;
    }

    /// <summary>
    /// 删除图片点击
    /// </summary>
    public void RemoveImageClick()
    {
        _onImageClick = null;
    }
    /// <summary>
    /// 解析标签
    /// </summary>
    private void ProcessHtml()
    {
        _builder.Length = 0;
        int index = 0;
        _hasLink = false;
        _hasUnderLine = false;

        for (int i = 0; i < _htmlSymbols.Count; i++) //回池
        {
            HtmlSymbol sym = _htmlSymbols[i];
            sym.Hide();
            if (sym is ImageSymbol)
            {
                ImagePool.Add(sym as ImageSymbol);
            }
            else if (sym is EmojiSymbol)
            {
                EmojiPool.Add(sym as EmojiSymbol);
            }
            else if (sym is LinkSymbol)
            {
                LinkPool.Add(sym as LinkSymbol);
            } 
            else if (sym is RectSymbol) {
                RectPool.Add(sym as RectSymbol);
            }
        }
        _htmlSymbols.Clear();

        MatchCollection result = HtmlUtil_MingUI.allReg.Matches(_htmlText);
        for (int i = 0; i < result.Count; i++)
        {
            Match match = result[i];
            _builder.Append(_htmlText.Substring(index, match.Index - index));
            int nowLength = _builder.Length;
            string matchValue = match.Value; //value 就是 match.Groups[0]

            if (HtmlUtil_MingUI.imgReg.IsMatch(matchValue))
            {
                ImageSymbol symbol = GetImageSymbol();
                symbol.src = match.Groups[1].Value;
                symbol.scale = Convert.ToSingle(match.Groups[2].Value);
                symbol.size = Convert.ToInt32(match.Groups[3].Value);
                symbol.click = match.Groups[4].Value;
                symbol.Show();
                _htmlSymbols.Add(symbol);

                bool same = AddBlank(symbol.size);
                if (same)
                {
                    symbol.startIndex = nowLength;
                }
                else
                {
                    symbol.startIndex = nowLength + symbol.size.ToString().Length + 7; //要加上这7个符号位<Size=>
                }
            }
            else if (HtmlUtil_MingUI.aReg.IsMatch(matchValue))
            {
                _hasLink = true;

                LinkSymbol symbol = GetLinkSymbol();
                symbol.size = fontSize;
                symbol.linkStr = match.Groups[5].Value;
                symbol.isUnderLine = match.Groups[6].Value == "true";
                symbol.content = match.Groups[7].Value;
                _htmlSymbols.Add(symbol);

                symbol.startIndex = nowLength;
                _builder.Append(symbol.content);

                if (symbol.isUnderLine) {
                    _hasUnderLine = true;
                }
            }
            else if (HtmlUtil_MingUI.emoReg.IsMatch(matchValue))
            {
                EmojiSymbol symbol = GetEmojiSymbol();
                symbol.name = match.Groups[8].Value;
                symbol.scale = Convert.ToSingle(match.Groups[9].Value);
                symbol.size = Convert.ToInt32(match.Groups[10].Value);
                symbol.Show();
                _htmlSymbols.Add(symbol);

                bool same = AddBlank(symbol.size);
                if (same)
                {
                    symbol.startIndex = nowLength;
                }
                else
                {
                    symbol.startIndex = nowLength + symbol.size.ToString().Length + 7; //要加上这7个符号位<Size=>
                }
            } 
            else if (HtmlUtil_MingUI.rectReg.IsMatch(matchValue)) {
                RectSymbol symbol = GetRectSymbol();
                symbol.index = Convert.ToInt32(match.Groups[11].Value);
                symbol.width = Convert.ToInt32(match.Groups[12].Value);
                symbol.height = Convert.ToInt32(match.Groups[13].Value);
                symbol.inside = Convert.ToInt32(match.Groups[14].Value) == 1;
                symbol.size = symbol.width;
                symbol.Show();
                if (symbol.inside && rectInsideItemList != null && rectInsideItemList.Count > symbol.index) {
                    RectTransform rectItem = rectInsideItemList[symbol.index];
                    SetRectInsideItemParent(rectItem);
                    symbol.SetInsideRect(rectItem);
                }
                _htmlSymbols.Add(symbol);

                bool same = AddBlank(symbol.size);
                if (same) {
                    symbol.startIndex = nowLength;
                } else {
                    symbol.startIndex = nowLength + symbol.size.ToString().Length + 7; //要加上这7个符号位<Size=>
                }
            }
            index = match.Index + match.Length;
        }
        _builder.Append(_htmlText.Substring(index, _htmlText.Length - index));

        m_Text = _builder.ToString();
        SetVerticesDirty();
        SetLayoutDirty();
        //for (int i = 0; i < Text.Length; i++)
        //{
        //    print("[" + i + "]:" + Text[i]);
        //}
    }

    protected override void OnPopulateMesh(VertexHelper toFill)
    {
        base.OnPopulateMesh(toFill);
        ProcessSymbol(toFill);
    }

    /// <summary>
    /// 解析符号
    /// </summary>
    private void ProcessSymbol(VertexHelper toFill)
    {
        UICharInfo[] chars = cachedTextGenerator.GetCharactersArray();
        UILineInfo[] lines = cachedTextGenerator.GetLinesArray();
        //for (int i = 0; i < chars.Length; i++)
        //{
        //    print("[" + i + "]宽度：" + chars[i].charWidth / pixelsPerUnit + "坐标：" + chars[i].cursorPos / pixelsPerUnit);
        //}
        //for (int j = 0; j < lines.Length; j++)
        //{
        //    print("[[" + j + "]]高度" + lines[j].height / pixelsPerUnit + "TopY：" +  lines[j].topY / pixelsPerUnit);
        //}
        if (chars.Length == 0 || lines.Length == 0)
        {
            return;
        }
        if (_hasUnderLine) {
            //下划线原始网格构建
            Vector2 extents = rectTransform.rect.size;
            var settings = GetGenerationSettings(extents);
            cachedTextGenerator.Populate("_", settings);
        }
        int nowLineIndex = 0; //当前行的索引
        for (int i = 0; i < _htmlSymbols.Count; i++)
        {
            HtmlSymbol sym = _htmlSymbols[i];
            UILineInfo line = lines[0];
            for (int j = nowLineIndex; j < lines.Length; j++)
            {
                if (sym.startIndex >= lines[j].startCharIdx)
                {
                    if (j == lines.Length - 1 || (sym.startIndex < lines[j + 1].startCharIdx)) //最后一排
                    {
                        nowLineIndex = j;
                        line = lines[j];
                        break;
                    }
                }
            }
          
            //Debug.Log("[line]:" + nowLineIndex + "  [index]:" + sym.startIndex + "是：" + Text[sym.startIndex]);
            if (sym.startIndex >= chars.Length)
            {
                var pos = sym.localPosition;
                pos.x = -10000;
                sym.localPosition = pos;
                sym.AdjustPosition(_align);
                if(sym is RectSymbol) {
                    SetRectItemPosition(sym as RectSymbol);
                }
                continue;
            }

            sym.lineHeight = line.height / pixelsPerUnit;//记录高度
            UICharInfo charInfo = chars[sym.startIndex];
            Vector2 startPos = charInfo.cursorPos / pixelsPerUnit;//这个符号的起始字符的位置
            if (sym is ImageSymbol || sym is EmojiSymbol)//图片符号/表情符号
            {
                startPos.x += charInfo.charWidth / pixelsPerUnit * 0.5f; //取当前字符的宽度
                startPos.y = (line.topY - line.height) / pixelsPerUnit; //取这一行的最大高度,+2是因为行高存在偏移量
                sym.localPosition = startPos;
                sym.AdjustPosition(_align);
            } 
            else if (sym is RectSymbol) {
                startPos.x += charInfo.charWidth / pixelsPerUnit * 0.5f; //取当前字符的宽度
                startPos.y = (line.topY - line.height) / pixelsPerUnit; //取这一行的最大高度,+2是因为行高存在偏移量
                sym.localPosition = startPos;
                sym.AdjustPosition(_align);
                SetRectItemPosition(sym as RectSymbol);
            }
            else if (sym is LinkSymbol)//链接符号
            {
                LinkSymbol linkSym = sym as LinkSymbol;
                linkSym.linkRects.Clear();
                int contentLength = linkSym.content.Length;//内容长度
                int lastCharIndex = sym.startIndex + contentLength - 1; //最后一个字符位置
                int currentLine = nowLineIndex;//当前所在的行
                int lineEndIndex;//当前行的最后一个字符索引

                if (currentLine < (lines.Length - 1))//当前行的最后一个字符
                {
                    lineEndIndex = lines[currentLine + 1].startCharIdx - 1;
                }
                else
                {
                    lineEndIndex = lastCharIndex;//最后一行
                }

                float charWidth = charInfo.charWidth / pixelsPerUnit;//真实的字宽度
                float lineHeight = line.height / pixelsPerUnit;//真实的行高度
                float lineTopY = line.topY / pixelsPerUnit;//真实的行上Y坐标
                Rect rect = new Rect(startPos.x, lineTopY - lineHeight, charWidth, lineHeight);

                for (int index = sym.startIndex + 1; index <= lastCharIndex; index++)
                {
                    if (index >= chars.Length) break;//编辑器会莫名其妙报空错

                    if (index > lineEndIndex) //换行了
                    {
                        linkSym.linkRects.Add(rect);//提交一个包围盒
                        currentLine++;
                        if (currentLine < (lines.Length - 1)) //当前行的最后一个字符
                        {
                            lineEndIndex = lines[currentLine + 1].startCharIdx - 1;
                        }
                        else
                        {
                            lineEndIndex = lastCharIndex; //最后一行
                        }
                        startPos = chars[index].cursorPos / pixelsPerUnit;
                        charWidth = chars[index].charWidth / pixelsPerUnit;
                        lineHeight = lines[currentLine].height / pixelsPerUnit;
                        lineTopY = lines[currentLine].topY / pixelsPerUnit;
                        rect = new Rect(startPos.x, lineTopY - lineHeight, charWidth, lineHeight);
                    }
                    else //在本行
                    {
                        rect.width += chars[index].charWidth / pixelsPerUnit;
                    }
                }
                linkSym.linkRects.Add(rect);//提交包围盒

                //使用包围盒数据建立下划线
                if (linkSym.isUnderLine) {
                    for (int k = 0; k < linkSym.linkRects.Count; k++) {
                        Vector3 _StartBoxPos = new Vector3(linkSym.linkRects[k].x, linkSym.linkRects[k].y, 0.0f);
                        Vector3 _EndBoxPos = _StartBoxPos + new Vector3(linkSym.linkRects[k].width, 0.0f, 0.0f);
                        string colorTxt = HtmlUtil_MingUI.ExtractTextHtmlColor(linkSym.content);
                        if (string.IsNullOrEmpty(colorTxt)) {
                            AddUnderlineQuad(toFill, cachedTextGenerator.verts, _StartBoxPos, _EndBoxPos, color);
                        } else {
                            AddUnderlineQuad(toFill, cachedTextGenerator.verts, _StartBoxPos, _EndBoxPos, HtmlUtil_MingUI.ConverHtmlClrToUnityClr(colorTxt));
                        }
                    }
                }
            }
        }
    }

    private void SetRectItemPosition(RectSymbol symbol) {
        if (rectItemList != null && rectItemList.Count > symbol.index) {
            var item = rectItemList[symbol.index];
            Vector3 pos = symbol.symbolPosition;
            if(item.pivot.y > 0) {
                pos.y += item.pivot.y * item.sizeDelta.y;
            }
            item.position = new Vector3(pos.x * item.lossyScale.x, pos.y * item.lossyScale.y, pos.z * item.lossyScale.z) + transform.position;
        }
    }

    private void SetRectInsideItemParent(RectTransform rectInside) {
        rectInside.SetParent(transform);
        rectInside.localEulerAngles = Vector3.zero;
        rectInside.localScale = Vector3.one;
        rectInside.pivot = new Vector2(0.5f, 0);//右中为原点
    }

    /// <summary>
    /// 添加中文空格
    /// </summary>
    /// <param name="size"></param>
    /// <returns>这个空格大小是否和文本字体大小相同？</returns>
    private bool AddBlank(int size)
    {
        if (size == fontSize)
        {
            _builder.Append(HtmlUtil_MingUI.BLANK);
            return true;
        }
        else
        {
            _builder.Append(HtmlUtil_MingUI.Size(HtmlUtil_MingUI.BLANK, size));
            return false;
        }
    }

    //添加下划线
    void AddUnderlineQuad(VertexHelper _VToFill, IList<UIVertex> _VTUT, Vector3 _VStartPos, Vector3 _VEndPos,Color col) {
        Vector3[] _TUnderlinePos = new Vector3[4];
        float gapWidth = _VEndPos.x - _VStartPos.x;
        _TUnderlinePos[0] = _VStartPos;
        _TUnderlinePos[1] = _VEndPos;
        _TUnderlinePos[2] = _VEndPos + new Vector3(0, fontSize * 0.12f, 0);
        _TUnderlinePos[3] = _VStartPos + new Vector3(0, fontSize * 0.12f, 0);
        float uvGapX = Mathf.Abs(_VTUT[2].uv0.x - _VTUT[0].uv0.x);
        float uvGapY = Mathf.Abs(_VTUT[1].uv0.y - _VTUT[0].uv0.y);
        for (int i = 0; i < 4; ++i) {
            int tempVertsIndex = i & 3;
            m_TempVerts[tempVertsIndex] = _VTUT[i % 4];
            //改变uv,避免拉伸过渡（因为"_"这个字符的uv是去不到网格的边缘的，拉伸就会出现拖尾）
            if (i < 2) {
                m_TempVerts[tempVertsIndex].uv0.x -= uvGapX * 0.2f;
            } else {
                m_TempVerts[tempVertsIndex].uv0.x += uvGapX * 0.2f;
            }
            if (i == 1 || i == 2) {
                m_TempVerts[tempVertsIndex].uv0.y += uvGapY * 0.2f;
            } else {
                m_TempVerts[tempVertsIndex].uv0.y -= uvGapY * 0.2f;
            }
            m_TempVerts[tempVertsIndex].color = col;
            m_TempVerts[tempVertsIndex].position = _TUnderlinePos[i];
            if (tempVertsIndex == 3)
                _VToFill.AddUIVertexQuad(m_TempVerts);
        }
    }

    private ImageSymbol GetImageSymbol()
    {
        ImageSymbol sym;
        int last = ImagePool.Count - 1;
        if (last >= 0)
        {
            sym = ImagePool[last];
            ImagePool.RemoveAt(last);
        }
        else
        {
            sym = new ImageSymbol();
            GameObject go = new GameObject("CSprite");
            go.transform.SetParent(transform);
            go.transform.localEulerAngles = Vector3.zero;
            go.transform.localScale = Vector3.one;
            sym.sprite = go.AddComponent<CSprite>();
            sym.sprite.showWhiteSource = false;
            sym.sprite.rectTransform.pivot = new Vector2(0.5f,0);//右中为原点
            sym.sprite.autoSnap = true;
            sym.sprite.AddNativeSize((Vector2 size) =>
            {
                sym.selfHeight = size.y;
                sym.AdjustPosition(_align);
            });
            sym.AddClick(OnImageClick);
        }
        return sym;
    }

    private EmojiSymbol GetEmojiSymbol()
    {
        EmojiSymbol sym;
        int last = EmojiPool.Count - 1;
        if (last >= 0)
        {
            sym = EmojiPool[last];
            EmojiPool.RemoveAt(last);
        }
        else
        {
            sym = new EmojiSymbol();
            GameObject go = new GameObject("CSprite");
            go.transform.SetParent(transform);
            go.transform.localEulerAngles = Vector3.zero;
            go.transform.localScale = Vector3.one;
            sym.sprite = go.AddComponent<CSprite>();
            sym.sprite.showWhiteSource = false;
            sym.sprite.rectTransform.pivot = new Vector2(0.5f, 0);//右中为原点
            sym.sprite.autoSnap = true;
            sym.sprite.AddNativeSize((Vector2 size) =>
            {
                sym.selfHeight = size.y;
                sym.AdjustPosition(_align);
            });
            sym.sprite.raycastTarget = false;
        }
        return sym;
    }


    private LinkSymbol GetLinkSymbol()
    {
        LinkSymbol sym;
        int last = LinkPool.Count - 1;
        if (last >= 0)
        {
            sym = LinkPool[last];
            LinkPool.RemoveAt(last);
        }
        else
        {
            sym = new LinkSymbol();
        }
        return sym;
    }

    private RectSymbol GetRectSymbol() {
        RectSymbol sym;
        int last = RectPool.Count - 1;
        if (last >= 0) {
            sym = RectPool[last];
            RectPool.RemoveAt(last);
        } else {
            sym = new RectSymbol();
        }
        return sym;
    }

    private void OnImageClick(string click)
    {
        print(click);
        if (_onImageClick != null) _onImageClick(click);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!_hasLink) return; //前提是有超链接标签

        Vector2 lp;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out lp);

        for (int i = 0; i < _htmlSymbols.Count; i++)
        {
            LinkSymbol symbol = _htmlSymbols[i] as LinkSymbol;
            if (symbol != null)
            {
                for (int j = 0; j < symbol.linkRects.Count; j++)
                {
                    if (symbol.linkRects[j].Contains(lp))
                    {
                        OnTextClick(symbol.linkStr);
                        return;
                    }
                }
            }
        }
    }

    private void OnTextClick(string linkStr)
    {
        print(linkStr);
        if (_onLink != null) _onLink(linkStr);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        _onLink = null;
        for (int i = 0; i < _htmlSymbols.Count; i++)
        {
            _htmlSymbols[i].Dispose();
        }
        _htmlSymbols.Clear();
        _htmlSymbols = null;

        if (_imagePool != null)
        {
            _imagePool.Clear();
        }
        if (_emojiPool != null)
        {
            _emojiPool.Clear();
        }
        if (_linkPool != null)
        {
            _linkPool.Clear();
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
    }

    public void OnPointerUp(PointerEventData eventData)
    {
    }
}

#region 符号类型

public abstract class HtmlSymbol
{
    public CRichLabel.HtmlType type;
    public int size; //该符号默认占用多大号字体的空间
    public int startIndex; //起始索引
    public float lineHeight;//所处的行高
    public float selfHeight = 0;//自身高度偏移
    public Vector3 localPosition;//自身的相对坐标
    public virtual void Show()
    {
    }

    public virtual void Hide()
    {
    }

    public virtual void Dispose()
    {

    }
    public virtual void AdjustPosition(CRichLabel.AlignType align)
    {

    }
}

public class ImageSymbol : HtmlSymbol
{
    public string src; //来源：MingUI.sss
    public float scale; //缩放
    public string click; //点击事件
    public CSprite sprite;

    private UIEventListener.StringDelegate _onClick;

    public ImageSymbol()
    {
        type = CRichLabel.HtmlType.Image;
    }

    private CButton _button;

    private CButton Button
    {
        get
        {
            if (_button == null)
            {
                _button = sprite.gameObject.AddComponent<CButton>();
                _button.AddClick(data => { if (_onClick != null) _onClick(click); });
            }
            return _button;
        }
    }

    public void AddClick(UIEventListener.StringDelegate callback)
    {
        _onClick = callback;
    }

    public override void AdjustPosition(CRichLabel.AlignType align)
    {
        Vector3 pos = localPosition;
        float gap = lineHeight - selfHeight * scale;
        float offsetY = 0;
        if (align == CRichLabel.AlignType.Bottom)
        {
            offsetY = 0;
        }
        else if (align == CRichLabel.AlignType.Center)
        {
            offsetY = gap * 0.5f;
        }
        else
        {
            offsetY = gap;
        }

        pos.y += offsetY;
        sprite.transform.localPosition = pos;
    }

    public override void Show()
    {
        if (sprite != null)
        {
            sprite.Path = src;
            sprite.SetNativeSize();
            sprite.raycastTarget = click != HtmlUtil_MingUI.EMPTY_CLICK;
            if (click == HtmlUtil_MingUI.EMPTY_CLICK)
            {
                sprite.raycastTarget = false;
            }
            else
            {
                sprite.raycastTarget = true;
                Button.OpenButtonEffect = true;
                CButtonEffect effect = Button.GetComponent<CButtonEffect>();
                effect.orginScale = Vector3.one * scale;
            }
            sprite.transform.localScale = Vector3.one * scale;
            sprite.gameObject.SetActive(true);
        }
    }

    public override void Hide()
    {
        src = "";
        scale = 1;
        if (sprite != null)
        {
            sprite.gameObject.SetActive(false);
        }
        if (_button != null)
        {
            _button.enabled = false;
        }
    }

    public override void Dispose()
    {
        _onClick = null;
        sprite = null;
    }

    public override string ToString()
    {
        return "[ImageSymbol]:" + "src:" + src + "   scale:" + scale + "   Size:" + size;
    }
}

public class LinkSymbol : HtmlSymbol
{
    public string linkStr; //超链接内容
    public string content; //文本内容
    public bool isUnderLine;
    public List<Rect> linkRects = new List<Rect>();

    public LinkSymbol()
    {
        type = CRichLabel.HtmlType.Link;
    }

    public override void Hide()
    {
        linkStr = "";
        content = "";
        linkRects.Clear();
    }

    public override void Dispose()
    {
        linkRects.Clear();
        linkRects = null;
    }

    public override string ToString()
    {
        return "[LinkSymbol]:" + "linkStr:" + linkStr + "   content:" + content + "   Size:" + size;
    }
}

public class EmojiSymbol : HtmlSymbol
{
    public const string EMOJI_PREFAB = "UI/Emoji/Prefabs/Emoji";//记录表情的表资源
    public string name; //表情名称：&31
    public float scale; //缩放
    public CSprite sprite;

//    private Emoji _emoji;
//    private CSpriteAnimation _spriteAnimation;
//
//    private CSpriteAnimation SpriteAnimation
//    {
//        get
//        {
//            if (_spriteAnimation == null)
//            {
//                _spriteAnimation = sprite.gameObject.AddComponent<CSpriteAnimation>();
//            }
//            return _spriteAnimation;
//        }
//    }

    public EmojiSymbol()
    {
        type = CRichLabel.HtmlType.Emoji;
    }

    public override void AdjustPosition(CRichLabel.AlignType align)
    {
        Vector3 pos = localPosition;
        float gap = lineHeight - selfHeight * scale;
        float offsetY = 0;
        if (align == CRichLabel.AlignType.Bottom)
        {
            offsetY = 0;
        }
        else if (align == CRichLabel.AlignType.Center)
        {
            offsetY = gap * 0.5f;
        }
        else
        {
            offsetY = gap;
        }

        pos.y += offsetY;
        sprite.transform.localPosition = pos;
    }

    public override void Show()
    {
        if (sprite != null)
        {
            MingUIAgent.LoadUIPrefab(EMOJI_PREFAB, LoadComplete);
        }
    }

    private void LoadComplete(ItemVo vo)
    {
        GameObject emojiGo = vo.getObject() as GameObject;
        if (emojiGo != null)
        {
            CAtlas atlas = emojiGo.GetComponent<CAtlas>();
            sprite.Atlas = atlas;
            sprite.SetNativeSize();
            sprite.transform.localScale = Vector3.one * scale;
            sprite.gameObject.SetActive(true);
            sprite.SpriteName = name;
//            _emoji = atlas.GetSprite(name);
//            if (_emoji != null)
//            {
//                
//
////                SpriteAnimation.Loop = true;
////                SpriteAnimation.NamePrefix = _emoji.prefix;
////                SpriteAnimation.FramesPerSecond = _emoji.fps;
//            }
//            else
//            {
//                MingUIAgent.Info("找不到表情："+name);
//            }
        }
    }
    public override void Hide()
    {
        name = "";
        scale = 1;
        if (sprite != null)
        {
            sprite.gameObject.SetActive(false);
        }
    }

    public override void Dispose()
    {
//        _emoji = null;
        sprite = null;
//        _spriteAnimation = null;
    }

    public override string ToString()
    {
        return "[EmojiSymbol]:" + "name:" + name + "   scale:" + scale + "   Size:" + size;
    }
}

public class RectSymbol : HtmlSymbol {

    public int index;//GameObject绑定的id
    public int width;
    public int height;
    public Vector3 symbolPosition;

    public bool inside;
    private RectTransform insideRect;

    public RectSymbol() {
        type = CRichLabel.HtmlType.Rect;
    }

    public override void AdjustPosition(CRichLabel.AlignType align) {
        Vector3 pos = localPosition;
        float gap = lineHeight - height;
        float offsetY = 0;
        if (align == CRichLabel.AlignType.Bottom) {
            offsetY = 0;
        } else if (align == CRichLabel.AlignType.Center) {
            offsetY = gap * 0.5f;
        } else {
            offsetY = gap;
        }

        pos.y += offsetY;
        symbolPosition = pos;

        if (inside && insideRect != null) {
            insideRect.localPosition = pos;
        }
    }

    public void SetInsideRect(RectTransform rect) {
        insideRect = rect;
    }

    public override void Show() {
        
    }

    public override void Hide() {
        
    }

    public override void Dispose() {
        insideRect = null;
    }

    public override string ToString() {
        return string.Format("[RectSymbol]: width:{0}   height:{1}  size:{2}    localPosition:{3}", width, height, size, symbolPosition.ToString());
    }
}
#endregion