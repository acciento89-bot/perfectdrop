using UnityEngine;
namespace Kamilunavo.PerfectDrop.Visuals {
    // One finish definition feeds the real shader overrides and menu specimens.
    public readonly struct StackStyleFinish {
        public readonly Color Body,Plate,Accent;
        public readonly float GoldFinish,Metallic,Smoothness;
        public StackStyleFinish(Color body,Color plate,Color accent,float goldFinish=0,float metallic=.72f,float smoothness=.62f){Body=body;Plate=plate;Accent=accent;GoldFinish=goldFinish;Metallic=metallic;Smoothness=smoothness;}
    }
    public static class StackStylePalette {
        public static StackStyleFinish Get(int style)=>Mathf.Clamp(style,0,7) switch {
            1=>new(new(.07f,.22f,.31f),new(.28f,.77f,.91f),new(.08f,.8f,1f),.08f),
            2=>new(new(.26f,.06f,.17f),new(.91f,.36f,.57f),new(1f,.18f,.55f),.06f),
            3=>new(new(.07f,.22f,.14f),new(.34f,.79f,.49f),new(.2f,1f,.5f),.08f),
            4=>new(new(.32f,.15f,.075f),new(.94f,.57f,.31f),new(1f,.36f,.14f),.42f),
            5=>new(new(.73f,.83f,.88f),new(.76f,.91f,.96f),new(.75f,.93f,1f),0,.20f,.85f),
            6=>new(new(.085f,.035f,.16f),new(.68f,.42f,.97f),new(.64f,.25f,1f),.04f),
            7=>new(new(.30f,.22f,.065f),new(1f,.87f,.25f),new(1f,.87f,.25f),1),
            _=>new(new(.30f,.35f,.44f),new(1f,.80f,.35f),new(1f,.55f,.06f),1,.48f,.46f)
        };
    }
}
