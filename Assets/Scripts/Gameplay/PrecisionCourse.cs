using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Kamilunavo.PerfectDrop.Visuals;

namespace Kamilunavo.PerfectDrop.Gameplay
{
    public sealed class PrecisionCourse : MonoBehaviour
    {
        public Transform Player;
        public Text FloorText;
        public Text BestText;
        public Text StreakText;
        public Text CoinsText;
        public Text FeedbackText;
        public Image ProgressFill;

        private readonly List<PrecisionPlatform> _platforms = new();
        private int _currentFloor;
        private int _best;
        private int _streak;
        private int _coins;
        private Vector3 _safePosition;
        private const float FallRecoveryDistance = 3.6f;

        public void Build()
        {
            Random.InitState(260906);
            var x = 0f;
            var y = 0f;
            var z = 0f;

            for (var i = 0; i < 30; i++)
            {
                if (i > 0)
                {
                    x = Mathf.Clamp(x + Random.Range(-2.2f, 2.2f), -5.8f, 5.8f);
                    y += Random.Range(0.65f, 1.05f);
                    z += Random.Range(4.3f, 5.1f);
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
                WorldArt.CreateLandingBay(go.transform, marker.BayHalfWidth, marker.BayHalfDepth);
            }

            WorldArt.BuildSkyline(transform);
            WorldArt.BuildGoalBeacon(transform, new Vector3(x, y + 4.8f, z + 8f));

            _safePosition = SpawnPoint(_platforms[0].transform);
            Player.position = _safePosition;
            RefreshHud();
        }

        private void Update()
        {
            if (Player == null || _platforms.Count == 0) return;
            var floorY = _platforms[Mathf.Clamp(_currentFloor, 0, _platforms.Count - 1)].transform.position.y;
            if (Player.position.y < floorY - FallRecoveryDistance) Respawn();
        }

        public void RegisterLanding(PrecisionPlatform platform, Vector3 playerPosition)
        {
            if (platform.Index <= _currentFloor || platform.Index > _currentFloor + 1) return;

            var offset = playerPosition - platform.transform.position;
            var localX = Vector3.Dot(offset, platform.transform.right.normalized);
            var localZ = Vector3.Dot(offset, platform.transform.forward.normalized);
            var grade = PrecisionScoring.Grade(localX, localZ, platform.BayHalfWidth, platform.BayHalfDepth);

            _currentFloor = platform.Index;
            _best = Mathf.Max(_best, _currentFloor + 1);
            _streak = grade == LandingGrade.Safe ? 0 : _streak + 1;
            _coins += PrecisionScoring.CoinReward(grade, _streak);
            _safePosition = SpawnPoint(platform.transform);

            if (FeedbackText != null) FeedbackText.text = grade.ToString().ToUpperInvariant();
            RefreshHud();
        }

        public void Respawn()
        {
            var controller = Player.GetComponent<CharacterController>();
            if (controller != null) controller.enabled = false;
            Player.position = _safePosition;
            if (controller != null) controller.enabled = true;

            _streak = 0;
            if (FeedbackText != null) FeedbackText.text = "READY";
            RefreshHud();
        }

        private static Vector3 SpawnPoint(Transform platform)
        {
            var top = platform.lossyScale.y * 0.5f;
            return platform.position + Vector3.up * (top + 0.06f);
        }

        private void RefreshHud()
        {
            FloorText.text = $"FLOOR\n{_currentFloor + 1}";
            BestText.text = $"BEST\n{Mathf.Max(1, _best)}";
            StreakText.text = $"STREAK\nx{_streak}";
            CoinsText.text = $"COINS\n{_coins}";
            if (ProgressFill != null) ProgressFill.fillAmount = (_currentFloor + 1) / 30f;
        }
    }
}
