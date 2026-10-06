#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.SceneManagement;
using UnityEngine;
using Kamilunavo.PerfectDrop;

namespace Kamilunavo.PerfectDrop.Editor
{
    [InitializeOnLoad]
    public static class ProjectBootstrap
    {
        private const string ScenePath = "Assets/Scenes/Main.unity";

        static ProjectBootstrap() => EditorApplication.delayCall += InitializeProject;

        public static void InitializeProject()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;
            PlayerSettings.companyName = "Kamilunavo";
            PlayerSettings.productName = "Perfect Drop";
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
            PlayerSettings.bundleVersion = "1.0";
            PlayerSettings.iOS.buildNumber = "1";
            PlayerSettings.Android.bundleVersionCode = 1;
            PlayerSettings.iOS.targetOSVersionString = "15.0";
            PlayerSettings.iOS.appleDeveloperTeamID = "TKG684N5GL";
            PlayerSettings.iOS.appleEnableAutomaticSigning = true;
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.iOS, "com.kamilunavo.perfectdrop");
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, "com.kamilunavo.perfectdrop");

            if (!File.Exists(ScenePath))
            {
                Directory.CreateDirectory("Assets/Scenes");
                var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                new GameObject("GameBootstrap").AddComponent<GameBootstrap>();
                EditorSceneManager.SaveScene(scene, ScenePath);
            }

            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
        }
    }
}

#endif
