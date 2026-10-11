#if UNITY_EDITOR
using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
namespace Kamilunavo.PerfectDrop.Editor
{
 public static class OriginalMusicValidation
 {
  static int checks;
  static void Require(bool value,string message){checks++;if(!value)throw new InvalidOperationException("Original music: "+message);}
  public static void Validate()
  {
   checks=0;
   var errors=new System.Collections.Generic.List<string>();
   Application.LogCallback capture=(message,stack,kind)=>{if(kind==LogType.Error||kind==LogType.Exception||kind==LogType.Assert)errors.Add(message);};
   Application.logMessageReceived+=capture;
   try{
   var type=FindType("Kamilunavo.PerfectDrop.Audio.MusicPlayer");
   Require(type!=null,"composed music player replaces the drone-only baseline");
   const string resource="Audio/DropTerraces";
   var clip=Resources.Load<AudioClip>(resource);
   Require(clip!=null,"original musical phrase is packaged as a resource");
   Require(clip.channels==2&&clip.frequency==22050&&clip.length>=20&&clip.length<=32,"bounded stereo phrase with explicit mobile sample rate");
   var importer=(AudioImporter)AssetImporter.GetAtPath("Assets/Resources/"+resource+".wav");
   var settings=importer.defaultSampleSettings;
   Require(settings.loadType==AudioClipLoadType.CompressedInMemory&&settings.compressionFormat==AudioCompressionFormat.Vorbis,"music remains compressed in memory");
   Require(settings.sampleRateSetting==AudioSampleRateSetting.OverrideSampleRate&&settings.sampleRateOverride==22050,"explicit 22kHz mobile import");
   Require(settings.preloadAudioData&&!importer.loadInBackground,"loop is ready before initial fade and is never streamed from disk");
   InspectWave("Assets/Resources/"+resource+".wav");
   ValidatePersistence();
   var host=new GameObject("OriginalMusicFixture");
   AudioSource owned=null;
   try{
    var player=host.AddComponent(type);
    bool sound=true,music=true;
    object[] args={resource,(Func<bool>)(()=>sound),(Func<bool>)(()=>music),.22f};
    var initialize=type.GetMethod("Initialize");
    Require(initialize!=null,"preference callbacks remain an explicit binding contract");
    Require((bool)initialize.Invoke(player,args),"packaged clip resolves");
    initialize.Invoke(player,args);
    Require(host.GetComponents<AudioSource>().Length==1,"repeated initialization does not create duplicate sources");
    owned=host.GetComponent<AudioSource>();
    Require(owned.loop&&!owned.playOnAwake&&owned.spatialBlend==0&&owned.dopplerLevel==0,"unobtrusive nonspatial looping source");
    var advance=type.GetMethod("Advance",BindingFlags.Instance|BindingFlags.NonPublic);
    Require(advance!=null,"production fade state can be advanced deterministically");
    Action<float> tick=dt=>advance.Invoke(player,new object[]{dt});
    tick(.08f);Require(owned.volume>0&&owned.volume<.22f,"initial playback fades in");
    tick(.5f);Require(Mathf.Abs(owned.volume-.22f)<.0001f,"mix gain is bounded");
    music=false;tick(.08f);Require(owned.volume>0&&owned.volume<.22f,"music-only toggle fades out");
    tick(.5f);Require(owned.volume==0,"music preference fully silences the phrase");
    music=true;tick(.5f);sound=false;tick(.001f);Require(owned.volume==0,"master sound silences music immediately");
    sound=true;tick(.5f);
    Action<bool> pause=v=>type.GetMethod("OnApplicationPause",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(player,new object[]{v});
    Action<bool> focus=v=>type.GetMethod("OnApplicationFocus",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(player,new object[]{v});
    pause(true);Require(owned.volume==0,"application background is immediately silent");
    focus(false);focus(true);tick(.5f);Require(owned.volume==0,"returning focus cannot bypass active application pause");
    pause(false);tick(.08f);Require(owned.volume>0&&owned.volume<.22f,"foreground resumes with a fade");
    focus(false);tick(.5f);Require(owned.volume==0,"focus loss is independently silent");
    pause(false);tick(.5f);Require(owned.volume==0,"pause callback cannot bypass lost focus");
    focus(true);tick(.5f);Require(owned.volume>0,"both gates permit resuming");
    Require(!(bool)initialize.Invoke(player,new object[]{"Audio/IntentionallyMissingMusic",(Func<bool>)(()=>sound),(Func<bool>)(()=>music),.22f}),"missing resource fails safely");
    Require(owned.volume==0&&owned.clip==null&&host.GetComponents<AudioSource>().Length==1,"missing clip is silent without source proliferation");
    initialize.Invoke(player,args);
    UnityEngine.Object.DestroyImmediate(player);
    var flush=type.GetMethod("FlushEditorDisposals",BindingFlags.Static|BindingFlags.NonPublic);
    Require(flush!=null,"editor cleanup is deferred independently of runtime destruction");
    flush.Invoke(null,null);
    Require(host!=null,"component-only removal preserves the audio host");
    Require(owned==null,"owned source is destroyed with its player");
    Require(clip!=null,"shared imported clip is never destroyed by one consumer");
   }finally{UnityEngine.Object.DestroyImmediate(host);}
   ValidateWholeHostTeardown(type,resource);
   Require(errors.Count==0,"audio validation has no logged errors: "+string.Join("; ",errors));
   Debug.Log("PERFECTDROP_ORIGINAL_MUSIC_PASS checks="+checks);
   }finally{Application.logMessageReceived-=capture;}
  }
  static void ValidateWholeHostTeardown(Type type,string resource)
  {
   var host=new GameObject("MusicWholeHostTeardownFixture");
   try{
    var player=host.AddComponent(type);
    type.GetMethod("Initialize").Invoke(player,new object[]{resource,(Func<bool>)(()=>true),(Func<bool>)(()=>true),.22f});
    var source=host.GetComponent<AudioSource>();
    UnityEngine.Object.DestroyImmediate(host);
    type.GetMethod("FlushEditorDisposals",BindingFlags.Static|BindingFlags.NonPublic).Invoke(null,null);
    Require(source==null,"whole-host destruction skips sources Unity already disposed");
    Require(Resources.Load<AudioClip>(resource)!=null,"whole-host destruction preserves the shared music asset");
   }finally{if(host!=null)UnityEngine.Object.DestroyImmediate(host);}
  }
  static Type FindType(string name){foreach(var assembly in AppDomain.CurrentDomain.GetAssemblies()){var result=assembly.GetType(name);if(result!=null)return result;}return null;}
  static void ValidatePersistence()
  {
   const string music="perfectdrop.settings.music",sound="perfectdrop.settings.audio";
   bool hasMusic=PlayerPrefs.HasKey(music),hasSound=PlayerPrefs.HasKey(sound);int oldMusic=PlayerPrefs.GetInt(music),oldSound=PlayerPrefs.GetInt(sound);
   try{
    PlayerPrefs.DeleteKey(music);PlayerPrefs.SetInt(sound,0);
    var preference=typeof(GamePreferences).GetProperty("MusicEnabled");Require(preference!=null&&(bool)preference.GetValue(null),"historical preferences default music on while master sound stays off");Require(!GamePreferences.AudioEnabled,"music defaults do not turn master sound on");
    preference.SetValue(null,false);PlayerPrefs.Save();Require(PlayerPrefs.GetInt(music,1)==0&&!GamePreferences.AudioEnabled,"independent music key persists disabled preference");GamePreferences.AudioEnabled=true;Require(!(bool)preference.GetValue(null),"master audio toggle does not overwrite music choice");
   }finally{if(hasMusic)PlayerPrefs.SetInt(music,oldMusic);else PlayerPrefs.DeleteKey(music);if(hasSound)PlayerPrefs.SetInt(sound,oldSound);else PlayerPrefs.DeleteKey(sound);PlayerPrefs.Save();}
  }
  static void InspectWave(string path)
  {
   using var stream=File.OpenRead(path);using var reader=new BinaryReader(stream);
   Require(new string(reader.ReadChars(4))=="RIFF","PCM source has a RIFF header");reader.ReadInt32();Require(new string(reader.ReadChars(4))=="WAVE","PCM source is WAVE");
   short channels=0,bits=0;int rate=0;byte[] pcm=null;
   while(stream.Position+8<=stream.Length){var chunk=new string(reader.ReadChars(4));int size=reader.ReadInt32();long end=stream.Position+size;if(chunk=="fmt "){Require(reader.ReadInt16()==1,"original source uses reproducible PCM");channels=reader.ReadInt16();rate=reader.ReadInt32();reader.ReadInt32();reader.ReadInt16();bits=reader.ReadInt16();}else if(chunk=="data")pcm=reader.ReadBytes(size);stream.Position=end+(size&1);}
   Require(channels==2&&bits==16&&rate==22050&&pcm!=null&&pcm.Length<3000000,"bounded original PCM buffer");
   int frames=pcm.Length/4;double peak=0,squares=0,mean=0,seam=0,slope=0,minEnergy=double.MaxValue,maxEnergy=0;int window=rate/2;
   double sample(int frame,int channel)=>BitConverter.ToInt16(pcm,frame*4+channel*2)/32768.0;
   for(int i=0;i<frames;i++)for(int c=0;c<2;c++){double s=sample(i,c);peak=Math.Max(peak,Math.Abs(s));squares+=s*s;mean+=s;}
   for(int c=0;c<2;c++){seam=Math.Max(seam,Math.Abs(sample(0,c)-sample(frames-1,c)));slope=Math.Max(slope,Math.Abs((sample(1,c)-sample(0,c))-(sample(0,c)-sample(frames-1,c))));}
   for(int start=0;start+window<=frames;start+=window){double energy=0;for(int i=start;i<start+window;i++){double s=sample(i,0);energy+=s*s;}double rms=Math.Sqrt(energy/window);minEnergy=Math.Min(minEnergy,rms);maxEnergy=Math.Max(maxEnergy,rms);}
   double totalRms=Math.Sqrt(squares/(frames*2));
   Require(peak>=.2&&peak<=.82&&totalRms>=.035&&totalRms<=.18,"quiet normalized mix has musical energy and ample headroom");
   Require(Math.Abs(mean/(frames*2))<.001&&seam<.015&&slope<.015,"loop boundary has no DC step or slope spike");
   Require(minEnergy>.008&&maxEnergy-minEnergy>.015,"phrased dynamics remain musical without a silent loop gap");
   Debug.Log("PERFECTDROP_MUSIC_WAVE peak="+peak+" rms="+totalRms+" seam="+seam+" slope="+slope+" bytes="+pcm.Length);
  }
 }
}
#endif
