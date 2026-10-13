using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Kamilunavo.PerfectDrop.UI
{
    // Original bitmap surfaces share real Canvas clipping and button state tint.
    [RequireComponent(typeof(Image))]
    public sealed class ConceptCardSkin : MonoBehaviour
    {
        static readonly Sprite[] Faces = new Sprite[4];
        static readonly Dictionary<Color, Material> Materials = new();
        static Sprite selectionSprite;
        Image image, selection;

        public static bool Apply(Image target, Color semantic)
        {
            int kind = GalleryTheme.IsIvory(semantic) ? 1 : target.name == "Style3" ? 2 : target.name == "Style5" ? 3 : 0;
            var face = Faces[kind];
            if (face == null)
            {
                face = Load(new[] { "NavyCard", "IvoryCard", "JadeCard", "PearlCard" }[kind]);
                if (face == null) return false;
                Faces[kind] = face;
            }
            if (!Materials.TryGetValue(semantic, out var material))
            {
                var shader = Resources.Load<Shader>("PerfectDropAuthoredUI");
                if (shader == null) return false;
                material = new Material(shader) { name = "OriginalCardStateTint", hideFlags = HideFlags.HideAndDontSave };
                material.SetVector("_ReferenceTint", new Vector4(semantic.r, semantic.g, semantic.b, semantic.a));
                Materials.Add(semantic, material);
            }
            target.sprite = face; target.type = Image.Type.Sliced; target.material = material;
            var skin = target.GetComponent<ConceptCardSkin>() ?? target.gameObject.AddComponent<ConceptCardSkin>();
            skin.image = target; skin.enabled = true; skin.Fit();
            var panel = target.GetComponent<PanelSkin>(); if (panel != null) panel.enabled = false;
            var frame = target.transform.Find("MetalFrame")?.GetComponent<GalleryFrameGraphic>(); if (frame != null) frame.enabled = false;
            return true;
        }

        public static void Select(Image target, bool selected)
        {
            var skin = target.GetComponent<ConceptCardSkin>();
            if (skin == null || !skin.enabled) return;
            if (skin.selection == null)
            {
                if (selectionSprite == null) selectionSprite = Load("SelectedFrame");
                if (selectionSprite == null) return;
                var go = new GameObject("OriginalSelectedFrame", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
                go.transform.SetParent(target.transform, false);
                skin.selection = go.GetComponent<Image>(); skin.selection.sprite = selectionSprite;
                skin.selection.type = Image.Type.Sliced; skin.selection.raycastTarget = false;
                var rect = skin.selection.rectTransform; rect.anchorMin = Vector2.zero; rect.anchorMax = Vector2.one;
                rect.offsetMin = rect.offsetMax = Vector2.zero;
            }
            skin.selection.gameObject.SetActive(selected); skin.selection.transform.SetAsLastSibling(); skin.Fit();
        }

        static Sprite Load(string name)
        {
            var texture = Resources.Load<Texture2D>("Art/ConceptV3/" + name); if (texture == null) return null;
            float edge = Mathf.Min(texture.width, texture.height) * (name=="SelectedFrame"?.23f:.15f);
            var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * .5f,
                100, 0, SpriteMeshType.FullRect, Vector4.one * edge);
            sprite.name = "Original" + name + "9Slice"; sprite.hideFlags = HideFlags.HideAndDontSave; return sprite;
        }

        void OnRectTransformDimensionsChange() { Fit(); }
        void Fit()
        {
            if (image == null || image.sprite == null) return;
            var canvas = image.GetComponentInParent<Canvas>();
            float unit = UiMetrics.PointScale / Mathf.Max(.001f, canvas != null ? canvas.scaleFactor : 1);
            image.pixelsPerUnitMultiplier = image.sprite.border.x / (11 * unit);
            if (selection != null) selection.pixelsPerUnitMultiplier = selection.sprite.border.x / (11 * unit);
        }
    }
}
