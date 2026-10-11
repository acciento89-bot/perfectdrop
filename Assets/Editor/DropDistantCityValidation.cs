#if UNITY_EDITOR
using System;
using UnityEngine;
using Kamilunavo.PerfectDrop.Visuals;
namespace Kamilunavo.PerfectDrop.Editor
{
    public static class DropDistantCityValidation
    {
        public static void Validate()
        {
            var host=new GameObject("DistantCityActualRenderProbe");
            var owner=host.AddComponent<TerraceArtOwner>();
            var cameraNode=new GameObject("DistantCityProbeCamera");
            var camera=cameraNode.AddComponent<Camera>();
            var target=new RenderTexture(96,96,24);target.Create();
            var pixels=new Texture2D(96,96,TextureFormat.RGB24,false);
            var previousTarget=RenderTexture.active;
            int checks=0;
            Material occluderMaterial=null;GameObject occluder=null;
            try
            {
                camera.enabled=false;camera.cullingMask=1<<27;camera.clearFlags=CameraClearFlags.Skybox;
                camera.fieldOfView=43;camera.farClipPlane=180;camera.targetTexture=target;
                for(int chapter=0;chapter<3;chapter++)
                {
                    var theme=new GameObject("ProbeTheme"+chapter);theme.transform.SetParent(host.transform,false);
                    DistantCityBackdrop.Build(theme.transform,chapter,owner);
                    var matte=theme.transform.GetChild(0).GetComponent<Renderer>();matte.gameObject.layer=27;
                    foreach(var pose in new[]{new Vector3(0,10,0),new Vector3(0,80.5f,117.22f),new Vector3(0,1000,0)})
                    foreach(float heading in new[]{35f,180f})
                    {
                        camera.transform.SetPositionAndRotation(pose,Quaternion.Euler(46,heading,0));
                        matte.enabled=false;camera.Render();RenderTexture.active=target;pixels.ReadPixels(new Rect(0,0,96,96),0,0);pixels.Apply();var sky=pixels.GetPixels();
                        matte.enabled=true;camera.Render();RenderTexture.active=target;pixels.ReadPixels(new Rect(0,0,96,96),0,0);pixels.Apply();var actual=pixels.GetPixels();
                        double delta=0;int changed=0;
                        for(int i=0;i<actual.Length;i++){double d=(Math.Abs(actual[i].r-sky[i].r)+Math.Abs(actual[i].g-sky[i].g)+Math.Abs(actual[i].b-sky[i].b))/3;delta+=d;if(d>.025)changed++;}
                        delta/=actual.Length;checks++;
                        string diagnostic=Environment.GetEnvironmentVariable("DROP_DISTANT_QA_OUT");
                        if(!string.IsNullOrEmpty(diagnostic)&&pose.y==10&&heading==35){System.IO.Directory.CreateDirectory(diagnostic);System.IO.File.WriteAllBytes(System.IO.Path.Combine(diagnostic,"city-art-chapter"+chapter+".png"),pixels.EncodeToPNG());}
                        if(delta<.04 || changed<actual.Length*.65)throw new InvalidOperationException("Actual distant city was overwritten or clipped: chapter="+chapter+" pose="+pose+" heading="+heading+" delta="+delta+" visiblePixels="+changed);
                    }
                    if(chapter==0)
                    {
                        foreach(float pitch in new[]{-89f,89f})
                        {
                            camera.transform.SetPositionAndRotation(Vector3.up*1000,Quaternion.Euler(pitch,180,0));
                            matte.enabled=false;camera.Render();RenderTexture.active=target;pixels.ReadPixels(new Rect(0,0,96,96),0,0);pixels.Apply();var before=pixels.GetPixel(48,48);
                            matte.enabled=true;camera.Render();RenderTexture.active=target;pixels.ReadPixels(new Rect(0,0,96,96),0,0);pixels.Apply();var after=pixels.GetPixel(48,48);
                            checks++;if(Math.Abs(before.r-after.r)+Math.Abs(before.g-after.g)+Math.Abs(before.b-after.b)<.1f)throw new InvalidOperationException("Steep City orbit loses the distant backdrop at pitch="+pitch);
                        }
                        camera.transform.SetPositionAndRotation(Vector3.up*10,Quaternion.identity);
                        foreach(float distance in new[]{5f,120f})
                        {
                            occluder=GameObject.CreatePrimitive(PrimitiveType.Cube);occluder.layer=27;
                            occluder.transform.position=camera.transform.position+camera.transform.forward*distance;occluder.transform.localScale=Vector3.one*(distance*.12f);
                            if(occluderMaterial==null){occluderMaterial=new Material(Shader.Find("Unlit/Color"));occluderMaterial.color=Color.magenta;}
                            occluder.GetComponent<Renderer>().sharedMaterial=occluderMaterial;
                            camera.Render();RenderTexture.active=target;pixels.ReadPixels(new Rect(0,0,96,96),0,0);pixels.Apply();var center=pixels.GetPixel(48,48);
                            checks++;if(center.r<.9f||center.b<.9f||center.g>.05f)throw new InvalidOperationException("Distant matte overdraws opaque geometry at distance="+distance+": "+center);
                            UnityEngine.Object.DestroyImmediate(occluder);occluder=null;
                        }
                    }
                    theme.SetActive(false);
                }
            }
            finally
            {
                if(occluder!=null)UnityEngine.Object.DestroyImmediate(occluder);if(occluderMaterial!=null)UnityEngine.Object.DestroyImmediate(occluderMaterial);camera.targetTexture=null;RenderTexture.active=previousTarget;target.Release();UnityEngine.Object.DestroyImmediate(target);UnityEngine.Object.DestroyImmediate(pixels);UnityEngine.Object.DestroyImmediate(cameraNode);UnityEngine.Object.DestroyImmediate(host);
            }
            Debug.Log("DROP_DISTANT_CITY_RENDER_PASS checks="+checks);
        }
    }
}
#endif
