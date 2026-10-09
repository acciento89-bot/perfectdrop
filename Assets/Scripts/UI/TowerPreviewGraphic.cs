using UnityEngine;
using UnityEngine.UI;
namespace Kamilunavo.PerfectDrop.UI
{
    /// <summary>Dimensional specimens of the game's metal-and-light slab styles.</summary>
    public sealed class TowerPreviewGraphic : MaskableGraphic
    {
        public Color Accent=new Color(1,.79f,.16f);
        public int Floors=4;
        protected override void OnPopulateMesh(VertexHelper mesh)
        {
            mesh.Clear();var r=rectTransform.rect;
            var scale=Mathf.Min(r.width/1.25f,r.height/1.25f);
            var center=r.center+new Vector2(0,-scale*.24f);
            for(var i=0;i<Floors;i++)
            {
                var c=center+new Vector2(0,i*scale*.115f);
                var a=c+new Vector2(-scale*.47f,0);var b=c+new Vector2(-scale*.08f,-scale*.19f);
                var d=c+new Vector2(scale*.47f,scale*.045f);var e=c+new Vector2(scale*.08f,scale*.23f);
                var depth=Vector2.down*scale*.075f;
                Quad(mesh,a,b,b+depth,a+depth,new Color(.08f,.11f,.17f));
                Quad(mesh,b,d,d+depth,b+depth,new Color(.035f,.055f,.095f));
                Quad(mesh,a+depth*.65f,b+depth*.65f,b+depth*.82f,a+depth*.82f,Accent*.85f);
                Quad(mesh,b+depth*.65f,d+depth*.65f,d+depth*.82f,b+depth*.82f,Accent);
                Quad(mesh,a,b,d,e,new Color(.15f,.19f,.25f));
                var q=(a+b+d+e)*.25f;
                Quad(mesh,Vector2.Lerp(q,a,.81f),Vector2.Lerp(q,b,.81f),Vector2.Lerp(q,d,.81f),Vector2.Lerp(q,e,.81f),Accent);
                Quad(mesh,Vector2.Lerp(q,a,.74f),Vector2.Lerp(q,b,.74f),Vector2.Lerp(q,d,.74f),Vector2.Lerp(q,e,.74f),Color.Lerp(Accent,new Color(.20f,.22f,.25f),.2f));
                Quad(mesh,Vector2.Lerp(q,a,.70f),Vector2.Lerp(q,b,.70f),Vector2.Lerp(q,d,.70f),Vector2.Lerp(q,e,.70f),new Color(.35f,.28f,.12f));
            }
        }
        private static void Quad(VertexHelper mesh,Vector2 a,Vector2 b,Vector2 c,Vector2 d,Color tint)
        {
            var n=mesh.currentVertCount;mesh.AddVert(a,tint,Vector2.zero);mesh.AddVert(b,tint,Vector2.zero);mesh.AddVert(c,tint,Vector2.zero);mesh.AddVert(d,tint,Vector2.zero);mesh.AddTriangle(n,n+1,n+2);mesh.AddTriangle(n,n+2,n+3);
        }
        public static TowerPreviewGraphic Add(Transform parent,string name,Color tint,Vector2 min,Vector2 max,int floors=4)
        {
            var go=new GameObject(name,typeof(RectTransform),typeof(CanvasRenderer),typeof(TowerPreviewGraphic));go.transform.SetParent(parent,false);
            var rect=go.GetComponent<RectTransform>();rect.anchorMin=min;rect.anchorMax=max;rect.offsetMin=rect.offsetMax=Vector2.zero;
            var graphic=go.GetComponent<TowerPreviewGraphic>();graphic.Accent=tint;graphic.Floors=floors;graphic.raycastTarget=false;return graphic;
        }
    }
}
