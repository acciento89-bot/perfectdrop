using UnityEngine;
using UnityEngine.UI;
using Kamilunavo.PerfectDrop.Visuals;
namespace Kamilunavo.PerfectDrop.UI {
 // Cache leases are attached to the exact RawImage owner, including hidden menus.
 public sealed class TerracePreviewLease:MonoBehaviour {
  int chapter;bool portrait,leased;RawImage image;
  public void Apply(int selected,bool tall){selected=Mathf.Clamp(selected,0,2);if(leased&&chapter==selected&&portrait==tall)return;Release();image=GetComponent<RawImage>();chapter=selected;portrait=tall;image.texture=StackSpecimenStudio.AcquireEnvironment(chapter,portrait);leased=true;}
  void Release(){if(!leased)return;if(image!=null)image.texture=null;StackSpecimenStudio.ReleaseEnvironment(chapter,portrait);leased=false;}
  void OnDestroy(){Release();}
 }
}
