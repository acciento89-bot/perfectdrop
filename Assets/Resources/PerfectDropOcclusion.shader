Shader "Kamilunavo/PerfectDropOcclusion"
{
    Properties { _MainTex ("Source",2D) = "white" {} }
    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        CGINCLUDE
        #include "UnityCG.cginc"
        sampler2D _CameraDepthNormalsTexture, _MainTex;
        float4 _MainTex_TexelSize;
        float4x4 _InverseProjection;
        float _FarPlane, _ProjectionScale, _Aspect, _Orthographic;
        void Geometry(float2 uv,out float depth,out float3 normal)
        {
            DecodeDepthNormal(tex2D(_CameraDepthNormalsTexture,uv),depth,normal);
        }
        float3 Position(float2 uv,float depth)
        {
            float4 ray=mul(_InverseProjection,float4(uv*2-1,1,1));
            ray.xyz/=ray.w;
            if(_Orthographic>.5)return float3(ray.xy,-depth*_FarPlane);
            return ray.xyz*(depth*_FarPlane/max(.0001,-ray.z));
        }
        half4 Contact(v2f_img input):SV_Target
        {
            float depth;float3 normal;Geometry(input.uv,depth,normal);
            if(depth>=.999 || depth<=.00001)return 1;
            float3 position=Position(input.uv,depth);
            float radius=clamp(.65*_ProjectionScale*.5/(_Orthographic>.5?1:max(.1,depth*_FarPlane)),.002,.045);
            float rotation=frac(sin(dot(floor(input.pos.xy),float2(12.9898,78.233)))*43758.5453)*6.2831853;
            float obscured=0;
            [unroll] for(int tap=0;tap<8;tap++)
            {
                float angle=rotation+tap*.7853982;
                float2 offset=float2(cos(angle)/_Aspect,sin(angle))*radius*(tap%2==0?.45:1);
                float2 uv=input.uv+offset;
                if(any(uv<0)||any(uv>1))continue;
                float neighborDepth;float3 neighborNormal;Geometry(uv,neighborDepth,neighborNormal);
                if(neighborDepth>=.999 || neighborDepth<=.00001)continue;
                float3 delta=Position(uv,neighborDepth)-position;
                float separation=length(delta);
                float hemisphere=max(0,dot(normal,delta/max(.001,separation))-.12);
                obscured+=hemisphere*saturate(1-separation/1.25);
            }
            float light=1-saturate(obscured*.55);
            return half4(light,light,light,1);
        }
        half4 Filter(v2f_img input):SV_Target
        {
            float depth;float3 normal;Geometry(input.uv,depth,normal);
            float total=tex2D(_MainTex,input.uv).r*2,weight=2;
            [unroll] for(int tap=0;tap<4;tap++)
            {
                float2 offset=tap==0?float2(1,0):tap==1?float2(-1,0):tap==2?float2(0,1):float2(0,-1);
                float2 uv=input.uv+offset*abs(_MainTex_TexelSize.xy);
                float otherDepth;float3 otherNormal;Geometry(uv,otherDepth,otherNormal);
                float w=saturate(1-abs(depth-otherDepth)*_FarPlane*4)*saturate(dot(normal,otherNormal));
                total+=tex2D(_MainTex,uv).r*w;weight+=w;
            }
            float light=total/weight;return half4(light,light,light,1);
        }
        ENDCG
        Pass { CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert_img
            #pragma fragment Contact
        ENDCG }
        Pass { CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert_img
            #pragma fragment Filter
        ENDCG }
    }
    FallBack Off
}
