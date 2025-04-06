using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

[System.Serializable]
[PostProcess(typeof(OilspillRenderer), PostProcessEvent.AfterStack, "Custom/MysticalOilspill")]
public sealed class MysticalOilspill : PostProcessEffectSettings
{
    public TextureParameter distortionTex = new TextureParameter { value = null };
    public FloatParameter distortionStrength = new FloatParameter { value = 0.15f };
    public FloatParameter hueShiftSpeed = new FloatParameter { value = 1.0f };
    public FloatParameter overlayStrength = new FloatParameter { value = 1.0f };
    public Vector2Parameter scrollSpeed = new Vector2Parameter { value = new Vector2(0.05f, 0.05f) };
}

