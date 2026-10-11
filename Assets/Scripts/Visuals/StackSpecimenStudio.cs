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
        private static readonly Dictionary<int,Entry> HeroEnvironments=new();
        public static Texture2D AcquireHero(int chapter,int style){int key=Mathf.Clamp(chapter,0,2)*8+Mathf.Clamp(style,0,7);if(!HeroEnvironments.TryGetValue(key,out var entry)){entry=new Entry{Texture=RenderEnvironmentCore(chapter,true,style)};HeroEnvironments.Add(key,entry);}entry.Users++;return entry.Texture;}
        public static int HeroUsers(int chapter,int style)=>HeroEnvironments.TryGetValue(Mathf.Clamp(chapter,0,2)*8+Mathf.Clamp(style,0,7),out var entry)?entry.Users:0;
        public static void ReleaseHero(int chapter,int style){int key=Mathf.Clamp(chapter,0,2)*8+Mathf.Clamp(style,0,7);if(!HeroEnvironments.TryGetValue(key,out var entry)||--entry.Users>0)return;HeroEnvironments.Remove(key);Dispose(entry.Texture);}
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
        private static Texture2D RenderEnvironment(int chapter,bool portrait)=>RenderEnvironmentCore(chapter,portrait,-1);
        private static Texture2D RenderEnvironmentCore(int chapter,bool portrait,int heroStyle)
        {
            var root=new GameObject("ActualTerraceEnvironmentPreview"){hideFlags=HideFlags.HideAndDontSave};root.transform.position=new Vector3(0,-1800,0);
            var cameraObject=new GameObject("EnvironmentCamera",typeof(Camera),typeof(Skybox));cameraObject.transform.SetParent(root.transform,false);
            var camera=cameraObject.GetComponent<Camera>();camera.enabled=false;camera.cullingMask=1<<Layer;camera.clearFlags=CameraClearFlags.Skybox;camera.fieldOfView=48;camera.nearClipPlane=.1f;camera.farClipPlane=180;camera.allowHDR=true;
            cameraObject.AddComponent<CinematicGrade>();
            var sky=WorldArt.CreateTerracePreviewSky(chapter,root.transform.position);cameraObject.GetComponent<Skybox>().material=sky;
            int width=portrait?512:768,height=portrait?768:512;camera.aspect=(float)width/height;
            camera.transform.position=root.transform.position+(heroStyle>=0?new Vector3(-10,10,-17):new Vector3(-11,12,-19));camera.transform.LookAt(root.transform.position+(heroStyle>=0?new Vector3(0,0,0):new Vector3(0,3,6)));
            if(heroStyle>=0)BuildHeroTower(root.transform,heroStyle);
            if(portrait && chapter==1 && heroStyle<0)BuildDesignHall(root.transform,camera);
            var dais=WorldArt.BuildStackDais(root.transform);var world=dais.transform.Find("TerraceWorld");for(int i=0;i<3;i++)world.Find("Theme"+i).gameObject.SetActive(i==chapter);
            foreach(var t in root.GetComponentsInChildren<Transform>(true))t.gameObject.layer=Layer;
            var lightObject=new GameObject("TerracePreviewSun",typeof(Light));lightObject.transform.SetParent(root.transform,false);var light=lightObject.GetComponent<Light>();light.type=LightType.Directional;light.cullingMask=1<<Layer;light.color=chapter==2?new Color(.68f,.79f,1):new Color(1,.88f,.70f);light.intensity=.68f;light.shadows=LightShadows.Soft;light.shadowStrength=.7f;light.shadowBias=.04f;light.transform.rotation=Quaternion.Euler(32,-38,0);
            var format=SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf)?RenderTextureFormat.ARGBHalf:RenderTextureFormat.ARGB32;
            var render=RenderTexture.GetTemporary(width,height,24,format,RenderTextureReadWrite.Default,4);var previous=RenderTexture.active;Texture2D texture=null;
            var oldProbe=RenderSettings.ambientProbe;var oldAmbient=RenderSettings.ambientMode;var oldTop=RenderSettings.ambientSkyColor;var oldEquator=RenderSettings.ambientEquatorColor;var oldBottom=RenderSettings.ambientGroundColor;var oldReflectionIntensity=RenderSettings.reflectionIntensity;
            RenderSettings.ambientMode=UnityEngine.Rendering.AmbientMode.Trilight;
            RenderSettings.ambientSkyColor=chapter==2?new Color(.35f,.43f,.66f):chapter==0?new Color(.43f,.48f,.58f):new Color(.47f,.48f,.67f);
            RenderSettings.ambientEquatorColor=new Color(.40f,.32f,.37f);RenderSettings.ambientGroundColor=new Color(.15f,.17f,.25f);RenderSettings.reflectionIntensity=.8f;
            var probe=new UnityEngine.Rendering.SphericalHarmonicsL2();probe.AddAmbientLight(RenderSettings.ambientEquatorColor*.6f);probe.AddDirectionalLight(Vector3.up,RenderSettings.ambientSkyColor,.4f);probe.AddDirectionalLight(Vector3.down,RenderSettings.ambientGroundColor,.25f);RenderSettings.ambientProbe=probe;
            try{camera.targetTexture=render;camera.Render();RenderTexture.active=render;texture=new Texture2D(width,height,TextureFormat.RGBA32,false){name=heroStyle>=0?"ActualTerraceHero_"+chapter+"_"+heroStyle:"ActualTerraceEnvironment_"+chapter+"_"+portrait,hideFlags=HideFlags.HideAndDontSave,filterMode=FilterMode.Bilinear,wrapMode=TextureWrapMode.Clamp};texture.ReadPixels(new Rect(0,0,width,height),0,0);texture.Apply(false,true);return texture;}
            catch{if(texture!=null)Dispose(texture);throw;}
            finally{RenderSettings.ambientMode=oldAmbient;RenderSettings.ambientProbe=oldProbe;RenderSettings.ambientSkyColor=oldTop;RenderSettings.ambientEquatorColor=oldEquator;RenderSettings.ambientGroundColor=oldBottom;RenderSettings.reflectionIntensity=oldReflectionIntensity;camera.targetTexture=null;RenderTexture.active=previous;RenderTexture.ReleaseTemporary(render);root.SetActive(false);Dispose(root);Dispose(sky);}
        }

        private static void BuildHeroTower(Transform root,int style)
        {
            float height=Kamilunavo.PerfectDrop.Gameplay.StackGame.LayerHeight;
            var pedestal=WorldArt.CreateStackBlock(root,"ActualMenuPedestal",Vector3.down*height,new Vector3(4,height,4),0);
            WorldArt.StyleStackBlock(pedestal,style);
            for(int layer=0;layer<6;layer++)
            {
                float width=4-layer*.08f;
                var block=WorldArt.CreateStackBlock(root,"ActualMenuTower"+layer,Vector3.up*layer*height,new Vector3(width,height,width),layer);
                WorldArt.StyleStackBlock(block,style);
            }
        }

        // The design showroom uses an original modeled stone arch around its view
        // of the same live city. It is rendered once and owned by the preview root.
        private static void BuildDesignHall(Transform root,Camera camera)
        {
            var hall=new GameObject("AuthoredDesignHall").transform;hall.SetParent(root,false);
            hall.position=camera.transform.position+camera.transform.forward*18-camera.transform.up*3.5f;
            hall.rotation=Quaternion.LookRotation(-camera.transform.forward,camera.transform.up);
            var owner=hall.gameObject.AddComponent<TerraceArtOwner>();var geometry=new TerraceArtGeometry.Buffer();
            geometry.Arch(Vector3.zero,5.2f,6.5f,4,.55f,.85f,32);
            foreach(int side in new[]{-1,1}){geometry.Lathe(new Vector3(side*5.45f,0,0),new[]{0f,.22f,.40f,3.6f,3.9f,4.12f},new[]{.60f,.60f,.40f,.40f,.55f,.55f},12);geometry.Box(new Vector3(side*5.45f,.15f,0),new Vector3(1.45f,.30f,1.30f));}
            var material=new Material(Resources.Load<Shader>("PerfectDropSurface")){name="OwnedShowroomIvory",color=new Color(.84f,.78f,.66f)};material.SetFloat("_StoneFinish",1);material.SetTexture("_StoneAlbedo",Resources.Load<Texture2D>("Art/AuthoredIvoryLimestone"));material.SetFloat("_Glossiness",.25f);owner.Own(material);geometry.Upload(hall,"AuthoredShowroomColonnade",material,owner);
        }

        public static Texture2D Acquire(int style,int floors)
        {
            var key=Key(style,floors);
            if(!Entries.TryGetValue(key,out var entry))
            {
                entry=new Entry{Texture=Render(Mathf.Clamp(style,0,7),Mathf.Clamp(floors,1,16))};
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
        private static int Key(int style,int floors)=>Mathf.Clamp(style,0,7)*16+Mathf.Clamp(floors,1,16);
        private static Texture2D Render(int style,int floors)
        {
            var root=new GameObject("LiveTowerSpecimenStudio"){hideFlags=HideFlags.HideAndDontSave};
            root.transform.position=new Vector3(0,-1800,0);
            var cameraObject=new GameObject("SpecimenCamera",typeof(Camera));cameraObject.transform.SetParent(root.transform,false);
            var camera=cameraObject.GetComponent<Camera>();camera.enabled=false;camera.orthographic=true;
            camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=Color.clear;
            camera.cullingMask=1<<Layer;camera.allowHDR=true;camera.nearClipPlane=.05f;camera.farClipPlane=50;
            cameraObject.AddComponent<CinematicGrade>();
            bool column=floors>=8;camera.aspect=column?.35f:.8f;camera.orthographicSize=column?floors*Kamilunavo.PerfectDrop.Gameplay.StackGame.LayerHeight*.51f+.25f:Mathf.Max(2.85f,floors*Kamilunavo.PerfectDrop.Gameplay.StackGame.LayerHeight*.63f+.20f);
            var target=root.transform.position+Vector3.up*(floors-1)*Kamilunavo.PerfectDrop.Gameplay.StackGame.LayerHeight*.5f;
            camera.transform.position=target+new Vector3(-6,column?3.5f:5,-8);
            camera.transform.LookAt(target);
            var lightObject=new GameObject("SpecimenSoftbox",typeof(Light));lightObject.transform.SetParent(root.transform,false);
            var light=lightObject.GetComponent<Light>();light.type=LightType.Directional;light.cullingMask=1<<Layer;
            light.color=new Color(1,.92f,.78f);light.intensity=.78f;light.shadows=LightShadows.None;
            light.transform.rotation=Quaternion.Euler(40,-28,0);
            for(var i=0;i<floors;i++)
            {
                // The same six-renderer block, shared rounded mesh and shader used
                // in play; a slight architectural setback helps the hero read.
                var width=column?2.25f-i*.015f:3.5f-i*.085f;
                var block=WorldArt.CreateStackBlock(root.transform,"SpecimenLayer"+i,new Vector3(0,i*Kamilunavo.PerfectDrop.Gameplay.StackGame.LayerHeight,0),new Vector3(width,Kamilunavo.PerfectDrop.Gameplay.StackGame.LayerHeight,width),i);
                WorldArt.StyleStackBlock(block,style);
                foreach(var t in block.GetComponentsInChildren<Transform>())t.gameObject.layer=Layer;
            }
            var format=SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf)?RenderTextureFormat.ARGBHalf:RenderTextureFormat.ARGB32;
            var render=RenderTexture.GetTemporary(column?320:256,column?914:320,24,format,RenderTextureReadWrite.Default,4);
            var previous=RenderTexture.active;
            Texture2D texture=null;
            try
            {
                camera.targetTexture=render;camera.Render();RenderTexture.active=render;
                texture=new Texture2D(column?320:256,column?914:320,TextureFormat.RGBA32,false){name="LiveTowerSpecimen_"+style+"_"+floors,hideFlags=HideFlags.HideAndDontSave,filterMode=FilterMode.Bilinear,wrapMode=TextureWrapMode.Clamp};
                texture.ReadPixels(new Rect(0,0,column?320:256,column?914:320),0,0);texture.Apply(false,true);
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
