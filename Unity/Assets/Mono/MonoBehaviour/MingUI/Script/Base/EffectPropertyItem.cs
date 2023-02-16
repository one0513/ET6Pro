using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 1. 区分特效，一类是基于粒子系统的，另外一类是非粒子特效的
/// 2. 根据特效分类使用不同的shader，
///         粒子相关的合批方案是顶点数据流加上MaterialPropertyBlock，需要考虑粒子自带的排序
///         非粒子相关的则直接使用GPU Instancing，无法精准控制排序
/// </summary>
public class EffectPropertyItem : MonoBehaviour {

    private const string PARTICLE_SORT_HASH_STR = "PARTICLE_SORT_HASH_STR";
    private const string NONE_PARTICLE_SORT_HASH_STR = "NONE_PARTICLE_SORT_HASH_STR";

    public const string SHADER_ADDTIVE = "Particles/Additive";
    public const string SHADER_ALPHA_BLEND = "Particles/Alpha Blended";
    public const string SHADER_ADDTIVE_ORIGIN = "Particles/Additive/Origin";
    public const string SHADER_ALPHA_BLEND_ORIGIN = "Particles/Alpha Blended/Origin";
    public const string SHADER_ADD_CENTER_GLOW = "ErbGameArt/LWRP/Particles/Add_CenterGlow";
    public const string SHADER_BLEND_CENTER_GLOW = "ErbGameArt/LWRP/Particles/Blend_CenterGlow";
    public const string SHADER_ADD_TRAIL = "ErbGameArt/LWRP/Particles/Add_Trail";

    private static Dictionary<string, bool> BATCH_SHADER_DIC = new Dictionary<string, bool>() {
        {SHADER_ADDTIVE,true},
        {SHADER_ALPHA_BLEND,true},
        {SHADER_ADDTIVE_ORIGIN,true},
        {SHADER_ALPHA_BLEND_ORIGIN,true},
        {SHADER_ADD_CENTER_GLOW,true},
        {SHADER_BLEND_CENTER_GLOW,true},
        {SHADER_ADD_TRAIL,true},
    };

    private static Dictionary<string, bool> SORT_SHADER_DIC = new Dictionary<string, bool>() {
        {SHADER_ADDTIVE,true},
        {SHADER_ALPHA_BLEND,true},
        {SHADER_ADDTIVE_ORIGIN,true},
        {SHADER_ALPHA_BLEND_ORIGIN,true},
        {"Mobile/Particles/Additive",true},
        {"Mobile/Particles/Alpha Blended",true},
        {"Particles/Additive (Soft)",true},
        {"Particles/Anim Alpha Blended",true },
        {"Particles/Alpha Blended Premultiply", true}
    };

    protected static int colorConst = Shader.PropertyToID("_TintColor");
    protected static int cliptConst = Shader.PropertyToID("_PclipRect");
    protected static int uiConst = Shader.PropertyToID("_NoClip");

    protected static ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve(0f);
    protected static ParticleSystem.MinMaxGradient gradient = new ParticleSystem.MinMaxGradient(Color.white);

    protected static List<ParticleSystemVertexStream> pSteam;

    protected static int PARTICLE_SORT_HASH = PARTICLE_SORT_HASH_STR.GetHashCode();
    protected static int NONE_PARTICLE_SORT_HASH = NONE_PARTICLE_SORT_HASH_STR.GetHashCode();

    protected Vector4 m_tex_clip = new Vector4(1, 1, 0, 0);
    protected Color m_color = Color.white;

    protected MaterialPropertyBlock block;
    protected Renderer rener;

    protected ParticleSystem pSystem;
    protected ParticleSystemRenderer pSystemRen;
    protected ParticleSystem.CustomDataModule customDataModule;

    private string shaderName;
    private bool isColorInit = false;
    private bool useProperty = false;
    private bool useSorting = false;
    private bool isPropertyInit = false;

    public bool UseProperty{
        get { return useProperty; }
    }

    public void InitProperty() {
        if (!isPropertyInit) {
            isPropertyInit = true;
            rener = GetComponent<Renderer>();
            if (rener == null) return;
            if (rener.sharedMaterial == null) return;
            shaderName = rener.sharedMaterial.shader.name;
            useProperty = BATCH_SHADER_DIC.ContainsKey(shaderName);
            useSorting = SORT_SHADER_DIC.ContainsKey(shaderName);
            if (useProperty == false) return;
            InitBlock();
            InitCustomData();
        }
    }

    void Awake() {
        InitProperty();
    }

    protected void InitBlock() {
        if (block == null) {
            block = new MaterialPropertyBlock();
        }
        if(pSteam == null) {
            pSteam = new List<ParticleSystemVertexStream>();
            pSteam.Add(ParticleSystemVertexStream.Position);
            pSteam.Add(ParticleSystemVertexStream.Normal);
            pSteam.Add(ParticleSystemVertexStream.Color);
            pSteam.Add(ParticleSystemVertexStream.UV);
            pSteam.Add(ParticleSystemVertexStream.UV2);
            // pSteam.Add(ParticleSystemVertexStream.Custom1XYZW);
            // pSteam.Add(ParticleSystemVertexStream.Custom2XYZW);
        }
    }

    protected void InitCustomData() {
        if (pSystem == null) {
            pSystem = GetComponent<ParticleSystem>();
            if (pSystem != null) {
                pSystemRen = GetComponent<ParticleSystemRenderer>();
                customDataModule = pSystem.customData;
                if (pSystemRen.activeVertexStreamsCount < 7) {
                    pSystemRen.SetActiveVertexStreams(pSteam);
                    customDataModule.enabled = true;
                    customDataModule.SetMode(ParticleSystemCustomData.Custom1, ParticleSystemCustomDataMode.Vector);
                    customDataModule.SetMode(ParticleSystemCustomData.Custom2, ParticleSystemCustomDataMode.Color);
                    customDataModule.SetVectorComponentCount(ParticleSystemCustomData.Custom1, 4);
                    SetAlpha(1.0f);
                    SetUVTiling(Vector2.one);
                    SetUVOffset(Vector2.zero);
                }
            } else {
                //todo 这里后续可以尝试指定的GpuInstancing的方式去优化
                if (shaderName == SHADER_ADDTIVE) {
                    rener.sharedMaterial.shader = MingUIAgent.ShaderFind(SHADER_ADDTIVE_ORIGIN);
                }else if(shaderName == SHADER_ALPHA_BLEND) {
                    rener.sharedMaterial.shader = MingUIAgent.ShaderFind(SHADER_ALPHA_BLEND_ORIGIN);
                }
            }
        }
    }

    ////////////////////////////////  这里是材质球稳定相关内容  ////////////////////////////
    #region

    public void SetClipable(bool isClip) {
        if (!useProperty) return;
        if (rener != null) {
            block.SetFloat(uiConst, isClip ? 0 : 1);
            rener.SetPropertyBlock(block);
        }
    }

    public void SetClipRect(Vector4 clip) {
        if (!useProperty) return;
        if (rener != null) {
            m_tex_clip = clip;
            block.SetVector(cliptConst, m_tex_clip);
            rener.SetPropertyBlock(block);
        }
    }

    public void SetUVOffset(Vector2 off) {
        if (!useProperty) return;
        if (pSystem != null) {
            curve.constant = off.x;
            customDataModule.SetVector(ParticleSystemCustomData.Custom1, 2, curve);
            curve.constant = off.y;
            customDataModule.SetVector(ParticleSystemCustomData.Custom1, 3, curve);
        } else {
            if (rener != null) {
                rener.material.mainTextureOffset = off;
            }
        }
    }

    public void SetUVTiling(Vector2 tiling) {
        if (!useProperty) return;
        if (pSystem != null) {
            curve.constant = tiling.x;
            customDataModule.SetVector(ParticleSystemCustomData.Custom1, 0, curve);
            curve.constant = tiling.y;
            customDataModule.SetVector(ParticleSystemCustomData.Custom1, 1, curve);
        } else {
            if (rener != null) {
                rener.material.mainTextureScale = tiling;
            }
        }
    }

    public void SetAlpha(float alpha) {
        if (!useProperty) return;
        if (pSystem != null) {
            m_color.a = alpha;
            gradient.color = m_color;
            customDataModule.SetColor(ParticleSystemCustomData.Custom2, gradient);
        } else {
            if (rener != null && rener.material != null && rener.material.HasProperty(colorConst)) {
                Color col = rener.material.GetColor(colorConst);
                if (col != null) {
                    col.a = alpha;
                    rener.material.SetColor(colorConst, col);
                }
            }
        }
    }

    public void SetColor(Color color) {
        if (!useProperty) return;
        if (pSystem != null) {
            m_color = color;
            gradient.color = color;
            customDataModule.SetColor(ParticleSystemCustomData.Custom2, gradient);
        } else {
            if (rener != null && rener.material != null && rener.material.HasProperty(colorConst)) {
                rener.material.SetColor(colorConst, color);
            }
        }
    }

    public Color GetColor() {
        if (!useProperty) return Color.white;
        if (pSystem != null) {
            if (!isColorInit) {
                isColorInit = true;
                m_color = rener.sharedMaterial.GetColor(colorConst);
            }
            return m_color;
        } else {
            if (rener != null && rener.material != null && rener.material.HasProperty(colorConst)) {
                return rener.material.GetColor(colorConst);
            }
        }
        return Color.white;
    }
    #endregion

    ////////////////////////////////  这里是渲染排序相关内容  ////////////////////////////
    #region

    /// <summary>
    /// 获取唯一标识，如果HashCode的方式无效，后续修改成string拼接的方式
    /// </summary>
    /// <param name="render"></param>
    /// <returns></returns>
    private static int GetParticleSystemRenderStatusCode(ParticleSystemRenderer render) {
        int matCode = 0;
        //判断材质球是否相同
        matCode = render.sharedMaterial.GetHashCode();
        //判断网格是否相同（不同的话会中断batch）
        if (render.mesh != null) {
            matCode += render.mesh.GetHashCode();
        }
        return matCode + PARTICLE_SORT_HASH;
    }

    /// <summary>
    /// 获取唯一标识，如果HashCode的方式无效，后续修改成string拼接的方式
    /// </summary>
    /// <param name="render"></param>
    /// <returns></returns>
    private static int GetRenderStatusStr(Renderer render) {
        return render.sharedMaterial.GetHashCode() + NONE_PARTICLE_SORT_HASH;
    }

    public float customSortingFudge {
        get {
            if (pSystemRen != null) {
                return pSystemRen.sortingFudge;
            } else {
                return 0;
            }
        }
    }
    
    public int GetSortingCode() {
        //todo 注意区分混用code的情况
        if(pSystemRen != null) {
            return GetParticleSystemRenderStatusCode(pSystemRen);
        } else {
            return GetRenderStatusStr(rener);
        }
    }

    public void SetSortingFudge(float fudge) {
        if(pSystemRen != null) {
            pSystemRen.sortingFudge = fudge;
        }
    }

    public void SetSortingOrder(int order) {
        if (pSystemRen != null) {
            pSystemRen.sortingOrder = order;
        } else {
            if (rener != null) {
                rener.sortingOrder = order;
            }
        }
    }

    public bool CanSorting() {
        return useSorting && rener != null && rener.sharedMaterial != null;
    }

    public ParticleSystemRenderer GetParticleSysRenderer() {
        return pSystemRen;
    }

    #endregion
}
