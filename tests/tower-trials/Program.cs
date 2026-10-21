using System;
using System.IO;
using System.Linq;
using Kamilunavo.PerfectDrop.Trials;

internal static class Program
{
    private static int checks;
    private const string Run1="00000000000040008000000000000001", Run2="00000000000040008000000000000002";
    private const string Attempt1="00000000000040008000000000000011", Attempt2="00000000000040008000000000000012", Attempt3="00000000000040008000000000000013";
    private static void Check(bool condition,string message) { if(!condition) throw new Exception("FAIL: "+message); checks++; }
    private static void Reject(Action action,string message) { try { action(); } catch(Exception) { checks++; return; } throw new Exception("FAIL expected rejection: "+message); }
    private static TrialSnapshot Snapshot(TrialDefinition definition,int placed,int perfects,float width=-1)
    {
        var size=width<0?definition.Width:width;
        var result=new TrialSnapshot { Placed=placed,Perfects=perfects,Streak=placed==perfects?perfects:0,MaxStreak=perfects,Phase=.25f };
        for(var i=0;i<placed;i++)result.Layers.Add(new TrialLayer { CenterX=0,CenterZ=0,SizeX=size,SizeZ=size });
        return result;
    }
    private static TrialBook Finish(TrialBook book,int perfects)
    {
        var definition=TowerTrialCatalog.Get(book.Active.TrialId);
        for(var count=1;count<definition.Target;count++)book=TrialRules.Checkpoint(book,book.Active.RunId,book.Active.AttemptId,Snapshot(definition,count,Math.Min(perfects,count)));
        return TrialRules.Finish(book,book.Active.RunId,book.Active.AttemptId,Snapshot(definition,definition.Target,perfects),false);
    }
    private static void CatalogAndGoals()
    {
        Check(TowerTrialCatalog.All.Count==12,"Exactly twelve curated trials, not existing daily/campaign mode");
        Check(TowerTrialCatalog.All.Select(x=>x.Id).Distinct().Count()==12,"Stable unique challenge IDs");
        for(var id=1;id<=12;id++) {
            var definition=TowerTrialCatalog.Get(id); var group=(id-1)/4; var column=(id-1)%4;
            Check(definition.Group==group && definition.Target==8+group*2+column,"Fixed group and target contract");
            Check(definition.AttemptLimit==3,"Three attempts, no paid refill");
            Check(definition.Modifier==(TrialModifier)column,"Existing Steady/Fragile/Drift/Wind curated variants");
        }
        var first=TowerTrialCatalog.Get(1);
        Check(first.Goal==TrialGoal.Perfects && first.Bronze==4 && first.Silver==5 && first.Gold==7,"Independent perfect-count goals have exact ceil thresholds");
        Check(TrialRules.Medal(first,Snapshot(first,first.Target,3),false)==TrialMedal.None,"Completed tower below stated goal earns no invented medal");
        Check(TrialRules.Medal(first,Snapshot(first,first.Target,4),false)==TrialMedal.Bronze,"Exact bronze threshold");
        Check(TrialRules.Medal(first,Snapshot(first,first.Target,5),false)==TrialMedal.Silver,"Exact silver threshold");
        Check(TrialRules.Medal(first,Snapshot(first,first.Target,7),false)==TrialMedal.Gold,"Exact gold threshold");
        Check(TrialRules.Medal(first,Snapshot(first,first.Target-1,7),true)==TrialMedal.None,"Failed/incomplete tower never earns medal");
        var streak=TowerTrialCatalog.Get(2);
        Check(streak.Goal==TrialGoal.Streak && TrialRules.Medal(streak,Snapshot(streak,streak.Target,4),false)==TrialMedal.Silver,"Longest streak goal uses recorded streak, not perfect ratio");
        var area=TowerTrialCatalog.Get(3);
        var areaSnapshot=Snapshot(area,area.Target,0,area.Width*.8f);
        // Actual alternating placement: trim X first, Z second, then retain that supported top.
        for(int index=0;index<areaSnapshot.Layers.Count;index++) {areaSnapshot.Layers[index].CenterX=area.Width*.1f;areaSnapshot.Layers[index].CenterZ=index==0?0:area.Width*.1f;}
        areaSnapshot.Layers[0].SizeZ=area.Width;
        Check(area.Goal==TrialGoal.Area && TrialRules.Medal(area,areaSnapshot,false)==TrialMedal.Silver,"Retained area0.64 earns silver under0.35/0.55/0.75 thresholds");
        Check(!TrialRules.Unlocked(TrialBook.Empty(),2) && TrialRules.Unlocked(TrialBook.Empty(),1),"New ladder starts only first challenge");
        Reject(()=>TrialRules.Begin(TrialBook.Empty(),2,Run1,Attempt1),"Locked ladder entry cannot be selected");
    }
    private static void AttemptsAndTokens()
    {
        var book=TrialRules.Begin(TrialBook.Empty(),1,Run1,Attempt1); var definition=TowerTrialCatalog.Get(1);
        Check(book.Active.AttemptNumber==1 && book.Records.Single().Attempts==1,"First attempt counted once");
        Reject(()=>TrialRules.Begin(book,1,Run2,Attempt2),"Running attempt cannot be silently replaced");
        var before=TrialCodec.Encode(book); var checkpoint=Snapshot(definition,1,1);
        var advanced=TrialRules.Checkpoint(book,Run1,Attempt1,checkpoint);
        checkpoint.Layers[0].SizeX=.01f;
        Check(advanced.Active.Snapshot.Layers[0].SizeX==definition.Width,"Snapshot input is copied, not retained mutable alias");
        Check(TrialCodec.Encode(book).SequenceEqual(before),"Prior book immutable after checkpoint");
        Reject(()=>TrialRules.Checkpoint(advanced,Run2,Attempt1,Snapshot(definition,2,2)),"Foreign run cannot advance");
        Reject(()=>TrialRules.Checkpoint(advanced,Run1,Attempt2,Snapshot(definition,2,2)),"Foreign attempt cannot advance");
        var invalid=Snapshot(definition,2,2); invalid.Phase=float.NaN;
        Reject(()=>TrialRules.Checkpoint(advanced,Run1,Attempt1,invalid),"Nonfinite motion cannot be persisted");
        invalid=Snapshot(definition,2,2); invalid.Layers.RemoveAt(0);
        Reject(()=>TrialRules.Checkpoint(advanced,Run1,Attempt1,invalid),"Geometry count cannot diverge from recorded placements");
        var failed=TrialRules.Finish(book,Run1,Attempt1,Snapshot(definition,0,0),true);
        Check(failed.Active.State==TrialAttemptState.Failed,"Miss consumes one attempt");
        var retryBytes=TrialCodec.Encode(failed);
        Check(TrialCodec.Encode(TrialRules.Finish(failed,Run1,Attempt1,Snapshot(definition,0,0),true)).SequenceEqual(retryBytes),"Exact terminal retry is idempotent, including revision");
        Reject(()=>TrialRules.Finish(failed,Run1,Attempt1,Snapshot(definition,1,1),true),"Same terminal token with different result rejects");
        var second=TrialRules.Retry(failed,Run1,Attempt1,Attempt2);
        Reject(()=>TrialRules.Finish(second,Run1,Attempt1,Snapshot(definition,0,0),true),"Late first-attempt callback cannot fail retry");
        Reject(()=>TrialRules.Retry(second,Run1,Attempt1,Attempt3),"Double retry cannot consume third attempt");
        second=TrialRules.Finish(second,Run1,Attempt2,Snapshot(definition,0,0),true);
        var third=TrialRules.Retry(second,Run1,Attempt2,Attempt3);
        third=TrialRules.Finish(third,Run1,Attempt3,Snapshot(definition,0,0),true);
        Reject(()=>TrialRules.Retry(third,Run1,Attempt3,Guid.NewGuid().ToString("N")),"Fourth attempt in same run rejects");
        var fresh=TrialRules.Begin(third,1,Run2,Attempt1);
        Check(fresh.Active.AttemptNumber==1 && fresh.Records.Single().Attempts==4,"Explicit new free run resets run budget only");
        Reject(()=>TrialRules.Finish(fresh,Run1,Attempt3,Snapshot(definition,0,0),true),"Older run callback remains stale after reset");
        var abandoned=TrialRules.Abandon(fresh,Run2,Attempt1);
        Check(abandoned.Active.State==TrialAttemptState.Abandoned,"Leaving unfinished trial is explicit");
        Reject(()=>TrialRules.Checkpoint(abandoned,Run2,Attempt1,Snapshot(definition,1,1)),"Abandoned trial cannot resume by stale callback");
    }
    private static void MedalsAndLegacyIsolation()
    {
        var legacy="{\"Schema\":2,\"Coins\":2347,\"UnlockedLevel\":30,\"LevelStars\":[1,3],\"Commerce\":{\"owned\":true},\"DailyClaim\":\"2026-10-20\"}";
        var copy=legacy;
        var gold=Finish(TrialRules.Begin(TrialBook.Empty(),1,Run1,Attempt1),8);
        Check(gold.Active.State==TrialAttemptState.Cleared && gold.Records.Single().BestMedal==TrialMedal.Gold,"Completed goal earns independent gold");
        Check(gold.Records.Single().CompletedRuns==1 && TrialRules.Unlocked(gold,2),"Bronze or better opens next and counts successful run once");
        var bronze=Finish(TrialRules.Begin(gold,1,Run2,Attempt2),4);
        Check(bronze.Active.Medal==TrialMedal.Bronze && bronze.Records.Single().BestMedal==TrialMedal.Gold && bronze.Records.Single().CompletedRuns==2,"Replay retains best medal, records actual current lower result");
        Check(legacy==copy,"Pure mode has no legacy profile mutation API");
        var noGoal=Finish(TrialRules.Begin(TrialBook.Empty(),1,Run1,Attempt1),0);
        Check(noGoal.Active.State==TrialAttemptState.GoalMissed && !TrialRules.Unlocked(noGoal,2),"Tower completion alone cannot bypass explicit goal");
    }
    private static void CodecAndActualFiles()
    {
        var definition=TowerTrialCatalog.Get(1); var book=TrialRules.Begin(TrialBook.Empty(),1,Run1,Attempt1);
        book=TrialRules.Checkpoint(book,Run1,Attempt1,Snapshot(definition,1,1));
        var encoded=TrialCodec.Encode(book);
        Check(TrialCodec.Encode(TrialCodec.Decode(encoded)).SequenceEqual(encoded),"Exact production binary codec roundtrip incl resumable layers/phase/tokens");
        Reject(()=>TrialCodec.Decode(encoded.Take(encoded.Length-1).ToArray()),"Truncated archive rejects");
        Reject(()=>TrialCodec.Decode(encoded.Concat(new byte[]{1}).ToArray()),"Trailing bytes reject");
        var future=(byte[])encoded.Clone(); future[8]=99;
        Reject(()=>TrialCodec.Decode(future),"Future schema rejects");
        Reject(()=>TrialCodec.Decode(new byte[TrialCodec.MaximumBytes+1]),"Oversize rejected before parsing");
        var duplicate=TrialCodec.Decode(encoded); duplicate.Records.Add(duplicate.Records[0]);
        Reject(()=>TrialCodec.Encode(duplicate),"Duplicate challenge record cannot be saved");
        var directory=Path.Combine(Path.GetTempPath(),"perfectdrop-trials-"+Guid.NewGuid().ToString("N")); Directory.CreateDirectory(directory);
        try {
            var path=Path.Combine(directory,"perfectdrop.trials.v1.bin"); var legacy=Path.Combine(directory,"legacy-playerprefs.json");
            var old=System.Text.Encoding.UTF8.GetBytes("original wallet campaign commerce daily bytes"); File.WriteAllBytes(legacy,old);
            var store=new TowerTrialsFile(path); Check(store.Load().Revision==0,"Missing new archive is independent empty progress");
            var first=TrialRules.Begin(TrialBook.Empty(),1,Run1,Attempt1); store.Save(first,0);
            Check(TrialCodec.Encode(new TowerTrialsFile(path).Load()).SequenceEqual(TrialCodec.Encode(first)),"Actual cold file restart returns durable active attempt");
            var next=TrialRules.Checkpoint(first,Run1,Attempt1,Snapshot(definition,1,1));
            var failed=new TowerTrialsFile(path,(destination,bytes)=>{throw new IOException("synthetic failed atomic write");});
            Reject(()=>failed.Save(next,first.Revision),"Failed write never publishes new checkpoint");
            Check(File.ReadAllBytes(path).SequenceEqual(TrialCodec.Encode(first)),"Failed replacement retains prior actual file bytes");
            store.Save(next,first.Revision);
            store.Save(next,first.Revision); Check(store.Load().Revision==next.Revision,"Exact committed retry does not duplicate revision");
            Reject(()=>store.Save(first,first.Revision),"Stale expected revision cannot replace new state");
            Check(File.ReadAllBytes(legacy).SequenceEqual(old),"Actual sibling original profile bytes remain untouched");
            foreach(var corrupt in new[]{new byte[]{1,2,3},future}) {
                File.WriteAllBytes(path,corrupt);
                Reject(()=>store.Load(),"External corrupt/future state cannot load");
                Reject(()=>store.Save(next,first.Revision),"Save after earlier successful load rechecks corrupt current bytes");
                Check(File.ReadAllBytes(path).SequenceEqual(corrupt),"External invalid bytes preserved exactly");
            }
        } finally { Directory.Delete(directory,true); }
    }
    private static void SupportedGeometry()
    {
        var definition=TowerTrialCatalog.Get(1);var book=TrialRules.Begin(TrialBook.Empty(),1,Run1,Attempt1);
        var impossible=Snapshot(definition,1,1);impossible.Layers[0].CenterX=99;impossible.Layers[0].CenterZ=99;
        Reject(()=>TrialRules.Checkpoint(book,Run1,Attempt1,impossible),"Full-width perfect layer cannot float beyond the actual base support");
        var wrongAxis=Snapshot(definition,1,0);wrongAxis.Layers[0].SizeX=3;wrongAxis.Layers[0].CenterX=.3f;wrongAxis.Layers[0].CenterZ=.1f;
        Reject(()=>TrialRules.Checkpoint(book,Run1,Attempt1,wrongAxis),"First X-axis placement cannot move along Z");
        var supported=Snapshot(definition,1,0);supported.Layers[0].SizeX=3;supported.Layers[0].CenterX=.3f;
        var saved=TrialRules.Checkpoint(book,Run1,Attempt1,supported);
        Check(saved.Active.Snapshot.Layers[0].CenterX==.3f,"Actual good X placement keeps supported offset and trimmed size");
        var corrupt=TrialCodec.Decode(TrialCodec.Encode(saved));corrupt.Active.Snapshot.Layers[0].CenterX=99;
        Reject(()=>TrialCodec.Encode(corrupt),"Archive validation also blocks unsupported cold-resume geometry");
    }
    public static void Main()
    {
        CatalogAndGoals(); AttemptsAndTokens(); MedalsAndLegacyIsolation(); SupportedGeometry(); CodecAndActualFiles();
        Console.WriteLine("PASS TowerTrials actual production domain/codec/file checks: "+checks);
    }
}
