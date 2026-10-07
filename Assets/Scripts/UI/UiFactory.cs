using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Kamilunavo.PerfectDrop.UI
{
    public static class UiFactory
    {
        private static Font _font;
        private static Sprite _roundedSprite;
        private static Sprite _circleSprite;
        private static Sprite _pillSprite;

        public static Font DefaultFont => _font != null ? _font : (_font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"));
        public static Sprite RoundedSprite => _roundedSprite != null ? _roundedSprite : (_roundedSprite = BuildMaskSprite(64, 14f, sliced: true));
        public static Sprite CircleSprite => _circleSprite != null ? _circleSprite : (_circleSprite = BuildMaskSprite(64, 31f, sliced: false));
        public static Sprite PillSprite => _pillSprite != null ? _pillSprite : (_pillSprite = BuildMaskSprite(64, 28f, sliced: true));

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

            var image = go.GetComponent<Image>();
            image.color = color;
            image.sprite = RoundedSprite;
            image.type = Image.Type.Sliced;
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

            var shadow = go.AddComponent<Shadow>();
            shadow.effectColor = new Color(0f, 0f, 0f, 0.45f);
            shadow.effectDistance = new Vector2(0f, -1.5f);
            shadow.useGraphicAlpha = true;
            return label;
        }

        public static Button Button(Transform parent, string name, string text, Color background, Color foreground, Vector2 min, Vector2 max, UnityAction onClick)
        {
            var rect = Panel(parent, name, background, min, max);
            var shadow = rect.gameObject.AddComponent<Shadow>();
            shadow.effectColor = new Color(0f, 0f, 0f, 0.34f);
            shadow.effectDistance = new Vector2(0f, -4f);

            var button = rect.gameObject.AddComponent<Button>();
            var colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(1.08f, 1.08f, 1.08f, 1f);
            colors.pressedColor = new Color(0.84f, 0.84f, 0.84f, 1f);
            colors.fadeDuration = 0.08f;
            button.colors = colors;
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
            image.fillOrigin = 0;
            image.fillAmount = 0f;
            return image;
        }

        public static void ApplyCircularImage(Image image)
        {
            if (image == null) return;
            image.sprite = CircleSprite;
            image.type = Image.Type.Simple;
            image.preserveAspect = true;
        }

        public static void ApplyRoundedImage(Image image)
        {
            if (image == null) return;
            image.sprite = RoundedSprite;
            image.type = Image.Type.Sliced;
        }

        public static void ApplyPillImage(Image image)
        {
            if (image == null) return;
            image.sprite = PillSprite;
            image.type = Image.Type.Sliced;
        }

        public static Outline AddOutline(Graphic graphic, Color color, float thickness)
        {
            if (graphic == null) return null;
            var outline = graphic.gameObject.AddComponent<Outline>();
            outline.effectColor = color;
            outline.effectDistance = new Vector2(Mathf.Max(1f, thickness), Mathf.Max(1f, thickness));
            outline.useGraphicAlpha = true;
            return outline;
        }

        private static Sprite BuildMaskSprite(int size, float radius, bool sliced)
        {
            var texture = new Texture2D(size, size, TextureFormat.RGBA32, false)
            {
                name = sliced ? "KamilunavoRoundedMask" : "KamilunavoCircleMask",
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp,
                hideFlags = HideFlags.HideAndDontSave
            };

            var pixels = new Color32[size * size];
            var half = size * 0.5f;
            var inner = half - radius;

            for (var y = 0; y < size; y++)
            {
                for (var x = 0; x < size; x++)
                {
                    var px = Mathf.Abs((x + 0.5f) - half);
                    var py = Mathf.Abs((y + 0.5f) - half);
                    var dx = Mathf.Max(px - inner, 0f);
                    var dy = Mathf.Max(py - inner, 0f);
                    var distance = Mathf.Sqrt(dx * dx + dy * dy);
                    var alpha = Mathf.Clamp01(radius + 0.75f - distance);
                    pixels[y * size + x] = new Color32(255, 255, 255, (byte)Mathf.RoundToInt(alpha * 255f));
                }
            }

            texture.SetPixels32(pixels);
            texture.Apply(false, true);

            var border = sliced ? new Vector4(radius + 2f, radius + 2f, radius + 2f, radius + 2f) : Vector4.zero;
            var sprite = Sprite.Create(texture, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect, border);
            sprite.name = texture.name + "Sprite";
            sprite.hideFlags = HideFlags.HideAndDontSave;
            return sprite;
        }
    }
}
