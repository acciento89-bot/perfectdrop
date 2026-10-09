#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;
using Kamilunavo.PerfectDrop.Visuals;
using Kamilunavo.PerfectDrop.UI;
using Kamilunavo.PerfectDrop.Gameplay;
namespace Kamilunavo.PerfectDrop.Editor {
 public static class StackStyleValidation {
  public static void Validate(){var parent=new GameObject("StackStyleFixture");var previewObject=new GameObject("StylePreviewFixture",typeof(RectTransform),typeof(CanvasRenderer),typeof(TowerPreviewGraphic));try{
   var block=WorldArt.CreateStackBlock(parent.transform,"ActualStyleBlock",Vector3.zero,new Vector3(3.6f,.52f,3.6f),1);var pedestal=WorldArt.CreateStackBlock(parent.transform,"Pedestal",Vector3.down*.52f,new Vector3(4,.52f,4),0);var preview=previewObject.GetComponent<TowerPreviewGraphic>();var signatures=new HashSet<string>();
   for(int style=0;style<8;style++){WorldArt.StyleStackBlock(block,style);WorldArt.StyleStackBlock(pedestal,style);ValidateBlock(pedestal,style);var finish=StackStylePalette.Get(style);ValidateBlock(block,style);preview.ApplyStyle(style);Equal(preview.Body,finish.Body,"Preview body mismatch");Equal(preview.Plate,finish.Plate,"Preview plate mismatch");Equal(preview.Accent,finish.Accent,"Preview rim mismatch");if(!signatures.Add(finish.Body.ToString()+finish.Plate))throw new Exception("STYLE_RENDER_FAIL duplicate main finish");}
   WorldArt.StyleStackBlock(block,0);ValidateBlock(block,0);WorldArt.MarkSpecialBlock(block,StackBlockKind.Fragile);Equal(Paint(block,"TopPlate").GetColor("_Color"),new Color(.9f,.25f,.65f),"Special plate cue missing");
   if(block.GetComponentsInChildren<Renderer>().Length!=6)throw new Exception("STYLE_RENDER_FAIL renderer budget changed");Debug.Log("[PerfectDrop] Style shader-property/preview/reset/special validation PASS 8");
  }finally{UnityEngine.Object.DestroyImmediate(parent);UnityEngine.Object.DestroyImmediate(previewObject);}}
  public static void ValidateBlock(GameObject block,int style){var finish=StackStylePalette.Get(style);foreach(string name in new[]{"MetalDeck","DeckCrown","TopPlate","GoldBand","GoldInset"}){var renderer=block.transform.Find(name).GetComponent<Renderer>();if(renderer.sharedMaterial.shader.name!="Kamilunavo/PerfectDropSurface"||!renderer.sharedMaterial.HasProperty("_Color"))throw new Exception("STYLE_RENDER_FAIL renderer no longer uses expected consumed shader color "+name);var paint=Paint(block,name);Equal(paint.GetColor("_Color"),name=="TopPlate"?finish.Plate:name.StartsWith("Gold")?finish.Accent:style==0&&name=="MetalDeck"?paint.GetColor("_DeckBaseColor"):finish.Body,"Actual shader tint mismatch "+name);if(name=="TopPlate"&&Mathf.Abs(paint.GetFloat("_GoldFinish")-finish.GoldFinish)>.0001f)throw new Exception("STYLE_RENDER_FAIL plate finish override missing");}}
  static MaterialPropertyBlock Paint(GameObject block,string part){var p=new MaterialPropertyBlock();block.transform.Find(part).GetComponent<Renderer>().GetPropertyBlock(p);return p;}
  static void Equal(Color a,Color b,string message){if(Mathf.Abs(a.r-b.r)+Mathf.Abs(a.g-b.g)+Mathf.Abs(a.b-b.b)+Mathf.Abs(a.a-b.a)>.005f)throw new Exception("STYLE_RENDER_FAIL "+message);}
 }
}
#endif
