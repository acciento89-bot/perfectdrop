using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Kamilunavo.PerfectDrop.Visuals;
using Kamilunavo.PerfectDrop.Feedback;

namespace Kamilunavo.PerfectDrop.Gameplay
{
    public sealed class PrecisionCourse : MonoBehaviour
    {
        private const string BestKey = "perfectdrop.bestFloor";
        private const string CoinsKey = "perfectdrop.coins";
        private const float FallRecoveryDistance = 3.0f;

        // Authored 30-floor route. The path deliberately alternates gentle and stronger
        // lateral corrections while keeping every center-to-center jump inside the
        // conservative no-boost reachability envelope validated by GameplayValidation.
        private static readonly float[] LateralPattern =
        {
             0.55f, -0.95f,  1.30f, -0.45f, -1.35f,  1.65f,  0.35f, -1.70f,  1.15f,  0.80f,
            -1.55f,  1.75f, -0.70f, -1.40f,  1.10f,  1.55f, -1.65f,  0.50f,  1.25f, -1.30f,
            -0.85f,  1.70f, -1.10f,  0.60f,  1.50f, -1.45f,  0.95f, -0.55f,  0.00f
        };

        private static readonly float[] RisePattern =
        {
            0.58f, 0.62f, 0.66f, 0.60f, 0.72f, 0.68f, 0.74f, 0.64f, 0.76f, 0.70f,
            0.78f, 0.70f, 0.82f, 0.74f, 0.84f, 0.76f, 0.80f, 0.72f, 0.86f, 0.78f,
            0.84f, 0.76f, 0.88f, 0.80f, 0.86f, 0.82f, 0.90f, 0.84f, 0.72f
        };

        private static readonly float[] ForwardPattern =
        {
            3.15f, 3.25f, 3.30f, 3.20f, 3.35f, 3.45f, 3.30f, 3.55f, 3.40f, 3.25f,
            3.60f, 3.35f, 3.50f, 3.65f, 3.45f, 3.55f, 3.70f, 3.40f, 3.65f, 3.50f,
            3.55f, 3.70f, 3.60f, 3.45f, 3.70f, 3.55f, 3.65f, 3.50f, 3.40f
        };

        public Transform Player;
        public Text FloorText;
        public Text BestText;
        public Text StreakText;
        public Text CoinsText;
        public Text FeedbackText;
        public Image ProgressFill;
        public GameObject CompletionPanel;
        public Text CompletionText;

        private readonly List<PrecisionPlatform> _platforms = new();
        private PlayerMotor _motor;
        private FeedbackSystem _feedback;
        private int _currentFloor;
        private int _best;
        private int _streak;
        private int _coins;
        private Vector3 _safePosition;
        private bool _completed;

        public int CurrentFloor => _currentFloor + 1;
        public int Best => _best;
        public int Streak => _streak;
        public int Coins => _coins;
        public bool IsCompleted => _completed;
        public IReadOnlyList<PrecisionPlatform> Platforms => _platforms;

        public void Build()
        {
            _best = Mathf.Clamp(PlayerPrefs.GetInt(BestKey, 1), 1, 30);
            _coins = Mathf.Max(0, PlayerPrefs.GetInt(CoinsKey, 0));
            _motor = Player != null ? Player.GetComponent<PlayerMotor>() : null;
            _feedback = Player != null ? Player.GetComponent<FeedbackSystem>() : null;

            var x = 0f;
            var y = 0f;
            var z = 0f;

            for (var i = 0; i < 30; i++)
            {
                if (i > 0)
                {
                    var patternIndex = i - 1;
                    x = Mathf.Clamp(x + LateralPattern[patternIndex], -5.2f, 5.2f);
                    y += RisePattern[patternIndex];
                    z += ForwardPattern[patternIndex];
                }

                var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                go.name = $"Floor_{i + 1:00}";
                go.transform.SetParent(transform, false);
                go.transform.position = new Vector3(x, y, z);
                go.transform.localScale = new Vector3(5.6f, 0.65f, 4.2f);

                var marker = go.AddComponent<PrecisionPlatform>();
                marker.Index = i;
                marker.BayHalfWidth = 1.65f;
                marker.BayHalfDepth = 1.10f;
                _platforms.Add(marker);

                WorldArt.DecoratePlatform(go.transform, i);
                marker.LandingBayRoot = WorldArt.CreateLandingBay(go.transform, marker.BayHalfWidth, marker.BayHalfDepth);
                marker.SetTarget(false);
            }

            WorldArt.BuildAtmosphere(transform);
            WorldArt.BuildSkyline(transform);
            WorldArt.BuildGoalBeacon(transform, new Vector3(x, y + 4.8f, z + 8f));

            StartRun();
        }

        private void Update()
        {
            if (_completed || Player == null || _platforms.Count == 0) return;
            if (Player.position.y < _safePosition.y - FallRecoveryDistance) Respawn();
        }

        public void RegisterLanding(PrecisionPlatform platform, Vector3 playerPosition)
        {
            if (_completed || platform == null) return;
            if (platform.Index <= _currentFloor || platform.Index > _currentFloor + 1) return;

            var offset = playerPosition - platform.transform.position;
            var localX = Vector3.Dot(offset, platform.transform.right.normalized);
            var localZ = Vector3.Dot(offset, platform.transform.forward.normalized);
            var grade = PrecisionScoring.Grade(localX, localZ, platform.BayHalfWidth, platform.BayHalfDepth);

            _platforms[_currentFloor].SetTarget(false);
            platform.SetTarget(false);
            _currentFloor = platform.Index;
            _best = Mathf.Max(_best, _currentFloor + 1);
            _streak = grade == LandingGrade.Safe ? 0 : _streak + 1;
            _coins += PrecisionScoring.CoinReward(grade, _streak);
            _safePosition = SpawnPoint(platform.transform);
            WorldArt.SpawnLandingBurst(platform.transform.position + Vector3.up * (platform.transform.lossyScale.y * 0.5f), grade);
            _feedback?.PlayLanding(grade);

            PlayerPrefs.SetInt(BestKey, _best);
            PlayerPrefs.SetInt(CoinsKey, _coins);
            PlayerPrefs.Save();

            if (FeedbackText != null) FeedbackText.text = UI.GameText.Landing(grade);
            RefreshHud();

            if (_currentFloor >= _platforms.Count - 1)
                CompleteRun();
            else
                _platforms[_currentFloor + 1].SetTarget(true);
        }

        public void Respawn()
        {
            if (_completed || Player == null) return;
            WarpPlayer(_safePosition);
            _feedback?.PlayRecovery();
            _streak = 0;
            if (FeedbackText != null) FeedbackText.text = UI.GameText.Ready;
            RefreshHud();
        }

        public void RestartRun()
        {
            StartRun();
        }

        private void StartRun()
        {
            if (_platforms.Count == 0 || Player == null) return;

            foreach (var platform in _platforms) platform.SetTarget(false);
            _currentFloor = 0;
            _streak = 0;
            _completed = false;
            _safePosition = SpawnPoint(_platforms[0].transform);
            WarpPlayer(_safePosition);
            if (_platforms.Count > 1) _platforms[1].SetTarget(true);
            if (_motor != null) _motor.InputEnabled = true;
            if (CompletionPanel != null) CompletionPanel.SetActive(false);
            if (FeedbackText != null) FeedbackText.text = UI.GameText.Ready;
            RefreshHud();
        }

        private void CompleteRun()
        {
            _completed = true;
            if (_motor != null) _motor.InputEnabled = false;
            if (FeedbackText != null) FeedbackText.text = UI.GameText.TowerCleared;
            if (CompletionText != null)
                CompletionText.text = UI.GameText.Completion(_best, _coins);
            if (CompletionPanel != null) CompletionPanel.SetActive(true);
            _feedback?.PlayComplete();
            RefreshHud();
        }

        private void WarpPlayer(Vector3 position)
        {
            var controller = Player.GetComponent<CharacterController>();
            if (controller != null) controller.enabled = false;
            Player.position = position;
            if (controller != null) controller.enabled = true;
            if (_motor != null) _motor.ResetMotion();
        }

        private static Vector3 SpawnPoint(Transform platform)
        {
            var top = platform.lossyScale.y * 0.5f;
            return platform.position + Vector3.up * (top + 0.06f);
        }

        private void RefreshHud()
        {
            FloorText.text = UI.GameText.Floor(_currentFloor + 1);
            BestText.text = UI.GameText.Best(Mathf.Max(1, _best));
            StreakText.text = UI.GameText.Streak(_streak);
            CoinsText.text = UI.GameText.Coins(_coins);
            if (ProgressFill != null) ProgressFill.fillAmount = (_currentFloor + 1) / 30f;
        }
    }
}
