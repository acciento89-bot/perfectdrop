using System.Runtime.InteropServices;
using UnityEngine;

namespace Kamilunavo.PerfectDrop.Feedback
{
    public enum HapticCue
    {
        Light = 0,
        Medium = 1,
        Success = 2
    }

    public static class HapticFeedback
    {
        public static void Play(HapticCue cue)
        {
            if (!GamePreferences.HapticsEnabled) return;

#if UNITY_IOS && !UNITY_EDITOR
            PDPlayHaptic((int)cue);
#elif UNITY_ANDROID && !UNITY_EDITOR
            Handheld.Vibrate();
#endif
        }

#if UNITY_IOS && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void PDPlayHaptic(int kind);
#endif
    }
}
