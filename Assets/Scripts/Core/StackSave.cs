using System;
using System.Collections.Generic;
using UnityEngine;
using Kamilunavo.PerfectDrop.Gameplay;

namespace Kamilunavo.PerfectDrop
{
    [Serializable]
    public sealed class StackProfile
    {
        public int Schema = 1, Best, Coins, Towers, Streak, RunCoins;
        public bool ResumeActive;
        public float Phase;
        public string DailyClaim = "";
        public List<StackLayer> Layers = new();
    }
    public static class StackSave
    {
        public const string Key = "perfectdrop.stack.profile.v1";
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        public static string QaKey;
        private static string StorageKey => string.IsNullOrEmpty(QaKey) ? Key : QaKey;
#else
        private static string StorageKey => Key;
#endif
        public static StackProfile Load() => Parse(PlayerPrefs.GetString(StorageKey, ""));
        public static StackProfile Parse(string json)
        {
            if (string.IsNullOrEmpty(json)) return new StackProfile();
            StackProfile data;
            try { data = JsonUtility.FromJson<StackProfile>(json); }
            catch (ArgumentException) { return new StackProfile(); }
            if (data == null || data.Schema != 1) return new StackProfile();
            data.Best = Mathf.Clamp(data.Best, 0, StackRules.Target);
            data.Coins = Mathf.Max(0, data.Coins);
            data.Towers = Mathf.Max(0, data.Towers);
            if (data.Layers == null) data.Layers = new List<StackLayer>();
            var valid = data.Layers.Count < StackRules.Target && !float.IsNaN(data.Phase) && !float.IsInfinity(data.Phase);
            foreach (var layer in data.Layers)
                valid &= Finite(layer.Center) && Finite(layer.Size) && layer.Size.x > 0 && layer.Size.y > 0 && layer.Size.x <= StackRules.BaseWidth && layer.Size.y <= StackRules.BaseWidth;
            if (!valid) { data.ResumeActive = false; data.Layers.Clear(); data.Streak = data.RunCoins = 0; data.Phase = 0; }
            return data;
        }
        private static bool Finite(Vector2 v) => !float.IsNaN(v.x) && !float.IsInfinity(v.x) && !float.IsNaN(v.y) && !float.IsInfinity(v.y);
        public static void Save(StackProfile profile)
        {
            PlayerPrefs.SetString(StorageKey, JsonUtility.ToJson(profile));
            PlayerPrefs.Save();
        }
        public static bool CanClaim(StackProfile profile) => profile.DailyClaim != DateTime.UtcNow.ToString("yyyy-MM-dd");
        public static bool ClaimDaily(StackProfile profile)
        {
            if (!CanClaim(profile)) return false;
            profile.DailyClaim = DateTime.UtcNow.ToString("yyyy-MM-dd");
            profile.Coins += 25;
            Save(profile);
            return true;
        }
    }
}
