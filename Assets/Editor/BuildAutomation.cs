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
        Build(BuildTarget.Android, GetOutput("-buildOutput", "Builds/Android/app-dev.apk"));
    }

    public static void BuildIOS()
    {
        PlayerSettings.iOS.appleDeveloperTeamID = "TKG684N5GL";
        Build(BuildTarget.iOS, GetOutput("-buildOutput", "Builds/iOS"));
    }

    private static void Build(BuildTarget target, string output)
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
            options = BuildOptions.Development | BuildOptions.AllowDebugging
        };

        var report = BuildPipeline.BuildPlayer(options);
        if (report.summary.result != BuildResult.Succeeded)
            throw new InvalidOperationException($"Build failed: {report.summary.result} with {report.summary.totalErrors} error(s).");
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
