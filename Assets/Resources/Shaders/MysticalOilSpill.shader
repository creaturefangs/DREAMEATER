Shader "Hidden/PostProcessing/MysticalOilspill"
{
    Properties
    {
        _MainTex ("MainTex", 2D) = "white" {}
        _DistortionTex ("Distortion Map", 2D) = "gray" {}
        _DistortionStrength ("Distortion Strength", Range(0, 1)) = 0.2
        _HueShiftSpeed ("Hue Shift Speed", Range(0, 5)) = 1.0
        _OverlayStrength ("Overlay Strength", Range(0, 2)) = 1.0
        _ScrollSpeed ("Distortion Scroll Speed", Vector) = (0.1, 0.1, 0, 0)
    }

    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _DistortionTex;

            float _DistortionStrength;
            float _HueShiftSpeed;
            float _OverlayStrength;
            float4 _ScrollSpeed;

            float _TimeY;

            float3 HueShift(float3 color, float shift)
            {
                float angle = shift * 6.2831; // 2 * PI
                float s = sin(angle), c = cos(angle);
                float3x3 hueRot = float3x3(
                    0.299 + 0.701 * c + 0.168 * s, 0.587 - 0.587 * c + 0.330 * s, 0.114 - 0.114 * c - 0.497 * s,
                    0.299 - 0.299 * c - 0.328 * s, 0.587 + 0.413 * c + 0.035 * s, 0.114 - 0.114 * c + 0.292 * s,
                    0.299 - 0.3   * c + 1.25  * s, 0.587 - 0.588 * c - 1.05  * s, 0.114 + 0.886 * c - 0.203 * s
                );
                return saturate(mul(color, hueRot));
            }

            fixed4 frag(v2f_img i) : SV_Target
            {
                float2 scrollUV = i.uv + _TimeY * _ScrollSpeed.xy;
                float2 distortion = (tex2D(_DistortionTex, scrollUV).rg - 0.5) * _DistortionStrength;

                float2 uv = i.uv + distortion;
                float3 baseColor = tex2D(_MainTex, uv).rgb;

                float hueShift = frac(_TimeY * _HueShiftSpeed);
                float3 iridescentColor = HueShift(baseColor, hueShift);

                float3 finalColor = lerp(baseColor, iridescentColor, _OverlayStrength);
                return float4(finalColor, 1.0);
            }
            ENDCG
        }
    }
    Fallback Off
}
