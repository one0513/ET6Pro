using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CMeshProLabel : TextMeshProUGUI, ICanvasRaycastFilter, IGray ,IPointerClickHandler
{
    private float _groupAlpha = 1;
    public bool raycastConsiderAlpha = false; //自身透明度是否影响事件？默认不影响：即alpha即使为0，也可以接受点击等事件
    private bool _gray;
    private int currentTextValueTag = 0;
    private int lastTextValueTag = 0;
    private string lasetText;
    private Camera eventCamera;
    private Dictionary<int, int> line_index_Link_index = new Dictionary<int, int>();
    private System.Action<string, PointerEventData> onLinkClick;
    private CMeshProEffectUGUI cMeshProEffectUGUI;

    [SerializeField]
    private bool m_EnabelLink = false;

    public bool EnabelLink {
        get { return m_EnabelLink; }
        set {
            m_EnabelLink = value;
            UpdateLinkDict();
        }
    }

    public override string text {
        get { return base.text; }
        set {
            if (m_text != value) {
                currentTextValueTag++;
            }

            base.text = value;
        }
    }

    private CMeshProColor cMeshProColor;

    protected override void OnRectTransformDimensionsChange() {
        base.OnRectTransformDimensionsChange();
        currentTextValueTag++;
    }

    public override void Rebuild(CanvasUpdate update) {
        base.Rebuild(update);
        onRebuild();
    }

    private void onRebuild() {
        if (lastTextValueTag != currentTextValueTag) {
            UpdateLinkDict();
            lastTextValueTag = currentTextValueTag;
        }
    }

    protected override void Awake() {
        base.Awake();
        cMeshProEffectUGUI = transform.GetComponent<CMeshProEffectUGUI>();
    }

    #region Link相关

    public void SetLinkEnabel(bool enabel) {
        raycastTarget = enabel;
        EnabelLink = enabel;
        if (enabel) {
            richText = true;
        }
    }

    public void AddClick(System.Action<string, PointerEventData> _onLinkClick, Camera camera) {
        onLinkClick = _onLinkClick;
        eventCamera = camera;
    }

    public void RemoveClick() {
        onLinkClick = null;
        eventCamera = null;
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (!richText || !EnabelLink) {
            return;
        }
        

        // if (eventCamera == null) {
        //     Debug.LogError("UI相机未传入");
        //     return;
        // }
        int lineIndex = TMP_TextUtilities.FindIntersectingLink(this, eventData.position, eventCamera);
        if (lineIndex < 0) {
            return;
        }

        //int linkIndex = GetLineLinkIndex(lineIndex);
        //if (linkIndex < 0) {
        //    // Debug.LogWarning($"第 {lineIndex} 行没有超链接");
        //    return;
        //}

        TMP_LinkInfo linkInfo = textInfo.linkInfo[lineIndex];
        if (onLinkClick != null) {
            onLinkClick(linkInfo.GetLinkID(), eventData);
        }
    }

    private void UpdateLinkDict() {
        if (!richText || !EnabelLink || !raycastTarget) {
            return;
        }

        if (string.IsNullOrEmpty(text)) {
            return;
        }

        line_index_Link_index.Clear();
        if (textInfo.lineInfo.Length <= 0) {
            return;
        }

        for (int i = 0; i < textInfo.linkInfo.Length; i++) {
            var a = textInfo.linkInfo[i];
            int linkTextfirstCharacterIndex = textInfo.linkInfo[i].linkTextfirstCharacterIndex;
            int line = GetLinkLine(linkTextfirstCharacterIndex);
            if (line != -1) {
                line_index_Link_index[line] = i;
            }
        }
    }

    private int GetLinkLine(int index) {
        for (int i = 0; i < textInfo.lineInfo.Length; i++) {
            var lineInfo = textInfo.lineInfo[i];
            if (index >= lineInfo.firstCharacterIndex && index <= lineInfo.lastVisibleCharacterIndex) {
                return i;
            }
        }

        return -1;
    }

    public int GetLineLinkIndex(int line) {
        int index;
        if (!line_index_Link_index.TryGetValue(line, out index)) {
            index = -1;
        }

        return index;
    }

    #endregion


    /// <summary>
    /// 这里重写是因为UGUI在响应事件时没有计算透明度
    /// </summary>
    /// <param name="screenPoint"></param>
    /// <param name="eventCamera"></param>
    /// <returns></returns>
    public virtual bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera) {
        if (raycastConsiderAlpha && (color.a <= 0f || _groupAlpha <= 0f)) {
            return false;
        }

        return true;
    }

    /// <summary>
    /// 透明度
    /// </summary>
    public float Alpha
    {
        get { return color.a; }
        set
        {
            if (Math.Abs(color.a - value) > 0)
            {
                Color temp = color;
                temp.a = value;
                color = temp;
            }
        }
    }

    /// <summary>
    /// 灰度化？
    /// </summary>
    public bool Gray {
        get { return _gray; }
        set {
            if (value != _gray) {
                _gray = value;
                this.isGray = value ? 1 : -1;
            }
        }
    }

    public void ValidSize() {
        LayoutRebuilder.ForceRebuildLayoutImmediate(this.rectTransform);
    }

    /// <summary>
    /// 宽度
    /// </summary>
    public float Width {
        get { return Size.x; }
        set { Size = new Vector2(value, Size.y); }
    }

    /// <summary>
    /// 高度
    /// </summary>
    public float Height {
        get { return Size.y; }
        set { Size = new Vector2(Size.x, value); }
    }


    /// <summary>
    /// 大小
    /// </summary>
    public Vector2 Size {
        get { return rectTransform.rect.size; }
        set {
            if (value != rectTransform.rect.size) {
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, value.x);
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, value.y);
            }
        }
    }

    public Color EffectOutLineColor {
        get {
            if (cMeshProEffectUGUI == null) {
                return Color.white;
            }

            return cMeshProEffectUGUI.effectColor;
        }
        set {
            if (cMeshProEffectUGUI != null && cMeshProEffectUGUI.effectColor != value) {
                cMeshProEffectUGUI.effectColor = value;
            }
        }
    }

    public float EffectOutLineWidth {
        get {
            if (cMeshProEffectUGUI != null) {
                return cMeshProEffectUGUI.OutlineWidth;
            } else {
                return 0;
            }
        }
        set {
            if (cMeshProEffectUGUI != null) {
                cMeshProEffectUGUI.OutlineWidth = value;
            }
        }
    }

    public float EffectShadowOffsetX {
        get {
            if (cMeshProEffectUGUI != null) {
                return cMeshProEffectUGUI.shadowOffsetX;
            } else {
                return 0;
            }
        }
        set {
            if (cMeshProEffectUGUI != null) {
                cMeshProEffectUGUI.shadowOffsetX = value;
            }
        }
    }

    public float EffectShadowOffsetY {
        get {
            if (cMeshProEffectUGUI != null) {
                return cMeshProEffectUGUI.shadowOffsetY;
            } else {
                return 0;
            }
        }
        set {
            if (cMeshProEffectUGUI != null) {
                cMeshProEffectUGUI.shadowOffsetY = value;
            }
        }
    }

    public CMeshProColor MeshProColor {
        get
        {
            if (cMeshProColor == null)
            {
                cMeshProColor = GetComponent<CMeshProColor>();
            }
            return cMeshProColor;
        }
    }

    /*public void SetColor(LuaTable bBolors, LuaTable tColors)
    {
        if (MeshProColor == null)
        {
            gameObject.AddComponent<CMeshProColor>();
        }
        MeshProColor.SetColor(bBolors.ToList(), tColors.ToList());
    }*/

    public void SetColorEnabled(bool isEnabled)
    {
        if (MeshProColor != null)
        {
            MeshProColor.SetEnabled(isEnabled);
        }
    }

    //public void SetColor(Color BLColor, Color TLColor, Color BCColor, Color TCColor, Color BRColor, Color TRColor)
    //{
    //    if (CMeshProColor == null)
    //    {
    //        gameObject.AddComponent<CMeshProColor>();
    //    }
    //    CMeshProColor.SetColor(BLColor, TLColor, BCColor, TCColor, BRColor, TRColor);
    //}

    protected override void FillCharacterVertexBuffers(int i, int index_X4)
    {
        if(MeshProColor==null)
        {
            base.FillCharacterVertexBuffers(i, index_X4);
        }
        else if (!MeshProColor.isEnable)
        {
            base.FillCharacterVertexBuffers(i, index_X4);
        }
        else
        {
            int materialIndex = m_textInfo.characterInfo[i].materialReferenceIndex;
            index_X4 = m_textInfo.meshInfo[materialIndex].vertexCount;

            // Check to make sure our current mesh buffer allocations can hold these new Quads.
            if (index_X4 >= m_textInfo.meshInfo[materialIndex].vertices.Length)
                m_textInfo.meshInfo[materialIndex].ResizeMeshInfo(Mathf.NextPowerOfTwo((index_X4 + 4) / 4));


            TMP_CharacterInfo[] characterInfoArray = m_textInfo.characterInfo;
            m_textInfo.characterInfo[i].vertexIndex = index_X4;

            // Setup Vertices for Characters
            m_textInfo.meshInfo[materialIndex].vertices[0 + index_X4] = characterInfoArray[i].vertex_BL.position;
            m_textInfo.meshInfo[materialIndex].vertices[1 + index_X4] = characterInfoArray[i].vertex_TL.position;
            m_textInfo.meshInfo[materialIndex].vertices[2 + index_X4] = characterInfoArray[i].vertex_TR.position;
            m_textInfo.meshInfo[materialIndex].vertices[3 + index_X4] = characterInfoArray[i].vertex_BR.position;


            // Setup UVS0
            m_textInfo.meshInfo[materialIndex].uvs0[0 + index_X4] = characterInfoArray[i].vertex_BL.uv;
            m_textInfo.meshInfo[materialIndex].uvs0[1 + index_X4] = characterInfoArray[i].vertex_TL.uv;
            m_textInfo.meshInfo[materialIndex].uvs0[2 + index_X4] = characterInfoArray[i].vertex_TR.uv;
            m_textInfo.meshInfo[materialIndex].uvs0[3 + index_X4] = characterInfoArray[i].vertex_BR.uv;


            // Setup UVS2
            m_textInfo.meshInfo[materialIndex].uvs2[0 + index_X4] = characterInfoArray[i].vertex_BL.uv2;
            m_textInfo.meshInfo[materialIndex].uvs2[1 + index_X4] = characterInfoArray[i].vertex_TL.uv2;
            m_textInfo.meshInfo[materialIndex].uvs2[2 + index_X4] = characterInfoArray[i].vertex_TR.uv2;
            m_textInfo.meshInfo[materialIndex].uvs2[3 + index_X4] = characterInfoArray[i].vertex_BR.uv2;


            // Setup UVS3
            m_textInfo.meshInfo[materialIndex].uvs3[0 + index_X4] = characterInfoArray[i].vertex_BL.uv3;
            m_textInfo.meshInfo[materialIndex].uvs3[1 + index_X4] = characterInfoArray[i].vertex_TL.uv3;
            m_textInfo.meshInfo[materialIndex].uvs3[2 + index_X4] = characterInfoArray[i].vertex_TR.uv3;
            m_textInfo.meshInfo[materialIndex].uvs3[3 + index_X4] = characterInfoArray[i].vertex_BR.uv3;

            // Setup UVS4
            m_textInfo.meshInfo[materialIndex].uvs4[0 + index_X4] = characterInfoArray[i].vertex_BL.uv4;
            m_textInfo.meshInfo[materialIndex].uvs4[1 + index_X4] = characterInfoArray[i].vertex_TL.uv4;
            m_textInfo.meshInfo[materialIndex].uvs4[2 + index_X4] = characterInfoArray[i].vertex_TR.uv4;
            m_textInfo.meshInfo[materialIndex].uvs4[3 + index_X4] = characterInfoArray[i].vertex_BR.uv4;

            

            if (!characterInfoArray[i].isHtmlColor)
            {
                int curIndex = i;
                float length = m_characterCount * 2 - 1;
                if(MeshProColor.isContinuity)
                {
                    for (int k = i; k >= 0; k--)
                    {
                        if (characterInfoArray[k].isHtmlColor)
                        {
                            curIndex--;
                        }
                    }
                    int count = 0;
                    for (int k = 0; k < m_characterCount; k++)
                    {
                        if (!characterInfoArray[k].isHtmlColor)
                        {
                            count++;
                        }
                    }
                    length = count * 2 - 1;
                }
                int bSplitCount = MeshProColor.bColors.Count - 1;
                int tSplitCount = MeshProColor.tColors.Count - 1;
                Color color0 = MeshProColor.bColors[0];   //左下
                Color color1 = MeshProColor.tColors[0];   //左上
                Color color2 = MeshProColor.tColors[0];   //左上;
                Color color3 = MeshProColor.bColors[0];   //左下;
                                                          // setup Vertex Colors
                for (int j = 0; j < bSplitCount; j++)
                {
                    color0 = Color.Lerp(color0, MeshProColor.bColors[j + 1], (2 * curIndex - length / bSplitCount * j) / (length / bSplitCount));
                    color3 = Color.Lerp(color3, MeshProColor.bColors[j + 1], (2 * curIndex - length / bSplitCount * j + 1) / (length / bSplitCount));
                }
                for (int j = 0; j < tSplitCount; j++)
                {
                    color1 = Color.Lerp(color1, MeshProColor.tColors[j + 1], (2 * curIndex - length / tSplitCount * j) / (length / tSplitCount));
                    color2 = Color.Lerp(color2, MeshProColor.tColors[j + 1], (2 * curIndex - length / tSplitCount * j + 1) / (length / tSplitCount));
                }
                //m_textInfo.meshInfo[materialIndex].colors32[0 + index_X4] = Color.Lerp(Color.Lerp(MeshProColor.BLColor, MeshProColor.BCColor, (2 * i) / (length/2)), MeshProColor.BRColor, (2 * i- length/2) / (length / 2));
                //m_textInfo.meshInfo[materialIndex].colors32[1 + index_X4] = Color.Lerp(Color.Lerp(MeshProColor.TLColor, MeshProColor.TCColor, (2 * i + 1) / (length / 2)), MeshProColor.TRColor, (2 * i + 1- length/2) / (length / 2));
                //m_textInfo.meshInfo[materialIndex].colors32[2 + index_X4] = Color.Lerp(Color.Lerp(MeshProColor.TLColor, MeshProColor.TCColor, (2 * i) / (length / 2)), MeshProColor.TRColor, (2 * i- length/2) / (length / 2));
                //m_textInfo.meshInfo[materialIndex].colors32[3 + index_X4] = Color.Lerp(Color.Lerp(MeshProColor.BLColor, MeshProColor.BCColor, (2 * i + 1) / (length / 2)), MeshProColor.BRColor, (2 * i + 1- length/2) / (length / 2));
                m_textInfo.meshInfo[materialIndex].colors32[0 + index_X4] = color0;
                m_textInfo.meshInfo[materialIndex].colors32[1 + index_X4] = color1;
                m_textInfo.meshInfo[materialIndex].colors32[2 + index_X4] = color2;
                m_textInfo.meshInfo[materialIndex].colors32[3 + index_X4] = color3;
            }
            else
            {
                m_textInfo.meshInfo[materialIndex].colors32[0 + index_X4] = characterInfoArray[i].color;
                m_textInfo.meshInfo[materialIndex].colors32[1 + index_X4] = characterInfoArray[i].color;
                m_textInfo.meshInfo[materialIndex].colors32[2 + index_X4] = characterInfoArray[i].color;
                m_textInfo.meshInfo[materialIndex].colors32[3 + index_X4] = characterInfoArray[i].color;
            }
   

            m_textInfo.meshInfo[materialIndex].vertexCount = index_X4 + 4;
        }
    }

    public void UpdateAll()
    {
        m_havePropertiesChanged = true;
        SetVerticesDirty();
    }

}
