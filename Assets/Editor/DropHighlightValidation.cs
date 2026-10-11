#if UNITY_EDITOR
using System;
using UnityEngine;
namespace Kamilunavo.PerfectDrop.Editor
{
    public static class DropHighlightValidation
    {
        public static void Validate()
        {
            var shader=Resources.Load<Shader>("PerfectDropGrade");
            if(shader==null||!shader.isSupported)throw new InvalidOperationException("Missing supported actual highlight grade shader");
            var material=new Material(shader);
            var source=new Texture2D(3,1,TextureFormat.RGBAHalf,false,true){filterMode=FilterMode.Point,wrapMode=TextureWrapMode.Clamp};
            var target=RenderTexture.GetTemporary(3,1,0,RenderTextureFormat.ARGBHalf,RenderTextureReadWrite.Linear);
            var pixels=new Texture2D(3,1,TextureFormat.RGBAFloat,false,true);var previous=RenderTexture.active;
            try
            {
                source.SetPixels(new[]{new Color(.9f,.9f,.9f),new Color(1.4f,1.4f,1.4f),new Color(2.2f,2.2f,2.2f)});source.Apply();
                material.SetFloat("_Vignette",0);material.SetFloat("_Warmth",0);material.SetFloat("_Saturation",1);material.SetFloat("_Contrast",1);
                material.SetTexture("_BloomTex",Texture2D.blackTexture);material.SetTexture("_OcclusionTex",Texture2D.whiteTexture);
                Graphics.Blit(source,target,material);RenderTexture.active=target;pixels.ReadPixels(new Rect(0,0,3,1),0,0);pixels.Apply();var actual=pixels.GetPixels();
                foreach(var color in actual)if(!float.IsFinite(color.r)||color.r<=.7f||color.r>=.99f)throw new InvalidOperationException("Actual gold-highlight grading clips a bright surface instead of retaining detail: "+color);
                if(actual[1].r-actual[0].r<.02f||actual[2].r-actual[1].r<.02f)throw new InvalidOperationException("Actual HDR highlights lose their ordered brightness differences");
                Debug.Log("DROP_HIGHLIGHT_RANGE_PASS actual="+actual[0].r+","+actual[1].r+","+actual[2].r);
            }
            finally
            {
                RenderTexture.active=previous;RenderTexture.ReleaseTemporary(target);
                UnityEngine.Object.DestroyImmediate(source);UnityEngine.Object.DestroyImmediate(pixels);UnityEngine.Object.DestroyImmediate(material);
            }
        }
    }
}
#endif
