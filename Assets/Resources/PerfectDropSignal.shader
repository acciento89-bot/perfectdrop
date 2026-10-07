Shader "Kamilunavo/PerfectDropSignal"
{
    Properties { _Tint("Signal",Color)=(1,.65,.12,1) }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha One ZWrite Off Cull Off
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"
            UNITY_INSTANCING_BUFFER_START(Signals)
                UNITY_DEFINE_INSTANCED_PROP(float4,_Tint)
            UNITY_INSTANCING_BUFFER_END(Signals)
            struct V {float4 vertex:POSITION; UNITY_VERTEX_INPUT_INSTANCE_ID};
            struct F {float4 position:SV_POSITION; UNITY_VERTEX_INPUT_INSTANCE_ID};
            F vert(V v) {F o; UNITY_SETUP_INSTANCE_ID(v); UNITY_TRANSFER_INSTANCE_ID(v,o); o.position=UnityObjectToClipPos(v.vertex); return o;}
            half4 frag(F i):SV_Target {UNITY_SETUP_INSTANCE_ID(i); half4 c=UNITY_ACCESS_INSTANCED_PROP(Signals,_Tint);c.rgb*=1.6;return c;}
            ENDCG
        }
    }
    FallBack Off
}
