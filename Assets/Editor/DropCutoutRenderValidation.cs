#if UNITY_EDITOR
using System;
using UnityEngine;
using Kamilunavo.PerfectDrop.Visuals;
namespace Kamilunavo.PerfectDrop.Editor
{
    // Check real imported alpha and actual rendered cutout/blend shaders; a
    // solid rectangular placeholder or invisible card cannot satisfy this.
    public static class DropCutoutRenderValidation
    {
        public static void Validate()
        {
            var host=new GameObject("OriginalCutoutRenderFixture");var owner=host.AddComponent<TerraceArtOwner>();
            var cameraNode=new GameObject("CutoutProbeCamera");var camera=cameraNode.AddComponent<Camera>();
            camera.enabled=false;camera.cullingMask=1<<27;camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=Color.magenta;camera.orthographic=true;camera.orthographicSize=6;
            var target=new RenderTexture(192,192,24);target.Create();camera.targetTexture=target;
            var pixels=new Texture2D(192,192,TextureFormat.RGBA32,false);var old=RenderTexture.active;
            int checks=0;
            try
            {
                foreach(string textureName in new[]{"AuthoredCypressCutout","AuthoredCloudCutout"})
                {
                    var texture=Resources.Load<Texture2D>("Art/"+textureName);if(texture==null||texture.width>1024||texture.height>1024)throw new InvalidOperationException("Original cutout is missing or exceeds its mobile texture budget: "+textureName);
                    Graphics.Blit(texture,target);RenderTexture.active=target;pixels.ReadPixels(new Rect(0,0,192,192),0,0);pixels.Apply();int transparent=0,opaque=0;
                    foreach(var c in pixels.GetPixels32()){if(c.a<16)transparent++;if(c.a>180)opaque++;}
                    if(transparent<192*192*.15f||opaque<192*192*.03f)throw new InvalidOperationException("Imported original alpha was lost: "+textureName);checks++;
                }
                CloudCardGeometry.Build(host.transform,0,owner);
                var cloud=host.transform.Find("TerraceCloudCards");cloud.gameObject.layer=27;
                var source=cloud.GetComponent<MeshFilter>().sharedMesh;var card=UnityEngine.Object.Instantiate(source);owner.Own(card);
                // Isolate one actual world-anchored cloud's geometry/size channels.
                var v=source.vertices;var uv=source.uv;var uv2=source.uv2;var n=source.normals;
                card.Clear();card.vertices=new[]{v[0],v[1],v[2],v[3]};card.uv=new[]{uv[0],uv[1],uv[2],uv[3]};card.uv2=new[]{uv2[0],uv2[1],uv2[2],uv2[3]};card.normals=new[]{n[0],n[1],n[2],n[3]};card.triangles=new[]{0,1,2,0,2,3};card.bounds=new Bounds(new Vector3(14,2.5f,0),Vector3.one*30);cloud.GetComponent<MeshFilter>().sharedMesh=card;
                foreach(float heading in new[]{0f,90f,180f})
                {
                    var center=new Vector3(14,2.5f,0);camera.transform.SetPositionAndRotation(center+Quaternion.Euler(0,heading,0)*Vector3.back*20,Quaternion.Euler(0,heading,0));
                    camera.Render();RenderTexture.active=target;pixels.ReadPixels(new Rect(0,0,192,192),0,0);pixels.Apply();int visible=0,clear=0;
                    foreach(var c in pixels.GetPixels32()){if(c.g>16)visible++;if(c.r>240&&c.g<8&&c.b>240)clear++;}
                    if(visible<192*192*.08f||clear<192*192*.25f)throw new InvalidOperationException("Actual cloud card has lost its soft cutout or disappears during orbit at heading="+heading);checks++;
                }
                var retained=cloud.GetComponent<Renderer>().sharedMaterial;UnityEngine.Object.DestroyImmediate(host);host=null;
                if(source!=null||card!=null||retained!=null)throw new InvalidOperationException("Original cloud resources leaked after owned fixture teardown");checks++;
            }
            finally
            {
                camera.targetTexture=null;RenderTexture.active=old;target.Release();UnityEngine.Object.DestroyImmediate(target);UnityEngine.Object.DestroyImmediate(pixels);UnityEngine.Object.DestroyImmediate(cameraNode);if(host!=null)UnityEngine.Object.DestroyImmediate(host);
            }
            Debug.Log("DROP_ORIGINAL_CUTOUT_RENDER_PASS checks="+checks);
        }
    }
}
#endif
