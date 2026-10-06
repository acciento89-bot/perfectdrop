using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Kamilunavo.PerfectDrop.Input
{
    public sealed class HoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private bool _pressed;
        public void OnPointerDown(PointerEventData eventData) => _pressed = true;
        public void OnPointerUp(PointerEventData eventData) { }
        public bool ConsumePress() { if (!_pressed) return false; _pressed = false; return true; }

        public static HoldButton Create(Transform parent, string label, Vector2 min, Vector2 max, Color color)
        {
            var go = new GameObject(label + "Button", typeof(RectTransform), typeof(Image), typeof(HoldButton));
            go.transform.SetParent(parent, false);
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = min; rect.anchorMax = max; rect.offsetMin = Vector2.zero; rect.offsetMax = Vector2.zero;
            go.GetComponent<Image>().color = color;
            UI.UiFactory.Label(go.transform, "Label", label, 42, Vector2.zero, Vector2.one, TextAnchor.MiddleCenter, Color.white, FontStyle.Bold);
            return go.GetComponent<HoldButton>();
        }
    }
}
