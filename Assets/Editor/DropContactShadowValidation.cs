#if UNITY_EDITOR
using System;
using UnityEngine;
using Kamilunavo.PerfectDrop.Visuals;
namespace Kamilunavo.PerfectDrop.Editor
{
    public static class DropContactShadowValidation
    {
        public static void Validate()
        {
            var root=new GameObject("ContactShadowRenderProbe");root.transform.position=Vector3.down*2200;
            var cameraObject=new GameObject("ProbeCamera");cameraObject.transform.SetParent(root.transform,false);
            var camera=cameraObject.AddComponent<Camera>();camera.enabled=false;camera.cullingMask=1<<29;
            camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=new Color(.3f,.35f,.4f);
            camera.nearClipPlane=.1f;camera.farClipPlane=30;camera.fieldOfView=43;camera.aspect=1;camera.allowHDR=false;
            camera.transform.position=root.transform.position+new Vector3(-7,6,-10);camera.transform.LookAt(root.transform.position);
            var target=new RenderTexture(256,256,24,RenderTextureFormat.ARGBHalf){antiAliasing=4};target.Create();camera.targetTexture=target;
            var pixels=new Texture2D(256,256,TextureFormat.RGBA32,false);
            var material=new Material(Resources.Load<Shader>("PerfectDropSurface")){color=new Color(.78f,.74f,.65f)};
            var previous=RenderTexture.active;
            try
            {
                foreach(var box in new[]{new Bounds(new Vector3(0,-1,0),new Vector3(10,.4f,10)),new Bounds(new Vector3(-1,1,0),new Vector3(2,4,2)),new Bounds(new Vector3(1,0,1),new Vector3(2,2,2))})
                {
                    var block=GameObject.CreatePrimitive(PrimitiveType.Cube);block.transform.SetParent(root.transform,false);
                    block.transform.localPosition=box.center;block.transform.localScale=box.size;block.layer=29;
                    block.GetComponent<MeshRenderer>().sharedMaterial=material;
                    UnityEngine.Object.DestroyImmediate(block.GetComponent<Collider>());
                }
                var light=root.AddComponent<Light>();light.type=LightType.Directional;light.cullingMask=1<<29;light.intensity=.8f;light.shadows=LightShadows.None;
                light.transform.rotation=Quaternion.Euler(40,-25,0);
                var grade=cameraObject.AddComponent<CinematicGrade>();grade.Vignette=0;grade.Contrast=1;grade.Saturation=1;grade.Warmth=0;
                foreach(bool orthographic in new[]{false,true})
                {
                camera.orthographic=orthographic;camera.orthographicSize=6;camera.ResetProjectionMatrix();var projection=camera.projectionMatrix;
                if(orthographic){projection.m03=.08f;projection.m13=-.12f;}else{projection.m02=.08f;projection.m12=-.12f;}camera.projectionMatrix=projection;
                grade.ContactShadows=false;camera.Render();RenderTexture.active=target;pixels.ReadPixels(new Rect(0,0,256,256),0,0);pixels.Apply();var before=pixels.GetPixels();
                grade.ContactShadows=true;camera.Render();RenderTexture.active=target;pixels.ReadPixels(new Rect(0,0,256,256),0,0);pixels.Apply();var after=pixels.GetPixels();
                double difference=0;int darkened=0;
                for(int i=0;i<before.Length;i++)
                {
                    if(!float.IsFinite(after[i].r)||!float.IsFinite(after[i].g)||!float.IsFinite(after[i].b))throw new InvalidOperationException("Contact shadow output contains nonfinite pixels");
                    double delta=(before[i].r-after[i].r+before[i].g-after[i].g+before[i].b-after[i].b)/3;difference+=delta;if(delta>.012)darkened++;
                }
                difference/=before.Length;
                if(difference<.0007 || difference>.04 || darkened<40)throw new InvalidOperationException("Real corner geometry has missing/excessive contact shading: mean="+difference+" shadedPixels="+darkened);
                // A flat exposed face away from all contacts must not self-occlude.
                var face=camera.WorldToViewportPoint(root.transform.TransformPoint(new Vector3(-1,1,-1.01f)));
                int faceX=Mathf.Clamp(Mathf.RoundToInt(face.x*255),1,254),faceY=Mathf.Clamp(Mathf.RoundToInt(face.y*255),1,254);
                for(int y=faceY-1;y<=faceY+1;y++)for(int x=faceX-1;x<=faceX+1;x++)
                {int i=y*256+x;if((before[i]-after[i]).maxColorComponent>.03f)throw new InvalidOperationException("Exposed flat facade self-occludes");}
                for(int y=245;y<256;y++)for(int x=0;x<256;x++)
                {
                    int i=y*256+x;if((before[i]-after[i]).maxColorComponent>.01f)throw new InvalidOperationException("Contact shadows altered empty upper background");
                }
                Debug.Log("DROP_CONTACT_SHADOW_PASS actualPixels="+before.Length+" meanDarkening="+difference+" cornerPixels="+darkened+" orthographic="+orthographic);
                }
            }
            finally
            {
                camera.targetTexture=null;RenderTexture.active=previous;target.Release();UnityEngine.Object.DestroyImmediate(target);UnityEngine.Object.DestroyImmediate(pixels);
                UnityEngine.Object.DestroyImmediate(root);UnityEngine.Object.DestroyImmediate(material);
            }
        }
    }
}
#endif
