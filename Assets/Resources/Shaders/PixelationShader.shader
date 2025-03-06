Shader "Custom/PixelationEffect"
{
    Properties
    {
        _PixelSize ("Pixel Size", Range(1, 200)) = 50
        _DitherStrength ("Dither Strength", Range(0, 1)) = 0.2
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            ZTest Always Cull Off ZWrite Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _UITexture; // Separate UI texture to exclude UI from pixelation
            float4 _MainTex_TexelSize;
            float _PixelSize;
            float _DitherStrength;

            static const float2 ditherPattern[4] =
            {
                float2(0.0, 0.5),
                float2(0.75, 0.25),
                float2(0.5, 1.0),
                float2(0.25, 0.75)
            };

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Higher detail pixelation
                float pixelScale = 1.0 / _PixelSize;
                float2 pixelUV = floor(i.uv / pixelScale) * pixelScale;

                // Fetch pixelated color
                fixed4 col = tex2D(_MainTex, pixelUV);

                // Apply dithering for a retro look
                int2 screenPos = int2(i.uv * _MainTex_TexelSize.zw);
                float ditherValue = ditherPattern[screenPos.x % 2 + (screenPos.y % 2) * 2].x;
                col.rgb += (_DitherStrength * (ditherValue - 0.5));

                // Exclude UI from pixelation
                fixed4 uiCol = tex2D(_UITexture, i.uv);
                return lerp(col, uiCol, uiCol.a); // Blend UI back in if it's visible
            }
            ENDCG
        }
    }
}

