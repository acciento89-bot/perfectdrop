using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Kamilunavo.PerfectDrop.Visuals;

namespace Kamilunavo.PerfectDrop.Gameplay
{
    public sealed class PrecisionCourse : MonoBehaviour
    {
        private const string BestKey = "perfectdrop.bestFloor";
        private const string CoinsKey = "perfectdrop.coins";
        private const float FallRecoveryDistance = 3.0f;

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

            Random.InitState(260906);
            var x = 0f;
            var y = 0f;
            var z = 0f;

            for (var i = 0; i < 30; i++)
            {
                if (i > 0)
                {
                    // Mobile-first spacing: every jump is reachable without requiring a paid/temporary boost.
                    // Difficulty comes from landing precision and lateral correction, not impossible gaps.
                    x = Mathf.Clamp(x + Random.Range(-1.45f, 1.45f), -5.2f, 5.2f);
                    y += Random.Range(0.58f, 0.92f);
                    z += Random.Range(3.15f, 3.75f);
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
