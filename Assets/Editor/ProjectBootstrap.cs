#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Android;
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
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.AutoRotation;
            PlayerSettings.allowedAutorotateToPortrait = true;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
            PlayerSettings.allowedAutorotateToLandscapeLeft = true;
            PlayerSettings.allowedAutorotateToLandscapeRight = true;
            if (string.IsNullOrWhiteSpace(PlayerSettings.bundleVersion))
                PlayerSettings.bundleVersion = "1.0";
            if (string.IsNullOrWhiteSpace(PlayerSettings.iOS.buildNumber))
                PlayerSettings.iOS.buildNumber = "1";
            if (PlayerSettings.Android.bundleVersionCode < 1)
                PlayerSettings.Android.bundleVersionCode = 1;
            var icon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Art/PerfectDropIcon.png");
            if (icon != null)
                PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Unknown, new[] { icon });
            ConfigureAndroidIcons();
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

        private static void ConfigureAndroidIcons()
        {
            const string foregroundPath = "Assets/Art/AndroidIconForeground.png";
            const string backgroundPath = "Assets/Art/AndroidIconBackground.asset";
            var foreground = AssetDatabase.LoadAssetAtPath<Texture2D>(foregroundPath);
            if (foreground == null) return;
            if (AssetImporter.GetAtPath(foregroundPath) is TextureImporter importer &&
                (importer.mipmapEnabled || !importer.alphaIsTransparency ||
                 importer.textureCompression != TextureImporterCompression.Uncompressed))
            {
                importer.mipmapEnabled = false;
                importer.alphaIsTransparency = true;
                importer.npotScale = TextureImporterNPOTScale.None;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.SaveAndReimport();
                foreground = AssetDatabase.LoadAssetAtPath<Texture2D>(foregroundPath);
            }
            var background = AssetDatabase.LoadAssetAtPath<Texture2D>(backgroundPath);
            if (background == null)
            {
                background = new Texture2D(8, 8, TextureFormat.RGBA32, false)
                {
                    name = "AndroidIconBackground",
                    wrapMode = TextureWrapMode.Clamp
                };
                var pixels = new Color32[64];
                for (var i = 0; i < pixels.Length; i++)
                    pixels[i] = new Color32(8, 18, 32, 255);
                background.SetPixels32(pixels);
                background.Apply();
                AssetDatabase.CreateAsset(background, backgroundPath);
            }
            var icons = PlayerSettings.GetPlatformIcons(NamedBuildTarget.Android, AndroidPlatformIconKind.Adaptive);
            foreach (var adaptiveIcon in icons)
                adaptiveIcon.SetTextures(new[] { background, foreground });
            PlayerSettings.SetPlatformIcons(NamedBuildTarget.Android, AndroidPlatformIconKind.Adaptive, icons);
        }
    }
}

#endif
