using UnityEngine;
using Kamilunavo.PerfectDrop.Gameplay;

namespace Kamilunavo.PerfectDrop.UI
{
    public static class GameText
    {
        private const string LanguageKey = "perfectdrop.language";

        public static bool German
        {
            get
            {
                var preference = PlayerPrefs.GetString(LanguageKey, string.Empty);
                if (preference == "de") return true;
                if (preference == "en") return false;
                return Application.systemLanguage == SystemLanguage.German;
            }
        }

        public static string Floor(int value) => German ? $"ETAGE\n{value}" : $"FLOOR\n{value}";
        public static string Best(int value) => German ? $"BESTE\n{value}" : $"BEST\n{value}";
        public static string Streak(int value) => German ? $"SERIE\nx{value}" : $"STREAK\nx{value}";
        public static string Coins(int value) => German ? $"COINS\n{value}" : $"COINS\n{value}";
        public static string ObjectiveTitle => German ? "PRÄZISIONSSPRUNG" : "PRECISION JUMP";
        public static string ObjectiveSubtitle => German ? "Lande innerhalb der markierten Zone." : "Land inside the marked bay.";
        public static string Ready => German ? "BEREIT" : "READY";
        public static string Boosts => German ? "BOOST" : "BOOSTS";
        public static string TowerCleared => German ? "TURM GESCHAFFT" : "TOWER CLEARED";
        public static string RunAgain => German ? "NOCHMAL" : "RUN AGAIN";
        public static string Menu => German ? "MENÜ" : "MENU";
        public static string SettingsTitle => German ? "EINSTELLUNGEN" : "SETTINGS";
        public static string Sound => German ? "TON" : "SOUND";
        public static string Haptics => German ? "HAPTIK" : "HAPTICS";
        public static string ReducedMotion => German ? "WENIGER BEWEGUNG" : "REDUCED MOTION";
        public static string Close => German ? "SCHLIESSEN" : "CLOSE";
        public static string Progression => German ? "PROFIL" : "PROFILE";
        public static string ProfileTitle => German ? "FORTSCHRITT" : "PROGRESS";
        public static string DailyReward => German ? "TÄGLICHE BELOHNUNG" : "DAILY REWARD";
        public static string DailyChallenge => German ? "TAGESCHALLENGE" : "DAILY CHALLENGE";
        public static string RunnerStyle => German ? "RUNNER-STIL" : "RUNNER STYLE";
        public static string Claim => German ? "HOLEN" : "CLAIM";
        public static string Claimed => German ? "GEHOLT" : "CLAIMED";
        public static string NextStyle => German ? "NÄCHSTER" : "NEXT";
        public static string ToggleValue(bool enabled) => German ? (enabled ? "AN" : "AUS") : (enabled ? "ON" : "OFF");

        public static string Landing(LandingGrade grade)
        {
            if (!German) return grade.ToString().ToUpperInvariant();
            return grade switch
            {
                LandingGrade.Perfect => "PERFEKT",
                LandingGrade.Good => "GUT",
                _ => "SICHER"
            };
        }

        public static string Completion(int best, int coins, int level) =>
            German
                ? $"30 ETAGEN GESCHAFFT\nBESTE {best}  •  LV {level}  •  COINS {coins}"
                : $"30 FLOORS CLEARED\nBEST {best}  •  LV {level}  •  COINS {coins}";

        public static string ProfileLevel(int level) => German ? $"LEVEL {level}" : $"LEVEL {level}";
        public static string ProfileXp(int current, int needed) => German ? $"XP  {current} / {needed}" : $"XP  {current} / {needed}";
        public static string DailyRewardStatus(bool available) => available
            ? (German ? "50 COINS + 40 XP BEREIT" : "50 COINS + 40 XP READY")
            : (German ? "HEUTE BEREITS GEHOLT" : "CLAIMED TODAY");
        public static string DailyChallengeStatus(int progress, int target, bool claimed) => claimed
            ? (German ? "CHALLENGE ABGESCHLOSSEN" : "CHALLENGE COMPLETE")
            : (German ? $"GUTE LANDUNGEN  {progress}/{target}" : $"GOOD LANDINGS  {progress}/{target}");
        public static string StyleStatus(string style, int level) => German
            ? $"{style}  •  LEVEL {level}"
            : $"{style}  •  LEVEL {level}";
        public static string UnlockAtLevel(int level) => German ? $"AB LV {level}" : $"LV {level}";
    }
}
