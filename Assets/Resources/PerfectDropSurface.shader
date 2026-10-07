Shader "Kamilunavo/PerfectDropSurface"
{
    Properties
    {
        _MainTex ("Metal albedo", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _Detail ("Metal surface detail", Range(0,1)) = 0
        _Metallic ("Metallic", Range(0,1)) = 0
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _EmissionColor ("Emission", Color) = (0,0,0,0)
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0
        #pragma multi_compile_instancing

        sampler2D _MainTex;
        UNITY_INSTANCING_BUFFER_START(DeckProperties)
            UNITY_DEFINE_INSTANCED_PROP(float4, _Color)
            UNITY_DEFINE_INSTANCED_PROP(float4, _EmissionColor)
        UNITY_INSTANCING_BUFFER_END(DeckProperties)
        half _Detail;
        half _Metallic;
        half _Glossiness;


        struct Input
        {
            float3 worldPos;
            float2 uv_MainTex;
        };

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            float grain = frac(sin(dot(floor(IN.worldPos.xz*220),float2(12.9898,78.233)))*43758.5453);
            float2 panel = abs(frac(IN.worldPos.xz*1.4)-.5);
            float seam = smoothstep(.475,.489,max(panel.x,panel.y));
            float4 tint = UNITY_ACCESS_INSTANCED_PROP(DeckProperties, _Color);
            o.Albedo = tint.rgb * tex2D(_MainTex, IN.uv_MainTex).rgb * lerp(1, (.94+grain*.12)*(1-seam*.16), _Detail);
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Emission = UNITY_ACCESS_INSTANCED_PROP(DeckProperties, _EmissionColor).rgb;
            o.Alpha = tint.a;
        }
        ENDCG
    }

    FallBack "Diffuse"
}
