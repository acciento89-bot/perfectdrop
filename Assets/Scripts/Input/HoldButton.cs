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
            var image = go.GetComponent<Image>();
            image.color = color;
            UI.UiFactory.ApplyRoundedImage(image);
            var shadow = go.AddComponent<Shadow>();
            shadow.effectColor = new Color(0f, 0f, 0f, 0.34f);
            shadow.effectDistance = new Vector2(0f, -4f);
            UI.UiFactory.Label(go.transform, "Label", label, 42, Vector2.zero, Vector2.one, TextAnchor.MiddleCenter, Color.white, FontStyle.Bold);
            return go.GetComponent<HoldButton>();
        }
    }
}
