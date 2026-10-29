using System;
using System.Collections.Generic;
using System.Linq;

namespace Kamilunavo.PerfectDrop.Trials
{
    public enum TrialModifier { Steady, Fragile, Drift, Wind }
    public enum TrialGoal { Perfects, Streak, Area }
    public enum TrialMedal { None, Bronze, Silver, Gold }
    public enum TrialAttemptState { Running, Failed, GoalMissed, Cleared, Abandoned }
    public sealed class TrialDefinition
    {
        public int Id, Group, Target, AttemptLimit;
        public TrialModifier Modifier;
        public TrialGoal Goal;
        public float Width, Speed, Bronze, Silver, Gold;
    }
    public sealed class TrialLayer { public float CenterX, CenterZ, SizeX, SizeZ; }
    public sealed class TrialSnapshot
    {
        public int Placed, Perfects, Streak, MaxStreak;
        public float Phase;
        public List<TrialLayer> Layers = new List<TrialLayer>();
    }
    public sealed class TrialSession
    {
        public string RunId, AttemptId;
        public int TrialId, AttemptNumber;
        public TrialAttemptState State;
        public TrialSnapshot Snapshot = new TrialSnapshot();
        public TrialMedal Medal;
    }
    public sealed class TrialRecord { public int TrialId, CompletedRuns, Attempts; public TrialMedal BestMedal; }
    public sealed class TrialBook
    {
        public int Schema = 1, Revision;
        public List<TrialRecord> Records = new List<TrialRecord>();
        public TrialSession Active;
        public static TrialBook Empty() { return new TrialBook(); }
    }
    public static class TowerTrialCatalog
    {
        // Return fresh definitions so UI callers cannot mutate the authoritative catalog.
        public static IList<TrialDefinition> All { get { return Enumerable.Range(1,12).Select(Get).ToList().AsReadOnly(); } }
        public static TrialDefinition Get(int id)
        {
            if(id<1 || id>12) throw new ArgumentOutOfRangeException(nameof(id));
            int group=(id-1)/4, column=(id-1)%4, target=8+group*2+column;
            var goal=(TrialGoal)((id-1)%3);
            return new TrialDefinition { Id=id,Group=group,Target=target,AttemptLimit=3,Modifier=(TrialModifier)column,
                Goal=goal,Width=3.6f-.2f*group,Speed=1.10f+.15f*group+.04f*column,
                Bronze=goal==TrialGoal.Perfects?(float)Math.Ceiling(target*.4):goal==TrialGoal.Streak?2:.35f,
                Silver=goal==TrialGoal.Perfects?(float)Math.Ceiling(target*.6):goal==TrialGoal.Streak?4:.55f,
                Gold=goal==TrialGoal.Perfects?(float)Math.Ceiling(target*.8):goal==TrialGoal.Streak?6:.75f };
        }
    }
    public static class TrialRules
    {
        private static void Require(bool condition,string message) { if(!condition) throw new InvalidOperationException(message); }
        internal static bool Finite(float value) { return !float.IsNaN(value) && !float.IsInfinity(value); }
        internal static void Token(string token)
        {
            Guid parsed;
            Require(token!=null && token.Length==32 && Guid.TryParseExact(token,"N",out parsed) && parsed!=Guid.Empty,"Invalid trial identity.");
        }
        internal static void SnapshotValid(TrialDefinition definition,TrialSnapshot value)
        {
            Require(value!=null && value.Layers!=null,"Missing trial snapshot.");
            Require(value.Placed>=0 && value.Placed<=definition.Target && value.Layers.Count==value.Placed,"Invalid layer count.");
            Require(value.Perfects>=0 && value.Perfects<=value.Placed && value.Streak>=0 && value.Streak<=value.Perfects &&
                value.MaxStreak>=value.Streak && value.MaxStreak<=value.Perfects && (value.Perfects==0 || value.MaxStreak>0),"Invalid trial statistics.");
            Require(Finite(value.Phase) && Math.Abs(value.Phase)<=10000000f,"Invalid motion phase.");
            float previousX=definition.Width,previousZ=definition.Width,previousCenterX=0,previousCenterZ=0;
            int index=0;const float geometryEpsilon=.0001f;
            foreach(var layer in value.Layers)
            {
                Require(layer!=null && Finite(layer.CenterX) && Finite(layer.CenterZ) && Math.Abs(layer.CenterX)<=100 && Math.Abs(layer.CenterZ)<=100 &&
                    Finite(layer.SizeX) && Finite(layer.SizeZ) && layer.SizeX>0 && layer.SizeZ>0 &&
                    layer.SizeX<=previousX+.00001f && layer.SizeZ<=previousZ+.00001f,"Invalid trial geometry.");
                // StackRules.Evaluate retains a rectangle inside the previous top;
                // Fragile only trims it further. Neither can move/shrink the inactive axis.
                Require(Math.Abs(layer.CenterX-previousCenterX)<=(previousX-layer.SizeX)*.5f+geometryEpsilon &&
                    Math.Abs(layer.CenterZ-previousCenterZ)<=(previousZ-layer.SizeZ)*.5f+geometryEpsilon,"Trial layer is outside its support.");
                bool xAxis=index%2==0;
                Require(xAxis?Math.Abs(layer.CenterZ-previousCenterZ)<=geometryEpsilon && Math.Abs(layer.SizeZ-previousZ)<=geometryEpsilon:
                    Math.Abs(layer.CenterX-previousCenterX)<=geometryEpsilon && Math.Abs(layer.SizeX-previousX)<=geometryEpsilon,"Trial layer changed the inactive axis.");
                previousX=layer.SizeX; previousZ=layer.SizeZ;previousCenterX=layer.CenterX;previousCenterZ=layer.CenterZ;index++;
            }
        }
        private static bool SameLayer(TrialLayer a,TrialLayer b)
        { return a.CenterX==b.CenterX && a.CenterZ==b.CenterZ && a.SizeX==b.SizeX && a.SizeZ==b.SizeZ; }
        internal static bool SameSnapshot(TrialSnapshot a,TrialSnapshot b)
        {
            return a.Placed==b.Placed && a.Perfects==b.Perfects && a.Streak==b.Streak && a.MaxStreak==b.MaxStreak && a.Phase==b.Phase &&
                a.Layers.Count==b.Layers.Count && a.Layers.Select((layer,index)=>SameLayer(layer,b.Layers[index])).All(x=>x);
        }
        private static void Transition(TrialSnapshot prior,TrialSnapshot next,bool failed)
        {
            Require(next.Placed>=prior.Placed && next.Placed<=prior.Placed+1,"Out-of-order placement.");
            Require(next.Perfects>=prior.Perfects && next.Perfects<=prior.Perfects+next.Placed-prior.Placed && next.MaxStreak>=prior.MaxStreak,"Regressed placement statistics.");
            for(int i=0;i<prior.Layers.Count;i++) Require(SameLayer(prior.Layers[i],next.Layers[i]),"Existing layers cannot change.");
            if(next.Placed==prior.Placed)
                Require(next.Perfects==prior.Perfects && next.MaxStreak==prior.MaxStreak && next.Streak==(failed?0:prior.Streak),"Motion checkpoint changed placement statistics.");
            else
                Require(!failed && next.Streak==(next.Perfects>prior.Perfects?prior.Streak+1:0) && next.MaxStreak==Math.Max(prior.MaxStreak,next.Streak),"Invalid placement statistics.");
        }
        internal static TrialSnapshot CopySnapshot(TrialSnapshot value)
        {
            return new TrialSnapshot { Placed=value.Placed,Perfects=value.Perfects,Streak=value.Streak,MaxStreak=value.MaxStreak,Phase=value.Phase,
                Layers=value.Layers.Select(layer=>new TrialLayer { CenterX=layer.CenterX,CenterZ=layer.CenterZ,SizeX=layer.SizeX,SizeZ=layer.SizeZ }).ToList() };
        }
        private static TrialBook Copy(TrialBook value)
        {
            var copy=new TrialBook { Schema=value.Schema,Revision=value.Revision,
                Records=value.Records.Select(record=>new TrialRecord { TrialId=record.TrialId,BestMedal=record.BestMedal,CompletedRuns=record.CompletedRuns,Attempts=record.Attempts }).ToList() };
            if(value.Active!=null) copy.Active=new TrialSession { RunId=value.Active.RunId,AttemptId=value.Active.AttemptId,TrialId=value.Active.TrialId,
                AttemptNumber=value.Active.AttemptNumber,State=value.Active.State,Medal=value.Active.Medal,Snapshot=CopySnapshot(value.Active.Snapshot) };
            return copy;
        }
        private static TrialBook Advance(TrialBook value)
        { Validate(value); var copy=Copy(value); copy.Revision=checked(copy.Revision+1); return copy; }
        internal static void Validate(TrialBook book)
        {
            Require(book!=null && book.Schema==1 && book.Revision>=0 && book.Records!=null && book.Records.Count<=12,"Invalid or future trial archive.");
            var ids=new HashSet<int>();
            foreach(var record in book.Records)
            {
                Require(record!=null && ids.Add(record.TrialId),"Duplicate trial record."); TowerTrialCatalog.Get(record.TrialId);
                Require(record.Attempts>=1 && record.CompletedRuns>=0 && record.CompletedRuns<=record.Attempts && (int)record.BestMedal>=0 && (int)record.BestMedal<=3 &&
                    ((record.CompletedRuns==0)==(record.BestMedal==TrialMedal.None)),"Invalid trial record.");
                if(record.TrialId>1) Require(book.Records.Any(x=>x!=null && x.TrialId==record.TrialId-1 && x.BestMedal>=TrialMedal.Bronze),"Invalid locked trial record.");
            }
            var active=book.Active;
            if(active==null) return;
            Token(active.RunId); Token(active.AttemptId);
            var definition=TowerTrialCatalog.Get(active.TrialId);
            Require(active.AttemptNumber>=1 && active.AttemptNumber<=definition.AttemptLimit && (int)active.State>=0 && (int)active.State<=4 &&
                (int)active.Medal>=0 && (int)active.Medal<=3,"Invalid trial session.");
            var own=book.Records.SingleOrDefault(x=>x.TrialId==active.TrialId);
            Require(own!=null && own.Attempts>=active.AttemptNumber,"Missing active trial record.");
            SnapshotValid(definition,active.Snapshot);
            if(active.State==TrialAttemptState.Running) Require(active.Snapshot.Placed<definition.Target && active.Medal==TrialMedal.None,"Terminal tower marked running.");
            if(active.State==TrialAttemptState.Failed) Require(active.Snapshot.Placed<definition.Target && active.Snapshot.Streak==0 && active.Medal==TrialMedal.None,"Invalid failed trial.");
            if(active.State==TrialAttemptState.Abandoned) Require(active.Medal==TrialMedal.None,"Abandoned trial has medal.");
            if(active.State==TrialAttemptState.Cleared || active.State==TrialAttemptState.GoalMissed)
            {
                Require(active.Snapshot.Placed==definition.Target && active.Medal==Medal(definition,active.Snapshot,false),"Terminal medal mismatch.");
                Require((active.State==TrialAttemptState.Cleared)==(active.Medal!=TrialMedal.None) && own.BestMedal>=active.Medal,"Terminal state mismatch.");
            }
        }
        public static bool Unlocked(TrialBook book,int id)
        { TowerTrialCatalog.Get(id); return id==1 || book.Records.Any(x=>x.TrialId==id-1 && x.BestMedal>=TrialMedal.Bronze); }
        public static TrialMedal Medal(TrialDefinition definition,TrialSnapshot snapshot,bool failed)
        {
            SnapshotValid(definition,snapshot);
            if(failed || snapshot.Placed!=definition.Target) return TrialMedal.None;
            float value=definition.Goal==TrialGoal.Perfects?snapshot.Perfects:definition.Goal==TrialGoal.Streak?snapshot.MaxStreak:
                snapshot.Layers[snapshot.Layers.Count-1].SizeX*snapshot.Layers[snapshot.Layers.Count-1].SizeZ/(definition.Width*definition.Width);
            return value>=definition.Gold?TrialMedal.Gold:value>=definition.Silver?TrialMedal.Silver:value>=definition.Bronze?TrialMedal.Bronze:TrialMedal.None;
        }
        private static TrialSession Current(TrialBook book,string run,string attempt)
        { Validate(book); Token(run); Token(attempt); Require(book.Active!=null && book.Active.RunId==run && book.Active.AttemptId==attempt,"Stale trial callback."); return book.Active; }
        public static TrialBook Begin(TrialBook book,int trialId,string runId,string attemptId)
        {
            Validate(book); Token(runId); Token(attemptId); Require(Unlocked(book,trialId),"Trial is locked.");
            Require(book.Active==null || (book.Active.State!=TrialAttemptState.Running && book.Active.RunId!=runId),"End the previous trial before starting another.");
            var result=Advance(book); var record=result.Records.SingleOrDefault(x=>x.TrialId==trialId);
            if(record==null) { record=new TrialRecord { TrialId=trialId }; result.Records.Add(record); }
            record.Attempts=checked(record.Attempts+1);
            result.Active=new TrialSession { TrialId=trialId,RunId=runId,AttemptId=attemptId,AttemptNumber=1,State=TrialAttemptState.Running };
            Validate(result); return result;
        }
        public static TrialBook Retry(TrialBook book,string runId,string oldAttemptId,string newAttemptId)
        {
            var active=Current(book,runId,oldAttemptId); Token(newAttemptId);
            Require((active.State==TrialAttemptState.Failed || active.State==TrialAttemptState.GoalMissed) && active.AttemptNumber<3 && oldAttemptId!=newAttemptId,"No retry remains for this attempt.");
            var result=Advance(book); result.Active.AttemptNumber++; result.Active.AttemptId=newAttemptId; result.Active.State=TrialAttemptState.Running;
            result.Active.Medal=TrialMedal.None; result.Active.Snapshot=new TrialSnapshot();
            var record=result.Records.Single(x=>x.TrialId==active.TrialId); record.Attempts=checked(record.Attempts+1);
            Validate(result); return result;
        }
        public static TrialBook Checkpoint(TrialBook book,string runId,string attemptId,TrialSnapshot snapshot)
        {
            var active=Current(book,runId,attemptId); var definition=TowerTrialCatalog.Get(active.TrialId);
            Require(active.State==TrialAttemptState.Running,"Trial is terminal."); SnapshotValid(definition,snapshot);
            Require(snapshot.Placed<definition.Target,"Completed tower requires a terminal result."); Transition(active.Snapshot,snapshot,false);
            if(SameSnapshot(active.Snapshot,snapshot)) return Copy(book);
            var result=Advance(book); result.Active.Snapshot=CopySnapshot(snapshot); Validate(result); return result;
        }
        public static TrialBook Finish(TrialBook book,string runId,string attemptId,TrialSnapshot snapshot,bool failed)
        {
            var active=Current(book,runId,attemptId); var definition=TowerTrialCatalog.Get(active.TrialId); SnapshotValid(definition,snapshot);
            if(active.State!=TrialAttemptState.Running)
            {
                Require((failed && active.State==TrialAttemptState.Failed || !failed && (active.State==TrialAttemptState.Cleared || active.State==TrialAttemptState.GoalMissed)) &&
                    SameSnapshot(active.Snapshot,snapshot),"Changed terminal result."); return Copy(book);
            }
            Require(failed?snapshot.Placed<definition.Target:snapshot.Placed==definition.Target,"Invalid terminal placement.");
            Transition(active.Snapshot,snapshot,failed);
            var result=Advance(book); result.Active.Snapshot=CopySnapshot(snapshot); result.Active.Medal=Medal(definition,snapshot,failed);
            result.Active.State=failed?TrialAttemptState.Failed:result.Active.Medal==TrialMedal.None?TrialAttemptState.GoalMissed:TrialAttemptState.Cleared;
            if(result.Active.Medal!=TrialMedal.None)
            {
                var record=result.Records.Single(x=>x.TrialId==active.TrialId); record.CompletedRuns=checked(record.CompletedRuns+1);
                if(result.Active.Medal>record.BestMedal) record.BestMedal=result.Active.Medal;
            }
            Validate(result); return result;
        }
        public static TrialBook Abandon(TrialBook book,string runId,string attemptId)
        {
            var active=Current(book,runId,attemptId); Require(active.State==TrialAttemptState.Running,"Only running trials can be abandoned.");
            var result=Advance(book); result.Active.State=TrialAttemptState.Abandoned; Validate(result); return result;
        }
    }
}
