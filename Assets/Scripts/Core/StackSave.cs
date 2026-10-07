using System;
using System.Collections.Generic;
using UnityEngine;
using Kamilunavo.PerfectDrop.Gameplay;

namespace Kamilunavo.PerfectDrop
{
    [Serializable]
    public sealed class StackProfile
    {
        public int Schema = 2, Best, Coins, Towers, Streak, RunCoins;
        public int UnlockedLevel=1, RunLevel=1, TotalPlaced, PerfectDrops, MaxStreak, EndlessBest, OwnedStyles=1, Style;
        public int[] LevelStars = new int[30];
        public bool RunEndless;
        public bool RunChallenge, ChallengeRewarded;
        public string RunChallengeDate="",ChallengeDate="";
        public int ChallengeStars;
        public StackPowers Powers = new();
        public bool ResumeActive;
        public float Phase;
        public string DailyClaim = "";
        public List<StackLayer> Layers = new();
    }
    public static class StackSave
    {
        public const string Key = "perfectdrop.stack.profile.v1";
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        public static string QaKey;
        private static string StorageKey => string.IsNullOrEmpty(QaKey) ? Key : QaKey;
#else
        private static string StorageKey => Key;
#endif
        public static StackProfile Load() => Parse(PlayerPrefs.GetString(StorageKey, ""));
        public static StackProfile Parse(string json)
        {
            if (string.IsNullOrEmpty(json)) return new StackProfile();
            StackProfile data;
            try { data = JsonUtility.FromJson<StackProfile>(json); }
            catch (ArgumentException) { return new StackProfile(); }
            if (data == null || (data.Schema != 1 && data.Schema != 2)) return new StackProfile();
            if(data.Schema==1)
            { data.Schema=2; data.ResumeActive=false; data.Layers=new(); data.Streak=data.RunCoins=data.TotalPlaced=0; data.Phase=0; }
            data.Best = Mathf.Clamp(data.Best, 0, StackRules.Target);
            data.Coins = Mathf.Max(0, data.Coins);
            data.Towers = Mathf.Max(0, data.Towers);
            data.UnlockedLevel=Mathf.Clamp(data.UnlockedLevel,1,30);
            data.RunLevel=Mathf.Clamp(data.RunLevel,1,data.UnlockedLevel);
            data.EndlessBest=Mathf.Max(0,data.EndlessBest);
            data.OwnedStyles=(data.OwnedStyles&15)|1;
            data.Style=Mathf.Clamp(data.Style,0,3);
            if((data.OwnedStyles&(1<<data.Style))==0) data.Style=0;
            if(data.LevelStars==null || data.LevelStars.Length!=30) data.LevelStars=new int[30];
            for(var i=0;i<30;i++) data.LevelStars[i]=Mathf.Clamp(data.LevelStars[i],0,3);
            if (data.Layers == null) data.Layers = new List<StackLayer>();
            data.TotalPlaced=Mathf.Max(data.TotalPlaced,data.Layers.Count);
            var valid = !float.IsNaN(data.Phase) && !float.IsInfinity(data.Phase);
            var validChallengeDate=DateTime.TryParseExact(data.RunChallengeDate,"yyyy-MM-dd",System.Globalization.CultureInfo.InvariantCulture,System.Globalization.DateTimeStyles.None,out var challengeDay);
            var runTarget=data.RunChallenge && validChallengeDate?StackCampaign.Daily(challengeDay).Target:StackCampaign.Level(data.RunLevel).Target;
            valid &= !data.RunChallenge || !data.RunEndless && validChallengeDate;
            valid &= data.RunEndless ? StackCampaign.EndlessUnlocked(data) && data.Layers.Count==Mathf.Min(StackRun.RetainedLayers,data.TotalPlaced) : data.TotalPlaced<runTarget && data.Layers.Count==data.TotalPlaced;
            data.Powers ??= new StackPowers();data.Powers.Energy=Mathf.Clamp(data.Powers.Energy,0,6);
            data.Powers.SlowSeconds=float.IsNaN(data.Powers.SlowSeconds) || float.IsInfinity(data.Powers.SlowSeconds)?0:Mathf.Clamp(data.Powers.SlowSeconds,0,3);
            data.ChallengeStars=Mathf.Clamp(data.ChallengeStars,0,3);
            foreach (var layer in data.Layers)
                valid &= Finite(layer.Center) && Finite(layer.Size) && layer.Size.x > 0 && layer.Size.y > 0 && layer.Size.x <= StackRules.BaseWidth && layer.Size.y <= StackRules.BaseWidth;
            if (!valid) { if(!validChallengeDate)data.RunChallenge=false;data.Powers=new StackPowers();data.PerfectDrops=data.MaxStreak=0; data.ResumeActive = false; data.Layers.Clear(); data.Streak = data.RunCoins = data.TotalPlaced = 0; data.Phase = 0; }
            return data;
        }
        private static bool Finite(Vector2 v) => !float.IsNaN(v.x) && !float.IsInfinity(v.x) && !float.IsNaN(v.y) && !float.IsInfinity(v.y);
        public static void Save(StackProfile profile)
        {
            PlayerPrefs.SetString(StorageKey, JsonUtility.ToJson(profile));
            PlayerPrefs.Save();
        }
        public static bool CanClaim(StackProfile profile) => profile.DailyClaim != DateTime.UtcNow.ToString("yyyy-MM-dd");
        public static bool ClaimDaily(StackProfile profile)
        {
            if (!CanClaim(profile)) return false;
            profile.DailyClaim = DateTime.UtcNow.ToString("yyyy-MM-dd");
            profile.Coins += 25;
            Save(profile);
            return true;
        }
    }
}
