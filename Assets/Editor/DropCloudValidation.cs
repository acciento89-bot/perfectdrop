#if UNITY_EDITOR
using System;
using System.IO;
using UnityEngine;
using Kamilunavo.PerfectDrop.Visuals;
namespace Kamilunavo.PerfectDrop.Editor
{
    public static class DropCloudValidation
    {
        public static void Validate()
        {
            var oldSky=RenderSettings.skybox;var oldReflection=RenderSettings.customReflectionTexture;
            var oldMode=RenderSettings.defaultReflectionMode;var oldAmbient=RenderSettings.ambientMode;
            var oldIntensity=RenderSettings.reflectionIntensity;var oldTop=RenderSettings.ambientSkyColor;
            var oldEquator=RenderSettings.ambientEquatorColor;var oldBottom=RenderSettings.ambientGroundColor;
            var oldTarget=RenderTexture.active;int chapter=WorldArt.ActiveChapter,checks=0;
            var host=new GameObject("BelowHorizonCloudProbe");var camera=host.AddComponent<Camera>();
            var target=new RenderTexture(160,120,24,RenderTextureFormat.ARGB32);target.Create();
            var pixels=new Texture2D(160,120,TextureFormat.RGBA32,false);Material material=null;
            try
            {
                camera.enabled=false;camera.cullingMask=0;camera.clearFlags=CameraClearFlags.Skybox;camera.fieldOfView=43;camera.aspect=4f/3;camera.targetTexture=target;
                var sky=host.AddComponent<Skybox>();WorldArt.ApplySkybox();
                for(int theme=0;theme<3;theme++)
                {
                    WorldArt.SetChapter(theme);material=new Material(RenderSettings.skybox);sky.material=material;
                    if(theme==0)
                    {
                        foreach(float pitch in new[]{25f,0f,-30f})
                        {
                            camera.transform.position=Vector3.up*10;camera.transform.rotation=Quaternion.Euler(pitch,35,0);camera.Render();RenderTexture.active=target;pixels.ReadPixels(new Rect(0,0,160,120),0,0);pixels.Apply();
                            var middle=pixels.GetPixel(80,60);Debug.Log("DROP_SKY_DIRECTION pitch="+pitch+" center="+middle.ToString("F3")+" actualRay="+camera.ViewportPointToRay(new Vector3(.5f,.5f,0)).direction.ToString("F3"));
                            checks++;if(pitch<=0 && middle.b<=middle.r)throw new InvalidOperationException("Gameplay heading samples the cloud underside instead of the original upper blue sky: pitch="+pitch+" color="+middle);
                            string diagnostic=Environment.GetEnvironmentVariable("DROP_SKY_QA_OUT");if(!string.IsNullOrEmpty(diagnostic)){Directory.CreateDirectory(diagnostic);File.WriteAllBytes(Path.Combine(diagnostic,"sky-direction-"+pitch+".png"),pixels.EncodeToPNG());}
                        }
                    }
                    if(theme==0)
                    {
                        // Free orbit must not cross an atan2 branch or a texture edge jump.
                        camera.fieldOfView=.005f;
                        foreach(float heading in new[]{180f,0f,90f})
                        {
                            camera.transform.rotation=Quaternion.Euler(0,heading-.01f,0);camera.Render();RenderTexture.active=target;pixels.ReadPixels(new Rect(0,0,160,120),0,0);pixels.Apply();var before=pixels.GetPixels();
                            camera.transform.rotation=Quaternion.Euler(0,heading+.01f,0);camera.Render();RenderTexture.active=target;pixels.ReadPixels(new Rect(0,0,160,120),0,0);pixels.Apply();var after=pixels.GetPixels();double error=0;
                            for(int i=0;i<before.Length;i++)error+=(Math.Abs(before[i].r-after[i].r)+Math.Abs(before[i].g-after[i].g)+Math.Abs(before[i].b-after[i].b))/3;
                            error/=before.Length;checks++;Debug.Log("DROP_SKY_ORBIT heading="+heading+" meanRgbDelta="+error);
                            if(error>.02)throw new InvalidOperationException("Panorama discontinuity during actual orbit near heading="+heading+" delta="+error);
                        }
                        camera.fieldOfView=43;
                    }
                    foreach(float height in new[]{10f,35f})
                    {
                        camera.transform.position=Vector3.up*height;camera.transform.rotation=Quaternion.Euler(69,30,0);camera.Render();RenderTexture.active=target;pixels.ReadPixels(new Rect(0,0,160,120),0,0);pixels.Apply();
                        var colors=pixels.GetPixels();double sum=0,squared=0;foreach(var c in colors){double l=.2126*c.r+.7152*c.g+.0722*c.b;sum+=l;squared+=l*l;}
                        double mean=sum/colors.Length,variation=Math.Sqrt(Math.Max(0,squared/colors.Length-mean*mean));
                        Debug.Log("DROP_CLOUD_MEASURE theme="+theme+" cameraHeight="+height+" luminanceStd="+variation);
                        string output=Environment.GetEnvironmentVariable("DROP_SKY_QA_OUT");if(!string.IsNullOrEmpty(output)){Directory.CreateDirectory(output);File.WriteAllBytes(Path.Combine(output,"cloud-theme"+theme+"-height"+height+".png"),pixels.EncodeToPNG());}
                        checks++;if(variation<.025)throw new InvalidOperationException("Below-horizon cloudscape is flat at theme="+theme+" height="+height+" std="+variation);
                        camera.Render();RenderTexture.active=target;pixels.ReadPixels(new Rect(0,0,160,120),0,0);pixels.Apply();var again=pixels.GetPixels32();var first=new Color32[colors.Length];for(int i=0;i<colors.Length;i++)first[i]=colors[i];
                        for(int i=0;i<first.Length;i++)if(!first[i].Equals(again[i]))throw new InvalidOperationException("Static reduced-motion cloud render changed without camera movement");checks++;
                    }
                    UnityEngine.Object.DestroyImmediate(material);material=null;
                }
            }
            finally
            {
                camera.targetTexture=null;RenderTexture.active=oldTarget;target.Release();UnityEngine.Object.DestroyImmediate(target);UnityEngine.Object.DestroyImmediate(pixels);if(material!=null)UnityEngine.Object.DestroyImmediate(material);UnityEngine.Object.DestroyImmediate(host);
                WorldArt.SetChapter(chapter);RenderSettings.skybox=oldSky;RenderSettings.customReflectionTexture=oldReflection;RenderSettings.defaultReflectionMode=oldMode;RenderSettings.reflectionIntensity=oldIntensity;RenderSettings.ambientMode=oldAmbient;RenderSettings.ambientSkyColor=oldTop;RenderSettings.ambientEquatorColor=oldEquator;RenderSettings.ambientGroundColor=oldBottom;
            }
            Debug.Log("DROP_CLOUD_RENDER_PASS checks="+checks);
        }
    }
}
#endif
