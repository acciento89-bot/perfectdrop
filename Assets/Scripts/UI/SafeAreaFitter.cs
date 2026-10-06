using UnityEngine;

namespace Kamilunavo.PerfectDrop.UI
{
    [RequireComponent(typeof(RectTransform))]
    public sealed class SafeAreaFitter : MonoBehaviour
    {
        private RectTransform _rect;
        private Rect _last;

        private void Awake() { _rect = GetComponent<RectTransform>(); Apply(); }
        private void Update() { if (Screen.safeArea != _last) Apply(); }

        private void Apply()
        {
            _last = Screen.safeArea;
            var min = _last.position;
            var max = _last.position + _last.size;
            min.x /= Screen.width; min.y /= Screen.height;
            max.x /= Screen.width; max.y /= Screen.height;
            _rect.anchorMin = min;
            _rect.anchorMax = max;
            _rect.offsetMin = Vector2.zero;
            _rect.offsetMax = Vector2.zero;
        }
    }
}
