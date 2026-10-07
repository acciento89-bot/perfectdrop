using UnityEngine;

namespace Kamilunavo.PerfectDrop.UI
{
    public enum MobileLayoutClass
    {
        CompactPortrait,
        WidePortrait,
        Landscape,
        VerticalDivision,
        HorizontalDivision
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
        private RectTransform _completion;
        private RectTransform _settings;
        private RectTransform _progression;
        private RectTransform _root;

        private Vector2 _lastSize = new(-1f, -1f);
        private Rect _lastSafeArea = new(-1f, -1f, -1f, -1f);
        private Rect _divisionRegion;
        private bool _hasDivisionRegion;
        private float _nextReservedRegionPoll;

        private const float ReservedRegionPollInterval = 0.08f;

        public MobileLayoutClass CurrentLayout { get; private set; }
        public bool HasActiveDivisionRegion => _hasDivisionRegion;

        public void Bind(
            RectTransform[] stats,
            RectTransform objective,
            RectTransform feedback,
            RectTransform joystick,
            RectTransform boosts,
            RectTransform jump,
            RectTransform completion,
            RectTransform settings,
            RectTransform progression)
        {
            _root = GetComponent<RectTransform>();
            _stats = stats;
            _objective = objective;
            _feedback = feedback;
            _joystick = joystick;
            _boosts = boosts;
            _jump = jump;
            _completion = completion;
            _settings = settings;
            _progression = progression;
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
            var reservedRegionChanged = PollDivisionRegion();
            if (!force && !reservedRegionChanged && (size - _lastSize).sqrMagnitude < 0.25f && safeArea == _lastSafeArea)
                return;

            _lastSize = size;
            _lastSafeArea = safeArea;

            if (_hasDivisionRegion && TryConvertToSafeAreaSpace(_divisionRegion, out var safeDivision))
            {
                if (IsVerticalDivision(safeDivision))
                {
                    CurrentLayout = MobileLayoutClass.VerticalDivision;
                    ApplyVerticalDivision(safeDivision);
                }
                else
                {
                    CurrentLayout = MobileLayoutClass.HorizontalDivision;
                    ApplyHorizontalDivision(safeDivision);
                }
                return;
            }

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

        public static bool IsVerticalDivision(Rect region) => region.height >= region.width;

        private bool PollDivisionRegion()
        {
            if (Time.unscaledTime < _nextReservedRegionPoll) return false;
            _nextReservedRegionPoll = Time.unscaledTime + ReservedRegionPollInterval;

            var hasRegion = ReservedRegionProvider.TryGetDivisionRegion(out var region);
            var changed = hasRegion != _hasDivisionRegion || (hasRegion && RectChanged(region, _divisionRegion));
            _hasDivisionRegion = hasRegion;
            if (hasRegion) _divisionRegion = region;

            if (changed && Debug.isDebugBuild)
            {
                Debug.Log(hasRegion
                    ? $"[PerfectDrop][Duo] Division region active: x={region.x:F3}, y={region.y:F3}, w={region.width:F3}, h={region.height:F3}"
                    : "[PerfectDrop][Duo] Division region inactive.");
            }

            return changed;
        }

        private static bool RectChanged(Rect a, Rect b)
        {
            const float epsilon = 0.001f;
            return Mathf.Abs(a.x - b.x) > epsilon ||
                   Mathf.Abs(a.y - b.y) > epsilon ||
                   Mathf.Abs(a.width - b.width) > epsilon ||
                   Mathf.Abs(a.height - b.height) > epsilon;
        }

        private static bool TryConvertToSafeAreaSpace(Rect screenRegion, out Rect safeRegion)
        {
            safeRegion = default;
            if (Screen.width <= 0 || Screen.height <= 0) return false;

            var safe = Screen.safeArea;
            var safeNormalized = new Rect(
                safe.xMin / Screen.width,
                safe.yMin / Screen.height,
                safe.width / Screen.width,
                safe.height / Screen.height);

            var xMin = Mathf.Max(screenRegion.xMin, safeNormalized.xMin);
            var yMin = Mathf.Max(screenRegion.yMin, safeNormalized.yMin);
            var xMax = Mathf.Min(screenRegion.xMax, safeNormalized.xMax);
            var yMax = Mathf.Min(screenRegion.yMax, safeNormalized.yMax);
            if (xMax <= xMin || yMax <= yMin || safeNormalized.width <= 0f || safeNormalized.height <= 0f)
                return false;

            safeRegion = Rect.MinMaxRect(
                Mathf.Clamp01((xMin - safeNormalized.xMin) / safeNormalized.width),
                Mathf.Clamp01((yMin - safeNormalized.yMin) / safeNormalized.height),
                Mathf.Clamp01((xMax - safeNormalized.xMin) / safeNormalized.width),
                Mathf.Clamp01((yMax - safeNormalized.yMin) / safeNormalized.height));
            return safeRegion.width > 0.0001f && safeRegion.height > 0.0001f;
        }

        private void ApplyVerticalDivision(Rect division)
        {
            const float margin = 0.018f;
            var leftMax = Mathf.Clamp(division.xMin - margin, 0.30f, 0.49f);
            var rightMin = Mathf.Clamp(division.xMax + margin, 0.51f, 0.70f);
            var left = Rect.MinMaxRect(0.02f, 0.02f, leftMax, 0.98f);
            var right = Rect.MinMaxRect(rightMin, 0.02f, 0.98f, 0.98f);

            PlaceInArea(_stats[0], left, new Vector2(0.02f, 0.87f), new Vector2(0.48f, 0.98f));
            PlaceInArea(_stats[1], left, new Vector2(0.52f, 0.87f), new Vector2(0.98f, 0.98f));
            PlaceInArea(_stats[2], right, new Vector2(0.02f, 0.87f), new Vector2(0.48f, 0.98f));
            PlaceInArea(_stats[3], right, new Vector2(0.52f, 0.87f), new Vector2(0.98f, 0.98f));

            PlaceInArea(_objective, right, new Vector2(0.03f, 0.71f), new Vector2(0.97f, 0.84f));
            PlaceInArea(_feedback, right, new Vector2(0.26f, 0.63f), new Vector2(0.74f, 0.69f));
            PlaceInArea(_joystick, left, new Vector2(0.02f, 0.04f), new Vector2(0.42f, 0.30f));
            PlaceInArea(_boosts, right, new Vector2(0.04f, 0.06f), new Vector2(0.48f, 0.19f));
            PlaceInArea(_jump, right, new Vector2(0.62f, 0.04f), new Vector2(0.98f, 0.30f));
            PlaceInArea(_completion, right, new Vector2(0.05f, 0.30f), new Vector2(0.95f, 0.68f));
            PlaceInArea(_settings, right, new Vector2(0.03f, 0.18f), new Vector2(0.97f, 0.82f));
            PlaceInArea(_progression, right, new Vector2(0.02f, 0.08f), new Vector2(0.98f, 0.92f));
        }

        private void ApplyHorizontalDivision(Rect division)
        {
            const float margin = 0.018f;
            var bottomMax = Mathf.Clamp(division.yMin - margin, 0.28f, 0.49f);
            var topMin = Mathf.Clamp(division.yMax + margin, 0.51f, 0.72f);
            var bottom = Rect.MinMaxRect(0.02f, 0.02f, 0.98f, bottomMax);
            var top = Rect.MinMaxRect(0.02f, topMin, 0.98f, 0.98f);

            for (var i = 0; i < 4; i++)
            {
                var minX = 0.01f + i * 0.247f;
                PlaceInArea(_stats[i], top, new Vector2(minX, 0.76f), new Vector2(minX + 0.22f, 0.98f));
            }

            PlaceInArea(_objective, top, new Vector2(0.03f, 0.43f), new Vector2(0.97f, 0.70f));
            PlaceInArea(_feedback, top, new Vector2(0.32f, 0.26f), new Vector2(0.68f, 0.39f));
            PlaceInArea(_joystick, bottom, new Vector2(0.02f, 0.08f), new Vector2(0.25f, 0.52f));
            PlaceInArea(_boosts, bottom, new Vector2(0.36f, 0.12f), new Vector2(0.64f, 0.38f));
            PlaceInArea(_jump, bottom, new Vector2(0.79f, 0.08f), new Vector2(0.98f, 0.50f));
            PlaceInArea(_completion, top, new Vector2(0.12f, 0.12f), new Vector2(0.88f, 0.80f));
            PlaceInArea(_settings, top, new Vector2(0.08f, 0.04f), new Vector2(0.92f, 0.94f));
            PlaceInArea(_progression, top, new Vector2(0.04f, 0.02f), new Vector2(0.96f, 0.98f));
        }

        private static void PlaceInArea(RectTransform rect, Rect area, Vector2 localMin, Vector2 localMax)
        {
            var min = new Vector2(
                Mathf.Lerp(area.xMin, area.xMax, localMin.x),
                Mathf.Lerp(area.yMin, area.yMax, localMin.y));
            var max = new Vector2(
                Mathf.Lerp(area.xMin, area.xMax, localMax.x),
                Mathf.Lerp(area.yMin, area.yMax, localMax.y));
            Place(rect, min, max);
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
            Place(_completion, new Vector2(0.09f, 0.33f), new Vector2(0.91f, 0.67f));
            Place(_settings, new Vector2(0.12f, 0.25f), new Vector2(0.88f, 0.75f));
            Place(_progression, new Vector2(0.08f, 0.13f), new Vector2(0.92f, 0.86f));
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
            Place(_completion, new Vector2(0.12f, 0.32f), new Vector2(0.88f, 0.69f));
            Place(_settings, new Vector2(0.14f, 0.23f), new Vector2(0.86f, 0.77f));
            Place(_progression, new Vector2(0.10f, 0.11f), new Vector2(0.90f, 0.89f));
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
            Place(_completion, new Vector2(0.54f, 0.25f), new Vector2(0.96f, 0.72f));
            Place(_settings, new Vector2(0.54f, 0.15f), new Vector2(0.96f, 0.82f));
            Place(_progression, new Vector2(0.53f, 0.07f), new Vector2(0.97f, 0.93f));
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
