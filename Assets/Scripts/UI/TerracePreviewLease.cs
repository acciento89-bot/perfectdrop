using UnityEngine;
using UnityEngine.UI;
using Kamilunavo.PerfectDrop.Visuals;
namespace Kamilunavo.PerfectDrop.UI {
 // Cache leases are attached to the exact RawImage owner, including hidden menus.
 public sealed class TerracePreviewLease:MonoBehaviour {
  int chapter,style;bool portrait,leased,hero;RawImage image;
  public void Apply(int selected,bool tall){selected=Mathf.Clamp(selected,0,2);if(leased&&!hero&&chapter==selected&&portrait==tall)return;Release();image=GetComponent<RawImage>();chapter=selected;portrait=tall;hero=false;image.texture=StackSpecimenStudio.AcquireEnvironment(chapter,portrait);leased=true;}
  public void ApplyHero(int selected,int selectedStyle,bool tall=true){selected=Mathf.Clamp(selected,0,2);selectedStyle=Mathf.Clamp(selectedStyle,0,7);if(leased&&hero&&chapter==selected&&style==selectedStyle&&portrait==tall)return;Release();image=GetComponent<RawImage>();chapter=selected;style=selectedStyle;portrait=tall;hero=true;image.texture=StackSpecimenStudio.AcquireHero(chapter,style,portrait);leased=true;}
  void Release(){if(!leased)return;if(image!=null)image.texture=null;if(hero)StackSpecimenStudio.ReleaseHero(chapter,style,portrait);else StackSpecimenStudio.ReleaseEnvironment(chapter,portrait);leased=false;}
  void OnDestroy(){Release();}
 }
}
