#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace Kamilunavo.PerfectDrop.Editor
{
    public static class ReleaseConfigurationValidation
    {
        public static void Validate()
        {
            var version = PlayerSettings.bundleVersion;
            var iosBuild = PlayerSettings.iOS.buildNumber;
            var androidBuild = PlayerSettings.Android.bundleVersionCode;
            try
            {
                PlayerSettings.bundleVersion = "1.2.3";
                PlayerSettings.iOS.buildNumber = "97";
                PlayerSettings.Android.bundleVersionCode = 97;
                ProjectBootstrap.InitializeProject();
                if (PlayerSettings.bundleVersion != "1.2.3" ||
                    PlayerSettings.iOS.buildNumber != "97" ||
                    PlayerSettings.Android.bundleVersionCode != 97)
                    throw new InvalidOperationException("Project bootstrap must preserve configured release versions.");
                Debug.Log("Release configuration validation passed: bootstrap preserves release versions.");
            }
            finally
            {
                PlayerSettings.bundleVersion = version;
                PlayerSettings.iOS.buildNumber = iosBuild;
                PlayerSettings.Android.bundleVersionCode = androidBuild;
            }
        }
    }
}
#endif
