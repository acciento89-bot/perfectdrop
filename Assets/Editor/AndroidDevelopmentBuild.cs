#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
namespace PortfolioVisualIdentityQA
{
 // Local APK-only builder: SDK/catalog identities stay normal; parent owns offline device QA.
 public static class AndroidDevelopmentBuild
 {
  public static void Build()
  {
   if(EditorUserBuildSettings.activeBuildTarget!=BuildTarget.Android)throw new InvalidOperationException("Launch with -buildTarget Android.");
   string output=Environment.GetEnvironmentVariable("PORTFOLIO_IDENTITY_APK_PATH");
   if(string.IsNullOrWhiteSpace(output)||!Path.IsPathRooted(output)||Path.GetExtension(output)!=".apk"||File.Exists(output)||Directory.Exists(output))throw new InvalidOperationException("A fresh absolute APK output is required.");
   var project=Path.GetFullPath(Path.Combine(Application.dataPath,".."));
   if(Path.GetFullPath(output).StartsWith(project+Path.DirectorySeparatorChar,StringComparison.Ordinal))throw new InvalidOperationException("Identity APK must be outside the source/frozen build tree.");
   Directory.CreateDirectory(Path.GetDirectoryName(output));
   var settingsPath=Path.Combine(project,"ProjectSettings/ProjectSettings.asset");var originalSettings=File.ReadAllBytes(settingsPath);
   var identities=AssetDatabase.GetAllAssetPaths().Where(p=>p.StartsWith("Assets/")&&File.Exists(p)&&(p.EndsWith("MonetizationConfig.asset")||p.EndsWith("GoogleMobileAdsSettings.asset"))).ToDictionary(p=>p,File.ReadAllBytes);
   bool custom=PlayerSettings.Android.useCustomKeystore,bundle=EditorUserBuildSettings.buildAppBundle,development=EditorUserBuildSettings.development,export=EditorUserBuildSettings.exportAsGoogleAndroidProject;
   BuildReport report=null;string failure=null;var tools=new Dictionary<PropertyInfo,object>();
   try
   {
    var toolsType=AppDomain.CurrentDomain.GetAssemblies().Select(a=>a.GetType("UnityEditor.Android.AndroidExternalToolsSettings")).FirstOrDefault(x=>x!=null);
    if(toolsType==null)throw new InvalidOperationException("Android external tools API is unavailable.");
    foreach(var setting in new[]{new[]{"sdkRootPath","PORTFOLIO_ANDROID_SDK_ROOT"},new[]{"ndkRootPath","PORTFOLIO_ANDROID_NDK_ROOT"},new[]{"jdkRootPath","PORTFOLIO_ANDROID_JDK_ROOT"}})
    {var value=Environment.GetEnvironmentVariable(setting[1]);if(string.IsNullOrEmpty(value))continue;var property=toolsType.GetProperty(setting[0],BindingFlags.Static|BindingFlags.Public);if(property==null)throw new InvalidOperationException("Missing Android tool setting "+setting[0]);tools[property]=property.GetValue(null);property.SetValue(null,value);if((string)property.GetValue(null)!=value)throw new InvalidOperationException("Android tool override not applied: "+setting[0]);}
    // Never access a release signing key or produce/overwrite an AAB.
    PlayerSettings.Android.useCustomKeystore=false;EditorUserBuildSettings.buildAppBundle=false;EditorUserBuildSettings.development=true;EditorUserBuildSettings.exportAsGoogleAndroidProject=false;
    var scenes=EditorBuildSettings.scenes.Where(s=>s.enabled).Select(s=>s.path).ToArray();if(scenes.Length==0)throw new InvalidOperationException("No enabled scenes.");
    report=BuildPipeline.BuildPlayer(new BuildPlayerOptions{scenes=scenes,locationPathName=output,target=BuildTarget.Android,options=BuildOptions.Development|BuildOptions.AllowDebugging});
    if(report.summary.result!=BuildResult.Succeeded||!File.Exists(output))throw new InvalidOperationException("Development APK build did not succeed.");
   }
   catch(Exception error){failure=error.Message;throw;}
   finally
   {
    foreach(var pair in tools)pair.Key.SetValue(null,pair.Value);
    PlayerSettings.Android.useCustomKeystore=custom;EditorUserBuildSettings.buildAppBundle=bundle;EditorUserBuildSettings.development=development;EditorUserBuildSettings.exportAsGoogleAndroidProject=export;AssetDatabase.SaveAssets();
    // Restore exactly the source settings and existing own identity assets after build postprocessors.
    if(!File.ReadAllBytes(settingsPath).SequenceEqual(originalSettings))File.WriteAllBytes(settingsPath,originalSettings);
    foreach(var pair in identities)if(!File.ReadAllBytes(pair.Key).SequenceEqual(pair.Value))File.WriteAllBytes(pair.Key,pair.Value);
    var receipt=new Proof{Result=report==null?"FailedBeforeBuild":report.summary.result.ToString(),Failure=failure,Output=output,Bytes=File.Exists(output)?new FileInfo(output).Length:0,Errors=report==null?0:report.summary.totalErrors,Warnings=report==null?0:report.summary.totalWarnings,ProjectSettingsRestored=File.ReadAllBytes(settingsPath).SequenceEqual(originalSettings),ProviderIdentityAssetsRestored=identities.All(p=>File.ReadAllBytes(p.Key).SequenceEqual(p.Value)),Development=true,Bundle=false,ToolchainRestored=tools.All(p=>Equals(p.Key.GetValue(null),p.Value))};
    if(File.Exists(output)){using var sha=SHA256.Create();using var stream=File.OpenRead(output);receipt.Sha256=BitConverter.ToString(sha.ComputeHash(stream)).Replace("-","").ToLowerInvariant();}
    File.WriteAllText(output+".json",JsonUtility.ToJson(receipt,true));
   }
   Debug.Log("PORTFOLIO_ANDROID_DEVELOPMENT_APK_PASS "+output);
  }
  [Serializable]sealed class Proof{public string Result,Failure,Output,Sha256;public long Bytes;public int Errors,Warnings;public bool ProjectSettingsRestored,ProviderIdentityAssetsRestored,Development,Bundle,ToolchainRestored;}
 }
}
#endif
