#if UNITY_EDITOR
using System;using UnityEditor;using UnityEditor.SceneManagement;using UnityEngine;using UnityEngine.SceneManagement;using UnityEngine.Rendering;using Kamilunavo.PerfectDrop.Visuals;using Kamilunavo.PerfectDrop.Gameplay;
namespace Kamilunavo.PerfectDrop.Editor
{
 public static class DropConceptStudioSync
 {
  public static void Sync()
  {
   var incoming=SceneManager.GetActiveScene();if(incoming.isDirty)throw new InvalidOperationException("Save the artist scene before synchronizing original Concept assets.");var scene=EditorSceneManager.OpenScene("Assets/Scenes/DropConceptArtStudio.unity",OpenSceneMode.Additive);var root=scene.GetRootGameObjects()[0].transform;GameObject sample=null;
   try
   {
    int index=0;foreach(var name in new[]{"Navy & Brushed Gold","Jade Lacquer","Pearl Ceramic"})
    {
     int style=new[]{0,3,5}[index++],layer=0;foreach(Transform floor in root.Find(name))
     {
      sample=WorldArt.CreateStackBlock(null,"OriginalLiveAssetSample",Vector3.zero,new Vector3(4,StackGame.LayerHeight,4),layer++);WorldArt.StyleStackBlock(sample,style);
      foreach(var stored in floor.GetComponentsInChildren<MeshRenderer>(true))
      {
       var live=sample.transform.Find(stored.name).GetComponent<MeshRenderer>();stored.sharedMaterial.CopyPropertiesFromMaterial(live.sharedMaterial);var block=new MaterialPropertyBlock();live.GetPropertyBlock(block);
       var material=stored.sharedMaterial;for(int property=0;property<material.shader.GetPropertyCount();property++){var id=Shader.PropertyToID(material.shader.GetPropertyName(property));if(!block.HasProperty(id))continue;switch(material.shader.GetPropertyType(property)){case ShaderPropertyType.Color:material.SetColor(id,block.GetColor(id));break;case ShaderPropertyType.Vector:material.SetVector(id,block.GetVector(id));break;case ShaderPropertyType.Texture:material.SetTexture(id,block.GetTexture(id));break;default:material.SetFloat(id,block.GetFloat(id));break;}}EditorUtility.SetDirty(material);
       var storedMesh=stored.GetComponent<MeshFilter>().sharedMesh;var liveMesh=live.GetComponent<MeshFilter>().sharedMesh;if(AssetDatabase.Contains(storedMesh)){EditorUtility.CopySerialized(liveMesh,storedMesh);storedMesh.hideFlags=HideFlags.None;EditorUtility.SetDirty(storedMesh);}
      }
      UnityEngine.Object.DestroyImmediate(sample);sample=null;
     }
    }
    AssetDatabase.SaveAssets();if(!EditorSceneManager.SaveScene(scene))throw new InvalidOperationException("Concept Studio synchronization save failed.");Debug.Log("DROP_CONCEPT_V2_STUDIO_SYNC_PASS actual shared profiles/material textures / preserved asset identities");
   }
   finally{if(sample!=null)UnityEngine.Object.DestroyImmediate(sample);EditorSceneManager.CloseScene(scene,true);SceneManager.SetActiveScene(incoming);}
  }
 }
}
#endif
