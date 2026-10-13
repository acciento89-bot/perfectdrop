#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using Kamilunavo.PerfectDrop.Gameplay;
using Kamilunavo.PerfectDrop.UI;
using Kamilunavo.PerfectDrop.Visuals;
namespace Kamilunavo.PerfectDrop.Editor
{
 public static class DropMenuBoundaryValidation
 {
  static readonly BindingFlags Hidden=BindingFlags.Instance|BindingFlags.NonPublic;
  static void Set(object owner,string field,object value)=>owner.GetType().GetField(field,Hidden).SetValue(owner,value);
  static void Require(bool condition,string message){if(!condition)throw new InvalidOperationException("DROP_MENU_BOUNDARY: "+message);}
  public static void Validate()
  {
   var previous=StackSave.QaKey;int chapter=WorldArt.ActiveChapter;
   StackSave.QaKey="perfectdrop.owned.menu-boundary-20261013";
   try{foreach(var size in new[]{new Vector2(375,667),new Vector2(932,430)})Fixture(size);}
   finally{PlayerPrefs.DeleteKey(StackSave.QaKey);StackSave.QaKey=previous;UiMetrics.QaPointScale=null;WorldArt.SetChapter(chapter);}
   Debug.Log("DROP_MENU_BOUNDARY_PASS full home / opaque settings / hidden gameplay / parent navigation / intact run");
  }
  static void Fixture(Vector2 size)
  {
   var root=new GameObject("ActualDropBoundaryFixture");Canvas canvas=null;
   try{
    var game=root.AddComponent<StackGame>();var profile=new StackProfile{Coins=357,OwnedStyles=255,Style=3,UnlockedLevel=21,TutorialComplete=true};
    Set(game,"<Profile>k__BackingField",profile);Set(game,"<Run>k__BackingField",new StackRun());Set(game,"<Level>k__BackingField",StackCampaign.Level(1));
    Set(game,"_feedback",root.AddComponent<Kamilunavo.PerfectDrop.Feedback.FeedbackSystem>());
    var tower=new GameObject("ActualPlacedTower").transform;tower.SetParent(root.transform);WorldArt.CreateStackBlock(tower,"RealSlab",Vector3.zero,new Vector3(4,1,4),0);Set(game,"_tower",tower);
    var hud=root.AddComponent<StackHud>();Set(game,"<Hud>k__BackingField",hud);hud.Build(game);
    var safe=(RectTransform)typeof(StackHud).GetField("_safe",Hidden).GetValue(hud);canvas=safe.GetComponentInParent<Canvas>();canvas.GetComponent<CanvasScaler>().enabled=false;canvas.scaleFactor=2;UiMetrics.QaPointScale=2;safe.GetComponent<SafeAreaFitter>().enabled=false;safe.anchorMin=safe.anchorMax=new Vector2(.5f,.5f);safe.sizeDelta=size;
    typeof(StackHud).GetMethod("Layout",Hidden).Invoke(hud,new object[]{true});
    hud.HideMenus();hud.OpenSettings();Canvas.ForceUpdateCanvases();
    Require(!safe.Find("Drop").gameObject.activeInHierarchy,"settings must hide the actual Drop control "+size);
    Require(!tower.gameObject.activeInHierarchy,"settings must hide the actual placed/moving tower "+size);
    var backdrop=canvas.transform.Find("DropSettingsBackdrop");Require(backdrop!=null&&backdrop.gameObject.activeInHierarchy,"settings need an opaque full-screen backdrop");
    var fill=backdrop.GetComponent<Image>();Require(fill.color.a>=.999f&&fill.material==Graphic.defaultGraphicMaterial&&fill.sprite==null,"background has no transparent or rounded scene-revealing edge");
    var bg=UiMetrics.ScreenRect((RectTransform)backdrop);var full=UiMetrics.ScreenRect((RectTransform)canvas.transform);Require(Mathf.Abs(bg.xMin-full.xMin)<.5f&&Mathf.Abs(bg.yMin-full.yMin)<.5f&&Mathf.Abs(bg.xMax-full.xMax)<.5f&&Mathf.Abs(bg.yMax-full.yMax)<.5f,"settings backdrop covers the entire overlay canvas, including outside safe area");
    hud.Close();Require(tower.gameObject.activeInHierarchy&&!hud.ModalOpen,"gameplay settings resume the same run");
    hud.ShowHome();Canvas.ForceUpdateCanvases();var home=(RectTransform)safe.Find("LevelMap");var h=UiMetrics.ScreenRect(home);var s=UiMetrics.ScreenRect(safe);Require(Mathf.Abs(h.xMin-s.xMin)<.5f&&Mathf.Abs(h.xMax-s.xMax)<.5f&&Mathf.Abs(h.yMin-s.yMin)<.5f&&Mathf.Abs(h.yMax-s.yMax)<.5f,"home scene fills its pane without an inset picture");
    var action=home.Find("Continue").GetComponent<Image>();Require(action.GetComponent<ConceptGoldSkin>()?.enabled==true&&action.sprite.texture==Resources.Load<Texture2D>("Art/ConceptV2/GoldAction"),"primary action uses the actual authored gold sprite");foreach(var frame in action.GetComponentsInChildren<GalleryFrameGraphic>(true))Require(!frame.enabled,"authored capsule must not retain a rectangular procedural frame");
    home.Find("Styles").GetComponent<Button>().onClick.Invoke();var designs=safe.Find("StyleShop");foreach(var image in designs.GetComponentsInChildren<Image>(true))if(image.name=="SelectionPlate"&&image.color==GalleryTheme.Gold){Require(image.GetComponent<ConceptGoldSkin>()?.enabled==true,"owned design action must use its authored capsule");foreach(var frame in image.GetComponentsInChildren<GalleryFrameGraphic>(true))Require(!frame.enabled,"owned design action must not retain a rectangular frame");}hud.OpenSettings();hud.Close();Require(designs.gameObject.activeInHierarchy&&!home.gameObject.activeInHierarchy,"settings opened from designs return to designs");
    Image selection=null;foreach(var image in designs.GetComponentsInChildren<Image>(true))if(image.name=="SelectionPlate"&&image.transform.parent.name=="Style5")selection=image;Require(selection!=null,"actual pearl selection exists");var update=typeof(StackHud).GetMethod("UpdateDesignAction",Hidden);profile.OwnedStyles&=~(1<<5);update.Invoke(hud,new object[]{5});Require(!selection.GetComponent<ConceptGoldSkin>().enabled&&selection.material==Graphic.defaultGraphicMaterial&&selection.transform.Find("MetalFrame").GetComponent<GalleryFrameGraphic>().enabled,"revoked ownership restores the locked action skin and its own frame");profile.OwnedStyles|=1<<5;update.Invoke(hud,new object[]{5});Require(selection.GetComponent<ConceptGoldSkin>().enabled&&!selection.transform.Find("MetalFrame").GetComponent<GalleryFrameGraphic>().enabled,"restored ownership reapplies the original capsule without duplicate outlines");
    safe.Find("StyleShop/ExtraDesigns").GetComponent<Button>().onClick.Invoke();hud.OpenSettings();hud.Close();safe.Find("PremiumShop/ShopBack").GetComponent<Button>().onClick.Invoke();Require(safe.Find("StyleShop").gameObject.activeInHierarchy,"settings preserve Shop original designs parent");
    hud.ShowHome();home.Find("DirectShop").GetComponent<Button>().onClick.Invoke();hud.OpenSettings();hud.Close();Require(safe.Find("PremiumShop").gameObject.activeInHierarchy,"settings opened from Shop return to Shop");
    hud.ShowHome();typeof(StackHud).GetMethod("LayoutCampaign",Hidden).Invoke(hud,new object[]{new Rect(0,0,.35f,1)});Canvas.ForceUpdateCanvases();home.Find("Chapter1").GetComponent<Button>().onClick.Invoke();
    var hero=home.Find("SpecimenGallery/ArchitecturalScene").GetComponent<RawImage>().texture;Require((hero.width>hero.height)==(home.rect.width>home.rect.height*1.2f),"chapter changes use actual reserved menu pane orientation");
    Require(profile.Coins==357&&profile.Style==3&&profile.OwnedStyles==255&&game.Run.Count==0,"navigation cannot mutate gameplay, wallet or selected finish");
   }finally{if(canvas!=null)UnityEngine.Object.DestroyImmediate(canvas.gameObject);UnityEngine.Object.DestroyImmediate(root);}
  }
 }
}
#endif
