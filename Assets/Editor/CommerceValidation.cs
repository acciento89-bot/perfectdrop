#if UNITY_EDITOR
using System;
using UnityEngine;
using Kamilunavo.PerfectDrop.Gameplay;
using Kamilunavo.PerfectDrop.Monetization;
namespace Kamilunavo.PerfectDrop.Editor
{
    public static class CommerceValidation
    {
        public static void Validate()
        {
            var profile=StackSave.Parse("{\"Schema\":2,\"Coins\":80,\"OwnedStyles\":7,\"Style\":2}");
            Require(profile.Commerce!=null && profile.Coins==80 && profile.OwnedStyles==7 && profile.Style==2,"Old profile migration changed progress.");
            Require(!CommerceRules.ApplyPending(profile,"unknown","tx0"),"Unknown product granted.");
            Require(!CommerceRules.ApplyPending(profile,CommerceRules.Starter,""),"Empty transaction granted.");
            Require(CommerceRules.ApplyPending(profile,CommerceRules.Starter,"tx1"),"Starter purchase not granted.");
            Require(profile.Coins==580 && (profile.OwnedStyles&16)!=0,"Starter rewards wrong.");
            Require(!CommerceRules.ApplyPending(profile,CommerceRules.Starter,"tx1") && profile.Coins==580,"Replay granted twice.");
            Require(CommerceRules.ApplyPending(profile,CommerceRules.Starter,"tx2") && profile.Coins==580,"Different transaction repeated one-time starter credit.");
            var restored=new StackProfile();
            Require(CommerceRules.RestoreEntitlement(restored,CommerceRules.Starter) && restored.Coins==0 && (restored.OwnedStyles&16)!=0,"Restore minted starter coins.");
            Require(CommerceRules.ApplyPending(profile,CommerceRules.Collection,"tx3") && (profile.OwnedStyles&224)==224,"Collection styles not granted.");
            profile.Style=7;var loaded=StackSave.Parse(JsonUtility.ToJson(profile));
            Require(loaded.Coins==580 && loaded.Style==7 && (loaded.OwnedStyles&240)==240 && !CommerceRules.ApplyPending(loaded,CommerceRules.Starter,"tx1"),"Paid profile round trip/replay failed.");
            CommerceRules.ReconcileEntitlements(loaded,new[]{CommerceRules.Starter});
            Require(loaded.Style==0 && (loaded.OwnedStyles&224)==0 && (loaded.OwnedStyles&7)==7 && loaded.Coins==580,"Revoked collection did not preserve free styles/wallet.");
            CommerceRules.ReconcileEntitlements(loaded,new string[0]);
            Require((loaded.OwnedStyles&240)==0 && loaded.Commerce.StarterCreditClaimed,"Revocation lost one-time credit marker.");
            var interrupted=new StackProfile { Coins=12 };
            try { CommerceRules.FulfillPending(interrupted,CommerceRules.Starter,"crash",_=>throw new InvalidOperationException("Disk failure"));throw new Exception("Expected save failure"); }
            catch(InvalidOperationException){}
            Require(interrupted.Coins==12 && interrupted.Commerce.FulfilledTransactions.Count==0 && interrupted.OwnedStyles==1,"Save failure left an unpersisted reward.");
            Require(CommerceRules.FulfillPending(interrupted,CommerceRules.Starter,"crash",_=>{}) && interrupted.Coins==512,"Retry after failed persistence lost reward.");
            Require(CommerceRules.FulfillPending(interrupted,CommerceRules.Starter,"crash",_=>{}) && interrupted.Coins==512,"Persisted replay did not remain acknowledgeable exactly once.");
            Require(CommerceRules.ApplyPending(restored,CommerceRules.Starter,"old-restored") && restored.Coins==0,"Restored starter later minted currency.");
            var locked=new StackProfile { Coins=10000 };
            Require(!StackCampaign.SelectStyle(locked,4) && locked.Coins==10000,"Premium style bought using earned coins.");
            Require(StackCampaign.SelectStyle(profile,7) && profile.Style==7 && profile.Coins==580,"Owned premium style cannot be selected.");
            Require(!StackCampaign.SelectStyle(profile,8),"Unknown style selected.");
            var loads=new AdLoadGeneration();var first=loads.Begin();Require(loads.IsCurrent(first),"Current ad load rejected.");
            loads.Invalidate();Require(!loads.IsCurrent(first),"Ad loaded under old privacy choices accepted.");
            var fresh=loads.Begin();Require(loads.IsCurrent(fresh) && !loads.IsCurrent(first),"New ad load invalidates prior request.");
            var time=new DateTime(2026,10,8,12,0,0,DateTimeKind.Utc);var reward=new StackProfile {Coins=8};
            Require(RewardRules.Fulfill(reward,"video1",time,_=>{}) && reward.Coins==58,"Completed video not rewarded.");
            Require(!RewardRules.Fulfill(reward,"video1",time.AddMinutes(2),_=>{}) && reward.Coins==58,"Reward callback replay minted coins.");
            Require(!RewardRules.Fulfill(reward,"video2",time.AddSeconds(30),_=>{}),"Reward cooldown ignored.");
            for(var i=2;i<=5;i++)Require(RewardRules.Fulfill(reward,"video"+i,time.AddMinutes(i),_=>{}),"Daily allowed reward failed.");
            Require(!RewardRules.Fulfill(reward,"video6",time.AddMinutes(8),_=>{}) && reward.Coins==258,"Daily cap ignored.");
            var next=StackSave.Parse(JsonUtility.ToJson(reward));Require(RewardRules.Fulfill(next,"newday",time.AddDays(1),_=>{}) && next.Coins==308,"UTC day reset failed.");
            var rollback=new StackProfile();try{RewardRules.Fulfill(rollback,"disk",time,_=>throw new InvalidOperationException());}catch(InvalidOperationException){}
            Require(rollback.Coins==0 && RewardRules.Fulfill(rollback,"disk",time,_=>{}) && rollback.Coins==50,"Reward rollback/retry failed.");
            Debug.Log("Commerce validation passed: legacy migration, known products, replay, restore, premium persistence and revocation.");
        }
        private static void Require(bool ok,string message){if(!ok)throw new InvalidOperationException(message);}
    }
}
#endif
