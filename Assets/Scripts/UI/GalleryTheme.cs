using UnityEngine;
using UnityEngine.UI;
namespace Kamilunavo.PerfectDrop.UI
{
 public static class GalleryTheme
 {
  public static readonly Color Ivory=new(.97f,.94f,.86f),Paper=new(.90f,.85f,.74f),Ink=new(.035f,.065f,.12f),Gold=new(1,.79f,.16f);
  public static bool IsIvory(Color color)=>color.a>=.98f&&color.r>.90f&&color.g>.85f&&color.b>.70f;
  public static float Contrast(Color a,Color b){float Linear(float c)=>c<=.04045f?c/12.92f:Mathf.Pow((c+.055f)/1.055f,2.4f);float L(Color c)=>.2126f*Linear(c.r)+.7152f*Linear(c.g)+.0722f*Linear(c.b);return(Mathf.Max(L(a),L(b))+.05f)/(Mathf.Min(L(a),L(b))+.05f);}
  public static void Style(Button button,Color surface){button.GetComponent<Image>().color=surface;var states=button.colors;states.normalColor=Color.white;states.highlightedColor=new Color(.98f,.98f,.98f,1);states.selectedColor=states.highlightedColor;states.pressedColor=new Color(.93f,.93f,.93f,1);states.disabledColor=new Color(.90f,.90f,.90f,1);states.colorMultiplier=1;button.colors=states;var ink=Contrast(Ink,surface*states.disabledColor)>Contrast(Color.white,surface)?Ink:Color.white;foreach(var text in button.GetComponentsInChildren<Text>(true)){text.color=ink;text.resizeTextMinSize=12;var shadow=text.GetComponent<Shadow>();if(shadow!=null)shadow.enabled=ink==Color.white;}}
  // Owned chapter artwork is menu illustration; the tower above it uses the live mesh specimen.
  public static RawImage Art(Transform parent,string name,int chapter,Vector2 min,Vector2 max,bool portrait=false){var go=new GameObject(name,typeof(RectTransform),typeof(CanvasRenderer),typeof(RawImage));go.transform.SetParent(parent,false);var rect=(RectTransform)go.transform;rect.anchorMin=min;rect.anchorMax=max;rect.offsetMin=rect.offsetMax=Vector2.zero;var image=go.GetComponent<RawImage>();string family=chapter==0?"CloudCityDay":chapter==2?"CloudCityNight":"CloudCity";image.texture=Resources.Load<Texture2D>("Art/"+family+(portrait?"Portrait":"Landscape"));image.raycastTarget=false;return image;}
  public static void Readable(Text text,float points=13){var canvas=text.GetComponentInParent<Canvas>();float unit=UiMetrics.PointScale/Mathf.Max(.001f,canvas.scaleFactor);text.fontSize=Mathf.RoundToInt(points*unit);text.resizeTextMaxSize=text.fontSize;text.resizeTextMinSize=Mathf.RoundToInt(Mathf.Min(points,10.5f)*unit);}
  public static void Crop(RawImage image){if(image==null||image.texture==null)return;float rectAspect=image.rectTransform.rect.width/Mathf.Max(1,image.rectTransform.rect.height),sourceAspect=(float)image.texture.width/image.texture.height;image.uvRect=rectAspect<sourceAspect?new Rect((1-rectAspect/sourceAspect)*.5f,0,rectAspect/sourceAspect,1):new Rect(0,(1-sourceAspect/rectAspect)*.5f,1,sourceAspect/rectAspect);}
 }
}
