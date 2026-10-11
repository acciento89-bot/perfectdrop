Shader "Kamilunavo/PerfectDropFoliage"
{
    Properties { _MainTex("Original cypress cutout",2D)="white"{} _Color("Foliage tint",Color)=(1,1,1,1) _Cutoff("Leaf coverage",Range(0,1))=.35 }
    SubShader
    {
        Tags {"Queue"="AlphaTest" "RenderType"="TransparentCutout"}
        Cull Off
        CGPROGRAM
        #pragma surface surf Standard alphatest:_Cutoff addshadow
        #pragma target 3.0
        sampler2D _MainTex;fixed4 _Color;
        struct Input {float2 uv_MainTex;};
        void surf(Input IN,inout SurfaceOutputStandard o)
        {fixed4 c=tex2D(_MainTex,IN.uv_MainTex)*_Color;o.Albedo=c.rgb;o.Alpha=c.a;o.Metallic=0;o.Smoothness=.12;o.Emission=c.rgb*.20;}
        ENDCG
    }
    FallBack "Transparent/Cutout/Diffuse"
}
