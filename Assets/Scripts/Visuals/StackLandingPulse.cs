using UnityEngine;

namespace Kamilunavo.PerfectDrop.Visuals
{
    public sealed class StackLandingPulse : MonoBehaviour
    {
        private Vector3 _scale;
        private Color _color;
        private Renderer _renderer;
        private MaterialPropertyBlock _tint;
        private float _age;
        public void Initialize(Color color)
        {
            _tint=new MaterialPropertyBlock();
            _color=color;_scale=transform.localScale;_renderer=GetComponent<Renderer>();
            Apply(1);
        }
        private void Update()
        {
            if(GamePreferences.ReducedMotion){Destroy(gameObject);return;}
            _age+=Time.deltaTime;var t=Mathf.Clamp01(_age/.38f);
            transform.localScale=new Vector3(_scale.x*(1+t*.16f),_scale.y,_scale.z*(1+t*.16f));
            Apply((1-t)*(1-t));
            if(t>=1)Destroy(gameObject);
        }
        private void Apply(float alpha)
        {
            _tint.SetColor("_Tint",new Color(_color.r,_color.g,_color.b,alpha));_renderer.SetPropertyBlock(_tint);
        }
    }
}
