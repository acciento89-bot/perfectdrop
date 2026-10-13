using UnityEngine;
namespace Kamilunavo.PerfectDrop.Visuals {
    // One finish definition feeds the real shader overrides and menu specimens.
    public readonly struct StackStyleFinish {
        public readonly Color Body,Plate,Accent;
        public readonly float GoldFinish,Metallic,Smoothness;
        public StackStyleFinish(Color body,Color plate,Color accent,float goldFinish=0,float metallic=.72f,float smoothness=.62f){Body=body;Plate=plate;Accent=accent;GoldFinish=goldFinish;Metallic=metallic;Smoothness=smoothness;}
    }
    public static class StackStylePalette {
        public const float PlateGlow=.14f,RimGlow=.12f,PlateMetallic=.58f,PlateSmoothness=.68f;
        public static StackStyleFinish Get(int style)=>Mathf.Clamp(style,0,7) switch {
            1=>new(new(.07f,.22f,.31f),new(.28f,.77f,.91f),new(.08f,.8f,1f),.08f),
            2=>new(new(.26f,.06f,.17f),new(.91f,.36f,.57f),new(1f,.18f,.55f),.06f),
            3=>new(new(.035f,.32f,.22f),new(.94f,.81f,.51f),new(.93f,.75f,.36f),.72f,.08f,.88f),
            4=>new(new(.32f,.15f,.075f),new(.94f,.57f,.31f),new(1f,.36f,.14f),.42f),
            5=>new(new(.94f,.93f,.88f),new(.97f,.89f,.65f),new(.94f,.80f,.48f),.52f,.16f,.86f),
            6=>new(new(.085f,.035f,.16f),new(.68f,.42f,.97f),new(.64f,.25f,1f),.04f),
            7=>new(new(.30f,.22f,.065f),new(1f,.87f,.25f),new(1f,.87f,.25f),1),
            _=>new(new(.13f,.16f,.20f),new(.97f,.80f,.40f),new(1f,.73f,.20f),1,.76f,.68f)
        };
    }
}
