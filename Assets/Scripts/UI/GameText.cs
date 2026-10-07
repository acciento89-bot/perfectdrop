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

        public static string Completion(int best, int coins) =>
            German
                ? $"30 ETAGEN GESCHAFFT\nBESTE  {best}   •   COINS  {coins}"
                : $"30 FLOORS CLEARED\nBEST  {best}   •   COINS  {coins}";
    }
}
