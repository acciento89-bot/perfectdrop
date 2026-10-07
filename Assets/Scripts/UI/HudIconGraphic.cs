using UnityEngine;
using UnityEngine.UI;

namespace Kamilunavo.PerfectDrop.UI
{
    public enum HudIconType
    {
        Floors,
        Crown,
        Flame,
        Diamond,
        Bolt,
        ArrowUp
    }

    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class HudIconGraphic : MaskableGraphic
    {
        public HudIconType IconType;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            switch (IconType)
            {
                case HudIconType.Floors:
                    AddLayer(vh, 0.50f, 0.68f, 0.32f, 0.15f);
                    AddLayer(vh, 0.50f, 0.49f, 0.32f, 0.15f);
                    AddLayer(vh, 0.50f, 0.30f, 0.32f, 0.15f);
                    break;

                case HudIconType.Crown:
                    AddQuad(vh, P(0.18f, 0.24f), P(0.82f, 0.24f), P(0.76f, 0.38f), P(0.24f, 0.38f));
                    AddTriangle(vh, P(0.24f, 0.38f), P(0.15f, 0.78f), P(0.42f, 0.47f));
                    AddTriangle(vh, P(0.34f, 0.42f), P(0.50f, 0.86f), P(0.63f, 0.42f));
                    AddTriangle(vh, P(0.58f, 0.47f), P(0.85f, 0.78f), P(0.76f, 0.38f));
                    break;

                case HudIconType.Flame:
                    AddTriangle(vh, P(0.25f, 0.25f), P(0.50f, 0.88f), P(0.63f, 0.30f));
                    AddTriangle(vh, P(0.37f, 0.24f), P(0.78f, 0.72f), P(0.70f, 0.25f));
                    AddTriangle(vh, P(0.32f, 0.22f), P(0.62f, 0.22f), P(0.50f, 0.55f));
                    break;

                case HudIconType.Diamond:
                    AddThickLine(vh, P(0.50f, 0.86f), P(0.84f, 0.50f), 0.075f);
                    AddThickLine(vh, P(0.84f, 0.50f), P(0.50f, 0.14f), 0.075f);
                    AddThickLine(vh, P(0.50f, 0.14f), P(0.16f, 0.50f), 0.075f);
                    AddThickLine(vh, P(0.16f, 0.50f), P(0.50f, 0.86f), 0.075f);
                    break;

                case HudIconType.Bolt:
                    AddTriangle(vh, P(0.54f, 0.92f), P(0.24f, 0.48f), P(0.48f, 0.48f));
                    AddTriangle(vh, P(0.46f, 0.08f), P(0.76f, 0.55f), P(0.52f, 0.55f));
                    AddQuad(vh, P(0.45f, 0.44f), P(0.60f, 0.44f), P(0.54f, 0.61f), P(0.39f, 0.61f));
                    break;

                case HudIconType.ArrowUp:
                    AddTriangle(vh, P(0.50f, 0.86f), P(0.18f, 0.48f), P(0.82f, 0.48f));
                    AddQuad(vh, P(0.38f, 0.16f), P(0.62f, 0.16f), P(0.62f, 0.54f), P(0.38f, 0.54f));
                    break;
            }
        }

        private static Vector2 P(float x, float y) => new(x, y);

        private void AddLayer(VertexHelper vh, float cx, float cy, float halfWidth, float halfHeight)
        {
            AddQuad(
                vh,
                P(cx, cy + halfHeight),
                P(cx + halfWidth, cy),
                P(cx, cy - halfHeight),
                P(cx - halfWidth, cy));
        }

        private void AddThickLine(VertexHelper vh, Vector2 a, Vector2 b, float width)
        {
            var dir = (b - a).normalized;
            var normal = new Vector2(-dir.y, dir.x) * width * 0.5f;
            AddQuad(vh, a - normal, a + normal, b + normal, b - normal);
        }

        private void AddTriangle(VertexHelper vh, Vector2 a, Vector2 b, Vector2 c)
        {
            var i = vh.currentVertCount;
            vh.AddVert(ToLocal(a), color, Vector2.zero);
            vh.AddVert(ToLocal(b), color, Vector2.zero);
            vh.AddVert(ToLocal(c), color, Vector2.zero);
            vh.AddTriangle(i, i + 1, i + 2);
        }

        private void AddQuad(VertexHelper vh, Vector2 a, Vector2 b, Vector2 c, Vector2 d)
        {
            var i = vh.currentVertCount;
            vh.AddVert(ToLocal(a), color, Vector2.zero);
            vh.AddVert(ToLocal(b), color, Vector2.zero);
            vh.AddVert(ToLocal(c), color, Vector2.zero);
            vh.AddVert(ToLocal(d), color, Vector2.zero);
            vh.AddTriangle(i, i + 1, i + 2);
            vh.AddTriangle(i, i + 2, i + 3);
        }

        private Vector3 ToLocal(Vector2 normalized)
        {
            var r = rectTransform.rect;
            return new Vector3(
                Mathf.Lerp(r.xMin, r.xMax, normalized.x),
                Mathf.Lerp(r.yMin, r.yMax, normalized.y),
                0f);
        }
    }
}
