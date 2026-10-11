#if UNITY_EDITOR
using System;
using System.IO;
using UnityEngine;
using Kamilunavo.PerfectDrop.Visuals;
namespace Kamilunavo.PerfectDrop.Editor
{
    public static class DropCompassRenderValidation
    {
        public static void Diagnose()
        {
            string folder=Environment.GetEnvironmentVariable("DROP_COMPASS_QA_OUT");
            if(string.IsNullOrEmpty(folder))throw new InvalidOperationException("Compass diagnostic output is required.");
            Directory.CreateDirectory(folder);
            var host=new GameObject("CompassRenderDiagnostic");
            var dais=WorldArt.BuildStackDais(host.transform);
            var drum=dais.transform.Find("NavyDrum").GetComponent<MeshRenderer>();
            foreach(var r in dais.GetComponentsInChildren<Renderer>(true))r.enabled=r==drum;
            drum.gameObject.layer=27;
            var node=new GameObject("CompassDiagnosticCamera");var camera=node.AddComponent<Camera>();
            camera.enabled=false;camera.cullingMask=1<<27;camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=Color.magenta;
            camera.orthographic=true;camera.orthographicSize=3.6f;camera.transform.SetPositionAndRotation(drum.bounds.center+Vector3.up*10,Quaternion.Euler(90,0,0));
            var target=new RenderTexture(512,512,24);target.Create();camera.targetTexture=target;
            var pixels=new Texture2D(512,512,TextureFormat.RGB24,false);var old=RenderTexture.active;
            var original=drum.sharedMaterial;var unlit=new Material(Shader.Find("Unlit/Texture"));unlit.mainTexture=original.mainTexture;
            try
            {
                foreach(var pair in new[]{("unlit",unlit),("surface",original)})
                {
                    drum.sharedMaterial=pair.Item2;camera.Render();RenderTexture.active=target;pixels.ReadPixels(new Rect(0,0,512,512),0,0);pixels.Apply();
                    File.WriteAllBytes(Path.Combine(folder,"compass-"+pair.Item1+".png"),pixels.EncodeToPNG());
                }
                var mesh=drum.GetComponent<MeshFilter>().sharedMesh;var v=mesh.vertices;var n=mesh.normals;var uv=mesh.uv;var tr=mesh.triangles;
                using(var writer=new StreamWriter(Path.Combine(folder,"compass-mesh.txt")))
                {for(int i=0;i<v.Length;i++)writer.WriteLine(i+" "+v[i].ToString("R")+" normal="+n[i].ToString("R")+" uv="+uv[i].ToString("R"));for(int i=0;i<tr.Length;i+=3)writer.WriteLine("triangle "+tr[i]+" "+tr[i+1]+" "+tr[i+2]);}
            }
            finally
            {
                drum.sharedMaterial=original;camera.targetTexture=null;RenderTexture.active=old;target.Release();UnityEngine.Object.DestroyImmediate(target);UnityEngine.Object.DestroyImmediate(pixels);UnityEngine.Object.DestroyImmediate(unlit);UnityEngine.Object.DestroyImmediate(node);UnityEngine.Object.DestroyImmediate(host);
            }
            Debug.Log("DROP_COMPASS_DIAGNOSTIC_CAPTURED");
        }
    }
}
#endif
