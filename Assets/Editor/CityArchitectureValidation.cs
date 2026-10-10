#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEngine;
using Kamilunavo.PerfectDrop.Visuals;
namespace Kamilunavo.PerfectDrop.Editor {
 public static class CityArchitectureValidation {
  [UnityEditor.MenuItem("Kamilunavo/Validate Modeled City Architecture")]
  public static void Validate(){
   var holder=new GameObject("OwnedCityArchitectureFixture");var profile=new StackProfile();
   try{
    var fresh=WorldArt.BuildPlayerCity(holder.transform,profile);Check(fresh.GetComponentsInChildren<CityMeshOwner>().Length==3,"Fresh company must have landscaping only, no unearned buildings");UnityEngine.Object.DestroyImmediate(fresh);
    for(int i=0;i<30;i++)profile.LevelStars[i]=3;var before=JsonUtility.ToJson(profile);var city=WorldArt.BuildPlayerCity(holder.transform,profile);Check(JsonUtility.ToJson(profile)==before,"Art changed wallet/stars/profile progression");int totalVisible=0,ownedVertices=0;
    for(int district=0;district<3;district++){
     var group=city.transform.Find("CityDistrict"+district);Check(group!=null,"Named district disappeared");int visible=0;
     foreach(var renderer in group.GetComponentsInChildren<MeshRenderer>())if(renderer.enabled){visible++;ownedVertices+=renderer.GetComponent<MeshFilter>().sharedMesh.vertexCount;}
     Check(visible<=88,"District exceeds bounded 88 material-batch renderer budget");totalVisible+=visible;
     var landscape=group.Find("CityLandscape"+district);Check(landscape!=null&&landscape.GetComponent<CityMeshOwner>()!=null,"District landscaping is not material-batched");
     int rounded=0;foreach(var filter in landscape.GetComponentsInChildren<MeshFilter>())if(filter.name=="RoundedLeafCrown"){rounded++;Check(filter.sharedMesh.vertexCount>40,"Foliage returned to a cuboid crown");}Check(rounded>=60,"Modeled gardens missing rounded tree crowns");
     for(int slot=0;slot<10;slot++){
      var id=district*10+slot+1;var tower=group.Find("CityTower"+id);Check(tower!=null,"Earned named tower missing "+id);Check(tower.GetComponent<CityMeshOwner>()!=null,"Tower meshes have no scene owner");var type=(slot+district*2)%6;string[] silhouettes={"StoneShaft","PairedMetalTower-1","CourtyardStoneWing-1","GlazedMetalShaft","BelvedereStoneShaft","TerracedLowerWing"};Check(tower.Find(silhouettes[type])!=null,"Distinct modeled architectural silhouette missing "+silhouettes[type]);int antennas=0;foreach(Transform child in tower){if(child.name.StartsWith("StarAntenna"))antennas++;Check(!child.name.StartsWith("CityFloor"),"Architecture returned to repeated floor cuboids");}Check(antennas==3,"Star record artwork no longer corresponds to profile");
      foreach(var renderer in tower.GetComponentsInChildren<MeshRenderer>())if(renderer.enabled){var bounds=renderer.bounds;Check(bounds.max.y<=7.5f,"City tower exceeds unchanged camera framing height");}
     }
    }
    var batch=typeof(WorldArt).GetMethod("BakeBuilding",BindingFlags.NonPublic|BindingFlags.Static);var first=city.transform.Find("CityDistrict0/CityTower1").gameObject;int oldMeshes=first.GetComponent<CityMeshOwner>().Meshes.Count;batch.Invoke(null,new object[]{first});Check(first.GetComponent<CityMeshOwner>().Meshes.Count==oldMeshes,"Rebaking duplicates already owned tower geometry");
    Check(city.GetComponentsInChildren<Collider>().Length==0,"City artwork created gameplay collision");
    Debug.Log("[PerfectDrop] CITY_ARCHITECTURE_PASS visibleRenderers="+totalVisible+" ownedVertices="+ownedVertices+" six silhouettes / rounded landscaping / stars / idempotent batches");
   }finally{UnityEngine.Object.DestroyImmediate(holder);}
  }
  static void Check(bool condition,string reason){if(!condition)throw new InvalidOperationException("CITY_ARCHITECTURE_FAIL "+reason);}
 }
}
#endif
