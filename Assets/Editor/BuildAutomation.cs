#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;

public static class BuildAutomation
{
    public static void BuildAndroid()
    {
        EditorUserBuildSettings.buildAppBundle = false;
        ApplyVersionArguments();
        Build(BuildTarget.Android, GetOutput("-buildOutput", "Builds/Android/app-dev.apk"), development: true);
    }

    public static void BuildAndroidRelease()
    {
        EditorUserBuildSettings.buildAppBundle = true;
        ApplyVersionArguments();
        Build(BuildTarget.Android, GetOutput("-buildOutput", "Builds/Android/PerfectDrop.aab"), development: false);
    }

    public static void BuildIOS()
    {
        var previousSdk = PlayerSettings.iOS.sdkVersion;
        try
        {
            PlayerSettings.iOS.appleDeveloperTeamID = "TKG684N5GL";
            PlayerSettings.iOS.sdkVersion = iOSSdkVersion.DeviceSDK;
            ApplyVersionArguments();
            Build(BuildTarget.iOS, GetOutput("-buildOutput", "Builds/iOS"), development: false);
        }
        finally
        {
            PlayerSettings.iOS.sdkVersion = previousSdk;
        }
    }

    public static void BuildIOSSimulator()
    {
        var previousSdk = PlayerSettings.iOS.sdkVersion;
        try
        {
            PlayerSettings.iOS.sdkVersion = iOSSdkVersion.SimulatorSDK;
            Build(BuildTarget.iOS, GetOutput("-buildOutput", "/private/tmp/PerfectDrop-iOS-Simulator"), development: false);
        }
        finally
        {
            PlayerSettings.iOS.sdkVersion = previousSdk;
        }
    }

    public static void BuildMacPreview()
    {
        PlayerSettings.resizableWindow = true;
        Build(BuildTarget.StandaloneOSX, GetOutput("-buildOutput", "/private/tmp/PerfectDropPreview.app"), development: true);
    }

    public static void BuildMacReview()
    {
        PlayerSettings.resizableWindow = true;
        Build(BuildTarget.StandaloneOSX, GetOutput("-buildOutput", "/private/tmp/PerfectDropReview.app"), development: false);
    }

    private static void Build(BuildTarget target, string output, bool development)
    {
        if (target == BuildTarget.iOS)
        {
            Directory.CreateDirectory(output);
        }
        else
        {
            var directory = Path.GetDirectoryName(output);
            if (!string.IsNullOrEmpty(directory)) Directory.CreateDirectory(directory);
        }

        var scenes = EditorBuildSettings.scenes
            .Where(scene => scene.enabled)
            .Select(scene => scene.path)
            .ToArray();

        if (scenes.Length == 0)
            throw new InvalidOperationException("No enabled scenes exist in Build Settings.");

        var options = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = output,
            target = target,
            options = development ? BuildOptions.Development | BuildOptions.AllowDebugging : BuildOptions.None
        };

        var report = BuildPipeline.BuildPlayer(options);
        foreach(var shader in UnityEngine.Resources.LoadAll<UnityEngine.Shader>(""))
            if(ShaderUtil.ShaderHasError(shader))throw new InvalidOperationException("Build contains a shader compilation error: "+shader.name);
        if (report.summary.result != BuildResult.Succeeded)
            throw new InvalidOperationException($"Build failed: {report.summary.result} with {report.summary.totalErrors} error(s).");
    }

    private static void ApplyVersionArguments()
    {
        var versionName = GetOptionalArgument("-versionName");
        if (!string.IsNullOrWhiteSpace(versionName))
            PlayerSettings.bundleVersion = versionName.Trim();

        var buildNumber = GetOptionalArgument("-buildNumber");
        if (string.IsNullOrWhiteSpace(buildNumber)) return;
        if (!int.TryParse(buildNumber, out var numericBuild) || numericBuild < 1)
            throw new InvalidOperationException($"Invalid -buildNumber '{buildNumber}'. Expected a positive integer.");

        PlayerSettings.iOS.buildNumber = numericBuild.ToString();
        PlayerSettings.Android.bundleVersionCode = numericBuild;
    }

    private static string GetOptionalArgument(string key)
    {
        var args = Environment.GetCommandLineArgs();
        for (var i = 0; i < args.Length - 1; i++)
        {
            if (args[i] == key) return args[i + 1];
        }
        return null;
    }

    private static string GetOutput(string key, string fallback)
    {
        var args = Environment.GetCommandLineArgs();
        for (var i = 0; i < args.Length - 1; i++)
        {
            if (args[i] == key) return args[i + 1];
        }

        return fallback;
    }
}
#endif
