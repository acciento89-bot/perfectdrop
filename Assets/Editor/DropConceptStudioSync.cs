#if UNITY_EDITOR
using System;using System.Collections.Generic;using UnityEditor;using UnityEditor.SceneManagement;using UnityEngine;using UnityEngine.SceneManagement;using UnityEngine.Rendering;using Kamilunavo.PerfectDrop.Visuals;using Kamilunavo.PerfectDrop.Gameplay;
namespace Kamilunavo.PerfectDrop.Editor
{
 public static class DropConceptStudioSync
 {
  public static void Sync()
  {
   ImportPolishedJade();
   var incoming=SceneManager.GetActiveScene();if(incoming.isDirty)throw new InvalidOperationException("Save the artist scene before synchronizing original Concept assets.");var scene=EditorSceneManager.OpenScene("Assets/Scenes/DropConceptArtStudio.unity",OpenSceneMode.Additive);var root=scene.GetRootGameObjects()[0].transform;GameObject sample=null;
   try
   {
    const string libraryPath="Assets/Art/DropConceptStudioLibrary.asset";
    var library=AssetDatabase.LoadMainAssetAtPath(libraryPath);var meshes=new Dictionary<string,Mesh>();
    foreach(var asset in AssetDatabase.LoadAllAssetsAtPath(libraryPath))if(asset is Mesh mesh)meshes[mesh.name]=mesh;
    var copied=new HashSet<Mesh>();int index=0;foreach(var name in new[]{"Navy & Brushed Gold","Jade Lacquer","Pearl Ceramic"})
    {
     int style=new[]{0,3,5}[index++],layer=0;foreach(Transform floor in root.Find(name))
     {
      var envelope=floor.Find("MetalDeck").localScale;
      sample=WorldArt.CreateStackBlock(null,"OriginalLiveAssetSample",Vector3.zero,new Vector3(envelope.x,StackGame.LayerHeight,envelope.z),layer++);WorldArt.StyleStackBlock(sample,style);
      foreach(var stored in floor.GetComponentsInChildren<MeshRenderer>(true))
      {
       var live=sample.transform.Find(stored.name).GetComponent<MeshRenderer>();stored.transform.localPosition=live.transform.localPosition;stored.transform.localRotation=live.transform.localRotation;stored.transform.localScale=live.transform.localScale;stored.sharedMaterial.CopyPropertiesFromMaterial(live.sharedMaterial);var block=new MaterialPropertyBlock();live.GetPropertyBlock(block);
       var material=stored.sharedMaterial;for(int property=0;property<material.shader.GetPropertyCount();property++){var id=Shader.PropertyToID(material.shader.GetPropertyName(property));if(!block.HasProperty(id))continue;switch(material.shader.GetPropertyType(property)){case ShaderPropertyType.Color:material.SetColor(id,block.GetColor(id));break;case ShaderPropertyType.Vector:material.SetVector(id,block.GetVector(id));break;case ShaderPropertyType.Texture:material.SetTexture(id,block.GetTexture(id));break;default:material.SetFloat(id,block.GetFloat(id));break;}}EditorUtility.SetDirty(material);
       var liveMesh=live.GetComponent<MeshFilter>().sharedMesh;
       // Previously crown, plate and undercore shared the old box mesh. New
       // profiles need distinct persistent assets; overwriting that shared ID
       // three times would silently change every stored part to the last shape.
       if(!meshes.TryGetValue(liveMesh.name,out var storedMesh)){storedMesh=UnityEngine.Object.Instantiate(liveMesh);storedMesh.hideFlags=HideFlags.None;AssetDatabase.AddObjectToAsset(storedMesh,library);meshes.Add(liveMesh.name,storedMesh);}
       if(copied.Add(storedMesh)){EditorUtility.CopySerialized(liveMesh,storedMesh);storedMesh.hideFlags=HideFlags.None;EditorUtility.SetDirty(storedMesh);}stored.GetComponent<MeshFilter>().sharedMesh=storedMesh;
      }
      UnityEngine.Object.DestroyImmediate(sample);sample=null;
     }
    }
    AssetDatabase.SaveAssets();if(!EditorSceneManager.SaveScene(scene))throw new InvalidOperationException("Concept Studio synchronization save failed.");Debug.Log("DROP_CONCEPT_V2_STUDIO_SYNC_PASS actual shared profiles/material textures / preserved asset identities");
   }
   finally{if(sample!=null)UnityEngine.Object.DestroyImmediate(sample);EditorSceneManager.CloseScene(scene,true);SceneManager.SetActiveScene(incoming);}
  }
  static void ImportPolishedJade()
  {
   const string path="Assets/Resources/Art/ConceptV4/JadePolishedSurface.png";
   var importer=AssetImporter.GetAtPath(path) as TextureImporter;if(importer==null)throw new InvalidOperationException("Original polished jade source is missing.");
   if(importer.maxTextureSize==1024&&importer.wrapMode==TextureWrapMode.Repeat&&importer.filterMode==FilterMode.Trilinear&&importer.mipmapEnabled&&importer.sRGBTexture)return;
   importer.maxTextureSize=1024;importer.wrapMode=TextureWrapMode.Repeat;importer.filterMode=FilterMode.Trilinear;importer.mipmapEnabled=true;importer.sRGBTexture=true;importer.SaveAndReimport();
  }
 }
}
#endif
