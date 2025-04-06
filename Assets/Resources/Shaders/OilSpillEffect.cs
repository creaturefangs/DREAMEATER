using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(OilspillEffectRenderer), PostProcessEvent.AfterStack, "Custom/OilspillEffect")]
public class OilspillEffect : PostProcessEffectSettings
{
    [Range(0f, 1f)]
    public FloatParameter distortionStrength = new FloatParameter { value = 0.15f };

    [Range(0f, 5f)]
    public FloatParameter distortionSpeed = new FloatParameter { value = 1f };

    public TextureParameter distortionTexture = new TextureParameter { value = null };

    [Tooltip("Tiling scale of the distortion texture.")]
    public Vector2Parameter distortionTiling = new Vector2Parameter { value = new Vector2(1f, 1f) };

    [Range(0f, 1f)]
    public FloatParameter distortionOpacity = new FloatParameter { value = 1f };

    public ColorParameter tintColor = new ColorParameter { value = Color.white };

    [Range(0f, 10f)]
    public FloatParameter hueShiftSpeed = new FloatParameter { value = 2f };

    [Range(0f, 1f)]
    public FloatParameter effectOpacity = new FloatParameter { value = 1f };

    [Tooltip("Enable or disable RGB split chromatic aberration.")]
    public BoolParameter enableChromaticAberration = new BoolParameter { value = true };
}

public sealed class OilspillEffectRenderer : PostProcessEffectRenderer<OilspillEffect>
{
    private Shader shader;
    private float timeX = 0f;

    public override void Init()
    {
        shader = Shader.Find("Hidden/MysticalOilSpill");
    }

    public override void Render(PostProcessRenderContext context)
    {
        if (shader == null)
        {
            Debug.LogError("Oilspill shader not found!");
            return;
        }

        Material mat = new Material(shader);
        timeX += Time.deltaTime;

        mat.SetFloat("_TimeX", timeX);
        mat.SetFloat("_DistortionStrength", settings.distortionStrength);
        mat.SetFloat("_DistortionSpeed", settings.distortionSpeed);
        mat.SetTexture("_DistortionTex", settings.distortionTexture);
        mat.SetVector("_DistortionTiling", settings.distortionTiling);
        mat.SetFloat("_DistortionOpacity", settings.distortionOpacity);
        mat.SetColor("_TintColor", settings.tintColor);
        mat.SetFloat("_HueShiftSpeed", settings.hueShiftSpeed);
        mat.SetFloat("_EffectOpacity", settings.effectOpacity);
        mat.SetFloat("_EnableChromaticAberration", settings.enableChromaticAberration ? 1f : 0f);

        CommandBuffer cmd = context.command;
        cmd.BeginSample("OilspillEffect");
        cmd.Blit(context.source, context.destination, mat);
        cmd.EndSample("OilspillEffect");
    }
}
