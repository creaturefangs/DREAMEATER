Shader "Hidden/MysticalOilSpill"
{
    Properties
    {
        _MainTex("Main Texture", 2D) = "white" {}
        _DistortionTex("Distortion Texture", 2D) = "white" {}
        _TintColor("Tint Color", Color) = (1,1,1,1)
        _HueShiftSpeed("Hue Shift Speed", Float) = 1
        _DistortionStrength("Distortion Strength", Float) = 0.1
        _DistortionSpeed("Distortion Speed", Float) = 1
        _DistortionTiling("Distortion Tiling", Vector) = (1,1,0,0)
        _DistortionOpacity("Distortion Opacity", Float) = 1
        _EffectOpacity("Effect Opacity", Float) = 1
        _EnableChromaticAberration("Enable Chromatic Aberration", Float) = 1
        _TimeX("Time", Float) = 0
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        Pass
        {
            ZTest Always Cull Off ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _DistortionTex;

            float4 _MainTex_TexelSize;

            float4 _TintColor;
            float _HueShiftSpeed;
            float _DistortionStrength;
            float _DistortionSpeed;
            float2 _DistortionTiling;
            float _DistortionOpacity;
            float _EffectOpacity;
            float _EnableChromaticAberration;
            float _TimeX;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float3 HueShift(float3 color, float shift)
            {
                float angle = shift * 6.2831853; // 2 * PI
                float s = sin(angle), c = cos(angle);
                float3 weights = (float3(2.0 * c, -sqrt(3.0) * s - c, sqrt(3.0) * s - c) + 1.0) / 3.0;
                return float3(
                    dot(color, weights.xyz),
                    dot(color, weights.zxy),
                    dot(color, weights.yzx)
                );
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Scroll distortion texture
                float2 distortionUV = i.uv * _DistortionTiling;
                distortionUV += float2(_TimeX * _DistortionSpeed, _TimeX * _DistortionSpeed);
                float3 distortion = tex2D(_DistortionTex, distortionUV).rgb;

                // Apply distortion with opacity
                float2 offset = (distortion.rg - 0.5) * 2 * _DistortionStrength * _DistortionOpacity;
                float2 uvDistorted = i.uv + offset;

                float4 color = tex2D(_MainTex, uvDistorted);

                // Hue shift
                color.rgb = HueShift(color.rgb, _TimeX * _HueShiftSpeed);

                // Tint
                color.rgb *= _TintColor.rgb;

                // Optional chromatic aberration
                if (_EnableChromaticAberration > 0.5)
                {
                    float2 offsetR = offset * 0.5;
                    float2 offsetG = offset * 0.25;
                    float2 offsetB = offset * -0.25;

                    float r = tex2D(_MainTex, i.uv + offsetR).r;
                    float g = tex2D(_MainTex, i.uv + offsetG).g;
                    float b = tex2D(_MainTex, i.uv + offsetB).b;

                    color.rgb = float3(r, g, b);
                }

                // Final blend with original based on EffectOpacity
                float4 original = tex2D(_MainTex, i.uv);
                color = lerp(original, color, _EffectOpacity);

                return color;
            }
            ENDCG
        }
    }
}
