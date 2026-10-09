#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Kamilunavo.PerfectDrop.QA
{
    // simctl can supply environment variables even where managed argv is empty.
    // This bridge is absent from release players and still uses isolated QA saves.
    internal static class QaLaunch
    {
        private static string[] _arguments;
        public static string[] Arguments()
        {
            if (_arguments != null) return _arguments;
            var arguments = new List<string>(Environment.GetCommandLineArgs());
            var mode = Environment.GetEnvironmentVariable("PERFECTDROP_QA_MODE");
            var flag = mode switch
            {
                "stack" => "-qaSmoke", "arcade" => "-qaArcade",
                "arcade-ui" => "-qaArcadeUI", "city" => "-qaCity",
                "presentation" => "-qaPresentation", "soak" => "-qaRenderSoak", _ => null
            };
            if (flag == null) return _arguments = arguments.ToArray();
            var blocked = Environment.GetEnvironmentVariable("PERFECTDROP_QA_BLOCKED_BUTTON") == "1";
            arguments.Add(flag);
            arguments.Add(Environment.GetEnvironmentVariable("PERFECTDROP_QA_OUTPUT") ??
                Path.Combine(Application.persistentDataPath, "qa-" + mode + (blocked ? "-blocked" : "") + "-" + Guid.NewGuid().ToString("N")));
            if (blocked) arguments.Add("-qaBlockedButtonProbe");
            return _arguments = arguments.ToArray();
        }

        public static string ScreenshotPath(string output, string name)
        {
            var path = Path.Combine(output, name + ".png");
            // Unity prefixes the app Documents/persistent directory on mobile.
            return Application.isMobilePlatform ? Path.GetRelativePath(Application.persistentDataPath, path) : path;
        }
    }
}
#endif
