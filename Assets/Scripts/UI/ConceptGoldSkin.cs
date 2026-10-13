using UnityEngine;using UnityEngine.UI;
namespace Kamilunavo.PerfectDrop.UI
{
 [RequireComponent(typeof(Image))]public sealed class ConceptGoldSkin:MonoBehaviour
 {
  static Sprite sprite;static Material material;Image image;
  public static bool Apply(Image target)
  {
   if(sprite==null){var texture=Resources.Load<Texture2D>("Art/ConceptV2/GoldAction");if(texture==null)return false;var rect=new Rect(texture.width*.018f,texture.height*.278f,texture.width*.964f,texture.height*.464f);sprite=Sprite.Create(texture,rect,new Vector2(.5f,.5f),100,0,SpriteMeshType.FullRect,new Vector4(texture.height*.245f,texture.height*.07f,texture.height*.245f,texture.height*.07f));sprite.name="OriginalConceptGold9Slice";sprite.hideFlags=HideFlags.HideAndDontSave;}
   if(material==null){var shader=Resources.Load<Shader>("PerfectDropAuthoredUI");if(shader==null)return false;material=new Material(shader){name="OriginalGoldSpriteUI",hideFlags=HideFlags.HideAndDontSave};}
   target.sprite=sprite;target.type=Image.Type.Sliced;target.material=material;var skin=target.GetComponent<ConceptGoldSkin>()??target.gameObject.AddComponent<ConceptGoldSkin>();skin.image=target;skin.enabled=true;skin.Fit();var panel=target.GetComponent<PanelSkin>();if(panel!=null)panel.enabled=false;var frame=target.transform.Find("MetalFrame")?.GetComponent<GalleryFrameGraphic>();if(frame!=null)frame.enabled=false;return true;
  }
  void OnRectTransformDimensionsChange(){Fit();}
  void Fit(){if(image!=null&&sprite!=null&&image.rectTransform.rect.height>1)image.pixelsPerUnitMultiplier=sprite.rect.height/image.rectTransform.rect.height;}
 }
}
