Shader "Kamilunavo/PerfectDropPanel"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "CanUseSpriteAtlas"="True" }
        Stencil { Ref [_Stencil] Comp [_StencilComp] Pass [_StencilOp] ReadMask [_StencilReadMask] WriteMask [_StencilWriteMask] }
        Cull Off Lighting Off ZWrite Off ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"
            struct V { float4 vertex:POSITION; float4 color:COLOR; float2 uv:TEXCOORD0; float4 panel:TEXCOORD1; UNITY_VERTEX_INPUT_INSTANCE_ID };
            struct F { float4 vertex:SV_POSITION; float4 color:COLOR; float2 uv:TEXCOORD0; float4 panel:TEXCOORD1; float4 world:TEXCOORD2; UNITY_VERTEX_OUTPUT_STEREO };
            sampler2D _MainTex;
            fixed4 _Color, _TextureSampleAdd;
            float4 _ClipRect;
            F vert(V v)
            {
                F o; UNITY_SETUP_INSTANCE_ID(v); UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.world=v.vertex; o.vertex=UnityObjectToClipPos(v.vertex);
                o.color=v.color*_Color; o.uv=v.uv; o.panel=v.panel; return o;
            }
            fixed4 frag(F i):SV_Target
            {
                fixed4 c=(tex2D(_MainTex,i.uv)+_TextureSampleAdd)*i.color;
                float2 size=max(i.panel.zw,1), localPoint=(i.panel.xy-.5)*size;
                float radius=min(28,min(size.x,size.y)*.24);
                float2 q=abs(localPoint)-(size*.5-radius);
                float distance=length(max(q,0))+min(max(q.x,q.y),0)-radius;
                float aa=max(.7,fwidth(distance));
                float edge=1-smoothstep(.65,1.6,abs(distance+1.5));
                float gold=step(.48,i.color.r)*step(.25,i.color.g)*step(i.color.b,i.color.g*.7);
                float height=saturate(i.panel.y);
                float ivory=step(.78,min(i.color.r,i.color.g))*step(.68,i.color.b)*(1-gold);
                // Warm bevel, cream reflection and an amber lower face give gold
                // actions the same finish as the live decks. Dark cards stay cool.
                float3 navy=c.rgb*lerp(.82,1.12,height)+float3(.018,.029,.05)*pow(height,5);
                float3 metal=c.rgb*lerp(float3(.94,.69,.29),float3(1.04,1.11,1.5),height);
                float sheenBand=(height-.78)*10;
                float sheen=exp2(-sheenBand*sheenBand);
                metal+=float3(.11,.085,.025)*sheen;
                float3 paper=c.rgb*lerp(.97,1.01,height);
                c.rgb=lerp(lerp(navy,paper,ivory),metal,gold);
                float3 rim=lerp(lerp(float3(.30,.40,.57),float3(.79,.69,.49),ivory),float3(1,.91,.57),gold);
                c.rgb=lerp(c.rgb,rim,edge*lerp(.34,.68,gold));
                c.a*=1-smoothstep(-aa,aa,distance);
                #ifdef UNITY_UI_CLIP_RECT
                c.a*=UnityGet2DClipping(i.world.xy,_ClipRect);
                #endif
                #ifdef UNITY_UI_ALPHACLIP
                clip(c.a-.001);
                #endif
                return c;
            }
            ENDCG
        }
    }
}
