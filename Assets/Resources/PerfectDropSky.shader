Shader "Kamilunavo/PerfectDropSky"
{
    Properties
    {
        _CloudPanorama ("Original golden cloud panorama", 2D) = "black" {}
        _UseCloudPanorama ("World direction cloud asset", Float) = 0
        _SkyNight ("Night mineral sky grade", Float) = 0
        _SkyDay ("Cool daylight cloud grade", Float) = 0
        _SkyGrade ("Chapter sky grade", Color) = (1,1,1,1)
        _BackdropPortrait ("Portrait cloud city", 2D) = "black" {}
        _BackdropLandscape ("Landscape cloud city", 2D) = "black" {}
        _UseBackdrop ("Use authored environment", Float) = 0
        _TopColor ("Top", Color) = (0.035,0.12,0.32,1)
        _MidColor ("Mid", Color) = (0.23,0.34,0.62,1)
        _HorizonColor ("Horizon", Color) = (1.0,0.43,0.18,1)
        _BottomColor ("Bottom", Color) = (0.08,0.11,0.20,1)
        _CloudColor ("Cloud reflected light", Color) = (0.94,0.85,0.74,1)
        _SunColor ("Sun", Color) = (1.4,0.8,0.30,1)
        _SunDirection ("Sun Direction", Vector) = (0.72,0.05,0.65,0)
        _CloudOrigin ("Cloud world origin", Vector) = (0,0,0,0)
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
            fixed4 _CloudColor;
            float4 _SunDirection;
            float4 _CloudOrigin;
            sampler2D _BackdropPortrait, _BackdropLandscape;
            float4 _BackdropPortrait_TexelSize, _BackdropLandscape_TexelSize;
            float _UseBackdrop,_UseCloudPanorama,_SkyNight,_SkyDay;
            sampler2D _CloudPanorama;float4 _SkyGrade;

            struct appdata { float4 vertex : POSITION; };
            struct v2f { float4 pos : SV_POSITION; float3 dir : TEXCOORD0; float4 screen : TEXCOORD1; };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.dir = v.vertex.xyz;
                o.screen = ComputeScreenPos(o.pos);
                return o;
            }

            float hash21(float2 p) { float3 p3=frac(float3(p.xyx)*.1031);p3+=dot(p3,p3.yzx+33.33);return frac((p3.x+p3.y)*p3.z); }
            float cloudNoise(float2 p) {float2 cell=floor(p),f=frac(p);f=f*f*(3-2*f);return lerp(lerp(hash21(cell),hash21(cell+float2(1,0)),f.x),lerp(hash21(cell+float2(0,1)),hash21(cell+1),f.x),f.y);}
            float cloudBank(float2 p)
            {
                // Overlapping rounded billows, rather than a smooth noise carpet.
                // Fixed world cells keep the cloud relief still during reduced motion.
                float2 cell=floor(p);
                float billows=0;
                [unroll] for(int z=-1;z<=1;z++)
                [unroll] for(int x=-1;x<=1;x++)
                {
                    float2 seed=cell+float2(x,z);
                    float2 center=seed+.5+float2(hash21(seed+13.7),hash21(seed-8.3))*.48-.24;
                    float radius=.36+hash21(seed+4.2)*.28;
                    float2 delta=(p-center)/radius;
                    billows+=exp(-dot(delta,delta)*2.8)*(.7+hash21(seed-21.4)*.3);
                }
                float detail=cloudNoise(p*6.3+4.7)*.07+cloudNoise(p*13.1-2.6)*.025;
                return saturate(billows*.78+detail);
            }
            fixed4 frag(v2f i) : SV_Target
            {
                if (_UseBackdrop > .5)
                {
                    float aspect = _ScreenParams.x / _ScreenParams.y;
                    bool portrait = aspect < 1;
                    float imageAspect = portrait ? _BackdropPortrait_TexelSize.z / _BackdropPortrait_TexelSize.w : _BackdropLandscape_TexelSize.z / _BackdropLandscape_TexelSize.w;
                    float2 crop = float2(min(1, aspect / imageAspect), min(1, imageAspect / aspect));
                    float2 uv = (i.screen.xy / i.screen.w - .5) * crop + .5;
                    return portrait ? tex2D(_BackdropPortrait, uv) : tex2D(_BackdropLandscape, uv);
                }
                float3 d = normalize(i.dir);
                if(_UseCloudPanorama>.5)
                {
                    // Spherical world-direction sampling preserves camera orbit.
                    // This asset contains only sky/clouds; all buildings remain meshes.
                    // Two mirrored cycles per orbit are continuous at both the atan2 branch
                    // and the unmatched edges of the original flat sky texture.
                    float phase=atan2(d.x,d.z)*.636619772;
                    float panoramaU=1-abs(frac(phase*.5)*2-1);
                    // Keep downward rays inside the cloud bank: the former .65
                    // latitude scale clamped every steep ray to one texture row,
                    // stretching that row into vertical stripes on high towers.
                    float2 uv=float2(panoramaU,clamp(asin(clamp(d.y,-1,1))*.35+.80,.001,.999));
                    float3 sky=tex2D(_CloudPanorama,uv).rgb;
                    // Regrade the original cloud asset into daylight without
                    // replacing its billows or introducing a screen-space image.
                    if(_SkyDay>.5)
                    {
                        float warm=saturate((sky.r-sky.b)*3.5);
                        float luminance=dot(sky,float3(.22,.71,.07));
                        float3 daylight=lerp(float3(.30,.48,.71),float3(.96,.96,.90),smoothstep(.18,.85,luminance));
                        sky=lerp(sky,daylight,warm*.92);
                    }
                    if(_SkyNight>.5){float luminance=dot(sky,float3(.22,.71,.07));sky=lerp(float3(.035,.028,.14),float3(.40,.24,.66),luminance);}
                    return fixed4(sky*_SkyGrade.rgb,1);
                }
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
                // Static angular cloud volume belongs to the world view. The real
                // canal/arcades provide depth; no skyscraper wallpaper is sampled.
                float2 cloudUV=d.xz/max(.18,d.y+.64)*2.3;
                float density=cloudNoise(cloudUV)*.62+cloudNoise(cloudUV*2.17+4.2)*.26+cloudNoise(cloudUV*4.1-1.8)*.12;
                float veil=smoothstep(.53,.76,density)*smoothstep(-.30,-.04,d.y)*(1-smoothstep(.45,.85,d.y));
                c=lerp(c,_CloudColor.rgb+sunGlow*.22,veil*.72);
                // The elevated stack camera looks below the horizon. Project a
                // static cloud bank below the terrace into world space, so its
                // relief has parallax when the camera rises or the player orbits.
                // No fullscreen city photograph, transparent shell or animation.
                float below=1-smoothstep(-.24,-.015,d.y);
                if(below>0)
                {
                float3 eye=_WorldSpaceCameraPos-_CloudOrigin.xyz;
                float distance=min(360,max(1,eye.y+12)/max(.035,-d.y));
                float2 bankUV=(eye.xz+d.xz*distance)*.045;
                float bank=cloudBank(bankUV);
                float dx=cloudBank(bankUV+float2(.035,0))-bank;
                float dz=cloudBank(bankUV+float2(0,.035))-bank;
                float3 cloudNormal=normalize(float3(-dx*12,1,-dz*12));
                float lighting=saturate(dot(cloudNormal,normalize(float3(_SunDirection.x,.65,_SunDirection.z))));
                float crest=smoothstep(.08,.65,bank);
                float3 shadow=lerp(_MidColor.rgb,_CloudColor.rgb,.36);
                float3 bankColor=lerp(shadow,_CloudColor.rgb,crest);
                bankColor*=.88+lighting*.14;
                bankColor+=_SunColor.rgb*pow(saturate(bank),3)*.045;
                bankColor=lerp(bankColor,_HorizonColor.rgb*.84,smoothstep(100,360,distance)*.65);
                c=lerp(c,bankColor,below);
                }
                return fixed4(c, 1);
            }
            ENDCG
        }
    }
    FallBack Off
}
