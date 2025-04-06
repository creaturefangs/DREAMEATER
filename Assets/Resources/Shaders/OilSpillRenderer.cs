using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public sealed class OilspillRenderer : PostProcessEffectRenderer<MysticalOilspill>
{
    private Shader _shader;
    private int _timeID;

    public override void Init()
    {
        _shader = Shader.Find("Hidden/PostProcessing/MysticalOilspill");
        _timeID = Shader.PropertyToID("_Time");
    }

    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(_shader);
        sheet.properties.SetTexture("_DistortionTex", settings.distortionTex);
        sheet.properties.SetFloat("_DistortionStrength", settings.distortionStrength);
        sheet.properties.SetFloat("_HueShiftSpeed", settings.hueShiftSpeed);
        sheet.properties.SetFloat("_OverlayStrength", settings.overlayStrength);
        sheet.properties.SetVector("_ScrollSpeed", settings.scrollSpeed);
        sheet.properties.SetFloat("_Time", Time.time);

        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}

