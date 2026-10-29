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
    // Android release export only. Upload signing remains a separate verified step.
    public static class AndroidReleaseBuild
    {
        const string VersionName = "1.2.0";
        const int VersionCode = 15;
        const string PackageName = "com.kamilunavo.perfectdrop";

        public static void Build()
        {
            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
                throw new InvalidOperationException("Launch with -buildTarget Android.");
            if (Argument("-versionName") != null || Argument("-buildNumber") != null)
                throw new InvalidOperationException("Use the reviewed Android release metadata; shared iOS version overrides are forbidden.");
            string output = Argument("-buildOutput");
            if (string.IsNullOrWhiteSpace(output) || !Path.IsPathRooted(output) || Path.GetExtension(output) != ".aab" || File.Exists(output) || Directory.Exists(output) || File.Exists(output + ".json"))
                throw new InvalidOperationException("A fresh absolute external AAB output is required.");
            var project = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
            output = Path.GetFullPath(output);
            if (output.StartsWith(project + Path.DirectorySeparatorChar, StringComparison.Ordinal))
                throw new InvalidOperationException("Never overwrite source or frozen release outputs.");
            if (PlayerSettings.GetApplicationIdentifier(UnityEditor.Build.NamedBuildTarget.Android) != PackageName)
                throw new InvalidOperationException("Android package identity mismatch.");
            Directory.CreateDirectory(Path.GetDirectoryName(output));
            var settingsPath = Path.Combine(project, "ProjectSettings/ProjectSettings.asset");
            var originalSettings = File.ReadAllBytes(settingsPath);
            var identities = AssetDatabase.GetAllAssetPaths().Where(p => p.StartsWith("Assets/") && File.Exists(p) && (p.EndsWith("MonetizationConfig.asset") || p.EndsWith("CommerceConfiguration.asset") || p.EndsWith("BillingMode.json") || p.EndsWith("GoogleMobileAdsSettings.asset"))).ToDictionary(p => p, File.ReadAllBytes);
            var previousVersion = PlayerSettings.bundleVersion;
            var previousIosBuild = PlayerSettings.iOS.buildNumber;
            int previousCode = PlayerSettings.Android.bundleVersionCode;
            bool custom = PlayerSettings.Android.useCustomKeystore, bundle = EditorUserBuildSettings.buildAppBundle, development = EditorUserBuildSettings.development, export = EditorUserBuildSettings.exportAsGoogleAndroidProject;
            var tools = new Dictionary<PropertyInfo, object>();
            bool succeeded = false;
            string failure = null;
            try
            {
                var toolsType = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("UnityEditor.Android.AndroidExternalToolsSettings")).FirstOrDefault(t => t != null);
                if (toolsType == null) throw new InvalidOperationException("Android external tools API unavailable.");
                foreach (var setting in new[] { new[] { "sdkRootPath", "PORTFOLIO_ANDROID_SDK_ROOT" }, new[] { "ndkRootPath", "PORTFOLIO_ANDROID_NDK_ROOT" }, new[] { "jdkRootPath", "PORTFOLIO_ANDROID_JDK_ROOT" } })
                {
                    var value = Environment.GetEnvironmentVariable(setting[1]);
                    if (string.IsNullOrEmpty(value)) continue;
                    var property = toolsType.GetProperty(setting[0], BindingFlags.Static | BindingFlags.Public);
                    if (property == null) throw new InvalidOperationException("Missing Android tool setting " + setting[0]);
                    tools[property] = property.GetValue(null); property.SetValue(null, value);
                    if ((string)property.GetValue(null) != value) throw new InvalidOperationException("Android tool override failed.");
                }
                PlayerSettings.bundleVersion = VersionName;
                PlayerSettings.Android.bundleVersionCode = VersionCode;
                PlayerSettings.Android.useCustomKeystore = false;
                EditorUserBuildSettings.buildAppBundle = true;
                EditorUserBuildSettings.development = false;
                EditorUserBuildSettings.exportAsGoogleAndroidProject = false;
                BuildAutomation.BuildAndroidRelease();
                succeeded = File.Exists(output) && new FileInfo(output).Length > 0;
                if (!succeeded) throw new InvalidOperationException("Android release export did not produce an AAB.");
            }
            catch (Exception error) { failure = error.Message; throw; }
            finally
            {
                foreach (var pair in tools) pair.Key.SetValue(null, pair.Value);
                PlayerSettings.bundleVersion = previousVersion;
                PlayerSettings.iOS.buildNumber = previousIosBuild;
                PlayerSettings.Android.bundleVersionCode = previousCode;
                PlayerSettings.Android.useCustomKeystore = custom;
                EditorUserBuildSettings.buildAppBundle = bundle; EditorUserBuildSettings.development = development; EditorUserBuildSettings.exportAsGoogleAndroidProject = export;
                AssetDatabase.SaveAssets();
                if (!File.ReadAllBytes(settingsPath).SequenceEqual(originalSettings)) File.WriteAllBytes(settingsPath, originalSettings);
                foreach (var pair in identities) if (!File.ReadAllBytes(pair.Key).SequenceEqual(pair.Value)) File.WriteAllBytes(pair.Key, pair.Value);
                var proof = new Proof { BuildSucceeded = succeeded, Failure = failure, Output = output, Package = PackageName, AndroidVersionName = VersionName, AndroidVersionCode = VersionCode, ProjectSettingsRestored = File.ReadAllBytes(settingsPath).SequenceEqual(originalSettings), ProviderIdentityAssetsRestored = identities.All(p => File.ReadAllBytes(p.Key).SequenceEqual(p.Value)), IOSVersionRestored = PlayerSettings.bundleVersion == previousVersion && PlayerSettings.iOS.buildNumber == previousIosBuild, ToolchainRestored = tools.All(p => Equals(p.Key.GetValue(null), p.Value)), UploadSigned = false };
                if (File.Exists(output)) { using var sha = SHA256.Create(); using var stream = File.OpenRead(output); proof.Sha256 = BitConverter.ToString(sha.ComputeHash(stream)).Replace("-", "").ToLowerInvariant(); }
                File.WriteAllText(output + ".json", JsonUtility.ToJson(proof, true));
                if (!proof.ProjectSettingsRestored || !proof.ProviderIdentityAssetsRestored || !proof.IOSVersionRestored || !proof.ToolchainRestored) throw new InvalidOperationException("Release settings restoration failed; inspect the build receipt.");
            }
            Debug.Log("PORTFOLIO_ANDROID_RELEASE_EXPORT_PASS " + output + "; upload signing still required.");
        }

        static string Argument(string key)
        {
            var args = Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length; i++) if (args[i] == key) return i + 1 < args.Length ? args[i + 1] : "";
            return null;
        }
        [Serializable] sealed class Proof
        {
            public string Failure, Output, Package, AndroidVersionName, Sha256;
            public int AndroidVersionCode;
            public bool BuildSucceeded, ProjectSettingsRestored, ProviderIdentityAssetsRestored, IOSVersionRestored, ToolchainRestored, UploadSigned;
        }
    }
}
#endif
