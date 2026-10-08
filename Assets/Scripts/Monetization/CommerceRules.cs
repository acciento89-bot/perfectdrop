using System;
using System.Collections.Generic;
using UnityEngine;
namespace Kamilunavo.PerfectDrop.Monetization
{
    [Serializable]
    public sealed class CommerceProfile
    {
        public int Entitlements;
        public bool StarterCreditClaimed;
        public List<string> FulfilledTransactions=new();
    }
    public static class CommerceRules
    {
        public const string Starter="com.kamilunavo.perfectdrop.starter";
        public const string Collection="com.kamilunavo.perfectdrop.neoncollection";
        public static bool KnownProduct(string id)=>id==Starter || id==Collection;
        public static int PremiumStyles(int entitlements)=>((entitlements&1)!=0?16:0)|((entitlements&2)!=0?224:0);
        public static void Normalize(StackProfile profile)
        {
            profile.Commerce??=new CommerceProfile();
            profile.Commerce.Entitlements&=3;
            profile.Commerce.FulfilledTransactions??=new List<string>();
            profile.OwnedStyles=(profile.OwnedStyles&15)|1|PremiumStyles(profile.Commerce.Entitlements);
            profile.Style=Mathf.Clamp(profile.Style,0,7);
            if((profile.OwnedStyles&(1<<profile.Style))==0)profile.Style=0;
        }
        public static bool ApplyPending(StackProfile profile,string productId,string transactionId)
        {
            if(!KnownProduct(productId) || string.IsNullOrWhiteSpace(transactionId))return false;
            Normalize(profile);
            var marker=productId+":"+transactionId;
            if(profile.Commerce.FulfilledTransactions.Contains(marker))return false;
            if(productId==Starter)
            {
                profile.Commerce.Entitlements|=1;
                if(!profile.Commerce.StarterCreditClaimed)
                {
                    profile.Coins=(int)Math.Min(int.MaxValue,(long)profile.Coins+500);
                    profile.Commerce.StarterCreditClaimed=true;
                }
            }
            else profile.Commerce.Entitlements|=2;
            profile.Commerce.FulfilledTransactions.Add(marker);
            Normalize(profile);return true;
        }
        public static bool RestoreEntitlement(StackProfile profile,string productId)
        {
            if(!KnownProduct(productId))return false;
            Normalize(profile);var before=profile.Commerce.Entitlements;
            profile.Commerce.Entitlements|=productId==Starter?1:2;
            // Confirmed restoration restores cosmetics, never starter currency.
            if(productId==Starter)profile.Commerce.StarterCreditClaimed=true;
            Normalize(profile);return before!=profile.Commerce.Entitlements;
        }
        public static void ReconcileEntitlements(StackProfile profile,IEnumerable<string> activeProductIds)
        {
            var mask=0;
            foreach(var id in activeProductIds)mask|=id==Starter?1:id==Collection?2:0;
            Normalize(profile);profile.Commerce.Entitlements=mask;
            if((mask&1)!=0)profile.Commerce.StarterCreditClaimed=true;
            Normalize(profile);
        }
        public static bool FulfillPending(StackProfile profile,string productId,string transactionId,Action<StackProfile> persist)
        {
            if(!KnownProduct(productId) || string.IsNullOrWhiteSpace(transactionId))return false;
            Normalize(profile);
            var previous=JsonUtility.ToJson(profile.Commerce);
            var coins=profile.Coins;var styles=profile.OwnedStyles;var selection=profile.Style;
            ApplyPending(profile,productId,transactionId);
            try { persist(profile);return true; }
            catch
            {
                profile.Commerce=JsonUtility.FromJson<CommerceProfile>(previous);
                profile.Coins=coins;profile.OwnedStyles=styles;profile.Style=selection;
                throw;
            }
        }
    }
}
