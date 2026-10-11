using UnityEngine;

namespace Kamilunavo.PerfectDrop.Visuals
{
    [ExecuteAlways, RequireComponent(typeof(Camera))]
    public sealed class CinematicGrade : MonoBehaviour
    {
        [Range(0f, 1f)] public float Vignette = 0.18f;
        [Range(0f, 2f)] public float Saturation = 1.03f;
        [Range(0.5f, 2f)] public float Contrast = 1.03f;
        [Range(-1f, 1f)] public float Warmth = 0.025f;

        private Material _material;
        private Material _bloom;
        private Material _occlusion;
        private Camera _camera;
        public bool ContactShadows = true;

        private void OnEnable()
        {
            var shader = Resources.Load<Shader>("PerfectDropGrade");
            if (shader == null) shader = Shader.Find("Kamilunavo/PerfectDropGrade");
            if (shader != null)
                _material = new Material(shader) { hideFlags = HideFlags.HideAndDontSave };
            var bloomShader=Resources.Load<Shader>("PerfectDropBloom");
            if(bloomShader!=null && bloomShader.isSupported)_bloom=new Material(bloomShader){hideFlags=HideFlags.HideAndDontSave};
            _camera=GetComponent<Camera>();
            var contactShader=Resources.Load<Shader>("PerfectDropOcclusion");
            if(contactShader!=null && contactShader.isSupported)
            {
                _occlusion=new Material(contactShader){hideFlags=HideFlags.HideAndDontSave};
                _camera.depthTextureMode|=DepthTextureMode.DepthNormals;
            }
        }

        private void OnDisable()
        {
            if (_material != null)
                Dispose(_material);
            _material = null;
            if(_bloom!=null)Dispose(_bloom); _bloom=null;
            if(_occlusion!=null)Dispose(_occlusion); _occlusion=null;
        }
        private static void Dispose(Object material){if(Application.isPlaying)Destroy(material);else DestroyImmediate(material);}

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
            RenderTexture contact=null,filtered=null;
            try
            {
                if(ContactShadows && _occlusion!=null)
                {
                    var format=SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.R8)?RenderTextureFormat.R8:RenderTextureFormat.ARGB32;
                    contact=RenderTexture.GetTemporary(Mathf.Max(1,source.width/2),Mathf.Max(1,source.height/2),0,format);
                    filtered=RenderTexture.GetTemporary(contact.width,contact.height,0,format);
                    contact.filterMode=filtered.filterMode=FilterMode.Bilinear;
                    _occlusion.SetMatrix("_InverseProjection",_camera.projectionMatrix.inverse);
                    _occlusion.SetFloat("_FarPlane",_camera.farClipPlane);
                    _occlusion.SetFloat("_ProjectionScale",Mathf.Abs(_camera.projectionMatrix.m11));
                    _occlusion.SetFloat("_Aspect",_camera.aspect);
                    _occlusion.SetFloat("_Orthographic",_camera.orthographic?1:0);
                    Graphics.Blit(source,contact,_occlusion,0);
                    Graphics.Blit(contact,filtered,_occlusion,1);
                    _material.SetTexture("_OcclusionTex",filtered);
                }
                else _material.SetTexture("_OcclusionTex",Texture2D.whiteTexture);
                RenderBloom(source,destination);
            }
            finally
            {
                _material.SetTexture("_OcclusionTex",Texture2D.whiteTexture);
                if(contact!=null)RenderTexture.ReleaseTemporary(contact);
                if(filtered!=null)RenderTexture.ReleaseTemporary(filtered);
            }
        }

        private void RenderBloom(RenderTexture source,RenderTexture destination)
        {
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
