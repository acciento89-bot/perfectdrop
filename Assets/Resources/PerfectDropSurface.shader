Shader "Kamilunavo/PerfectDropSurface"
{
    Properties
    {
        _MainTex ("Metal albedo", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _Detail ("Metal surface detail", Range(0,1)) = 0
        _GoldFinish ("Brushed gold finish", Range(0,1)) = 0
        _BodyFinish ("Lacquer or ceramic detail", Range(0,2)) = 0
        _BrushFinish ("Fine metal brushing", Range(0,1)) = 0
        _StoneFinish ("Cut stone and mortar", Range(0,1)) = 0
        _WaterFinish ("Canal ripples", Range(0,1)) = 0
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
        half _GoldFinish;
        half _BodyFinish;
        half _BrushFinish;
        half _StoneFinish;
        half _WaterFinish;
        half _Metallic;
        half _Glossiness;


        struct Input
        {
            float3 worldPos;
            float3 worldNormal;
            float2 uv_MainTex;
        };

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            float grain = frac(sin(dot(floor(IN.worldPos.xz*220),float2(12.9898,78.233)))*43758.5453);
            float2 panel = abs(frac(IN.worldPos.xz*1.4)-.5);
            float seam = smoothstep(.475,.489,max(panel.x,panel.y));
            float4 tint = UNITY_ACCESS_INSTANCED_PROP(DeckProperties, _Color);
            float3 metal = tex2D(_MainTex, IN.uv_MainTex).rgb;
            // UV-bound finish travels with the deck instead of swimming through
            // world space as it moves. Reuse the existing metal texture sample.
            float2 uv=IN.uv_MainTex;
            half diagonal=saturate(uv.x*.65+uv.y*.35);
            half finish=lerp(.66,1.08,diagonal);
            half reflectionBand=(diagonal-.66)*8;
            half reflection=exp2(-reflectionBand*reflectionBand);
            half brush=sin(uv.y*920+metal.g*8)*.008;
            half edge=smoothstep(.44,.485,max(abs(uv.x-.5),abs(uv.y-.5)));
            float3 gold=lerp(float3(.77,.52,.21),float3(1.07,1.04,.85),diagonal)+reflection*float3(.13,.11,.055)+brush;
            // The same UV-bound finish runs across plate faces and bevels.
            // Avoid an emissive orange sheet: reflected light supplies the volume.
            // Smooth brushed shoulders and ceramic/lacquer skins share a neutral
            // fine finish. Industrial panel seams no longer dominate small slabs.
            half brushed=sin(uv.y*680+metal.g*2)*.026+sin(uv.y*1710+uv.x*3)*.005;
            float3 cleanMetal=.96+(brushed+reflection*.13)*_BrushFinish+metal.r*.045;
            // Veining belongs to the actual selected surface, using deck UVs so
            // it follows moving and cut pieces. No world-space swimming occurs.
            half vein=smoothstep(.94,.995,abs(sin(uv.x*13+sin(uv.y*8)*1.7+sin((uv.x+uv.y)*5)*.6)));
            half jade=step(.5,_BodyFinish)*(1-step(1.5,_BodyFinish));
            half ceramic=step(1.5,_BodyFinish);
            cleanMetal=lerp(cleanMetal,cleanMetal*.90+vein*.28,jade);
            cleanMetal=lerp(cleanMetal,1.02-vein*.065,ceramic);
            o.Albedo = tint.rgb * lerp(cleanMetal,lerp(.97,1.02,metal.r)*gold,_GoldFinish) * lerp(1, (.98+grain*.03)*(1-seam*.025), _Detail);
            float3 an=abs(IN.worldNormal);
            float2 stoneUV=an.y>.7?IN.worldPos.xz:an.x>an.z?IN.worldPos.zy:IN.worldPos.xy;
            float course=floor(stoneUV.y*1.55);
            float2 stoneCell=float2(stoneUV.x*1.18+frac(course*.5)*.5,stoneUV.y*1.55);
            float2 edgeDistance=min(frac(stoneCell),1-frac(stoneCell));
            half joint=1-smoothstep(.014,.039,min(edgeDistance.x,edgeDistance.y));
            half quarry=frac(sin(dot(floor(stoneCell),float2(31.13,71.29)))*13758.5453);
            half stonePore=frac(sin(dot(floor(stoneUV*110),float2(19.1,47.7)))*41758.3453);
            o.Albedo*=lerp(1,(.88+quarry*.15+stonePore*.035)*(1-joint*.14),_StoneFinish);
            half ripple=sin(IN.worldPos.x*3.9+sin(IN.worldPos.z*2.8))*sin(IN.worldPos.z*4.3+IN.worldPos.x*.3);
            o.Albedo*=lerp(1,.91+ripple*.12,_WaterFinish);
            o.Occlusion=1-joint*.12*_StoneFinish;
            o.Metallic = _Metallic;
            o.Smoothness = saturate(_Glossiness - _GoldFinish*((1-metal.r)*.055+edge*.06)-_WaterFinish*abs(ripple)*.10);
            o.Emission = UNITY_ACCESS_INSTANCED_PROP(DeckProperties, _EmissionColor).rgb * lerp(1, finish*.72, _GoldFinish);
            o.Alpha = tint.a;
        }
        ENDCG
    }

    FallBack "Diffuse"
}
