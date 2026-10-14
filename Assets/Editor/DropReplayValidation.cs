#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using Kamilunavo.PerfectDrop.Gameplay;
using Kamilunavo.PerfectDrop.UI;
using Kamilunavo.PerfectDrop.Visuals;
namespace Kamilunavo.PerfectDrop.Editor
{
 public static class DropReplayValidation
 {
  static readonly BindingFlags Hidden=BindingFlags.Instance|BindingFlags.NonPublic;
  static void Set(object o,string f,object value)=>o.GetType().GetField(f,Hidden).SetValue(o,value);
  public static void Validate()
  {
   var failures=new List<string>();var root=new GameObject("OwnedDropReplayFixture");Canvas canvas=null;int chapter=WorldArt.ActiveChapter;
   try
   {
    var game=root.AddComponent<StackGame>();var p=new StackProfile{Coins=2278,OwnedStyles=255,Style=2,UnlockedLevel=30,TutorialComplete=true};
    for(int i=0;i<30;i++)p.LevelStars[i]=1;
    Set(game,"<Profile>k__BackingField",p);Set(game,"<Run>k__BackingField",new StackRun());Set(game,"<Level>k__BackingField",StackCampaign.Level(30));
    var hud=root.AddComponent<StackHud>();Set(game,"<Hud>k__BackingField",hud);hud.Build(game);
    var safe=(RectTransform)typeof(StackHud).GetField("_safe",Hidden).GetValue(hud);canvas=safe.GetComponentInParent<Canvas>();
    var map=safe.Find("LevelMap");
    hud.ShowHome();Canvas.ForceUpdateCanvases();
    if(!map.Find("Endless").gameObject.activeInHierarchy)failures.Add("ENDLESS_HIDDEN: completed profile needs a direct visible Endless action on home");
    var bg=canvas.transform.Find("DropMenuBackdrop");
    var footer=canvas.transform.Find("DropMenuBackdrop/FullMenuFooter");
    if(footer==null||footer.GetComponent<Image>().sprite!=null)failures.Add("FOOTER_INSET: cream menu base must extend to full canvas edges without showing the game beneath the safe area");
    if(bg==null||!bg.gameObject.activeInHierarchy)failures.Add("HOME_INSET: menu artwork must cover the full canvas outside the safe controls");
    if(bg!=null){var full=UiMetrics.ScreenRect((RectTransform)canvas.transform);var image=bg.GetComponent<RawImage>();var rect=UiMetrics.ScreenRect(image.rectTransform);if(Mathf.Abs(full.xMin-rect.xMin)>.5f||Mathf.Abs(full.yMin-rect.yMin)>.5f||Mathf.Abs(full.xMax-rect.xMax)>.5f||Mathf.Abs(full.yMax-rect.yMax)>.5f||image.color.a<.999f||image.texture==null)failures.Add("full canvas bounds/opaque scene are missing");if(map.Find("SpecimenGallery/ArchitecturalScene").GetComponent<RawImage>().enabled)failures.Add("a second inset menu image is still rendered");}
    map.Find("Styles").GetComponent<Button>().onClick.Invoke();
    if(bg==null||!bg.gameObject.activeInHierarchy||safe.Find("StyleShop/DesignSkyline").GetComponent<RawImage>().enabled)failures.Add("DESIGN_INSET: gallery scene must use the same full-window background boundary");
    for(int c=0;c<3;c++)
    {
     hud.ShowHome();map.Find("Chapter"+c).GetComponent<Button>().onClick.Invoke();Canvas.ForceUpdateCanvases();
     if(!map.Find("LevelGalleryScroll").gameObject.activeInHierarchy)failures.Add("WORLD_PREVIEW_ONLY: chapter "+c+" must expose its ten replayable levels, not leave Play pointing at level 30");
    }
    p.RunLevel=1;p.ResumeActive=true;Set(game,"<Level>k__BackingField",StackCampaign.Level(1));Set(game,"<Run>k__BackingField",new StackRun(StackCampaign.Level(1).Target));hud.ShowHome();
    var destination=(Text)typeof(StackHud).GetField("_heroGoal",Hidden).GetValue(hud);
    if(!destination.text.StartsWith("Level 1 ·",StringComparison.Ordinal))failures.Add("RESUME_FRONTIER_MIXUP: home must describe the selected active level, not unlocked level 30");
    if(p.Coins!=2278||p.UnlockedLevel!=30||p.Style!=2)failures.Add("navigation changed saved progress");
   }
   finally{if(canvas!=null)UnityEngine.Object.DestroyImmediate(canvas.gameObject);UnityEngine.Object.DestroyImmediate(root);WorldArt.SetChapter(chapter);}
   if(failures.Count>0)throw new InvalidOperationException("DROP_REPLAY_RED:\n"+string.Join("\n",failures));
   Debug.Log("DROP_REPLAY_PASS direct chapter level selection / visible Endless / full-canvas menu artwork / save intact");
  }
 }
}
#endif
