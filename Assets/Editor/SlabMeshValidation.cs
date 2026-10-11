#if UNITY_EDITOR
using System;
using UnityEngine;
using Kamilunavo.PerfectDrop.Visuals;
using Kamilunavo.PerfectDrop.Gameplay;
namespace Kamilunavo.PerfectDrop.Editor
{
    public static class SlabMeshValidation
    {
        public static void Validate()
        {
            int checks=0;void Check(bool pass,string label){checks++;if(!pass)throw new InvalidOperationException("AUTHORED_SLAB: "+label);}
            var host=new GameObject("OriginalSlabFixture");
            try
            {
                foreach(float cut in new[]{.025f,.15f,1f,3.6f})
                {
                    var block=WorldArt.CreateStackBlock(host.transform,"ActualCut",Vector3.zero,new Vector3(cut,StackGame.LayerHeight,3.6f),0);
                    Check(block.GetComponentsInChildren<Renderer>().Length==6,"draw count unchanged for actual cut");
                    Check(block.GetComponentsInChildren<Collider>().Length==0,"visual mesh introduces no collision");
                    foreach(string part in new[]{"MetalDeck","GoldBand"})
                    {
                        var filter=block.transform.Find(part).GetComponent<MeshFilter>();var mesh=filter.sharedMesh;
                        Check(mesh.name.StartsWith("Authored"),"live cut uses original turned mesh: "+part);
                        Check(mesh.vertexCount<1000,"finite mobile mesh budget");
                        foreach(var v in mesh.vertices)Check(float.IsFinite(v.x)&&float.IsFinite(v.y)&&float.IsFinite(v.z)&&Mathf.Abs(v.x)<=.5001f&&Mathf.Abs(v.y)<=.5001f&&Mathf.Abs(v.z)<=.5001f,"local profile stays inside retained footprint");
                        var vertices=mesh.vertices;var normals=mesh.normals;var indices=mesh.triangles;
                        var uv=mesh.uv;
                        // Wide side faces must receive proportionate texture space,
                        // rather than the same UV interval as a tiny corner facet.
                        float longest=0,shortest=float.MaxValue;
                        for(int side=0;side<36;side++){int a=side*4;float span=uv[a+1].x-uv[a].x;Check(span>0,"strictly increasing continuous perimeter UV");longest=Mathf.Max(longest,span);shortest=Mathf.Min(shortest,span);}
                        Check(longest>shortest*40,"broad faces retain proportionate material detail");
                        for(int t=0;t<indices.Length;t+=3)
                        {
                            var a=indices[t];var b=indices[t+1];var c=indices[t+2];var face=Vector3.Cross(vertices[b]-vertices[a],vertices[c]-vertices[a]);
                            Check(face.sqrMagnitude>1e-12f,"no degenerate profile faces");
                            Check(Vector3.Dot(face, normals[a]+normals[b]+normals[c])>0,"outward winding agrees with material normals");
                            Check(Vector3.Dot(face.normalized,normals[a])>.999f,"actual chamfer normal agrees with geometric face");
                        }
                    }
                }
            }
            finally{UnityEngine.Object.DestroyImmediate(host);}
            Debug.Log("AUTHORED_SLAB_PASS checks="+checks);
        }
    }
}
#endif
