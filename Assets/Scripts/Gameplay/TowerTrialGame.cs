using System;
using System.IO;
using System.Linq;
using UnityEngine;
using Kamilunavo.PerfectDrop.Trials;
using Kamilunavo.PerfectDrop.Visuals;
using Kamilunavo.PerfectDrop.Feedback;

namespace Kamilunavo.PerfectDrop.Gameplay
{
    public sealed partial class StackGame
    {
        private TowerTrialsFile _trialFile;
        private TrialBook _trials=TrialBook.Empty(), _pendingTrial;
        private Action _pendingTrialPublish;
        private StackRun _legacyRun;
        private StackLevel _legacyLevel;
        private float _legacyPhase;
        private bool _legacyStarted;
        private int _legacyStars,_legacyBonus;
        public bool TrialMode { get; private set; }
        public string TrialSaveError { get; private set; }
        public TrialBook Trials => TrialCodec.Decode(TrialCodec.Encode(_trials));
        public TrialDefinition TrialDefinition => TrialMode?TowerTrialCatalog.Get(_trials.Active.TrialId):null;
        private StackBlockKind TrialKind => TrialDefinition.Modifier==TrialModifier.Fragile?(Run.Count%2==1?StackBlockKind.Fragile:StackBlockKind.Standard):
            TrialDefinition.Modifier==TrialModifier.Drift?StackBlockKind.Drift:TrialDefinition.Modifier==TrialModifier.Wind?StackBlockKind.Wind:StackBlockKind.Standard;
        private void LoadTrials()
        {
            _trialFile=new TowerTrialsFile(Path.Combine(Application.persistentDataPath,"perfectdrop.trials.v1.bin"));
            try { _trials=_trialFile.Load(); }
            catch(Exception exception) { TrialSaveError=exception.Message; }
        }
        private bool CommitTrial(TrialBook proposed,Action publish)
        {
            try
            {
                _trialFile.Save(proposed,_trials.Revision);
                bool recovering=TrialSaveError!=null;
                _trials=proposed; _pendingTrial=null; _pendingTrialPublish=null; TrialSaveError=null;
                if(recovering)Hud?.HideTrialPanels();
                publish?.Invoke(); return true;
            }
            catch(Exception exception)
            {
                _pendingTrial=proposed;_pendingTrialPublish=publish;TrialSaveError=exception.Message;
                Hud?.ShowTrialSaveFailure(); return false;
            }
        }
        public void RetryTrialSave()
        {
            if(NativePresentationActive)return;
            if(_pendingTrial!=null) { var proposed=_pendingTrial;var publish=_pendingTrialPublish;CommitTrial(proposed,publish);return; }
            try { _trials=_trialFile.Load();TrialSaveError=null;Hud.ShowTrials(); }
            catch(Exception exception) {TrialSaveError=exception.Message;Hud.ShowTrialSaveFailure();}
        }
        public void ReloadSavedTrials()
        {
            if(NativePresentationActive)return;
            try
            {
                var saved=_trialFile.Load();
                _trials=saved;_pendingTrial=null;_pendingTrialPublish=null;TrialSaveError=null;
                RestoreLegacyTrialView();Hud.ShowTrials();
            }
            catch(Exception exception) {TrialSaveError=exception.Message;Hud.ShowTrialSaveFailure();}
        }
        public void StartTrial(int id)
        {
            if(NativePresentationActive || TrialSaveError!=null || _trials.Active?.State==TrialAttemptState.Running)return;
            try
            {
                var proposed=TrialRules.Begin(_trials,id,Guid.NewGuid().ToString("N"),Guid.NewGuid().ToString("N"));
                CommitTrial(proposed,InstallTrial);
            }
            catch(Exception exception) {TrialSaveError=exception.Message;Hud.ShowTrialSaveFailure();}
        }
        public void ResumeTrial()
        {
            if(NativePresentationActive || TrialSaveError!=null || _trials.Active==null || _trials.Active.State!=TrialAttemptState.Running)return;
            InstallTrial();
        }
        public void RetryTrial()
        {
            if(!TrialMode || NativePresentationActive || TrialSaveError!=null)return;
            try
            {
                var active=_trials.Active;
                if((active.State!=TrialAttemptState.Failed && active.State!=TrialAttemptState.GoalMissed) || active.AttemptNumber>=3)return;
                var proposed=TrialRules.Retry(_trials,active.RunId,active.AttemptId,Guid.NewGuid().ToString("N"));
                CommitTrial(proposed,InstallTrial);
            }
            catch(Exception exception) {TrialSaveError=exception.Message;Hud.ShowTrialSaveFailure();}
        }
        public void AbandonTrial()
        {
            if(NativePresentationActive || TrialSaveError!=null || _trials.Active==null || _trials.Active.State!=TrialAttemptState.Running)return;
            var active=_trials.Active;
            CommitTrial(TrialRules.Abandon(_trials,active.RunId,active.AttemptId),()=>{RestoreLegacyTrialView();Hud.ShowTrials();});
        }
        private void InstallTrial()
        {
            if(!TrialMode)
            {
                // Serialize only the existing run BEFORE changing its in-memory view.
                Save();_legacyRun=Run;_legacyLevel=Level;_legacyPhase=_phase;_legacyStarted=_hasStarted;_legacyStars=LastStars;_legacyBonus=LastBonus;
            }
            TrialMode=true;
            var definition=TrialDefinition;var snapshot=_trials.Active.Snapshot;
            Level=new StackLevel(definition.Target,definition.Width,definition.Speed,(int)definition.Goal);
            Run=RestoreTrialRun(definition,snapshot);_phase=snapshot.Phase;_hasStarted=true;LastStars=LastBonus=0;_nextDrop=0;
            RebuildTrialView();Hud.HideMenus();Hud.ResetMessage();UpdateCamera(true);
        }
        private static StackRun RestoreTrialRun(TrialDefinition definition,TrialSnapshot snapshot)
        {
            var run=new StackRun(definition.Target,false,Vector2.one*definition.Width);
            run.Restore(snapshot.Layers.Select(x=>new StackLayer { Center=new Vector2(x.CenterX,x.CenterZ),Size=new Vector2(x.SizeX,x.SizeZ) }),snapshot.Streak,0,snapshot.Placed,snapshot.Perfects,snapshot.MaxStreak);
            return run;
        }
        public static TrialSnapshot TrialSnapshotOf(StackRun run,float phase)
        {
            return new TrialSnapshot { Placed=run.Count,Perfects=run.PerfectDrops,Streak=run.Streak,MaxStreak=run.MaxStreak,Phase=phase,
                Layers=run.Layers.Select(x=>new TrialLayer { CenterX=x.Center.x,CenterZ=x.Center.y,SizeX=x.Size.x,SizeZ=x.Size.y }).ToList() };
        }
        private void RebuildTrialView()
        {
            foreach(var block in _placed)if(block!=null)Destroy(block);_placed.Clear();
            if(_moving!=null)Destroy(_moving.gameObject);_moving=null;
            for(int i=0;i<Run.Layers.Count;i++)_placed.Add(CreateBlock("Placed_"+(i+1),Run.Layers[i].Center,Run.Layers[i].Size,i*LayerHeight,i+1));
            if(!Run.Failed && !Run.Completed)SpawnMoving();
            WorldArt.SetChapter(TrialMode?TrialDefinition.Group:Level.Chapter);
        }
        private bool SaveTrial()
        {
            if(TrialSaveError!=null)return false;
            var active=_trials.Active;
            if(active.State!=TrialAttemptState.Running)return true;
            try
            {
                var proposed=TrialRules.Checkpoint(_trials,active.RunId,active.AttemptId,TrialSnapshotOf(Run,_phase));
                return CommitTrial(proposed,()=>{if(Hud!=null)Hud.Refresh();});
            }
            catch(Exception exception) {TrialSaveError=exception.Message;Hud?.ShowTrialSaveFailure();return false;}
        }
        private bool LeaveTrial()
        {
            if(!TrialMode)return true;
            if(!SaveTrial())return false;
            RestoreLegacyTrialView();return true;
        }
        private void RestoreLegacyTrialView()
        {
            if(!TrialMode)return;
            TrialMode=false;Run=_legacyRun;Level=_legacyLevel;_phase=_legacyPhase;_hasStarted=_legacyStarted;LastStars=_legacyStars;LastBonus=_legacyBonus;
            _legacyRun=null;RebuildTrialView();UpdateCamera(true);
        }
        private void DropTrial()
        {
            if(TrialSaveError!=null || _trials.Active.State!=TrialAttemptState.Running)return;
            _nextDrop=Time.unscaledTime+.06f;
            // Evaluate against a private real StackRun; durable checkpoint precedes all visible/run/medal publication.
            var candidate=RestoreTrialRun(TrialDefinition,TrialSnapshotOf(Run,_phase));
            var drop=candidate.Place(MovingOffset,TrialKind);
            var nextPhase=candidate.Failed || candidate.Completed?_phase:-Mathf.PI/2;
            var snapshot=TrialSnapshotOf(candidate,nextPhase);var active=_trials.Active;
            var proposed=candidate.Failed || candidate.Completed?TrialRules.Finish(_trials,active.RunId,active.AttemptId,snapshot,candidate.Failed):TrialRules.Checkpoint(_trials,active.RunId,active.AttemptId,snapshot);
            CommitTrial(proposed,()=>
            {
                Run=candidate;_phase=nextPhase;
                if(drop.Grade==StackGrade.Miss)
                {if(_moving!=null)_moving.gameObject.AddComponent<StackOffcut>();_moving=null;_feedback.PlayRecovery();}
                else
                {
                    if(_moving!=null)Destroy(_moving.gameObject);_moving=null;
                    var height=(Run.Layers.Count-1)*LayerHeight;
                    _placed.Add(CreateBlock("Placed_"+Run.Count,drop.Center,drop.Size,height,Run.Count));
                    if(drop.Grade==StackGrade.Good && drop.CutSize.x>0 && drop.CutSize.y>0)
                        CreateBlock("Overhang",drop.CutCenter,drop.CutSize,height,Run.Count).AddComponent<StackOffcut>();
                    _feedback.PlayLanding(drop.Grade==StackGrade.Perfect?LandingGrade.Perfect:LandingGrade.Good);
                    if(!Run.Completed)SpawnMoving();
                }
                Hud.Grade(drop.Grade);
                if(Run.Completed || Run.Failed)Hud.ShowTrialResult();else {Hud.HideTrialPanels();Hud.Refresh();}
            });
        }
    }
}
