using UnityEngine;
using UnityEngine.UI;

namespace Kamilunavo.PerfectDrop.UI
{
    // A thin, original metal surround; the hollow center never obscures previews or labels.
    public sealed class GalleryFrameGraphic : MaskableGraphic
    {
        protected override void OnPopulateMesh(VertexHelper mesh)
        {
            mesh.Clear();
            var rect = rectTransform.rect;
            if (rect.width <= 0 || rect.height <= 0) return;
            var canvas = GetComponentInParent<Canvas>();
            float unit = UiMetrics.PointScale / Mathf.Max(.001f, canvas != null ? canvas.scaleFactor : 1);
            float inset = Mathf.Min(1.6f * unit, Mathf.Min(rect.width, rect.height) * .1f);
            var surface=transform.parent.GetComponent<Image>();bool rounded=surface!=null&&surface.sprite!=null&&surface.sprite.name.Contains("Circle");
            float radius=rounded?Mathf.Min(rect.width,rect.height)*.5f:Mathf.Min(2*unit,Mathf.Min(rect.width,rect.height)*.25f);
            const int steps = 8, count = steps * 4;
            for (int i = 0; i < count; i++)
            {
                int corner = i / steps;
                float angle = (corner * 90 + i % steps * 90f / steps) * Mathf.Deg2Rad;
                var center = new Vector2(corner == 0 || corner == 3 ? rect.xMax - radius : rect.xMin + radius,
                    corner < 2 ? rect.yMax - radius : rect.yMin + radius);
                var direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                var outer = center + direction * radius;
                var inner = center + direction * (radius - inset);
                var tint = Color.Lerp(color * new Color(.68f, .68f, .68f, 1), color,
                    Mathf.InverseLerp(rect.yMin, rect.yMax, outer.y));
                mesh.AddVert(outer, tint, Vector2.zero);
                mesh.AddVert(inner, tint, Vector2.zero);
            }
            for (int i = 0; i < count; i++)
            {
                int next = (i + 1) % count;
                mesh.AddTriangle(i * 2, next * 2, i * 2 + 1);
                mesh.AddTriangle(next * 2, next * 2 + 1, i * 2 + 1);
            }
        }

        public static GalleryFrameGraphic Apply(Transform owner, Color tint)
        {
            var child = owner.Find("MetalFrame");
            GalleryFrameGraphic frame;
            if (child != null) frame = child.GetComponent<GalleryFrameGraphic>();
            else
            {
                var go = new GameObject("MetalFrame", typeof(RectTransform), typeof(CanvasRenderer), typeof(GalleryFrameGraphic));
                go.transform.SetParent(owner, false);
                frame = go.GetComponent<GalleryFrameGraphic>();
                frame.rectTransform.anchorMin = Vector2.zero;
                frame.rectTransform.anchorMax = Vector2.one;
                frame.rectTransform.offsetMin = frame.rectTransform.offsetMax = Vector2.zero;
                frame.raycastTarget = false;
            }
            // The authored capsule already includes its own complete metal rim.
            frame.enabled = owner.GetComponent<ConceptGoldSkin>()?.enabled != true && owner.GetComponent<ConceptCardSkin>()?.enabled != true;
            frame.color = tint;
            return frame;
        }
    }
}
