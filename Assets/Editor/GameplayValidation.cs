#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEngine.UI;
using Kamilunavo.PerfectDrop.Gameplay;

namespace Kamilunavo.PerfectDrop.Editor
{
    public static class GameplayValidation
    {
        public static void Validate()
        {
            const string bestKey = "perfectdrop.bestFloor";
            const string coinsKey = "perfectdrop.coins";
            const string audioKey = "perfectdrop.settings.audio";
            const string hapticsKey = "perfectdrop.settings.haptics";
            const string motionKey = "perfectdrop.settings.reducedMotion";
            const string profileKey = PlayerProfileStore.StorageKey;

            var hadProfile = PlayerPrefs.HasKey(profileKey);
            var oldProfile = PlayerPrefs.GetString(profileKey, string.Empty);
            var hadBest = PlayerPrefs.HasKey(bestKey);
            var hadCoins = PlayerPrefs.HasKey(coinsKey);
            var oldBest = PlayerPrefs.GetInt(bestKey, 1);
            var oldCoins = PlayerPrefs.GetInt(coinsKey, 0);
            var hadAudio = PlayerPrefs.HasKey(audioKey);
            var hadHaptics = PlayerPrefs.HasKey(hapticsKey);
            var hadMotion = PlayerPrefs.HasKey(motionKey);
            var oldAudio = PlayerPrefs.GetInt(audioKey, 1);
            var oldHaptics = PlayerPrefs.GetInt(hapticsKey, 1);
            var oldMotion = PlayerPrefs.GetInt(motionKey, 0);

            GameObject root = null;
            try
            {
                PlayerPrefs.DeleteKey(profileKey);
                PlayerProfileStore.ResetCacheForTests();
                PlayerPrefs.SetInt(bestKey, 1);
                PlayerPrefs.SetInt(coinsKey, 0);
                PlayerPrefs.SetInt(audioKey, 1);
                PlayerPrefs.SetInt(hapticsKey, 1);
                PlayerPrefs.SetInt(motionKey, 0);

                Assert(GamePreferences.AudioEnabled, "Audio must default to enabled.");
                Assert(GamePreferences.HapticsEnabled, "Haptics must default to enabled.");
                Assert(!GamePreferences.ReducedMotion, "Reduced motion must default to disabled.");
                GamePreferences.AudioEnabled = false;
                GamePreferences.HapticsEnabled = false;
                GamePreferences.ReducedMotion = true;
                Assert(!GamePreferences.AudioEnabled, "Audio preference must persist a disabled value.");
                Assert(!GamePreferences.HapticsEnabled, "Haptics preference must persist a disabled value.");
                Assert(GamePreferences.ReducedMotion, "Reduced-motion preference must persist an enabled value.");
                GamePreferences.AudioEnabled = true;
                GamePreferences.HapticsEnabled = true;
                GamePreferences.ReducedMotion = false;

                Assert(PrecisionScoring.Grade(0f, 0f, 1f, 1f) == LandingGrade.Perfect, "Center landing must be Perfect.");
                Assert(PrecisionScoring.Grade(0.3f, 0.2f, 1f, 1f) == LandingGrade.Good, "Mid landing must be Good.");
                Assert(PrecisionScoring.Grade(0.8f, 0.7f, 1f, 1f) == LandingGrade.Safe, "Edge landing must be Safe.");

                root = new GameObject("PerfectDropGameplayValidation");
                var player = new GameObject("ValidationRunner");
                player.transform.SetParent(root.transform, false);
                player.AddComponent<CharacterController>();
                var motor = player.AddComponent<PlayerMotor>();

                var courseObject = new GameObject("ValidationCourse");
                courseObject.transform.SetParent(root.transform, false);
                var course = courseObject.AddComponent<PrecisionCourse>();
                course.Player = player.transform;
                course.FloorText = Text(root.transform, "Floor");
                course.BestText = Text(root.transform, "Best");
                course.StreakText = Text(root.transform, "Streak");
                course.CoinsText = Text(root.transform, "Coins");
                course.FeedbackText = Text(root.transform, "Feedback");

                var progressObject = new GameObject("Progress", typeof(RectTransform), typeof(Image));
                progressObject.transform.SetParent(root.transform, false);
                course.ProgressFill = progressObject.GetComponent<Image>();

                var completion = new GameObject("Completion");
                completion.transform.SetParent(root.transform, false);
                course.CompletionPanel = completion;
                course.CompletionText = Text(root.transform, "CompletionText");

                course.Build();

                // Conservative ballistic reachability gate. The center of every next platform must be
                // reachable at normal run speed; Boost remains optional assistance rather than a requirement.
                var platforms = course.Platforms;
                Assert(platforms.Count == 30, "Course must contain exactly 30 floors.");
                for (var i = 1; i < platforms.Count; i++)
                {
                    var previous = platforms[i - 1].transform.position;
                    var next = platforms[i].transform.position;
                    var rise = Mathf.Max(0f, next.y - previous.y);
                    var discriminant = motor.JumpSpeed * motor.JumpSpeed - 2f * motor.Gravity * rise;
                    Assert(discriminant > 0f, $"Floor {i + 1} is vertically unreachable.");
                    var flightTime = (motor.JumpSpeed + Mathf.Sqrt(discriminant)) / motor.Gravity;
                    var travelBudget = motor.MoveSpeed * flightTime;
                    var planarGap = Vector2.Distance(new Vector2(previous.x, previous.z), new Vector2(next.x, next.z));
                    Assert(planarGap <= travelBudget * 0.94f, $"Floor {i + 1} planar gap {planarGap:F2} exceeds conservative travel budget {travelBudget * 0.94f:F2}.");
                }

                Assert(course.CurrentFloor == 1, "A run must begin at floor 1.");
                Assert(!course.IsCompleted, "A fresh run must not be completed.");
                Assert(motor.InputEnabled, "Input must be enabled at the start of a run.");
                Assert(!completion.activeSelf, "Completion panel must be hidden during active gameplay.");

                for (var floor = 2; floor <= 30; floor++)
                {
                    var target = course.transform.Find($"Floor_{floor:00}");
                    Assert(target != null, $"Floor {floor} is missing.");
                    var marker = target.GetComponent<PrecisionPlatform>();
                    Assert(marker != null, $"Floor {floor} has no PrecisionPlatform.");

                    course.RegisterLanding(marker, target.position);
                    Assert(course.CurrentFloor == floor, $"Landing on floor {floor} did not advance progress.");
                }

                Assert(course.IsCompleted, "Floor 30 must complete the run.");
                Assert(course.CurrentFloor == 30, "Completed run must report floor 30.");
                Assert(course.Best == 30, "Best floor must persist the completed tower.");
                Assert(course.Coins > 0, "A completed run must award coins.");
                Assert(completion.activeSelf, "Completion panel must open on floor 30.");
                Assert(!motor.InputEnabled, "Gameplay input must stop while completion panel is open.");

                course.RestartRun();
                Assert(!course.IsCompleted, "Restart must clear the completion state.");
                Assert(course.CurrentFloor == 1, "Restart must return to floor 1.");
                Assert(course.Best == 30, "Restart must preserve best floor.");
                Assert(motor.InputEnabled, "Restart must restore gameplay input.");
                Assert(!completion.activeSelf, "Restart must hide the completion panel.");

                Debug.Log("[PerfectDrop] Gameplay progression, completion, persistence and restart matrix passed.");
            }
            finally
            {
                if (root != null) UnityEngine.Object.DestroyImmediate(root);
                if (hadProfile) PlayerPrefs.SetString(profileKey, oldProfile); else PlayerPrefs.DeleteKey(profileKey);
                if (hadBest) PlayerPrefs.SetInt(bestKey, oldBest); else PlayerPrefs.DeleteKey(bestKey);
                if (hadCoins) PlayerPrefs.SetInt(coinsKey, oldCoins); else PlayerPrefs.DeleteKey(coinsKey);
                if (hadAudio) PlayerPrefs.SetInt(audioKey, oldAudio); else PlayerPrefs.DeleteKey(audioKey);
                if (hadHaptics) PlayerPrefs.SetInt(hapticsKey, oldHaptics); else PlayerPrefs.DeleteKey(hapticsKey);
                if (hadMotion) PlayerPrefs.SetInt(motionKey, oldMotion); else PlayerPrefs.DeleteKey(motionKey);
                PlayerPrefs.Save();
                PlayerProfileStore.ResetCacheForTests();
            }
        }

        private static Text Text(Transform parent, string name)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Text));
            go.transform.SetParent(parent, false);
            return go.GetComponent<Text>();
        }

        private static void Assert(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException(message);
        }
    }
}
#endif
