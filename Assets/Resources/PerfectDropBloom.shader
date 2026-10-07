Shader "Kamilunavo/PerfectDropBloom"
{
    Properties { _MainTex("Source",2D)="black"{} }
    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        CGINCLUDE
        #include "UnityCG.cginc"
        sampler2D _MainTex;
        float4 _MainTex_TexelSize;
        float2 _Direction;
        half4 extract(v2f_img i):SV_Target
        {
            half3 c=tex2D(_MainTex,i.uv).rgb;
            half brightness=max(c.r,max(c.g,c.b));
            // Only actual emissive rails bloom; the painted clouds stay crisp.
            return half4(c*max(0,brightness-1.05)/max(brightness,.001),1);
        }
        half4 blur(v2f_img i):SV_Target
        {
            float2 step=_MainTex_TexelSize.xy*_Direction;
            half3 c=tex2D(_MainTex,i.uv).rgb*.227027;
            c+=(tex2D(_MainTex,i.uv+step*1.384615).rgb+tex2D(_MainTex,i.uv-step*1.384615).rgb)*.316216;
            c+=(tex2D(_MainTex,i.uv+step*3.230769).rgb+tex2D(_MainTex,i.uv-step*3.230769).rgb)*.070270;
            return half4(c,1);
        }
        ENDCG
        Pass { CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment extract
            ENDCG }
        Pass { CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment blur
            ENDCG }
    }
    FallBack Off
}
