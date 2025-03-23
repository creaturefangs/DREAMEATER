Shader "Unlit/UnderwaterEffect"
{
    
    Properties
    {
        _Color ("Tint Color", Color) = (1, 0, 0, 1)
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
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _Color;

            v2f vert(float4 vertex : POSITION)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return _Color; // Should make screen RED
            }
            ENDHLSL
        }
    }
}
