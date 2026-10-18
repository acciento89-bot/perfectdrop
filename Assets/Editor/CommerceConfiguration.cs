#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
namespace Kamilunavo.PerfectDrop.Editor
{
    public static class CommerceConfiguration
    {
        public static void Configure()
        {
#if UNITY_ANDROID
            // Batch builds do not reliably execute the package's delayed default-store setup.
            // Persist the actual store selector before resources are collected for the player.
            UnityEditor.Purchasing.UnityPurchasingEditor.TargetAndroidStore(UnityEngine.Purchasing.AppStore.GooglePlay);
            var billing=Resources.Load<TextAsset>("BillingMode");
            if(billing==null||!billing.text.Contains("\"androidStore\":\"GooglePlay\""))
                throw new InvalidOperationException("Android release requires a serialized Google Play BillingMode resource");
            Debug.Log("ANDROID_REAL_STORE_PASS GooglePlay selected; no fake-store fallback");
#endif
            var type=Type.GetType("GoogleMobileAds.Editor.GoogleMobileAdsSettings, GoogleMobileAds.Editor",true);
            var asset=(ScriptableObject)type.GetMethod("LoadInstance",BindingFlags.Static|BindingFlags.NonPublic).Invoke(null,null);
            var settings=new SerializedObject(asset);
            settings.FindProperty("adMobIOSAppId").stringValue="ca-app-pub-8944085355624754~8084045088";
            settings.FindProperty("adMobAndroidAppId").stringValue="ca-app-pub-8944085355624754~1614005506";
            settings.ApplyModifiedPropertiesWithoutUndo();EditorUtility.SetDirty(asset);AssetDatabase.SaveAssets();
            CommerceValidation.Validate();
            Debug.Log("AdMob app IDs configured; internal test ads="+Monetization.MonetizationConfig.Load().InternalTestAds);
        }
    }
}
#endif
