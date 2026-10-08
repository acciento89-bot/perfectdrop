#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Kamilunavo.PerfectDrop.Gameplay;
using Kamilunavo.PerfectDrop.UI;
using Kamilunavo.PerfectDrop.Monetization;
namespace Kamilunavo.PerfectDrop.Editor
{
 public static class CommerceRecoveryValidation
 {
  static int checks;
  static void Check(bool ok,string message){checks++;if(!ok)throw new InvalidOperationException(message);}
  public static void Validate()
  {
   checks=0;var failures=new List<string>();
   try{Consent();}catch(Exception e){failures.Add("CONSENT: "+e.Message);}
   try{Reconnect();}catch(Exception e){failures.Add("STORE: "+e.Message);}
   if(failures.Count>0)throw new InvalidOperationException(string.Join("; ",failures));
   CommerceValidation.Validate();Debug.Log("PERFECTDROP_COMMERCE_RECOVERY_PASS checks="+checks);
  }
  static void Reconnect()
  {
   var type=typeof(StorePurchases).Assembly.GetType("Kamilunavo.PerfectDrop.Monetization.StoreReconnectGate");Check(type!=null,"missing same-session reconnect gate");
   var gate=Activator.CreateInstance(type);var begin=type.GetMethod("TryBegin");var end=type.GetMethod("EndAttempt");var fail=type.GetMethod("Failed");
   Check((bool)begin.Invoke(gate,new object[]{0d}),"initial connection");Check(!(bool)begin.Invoke(gate,new object[]{1d}),"reject duplicate pending connect");
   fail.Invoke(gate,new object[]{1d});Check(!(bool)begin.Invoke(gate,new object[]{20d}),"failure callback cannot overlap outstanding await");end.Invoke(gate,null);
   Check(!(bool)begin.Invoke(gate,new object[]{5d}),"failure cooldown");Check((bool)begin.Invoke(gate,new object[]{6d}),"retry after bounded cooldown");end.Invoke(gate,null);
  }
  static void Set(object target,string property,object value)=>target.GetType().GetField("<"+property+">k__BackingField",BindingFlags.Instance|BindingFlags.NonPublic).SetValue(target,value);
  static void Consent()
  {
   var method=typeof(RewardedVideos).GetMethod("TryPresentConsent",BindingFlags.Instance|BindingFlags.NonPublic);Check(method!=null,"missing active-shop consent gate");
   var root=new GameObject("CommerceRecoveryProbe");Canvas canvas=null;var previous=StackSave.QaKey;StackSave.QaKey="kamilunavo.perfectdrop.qa.commerce-recovery";
   try
   {
    var game=root.AddComponent<StackGame>();Set(game,"Profile",new StackProfile());Set(game,"Run",new StackRun());Set(game,"Level",StackCampaign.Level(1));
    var hud=root.AddComponent<StackHud>();Set(game,"Hud",hud);hud.Build(game);canvas=hud.GetComponentInChildren<Canvas>();
    var safe=(RectTransform)typeof(StackHud).GetField("_safe",BindingFlags.Instance|BindingFlags.NonPublic).GetValue(hud);canvas=safe.GetComponentInParent<Canvas>();
    var store=root.AddComponent<StorePurchases>();Set(game,"Purchases",store);store.Initialize(game);
    var videos=root.AddComponent<RewardedVideos>();Set(game,"Videos",videos);videos.Initialize(game);int shown=0;
    bool opened=(bool)method.Invoke(videos,new object[]{(Action)(()=>shown++)});Check(!opened&&shown==0&&!videos.IsPresenting,"late consent after leaving shop never presents");
    hud.ShowShop();opened=(bool)method.Invoke(videos,new object[]{(Action)(()=>shown++)});Check(opened&&shown==1&&videos.IsPresenting&&hud.ModalOpen,"shop consent pauses gameplay");
    hud.HideMenus();Check(hud.ModalOpen,"native consent remains paused after shop closes");
    typeof(RewardedVideos).GetProperty("IsPresenting").GetSetMethod(true).Invoke(videos,new object[]{false});
    hud.ShowShop();opened=(bool)method.Invoke(videos,new object[]{(Action)(()=>throw new InvalidOperationException("presenter unavailable"))});Check(!opened&&!videos.IsPresenting,"failed presenter unlocks UI");
   }
   finally{if(canvas!=null)UnityEngine.Object.DestroyImmediate(canvas.gameObject);UnityEngine.Object.DestroyImmediate(root);StackSave.QaKey=previous;}
  }
 }
}
#endif
