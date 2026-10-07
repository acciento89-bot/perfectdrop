Shader "Kamilunavo/PerfectDropSky"
{
    Properties
    {
        _TopColor ("Top", Color) = (0.035,0.12,0.32,1)
        _MidColor ("Mid", Color) = (0.23,0.34,0.62,1)
        _HorizonColor ("Horizon", Color) = (1.0,0.43,0.18,1)
        _BottomColor ("Bottom", Color) = (0.08,0.11,0.20,1)
        _SunColor ("Sun", Color) = (1.4,0.8,0.30,1)
        _SunDirection ("Sun Direction", Vector) = (0.72,0.05,0.65,0)
    }

    SubShader
    {
        Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            fixed4 _TopColor;
            fixed4 _MidColor;
            fixed4 _HorizonColor;
            fixed4 _BottomColor;
            fixed4 _SunColor;
            float4 _SunDirection;

            struct appdata { float4 vertex : POSITION; };
            struct v2f { float4 pos : SV_POSITION; float3 dir : TEXCOORD0; };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.dir = v.vertex.xyz;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float3 d = normalize(i.dir);
                float y = d.y;
                float horizon = saturate(1.0 - abs(y) * 4.5);
                float upper = saturate(y * 1.6 + 0.15);
                float lower = saturate(-y * 1.8);

                float3 c = lerp(_MidColor.rgb, _TopColor.rgb, upper);
                c = lerp(c, _BottomColor.rgb, lower);
                c = lerp(c, _HorizonColor.rgb, horizon * 0.74);

                float sunDot = saturate(dot(d, normalize(_SunDirection.xyz)));
                float sunCore = pow(sunDot, 520.0);
                float sunGlow = pow(sunDot, 26.0) * 0.34;
                c += _SunColor.rgb * (sunCore * 1.6 + sunGlow);
                return fixed4(c, 1);
            }
            ENDCG
        }
    }
    FallBack Off
}
