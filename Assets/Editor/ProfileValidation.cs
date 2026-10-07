#if UNITY_EDITOR
using System;
using UnityEngine;
using Kamilunavo.PerfectDrop.Gameplay;

namespace Kamilunavo.PerfectDrop.Editor
{
    public static class ProfileValidation
    {
        public static void Validate()
        {
            const string legacyBestKey = "perfectdrop.bestFloor";
            const string legacyCoinsKey = "perfectdrop.coins";

            var hadProfile = PlayerPrefs.HasKey(PlayerProfileStore.StorageKey);
            var oldProfile = PlayerPrefs.GetString(PlayerProfileStore.StorageKey, string.Empty);
            var hadBest = PlayerPrefs.HasKey(legacyBestKey);
            var oldBest = PlayerPrefs.GetInt(legacyBestKey, 1);
            var hadCoins = PlayerPrefs.HasKey(legacyCoinsKey);
            var oldCoins = PlayerPrefs.GetInt(legacyCoinsKey, 0);

            try
            {
                PlayerPrefs.DeleteKey(PlayerProfileStore.StorageKey);
                PlayerPrefs.SetInt(legacyBestKey, 7);
                PlayerPrefs.SetInt(legacyCoinsKey, 42);
                PlayerPrefs.Save();
                PlayerProfileStore.ResetCacheForTests();
                PlayerProfileStore.Initialize();

                Assert(PlayerProfileStore.Data.SchemaVersion == 1, "Profile schema must be version 1.");
                Assert(PlayerProfileStore.BestFloor == 7, "Legacy best-floor migration failed.");
                Assert(PlayerProfileStore.Coins == 42, "Legacy coin migration failed.");
                Assert(PlayerProfileStore.Level == 1, "Fresh migrated profile should start at level 1.");
                Assert(PlayerProfileStore.CanClaimDailyReward, "Fresh migrated profile must expose today's daily reward.");

                var beforeDailyCoins = PlayerProfileStore.Coins;
                var beforeDailyXp = PlayerProfileStore.Xp;
                Assert(PlayerProfileStore.ClaimDailyReward(), "First daily reward claim must succeed.");
                Assert(PlayerProfileStore.Coins == beforeDailyCoins + 50, "Daily reward must add 50 coins.");
                Assert(PlayerProfileStore.Xp == beforeDailyXp + 40, "Daily reward must add 40 XP.");
                Assert(!PlayerProfileStore.ClaimDailyReward(), "Daily reward must not be claimable twice on the same UTC day.");

                PlayerProfileStore.RecordLanding(LandingGrade.Perfect, 5, 10, 12);
                Assert(PlayerProfileStore.BestFloor == 10, "Landing must advance persisted best floor.");
                Assert(PlayerProfileStore.DailyChallengeProgress == 1, "Perfect landing must advance daily challenge.");
                Assert(PlayerProfileStore.HasAchievement(0), "First Perfect achievement must unlock.");
                Assert(PlayerProfileStore.HasAchievement(1), "Streak achievement must unlock at streak 5.");

                for (var i = 0; i < PlayerProfileStore.DailyChallengeTarget - 1; i++)
                    PlayerProfileStore.RecordLanding(LandingGrade.Good, i + 1, 10, 1);

                Assert(PlayerProfileStore.DailyChallengeProgress == PlayerProfileStore.DailyChallengeTarget, "Daily challenge progress must clamp at its target.");
                Assert(PlayerProfileStore.CanClaimDailyChallenge, "Completed daily challenge must become claimable.");
                var challengeCoins = PlayerProfileStore.Coins;
                var challengeXp = PlayerProfileStore.Xp;
                Assert(PlayerProfileStore.ClaimDailyChallenge(), "Completed daily challenge claim must succeed.");
                Assert(PlayerProfileStore.Coins == challengeCoins + 120, "Daily challenge must award 120 coins.");
                Assert(PlayerProfileStore.Xp == challengeXp + 80, "Daily challenge must award 80 XP.");
                Assert(!PlayerProfileStore.ClaimDailyChallenge(), "Daily challenge reward must not be claimable twice.");

                PlayerProfileStore.AddXp(4000);
                Assert(PlayerProfileStore.Level >= 6, "Progression test must reach level 6.");
                Assert(PlayerProfileStore.IsStyleUnlocked(1), "Cyan runner style must unlock by level 3.");
                Assert(PlayerProfileStore.IsStyleUnlocked(2), "Rose runner style must unlock by level 6.");
                var firstStyle = PlayerProfileStore.SelectedStyle;
                var nextStyle = PlayerProfileStore.SelectNextUnlockedStyle();
                Assert(nextStyle != firstStyle, "Cycling an unlocked runner style must change the selection.");

                PlayerProfileStore.CompleteTower();
                Assert(PlayerProfileStore.BestFloor == 30, "Tower completion must persist best floor 30.");
                Assert(PlayerProfileStore.HasAchievement(2), "Tower-clear achievement must unlock.");

                var coinsBeforeReload = PlayerProfileStore.Coins;
                var xpBeforeReload = PlayerProfileStore.Xp;
                var styleBeforeReload = PlayerProfileStore.SelectedStyle;
                PlayerProfileStore.ResetCacheForTests();
                PlayerProfileStore.Initialize();
                Assert(PlayerProfileStore.Coins == coinsBeforeReload, "Coins must survive profile reload.");
                Assert(PlayerProfileStore.Xp == xpBeforeReload, "XP must survive profile reload.");
                Assert(PlayerProfileStore.SelectedStyle == styleBeforeReload, "Selected runner style must survive profile reload.");

                Debug.Log("[PerfectDrop] Versioned profile, daily reward/challenge, achievements and style progression matrix passed.");
            }
            finally
            {
                if (hadProfile) PlayerPrefs.SetString(PlayerProfileStore.StorageKey, oldProfile); else PlayerPrefs.DeleteKey(PlayerProfileStore.StorageKey);
                if (hadBest) PlayerPrefs.SetInt(legacyBestKey, oldBest); else PlayerPrefs.DeleteKey(legacyBestKey);
                if (hadCoins) PlayerPrefs.SetInt(legacyCoinsKey, oldCoins); else PlayerPrefs.DeleteKey(legacyCoinsKey);
                PlayerPrefs.Save();
                PlayerProfileStore.ResetCacheForTests();
            }
        }

        private static void Assert(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException(message);
        }
    }
}
#endif
