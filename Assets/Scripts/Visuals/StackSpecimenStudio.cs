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
        public static int CachedCount=>Entries.Count;

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
            camera.aspect=.8f;camera.orthographicSize=2.85f+Mathf.Max(0,floors-4)*.16f;
            var target=root.transform.position+Vector3.up*(floors-1)*.26f;
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
                var block=WorldArt.CreateStackBlock(root.transform,"SpecimenLayer"+i,new Vector3(0,i*.52f,0),new Vector3(width,.52f,width),i);
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
