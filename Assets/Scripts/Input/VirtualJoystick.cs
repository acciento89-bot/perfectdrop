using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Kamilunavo.PerfectDrop.Input
{
    public sealed class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        public RectTransform Knob { get; set; }
        public Vector2 Value { get; private set; }
        private RectTransform _rect;
        private void Awake() => _rect = (RectTransform)transform;
        public void OnPointerDown(PointerEventData eventData) => UpdateValue(eventData);
        public void OnDrag(PointerEventData eventData) => UpdateValue(eventData);
        public void OnPointerUp(PointerEventData eventData) { Value = Vector2.zero; if (Knob != null) Knob.anchoredPosition = Vector2.zero; }

        private void UpdateValue(PointerEventData eventData)
        {
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(_rect, eventData.position, eventData.pressEventCamera, out var local)) return;
            var radius = Mathf.Min(_rect.rect.width, _rect.rect.height) * 0.5f;
            Value = Vector2.ClampMagnitude(local / Mathf.Max(1f, radius), 1f);
            if (Knob != null) Knob.anchoredPosition = Value * radius * 0.42f;
        }

        public static VirtualJoystick Create(Transform parent, Vector2 min, Vector2 max)
        {
            var root = new GameObject("Joystick", typeof(RectTransform), typeof(Image), typeof(VirtualJoystick));
            root.transform.SetParent(parent, false);
            var rect = root.GetComponent<RectTransform>();
            rect.anchorMin = min; rect.anchorMax = max; rect.offsetMin = Vector2.zero; rect.offsetMax = Vector2.zero;
            var baseImage = root.GetComponent<Image>();
            baseImage.color = new Color(0.03f, 0.06f, 0.11f, 0.72f);
            UI.UiFactory.ApplyCircularImage(baseImage);
            var baseShadow = root.AddComponent<Shadow>();
            baseShadow.effectColor = new Color(0f, 0f, 0f, 0.34f);
            baseShadow.effectDistance = new Vector2(0f, -4f);
            var knob = new GameObject("Knob", typeof(RectTransform), typeof(Image));
            knob.transform.SetParent(root.transform, false);
            var kr = knob.GetComponent<RectTransform>();
            kr.anchorMin = new Vector2(0.28f, 0.28f); kr.anchorMax = new Vector2(0.72f, 0.72f); kr.offsetMin = Vector2.zero; kr.offsetMax = Vector2.zero;
            var knobImage = knob.GetComponent<Image>();
            knobImage.color = new Color(0.86f, 0.90f, 0.96f, 0.94f);
            UI.UiFactory.ApplyCircularImage(knobImage);
            UI.UiFactory.Label(root.transform, "UpHint", "▲", 18, new Vector2(0.42f, 0.78f), new Vector2(0.58f, 0.96f), TextAnchor.MiddleCenter, new Color(0.72f, 0.77f, 0.86f, 0.62f), FontStyle.Bold);
            UI.UiFactory.Label(root.transform, "DownHint", "▼", 18, new Vector2(0.42f, 0.04f), new Vector2(0.58f, 0.22f), TextAnchor.MiddleCenter, new Color(0.72f, 0.77f, 0.86f, 0.62f), FontStyle.Bold);
            UI.UiFactory.Label(root.transform, "LeftHint", "◀", 18, new Vector2(0.04f, 0.42f), new Vector2(0.20f, 0.58f), TextAnchor.MiddleCenter, new Color(0.72f, 0.77f, 0.86f, 0.62f), FontStyle.Bold);
            UI.UiFactory.Label(root.transform, "RightHint", "▶", 18, new Vector2(0.80f, 0.42f), new Vector2(0.96f, 0.58f), TextAnchor.MiddleCenter, new Color(0.72f, 0.77f, 0.86f, 0.62f), FontStyle.Bold);
            var joystick = root.GetComponent<VirtualJoystick>();
            joystick.Knob = kr;
            return joystick;
        }
    }
}
