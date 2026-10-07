Shader "Kamilunavo/PerfectDropGrade"
{
    Properties
    {
        _MainTex ("Source", 2D) = "white" {}
        _BloomTex ("Soft emissive light",2D) = "black" {}
        _Vignette ("Vignette", Range(0,1)) = 0.32
        _Saturation ("Saturation", Range(0,2)) = 1.10
        _Contrast ("Contrast", Range(0.5,2)) = 1.08
        _Warmth ("Warmth", Range(-1,1)) = 0.07
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
            sampler2D _BloomTex;
            float _Vignette;
            float _Saturation;
            float _Contrast;
            float _Warmth;

            half4 frag(v2f_img i) : SV_Target
            {
                half4 src = tex2D(_MainTex, i.uv);
                float3 c = src.rgb + tex2D(_BloomTex,i.uv).rgb*.55;

                float luminance = dot(c, float3(0.2126, 0.7152, 0.0722));
                c = lerp(luminance.xxx, c, _Saturation);
                c = (c - 0.5) * _Contrast + 0.5;
                c += float3(_Warmth * 0.055, _Warmth * 0.018, -_Warmth * 0.040);

                float2 centered = i.uv * 2.0 - 1.0;
                float vignette = saturate(1.0 - dot(centered, centered) * _Vignette * 0.62);
                c *= lerp(0.78, 1.0, vignette);

                // Keep emissive gold/cyan highlights vivid without blowing out UI/world detail.
                c = c / (1.0 + max(c - 1.0, 0.0) * 0.35);
                return half4(saturate(c), src.a);
            }
            ENDCG
        }
    }

    FallBack Off
}
