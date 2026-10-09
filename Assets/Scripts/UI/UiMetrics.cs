using UnityEngine;
namespace Kamilunavo.PerfectDrop.UI
{
 public static class UiMetrics
 {
#if UNITY_IOS && !UNITY_EDITOR
 [System.Runtime.InteropServices.DllImport("__Internal")]private static extern float PDScreenScale();
#endif
#if UNITY_EDITOR || DEVELOPMENT_BUILD
 public static float? QaPointScale;
#endif
 public static float PointScale{get{
#if UNITY_EDITOR || DEVELOPMENT_BUILD
 if(QaPointScale.HasValue)return QaPointScale.Value;
#endif
#if UNITY_IOS && !UNITY_EDITOR
 return Mathf.Max(1,PDScreenScale());
#elif UNITY_ANDROID && !UNITY_EDITOR
 using var unity=new AndroidJavaClass("com.unity3d.player.UnityPlayer");using var activity=unity.GetStatic<AndroidJavaObject>("currentActivity");using var resources=activity.Call<AndroidJavaObject>("getResources");using var metrics=resources.Call<AndroidJavaObject>("getDisplayMetrics");return Mathf.Max(1,metrics.Get<float>("density"));
#else
 return 1;
#endif
 }}
 public static float TargetSize(Transform context,float nominal){var canvas=context.GetComponentInParent<Canvas>();return Mathf.Max(nominal,49*PointScale/Mathf.Max(.001f,canvas!=null?canvas.scaleFactor:1));}
 public static Rect ScreenRect(RectTransform rect){var c=new Vector3[4];rect.GetWorldCorners(c);Vector2 a=RectTransformUtility.WorldToScreenPoint(null,c[0]),b=RectTransformUtility.WorldToScreenPoint(null,c[2]);return Rect.MinMaxRect(a.x,a.y,b.x,b.y);}
 }
}
