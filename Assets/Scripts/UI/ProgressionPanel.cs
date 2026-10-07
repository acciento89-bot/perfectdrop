using UnityEngine;
using UnityEngine.UI;
using Kamilunavo.PerfectDrop.Gameplay;
using Kamilunavo.PerfectDrop.Visuals;

namespace Kamilunavo.PerfectDrop.UI
{
    public sealed class ProgressionPanel : MonoBehaviour
    {
        public GameObject Root;
        public PlayerMotor Motor;
        public PrecisionCourse Course;
        public RunnerStyleController StyleController;

        public Text LevelText;
        public Text XpText;
        public Text DailyText;
        public Text ChallengeText;
        public Text StyleText;

        public Button DailyButton;
        public Button ChallengeButton;
        public Button StyleButton;

        private void OnEnable()
        {
            PlayerProfileStore.Changed += Refresh;
        }

        private void OnDisable()
        {
            PlayerProfileStore.Changed -= Refresh;
        }

        public void Open()
        {
            PlayerProfileStore.Initialize();
            if (Root != null) Root.SetActive(true);
            if (Motor != null) Motor.InputEnabled = false;
            Refresh();
        }

        public void Close()
        {
            if (Root != null) Root.SetActive(false);
            if (Motor != null) Motor.InputEnabled = Course == null || !Course.IsCompleted;
        }

        public void ClaimDaily()
        {
            PlayerProfileStore.ClaimDailyReward();
            Refresh();
        }

        public void ClaimChallenge()
        {
            PlayerProfileStore.ClaimDailyChallenge();
            Refresh();
        }

        public void NextStyle()
        {
            PlayerProfileStore.SelectNextUnlockedStyle();
            StyleController?.ApplySelectedStyle();
            Refresh();
        }

        public void Refresh()
        {
            PlayerProfileStore.Initialize();

            if (LevelText != null) LevelText.text = GameText.ProfileLevel(PlayerProfileStore.Level);
            if (XpText != null) XpText.text = GameText.ProfileXp(PlayerProfileStore.XpIntoLevel, PlayerProfileStore.XpForNextLevel);
            if (DailyText != null) DailyText.text = GameText.DailyRewardStatus(PlayerProfileStore.CanClaimDailyReward);
            if (ChallengeText != null)
                ChallengeText.text = GameText.DailyChallengeStatus(
                    PlayerProfileStore.DailyChallengeProgress,
                    PlayerProfileStore.DailyChallengeTarget,
                    PlayerProfileStore.DailyChallengeClaimed);

            if (StyleText != null)
            {
                var style = PlayerProfileStore.SelectedStyle;
                StyleText.text = GameText.StyleStatus(
                    PlayerProfileStore.StyleName(style, GameText.German),
                    PlayerProfileStore.Level);
            }

            if (DailyButton != null)
            {
                DailyButton.interactable = PlayerProfileStore.CanClaimDailyReward;
                SetButtonLabel(DailyButton, PlayerProfileStore.CanClaimDailyReward ? GameText.Claim : GameText.Claimed);
            }

            if (ChallengeButton != null)
            {
                DailyButton?.gameObject.SetActive(true);
                ChallengeButton.interactable = PlayerProfileStore.CanClaimDailyChallenge;
                SetButtonLabel(ChallengeButton, PlayerProfileStore.DailyChallengeClaimed ? GameText.Claimed : GameText.Claim);
            }

            if (StyleButton != null)
            {
                var hasAlternative = PlayerProfileStore.Level >= PlayerProfileStore.StyleRequiredLevel(1);
                StyleButton.interactable = hasAlternative;
                SetButtonLabel(StyleButton, hasAlternative ? GameText.NextStyle : GameText.UnlockAtLevel(PlayerProfileStore.StyleRequiredLevel(1)));
            }
        }

        private static void SetButtonLabel(Button button, string value)
        {
            if (button == null) return;
            var label = button.transform.Find("Label")?.GetComponent<Text>();
            if (label != null) label.text = value;
        }
    }
}
