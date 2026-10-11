Shader "Kamilunavo/PerfectDropDistantCity"
{
    Properties { _MainTex ("Original distant city matte",2D)="white"{} _CloudTex ("Original below-horizon clouds",2D)="white"{} _CloudOrigin("Cloud bank world origin",Vector)=(0,0,0,0) _Night("Night clouds",Float)=0 _Day("Day clouds",Float)=0 }
    SubShader
    {
        Tags {"Queue"="Transparent-400" "RenderType"="Background"}
        Cull Back ZWrite Off ZTest LEqual
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            sampler2D _MainTex,_CloudTex;float _Night,_Day;float4 _CloudOrigin;
            struct appdata {float4 vertex:POSITION;float2 uv:TEXCOORD0;};
            struct v2f {float4 pos:SV_POSITION;float3 direction:TEXCOORD0;};
            v2f vert(appdata v){v2f o;o.pos=mul(UNITY_MATRIX_VP,float4(_WorldSpaceCameraPos+v.vertex.xyz,1));
                // Preserve angular artwork projection, but depth-test as scenery
                // behind every actual city building, even in narrow orbit panes.
                #if defined(UNITY_REVERSED_Z)
                    o.pos.z=0;
                #else
                    o.pos.z=o.pos.w;
                #endif
                o.direction=v.vertex.xyz;return o;}
            half4 frag(v2f i):SV_Target
            {
                float3 d=normalize(i.direction);
                float phase=(atan2(d.x,d.z)-.610865238)/1.570796327+.5;
                float u=1-abs(frac(phase*.5)*2-1);
                // The painting's horizon is tilted to the gameplay composition.
                // Direction sampling avoids cap/fan and near-clip UV stretching.
                float latitude=asin(clamp(d.y,-1,1));
                float v=.5+(latitude+.554995728)/1.1;
                float3 city=tex2D(_MainTex,float2(u,clamp(v,.001,.999))).rgb;
                float2 cloudUV=float2(u,clamp(latitude*.35+.80,.001,.999));
                float3 clouds=tex2D(_CloudTex,cloudUV).rgb;
                if(d.y<-.20)
                {
                    float3 eye=_WorldSpaceCameraPos-_CloudOrigin.xyz;
                    float distance=max(1,eye.y+28)/max(.12,-d.y);
                    float2 bank=1-abs(frac((eye.xz+d.xz*distance)*float2(.018,.09473684))*2-1);
                    clouds=lerp(clouds,tex2D(_CloudTex,float2(bank.x,.025+bank.y*.38)).rgb,1-smoothstep(-.52,-.20,d.y));
                }
                if(_Day>.5)clouds=lerp(clouds,float3(dot(clouds,float3(.2126,.7152,.0722))*.9+.10,dot(clouds,float3(.2126,.7152,.0722))*.95+.08,dot(clouds,float3(.2126,.7152,.0722))+ .04),.8);
                if(_Night>.5)clouds=dot(clouds,float3(.2126,.7152,.0722))*float3(.38,.25,.63);
                // Lower steep rays leave the painting before reaching its last
                // city row, so architecture never stretches into a cloud curtain.
                return half4(lerp(clouds,city,smoothstep(.20,.38,v)),1);
            }
            ENDCG
        }
    }
    FallBack Off
}
