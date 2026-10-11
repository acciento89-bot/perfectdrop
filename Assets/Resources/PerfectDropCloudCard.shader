Shader "Kamilunavo/PerfectDropCloudCard"
{
    Properties { _MainTex("Original billowing cloud alpha",2D)="white"{} _Color("Chapter cloud grade",Color)=(1,1,1,1) }
    SubShader
    {
        Tags {"Queue"="Transparent-100" "RenderType"="Transparent"}
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off Cull Off
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            sampler2D _MainTex;fixed4 _Color;
            struct appdata {float4 vertex:POSITION;float2 uv:TEXCOORD0;float2 size:TEXCOORD1;};
            struct v2f {float4 pos:SV_POSITION;float2 uv:TEXCOORD0;};
            v2f vert(appdata v)
            {
                v2f o;float2 offset=(v.uv-.5)*v.size;
                float3 center=mul(unity_ObjectToWorld,float4(v.vertex.xyz-float3(offset,0),1)).xyz;
                float3 right=float3(UNITY_MATRIX_V._m00,UNITY_MATRIX_V._m01,UNITY_MATRIX_V._m02);
                float3 up=float3(UNITY_MATRIX_V._m10,UNITY_MATRIX_V._m11,UNITY_MATRIX_V._m12);
                o.pos=mul(UNITY_MATRIX_VP,float4(center+right*offset.x+up*offset.y,1));o.uv=v.uv;return o;
            }
            fixed4 frag(v2f i):SV_Target{return tex2D(_MainTex,i.uv)*_Color;}
            ENDCG
        }
    }
    FallBack Off
}
