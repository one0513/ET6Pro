using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CSpriteTile : Image {
    public int MAX_TILE_X = 54;
    public int MAX_TILE_Z = 54;

    public int TILE_SIZE = 128;

    public int defaultIndex = 0;

    private Mesh mesh;
    private int MAX_X = 100;
    private Dictionary<int,int> indexDict = new Dictionary<int,int>();
    private bool isNeedUpdate = false;

    public Vector3 UpdataSpriteTile(Vector3 tile, int selIndex)
    {
        indexDict[(int)tile.x * MAX_X + (int)tile.z] = selIndex;
        isNeedUpdate = true;
        return tile;
    }

    [ContextMenu("测试")]
    public void UpdataSpriteTile2() {
        UpdateTile(1,1, 2);
        UpdateGeometry();
        
    }

    public void OnInitSpriteTile()
    {
        mesh = mesh != null ? mesh : CreateMesh();
    }


    // Use this for initialization
    protected override void OnPopulateMesh(VertexHelper toFill) {
        GenerateSimpleSprite(toFill, preserveAspect);
    }

    /// <summary>
    /// Generate vertices for a simple Image.
    /// </summary>
    void GenerateSimpleSprite(VertexHelper vh, bool lPreserveAspect) {
        var uv = (overrideSprite != null) ? UnityEngine.Sprites.DataUtility.GetOuterUV(overrideSprite) : Vector4.zero;
        var color32 = color;
        vh.Clear();

        mesh = mesh != null ? mesh : CreateMesh();

        var vertices = mesh.vertices;
        var uvs = mesh.uv;
        for (int i = 0; i < MAX_TILE_X; i++) {
            for (int j = 0; j < MAX_TILE_Z; j++) {
                int index = i * MAX_TILE_X + j;
                int index2 = index * 4 + 0;
                vh.AddVert(vertices[index2], color32, uvs[index2]);
                index2 += 1;
                vh.AddVert(vertices[index2], color32, uvs[index2]);
                index2 += 1;
                vh.AddVert(vertices[index2], color32, uvs[index2]);
                index2 += 1;
                vh.AddVert(vertices[index2], color32, uvs[index2]);
                vh.AddTriangle(index * 4 + 0, index * 4 + 1, index * 4 + 2);
                vh.AddTriangle(index * 4 + 2, index * 4 + 3, index * 4 + 0);
            }
        }
    }

    public Mesh CreateMesh() {

        Mesh mesh = new Mesh();
        try {
            Vector3[] vertices = new Vector3[4 * MAX_TILE_X * MAX_TILE_Z];
            Vector2[] uvs = new Vector2[4 * MAX_TILE_X * MAX_TILE_Z];
            int[] triangles = new int[6 * MAX_TILE_X * MAX_TILE_Z];
            for (int i = 0; i < MAX_TILE_X; i++) {
                for (int j = 0; j < MAX_TILE_Z; j++) {
                    int index = i * MAX_TILE_X + j;
                    Vector2 tileUV = new Vector2();
                    tileUV.x = 2;
                    tileUV.y = 1;

                    var tilePos = new Vector3(i * 0.5f + j * 0.5f, -j * 0.25f + i * 0.25f);

                    
                    vertices[index * 4 + 0] = (tilePos + new Vector3(0, -0.25f)) * TILE_SIZE + new Vector3((-rectTransform.sizeDelta.x + TILE_SIZE) / 2 , 0);
                    vertices[index * 4 + 1] = (tilePos + new Vector3(-0.5f, 0)) * TILE_SIZE + new Vector3((-rectTransform.sizeDelta.x + TILE_SIZE) / 2, 0);
                    vertices[index * 4 + 2] = (tilePos + new Vector3(0, 0.25f)) * TILE_SIZE + new Vector3((-rectTransform.sizeDelta.x + TILE_SIZE) / 2, 0);
                    vertices[index * 4 + 3] = (tilePos + new Vector3(0.5f, 0)) * TILE_SIZE + new Vector3((-rectTransform.sizeDelta.x + TILE_SIZE) / 2, 0);

                    uvs[index * 4 + 0] = new Vector2(0.5f * 10 + defaultIndex * 100, 0);
                    uvs[index * 4 + 1] = new Vector2(0 * 10 + defaultIndex * 100, 0.5f);
                    uvs[index * 4 + 2] = new Vector2(0.5f * 10 + defaultIndex * 100, 1);
                    uvs[index * 4 + 3] = new Vector2(1 * 10 + defaultIndex * 100, 0.5f);

                    triangles[index * 6 + 0] = index * 4 + 0;
                    triangles[index * 6 + 1] = index * 4 + 1;
                    triangles[index * 6 + 2] = index * 4 + 2;
                    triangles[index * 6 + 3] = index * 4 + 0;
                    triangles[index * 6 + 4] = index * 4 + 2;
                    triangles[index * 6 + 5] = index * 4 + 3;
                }
            }

            mesh.vertices = vertices;
            mesh.uv = uvs;
            mesh.triangles = triangles;
        } catch (Exception e) {
            Debug.Log((e.Message));
        }

        return mesh;
    }



    private void UpdateTile(int tileX,int tileZ, int selIndex) {
        if (tileX >= 0 && tileZ >= 0) {
            int index = tileX * MAX_TILE_X + tileZ;
            var uvs = mesh.uv;
            uvs[index * 4 + 0] = new Vector2(0.5f * 10 + selIndex * 100, 0);
            uvs[index * 4 + 1] = new Vector2(0 * 10 + selIndex * 100, 0.5f);
            uvs[index * 4 + 2] = new Vector2(0.5f * 10 + selIndex * 100, 1);
            uvs[index * 4 + 3] = new Vector2(1 * 10 + selIndex * 100, 0.5f);
            mesh.uv = uvs;
        }
    }

    public void ResetSizeDelta() {
        rectTransform.sizeDelta = new Vector2(MAX_TILE_X * TILE_SIZE, MAX_TILE_Z * (TILE_SIZE / 2));
    }

    public RectTransform _maskTrans;
    public RectTransform customMaskTrans;//支持自定义mask矩形
    private bool _maskChanged = false;
    private Vector3 _lossScale;
    private Vector3 _pos;
    private Rect _rect;
    public bool isDestroy = false;
    private void OnEnable() {
        base.OnEnable();
        FindParentMask();
    }

    protected override void OnDestroy() {
        base.OnDestroy();
        isDestroy = true;
    }


    void CullMask() {
        if (_maskTrans != null) {
            _lossScale = _maskTrans.lossyScale;
            _pos = _maskTrans.position;
            _rect = _maskTrans.rect;

            Vector3[] corners = new Vector3[4];
            _maskTrans.GetWorldCorners(corners);
            DoCull(corners[0].x, corners[0].y, corners[2].x, corners[2].y);
        } else {
            CancelCull();
        }
        _maskChanged = false;
    }
    private void CancelCull() {
        material.SetFloat("_MinX", -10);
        material.SetFloat("_MaxX", 10);
        material.SetFloat("_MinY", -10);
        material.SetFloat("_MaxY", 10);
    }
    private void DoCull(float minX, float minY, float maxX, float maxY) {
        material.SetFloat("_MinX", minX);
        material.SetFloat("_MaxX", maxX);
        material.SetFloat("_MinY", minY);
        material.SetFloat("_MaxY", maxY);
    }
    private void OnDisable() {
        base.OnDisable();
        _maskTrans = null;
        _maskChanged = false;
        _lossScale = Vector3.zero;
        _pos = Vector3.zero;
        _rect = Rect.zero;
    }
    protected override void OnTransformParentChanged() {
        base.OnTransformParentChanged();
        FindParentMask();
    }


    public void FindParentMask() {
        if (customMaskTrans != null && _maskTrans != customMaskTrans) {
            _maskTrans = customMaskTrans;
            _maskChanged = true;
            return;
        }
        var mask2D = this.GetComponentInParent<RectMask2D>();
        if (mask2D == null) {
            if (_maskTrans != null) {
                _maskTrans = null;
                _maskChanged = true;
            } else {
                _maskChanged = true;
            }
        } else if (_maskTrans != mask2D.rectTransform) {
            _maskTrans = mask2D.rectTransform;
            _maskChanged = true;
        }
    }
    private void Update() {
        //同步裁剪区域

        if (isNeedUpdate)
        {
            foreach (var index in indexDict.Keys)
            {
                UpdateTile(index / MAX_X, index % MAX_X, indexDict[index]);
            }
            UpdateGeometry();
            indexDict.Clear();
            isNeedUpdate = false;
        }

        if (_maskChanged == false && _maskTrans != null) {
            _maskChanged = (_lossScale != _maskTrans.lossyScale || _pos != _maskTrans.position || _rect != _maskTrans.rect);
        }

        if (_maskChanged) {
            CullMask();
        }
    }

}
