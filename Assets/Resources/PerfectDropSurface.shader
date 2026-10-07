Shader "Kamilunavo/PerfectDropSurface"
{
    Properties
    {
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

        fixed4 _Color;
        half _Detail;
        half _Metallic;
        half _Glossiness;
        fixed4 _EmissionColor;

        struct Input
        {
            float3 worldPos;
        };

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            float grain = frac(sin(dot(floor(IN.worldPos.xz*220),float2(12.9898,78.233)))*43758.5453);
            float2 panel = abs(frac(IN.worldPos.xz*1.4)-.5);
            float seam = smoothstep(.475,.489,max(panel.x,panel.y));
            o.Albedo = _Color.rgb * lerp(1, (.91+grain*.18)*(1-seam*.30), _Detail);
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Emission = _EmissionColor.rgb;
            o.Alpha = _Color.a;
        }
        ENDCG
    }

    FallBack "Diffuse"
}
