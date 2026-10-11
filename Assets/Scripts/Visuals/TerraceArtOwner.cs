using System.Collections.Generic;using UnityEngine;
namespace Kamilunavo.PerfectDrop.Visuals {
 // Decorative scenery owns its generated buffers/materials. Chapter selection
 // never rebuilds the stack or takes authority over collision or the camera.
 [ExecuteAlways] public sealed class TerraceArtOwner:MonoBehaviour {
  static readonly HashSet<TerraceArtOwner> Live=new();readonly List<Object> resources=new();Transform[] themes;
  public void Own(Object resource){resources.Add(resource);}
  public void Bind(Transform[] groups){themes=groups;Live.Add(this);Select(WorldArt.ActiveChapter);}
  void Select(int chapter){if(themes==null)return;for(int i=0;i<themes.Length;i++)themes[i].gameObject.SetActive(i==chapter);}
  public static void SelectChapter(int chapter){foreach(var owner in Live)if(owner!=null)owner.Select(chapter);}
  void OnDestroy(){Live.Remove(this);foreach(var resource in resources)if(resource!=null){if(Application.isPlaying)Destroy(resource);else DestroyImmediate(resource);}resources.Clear();}
 }
}
