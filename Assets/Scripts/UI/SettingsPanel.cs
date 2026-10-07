using UnityEngine;
using UnityEngine.UI;
using Kamilunavo.PerfectDrop.Gameplay;
using Kamilunavo.PerfectDrop;

namespace Kamilunavo.PerfectDrop.UI
{
    public sealed class SettingsPanel : MonoBehaviour
    {
        public GameObject Root;
        public PlayerMotor Motor;
        public PrecisionCourse Course;
        public Text SoundValue;
        public Text HapticsValue;
        public Text MotionValue;

        public void Open()
        {
            if (Root != null) Root.SetActive(true);
            if (Motor != null) Motor.InputEnabled = false;
            Refresh();
        }

        public void Close()
        {
            if (Root != null) Root.SetActive(false);
            if (Motor != null) Motor.InputEnabled = Course == null || !Course.IsCompleted;
        }

        public void ToggleSound()
        {
            GamePreferences.AudioEnabled = !GamePreferences.AudioEnabled;
            Refresh();
        }

        public void ToggleHaptics()
        {
            GamePreferences.HapticsEnabled = !GamePreferences.HapticsEnabled;
            Refresh();
        }

        public void ToggleReducedMotion()
        {
            GamePreferences.ReducedMotion = !GamePreferences.ReducedMotion;
            Refresh();
        }

        public void Refresh()
        {
            if (SoundValue != null) SoundValue.text = GameText.ToggleValue(GamePreferences.AudioEnabled);
            if (HapticsValue != null) HapticsValue.text = GameText.ToggleValue(GamePreferences.HapticsEnabled);
            if (MotionValue != null) MotionValue.text = GameText.ToggleValue(GamePreferences.ReducedMotion);
        }
    }
}
