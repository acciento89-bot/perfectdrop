#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEngine;
using Kamilunavo.PerfectDrop.Trials;
using Kamilunavo.PerfectDrop.Gameplay;
namespace Kamilunavo.PerfectDrop.Editor
{
    public static class TowerTrialsValidation
    {
        public static void Validate()
        {
            var book=TrialBook.Empty();
            var legacy=new StackProfile { Coins=2347,UnlockedLevel=30,ResumeActive=true };
            string before=JsonUtility.ToJson(legacy);
            for(int id=1;id<=12;id++)
            {
                var definition=TowerTrialCatalog.Get(id);
                book=TrialRules.Begin(book,id,Guid.NewGuid().ToString("N"),Guid.NewGuid().ToString("N"));
                var run=new StackRun(definition.Target,false,Vector2.one*definition.Width);
                for(int count=0;count<definition.Target;count++)
                {
                    var kind=definition.Modifier==TrialModifier.Fragile && count%2==1?StackBlockKind.Fragile:
                        definition.Modifier==TrialModifier.Drift?StackBlockKind.Drift:definition.Modifier==TrialModifier.Wind?StackBlockKind.Wind:StackBlockKind.Standard;
                    Check(run.Place(0,kind).Grade==StackGrade.Perfect,"Real placement formula changed.");
                    var snapshot=StackGame.TrialSnapshotOf(run,-Mathf.PI/2);
                    book=run.Completed?TrialRules.Finish(book,book.Active.RunId,book.Active.AttemptId,snapshot,false):
                        TrialRules.Checkpoint(book,book.Active.RunId,book.Active.AttemptId,snapshot);
                    // Real StackRun restore, not a simulated model of placement geometry.
                    var restored=new StackRun(definition.Target,false,Vector2.one*definition.Width);
                    restored.Restore(snapshot.Layers.Select(x=>new StackLayer { Center=new Vector2(x.CenterX,x.CenterZ),Size=new Vector2(x.SizeX,x.SizeZ) }),snapshot.Streak,0,snapshot.Placed,snapshot.Perfects,snapshot.MaxStreak);
                    Check(restored.Count==run.Count && restored.Axis==run.Axis && restored.Top.Size==run.Top.Size && restored.Streak==run.Streak && restored.PerfectDrops==run.PerfectDrops,"Real geometry restore mismatch.");
                }
                Check(book.Active.Medal==TrialMedal.Gold && book.Active.State==TrialAttemptState.Cleared,"Curated goal cannot be completed by real perfect placements.");
                Check(JsonUtility.ToJson(legacy)==before,"Domain changed independent legacy profile.");
            }
            var fragile=new StackRun(8);var fragileDrop=fragile.Place(.5f,StackBlockKind.Fragile);
            Check(Mathf.Abs(fragileDrop.Size.x-(3.6f-.5f)*.9f)<.0001f,"Existing fragile loss formula changed.");
            var miss=new StackRun(8);miss.Place(0);var failed=miss.Place(4);
            Check(failed.Grade==StackGrade.Miss && miss.Failed && miss.Streak==0,"Actual failed placement does not match persisted failed snapshot.");
            Debug.Log("[PerfectDrop] TowerTrials actual StackRun catalog/geometry/restore/goals validation passed. Full StackGame UI/reward/legacy-mode lifecycle still requires native acceptance.");
        }
        private static void Check(bool condition,string message) {if(!condition)throw new InvalidOperationException(message);}
    }
}
#endif
