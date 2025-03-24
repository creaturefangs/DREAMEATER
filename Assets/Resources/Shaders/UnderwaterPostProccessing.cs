using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(UnderwaterPostProcessingRenderer), PostProcessEvent.AfterStack, "Custom/UnderwaterEffect")]
public class UnderwaterPostProcessing : PostProcessEffectSettings
{
    [Range(0.001f, 0.5f)]
    public FloatParameter strength = new FloatParameter { value = 0.05f };

    [Range(0f, 5f)]
    public FloatParameter speed = new FloatParameter { value = 1f };

    public TextureParameter overlayTexture = new TextureParameter { value = null };

    [Range(0f, 2f)]
    public FloatParameter overlaySpeed = new FloatParameter { value = 0.2f };

    [Range(0f, 1f)]
    public FloatParameter overlayOpacity = new FloatParameter { value = 0.5f };

    public ColorParameter tintColor = new ColorParameter { value = new Color(0.2f, 0.4f, 0.8f, 1f) };

    // Bloom-like Glow (Custom)
    [Range(0f, 5f)]
    public FloatParameter bloomIntensity = new FloatParameter { value = 1f };

    [Range(0f, 1f)]
    public FloatParameter bloomThreshold = new FloatParameter { value = 0.5f };

    [Range(0f, 1f)]
    public FloatParameter bloomSoftness = new FloatParameter { value = 0.2f };
}

public class UnderwaterPostProcessingRenderer : PostProcessEffectRenderer<UnderwaterPostProcessing>
{
    private Shader shader;
    private float timeX = 0f;

    public override void Init()
    {
        shader = Shader.Find("Hidden/UnderwaterEffect");
    }

    public override void Render(PostProcessRenderContext context)
    {
        if (shader == null)
        {
            Debug.LogError("Underwater shader not found!");
            return;
        }

        Material mat = new Material(shader);
        timeX += Time.deltaTime;

        mat.SetFloat("_TimeX", timeX);
        mat.SetFloat("_Strength", settings.strength);
        mat.SetFloat("_Speed", settings.speed);
        mat.SetTexture("_OverlayTex", settings.overlayTexture);
        mat.SetFloat("_OverlaySpeed", settings.overlaySpeed);
        mat.SetFloat("_OverlayOpacity", settings.overlayOpacity);
        mat.SetColor("_TintColor", settings.tintColor);

        // Custom Bloom Parameters
        mat.SetFloat("_BloomIntensity", settings.bloomIntensity);
        mat.SetFloat("_BloomThreshold", settings.bloomThreshold);
        mat.SetFloat("_BloomSoftness", settings.bloomSoftness);

        // Blit with existing effects (preserves Unity post-processing stack)
        CommandBuffer cmd = context.command;
        cmd.BeginSample("UnderwaterEffect");
        cmd.Blit(context.source, context.destination, mat);
        cmd.EndSample("UnderwaterEffect");
    }
}