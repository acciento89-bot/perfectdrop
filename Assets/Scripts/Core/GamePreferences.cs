using UnityEngine;

namespace Kamilunavo.PerfectDrop
{
    public static class GamePreferences
    {
        private const string AudioKey = "perfectdrop.settings.audio";
        private const string HapticsKey = "perfectdrop.settings.haptics";
        private const string ReducedMotionKey = "perfectdrop.settings.reducedMotion";

        public static bool AudioEnabled
        {
            get => PlayerPrefs.GetInt(AudioKey, 1) != 0;
            set => SetBool(AudioKey, value);
        }

        public static bool HapticsEnabled
        {
            get => PlayerPrefs.GetInt(HapticsKey, 1) != 0;
            set => SetBool(HapticsKey, value);
        }

        public static bool ReducedMotion
        {
            get => PlayerPrefs.GetInt(ReducedMotionKey, 0) != 0;
            set => SetBool(ReducedMotionKey, value);
        }

        private static void SetBool(string key, bool value)
        {
            PlayerPrefs.SetInt(key, value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
}
