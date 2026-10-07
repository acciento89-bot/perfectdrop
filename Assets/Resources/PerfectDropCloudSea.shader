Shader "Kamilunavo/PerfectDropCloudSea"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Cull Off
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #include "UnityCG.cginc"
            struct V { float4 pos:SV_POSITION; float3 world:TEXCOORD0; UNITY_FOG_COORDS(1) };
            V vert(appdata_base v) { V o; o.pos=UnityObjectToClipPos(v.vertex); o.world=mul(unity_ObjectToWorld,v.vertex).xyz; UNITY_TRANSFER_FOG(o,o.pos); return o; }
            float hash(float2 p) { return frac(sin(dot(p,float2(127.1,311.7)))*43758.5453); }
            float noise(float2 p)
            {
                float2 i=floor(p), f=frac(p); f=f*f*(3-2*f);
                return lerp(lerp(hash(i),hash(i+float2(1,0)),f.x),lerp(hash(i+float2(0,1)),hash(i+1),f.x),f.y);
            }
            float cloud(float2 p) { return noise(p)*.55+noise(p*2.03)*.27+noise(p*4.13)*.13+noise(p*8.27)*.05; }
            fixed4 frag(V i):SV_Target
            {
                float2 p=i.world.xz*.075;
                float n=cloud(p);
                float shade=cloud(p+float2(.08,.05))-n;
                float3 c=lerp(float3(.24,.29,.46),float3(.91,.81,.84),smoothstep(.20,.72,n));
                c+=shade*2.8;
                c=lerp(c,float3(1,.72,.51),saturate((i.world.x+30)/140)*.18);
                fixed4 result=fixed4(c,1); UNITY_APPLY_FOG(i.fogCoord,result); return result;
            }
            ENDCG
        }
    }
}
