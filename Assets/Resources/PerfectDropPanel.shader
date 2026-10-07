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
                float radius=min(28,min(size.x,size.y)*.48);
                float2 q=abs(localPoint)-(size*.5-radius);
                float distance=length(max(q,0))+min(max(q.x,q.y),0)-radius;
                float edge=1-smoothstep(1,3,abs(distance+2));
                float gold=step(.5,i.color.r)*step(.3,i.color.g);
                c.rgb*=lerp(.83,1.25,i.panel.y);
                c.rgb+=pow(saturate(i.panel.y),8)*lerp(float3(.02,.035,.055),float3(.10,.07,.01),gold);
                c.rgb=lerp(c.rgb,lerp(float3(.23,.31,.44),float3(1,.87,.41),gold),edge*.65);
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
