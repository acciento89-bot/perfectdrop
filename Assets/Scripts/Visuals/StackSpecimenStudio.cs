using System.Collections.Generic;
using UnityEngine;

namespace Kamilunavo.PerfectDrop.Visuals
{
    /// <summary>One-shot menu specimens use the actual live slab kit and finish.</summary>
    public static class StackSpecimenStudio
    {
        public const int Layer=30;
        private sealed class Entry { public Texture2D Texture; public int Users; }
        private static readonly Dictionary<int,Entry> Entries=new();
        private static readonly Dictionary<int,Entry> Environments=new();
        public static int CachedCount=>Entries.Count;

        public static int EnvironmentUsers(int chapter,bool portrait)=>Environments.TryGetValue(Mathf.Clamp(chapter,0,2)*2+(portrait?1:0),out var entry)?entry.Users:0;

        public static Texture2D AcquireEnvironment(int chapter,bool portrait)
        {
            chapter=Mathf.Clamp(chapter,0,2);var key=chapter*2+(portrait?1:0);
            if(!Environments.TryGetValue(key,out var entry))
            {entry=new Entry{Texture=RenderEnvironment(chapter,portrait)};Environments.Add(key,entry);}
            entry.Users++;return entry.Texture;
        }
        public static void ReleaseEnvironment(int chapter,bool portrait)
        {
            var key=Mathf.Clamp(chapter,0,2)*2+(portrait?1:0);
            if(!Environments.TryGetValue(key,out var entry)||--entry.Users>0)return;
            Environments.Remove(key);Dispose(entry.Texture);
        }
        private static Texture2D RenderEnvironment(int chapter,bool portrait)
        {
            var root=new GameObject("ActualTerraceEnvironmentPreview"){hideFlags=HideFlags.HideAndDontSave};root.transform.position=new Vector3(0,-1800,0);
            var cameraObject=new GameObject("EnvironmentCamera",typeof(Camera),typeof(Skybox));cameraObject.transform.SetParent(root.transform,false);
            var camera=cameraObject.GetComponent<Camera>();camera.enabled=false;camera.cullingMask=1<<Layer;camera.clearFlags=CameraClearFlags.Skybox;camera.fieldOfView=48;camera.nearClipPlane=.1f;camera.farClipPlane=180;camera.allowHDR=false;
            var sky=WorldArt.CreateTerracePreviewSky(chapter,root.transform.position);cameraObject.GetComponent<Skybox>().material=sky;
            int width=portrait?512:768,height=portrait?768:512;camera.aspect=(float)width/height;
            camera.transform.position=root.transform.position+new Vector3(11,12,-19);camera.transform.LookAt(root.transform.position+new Vector3(0,3,6));
            var dais=WorldArt.BuildStackDais(root.transform);var world=dais.transform.Find("TerraceWorld");for(int i=0;i<3;i++)world.Find("Theme"+i).gameObject.SetActive(i==chapter);
            foreach(var t in root.GetComponentsInChildren<Transform>(true))t.gameObject.layer=Layer;
            var lightObject=new GameObject("TerracePreviewSun",typeof(Light));lightObject.transform.SetParent(root.transform,false);var light=lightObject.GetComponent<Light>();light.type=LightType.Directional;light.cullingMask=1<<Layer;light.color=chapter==2?new Color(.68f,.79f,1):new Color(1,.88f,.70f);light.intensity=1.25f;light.shadows=LightShadows.None;light.transform.rotation=Quaternion.Euler(32,-38,0);
            var render=RenderTexture.GetTemporary(width,height,24,RenderTextureFormat.ARGB32);var previous=RenderTexture.active;Texture2D texture=null;
            var oldProbe=RenderSettings.ambientProbe;var oldAmbient=RenderSettings.ambientMode;var oldTop=RenderSettings.ambientSkyColor;var oldEquator=RenderSettings.ambientEquatorColor;var oldBottom=RenderSettings.ambientGroundColor;var oldReflectionIntensity=RenderSettings.reflectionIntensity;
            RenderSettings.ambientMode=UnityEngine.Rendering.AmbientMode.Trilight;
            RenderSettings.ambientSkyColor=chapter==2?new Color(.35f,.43f,.66f):chapter==0?new Color(.57f,.65f,.81f):new Color(.47f,.48f,.67f);
            RenderSettings.ambientEquatorColor=new Color(.40f,.32f,.37f);RenderSettings.ambientGroundColor=new Color(.15f,.17f,.25f);RenderSettings.reflectionIntensity=.8f;
            var probe=new UnityEngine.Rendering.SphericalHarmonicsL2();probe.AddAmbientLight(RenderSettings.ambientEquatorColor*.6f);probe.AddDirectionalLight(Vector3.up,RenderSettings.ambientSkyColor,.4f);probe.AddDirectionalLight(Vector3.down,RenderSettings.ambientGroundColor,.25f);RenderSettings.ambientProbe=probe;
            try{camera.targetTexture=render;camera.Render();RenderTexture.active=render;texture=new Texture2D(width,height,TextureFormat.RGBA32,false){name="ActualTerraceEnvironment_"+chapter+"_"+portrait,hideFlags=HideFlags.HideAndDontSave,filterMode=FilterMode.Bilinear,wrapMode=TextureWrapMode.Clamp};texture.ReadPixels(new Rect(0,0,width,height),0,0);texture.Apply(false,true);return texture;}
            catch{if(texture!=null)Dispose(texture);throw;}
            finally{RenderSettings.ambientMode=oldAmbient;RenderSettings.ambientProbe=oldProbe;RenderSettings.ambientSkyColor=oldTop;RenderSettings.ambientEquatorColor=oldEquator;RenderSettings.ambientGroundColor=oldBottom;RenderSettings.reflectionIntensity=oldReflectionIntensity;camera.targetTexture=null;RenderTexture.active=previous;RenderTexture.ReleaseTemporary(render);root.SetActive(false);Dispose(root);Dispose(sky);}
        }

        public static Texture2D Acquire(int style,int floors)
        {
            var key=Key(style,floors);
            if(!Entries.TryGetValue(key,out var entry))
            {
                entry=new Entry{Texture=Render(Mathf.Clamp(style,0,7),Mathf.Clamp(floors,1,8))};
                Entries.Add(key,entry);
            }
            entry.Users++;return entry.Texture;
        }
        public static void Release(int style,int floors)
        {
            var key=Key(style,floors);
            if(!Entries.TryGetValue(key,out var entry))return;
            if(--entry.Users>0)return;
            Entries.Remove(key);Dispose(entry.Texture);
        }
        private static int Key(int style,int floors)=>Mathf.Clamp(style,0,7)*16+Mathf.Clamp(floors,1,8);
        private static Texture2D Render(int style,int floors)
        {
            var root=new GameObject("LiveTowerSpecimenStudio"){hideFlags=HideFlags.HideAndDontSave};
            root.transform.position=new Vector3(0,-1800,0);
            var cameraObject=new GameObject("SpecimenCamera",typeof(Camera));cameraObject.transform.SetParent(root.transform,false);
            var camera=cameraObject.GetComponent<Camera>();camera.enabled=false;camera.orthographic=true;
            camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=Color.clear;
            camera.cullingMask=1<<Layer;camera.allowHDR=false;camera.nearClipPlane=.05f;camera.farClipPlane=50;
            camera.aspect=.8f;camera.orthographicSize=Mathf.Max(2.85f,floors*Kamilunavo.PerfectDrop.Gameplay.StackGame.LayerHeight*.63f+.20f);
            var target=root.transform.position+Vector3.up*(floors-1)*Kamilunavo.PerfectDrop.Gameplay.StackGame.LayerHeight*.5f;
            camera.transform.position=target+new Vector3(-6,5,-8);
            camera.transform.LookAt(target);
            var lightObject=new GameObject("SpecimenSoftbox",typeof(Light));lightObject.transform.SetParent(root.transform,false);
            var light=lightObject.GetComponent<Light>();light.type=LightType.Directional;light.cullingMask=1<<Layer;
            light.color=new Color(1,.92f,.78f);light.intensity=1.1f;light.shadows=LightShadows.None;
            light.transform.rotation=Quaternion.Euler(40,-28,0);
            for(var i=0;i<floors;i++)
            {
                // The same six-renderer block, shared rounded mesh and shader used
                // in play; a slight architectural setback helps the hero read.
                var width=3.5f-i*.085f;
                var block=WorldArt.CreateStackBlock(root.transform,"SpecimenLayer"+i,new Vector3(0,i*Kamilunavo.PerfectDrop.Gameplay.StackGame.LayerHeight,0),new Vector3(width,Kamilunavo.PerfectDrop.Gameplay.StackGame.LayerHeight,width),i);
                WorldArt.StyleStackBlock(block,style);
                foreach(var t in block.GetComponentsInChildren<Transform>())t.gameObject.layer=Layer;
            }
            var render=RenderTexture.GetTemporary(256,320,24,RenderTextureFormat.ARGB32);
            var previous=RenderTexture.active;
            Texture2D texture=null;
            try
            {
                camera.targetTexture=render;camera.Render();RenderTexture.active=render;
                texture=new Texture2D(256,320,TextureFormat.RGBA32,false){name="LiveTowerSpecimen_"+style+"_"+floors,hideFlags=HideFlags.HideAndDontSave,filterMode=FilterMode.Bilinear,wrapMode=TextureWrapMode.Clamp};
                texture.ReadPixels(new Rect(0,0,256,320),0,0);texture.Apply(false,true);
                return texture;
            }
            catch { if(texture!=null)Dispose(texture);throw; }
            finally
            {
                camera.targetTexture=null;RenderTexture.active=previous;RenderTexture.ReleaseTemporary(render);
                root.SetActive(false);Dispose(root);
            }
        }
        private static void Dispose(Object obj){if(Application.isPlaying)Object.Destroy(obj);else Object.DestroyImmediate(obj);}
    }
}
