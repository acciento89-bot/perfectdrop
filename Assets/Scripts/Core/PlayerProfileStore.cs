using System;
using UnityEngine;
using Kamilunavo.PerfectDrop.Gameplay;

namespace Kamilunavo.PerfectDrop
{
    [Serializable]
    public sealed class PlayerProfileData
    {
        public int SchemaVersion = 1;
        public int BestFloor = 1;
        public int Coins;
        public int Xp;
        public int SelectedStyle;
        public string LastDailyClaimUtc = string.Empty;
        public string DailyChallengeDayUtc = string.Empty;
        public int DailyChallengeProgress;
        public bool DailyChallengeClaimed;
        public int AchievementMask;
    }

    public static class PlayerProfileStore
    {
        public const string StorageKey = "perfectdrop.profile.v1";
        public const int DailyChallengeTarget = 8;

        private const string LegacyBestKey = "perfectdrop.bestFloor";
        private const string LegacyCoinsKey = "perfectdrop.coins";

        private static PlayerProfileData _data;

        public static event Action Changed;

        public static PlayerProfileData Data
        {
            get
            {
                EnsureLoaded();
                return _data;
            }
        }

        public static int BestFloor => Data.BestFloor;
        public static int Coins => Data.Coins;
        public static int Xp => Data.Xp;
        public static int Level => LevelFromXp(Data.Xp);
        public static int SelectedStyle => Mathf.Clamp(Data.SelectedStyle, 0, StyleCount - 1);
        public static int DailyChallengeProgress => Mathf.Clamp(Data.DailyChallengeProgress, 0, DailyChallengeTarget);
        public static bool DailyChallengeClaimed => Data.DailyChallengeClaimed;
        public static int StyleCount => 3;

        public static int XpIntoLevel
        {
            get
            {
                var remaining = Mathf.Max(0, Xp);
                var level = 1;
                while (level < Level)
                {
                    remaining -= XpCostForNextLevel(level);
                    level++;
                }
                return Mathf.Max(0, remaining);
            }
        }

        public static int XpForNextLevel => XpCostForNextLevel(Level);

        public static void Initialize()
        {
            EnsureLoaded();
            EnsureDailyChallengeState();
        }

        public static void UpdateBest(int floor)
        {
            EnsureLoaded();
            var value = Mathf.Clamp(floor, 1, 30);
            if (value <= _data.BestFloor) return;
            _data.BestFloor = value;
            Save();
        }

        public static void AddCoins(int amount)
        {
            if (amount == 0) return;
            EnsureLoaded();
            _data.Coins = Mathf.Max(0, _data.Coins + amount);
            Save();
        }

        public static void AddXp(int amount)
        {
            if (amount <= 0) return;
            EnsureLoaded();
            _data.Xp = Mathf.Max(0, _data.Xp + amount);
            Save();
        }

        public static void RecordLanding(LandingGrade grade, int streak, int bestFloor, int coinReward)
        {
            EnsureLoaded();
            EnsureDailyChallengeState();

            _data.BestFloor = Mathf.Max(_data.BestFloor, Mathf.Clamp(bestFloor, 1, 30));
            _data.Coins = Mathf.Max(0, _data.Coins + Mathf.Max(0, coinReward));
            _data.Xp += grade switch
            {
                LandingGrade.Perfect => 18,
                LandingGrade.Good => 11,
                _ => 6
            };

            if (grade != LandingGrade.Safe)
                _data.DailyChallengeProgress = Mathf.Min(DailyChallengeTarget, _data.DailyChallengeProgress + 1);

            if (grade == LandingGrade.Perfect)
                GrantAchievementInternal(0, 25);
            if (streak >= 5)
                GrantAchievementInternal(1, 40);

            Save();
        }

        public static void CompleteTower()
        {
            EnsureLoaded();
            _data.BestFloor = 30;
            _data.Xp += 120;
            GrantAchievementInternal(2, 100);
            Save();
        }

        public static bool CanClaimDailyReward
        {
            get
            {
                EnsureLoaded();
                return !string.Equals(_data.LastDailyClaimUtc, TodayUtcKey(), StringComparison.Ordinal);
            }
        }

        public static bool ClaimDailyReward()
        {
            EnsureLoaded();
            if (!CanClaimDailyReward) return false;

            _data.LastDailyClaimUtc = TodayUtcKey();
            _data.Coins += 50;
            _data.Xp += 40;
            Save();
            return true;
        }

        public static bool CanClaimDailyChallenge
        {
            get
            {
                EnsureLoaded();
                EnsureDailyChallengeState();
                return !_data.DailyChallengeClaimed && _data.DailyChallengeProgress >= DailyChallengeTarget;
            }
        }

        public static bool ClaimDailyChallenge()
        {
            EnsureLoaded();
            EnsureDailyChallengeState();
            if (!CanClaimDailyChallenge) return false;

            _data.DailyChallengeClaimed = true;
            _data.Coins += 120;
            _data.Xp += 80;
            Save();
            return true;
        }

        public static bool IsStyleUnlocked(int styleIndex)
        {
            return Level >= StyleRequiredLevel(styleIndex);
        }

        public static int StyleRequiredLevel(int styleIndex)
        {
            return styleIndex switch
            {
                1 => 3,
                2 => 6,
                _ => 1
            };
        }

        public static int SelectNextUnlockedStyle()
        {
            EnsureLoaded();
            var start = SelectedStyle;
            for (var step = 1; step <= StyleCount; step++)
            {
                var candidate = (start + step) % StyleCount;
                if (!IsStyleUnlocked(candidate)) continue;
                _data.SelectedStyle = candidate;
                Save();
                return candidate;
            }

            return start;
        }

        public static void SetSelectedStyle(int styleIndex)
        {
            EnsureLoaded();
            var clamped = Mathf.Clamp(styleIndex, 0, StyleCount - 1);
            if (!IsStyleUnlocked(clamped)) return;
            if (_data.SelectedStyle == clamped) return;
            _data.SelectedStyle = clamped;
            Save();
        }

        public static string StyleName(int styleIndex, bool german)
        {
            return styleIndex switch
            {
                1 => german ? "NEON-CYAN" : "NEON CYAN",
                2 => german ? "ROSE-PULS" : "ROSE PULSE",
                _ => german ? "SIGNAL-GOLD" : "SIGNAL GOLD"
            };
        }

        public static bool HasAchievement(int bit)
        {
            EnsureLoaded();
            return (_data.AchievementMask & (1 << bit)) != 0;
        }

        public static int LevelFromXp(int xp)
        {
            var remaining = Mathf.Max(0, xp);
            var level = 1;
            while (level < 50)
            {
                var cost = XpCostForNextLevel(level);
                if (remaining < cost) break;
                remaining -= cost;
                level++;
            }
            return level;
        }

        public static int XpCostForNextLevel(int level)
        {
            return 120 + Mathf.Max(0, level - 1) * 80;
        }

#if UNITY_EDITOR
        public static void ResetCacheForTests()
        {
            _data = null;
            Changed = null;
        }
#endif

        private static void EnsureLoaded()
        {
            if (_data != null)
            {
                EnsureDailyChallengeState();
                return;
            }

            if (PlayerPrefs.HasKey(StorageKey))
            {
                try
                {
                    _data = JsonUtility.FromJson<PlayerProfileData>(PlayerPrefs.GetString(StorageKey));
                }
                catch
                {
                    _data = null;
                }
            }

            if (_data == null || _data.SchemaVersion <= 0)
            {
                _data = new PlayerProfileData
                {
                    SchemaVersion = 1,
                    BestFloor = Mathf.Clamp(PlayerPrefs.GetInt(LegacyBestKey, 1), 1, 30),
                    Coins = Mathf.Max(0, PlayerPrefs.GetInt(LegacyCoinsKey, 0))
                };
            }

            _data.SchemaVersion = 1;
            _data.BestFloor = Mathf.Clamp(_data.BestFloor, 1, 30);
            _data.Coins = Mathf.Max(0, _data.Coins);
            _data.Xp = Mathf.Max(0, _data.Xp);
            _data.SelectedStyle = Mathf.Clamp(_data.SelectedStyle, 0, StyleCount - 1);
            EnsureDailyChallengeState();
            Save(notify: false);
        }

        private static void EnsureDailyChallengeState()
        {
            if (_data == null) return;
            var today = TodayUtcKey();
            if (string.Equals(_data.DailyChallengeDayUtc, today, StringComparison.Ordinal)) return;

            _data.DailyChallengeDayUtc = today;
            _data.DailyChallengeProgress = 0;
            _data.DailyChallengeClaimed = false;
            Save(notify: false);
        }

        private static void GrantAchievementInternal(int bit, int coinReward)
        {
            var mask = 1 << bit;
            if ((_data.AchievementMask & mask) != 0) return;
            _data.AchievementMask |= mask;
            _data.Coins += Mathf.Max(0, coinReward);
        }

        private static string TodayUtcKey()
        {
            return DateTime.UtcNow.ToString("yyyy-MM-dd");
        }

        private static void Save(bool notify = true)
        {
            if (_data == null) return;
            PlayerPrefs.SetString(StorageKey, JsonUtility.ToJson(_data));
            PlayerPrefs.SetInt(LegacyBestKey, _data.BestFloor);
            PlayerPrefs.SetInt(LegacyCoinsKey, _data.Coins);
            PlayerPrefs.Save();
            if (notify) Changed?.Invoke();
        }
    }
}
