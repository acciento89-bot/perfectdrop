using UnityEngine;
using UnityEngine.UI;
namespace Kamilunavo.PerfectDrop.UI
{
    /// <summary>Dimensional specimens of the game's metal-and-light slab styles.</summary>
    public sealed class TowerPreviewGraphic : RawImage
    {
        public Color Accent=new Color(1,.79f,.16f);
        public Color Body=new Color(.12f,.16f,.23f);
        public Color Plate=new Color(1,.78f,.30f);
        public int Floors=4;
        private int _style,_cachedStyle=-1,_cachedFloors=-1;
        private void LateUpdate()
        {
            if(!Application.isPlaying)return;
            var floors=Mathf.Clamp(Floors,1,8);
            if(_cachedStyle==_style && _cachedFloors==floors)return;
            ReleaseSpecimen();
            texture=Kamilunavo.PerfectDrop.Visuals.StackSpecimenStudio.Acquire(_style,floors);
            _cachedStyle=_style;_cachedFloors=floors;SetVerticesDirty();
        }
        protected override void OnDestroy(){ReleaseSpecimen();base.OnDestroy();}
        private void ReleaseSpecimen()
        {
            texture=null;
            if(_cachedStyle>=0)Kamilunavo.PerfectDrop.Visuals.StackSpecimenStudio.Release(_cachedStyle,_cachedFloors);
            _cachedStyle=_cachedFloors=-1;
        }
        protected override void OnPopulateMesh(VertexHelper mesh)
        {
            if(texture!=null)
            {
                mesh.Clear();var bounds=rectTransform.rect;
                var imageScale=Mathf.Min(bounds.width/texture.width,bounds.height/texture.height);
                var half=new Vector2(texture.width,texture.height)*imageScale*.5f;var c=bounds.center;
                mesh.AddVert(c-half,Color.white,new Vector2(0,0));mesh.AddVert(c+new Vector2(-half.x,half.y),Color.white,new Vector2(0,1));
                mesh.AddVert(c+half,Color.white,new Vector2(1,1));mesh.AddVert(c+new Vector2(half.x,-half.y),Color.white,new Vector2(1,0));
                mesh.AddTriangle(0,1,2);mesh.AddTriangle(0,2,3);return;
            }
            mesh.Clear();var r=rectTransform.rect;
            var floors=Mathf.Clamp(Floors,1,8);
            var scale=Mathf.Min(r.width/1.12f,r.height/(.64f+(floors-1)*.145f));
            var center=r.center+Vector2.down*((floors-1)*.145f*.5f-.015f)*scale;
            var dark=Color.Lerp(Body,new Color(.025f,.04f,.075f),.56f);
            var warm=Color.Lerp(Accent,new Color(1,.94f,.71f),.35f);
            var plateLight=Color.Lerp(Plate,new Color(1,.97f,.79f),.60f);
            for(var i=0;i<floors;i++)
            {
                var c=center+Vector2.up*(i*scale*.145f);
                var a=c+new Vector2(-scale*.47f,0);var b=c+new Vector2(-scale*.08f,-scale*.19f);
                var d=c+new Vector2(scale*.47f,scale*.045f);var e=c+new Vector2(scale*.08f,scale*.23f);
                var depth=Vector2.down*scale*.105f;
                Quad(mesh,a,b,b+depth,a+depth,Color.Lerp(Body,Color.white,.10f),Body,Body,dark);
                Quad(mesh,b,d,d+depth,b+depth,Body,dark,dark,Shade(Body,.72f));
                Quad(mesh,a+depth*.70f,b+depth*.70f,b+depth*.91f,a+depth*.91f,warm,Accent,Shade(Accent,.65f),Shade(Accent,.77f));
                Quad(mesh,b+depth*.70f,d+depth*.70f,d+depth*.91f,b+depth*.91f,Accent,warm,Shade(Accent,.82f),Shade(Accent,.65f));
                // A broad metal shoulder and bright inset carry the game's slab silhouette.
                Quad(mesh,a,b,d,e,Body,Shade(Body,.72f),Color.Lerp(Body,Color.white,.18f),Color.Lerp(Body,Color.white,.30f));
                var q=(a+b+d+e)*.25f;
                var pa=Vector2.Lerp(q,a,.70f);var pb=Vector2.Lerp(q,b,.70f);var pd=Vector2.Lerp(q,d,.70f);var pe=Vector2.Lerp(q,e,.70f);
                Quad(mesh,pa,pb,pd,pe,warm,Shade(Accent,.83f),Accent,warm);
                Quad(mesh,Vector2.Lerp(q,a,.64f),Vector2.Lerp(q,b,.64f),Vector2.Lerp(q,d,.64f),Vector2.Lerp(q,e,.64f),Shade(Plate,.84f),Color.Lerp(Plate,new Color(.48f,.24f,.045f),.42f),plateLight,Color.Lerp(plateLight,Color.white,.25f));
                // Fine upper lip, kept inside the footprint and drawn with vertex gradients.
                Quad(mesh,a,b,Vector2.Lerp(b,q,.025f),Vector2.Lerp(a,q,.025f),new Color(.38f,.44f,.54f),Body,Body,Body);
                Quad(mesh,b,d,Vector2.Lerp(d,q,.025f),Vector2.Lerp(b,q,.025f),Body,new Color(.51f,.57f,.63f),Body,Body);
            }
        }
        private static Color Shade(Color color,float amount) => new Color(color.r*amount,color.g*amount,color.b*amount,1);
        private static void Quad(VertexHelper mesh,Vector2 a,Vector2 b,Vector2 c,Vector2 d,Color ca,Color cb,Color cc,Color cd)
        {
            var n=mesh.currentVertCount;mesh.AddVert(a,ca,Vector2.zero);mesh.AddVert(b,cb,Vector2.zero);mesh.AddVert(c,cc,Vector2.zero);mesh.AddVert(d,cd,Vector2.zero);mesh.AddTriangle(n,n+1,n+2);mesh.AddTriangle(n,n+2,n+3);
        }
        public void ApplyStyle(int style)
        {
            _style=Mathf.Clamp(style,0,7);var finish=Kamilunavo.PerfectDrop.Visuals.StackStylePalette.Get(_style);Accent=finish.Accent;Body=finish.Body;Plate=finish.Plate;SetVerticesDirty();
        }
        public static TowerPreviewGraphic AddStyle(Transform parent,string name,int style,Vector2 min,Vector2 max,int floors=4)
        {
            var graphic=Add(parent,name,Color.white,min,max,floors);graphic.ApplyStyle(style);return graphic;
        }
        public static TowerPreviewGraphic Add(Transform parent,string name,Color tint,Vector2 min,Vector2 max,int floors=4)
        {
            var go=new GameObject(name,typeof(RectTransform),typeof(CanvasRenderer),typeof(TowerPreviewGraphic));go.transform.SetParent(parent,false);
            var rect=go.GetComponent<RectTransform>();rect.anchorMin=min;rect.anchorMax=max;rect.offsetMin=rect.offsetMax=Vector2.zero;
            var graphic=go.GetComponent<TowerPreviewGraphic>();graphic.Accent=tint;graphic.Floors=floors;graphic.raycastTarget=false;return graphic;
        }
    }
}
