using System;
using UnityEngine;
namespace Kamilunavo.PerfectDrop.Monetization
{
    [Serializable] public sealed class MonetizationConfig
    {
        public bool InternalTestAds=true;
        public string IosRewarded="",AndroidRewarded="";
        public string IosInterstitial="",AndroidInterstitial="";
        public static MonetizationConfig Load()
        {
            var text=Resources.Load<TextAsset>("Monetization");
            return text==null?new MonetizationConfig():JsonUtility.FromJson<MonetizationConfig>(text.text);
        }
        public string InterstitialId=>InternalTestAds?
            (Application.platform==RuntimePlatform.IPhonePlayer?"ca-app-pub-3940256099942544/4411468910":"ca-app-pub-3940256099942544/1033173712"):
            (Application.platform==RuntimePlatform.IPhonePlayer?IosInterstitial:AndroidInterstitial);
        public string RewardedId=>InternalTestAds?
            (Application.platform==RuntimePlatform.IPhonePlayer?"ca-app-pub-3940256099942544/1712485313":"ca-app-pub-3940256099942544/5224354917"):
            (Application.platform==RuntimePlatform.IPhonePlayer?IosRewarded:AndroidRewarded);
    }
}
