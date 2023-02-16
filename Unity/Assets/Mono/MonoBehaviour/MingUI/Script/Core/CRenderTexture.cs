using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 用于渲染3D物体（模型、特效等）的UI贴图类(方案抛弃不用)
/// </summary>
public class CRenderTexture : RawImage
{
    public const int SPEED = 1;
    public bool interactable; //能否响应事件？
    public GameObject target; //照的目标

    private Camera _renderCamera;

    protected override void Start()
    {
        base.Start();
        UIEventListener.AddDrag(gameObject, OnDrag);
    }

    private void OnDrag(PointerEventData data)
    {
        target.transform.localRotation *= Quaternion.Euler(0f, -0.5f * data.delta.x * SPEED, 0f);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        if (Application.isPlaying)
        {
            int width = 512;
            int height = 256;
            this.target = GameObject.Find("Pre_512001");
            this.rectTransform.sizeDelta = new Vector2(width, height);
            RenderTexture renderTexture = CreateRenderTexture(width, height);


            this._renderCamera = CreateRenderCamera();
            this._renderCamera.gameObject.SetActive(true);
            this._renderCamera.targetTexture = renderTexture;
            //this._renderCamera.ResetAspect();
            //this._renderCamera.aspect = (float)width / (float)height;
            this._renderCamera.orthographicSize = 1;

            this.target.transform.SetParent(this._renderCamera.transform);
            this.target.transform.localPosition = new Vector3(0, -1f, 0);
            this.target.transform.localScale = Vector3.one;

            base.texture = this._renderCamera.targetTexture;
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        Dispose();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        _renderCamera = null;
        target = null;
    }

    private void Dispose()
    {
        if (base.texture != null)
        {
            if (MingUIAgent.IsEditorMode)
            {
                DestroyImmediate(texture);
            }
            else
            {
                Destroy(texture);
            }
        }
        if (_renderCamera != null)
        {
            _renderCamera.gameObject.SetActive(false);
            cameraPool.Add(_renderCamera);
        }
    }

    public static RenderTexture CreateRenderTexture(int width, int height)
    {
        RenderTexture texture = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
        texture.antiAliasing = 1;
        texture.filterMode = FilterMode.Bilinear;
        texture.anisoLevel = 0;
        texture.useMipMap = false;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.hideFlags = HideFlags.None;
        return texture;
    }

    /// <summary>
    /// /////////////////////////////////////////////////////////////////////////////////////////////////////
    /// </summary>
    private static List<Camera> cameraPool = new List<Camera>();

    public static Camera CreateRenderCamera()
    {
        if (cameraPool.Count > 0)
        {
            return cameraPool[0];
        }
        else
        {
            GameObject go = new GameObject("RenderCamera");
            Camera camera = go.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0, 0, 0, 0);
            //camera.cullingMask = 1 << 2;
            camera.orthographic = true;
            camera.orthographicSize = 1;
            camera.farClipPlane = 10;
            camera.nearClipPlane = -10;
            camera.depth = 1;
            return camera;
        }
    }
}