Shader "Hidden/UnderwaterEffect"
{
    Properties
    {
        _Strength ("Distortion Strength", Range(0.001, 0.5)) = 0.05
        _Speed ("Wave Speed", Range(0, 5)) = 1
        _OverlayTex ("Overlay Texture", 2D) = "white" {}
        _OverlaySpeed ("Overlay Scroll Speed", Range(0, 2)) = 0.2
        _OverlayOpacity ("Overlay Opacity", Range(0, 1)) = 0.5
        _TintColor ("Water Tint", Color) = (0.2, 0.4, 0.8, 1)

        // Bloom Settings
        _BloomIntensity ("Bloom Intensity", Range(0, 5)) = 1
        _BloomThreshold ("Bloom Threshold", Range(0, 1)) = 0.5
        _BloomSoftness ("Bloom Softness", Range(0, 1)) = 0.2
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 position : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _OverlayTex;
            float _Strength;
            float _Speed;
            float _OverlaySpeed;
            float _OverlayOpacity;
            float4 _TintColor;
            float _BloomIntensity;
            float _BloomThreshold;
            float _BloomSoftness;
            float _TimeX;

            v2f vert(float4 vertex : POSITION, float2 uv : TEXCOORD0)
            {
                v2f o;
                o.position = UnityObjectToClipPos(vertex);
                o.uv = uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Wave distortion effect
                float2 distortion = float2(
                    sin(i.uv.y * 10.0 + _TimeX * _Speed),
                    cos(i.uv.x * 10.0 + _TimeX * _Speed)
                );
                float2 offset = i.uv + distortion * _Strength;

                // Overlay scrolling texture
                float2 overlayUV = i.uv + float2(_TimeX * _OverlaySpeed, _TimeX * _OverlaySpeed * 0.5);
                fixed4 overlay = tex2D(_OverlayTex, overlayUV) * _OverlayOpacity;

                // Base underwater color effect
                fixed4 baseColor = tex2D(_MainTex, offset) * _TintColor;
                baseColor = lerp(baseColor, overlay, _OverlayOpacity);

                // Bloom effect: enhance bright areas
                float luminance = dot(baseColor.rgb, float3(0.2126, 0.7152, 0.0722)); // Luminance calculation
                float bloomFactor = smoothstep(_BloomThreshold, _BloomThreshold + _BloomSoftness, luminance);
                baseColor.rgb += bloomFactor * _BloomIntensity;

                return baseColor;
            }
            ENDHLSL
        }
    }
}
