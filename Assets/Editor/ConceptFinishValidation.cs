#if UNITY_EDITOR
using System;
using UnityEngine;
using Kamilunavo.PerfectDrop.Visuals;
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
                    var b=WorldArt.CreateStackBlock(parent.transform,"IconBlock",Vector3.zero,new Vector3(width,.52f,3.6f),1);
                    Check(b.transform.Find("TopPlate").localScale.x/width>=.75f,"Icon plate must occupy a broad inset, not a small orange center.");
                    var mesh=b.transform.Find("MetalDeck").GetComponent<MeshFilter>().sharedMesh;
                    Check(b.transform.Find("MetalDeck").GetComponent<Renderer>().sharedMaterial.GetFloat("_BrushFinish")==1,"Metal shoulder lost its fine brush finish.");
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
            }
            finally { UnityEngine.Object.DestroyImmediate(parent); }
            Debug.Log("[PerfectDrop] Icon finish/live geometry PASS");
        }
        static void Check(bool condition,string reason){if(!condition)throw new InvalidOperationException("ICON_FINISH_FAIL "+reason);}
    }
}
#endif
