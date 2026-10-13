using UnityEngine;
using UnityEngine.UI;

namespace Kamilunavo.PerfectDrop.UI
{
    public enum HudIconType
    {
        Floors,
        Crown,
        Flame,
        Diamond,
        Bolt,
        ArrowUp,
        Star,
        Lock,
        Clock,
        Target,
        Gear,
        Coin
    }

    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class HudIconGraphic : MaskableGraphic
    {
        public HudIconType IconType;
        private static Texture2D _coin;
        public override Texture mainTexture=>IconType==HudIconType.Coin?(_coin!=null?_coin:(_coin=Resources.Load<Texture2D>("Art/ConceptV2/GoldCoin")))??base.mainTexture:base.mainTexture;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            switch (IconType)
            {
                case HudIconType.Coin:
                    if(mainTexture!=base.mainTexture){var bounds=GetPixelAdjustedRect();float side=Mathf.Min(bounds.width,bounds.height);var origin=bounds.center-Vector2.one*side*.5f;vh.AddVert(origin,Color.white,Vector2.zero);vh.AddVert(origin+Vector2.up*side,Color.white,Vector2.up);vh.AddVert(origin+Vector2.one*side,Color.white,Vector2.one);vh.AddVert(origin+Vector2.right*side,Color.white,Vector2.right);vh.AddTriangle(0,1,2);vh.AddTriangle(0,2,3);break;}
                    for(int i=0;i<32;i++)
                    {
                        float a=i*Mathf.PI*2/32,b=(i+1)*Mathf.PI*2/32;
                        AddThickLine(vh,P(.5f+Mathf.Cos(a)*.37f,.5f+Mathf.Sin(a)*.37f),P(.5f+Mathf.Cos(b)*.37f,.5f+Mathf.Sin(b)*.37f),.09f);
                        AddThickLine(vh,P(.5f+Mathf.Cos(a)*.23f,.5f+Mathf.Sin(a)*.23f),P(.5f+Mathf.Cos(b)*.23f,.5f+Mathf.Sin(b)*.23f),.025f);
                    }
                    AddThickLine(vh,P(.5f,.34f),P(.5f,.66f),.075f);
                    break;
                case HudIconType.Gear:
                    for(int i=0;i<32;i++){float a=i*Mathf.PI*2/32,b=(i+1)*Mathf.PI*2/32;AddThickLine(vh,P(.5f+Mathf.Cos(a)*.26f,.5f+Mathf.Sin(a)*.26f),P(.5f+Mathf.Cos(b)*.26f,.5f+Mathf.Sin(b)*.26f),.10f);}
                    for(int i=0;i<8;i++){float a=i*Mathf.PI/4;AddThickLine(vh,P(.5f+Mathf.Cos(a)*.26f,.5f+Mathf.Sin(a)*.26f),P(.5f+Mathf.Cos(a)*.43f,.5f+Mathf.Sin(a)*.43f),.10f);}
                    break;
                case HudIconType.Clock:
                case HudIconType.Target:
                    for(int i=0;i<32;i++){float a=i*Mathf.PI*2/32,b=(i+1)*Mathf.PI*2/32;AddThickLine(vh,P(.5f+Mathf.Cos(a)*.35f,.5f+Mathf.Sin(a)*.35f),P(.5f+Mathf.Cos(b)*.35f,.5f+Mathf.Sin(b)*.35f),.045f);}
                    if(IconType==HudIconType.Clock){AddThickLine(vh,P(.5f,.5f),P(.5f,.76f),.055f);AddThickLine(vh,P(.5f,.5f),P(.70f,.38f),.055f);}
                    else{AddThickLine(vh,P(.5f,.08f),P(.5f,.92f),.04f);AddThickLine(vh,P(.08f,.5f),P(.92f,.5f),.04f);}
                    break;
                case HudIconType.Star:
                    for (var n = 0; n < 10; n++)
                    {
                        var a = (90+n*36)*Mathf.Deg2Rad;
                        var b = (90+(n+1)*36)*Mathf.Deg2Rad;
                        var ra = n%2==0?.40f:.18f;
                        var rb = n%2==0?.18f:.40f;
                        AddTriangle(vh,P(.5f,.5f),P(.5f+Mathf.Cos(a)*ra,.5f+Mathf.Sin(a)*ra),P(.5f+Mathf.Cos(b)*rb,.5f+Mathf.Sin(b)*rb));
                    }
                    break;
                case HudIconType.Lock:
                    AddQuad(vh,P(.23f,.16f),P(.77f,.16f),P(.77f,.58f),P(.23f,.58f));
                    AddThickLine(vh,P(.32f,.56f),P(.32f,.78f),.09f);
                    AddThickLine(vh,P(.32f,.78f),P(.50f,.86f),.09f);
                    AddThickLine(vh,P(.50f,.86f),P(.68f,.78f),.09f);
                    AddThickLine(vh,P(.68f,.78f),P(.68f,.56f),.09f);
                    break;
                case HudIconType.Floors:
                    AddLayer(vh, 0.50f, 0.68f, 0.32f, 0.15f);
                    AddLayer(vh, 0.50f, 0.49f, 0.32f, 0.15f);
                    AddLayer(vh, 0.50f, 0.30f, 0.32f, 0.15f);
                    break;

                case HudIconType.Crown:
                    AddQuad(vh, P(0.18f, 0.24f), P(0.82f, 0.24f), P(0.76f, 0.38f), P(0.24f, 0.38f));
                    AddTriangle(vh, P(0.24f, 0.38f), P(0.15f, 0.78f), P(0.42f, 0.47f));
                    AddTriangle(vh, P(0.34f, 0.42f), P(0.50f, 0.86f), P(0.63f, 0.42f));
                    AddTriangle(vh, P(0.58f, 0.47f), P(0.85f, 0.78f), P(0.76f, 0.38f));
                    break;

                case HudIconType.Flame:
                    AddTriangle(vh, P(0.25f, 0.25f), P(0.50f, 0.88f), P(0.63f, 0.30f));
                    AddTriangle(vh, P(0.37f, 0.24f), P(0.78f, 0.72f), P(0.70f, 0.25f));
                    AddTriangle(vh, P(0.32f, 0.22f), P(0.62f, 0.22f), P(0.50f, 0.55f));
                    break;

                case HudIconType.Diamond:
                    AddThickLine(vh, P(0.50f, 0.86f), P(0.84f, 0.50f), 0.075f);
                    AddThickLine(vh, P(0.84f, 0.50f), P(0.50f, 0.14f), 0.075f);
                    AddThickLine(vh, P(0.50f, 0.14f), P(0.16f, 0.50f), 0.075f);
                    AddThickLine(vh, P(0.16f, 0.50f), P(0.50f, 0.86f), 0.075f);
                    break;

                case HudIconType.Bolt:
                    AddTriangle(vh, P(0.54f, 0.92f), P(0.24f, 0.48f), P(0.48f, 0.48f));
                    AddTriangle(vh, P(0.46f, 0.08f), P(0.76f, 0.55f), P(0.52f, 0.55f));
                    AddQuad(vh, P(0.45f, 0.44f), P(0.60f, 0.44f), P(0.54f, 0.61f), P(0.39f, 0.61f));
                    break;

                case HudIconType.ArrowUp:
                    AddTriangle(vh, P(0.50f, 0.86f), P(0.18f, 0.48f), P(0.82f, 0.48f));
                    AddQuad(vh, P(0.38f, 0.16f), P(0.62f, 0.16f), P(0.62f, 0.54f), P(0.38f, 0.54f));
                    break;
            }
        }

        private static Vector2 P(float x, float y) => new(x, y);

        private void AddLayer(VertexHelper vh, float cx, float cy, float halfWidth, float halfHeight)
        {
            AddQuad(
                vh,
                P(cx, cy + halfHeight),
                P(cx + halfWidth, cy),
                P(cx, cy - halfHeight),
                P(cx - halfWidth, cy));
        }

        private void AddThickLine(VertexHelper vh, Vector2 a, Vector2 b, float width)
        {
            var dir = (b - a).normalized;
            var normal = new Vector2(-dir.y, dir.x) * width * 0.5f;
            AddQuad(vh, a - normal, a + normal, b + normal, b - normal);
        }

        private void AddTriangle(VertexHelper vh, Vector2 a, Vector2 b, Vector2 c)
        {
            var i = vh.currentVertCount;
            vh.AddVert(ToLocal(a), color, Vector2.zero);
            vh.AddVert(ToLocal(b), color, Vector2.zero);
            vh.AddVert(ToLocal(c), color, Vector2.zero);
            vh.AddTriangle(i, i + 1, i + 2);
        }

        private void AddQuad(VertexHelper vh, Vector2 a, Vector2 b, Vector2 c, Vector2 d)
        {
            var i = vh.currentVertCount;
            vh.AddVert(ToLocal(a), color, Vector2.zero);
            vh.AddVert(ToLocal(b), color, Vector2.zero);
            vh.AddVert(ToLocal(c), color, Vector2.zero);
            vh.AddVert(ToLocal(d), color, Vector2.zero);
            vh.AddTriangle(i, i + 1, i + 2);
            vh.AddTriangle(i, i + 2, i + 3);
        }

        private Vector3 ToLocal(Vector2 normalized)
        {
            var r = rectTransform.rect;
            var size=Mathf.Min(r.width,r.height);
            return new Vector3(r.center.x+(normalized.x-.5f)*size,
                r.center.y+(normalized.y-.5f)*size,0f);
        }
    }
}
