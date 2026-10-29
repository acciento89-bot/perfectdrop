#if UNITY_EDITOR
using System;using System.IO;using UnityEngine;
// Editor-only actual Canvas rasterizer. It never boots commerce or reads/writes player profiles.
public static class VisualIdentityCapture
{
 public static void Save(Canvas canvas,string name,Vector2? points=null,Action prepare=null)
 {
  string output=Environment.GetEnvironmentVariable("PORTFOLIO_IDENTITY_CAPTURE");if(string.IsNullOrEmpty(output))return;
  if(SystemInfo.graphicsDeviceType==UnityEngine.Rendering.GraphicsDeviceType.Null)throw new InvalidOperationException("Actual identity capture requires a graphics device; omit -nographics.");
  var cameraObject=new GameObject("IdentityCaptureCamera",typeof(Camera));var camera=cameraObject.GetComponent<Camera>();camera.orthographic=true;camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=new Color(.07f,.10f,.14f);camera.transform.position=new Vector3(0,0,-10);
  var size=points??canvas.GetComponent<RectTransform>().rect.size;int width=Mathf.RoundToInt(size.x*canvas.scaleFactor),height=Mathf.RoundToInt(size.y*canvas.scaleFactor);width=Mathf.Clamp(width,320,2200);height=Mathf.Clamp(height,320,2600);
  var target=new RenderTexture(width,height,24);var pixels=new Texture2D(width,height,TextureFormat.RGBA32,false);var previous=RenderTexture.active;var mode=canvas.renderMode;var oldCamera=canvas.worldCamera;float plane=canvas.planeDistance;
  try{target.Create();camera.targetTexture=target;canvas.renderMode=RenderMode.ScreenSpaceCamera;canvas.worldCamera=camera;canvas.planeDistance=1;Canvas.ForceUpdateCanvases();prepare?.Invoke();Canvas.ForceUpdateCanvases();camera.Render();RenderTexture.active=target;pixels.ReadPixels(new Rect(0,0,width,height),0,0);pixels.Apply();Directory.CreateDirectory(output);File.WriteAllBytes(Path.Combine(output,name+".png"),pixels.EncodeToPNG());Debug.Log("IDENTITY_ACTUAL_EDITOR_CAPTURE "+name+" "+width+"x"+height);}
  finally{canvas.renderMode=mode;canvas.worldCamera=oldCamera;canvas.planeDistance=plane;camera.targetTexture=null;RenderTexture.active=previous;target.Release();UnityEngine.Object.DestroyImmediate(target);UnityEngine.Object.DestroyImmediate(pixels);UnityEngine.Object.DestroyImmediate(cameraObject);Canvas.ForceUpdateCanvases();}
 }
}
#endif
