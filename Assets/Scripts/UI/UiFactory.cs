using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Kamilunavo.PerfectDrop.UI
{
    public static class UiFactory
    {
        private static Font _font;
        public static Font DefaultFont => _font != null ? _font : (_font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"));

        public static Canvas CreateCanvas()
        {
            var go = new GameObject("MobileCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = go.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = go.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080f, 1920f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;
            return canvas;
        }

        public static RectTransform Panel(Transform parent, string name, Color color, Vector2 min, Vector2 max)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image));
            go.transform.SetParent(parent, false);
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = min;
            rect.anchorMax = max;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            go.GetComponent<Image>().color = color;
            return rect;
        }

        public static Text Label(Transform parent, string name, string text, int size, Vector2 min, Vector2 max, TextAnchor alignment, Color color, FontStyle style = FontStyle.Normal)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Text));
            go.transform.SetParent(parent, false);
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = min;
            rect.anchorMax = max;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            var label = go.GetComponent<Text>();
            label.font = DefaultFont;
            label.text = text;
            label.fontSize = size;
            label.fontStyle = style;
            label.alignment = alignment;
            label.color = color;
            label.resizeTextForBestFit = true;
            label.resizeTextMinSize = Mathf.Max(12, size / 2);
            label.resizeTextMaxSize = size;
            return label;
        }

        public static Button Button(Transform parent, string name, string text, Color background, Color foreground, Vector2 min, Vector2 max, UnityAction onClick)
        {
            var rect = Panel(parent, name, background, min, max);
            var button = rect.gameObject.AddComponent<Button>();
            if (onClick != null) button.onClick.AddListener(onClick);
            Label(rect, "Label", text, 34, Vector2.zero, Vector2.one, TextAnchor.MiddleCenter, foreground, FontStyle.Bold);
            return button;
        }

        public static Image Progress(Transform parent, Vector2 min, Vector2 max, Color track, Color fill)
        {
            var trackRect = Panel(parent, "ProgressTrack", track, min, max);
            var fillRect = Panel(trackRect, "Fill", fill, Vector2.zero, Vector2.one);
            var image = fillRect.GetComponent<Image>();
            image.type = Image.Type.Filled;
            image.fillMethod = Image.FillMethod.Horizontal;
            image.fillAmount = 0f;
            return image;
        }
    }
}
