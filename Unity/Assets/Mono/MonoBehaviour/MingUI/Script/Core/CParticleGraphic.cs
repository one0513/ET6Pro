using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 仅支持UI遮罩的粒子特效类
/// </summary>
[ExecuteInEditMode]
[RequireComponent(typeof(CanvasRenderer))]
[RequireComponent(typeof(ParticleSystem))]
public class CParticleGraphic : MaskableGraphic
{
    private static Material _commonMaterial;
    public static Material CommonMaterial
    {
        get
        {
            if (_commonMaterial == null)
            {
                _commonMaterial = new Material(Shader.Find("UI/Particles/Hidden"));
                _commonMaterial.hideFlags = HideFlags.DontSave | HideFlags.NotEditable;
            }
            return _commonMaterial;
        }
    }
    public Texture particleTexture;
    public Sprite particleSprite;

    private Transform _transform;
    private ParticleSystem _particleSystem;
    private ParticleSystem.Particle[] _particles;
    private UIVertex[] _quad = new UIVertex[4];
    private Vector4 _uv = Vector4.zero;
    private ParticleSystem.TextureSheetAnimationModule _textureSheetAnimation;
    private int _textureSheetAnimationFrames;
    private Vector2 _textureSheedAnimationFrameSize;

    /// <summary>
    /// 宽度
    /// </summary>
    public float Width
    {
        get { return Size.x; }
        set { Size = new Vector2(value, Size.y); }
    }

    /// <summary>
    /// 高度
    /// </summary>
    public float Height
    {
        get { return Size.y; }
        set { Size = new Vector2(Size.x, value); }
    }

    /// <summary>
    /// 大小
    /// </summary>
    public Vector2 Size
    {
        get { return rectTransform.rect.size; }
        set
        {
            if (value != rectTransform.rect.size)
            {
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, value.x);
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, value.y);
            }
        }
    }

    public override Texture mainTexture
    {
        get
        {
            if (particleTexture)
            {
                return particleTexture;
            }

            if (particleSprite)
            {
                return particleSprite.texture;
            }

            return null;
        }
    }

    protected bool Initialize()
    {
        // initialize members
        if (_transform == null)
        {
            _transform = transform;
        }

        // prepare particle system
        ParticleSystemRenderer pRenderer = GetComponent<ParticleSystemRenderer>();
        bool setParticleSystemMaterial = false;

        if (_particleSystem == null)
        {
            _particleSystem = GetComponent<ParticleSystem>();

            if (_particleSystem == null)
            {
                return false;
            }
            if (Application.isPlaying)
            {
                ParticleSystem.MainModule main = _particleSystem.main;
                if (main.startSpeedMultiplier > 0)
                {
                    main.startSpeedMultiplier /= UIRoot.StandardScale;
                }
                if (main.startSizeMultiplier > 0)
                {
                    main.startSizeMultiplier /= UIRoot.StandardScale;
                }
                if (main.startRotationMultiplier > 0)
                {
                    main.startRotationMultiplier /= UIRoot.StandardScale;
                }
            }
            // get current particle texture
            if (pRenderer == null)
            {
                pRenderer = _particleSystem.gameObject.AddComponent<ParticleSystemRenderer>();
            }
            Material currentMaterial = pRenderer.sharedMaterial;//一定访问sharedMaterial，不然drawcall倍增
            if (currentMaterial)
            {
                string shaderName = currentMaterial.shader.name;
                if (shaderName.StartsWith("UI"))
                {
                    material = currentMaterial; //将材质传递给自己，不然很多属性不生效
                }
                else if (shaderName.StartsWith("Particles"))//这些是特殊的特效shader
                {
                    currentMaterial.shader = Shader.Find("UI/" + shaderName);//替换成UI的粒子shader
                    material = currentMaterial; //将材质传递给自己，不然很多属性不生效
                }
                else
                {
                    material = null;//使用默认的材质
                }
                if (currentMaterial.HasProperty("_MainTex"))
                {
                    particleTexture = currentMaterial.mainTexture;
                }
            }
            // automatically set scaling
            //_particleSystem.scalingMode = ParticleSystemScalingMode.Local;

            _particles = null;
            setParticleSystemMaterial = true;
        }
        else
        {
            if (Application.isPlaying)
            {
                setParticleSystemMaterial = (pRenderer.material == null);
            }
            else if (MingUIAgent.IsEditorMode)
            {
                setParticleSystemMaterial = (pRenderer.sharedMaterial == null);
            }
        }

        // automatically set material to UI/Particles/Hidden shader, and get previous texture
        if (setParticleSystemMaterial)
        {
            pRenderer.sharedMaterial = CommonMaterial;
        }

        // prepare particles array
        if (_particles == null)
        {
            _particles = new ParticleSystem.Particle[_particleSystem.main.maxParticles];
        }

        // prepare uvs
        if (particleTexture)
        {
            _uv = new Vector4(0, 0, 1, 1);
        }
        else if (particleSprite)
        {
            _uv = UnityEngine.Sprites.DataUtility.GetOuterUV(particleSprite);
        }

        // prepare texture sheet animation
        _textureSheetAnimation = _particleSystem.textureSheetAnimation;
        _textureSheetAnimationFrames = 0;
        _textureSheedAnimationFrameSize = Vector2.zero;
        if (_textureSheetAnimation.enabled)
        {
            _textureSheetAnimationFrames = _textureSheetAnimation.numTilesX * _textureSheetAnimation.numTilesY;
            _textureSheedAnimationFrameSize = new Vector2(1f / _textureSheetAnimation.numTilesX, 1f / _textureSheetAnimation.numTilesY);
        }

        return true;
    }

    protected override void Awake()
    {
        base.Awake();
        if (!Initialize())
        {
            enabled = false;
        }
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        if (MingUIAgent.IsEditorMode && !Application.isPlaying)
        {
            if (!Initialize())
            {
                return;
            }
        }

        // prepare vertices
        vh.Clear();

        if (!gameObject.activeInHierarchy)
        {
            return;
        }

        // iterate through current particles
        int count = _particleSystem.GetParticles(_particles);

        for (int i = 0; i < count; ++i)
        {
            ParticleSystem.Particle particle = _particles[i];

            // get particle properties
            Vector2 position = (_particleSystem.main.simulationSpace == ParticleSystemSimulationSpace.Local ? particle.position : _transform.InverseTransformPoint(particle.position));
            float rotation = -particle.rotation * Mathf.Deg2Rad;
            float rotation90 = rotation + Mathf.PI / 2;
            Color32 pColor = particle.GetCurrentColor(_particleSystem);
            float size = particle.GetCurrentSize(_particleSystem) * 0.5f;//整体缩放

            // apply scale
            if (_particleSystem.main.scalingMode == ParticleSystemScalingMode.Shape)
            {
                position /= canvas.scaleFactor;
            }

            // apply texture sheet animation
            Vector4 particleUv = _uv;
            if (_textureSheetAnimation.enabled)
            {
                float frameProgress = 1 - (particle.remainingLifetime / particle.startLifetime);
                frameProgress = Mathf.Repeat(frameProgress * _textureSheetAnimation.cycleCount, 1);
                int frame = 0;

                switch (_textureSheetAnimation.animation)
                {
                    case ParticleSystemAnimationType.WholeSheet:
                        frame = Mathf.FloorToInt(frameProgress * _textureSheetAnimationFrames);
                        break;

                    case ParticleSystemAnimationType.SingleRow:
                        frame = Mathf.FloorToInt(frameProgress * _textureSheetAnimation.numTilesX);
                        int row = _textureSheetAnimation.rowIndex;
                        frame += row * _textureSheetAnimation.numTilesX;
                        break;
                }

                frame %= _textureSheetAnimationFrames;

                particleUv.x = (frame % _textureSheetAnimation.numTilesX) * _textureSheedAnimationFrameSize.x;
                particleUv.y = Mathf.FloorToInt((float)frame / _textureSheetAnimation.numTilesX) * _textureSheedAnimationFrameSize.y;
                particleUv.z = particleUv.x + _textureSheedAnimationFrameSize.x;
                particleUv.w = particleUv.y + _textureSheedAnimationFrameSize.y;
            }

            _quad[0] = UIVertex.simpleVert;
            _quad[0].color = pColor;
            _quad[0].uv0 = new Vector4(particleUv.x, particleUv.y);

            _quad[1] = UIVertex.simpleVert;
            _quad[1].color = pColor;
            _quad[1].uv0 = new Vector4(particleUv.x, particleUv.w);

            _quad[2] = UIVertex.simpleVert;
            _quad[2].color = pColor;
            _quad[2].uv0 = new Vector4(particleUv.z, particleUv.w);

            _quad[3] = UIVertex.simpleVert;
            _quad[3].color = pColor;
            _quad[3].uv0 = new Vector4(particleUv.z, particleUv.y);

            if (Math.Abs(rotation) < 0.01f)
            {
                // no rotation
                Vector2 corner1 = new Vector2(position.x - size, position.y - size);
                Vector2 corner2 = new Vector2(position.x + size, position.y + size);

                _quad[0].position = new Vector2(corner1.x, corner1.y);
                _quad[1].position = new Vector2(corner1.x, corner2.y);
                _quad[2].position = new Vector2(corner2.x, corner2.y);
                _quad[3].position = new Vector2(corner2.x, corner1.y);
            }
            else
            {
                // apply rotation
                Vector2 right = new Vector2(Mathf.Cos(rotation), Mathf.Sin(rotation)) * size;
                Vector2 up = new Vector2(Mathf.Cos(rotation90), Mathf.Sin(rotation90)) * size;

                _quad[0].position = position - right - up;
                _quad[1].position = position - right + up;
                _quad[2].position = position + right + up;
                _quad[3].position = position + right - up;
            }

            vh.AddUIVertexQuad(_quad);
        }
    }

    void Update()
    {
        if (Application.isPlaying)
        {
            // unscaled animation within UI
            _particleSystem.Simulate(Time.unscaledDeltaTime, false, false);

            SetAllDirty();
        }
    }


    void LateUpdate()
    {
        if (MingUIAgent.IsEditorMode && !Application.isPlaying)
        {
            SetAllDirty();
        }
    }

    public void ResetParticleSystem()
    {
        //重置粒子系统
        if (_particleSystem != null)
        {
            var main = _particleSystem.main;
            if (_particleSystem.main.startSpeedMultiplier > 0)
            {
                main.startSpeedMultiplier *= UIRoot.StandardScale;
            }
            if (main.startSizeMultiplier> 0)
            {
                main.startSizeMultiplier *= UIRoot.StandardScale;
            }
            if (main.startRotationZMultiplier > 0)
            {
                main.startRotationZMultiplier *= main.startRotationZMultiplier * UIRoot.StandardScale;
            }
        }
        //重置材质球
        if (material != null)
        {
            // prepare particle system
            ParticleSystemRenderer pRenderer = GetComponent<ParticleSystemRenderer>();
            pRenderer.sharedMaterial = material;
        }
       
    }

}
