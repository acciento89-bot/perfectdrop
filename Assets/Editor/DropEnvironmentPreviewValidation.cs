#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEngine;
using Kamilunavo.PerfectDrop.Visuals;
namespace Kamilunavo.PerfectDrop.Editor {
 public static class DropEnvironmentPreviewValidation {
  public static void Validate(){
   var acquire=typeof(StackSpecimenStudio).GetMethod("AcquireEnvironment",BindingFlags.Public|BindingFlags.Static);
   if(acquire==null)throw new InvalidOperationException("Menu preview still uses unrelated modern skyline wallpaper; missing actual terrace-world preview");
   var release=typeof(StackSpecimenStudio).GetMethod("ReleaseEnvironment",BindingFlags.Public|BindingFlags.Static);
   var sky=RenderSettings.skybox;var reflection=RenderSettings.customReflectionTexture;int chapter=WorldArt.ActiveChapter;
   var probe=RenderSettings.ambientProbe;var mode=RenderSettings.defaultReflectionMode;var ambient=RenderSettings.ambientMode;var intensity=RenderSettings.reflectionIntensity;var top=RenderSettings.ambientSkyColor;var equator=RenderSettings.ambientEquatorColor;var bottom=RenderSettings.ambientGroundColor;
   var hashes=new System.Collections.Generic.HashSet<long>();int checks=0;
   try{WorldArt.ApplySkybox();checks+=VerifyColdPreviewLighting();
    for(int theme=0;theme<3;theme++){
     int previousUsers=StackSpecimenStudio.EnvironmentUsers(theme,true);int leases=0;try{var image=(Texture2D)acquire.Invoke(null,new object[]{theme,true});leases++;var again=(Texture2D)acquire.Invoke(null,new object[]{theme,true});leases++;
     if(image!=again||image.width<400||image.height<600)throw new InvalidOperationException("Preview is not a cached detailed actual render");checks++;
     var target=RenderTexture.GetTemporary(image.width,image.height,0);var old=RenderTexture.active;var pixels=new Texture2D(image.width,image.height,TextureFormat.RGB24,false);
     try{Graphics.Blit(image,target);RenderTexture.active=target;pixels.ReadPixels(new Rect(0,0,image.width,image.height),0,0);pixels.Apply();var colors=pixels.GetPixels32();long hash=17;int range=0;foreach(var c in colors){unchecked{hash=hash*31+c.r+c.g*3+c.b*7;}range+=Math.Abs(c.r-c.b);}
      if(range<colors.Length*2||!hashes.Add(hash))throw new InvalidOperationException("Chapter previews are blank or visually identical");checks++;
      var output=Environment.GetEnvironmentVariable("DROP_GRAPHICS_QA_OUT");if(!string.IsNullOrEmpty(output)){System.IO.Directory.CreateDirectory(output);System.IO.File.WriteAllBytes(System.IO.Path.Combine(output,"environment-preview-"+theme+".png"),pixels.EncodeToPNG());}
     }finally{RenderTexture.active=old;RenderTexture.ReleaseTemporary(target);UnityEngine.Object.DestroyImmediate(pixels);}
     release.Invoke(null,new object[]{theme,true});leases--;if(image==null)throw new InvalidOperationException("Shared preview disposed before last owner");checks++;
     release.Invoke(null,new object[]{theme,true});leases--;if(StackSpecimenStudio.EnvironmentUsers(theme,true)!=previousUsers||(previousUsers==0&&image!=null))throw new InvalidOperationException("Preview did not restore previous owners or leaked after final owner");checks++;
     }finally{while(leases-->0)release.Invoke(null,new object[]{theme,true});}
    }
    if(WorldArt.ActiveChapter!=chapter)throw new InvalidOperationException("Menu preview changed live chapter authority");checks++;
   }finally{WorldArt.SetChapter(chapter);RenderSettings.skybox=sky;RenderSettings.customReflectionTexture=reflection;RenderSettings.defaultReflectionMode=mode;RenderSettings.reflectionIntensity=intensity;RenderSettings.ambientMode=ambient;RenderSettings.ambientSkyColor=top;RenderSettings.ambientEquatorColor=equator;RenderSettings.ambientGroundColor=bottom;RenderSettings.ambientProbe=probe;}
   Debug.Log("DROP_ENVIRONMENT_PREVIEW_PASS checks="+checks);
  }
  static int VerifyColdPreviewLighting(){
   var render=typeof(StackSpecimenStudio).GetMethod("RenderEnvironment",BindingFlags.NonPublic|BindingFlags.Static);int chapter=WorldArt.ActiveChapter;var users=new int[6];for(int i=0;i<6;i++)users[i]=StackSpecimenStudio.EnvironmentUsers(i/2,i%2==1);int checks=0;
   try{foreach(int theme in new[]{0,2}){long first=0;
    for(int incoming=0;incoming<2;incoming++){WorldArt.SetChapter(incoming==0?0:2);RenderSettings.ambientMode=UnityEngine.Rendering.AmbientMode.Flat;RenderSettings.ambientSkyColor=incoming==0?Color.white:Color.black;RenderSettings.ambientEquatorColor=incoming==0?Color.white:Color.blue;RenderSettings.ambientGroundColor=incoming==0?Color.gray:Color.black;RenderSettings.reflectionIntensity=incoming==0?.2f:1.3f;var ambient=new UnityEngine.Rendering.SphericalHarmonicsL2();ambient.AddAmbientLight(incoming==0?Color.red:Color.blue);RenderSettings.ambientProbe=ambient;
     int current=WorldArt.ActiveChapter;var oldProbe=RenderSettings.ambientProbe;var oldMode=RenderSettings.ambientMode;var oldSky=RenderSettings.ambientSkyColor;var oldMiddle=RenderSettings.ambientEquatorColor;var oldGround=RenderSettings.ambientGroundColor;var oldReflection=RenderSettings.reflectionIntensity;Texture2D image=null;
     try{image=(Texture2D)render.Invoke(null,new object[]{theme,true});long hash=PixelHash(image);if(incoming==0)first=hash;else if(hash!=first)throw new InvalidOperationException("Cold chapter preview inherits incoming live-world lighting");checks++;
      if(WorldArt.ActiveChapter!=current||RenderSettings.ambientMode!=oldMode||RenderSettings.ambientSkyColor!=oldSky||RenderSettings.ambientEquatorColor!=oldMiddle||RenderSettings.ambientGroundColor!=oldGround||RenderSettings.reflectionIntensity!=oldReflection||!RenderSettings.ambientProbe.Equals(oldProbe))throw new InvalidOperationException("Preview failed to restore incoming live-world lighting");checks++;
     }finally{if(image!=null)UnityEngine.Object.DestroyImmediate(image);}
    }
   }}finally{WorldArt.SetChapter(chapter);}
   for(int i=0;i<6;i++){if(users[i]!=StackSpecimenStudio.EnvironmentUsers(i/2,i%2==1))throw new InvalidOperationException("Cold renderer changed existing menu leases");checks++;}return checks;
  }
  static long PixelHash(Texture2D image){var target=RenderTexture.GetTemporary(image.width,image.height,0);var previous=RenderTexture.active;var pixels=new Texture2D(image.width,image.height,TextureFormat.RGB24,false);
   try{Graphics.Blit(image,target);RenderTexture.active=target;pixels.ReadPixels(new Rect(0,0,image.width,image.height),0,0);pixels.Apply();long hash=17;foreach(var c in pixels.GetPixels32())unchecked{hash=hash*31+c.r+c.g*3+c.b*7;}return hash;}
   finally{RenderTexture.active=previous;RenderTexture.ReleaseTemporary(target);UnityEngine.Object.DestroyImmediate(pixels);}
  }
 }
}
#endif
