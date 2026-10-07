using UnityEngine;
using Kamilunavo.PerfectDrop.Gameplay;

namespace Kamilunavo.PerfectDrop.Feedback
{
    [RequireComponent(typeof(AudioSource))]
    public sealed class FeedbackSystem : MonoBehaviour
    {
        private AudioSource _source;
        private AudioClip _jump;
        private AudioClip _perfect;
        private AudioClip _good;
        private AudioClip _safe;
        private AudioClip _recovery;
        private AudioClip _complete;

        private void Awake()
        {
            _source = GetComponent<AudioSource>();
            _source.playOnAwake = false;
            _source.spatialBlend = 0f;
            _source.volume = 0.72f;

            _jump = Tone("Jump", 410f, 650f, 0.10f, 0.32f);
            _perfect = Tone("Perfect", 690f, 1040f, 0.18f, 0.42f);
            _good = Tone("Good", 540f, 720f, 0.14f, 0.32f);
            _safe = Tone("Safe", 360f, 420f, 0.11f, 0.22f);
            _recovery = Tone("Recovery", 230f, 150f, 0.17f, 0.24f);
            _complete = Tone("Complete", 520f, 1180f, 0.34f, 0.46f);
        }

        public void PlayJump()
        {
            Play(_jump, 0.58f);
        }

        public void PlayLanding(LandingGrade grade)
        {
            switch (grade)
            {
                case LandingGrade.Perfect:
                    Play(_perfect, 0.78f);
                    Haptic();
                    break;
                case LandingGrade.Good:
                    Play(_good, 0.65f);
                    break;
                default:
                    Play(_safe, 0.52f);
                    break;
            }
        }

        public void PlayRecovery()
        {
            Play(_recovery, 0.55f);
        }

        public void PlayComplete()
        {
            Play(_complete, 0.90f);
            Haptic();
        }

        private void Play(AudioClip clip, float volume)
        {
            if (_source == null || clip == null) return;
            _source.PlayOneShot(clip, volume);
        }

        private static void Haptic()
        {
#if UNITY_IOS || UNITY_ANDROID
            if (Application.isMobilePlatform) Handheld.Vibrate();
#endif
        }

        private static AudioClip Tone(string name, float startHz, float endHz, float duration, float gain)
        {
            const int sampleRate = 44100;
            var samples = Mathf.Max(1, Mathf.CeilToInt(duration * sampleRate));
            var data = new float[samples];

            var phase = 0f;
            for (var i = 0; i < samples; i++)
            {
                var progress = i / (float)Mathf.Max(1, samples - 1);
                var frequency = Mathf.Lerp(startHz, endHz, progress);
                phase += 2f * Mathf.PI * frequency / sampleRate;
                var envelope = Mathf.Sin(Mathf.PI * progress);
                envelope *= envelope;
                var fundamental = Mathf.Sin(phase);
                var harmonic = Mathf.Sin(phase * 2f) * 0.18f;
                data[i] = (fundamental + harmonic) * envelope * gain;
            }

            var clip = AudioClip.Create(name, samples, 1, sampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }
    }
}
