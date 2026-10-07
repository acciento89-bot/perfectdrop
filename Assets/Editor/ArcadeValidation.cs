#if UNITY_EDITOR
using System;
using UnityEngine;
using Kamilunavo.PerfectDrop.Gameplay;
namespace Kamilunavo.PerfectDrop.Editor
{
    public static class ArcadeValidation
    {
        public static void Validate()
        {
            var run=new StackRun();
            Check(!run.Powers.Activate(StackPower.Slow),"Empty energy must reject ability.");
            for(var i=0;i<6;i++)run.Place(0);
            Check(run.Powers.Energy==6,"Perfects must charge to six.");
            Check(run.Powers.Activate(StackPower.Slow) && run.Powers.Energy==4,"Slow cost incorrect.");
            run.Powers.Tick(1); Check(run.Powers.SlowSeconds==2,"Slow time duration incorrect.");
            Check(!run.Powers.Activate(StackPower.Slow),"Running slow ability must reject reactivation.");
            Check(run.Powers.Activate(StackPower.Repair) && run.Powers.Energy==0,"Repair cost incorrect.");
            run.Powers.Risk=true;
            var rescued=run.Place(5);
            Check(rescued.Rescued && !run.Failed && run.Count==7 && !run.Powers.RepairReady && !run.Powers.Risk,"Protection did not consume a bad risk placement.");
            var fragile=new StackRun();var cut=fragile.Place(.6f,StackBlockKind.Fragile);
            Check(Mathf.Abs(cut.Size.x-2.7f)<.0001f,"Fragile non-perfect must lose extra 10%.");
            var bonus=new StackRun();bonus.Place(0,StackBlockKind.Bonus);Check(bonus.EarnedCoins==8,"Bonus block reward incorrect.");
            var risk=new StackRun();risk.Powers.Risk=true;risk.Place(0);Check(risk.EarnedCoins==8 && risk.Powers.Energy==2,"Risk Perfect reward/charge incorrect.");
            risk.Powers.Risk=true;risk.Place(.5f);Check(risk.Failed,"Risk must require Perfect.");
            var profile=new StackProfile { UnlockedLevel=21 };
            Check(StackCampaign.PowerUnlocked(profile,StackPower.Repair),"Advanced power unlock failed.");
            Check(StackCampaign.Buildings(profile)==0,"Fresh city should contain zero completed towers.");
            profile.LevelStars[0]=3;Check(StackCampaign.Buildings(profile)==1,"Completed level did not add city building.");
            var day=StackCampaign.Daily(DateTime.UtcNow);
            var daily=new StackRun(day.Target,false,Vector2.one*day.Width);
            for(var i=0;i<day.Target;i++)daily.Place(0);
            var date=DateTime.UtcNow.ToString("yyyy-MM-dd");
            Check(StackCampaign.RecordDaily(profile,day,daily,date)==75,"Daily challenge reward failed.");
            Check(StackCampaign.RecordDaily(profile,day,daily,date)==0,"Daily challenge rewarded twice.");
            CampaignValidation.Validate();StackValidation.ValidatePlatform();
            Debug.Log("[PerfectDrop] Arcade powers/special-blocks/risk/city/daily matrix passed.");
        }
        private static void Check(bool value,string message) { if(!value)throw new InvalidOperationException(message); }
    }
}
#endif
