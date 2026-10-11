#if UNITY_EDITOR
using System;using System.Collections.Generic;using UnityEngine;
using Kamilunavo.PerfectDrop.Visuals;using Kamilunavo.PerfectDrop.Gameplay;
namespace Kamilunavo.PerfectDrop.Editor {
 public static class PerfectDropTerraceValidation {
  static int checks;static void Check(bool value,string label){checks++;if(!value)throw new InvalidOperationException("Perfect Drop terrace art: "+label);}
  public static void Validate(){checks=0;var host=new GameObject("PerfectDropConceptGeometryFixture");var oldSky=RenderSettings.skybox;var oldReflection=RenderSettings.customReflectionTexture;var oldMode=RenderSettings.defaultReflectionMode;var oldAmbient=RenderSettings.ambientMode;var oldSkyColor=RenderSettings.ambientSkyColor;var oldEquator=RenderSettings.ambientEquatorColor;var oldGround=RenderSettings.ambientGroundColor;float oldIntensity=RenderSettings.reflectionIntensity;int chapter=WorldArt.ActiveChapter;
   var ownedMeshes=new HashSet<Mesh>();var ownedMaterials=new HashSet<Material>();
   try{
    Check(StackGame.LayerHeight>=.80f&&StackGame.LayerHeight<=.88f,"actual moving and placed blocks have concept height instead of pancake spacing");
    var block=WorldArt.CreateStackBlock(host.transform,"ActualTowerBlock",Vector3.zero,new Vector3(4,StackGame.LayerHeight,4),0);WorldArt.StyleStackBlock(block,0);
    Check(block.GetComponentsInChildren<Renderer>().Length==6,"live six-renderer block budget remains");
    var deck=block.transform.Find("MetalDeck");Check(deck.localScale.y/StackGame.LayerHeight>=.86f,"metal shoulders fill the substantial retained layer");
    var plate=block.transform.Find("TopPlate");Check(plate.localScale.x/4>=.82f,"inset plate has broad authored top area");
    var rim=block.transform.Find("GoldInset").GetComponent<MeshFilter>().sharedMesh;Check(rim.vertexCount>=160,"inset perimeter has physically rounded corners");
    var paint=new MaterialPropertyBlock();block.transform.Find("GoldBand").GetComponent<Renderer>().GetPropertyBlock(paint);Check(paint.GetColor("_EmissionColor").maxColorComponent<.6f,"accent metal retains reflected volume instead of a white neon seam");
    var finishes=new HashSet<string>();for(int style=0;style<8;style++){WorldArt.StyleStackBlock(block,style);var body=new MaterialPropertyBlock();deck.GetComponent<Renderer>().GetPropertyBlock(body);var top=new MaterialPropertyBlock();plate.GetComponent<Renderer>().GetPropertyBlock(top);Check(finishes.Add(body.GetColor("_Color").ToString("R")+top.GetColor("_Color").ToString("R")),"eight purchased designs retain distinct dominant surface bindings");}
    WorldArt.MarkSpecialBlock(block,StackBlockKind.Fragile);plate.GetComponent<Renderer>().GetPropertyBlock(paint);Check(paint.GetColor("_Color")==new Color(.9f,.25f,.65f),"fragile special cue keeps priority");
    foreach(float narrow in new[]{.025f,.10f,.50f,4f}){var piece=WorldArt.CreateStackBlock(host.transform,"ThinCut",Vector3.zero,new Vector3(narrow,StackGame.LayerHeight,2),0);foreach(var renderer in piece.GetComponentsInChildren<MeshRenderer>()){var b=renderer.bounds;Check(b.min.x>=-narrow*.5f-.0001f&&b.max.x<=narrow*.5f+.0001f&&b.min.z>=-1.0001f&&b.max.z<=1.0001f,"rounded details remain inside actual narrow cut footprint");}}
    WorldArt.ApplySkybox();var dais=WorldArt.BuildStackDais(host.transform);var foundation=dais.transform.Find("TerraceFoundation");Check(foundation!=null,"tower is grounded on a modeled architectural pedestal");
    Check(foundation.GetComponent<MeshFilter>().sharedMesh.bounds.size.y>.85f,"pedestal has genuine structural depth");
    var world=dais.transform.Find("TerraceWorld");Check(world!=null,"live midground architectural world exists");
    int rendererBudget=0;for(int theme=0;theme<3;theme++){WorldArt.SetChapter(theme);int activeThemes=0;for(int candidate=0;candidate<3;candidate++){var group=world.Find("Theme"+candidate);Check(group!=null,"three authored theme groups exist");if(group.gameObject.activeSelf)activeThemes++;Check(group.gameObject.activeSelf==(candidate==theme),"chapter switches visible architecture without rebuilding gameplay");}Check(activeThemes==1,"only selected scenery is active");}
    var names=new HashSet<string>();foreach(var node in dais.GetComponentsInChildren<Transform>(true))names.Add(node.name);Check(names.Contains("CanalArcade")&&names.Contains("ArchitecturalDome")&&names.Contains("TerraceBalustrade"),"midground uses open arches, domes and stone terraces");Check(names.Contains("NightCrystalSpire")&&names.Contains("DayBellTower")&&names.Contains("SunsetPalazzo"),"three skyline themes have physically different landmarks");
    Check(dais.GetComponentsInChildren<Collider>(true).Length==0,"environment art adds no gameplay collision");
    int vertices=0;foreach(var filter in dais.GetComponentsInChildren<MeshFilter>(true)){var mesh=filter.sharedMesh;if(mesh.name.StartsWith("Terrace",StringComparison.Ordinal)&&ownedMeshes.Add(mesh)){vertices+=mesh.vertexCount;foreach(var v in mesh.vertices)Check(float.IsFinite(v.x)&&float.IsFinite(v.y)&&float.IsFinite(v.z),"finite authored terrace geometry");foreach(var n in mesh.normals)Check(n.sqrMagnitude>.5f,"valid architectural lighting normals");}}
    foreach(var renderer in dais.GetComponentsInChildren<MeshRenderer>(true)){if(renderer.enabled&&renderer.gameObject.activeInHierarchy)rendererBudget++;var m=renderer.sharedMaterial;if(m!=null&&m.name.StartsWith("Terrace",StringComparison.Ordinal))ownedMaterials.Add(m);}
    Check(vertices<180000&&rendererBudget<=36,"bounded three-theme geometry and active renderers");
    UnityEngine.Object.DestroyImmediate(host);foreach(var mesh in ownedMeshes)Check(mesh==null,"owned live scenery buffer released: "+(mesh!=null?mesh.name:"destroyed"));foreach(var material in ownedMaterials)Check(material==null,"owned live scenery materials released");Debug.Log("PERFECT_DROP_TERRACE_BUDGET allThemeVertices="+vertices+" activeRenderers="+rendererBudget);
   }finally{if(host!=null)UnityEngine.Object.DestroyImmediate(host);WorldArt.SetChapter(chapter);RenderSettings.skybox=oldSky;RenderSettings.customReflectionTexture=oldReflection;RenderSettings.defaultReflectionMode=oldMode;RenderSettings.reflectionIntensity=oldIntensity;RenderSettings.ambientMode=oldAmbient;RenderSettings.ambientSkyColor=oldSkyColor;RenderSettings.ambientEquatorColor=oldEquator;RenderSettings.ambientGroundColor=oldGround;}
   Debug.Log("PERFECT_DROP_TERRACE_ART_PASS checks="+checks);
  }
 }
}
#endif
