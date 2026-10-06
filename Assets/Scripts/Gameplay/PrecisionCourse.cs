using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        private static readonly Color Dark = new(0.035f, 0.055f, 0.09f);
        private static readonly Color Gold = new(1f, 0.63f, 0.08f);

        public void Build()
        {
            Random.InitState(260906);
            var x = 0f; var y = 0f; var z = 0f;
            for (var i = 0; i < 30; i++)
            {
                if (i > 0) { x = Mathf.Clamp(x + Random.Range(-2.2f, 2.2f), -5.8f, 5.8f); y += Random.Range(0.65f, 1.05f); z += Random.Range(4.3f, 5.1f); }
                var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                go.name = $"Floor_{i + 1:00}";
                go.transform.SetParent(transform, false);
                go.transform.position = new Vector3(x, y, z);
                go.transform.localScale = new Vector3(5.6f, 0.65f, 4.2f);
                go.GetComponent<Renderer>().material = new Material(Shader.Find("Standard")) { color = Dark };
                var marker = go.AddComponent<PrecisionPlatform>();
                marker.Index = i;
                marker.BayHalfWidth = 1.65f;
                _platforms.Add(marker);
                var bay = GameObject.CreatePrimitive(PrimitiveType.Cube);
                bay.name = "LandingBay";
                bay.transform.SetParent(go.transform, false);
                bay.transform.localPosition = new Vector3(0f, 0.53f, 0f);
                bay.transform.localScale = new Vector3(0.62f, 0.08f, 0.62f);
                Destroy(bay.GetComponent<Collider>());
                bay.GetComponent<Renderer>().material = new Material(Shader.Find("Standard")) { color = Gold };
            }
            _safePosition = _platforms[0].transform.position + Vector3.up * 1.5f;
            Player.position = _safePosition;
            RefreshHud();
        }

        private void Update()
        {
            if (Player == null || _platforms.Count == 0) return;
            var floorY = _platforms[Mathf.Clamp(_currentFloor, 0, _platforms.Count - 1)].transform.position.y;
            if (Player.position.y < floorY - 9f) Respawn();
        }

        public void RegisterLanding(PrecisionPlatform platform, Vector3 playerPosition)
        {
            if (platform.Index <= _currentFloor || platform.Index > _currentFloor + 1) return;
            var local = platform.transform.InverseTransformPoint(playerPosition);
            var grade = PrecisionScoring.Grade(Mathf.Abs(local.x), platform.BayHalfWidth);
            _currentFloor = platform.Index;
            _best = Mathf.Max(_best, _currentFloor + 1);
            _streak = grade == LandingGrade.Safe ? 0 : _streak + 1;
            _coins += PrecisionScoring.CoinReward(grade, _streak);
            _safePosition = platform.transform.position + Vector3.up * 1.5f;
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
