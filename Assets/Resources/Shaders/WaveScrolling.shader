Shader "Custom/WaveScrolling"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ScrollSpeed ("Scroll Speed (X,Y)", Vector) = (0.2, 0, 0, 0)
        _WaveStrength ("Wave Strength", Range(0,0.2)) = 0.05
        _WaveFrequency ("Wave Frequency", Float) = 5
        _WaveSpeed ("Wave Speed", Float) = 1
        _Tint ("Tint", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        Lighting Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float4 _ScrollSpeed;
            float _WaveStrength;
            float _WaveFrequency;
            float _WaveSpeed;
            float4 _Tint;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);

                float2 uv = v.uv;

                // Scrolling
                uv += _ScrollSpeed.xy * _Time.y;

                // Wave distortion
                float wave = sin((uv.y * _WaveFrequency) + (_Time.y * _WaveSpeed)) * _WaveStrength;
                uv.x += wave; // horizontal distortion

                o.uv = uv * _MainTex_ST.xy + _MainTex_ST.zw;
                o.color = v.color * _Tint;

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return tex2D(_MainTex, i.uv) * i.color;
            }

            ENDCG
        }
    }
}
