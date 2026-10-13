#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using Kamilunavo.PerfectDrop.Visuals;
using Kamilunavo.PerfectDrop.Gameplay;

namespace Kamilunavo.PerfectDrop.Editor
{
 // Captures the actual owned architecture once; the player never runs a probe.
 public static class DropNativeArtAuthoring
 {
  const string CubePath="Assets/Resources/Rendering/DropTerraceReflection.asset";
  const string LibraryPath="Assets/Art/DropConceptStudioLibrary.asset";
  const string ScenePath="Assets/Scenes/DropConceptArtStudio.unity";
  const int StudioLayer=30;
  public static void Validate()
  {
   var cube=Resources.Load<Cubemap>("Rendering/DropTerraceReflection");
   if(cube==null)throw new InvalidOperationException("DROP_NATIVE_ART: captured original city reflection is missing");
   if(cube.width!=64||cube.format!=TextureFormat.RGBAHalf||cube.mipmapCount<7)throw new InvalidOperationException("DROP_NATIVE_ART: expected small HDR cube with filtered distance levels");
   float min=float.PositiveInfinity,max=0;foreach(CubemapFace face in new[]{CubemapFace.PositiveX,CubemapFace.NegativeX,CubemapFace.PositiveY,CubemapFace.NegativeY,CubemapFace.PositiveZ,CubemapFace.NegativeZ})foreach(var color in cube.GetPixels(face)){min=Mathf.Min(min,color.grayscale);max=Mathf.Max(max,color.grayscale);}
   Debug.Log("DROP_NATIVE_HDR_CAPTURE_RANGE min="+min+" max="+max);
   if(max-min<.05f||max<1.5f)throw new InvalidOperationException("DROP_NATIVE_ART: native cube is empty or uniform");
   var reflection=RenderSettings.customReflectionTexture;var mode=RenderSettings.defaultReflectionMode;float intensity=RenderSettings.reflectionIntensity;
   try{WorldArt.ApplySkybox();if(RenderSettings.customReflectionTexture!=cube)throw new InvalidOperationException("DROP_NATIVE_ART: actual player material reflection does not use captured city");}
   finally{RenderSettings.customReflectionTexture=reflection;RenderSettings.defaultReflectionMode=mode;RenderSettings.reflectionIntensity=intensity;}
   var studio=EditorSceneManager.OpenPreviewScene(ScenePath);try{var navy=studio.GetRootGameObjects()[0].transform.Find("Navy & Brushed Gold");int checkedParts=0;var finish=StackStylePalette.Get(0);foreach(var renderer in navy.GetComponentsInChildren<Renderer>(true))if(renderer.name=="MetalDeck"||renderer.name=="DeckCrown"){var material=renderer.sharedMaterial;if(material.color!=finish.Body||Mathf.Abs(material.GetFloat("_Metallic")-finish.Metallic)>.0001f||Mathf.Abs(material.GetFloat("_Glossiness")-finish.Smoothness)>.0001f)throw new InvalidOperationException("DROP_NATIVE_ART: saved studio navy differs from real gameplay finish");checkedParts++;}if(checkedParts!=12)throw new InvalidOperationException("DROP_NATIVE_ART: saved studio loses original navy floors");}finally{EditorSceneManager.ClosePreviewScene(studio);}
   Debug.Log("DROP_NATIVE_ART_PASS six original HDR city faces / mipmaps / actual runtime binding / saved navy matches gameplay");
  }
  public static void Author()
  {
   if(File.Exists(ScenePath)||File.Exists(LibraryPath)||File.Exists(CubePath))throw new InvalidOperationException("Authored Drop studio already exists; refusing to replace artist edits.");
   var incomingScene=SceneManager.GetActiveScene();
   if(incomingScene.isDirty)throw new InvalidOperationException("Save the current artist scene before creating the Concept studio.");
   if(string.IsNullOrEmpty(incomingScene.path))
   {
    if(!Array.TrueForAll(incomingScene.GetRootGameObjects(),root=>root.name=="Main Camera"||root.name=="Directional Light"))throw new InvalidOperationException("Unsaved artist scene must be saved before studio creation.");
    incomingScene=EditorSceneManager.OpenScene("Assets/Scenes/Main.unity",OpenSceneMode.Single);
   }
   var chapter=WorldArt.ActiveChapter;
   var mode=RenderSettings.ambientMode;var probe=RenderSettings.ambientProbe;var top=RenderSettings.ambientSkyColor;var middle=RenderSettings.ambientEquatorColor;var bottom=RenderSettings.ambientGroundColor;var reflection=RenderSettings.customReflectionTexture;var reflectionMode=RenderSettings.defaultReflectionMode;var intensity=RenderSettings.reflectionIntensity;
   var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Additive);
   GameObject root=null;Material sky=null;
   try
   {
    SceneManager.SetActiveScene(scene);WorldArt.ApplySkybox();WorldArt.SetChapter(1);
    root=new GameObject("Perfect Drop · Original Concept Studio");root.transform.position=new Vector3(0,-1800,0);
    var dais=WorldArt.BuildStackDais(root.transform);var themes=dais.transform.Find("TerraceWorld");for(int i=0;i<3;i++)themes.Find("Theme"+i).gameObject.SetActive(i==1);
    var key=new GameObject("Golden Sunset Key",typeof(Light));key.transform.SetParent(root.transform,false);var light=key.GetComponent<Light>();light.type=LightType.Directional;light.color=new Color(1,.88f,.70f);light.intensity=.68f;light.cullingMask=1<<StudioLayer;light.shadows=LightShadows.Soft;light.transform.rotation=Quaternion.Euler(32,-38,0);
    var cameraObject=new GameObject("Concept Camera",typeof(Camera),typeof(Skybox));cameraObject.transform.SetParent(root.transform,false);var camera=cameraObject.GetComponent<Camera>();camera.enabled=false;camera.cullingMask=1<<StudioLayer;camera.clearFlags=CameraClearFlags.Skybox;camera.allowHDR=true;camera.nearClipPlane=.1f;camera.farClipPlane=180;
    sky=WorldArt.CreateTerracePreviewSky(1,root.transform.position);cameraObject.GetComponent<Skybox>().material=sky;
    foreach(var node in root.GetComponentsInChildren<Transform>(true))node.gameObject.layer=StudioLayer;
    camera.transform.position=root.transform.position+new Vector3(0,2,0);RenderSettings.reflectionIntensity=0;
    Directory.CreateDirectory("Assets/Resources/Rendering");var cube=Capture(camera);AssetDatabase.CreateAsset(cube,CubePath);RenderSettings.reflectionIntensity=.8f;RenderSettings.customReflection=cube;
    // These are genuine live mesh/material variants, not painted tower thumbnails.
    for(int tower=0;tower<3;tower++)
    {
     var specimen=new GameObject(new[]{"Navy & Brushed Gold","Jade Lacquer","Pearl Ceramic"}[tower]);specimen.transform.SetParent(root.transform,false);specimen.transform.localPosition=new Vector3((tower-1)*6,0,0);
     for(int floor=0;floor<6;floor++){float width=4-floor*.08f;var block=WorldArt.CreateStackBlock(specimen.transform,"Floor "+(floor+1),Vector3.up*floor*StackGame.LayerHeight,new Vector3(width,StackGame.LayerHeight,width),floor);WorldArt.StyleStackBlock(block,new[]{0,3,5}[tower]);}
    }
    foreach(var node in root.GetComponentsInChildren<Transform>(true)){node.gameObject.layer=StudioLayer;node.gameObject.hideFlags=HideFlags.None;}
    camera.fieldOfView=48;camera.aspect=1.5f;camera.transform.position=root.transform.position+new Vector3(-15,13,-27);camera.transform.LookAt(root.transform.position+Vector3.up*2);camera.enabled=true;cameraObject.AddComponent<CinematicGrade>();
    // Persist generated meshes/materials before saving the editable native scene.
    Directory.CreateDirectory("Assets/Art");var library=new Material(sky){name="Drop Concept Studio · Original Resource Library",hideFlags=HideFlags.None};AssetDatabase.CreateAsset(library,LibraryPath);
    var meshes=new Dictionary<Mesh,Mesh>();int sequence=0;
    foreach(var filter in root.GetComponentsInChildren<MeshFilter>(true))
    {
     var mesh=filter.sharedMesh;if(mesh==null||AssetDatabase.Contains(mesh))continue;
     if(!meshes.TryGetValue(mesh,out var stored)){stored=UnityEngine.Object.Instantiate(mesh);stored.name="StudioMesh_"+(sequence++);stored.hideFlags=HideFlags.None;AssetDatabase.AddObjectToAsset(stored,library);meshes.Add(mesh,stored);}filter.sharedMesh=stored;
    }
    foreach(var renderer in root.GetComponentsInChildren<Renderer>(true))
    {
     var block=new MaterialPropertyBlock();renderer.GetPropertyBlock(block);var originals=renderer.sharedMaterials;var materials=new Material[originals.Length];
     for(int slot=0;slot<originals.Length;slot++)
     {
      var original=originals[slot];if(original==null)continue;var material=new Material(original){name="StudioFinish_"+(sequence++),hideFlags=HideFlags.None};
      for(int property=0;property<material.shader.GetPropertyCount();property++)
      {
       var name=material.shader.GetPropertyName(property);int id=Shader.PropertyToID(name);if(!block.HasProperty(id))continue;
       switch(material.shader.GetPropertyType(property)){case ShaderPropertyType.Color:material.SetColor(id,block.GetColor(id));break;case ShaderPropertyType.Vector:material.SetVector(id,block.GetVector(id));break;case ShaderPropertyType.Texture:material.SetTexture(id,block.GetTexture(id));break;default:material.SetFloat(id,block.GetFloat(id));break;}
      }
      AssetDatabase.AddObjectToAsset(material,library);materials[slot]=material;
     }
     renderer.sharedMaterials=materials;renderer.SetPropertyBlock(null);
    }
    cameraObject.GetComponent<Skybox>().material=library;RenderSettings.skybox=library;AssetDatabase.SaveAssets();
    if(!EditorSceneManager.SaveScene(scene,ScenePath))throw new InvalidOperationException("Native Concept studio save failed.");
    Debug.Log("DROP_NATIVE_ART_AUTHOR_PASS original scene / persistent genuine slab variants / six-face HDR city reflection");
   }
   finally
   {
    EditorSceneManager.CloseScene(scene,true);
    SceneManager.SetActiveScene(incomingScene);WorldArt.SetChapter(chapter);RenderSettings.ambientMode=mode;RenderSettings.ambientProbe=probe;RenderSettings.ambientSkyColor=top;RenderSettings.ambientEquatorColor=middle;RenderSettings.ambientGroundColor=bottom;RenderSettings.customReflectionTexture=reflection;RenderSettings.defaultReflectionMode=reflectionMode;RenderSettings.reflectionIntensity=intensity;
    if(sky!=null)UnityEngine.Object.DestroyImmediate(sky);
   }
  }
  public static void RefineReflectionRig()
  {
   var incoming=SceneManager.GetActiveScene();if(incoming.isDirty)throw new InvalidOperationException("Save the incoming artist scene before reflection authoring.");
   var scene=EditorSceneManager.OpenScene(ScenePath,OpenSceneMode.Additive);SceneManager.SetActiveScene(scene);
   var root=scene.GetRootGameObjects()[0].transform;var camera=root.Find("Concept Camera").GetComponent<Camera>();var position=camera.transform.localPosition;var rotation=camera.transform.localRotation;bool enabled=camera.enabled;int mask=camera.cullingMask;float intensity=RenderSettings.reflectionIntensity;
   var specimens=new[]{root.Find("Navy & Brushed Gold"),root.Find("Jade Lacquer"),root.Find("Pearl Ceramic")};var active=new bool[3];for(int i=0;i<3;i++)active[i]=specimens[i].gameObject.activeSelf;bool completed=false;
   try
   {
    if(root.Find("Original Reflection Light Rig")==null)
    {
    var library=AssetDatabase.LoadAssetAtPath<Material>(LibraryPath);var rig=new GameObject("Original Reflection Light Rig").transform;rig.SetParent(root,false);
    var material=new Material(Resources.Load<Shader>("PerfectDropSurface")){name="Original Champagne Reflection Panels",color=Color.white};material.SetColor("_EmissionColor",new Color(3.4f,2.9f,2.1f));material.EnableKeyword("_EMISSION");material.hideFlags=HideFlags.None;AssetDatabase.AddObjectToAsset(material,library);
    void Panel(string name,Vector3 center,Vector3 scale){var panel=GameObject.CreatePrimitive(PrimitiveType.Cube);panel.name=name;panel.transform.SetParent(rig,false);panel.transform.localPosition=center;panel.transform.localScale=scale;panel.transform.LookAt(root.position+Vector3.up*2);panel.layer=29;panel.GetComponent<MeshRenderer>().sharedMaterial=material;UnityEngine.Object.DestroyImmediate(panel.GetComponent<Collider>());}
    // Genuine emissive studio geometry: captured into the small HDR asset once.
    // Separate authoring layer keeps softboxes outside the playable world/view.
    Panel("Overhead Champagne Softbox",new Vector3(0,13,0),new Vector3(22,22,.03f));
    Panel("Warm Camera-Side Ribbon",new Vector3(-9,6,-11),new Vector3(5,13,.03f));
    Panel("Cool Corner Separation",new Vector3(11,4,5),new Vector3(3,11,.03f));
    }
    var finish=StackStylePalette.Get(0);foreach(var renderer in specimens[0].GetComponentsInChildren<Renderer>(true))if(renderer.name=="MetalDeck"||renderer.name=="DeckCrown"){renderer.sharedMaterial.color=finish.Body;renderer.sharedMaterial.SetFloat("_Metallic",finish.Metallic);renderer.sharedMaterial.SetFloat("_Glossiness",finish.Smoothness);EditorUtility.SetDirty(renderer.sharedMaterial);}
    for(int i=0;i<3;i++){specimens[i].gameObject.SetActive(false);}
    camera.enabled=false;camera.cullingMask=(1<<30)|(1<<29);camera.transform.localPosition=new Vector3(0,2,0);RenderSettings.reflectionIntensity=0;
    var cube=Capture(camera);try{var stored=AssetDatabase.LoadAssetAtPath<Cubemap>(CubePath);EditorUtility.CopySerialized(cube,stored);EditorUtility.SetDirty(stored);}finally{UnityEngine.Object.DestroyImmediate(cube);}
    completed=true;Debug.Log("DROP_NATIVE_REFLECTION_RIG_PASS actual HDR geometry / champagne light panels / stable resource GUID");
   }
   finally
   {
    for(int i=0;i<3;i++)specimens[i].gameObject.SetActive(active[i]);camera.transform.localPosition=position;camera.transform.localRotation=rotation;camera.enabled=enabled;camera.cullingMask=mask;RenderSettings.reflectionIntensity=intensity;if(completed){AssetDatabase.SaveAssets();if(!EditorSceneManager.SaveScene(scene,ScenePath))throw new InvalidOperationException("Refined studio scene save failed.");}EditorSceneManager.CloseScene(scene,true);SceneManager.SetActiveScene(incoming);
   }
  }

  public static void PolishGold()
  {
   var incoming=SceneManager.GetActiveScene();if(incoming.isDirty)throw new InvalidOperationException("Save the artist scene before material polish.");
   var scene=EditorSceneManager.OpenScene(ScenePath,OpenSceneMode.Additive);
   try
   {
    var root=scene.GetRootGameObjects()[0].transform;int index=0;
    foreach(string name in new[]{"Navy & Brushed Gold","Jade Lacquer","Pearl Ceramic"})
    {
     var finish=StackStylePalette.Get(new[]{0,3,5}[index++]);foreach(var renderer in root.Find(name).GetComponentsInChildren<Renderer>(true))
     {
      if(renderer.name=="TopPlate"){renderer.sharedMaterial.color=finish.Plate;renderer.sharedMaterial.SetColor("_EmissionColor",finish.Plate*StackStylePalette.PlateGlow);renderer.sharedMaterial.SetFloat("_Metallic",StackStylePalette.PlateMetallic);renderer.sharedMaterial.SetFloat("_Glossiness",StackStylePalette.PlateSmoothness);}
      else if(renderer.name.StartsWith("Gold",StringComparison.Ordinal))renderer.sharedMaterial.SetColor("_EmissionColor",finish.Accent*StackStylePalette.RimGlow);
      else continue;EditorUtility.SetDirty(renderer.sharedMaterial);
     }
    }
    root.Find("Original Reflection Light Rig/Overhead Champagne Softbox").localScale=new Vector3(32,32,.03f);
    root.Find("Original Reflection Light Rig/Cool Corner Separation").localScale=new Vector3(10,11,.03f);root.Find("Golden Sunset Key").GetComponent<Light>().intensity=1.12f;
    AssetDatabase.SaveAssets();if(!EditorSceneManager.SaveScene(scene,ScenePath))throw new InvalidOperationException("Material polish save failed.");
   }
   finally{EditorSceneManager.CloseScene(scene,true);SceneManager.SetActiveScene(incoming);}
   RefineReflectionRig();
  }

  static Cubemap Capture(Camera camera)
  {
   const int size=64;var target=new RenderTexture(size,size,24,RenderTextureFormat.ARGBHalf){dimension=TextureDimension.Cube,useMipMap=true,autoGenerateMips=false};target.Create();var previous=RenderTexture.active;
   var read=new Texture2D(size,size,TextureFormat.RGBAHalf,false,true);Cubemap cube=null;
   var grade=camera.GetComponent<CinematicGrade>();bool graded=grade!=null&&grade.enabled;if(grade!=null)grade.enabled=false;
   try
   {
    if(!camera.RenderToCubemap(target,63))throw new InvalidOperationException("Native HDR reflection capture failed.");cube=new Cubemap(size,TextureFormat.RGBAHalf,true){name="Original Sunset Terrace HDR Reflection",filterMode=FilterMode.Trilinear,wrapMode=TextureWrapMode.Clamp};
    for(int face=0;face<6;face++){Graphics.SetRenderTarget(target,0,(CubemapFace)face);read.ReadPixels(new Rect(0,0,size,size),0,0);read.Apply();cube.SetPixels(read.GetPixels(),(CubemapFace)face);}cube.Apply(true,false);return cube;
   }
   catch{if(cube!=null)UnityEngine.Object.DestroyImmediate(cube);throw;}
   finally{if(grade!=null)grade.enabled=graded;RenderTexture.active=previous;target.Release();UnityEngine.Object.DestroyImmediate(target);UnityEngine.Object.DestroyImmediate(read);}
  }
 }
}
#endif
