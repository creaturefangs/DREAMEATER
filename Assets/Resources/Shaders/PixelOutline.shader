Shader "Custom/PixelOutline"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineSize ("Outline Size", Range(0, 5)) = 1
    }
    
    SubShader
    {
        Tags { "Queue"="Overlay" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _OutlineColor;
            float _OutlineSize;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 offsets[8] = {
                    float2(-_OutlineSize, 0), float2(_OutlineSize, 0),
                    float2(0, -_OutlineSize), float2(0, _OutlineSize),
                    float2(-_OutlineSize, -_OutlineSize), float2(-_OutlineSize, _OutlineSize),
                    float2(_OutlineSize, -_OutlineSize), float2(_OutlineSize, _OutlineSize)
                };

                float alpha = tex2D(_MainTex, i.uv).a;
                for (int j = 0; j < 8; j++)
                {
                    alpha = max(alpha, tex2D(_MainTex, i.uv + offsets[j] * _OutlineSize * 0.01).a);
                }

                if (alpha > 0.0)
                {
                    return _OutlineColor;
                }

                return tex2D(_MainTex, i.uv);
            }
            ENDCG
        }
    }
}
