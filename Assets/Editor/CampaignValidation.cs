#if UNITY_EDITOR
using System;
using UnityEngine;
using Kamilunavo.PerfectDrop.Gameplay;
namespace Kamilunavo.PerfectDrop.Editor
{
    public static class CampaignValidation
    {
        public static void Validate()
        {
            for (var level=1;level<=30;level++)
            {
                var spec=StackCampaign.Level(level);
                Check(spec.Target >= 6 && spec.Target <= 30,"Level target outside approved range.");
                var run=new StackRun(spec.Target,false,Vector2.one*spec.Width);
                for(var i=0;i<spec.Target;i++) run.Place(0);
                Check(run.Completed && run.Count==spec.Target,"Level did not finish at its own target.");
                Check(StackCampaign.Stars(spec,run)==3,"Perfect tower should earn three stars.");
            }
            Check(StackCampaign.Level(1).Target==6 && StackCampaign.Level(30).Target==30,"Campaign endpoints incorrect.");
            var profile=new StackProfile();
            var first=StackCampaign.Level(1);
            var completed=new StackRun(first.Target,false,Vector2.one*first.Width);
            for(var i=0;i<first.Target;i++) completed.Place(0);
            var bonus=StackCampaign.Record(profile,first,completed);
            Check(bonus>0 && profile.UnlockedLevel==2 && profile.LevelStars[0]==3,"First clear failed to unlock/reward.");
            Check(StackCampaign.Record(profile,first,completed)==0,"Repeated record must not duplicate completion reward.");
            profile.UnlockedLevel=5; Check(!StackCampaign.EndlessUnlocked(profile),"Endless unlocked before level 5 clear.");
            profile.UnlockedLevel=6; Check(StackCampaign.EndlessUnlocked(profile),"Endless did not unlock after level 5.");
            profile.Coins=100;
            Check(StackCampaign.SelectStyle(profile,1) && profile.Coins==25,"Style purchase failed.");
            Check(StackCampaign.SelectStyle(profile,1) && profile.Coins==25,"Owned style charged twice.");
            Check(!StackCampaign.SelectStyle(profile,3) && profile.Coins==25,"Unaffordable style changed wallet.");
            var endless=new StackRun(0,true,Vector2.one*3.6f);
            for(var i=0;i<1000;i++) endless.Place(0);
            Check(!endless.Completed && endless.Count==1000 && endless.Layers.Count==64,"Endless must pass 30 with bounded geometry.");
            profile.RunEndless=true; profile.TotalPlaced=endless.Count; profile.Layers=new(endless.Layers); profile.ResumeActive=true;
            var restored=StackSave.Parse(JsonUtility.ToJson(profile));
            Check(restored.ResumeActive && restored.TotalPlaced==1000 && restored.Layers.Count==64,"Endless resume failed.");
            Debug.Log("[PerfectDrop] Campaign targets/stars/unlocks/economy/endless/save matrix passed.");
        }
        private static void Check(bool value,string message) { if(!value) throw new InvalidOperationException(message); }
    }
}
#endif
