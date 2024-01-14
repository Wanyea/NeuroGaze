Shader "Custom/SimpleGlow"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _GlowColor ("Glow Color", Color) = (1,1,1,1)
        _Glow ("Glow Strength", Range(0, 10)) = 2.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 normal : NORMAL;
            };

            float4 _Color;
            float4 _GlowColor;
            float _Glow;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.normal = v.normal;
                return o;
            }
            
            float4 frag (v2f i) : SV_Target
            {
                float3 normal = normalize(i.normal);
                float intensity = pow(1 - dot(normal, float3(0,0,1)), _Glow);
                float4 color = _Color + _GlowColor * intensity;
                return color;
            }
            ENDCG
        }
    }
}
