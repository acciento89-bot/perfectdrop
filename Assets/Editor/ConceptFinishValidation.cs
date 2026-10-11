#if UNITY_EDITOR
using System;
using UnityEngine;
using Kamilunavo.PerfectDrop.Visuals;
using Kamilunavo.PerfectDrop.Gameplay;
namespace Kamilunavo.PerfectDrop.Editor
{
    public static class ConceptFinishValidation
    {
        public static void Validate()
        {
            var gold=StackStylePalette.Get(0);
            Check(gold.Body.maxColorComponent<=.22f,"Icon finish needs dark navy metal shoulders, not bright slate.");
            Check(gold.Metallic>=.65f && gold.Smoothness>=.65f,"Icon metal must have a polished reflective finish.");
            var parent=new GameObject("IconFinishFixture");
            try
            {
                foreach(var width in new[]{3.6f,.035f})
                {
                    var b=WorldArt.CreateStackBlock(parent.transform,"IconBlock",Vector3.zero,new Vector3(width,StackGame.LayerHeight,3.6f),1);
                    Check(b.transform.Find("TopPlate").localScale.x/width>=.75f,"Icon plate must occupy a broad inset, not a small orange center.");
                    var mesh=b.transform.Find("MetalDeck").GetComponent<MeshFilter>().sharedMesh;
                    Check(b.transform.Find("MetalDeck").GetComponent<Renderer>().sharedMaterial.GetFloat("_BrushFinish")==1,"Metal shoulder lost its fine brush finish.");
                    var shoulder=b.transform.Find("MetalDeck").GetComponent<Renderer>().sharedMaterial;
                    Check(shoulder.HasProperty("_MicroRelief")&&shoulder.GetFloat("_MicroRelief")>.5f,"Original brushed shoulder needs actual surface relief, not a flat tint.");
                    var authored=Resources.Load<Texture2D>("Art/AuthoredBrushedMetalAlbedo");
                    Check(authored!=null&&authored.width==256&&authored.height==256,"Original metal tile is imported within its mobile budget.");
                    Check(shoulder.GetTexture("_MainTex")==authored,"Live shoulder consumes the original metal tile.");
                    Check(b.transform.Find("TopPlate").GetComponent<Renderer>().sharedMaterial.GetFloat("_MicroRelief")==0,"Gold plate must retain its separate finish.");
                    foreach(var part in b.GetComponentsInChildren<MeshRenderer>())if(part.sharedMaterial.GetFloat("_MicroRelief")>.5f)
                    {
                        var lit=part.GetComponent<MeshFilter>().sharedMesh;
                        Check(lit.tangents.Length==lit.vertexCount,"Every relief-enabled shoulder has a tangent basis: "+part.name);
                        foreach(var tangent in lit.tangents)Check(float.IsFinite(tangent.x)&&float.IsFinite(tangent.y)&&float.IsFinite(tangent.z)&&new Vector3(tangent.x,tangent.y,tangent.z).sqrMagnitude>.5f,"Finite nonzero tangent basis: "+part.name);
                    }
                    Check(mesh.vertexCount>=200,"Icon block needs rounded bevel corners rather than eight hard chamfers.");
                    Check(b.GetComponentsInChildren<Renderer>().Length==6,"Live block renderer budget changed.");
                    foreach(var vertex in mesh.vertices)Check(float.IsFinite(vertex.x)&&float.IsFinite(vertex.y)&&float.IsFinite(vertex.z),"Nonfinite mesh coordinate.");
                    foreach(var normal in mesh.normals)Check(float.IsFinite(normal.x)&&float.IsFinite(normal.y)&&float.IsFinite(normal.z),"Nonfinite mesh normal.");
                    WorldArt.StyleStackBlock(b,0);StackStyleValidation.ValidateBlock(b,0);
                    foreach(var style in new[]{3,5,0})
                    {
                        WorldArt.StyleStackBlock(b,style);var paint=new MaterialPropertyBlock();b.transform.Find("MetalDeck").GetComponent<Renderer>().GetPropertyBlock(paint);
                        Check(paint.GetFloat("_BodyFinish")== (style==3?1:style==5?2:0),"Selected lacquer/ceramic surface detail failed to apply or clear.");
                    }
                }
                var dais=WorldArt.BuildStackDais(parent.transform);
                Check(dais.transform.Find("StoneDrum").GetComponent<Renderer>().sharedMaterial.GetFloat("_BrushFinish")==0,"Stone dais must not inherit radial metal brush stripes.");
                var inlay=dais.transform.Find("CompassInlay").GetComponent<MeshFilter>().sharedMesh;
                foreach(var normal in inlay.normals)Check(normal.y>.99f,"Compass intarsia faces away from the gameplay camera.");
                Check(inlay.name=="AuthoredGoldCompassRose"&&inlay.vertexCount>950&&inlay.vertexCount<1300,"Real dais includes the bounded original rose and gold perimeter ticks.");
                var points=inlay.vertices;var indices=inlay.triangles;
                Vector2 P(int i)=>new Vector2(points[indices[i]].x,points[indices[i]].z);
                for(int a=0;a<indices.Length;a+=3)for(int b=a+3;b<indices.Length;b+=3)
                    Check(!Overlapping(P(a),P(a+1),P(a+2),P(b),P(b+1),P(b+2)),"Gold intarsia must not overlap coplanar faces with different finish coordinates.");
            }
            finally { UnityEngine.Object.DestroyImmediate(parent); }
            Debug.Log("[PerfectDrop] Icon finish/live geometry PASS");
        }
        static void Check(bool condition,string reason){if(!condition)throw new InvalidOperationException("ICON_FINISH_FAIL "+reason);}
        static bool Overlapping(Vector2 a,Vector2 b,Vector2 c,Vector2 p,Vector2 q,Vector2 r)
        {
            bool Separated(Vector2 axis)
            {
                float a0=Vector2.Dot(a,axis),a1=Vector2.Dot(b,axis),a2=Vector2.Dot(c,axis);
                float b0=Vector2.Dot(p,axis),b1=Vector2.Dot(q,axis),b2=Vector2.Dot(r,axis);
                return Mathf.Min(Mathf.Max(a0,Mathf.Max(a1,a2)),Mathf.Max(b0,Mathf.Max(b1,b2)))-Mathf.Max(Mathf.Min(a0,Mathf.Min(a1,a2)),Mathf.Min(b0,Mathf.Min(b1,b2)))<=1e-5f*axis.magnitude;
            }
            Vector2 Cross(Vector2 edge)=>new Vector2(-edge.y,edge.x);
            return !Separated(Vector2.right)&&!Separated(Vector2.up)&&!Separated(Cross(b-a))&&!Separated(Cross(c-b))&&!Separated(Cross(a-c))&&!Separated(Cross(q-p))&&!Separated(Cross(r-q))&&!Separated(Cross(p-r));
        }
    }
}
#endif
