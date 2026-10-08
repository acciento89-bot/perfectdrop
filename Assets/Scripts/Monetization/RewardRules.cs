using System;
using System.Collections.Generic;
using UnityEngine;
namespace Kamilunavo.PerfectDrop.Monetization
{
    [Serializable] public sealed class RewardProfile
    {
        public string Day="";
        public int Count;
        public long LastAt;
        public List<string> Sessions=new();
    }
    public static class RewardRules
    {
        public const int Coins=50,DailyLimit=5,CooldownSeconds=60;
        private static string Day(DateTime now)=>now.ToUniversalTime().ToString("yyyy-MM-dd",System.Globalization.CultureInfo.InvariantCulture);
        private static long Seconds(DateTime now)=>new DateTimeOffset(now.ToUniversalTime()).ToUnixTimeSeconds();
        public static int Remaining(StackProfile profile,DateTime now)=>Math.Max(0,DailyLimit-(profile.Rewards?.Day==Day(now)?profile.Rewards.Count:0));
        public static int WaitSeconds(StackProfile profile,DateTime now)=>profile.Rewards==null || profile.Rewards.LastAt==0?0:(int)Math.Min(int.MaxValue,Math.Max(0,CooldownSeconds-(Seconds(now)-profile.Rewards.LastAt)));
        public static bool CanClaim(StackProfile profile,DateTime now)=>Remaining(profile,now)>0 && WaitSeconds(profile,now)==0;
        public static bool Fulfill(StackProfile profile,string session,DateTime now,Action<StackProfile> persist)
        {
            if(string.IsNullOrWhiteSpace(session) || !CanClaim(profile,now))return false;
            profile.Rewards??=new RewardProfile();profile.Rewards.Sessions??=new List<string>();
            if(profile.Rewards.Sessions.Contains(session))return false;
            var previous=JsonUtility.ToJson(profile.Rewards);var wallet=profile.Coins;
            if(profile.Rewards.Day!=Day(now)){profile.Rewards.Day=Day(now);profile.Rewards.Count=0;profile.Rewards.Sessions.Clear();}
            profile.Rewards.Count++;profile.Rewards.LastAt=Seconds(now);profile.Rewards.Sessions.Add(session);
            profile.Coins=(int)Math.Min(int.MaxValue,(long)profile.Coins+Coins);
            try{persist(profile);return true;}
            catch{profile.Coins=wallet;profile.Rewards=JsonUtility.FromJson<RewardProfile>(previous);throw;}
        }
    }
}
