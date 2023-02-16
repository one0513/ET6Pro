using System;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

//用来做行军线，类似与Tile之后uv在不断移动的效果。统一调用SetLoopSizeDelta来执行
//有2种实现方式：
//第一种：使用Image的Tile，复制若干网格之后，再移动uv；改变长度的时候可以自动适配，缺点是如果tile数量太多的话导致网格过多。（不推荐使用）
//第二种：使用RawIamge的UVRect，放大x轴的uv之后，再移动uv。不存在网格过多的问题。（推荐）【移动uv可以在uvRect放大的基础上直接用shader实现，不需要重构ugui网格的方式】
public class CTextureUV : CTexture {

    [SerializeField]
    protected bool _isMove = true;
    [SerializeField]
    protected bool _isUseTexSize = true;
    [SerializeField]
    protected Vector2 _moveSpeed = Vector2.zero;
    [SerializeField]
    protected Vector2 _tileSize = new Vector2(100, 100);
    [SerializeField]
    protected int _updateFps = 1;
    [SerializeField]
    protected bool _isLoopScaleMode = true;

    private Rect _uvRect = new Rect(0, 0, 1, 1);
    private Vector2 _uvSpeed = Vector2.zero;
    private Vector2 _lastLoopScale = Vector2.zero;

    public bool IsMove {
        get { return _isMove; }
        set {
            _isMove = value;
        }
    }

    public bool IsUseTexSize {
        get { return _isUseTexSize; }
        set {
            _isUseTexSize = value;
        }
    }

    public bool IsLoopScaleMode {
        get { return _isLoopScaleMode; }
        set {
            _isLoopScaleMode = value;
        }
    }

    public Vector2 MoveSpeed {
        get { return _moveSpeed; }
        set {
            _moveSpeed = value;
        }
    }

    public Vector2 TileSize {
        get { return _tileSize; }
        set {
            _tileSize = value;
        }
    }

    public int UpdateFps {
        get{return _updateFps;}
        set {
            _updateFps = Mathf.Max(1, value);
        }
    }

    public void SetTexSizeAsTileSize() {
        if (mainTexture != null) {
            _tileSize = new Vector2(mainTexture.width, mainTexture.height);
            if (_isLoopScaleMode) {
                this.rectTransform.sizeDelta = _tileSize;
                if (_lastLoopScale != Vector2.zero) {
                    SetLoopSizeDelta(_lastLoopScale);
                }
            }
        }
    }

    public void SetLoopSizeDelta(Vector2 size) {
        if (_isLoopScaleMode) {
            float loopScaleX = size.x / _tileSize.x;
            float loopScaleY = size.y / _tileSize.y;
            this.transform.localScale = new Vector3(loopScaleX, loopScaleY, 1);
            _uvRect.width = loopScaleX;
            _uvRect.height = loopScaleY;
            uvRect = _uvRect;
            _lastLoopScale = size;
        } else {
            this.rectTransform.sizeDelta = size;
        }
    }

    //直接由shader去实现这个效果，这个组件保留uvRect的长宽设置即可
    //private void Update() {
    //    if (_isMove) {
    //        if (Time.frameCount % _updateFps == 0) {
    //            _uvSpeed.x += Time.deltaTime * _moveSpeed.x;
    //            _uvSpeed.y += Time.deltaTime * _moveSpeed.y;
    //            _uvRect.x = _uvSpeed.x % 1.0f;
    //            _uvRect.y = _uvSpeed.y % 1.0f;
    //            _uvRect.width = uvRect.width;
    //            _uvRect.height = uvRect.height;
    //            uvRect = _uvRect;
    //        }
    //    }
    //}

    protected override void LoadComplete(ItemVo vo) {
        base.LoadComplete(vo);
        if (_isUseTexSize) {
            SetTexSizeAsTileSize();
        }
    }

    protected override void OnPopulateMesh(VertexHelper toFill) {
        if (texture != null) {
            //防止失败，第一种方案直接干掉
            //if (_isLoopScaleMode) {
            //    base.OnPopulateMesh(toFill);
            //} else {
            //    GenerateTiledTexture(toFill);
            //}
            base.OnPopulateMesh(toFill);
        } else {
            toFill.Clear();
        }
    }

    void GenerateTiledTexture(VertexHelper toFill) {
        Rect rect = GetPixelAdjustedRect();
        float tileWidth = (_tileSize.x) / 1;
        float tileHeight = (_tileSize.y) / 1;

        var uvMin = Vector2.zero;
        var uvMax = Vector2.one;

        var v = UIVertex.simpleVert;
        v.color = color;

        // Min to max max range for tiled region in coordinates relative to lower left corner.
        float xMin = 0;
        float xMax = rect.width;
        float yMin = 0;
        float yMax = rect.height;

        toFill.Clear();
        var clipped = uvMax;

        // if either with is zero we cant tile so just assume it was the full width.
        if (tileWidth == 0)
            tileWidth = xMax - xMin;

        if (tileHeight == 0)
            tileHeight = yMax - yMin;

        for (float y1 = yMin; y1 < yMax; y1 += tileHeight) {
            Vector2 maxUv = new Vector2(uvRect.xMax, uvRect.yMax);
            float y2 = y1 + tileHeight;
            if (y2 > yMax) {
                clipped.y = uvMin.y + (uvMax.y - uvMin.y) * (yMax - y1) / (y2 - y1);
                maxUv.y = uvRect.yMin + clipped.y;
                y2 = yMax;
            }
            clipped.x = uvMax.x;
            for (float x1 = xMin; x1 < xMax; x1 += tileWidth) {
                float x2 = x1 + tileWidth;
                if (x2 > xMax) {
                    clipped.x = uvMin.x + (uvMax.x - uvMin.x) * (xMax - x1) / (x2 - x1);
                    maxUv.x = uvRect.xMin + clipped.x;
                    x2 = xMax;
                }
                AddQuad(toFill, new Vector2(x1, y1) + rect.position, new Vector2(x2, y2) + rect.position, color, new Vector2(uvRect.xMin, uvRect.yMin), maxUv);
            }
        }
    }
}
