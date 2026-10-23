#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Kamilunavo.PerfectDrop.Gameplay;
using Kamilunavo.PerfectDrop.Trials;
namespace Kamilunavo.PerfectDrop.QA
{
 // Native functional acceptance only. Excluded from shipping; never creates earned medals or paid entitlements.
 public sealed class PerfectTrialsNativeProbe : MonoBehaviour
 {
  private StackGame game;private string output;private int checks;
  private static string Mode {
   get {
#if UNITY_ANDROID && !UNITY_EDITOR
    using(var player=new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
    using(var activity=player.GetStatic<AndroidJavaObject>("currentActivity"))
    using(var intent=activity.Call<AndroidJavaObject>("getIntent"))return intent.Call<string>("getStringExtra","portfolioQaMode")??"";
#else
    return Environment.GetEnvironmentVariable("PERFECT_QA_MODE")??"";
#endif
   }
  }
  [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
  private static void Configure(){if(Mode=="trials"||Mode=="trials-reload"||Mode=="trials-continue"||Mode=="manual")StackSave.QaKey="perfectdrop.portfolio.trials.native";}
  [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
  private static void Begin(){if(Mode=="trials"||Mode=="trials-reload"||Mode=="trials-continue"){var obj=new GameObject("PerfectTrialsNativeProbe");DontDestroyOnLoad(obj);obj.AddComponent<PerfectTrialsNativeProbe>();}else if(Mode=="manual")Debug.Log("PERFECT_MANUAL_QA_READY providers disabled, isolated profile, no autoplay");}
  private void Check(bool value,string message){checks++;if(!value){File.WriteAllText(Path.Combine(output,"FAIL.txt"),message);throw new InvalidOperationException(message);}Debug.Log("PERFECT_TRIAL_QA_PASS "+message);}
  private void Button(string name){var b=FindObjectsByType<Button>(FindObjectsInactive.Exclude,FindObjectsSortMode.None).FirstOrDefault(x=>x.name==name&&x.gameObject.activeInHierarchy);Check(b!=null&&b.interactable,"actual button ready "+name);b.onClick.Invoke();}
  private IEnumerator Capture(string name){yield return new WaitForEndOfFrame();var frame=ScreenCapture.CaptureScreenshotAsTexture();try{File.WriteAllBytes(Path.Combine(output,name+".png"),frame.EncodeToPNG());}finally{Destroy(frame);}yield return new WaitForSecondsRealtime(.25f);}
  private IEnumerator Start(){output=Path.Combine(Application.persistentDataPath,"PerfectTrialsQA",Mode);Directory.CreateDirectory(output);yield return new WaitForSecondsRealtime(1);game=FindFirstObjectByType<StackGame>();Check(game!=null&&!game.Purchases.Ready&&!string.IsNullOrEmpty(StackSave.QaKey),"native game ready, store disabled before initialize");Check(!File.Exists(Path.Combine(Application.persistentDataPath,"perfectdrop.trials.v1.bin")),"production trial archive remains absent and untouched in fresh owned native QA");
   if(Mode=="trials-reload"||Mode=="trials-continue") {
    var saved=game.Trials.Active;
    Check(saved!=null&&saved.State==TrialAttemptState.Running,"actual native persisted trial is running before resume");
    int first=saved.TrialId,placed=saved.Snapshot.Placed;
    if(Mode=="trials-reload")Check(first==2&&placed==3,"actual cold process restores second-trial three-layer geometry");
    else Check(first>=2&&first<=12&&placed>0&&placed<TowerTrialCatalog.Get(first).Target&&game.Trials.Records.Count(x=>x.TrialId<first&&x.BestMedal>=TrialMedal.Bronze)==first-1,"continue byte-exact genuinely earned native archive with valid prior medal chain");
    game.Hud.ShowTrials();Button("TrialPrimary");yield return null;Check(game.TrialMode&&game.Run.Count==placed,"native resume restores real stack without awarding extra layers");yield return Capture("cold-resumed-"+placed+"-layers");
    for(int id=first;id<=12;id++) {if(id>first){game.GoHome();game.Hud.ShowTrials();Button("TowerTrial"+id);yield return null;}yield return Complete();Check(game.Trials.Records.Any(x=>x.TrialId==id&&x.BestMedal>=TrialMedal.Bronze),"real completed trial medal "+id);yield return Capture("medal-"+id);}
    Check(game.Trials.Records.Count==12,"twelve native completed trial records");
   } else {
    Check(game.Trials.Revision==0&&game.Trials.Records.Count==0,"fresh native isolated trial archive");
    game.GoHome();Button("CampaignNavigation");yield return null;Button("TowerTrialsEntry");yield return Capture("trial-ladder-new");Button("TowerTrial1");yield return null;
    for(int attempt=1;attempt<=3;attempt++){float deadline=Time.realtimeSinceStartup+15;while(Mathf.Abs(game.MovingOffset)<3.7f&&Time.realtimeSinceStartup<deadline)yield return null;Check(Mathf.Abs(game.MovingOffset)>=3.7f,"real moving block reaches miss position");Button("Drop");yield return new WaitForSecondsRealtime(.15f);Check(game.Trials.Active.State==TrialAttemptState.Failed&&game.Trials.Active.AttemptNumber==attempt,"genuine miss consumes one of three attempts "+attempt);if(attempt<3){Button("TrialPrimary");yield return null;}}
    yield return Capture("three-attempts-failed");game.GoHome();game.Hud.ShowTrials();Button("TowerTrial1");yield return null;Check(game.Trials.Active.AttemptNumber==1,"new free run resets attempts after three misses");yield return Complete();Check(game.Trials.Records.Any(x=>x.TrialId==1&&x.BestMedal==TrialMedal.Gold),"actual perfect tower earns native gold medal");yield return Capture("first-gold-medal");
    game.GoHome();game.Hud.ShowTrials();Button("TowerTrial2");yield return null;for(int i=0;i<3;i++)yield return PerfectDrop();game.GoHome();Check(game.Trials.Active.Snapshot.Placed==3&&game.Trials.Active.State==TrialAttemptState.Running,"native saved checkpoint before cold force stop");
   }
   Check(game.Profile.Coins==0&&game.Profile.Towers==0&&game.Profile.UnlockedLevel==1&&game.Profile.LevelStars.All(x=>x==0),"trials preserve legacy wallet, campaign, towers and stars");
   foreach(bool wide in new[]{false,true}){Screen.orientation=wide?ScreenOrientation.LandscapeLeft:ScreenOrientation.Portrait;float end=Time.realtimeSinceStartup+5;while((Screen.width>Screen.height)!=wide&&Time.realtimeSinceStartup<end)yield return null;Check((Screen.width>Screen.height)==wide,"actual native trial orientation settled");game.GoHome();game.Hud.ShowTrials();yield return Capture("ladder-"+(wide?"wide":"portrait"));}
   Screen.orientation=ScreenOrientation.Portrait;
   File.WriteAllText(Path.Combine(output,"PASS.txt"),"PERFECT_TRIAL_NATIVE_PASS mode="+Mode+" checks="+checks+"; real native moving geometry/drop callbacks, isolated profile, no provider transactions; no OS-touch or FPS claim");Debug.Log("PERFECT_TRIAL_NATIVE_PASS "+Mode);
  }
  private bool NaturallyPerfect(){var top=game.Run.Top;var center=top.Center+(game.Run.Axis==StackAxis.X?new Vector2(game.MovingOffset,0):new Vector2(0,game.MovingOffset));return StackRules.Evaluate(center,top.Size,top.Center,game.Run.Axis).Grade==StackGrade.Perfect;}
  private IEnumerator PerfectDrop(){int before=game.Run.Count,perfects=game.Run.PerfectDrops;float end=Time.realtimeSinceStartup+30;while(!NaturallyPerfect()&&Time.realtimeSinceStartup<end)yield return null;Check(NaturallyPerfect(),"real moving block satisfies actual StackRules perfect grade before input");Button("Drop");yield return new WaitForSecondsRealtime(.15f);Check(game.Run.Count==before+1&&game.Run.PerfectDrops==perfects+1&&!game.Run.Failed&&game.TrialSaveError==null,"actual native perfect placement commits exactly one layer and one perfect");}
  private IEnumerator Complete(){while(!game.Run.Completed&&!game.Run.Failed)yield return PerfectDrop();Check(game.Run.Completed&&game.Trials.Active.State==TrialAttemptState.Cleared,"native full tower meets its goal and completes");}
 }
}
#endif
