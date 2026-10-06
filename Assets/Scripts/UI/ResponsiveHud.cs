using UnityEngine;

namespace Kamilunavo.PerfectDrop.UI
{
    public enum MobileLayoutClass
    {
        CompactPortrait,
        WidePortrait,
        Landscape
    }

    [RequireComponent(typeof(RectTransform))]
    public sealed class ResponsiveHud : MonoBehaviour
    {
        private RectTransform[] _stats;
        private RectTransform _objective;
        private RectTransform _feedback;
        private RectTransform _joystick;
        private RectTransform _boosts;
        private RectTransform _jump;
        private RectTransform _root;

        private Vector2 _lastSize = new(-1f, -1f);
        private Rect _lastSafeArea = new(-1f, -1f, -1f, -1f);

        public MobileLayoutClass CurrentLayout { get; private set; }

        public void Bind(
            RectTransform[] stats,
            RectTransform objective,
            RectTransform feedback,
            RectTransform joystick,
            RectTransform boosts,
            RectTransform jump)
        {
            _root = GetComponent<RectTransform>();
            _stats = stats;
            _objective = objective;
            _feedback = feedback;
            _joystick = joystick;
            _boosts = boosts;
            _jump = jump;
            Apply(force: true);
        }

        private void Awake()
        {
            _root = GetComponent<RectTransform>();
        }

        private void OnEnable()
        {
            Apply(force: true);
        }

        private void Update()
        {
            Apply(force: false);
        }

        private void OnRectTransformDimensionsChange()
        {
            if (!isActiveAndEnabled) return;
            Apply(force: true);
        }

        private void Apply(bool force)
        {
            if (_root == null || _stats == null || _stats.Length != 4) return;

            var size = _root.rect.size;
            if (size.x <= 1f || size.y <= 1f)
                size = new Vector2(Mathf.Max(1f, Screen.safeArea.width), Mathf.Max(1f, Screen.safeArea.height));

            var safeArea = Screen.safeArea;
            if (!force && (size - _lastSize).sqrMagnitude < 0.25f && safeArea == _lastSafeArea)
                return;

            _lastSize = size;
            _lastSafeArea = safeArea;

            CurrentLayout = Classify(size.x, size.y);

            switch (CurrentLayout)
            {
                case MobileLayoutClass.WidePortrait:
                    ApplyWidePortrait();
                    break;
                case MobileLayoutClass.Landscape:
                    ApplyLandscape();
                    break;
                default:
                    ApplyCompactPortrait();
                    break;
            }
        }

        public static MobileLayoutClass Classify(float width, float height)
        {
            var aspect = Mathf.Max(1f, width) / Mathf.Max(1f, height);
            if (aspect >= 1.05f) return MobileLayoutClass.Landscape;
            if (aspect >= 0.68f) return MobileLayoutClass.WidePortrait;
            return MobileLayoutClass.CompactPortrait;
        }

        private void ApplyCompactPortrait()
        {
            for (var i = 0; i < 4; i++)
            {
                var minX = 0.03f + i * 0.242f;
                Place(_stats[i], new Vector2(minX, 0.90f), new Vector2(minX + 0.215f, 0.975f));
            }

            Place(_objective, new Vector2(0.03f, 0.79f), new Vector2(0.97f, 0.885f));
            Place(_feedback, new Vector2(0.32f, 0.73f), new Vector2(0.68f, 0.78f));
            Place(_joystick, new Vector2(0.03f, 0.035f), new Vector2(0.29f, 0.18f));
            Place(_boosts, new Vector2(0.34f, 0.045f), new Vector2(0.68f, 0.125f));
            Place(_jump, new Vector2(0.78f, 0.035f), new Vector2(0.97f, 0.17f));
        }

        private void ApplyWidePortrait()
        {
            // iPhone Duo inner display in portrait is much wider than a classic iPhone.
            // Keep critical controls away from the central fold band while letting the world
            // render edge to edge behind it.
            for (var i = 0; i < 4; i++)
            {
                var minX = 0.035f + i * 0.24f;
                Place(_stats[i], new Vector2(minX, 0.905f), new Vector2(minX + 0.205f, 0.975f));
            }

            Place(_objective, new Vector2(0.07f, 0.80f), new Vector2(0.93f, 0.892f));
            Place(_feedback, new Vector2(0.34f, 0.735f), new Vector2(0.66f, 0.785f));
            Place(_joystick, new Vector2(0.04f, 0.04f), new Vector2(0.245f, 0.185f));
            Place(_boosts, new Vector2(0.355f, 0.05f), new Vector2(0.645f, 0.125f));
            Place(_jump, new Vector2(0.80f, 0.04f), new Vector2(0.965f, 0.175f));
        }

        private void ApplyLandscape()
        {
            // Fallback for Duo poses / resizable windows. The 12% center gutter keeps
            // important UI away from the physical division region when the device is folded.
            Place(_stats[0], new Vector2(0.03f, 0.855f), new Vector2(0.215f, 0.97f));
            Place(_stats[1], new Vector2(0.235f, 0.855f), new Vector2(0.42f, 0.97f));
            Place(_stats[2], new Vector2(0.03f, 0.72f), new Vector2(0.215f, 0.835f));
            Place(_stats[3], new Vector2(0.235f, 0.72f), new Vector2(0.42f, 0.835f));

            Place(_objective, new Vector2(0.58f, 0.82f), new Vector2(0.97f, 0.97f));
            Place(_feedback, new Vector2(0.655f, 0.735f), new Vector2(0.895f, 0.80f));
            Place(_joystick, new Vector2(0.03f, 0.055f), new Vector2(0.18f, 0.29f));
            Place(_boosts, new Vector2(0.245f, 0.07f), new Vector2(0.42f, 0.19f));
            Place(_jump, new Vector2(0.82f, 0.055f), new Vector2(0.97f, 0.29f));
        }

        private static void Place(RectTransform rect, Vector2 min, Vector2 max)
        {
            if (rect == null) return;
            rect.anchorMin = min;
            rect.anchorMax = max;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }
    }
}
