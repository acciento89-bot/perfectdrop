using UnityEngine;
using UnityEngine.UI;
namespace Kamilunavo.PerfectDrop.UI
{
 public static class GalleryTheme
 {
  public static readonly Color Ivory=new(.94f,.96f,.98f),Paper=new(.82f,.87f,.92f),Ink=new(.035f,.065f,.12f),Gold=new(.31f,.68f,1);
  public static readonly Color Night=new(.035f,.065f,.12f),Card=new(.10f,.14f,.22f),Text=new(.96f,.98f,1),Subtle=new(.76f,.81f,.89f);
  public static bool IsIvory(Color color)=>color.a>=.98f&&color.r>.90f&&color.g>.85f&&color.b>.70f;
  public static float Contrast(Color a,Color b){float Linear(float c)=>c<=.04045f?c/12.92f:Mathf.Pow((c+.055f)/1.055f,2.4f);float L(Color c)=>.2126f*Linear(c.r)+.7152f*Linear(c.g)+.0722f*Linear(c.b);return(Mathf.Max(L(a),L(b))+.05f)/(Mathf.Min(L(a),L(b))+.05f);}
  public static void Style(Button button,Color surface)
  {
   bool gold=surface==Gold;var image=button.GetComponent<Image>();image.color=surface;
   // The precision bench uses square metal beams and fine drafting rules.
   image.material=null;UiFactory.ApplyRoundedImage(image);
   var oldGold=image.GetComponent<ConceptGoldSkin>();if(oldGold!=null)oldGold.enabled=false;
   var oldCard=image.GetComponent<ConceptCardSkin>();if(oldCard!=null)oldCard.enabled=false;
   GalleryFrameGraphic.Apply(button.transform,gold?Ink:new Color(.40f,.57f,.70f));
   var elevation=button.GetComponent<Shadow>();if(elevation!=null){elevation.enabled=gold;elevation.effectColor=new Color(.13f,.075f,.015f,.50f);elevation.effectDistance=new Vector2(0,-2);}
   var states=button.colors;states.normalColor=Color.white;states.highlightedColor=new Color(.98f,.98f,.98f,1);states.selectedColor=states.highlightedColor;states.pressedColor=new Color(.93f,.93f,.93f,1);states.disabledColor=new Color(.90f,.90f,.90f,1);states.colorMultiplier=1;button.colors=states;
   var ink=Contrast(Ink,surface*states.disabledColor)>Contrast(Color.white,surface)?Ink:Color.white;
   foreach(var text in button.GetComponentsInChildren<Text>(true)){text.color=ink;text.resizeTextMinSize=12;var shadow=text.GetComponent<Shadow>();if(shadow!=null)shadow.enabled=ink==Color.white;}
  }

  // Actual owned one-shot scenery renders share gameplay architecture and chapter finishes.
  public static RawImage Art(Transform parent,string name,int chapter,Vector2 min,Vector2 max,bool portrait=false,bool menuAsset=false){var go=new GameObject(name,typeof(RectTransform),typeof(CanvasRenderer),typeof(RawImage));go.transform.SetParent(parent,false);var rect=(RectTransform)go.transform;rect.anchorMin=min;rect.anchorMax=max;rect.offsetMin=rect.offsetMax=Vector2.zero;var image=go.GetComponent<RawImage>();var asset=menuAsset?Resources.Load<Texture2D>("Art/ConceptV2/"+new[]{"WolkenwerkCard","AbendturmCard","NeonstadtCard"}[Mathf.Clamp(chapter,0,2)]):null;if(asset!=null)image.texture=asset;else go.AddComponent<TerracePreviewLease>().Apply(chapter,portrait);image.raycastTarget=false;return image;}
  public static void Readable(Text text,float points=13){var canvas=text.GetComponentInParent<Canvas>();float unit=UiMetrics.PointScale/Mathf.Max(.001f,canvas.scaleFactor);text.fontSize=Mathf.RoundToInt(points*unit);text.resizeTextMaxSize=text.fontSize;text.resizeTextMinSize=Mathf.RoundToInt(Mathf.Min(points,10.5f)*unit);}
  public static void Crop(RawImage image){if(image==null||image.texture==null)return;float rectAspect=image.rectTransform.rect.width/Mathf.Max(1,image.rectTransform.rect.height),sourceAspect=(float)image.texture.width/image.texture.height;image.uvRect=rectAspect<sourceAspect?new Rect((1-rectAspect/sourceAspect)*.5f,0,rectAspect/sourceAspect,1):new Rect(0,(1-sourceAspect/rectAspect)*.5f,1,sourceAspect/rectAspect);}
 }
}
