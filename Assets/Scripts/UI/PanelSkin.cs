using UnityEngine;
using UnityEngine.UI;

namespace Kamilunavo.PerfectDrop.UI
{
    [RequireComponent(typeof(Image))]
    public sealed class PanelSkin : BaseMeshEffect
    {
        public override void ModifyMesh(VertexHelper vertices)
        {
            if (!IsActive()) return;
            var rect = graphic.rectTransform.rect;
            if (rect.width <= 0 || rect.height <= 0) return;
            var vertex = new UIVertex();
            for (var i = 0; i < vertices.currentVertCount; i++)
            {
                vertices.PopulateUIVertex(ref vertex, i);
                vertex.uv1 = new Vector4((vertex.position.x-rect.xMin)/rect.width,
                    (vertex.position.y-rect.yMin)/rect.height, rect.width, rect.height);
                vertices.SetUIVertex(vertex, i);
            }
        }
    }
}
