using UnityEngine;

namespace Kamilunavo.PerfectDrop.Visuals
{
    [RequireComponent(typeof(Camera))]
    public sealed class CinematicGrade : MonoBehaviour
    {
        [Range(0f, 1f)] public float Vignette = 0.18f;
        [Range(0f, 2f)] public float Saturation = 1.03f;
        [Range(0.5f, 2f)] public float Contrast = 1.03f;
        [Range(-1f, 1f)] public float Warmth = 0.025f;

        private Material _material;
        private Material _bloom;

        private void OnEnable()
        {
            var shader = Resources.Load<Shader>("PerfectDropGrade");
            if (shader == null) shader = Shader.Find("Kamilunavo/PerfectDropGrade");
            if (shader != null)
                _material = new Material(shader) { hideFlags = HideFlags.HideAndDontSave };
            var bloomShader=Resources.Load<Shader>("PerfectDropBloom");
            if(bloomShader!=null && bloomShader.isSupported)_bloom=new Material(bloomShader){hideFlags=HideFlags.HideAndDontSave};
        }

        private void OnDisable()
        {
            if (_material != null)
                Destroy(_material);
            _material = null;
            if(_bloom!=null)Destroy(_bloom); _bloom=null;
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
            if(_bloom==null) { _material.SetTexture("_BloomTex",Texture2D.blackTexture); Graphics.Blit(source,destination,_material); return; }
            var width=Mathf.Max(1,source.width/4);var height=Mathf.Max(1,source.height/4);
            var format=SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf)?RenderTextureFormat.ARGBHalf:RenderTextureFormat.Default;
            var first=RenderTexture.GetTemporary(width,height,0,format);var second=RenderTexture.GetTemporary(width,height,0,format);
            try
            {
                first.filterMode=second.filterMode=FilterMode.Bilinear;
                Graphics.Blit(source,first,_bloom,0);
                _bloom.SetVector("_Direction",new Vector4(1,0,0,0)); Graphics.Blit(first,second,_bloom,1);
                _bloom.SetVector("_Direction",new Vector4(0,1,0,0)); Graphics.Blit(second,first,_bloom,1);
                _material.SetTexture("_BloomTex",first); Graphics.Blit(source,destination,_material);
            }
            finally
            {
                _material.SetTexture("_BloomTex",Texture2D.blackTexture);
                RenderTexture.ReleaseTemporary(first);RenderTexture.ReleaseTemporary(second);
            }
        }
    }
}
