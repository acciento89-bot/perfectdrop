using System;
using UnityEngine;
namespace Kamilunavo.PerfectDrop.Audio
{
 // One scene-owned player; imported clips are shared assets, never generated per run.
 [ExecuteAlways,DisallowMultipleComponent]
 public sealed class MusicPlayer : MonoBehaviour
 {
  AudioSource source;
  Func<bool> masterSound,musicPreference;
  readonly MusicEnvelope envelope=new MusicEnvelope();
  float mixGain;
  bool started,sourcePaused;
  public bool Initialize(string resourcePath,Func<bool> master,Func<bool> music,float gain=.22f)
  {
   if(master==null||music==null)throw new ArgumentNullException("Music needs both preference callbacks.");
   masterSound=master;musicPreference=music;
   mixGain=float.IsFinite(gain)?Mathf.Clamp(gain,0,.30f):.22f;
   if(source==null){source=gameObject.AddComponent<AudioSource>();}
   source.Stop();source.clip=null;source.volume=0;started=sourcePaused=false;envelope.Silence();
   source.playOnAwake=false;source.loop=true;source.spatialBlend=0;source.dopplerLevel=0;source.priority=200;
   source.clip=Resources.Load<AudioClip>(resourcePath);
   if(source.clip==null){Debug.LogWarning("Background music resource missing: "+resourcePath,this);return false;}
   return true;
  }
  void Update(){if(Application.isPlaying)Advance(Time.unscaledDeltaTime);}
  // Used by production Update and the deterministic editor lifecycle fixture.
  internal void Advance(float delta)
  {
   if(source==null||source.clip==null)return;
   source.volume=envelope.Advance(masterSound(),musicPreference(),delta,mixGain);
   if(source.volume>0){if(!started){source.Play();started=true;sourcePaused=false;}else if(sourcePaused){source.UnPause();sourcePaused=false;}}
   else if(started&&!sourcePaused){source.Pause();sourcePaused=true;}
  }
  void OnApplicationPause(bool paused){envelope.ApplicationPaused=paused;if(paused)Silence();}
  void OnApplicationFocus(bool focused){envelope.Focused=focused;if(!focused)Silence();}
  void OnDisable(){Silence();}
  void Silence(){envelope.Silence();if(source!=null){source.volume=0;if(started){source.Pause();sourcePaused=true;}}}
#if UNITY_EDITOR
  static readonly System.Collections.Generic.List<AudioSource> pendingEditorDisposals=new();
  // A host teardown already destroys its sources. Deferred editor cleanup also
  // handles removal of this component from a surviving host without double disposal.
  internal static void FlushEditorDisposals()
  {
   UnityEditor.EditorApplication.delayCall-=FlushEditorDisposals;
   var owned=pendingEditorDisposals.ToArray();pendingEditorDisposals.Clear();
   foreach(var item in owned)if(item!=null)DestroyImmediate(item);
  }
#endif
  void OnDestroy()
  {
   masterSound=null;musicPreference=null;
   if(source==null)return;
   source.Stop();source.clip=null;
   var owned=source;source=null;owned.volume=0;
   if(Application.isPlaying)Destroy(owned);
#if UNITY_EDITOR
   else{
    pendingEditorDisposals.Add(owned);
    if(pendingEditorDisposals.Count==1)UnityEditor.EditorApplication.delayCall+=FlushEditorDisposals;
   }
#endif
  }
 }
 // No allocations, clip generation, coroutines or native audio calls in the fade rule.
 internal sealed class MusicEnvelope
 {
  internal bool Focused=true,ApplicationPaused;
  internal float Volume;
  internal void Silence(){Volume=0;}
  internal float Advance(bool master,bool music,float delta,float gain)
  {
   if(!master||!Focused||ApplicationPaused){Volume=0;return 0;}
   float target=music?gain:0;
   float step=(float)Math.Max(0,Math.Min(1,float.IsFinite(delta)?delta:0))*gain/.35f;
   Volume=Volume<target?Math.Min(target,Volume+step):Math.Max(target,Volume-step);
   return Volume;
  }
 }
}
