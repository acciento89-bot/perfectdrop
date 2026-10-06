using UnityEngine;

namespace Kamilunavo.PerfectDrop.UI
{
    [RequireComponent(typeof(RectTransform))]
    public sealed class SafeAreaFitter : MonoBehaviour
    {
        private RectTransform _rect;
        private Rect _last = new(-1f, -1f, -1f, -1f);
        private int _lastWidth = -1;
        private int _lastHeight = -1;

        private void Awake()
        {
            _rect = GetComponent<RectTransform>();
            Apply();
        }

        private void Update()
        {
            if (Screen.safeArea != _last || Screen.width != _lastWidth || Screen.height != _lastHeight)
                Apply();
        }

        private void OnRectTransformDimensionsChange()
        {
            if (_rect != null) Apply();
        }

        private void Apply()
        {
            if (_rect == null || Screen.width <= 0 || Screen.height <= 0) return;

            _last = Screen.safeArea;
            _lastWidth = Screen.width;
            _lastHeight = Screen.height;

            var min = _last.position;
            var max = _last.position + _last.size;
            min.x /= Screen.width;
            min.y /= Screen.height;
            max.x /= Screen.width;
            max.y /= Screen.height;

            _rect.anchorMin = min;
            _rect.anchorMax = max;
            _rect.offsetMin = Vector2.zero;
            _rect.offsetMax = Vector2.zero;
        }
    }
}
