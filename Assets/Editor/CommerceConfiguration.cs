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
            var type=Type.GetType("GoogleMobileAds.Editor.GoogleMobileAdsSettings, GoogleMobileAds.Editor",true);
            var asset=(ScriptableObject)type.GetMethod("LoadInstance",BindingFlags.Static|BindingFlags.NonPublic).Invoke(null,null);
            var settings=new SerializedObject(asset);
            settings.FindProperty("adMobIOSAppId").stringValue="ca-app-pub-8944085355624754~8084045088";
            settings.FindProperty("adMobAndroidAppId").stringValue="ca-app-pub-8944085355624754~1614005506";
            settings.ApplyModifiedPropertiesWithoutUndo();EditorUtility.SetDirty(asset);AssetDatabase.SaveAssets();
            CommerceValidation.Validate();
            Debug.Log("AdMob app IDs configured; internal test ad units enabled.");
        }
    }
}
#endif
