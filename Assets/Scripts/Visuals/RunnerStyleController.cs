using UnityEngine;

namespace Kamilunavo.PerfectDrop.Visuals
{
    public sealed class RunnerStyleController : MonoBehaviour
    {
        private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
        private static readonly int ColorId = Shader.PropertyToID("_Color");
        private static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");

        private Renderer[] _renderers;
        private MaterialPropertyBlock _block;

        private void Awake()
        {
            _block = new MaterialPropertyBlock();
        }

        private void Start()
        {
            if (_block == null) _block = new MaterialPropertyBlock();
            RefreshRenderers();
            ApplySelectedStyle();
            PlayerProfileStore.Changed += ApplySelectedStyle;
        }

        private void OnDestroy()
        {
            PlayerProfileStore.Changed -= ApplySelectedStyle;
        }

        public void ApplySelectedStyle()
        {
            if (_renderers == null || _renderers.Length == 0)
                RefreshRenderers();

            var style = PlayerProfileStore.SelectedStyle;
            var body = style switch
            {
                1 => new Color(0.025f, 0.11f, 0.17f),
                2 => new Color(0.15f, 0.035f, 0.09f),
                _ => new Color(0.022f, 0.025f, 0.035f)
            };
            var accent = style switch
            {
                1 => new Color(0.05f, 0.82f, 1f),
                2 => new Color(1f, 0.20f, 0.55f),
                _ => new Color(1f, 0.48f, 0.035f)
            };

            foreach (var renderer in _renderers)
            {
                if (renderer == null) continue;
                var name = renderer.gameObject.name;
                if (IsBody(name))
                    ApplyTint(renderer, body, Color.black);
                else if (IsAccent(name))
                    ApplyTint(renderer, accent, accent * 1.55f);
            }
        }

        private void RefreshRenderers()
        {
            _renderers = GetComponentsInChildren<Renderer>(includeInactive: true);
        }

        private void ApplyTint(Renderer renderer, Color color, Color emission)
        {
            if (_block == null) _block = new MaterialPropertyBlock();
            renderer.GetPropertyBlock(_block);
            _block.SetColor(BaseColorId, color);
            _block.SetColor(ColorId, color);
            _block.SetColor(EmissionColorId, emission);
            renderer.SetPropertyBlock(_block);
        }

        private static bool IsBody(string name)
        {
            return name == "Torso" ||
                   name == "Waist" ||
                   name == "Hood" ||
                   name == "LeftArm" ||
                   name == "RightArm" ||
                   name == "LeftLeg" ||
                   name == "RightLeg";
        }

        private static bool IsAccent(string name)
        {
            return name == "Collar" ||
                   name == "LeftCuff" ||
                   name == "RightCuff" ||
                   name == "LeftSoleGlow" ||
                   name == "RightSoleGlow" ||
                   name.StartsWith("BackMark");
        }
    }
}
