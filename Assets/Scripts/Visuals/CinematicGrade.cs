using UnityEngine;

namespace Kamilunavo.PerfectDrop.Visuals
{
    [RequireComponent(typeof(Camera))]
    public sealed class CinematicGrade : MonoBehaviour
    {
        [Range(0f, 1f)] public float Vignette = 0.30f;
        [Range(0f, 2f)] public float Saturation = 1.10f;
        [Range(0.5f, 2f)] public float Contrast = 1.08f;
        [Range(-1f, 1f)] public float Warmth = 0.07f;

        private Material _material;

        private void OnEnable()
        {
            var shader = Resources.Load<Shader>("PerfectDropGrade");
            if (shader == null) shader = Shader.Find("Kamilunavo/PerfectDropGrade");
            if (shader != null)
                _material = new Material(shader) { hideFlags = HideFlags.HideAndDontSave };
        }

        private void OnDisable()
        {
            if (_material != null)
                Destroy(_material);
            _material = null;
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (_material == null)
            {
                Graphics.Blit(source, destination);
                return;
            }

            _material.SetFloat("_Vignette", Vignette);
            _material.SetFloat("_Saturation", Saturation);
            _material.SetFloat("_Contrast", Contrast);
            _material.SetFloat("_Warmth", Warmth);
            Graphics.Blit(source, destination, _material);
        }
    }
}
