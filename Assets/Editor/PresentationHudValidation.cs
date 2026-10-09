#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using Kamilunavo.PerfectDrop.Gameplay;
using Kamilunavo.PerfectDrop.UI;
namespace Kamilunavo.PerfectDrop.Editor
{
    public static class PresentationHudValidation
    {
        public static void Validate()
        {
            var checks=0;var errors=new List<string>();Application.LogCallback handler=(text,trace,type)=>{if(type==LogType.Error || type==LogType.Exception || type==LogType.Assert)errors.Add(text);};
            Application.logMessageReceived+=handler;
            try
            {
                foreach(var points in new[]{new Vector2(375,627),new Vector2(430,850),new Vector2(627,355),new Vector2(850,394),new Vector2(190,627)})
                {
                    var root=new GameObject("PresentationHudProbe");Canvas canvas=null;
                    try
                    {
                        var game=root.AddComponent<StackGame>();
                        var profile=new StackProfile{Coins=357,UnlockedLevel=21,TutorialComplete=true};
                        Field(game,"<Profile>k__BackingField",profile);Field(game,"<Run>k__BackingField",new StackRun());Field(game,"<Level>k__BackingField",StackCampaign.Level(1));
                        var hud=root.AddComponent<StackHud>();Field(game,"<Hud>k__BackingField",hud);hud.Build(game);hud.ShowHome();
                        var safe=(RectTransform)typeof(StackHud).GetField("_safe",BindingFlags.Instance|BindingFlags.NonPublic).GetValue(hud);canvas=safe.GetComponentInParent<Canvas>();
                        canvas.GetComponent<CanvasScaler>().enabled=false;canvas.scaleFactor=3;UiMetrics.QaPointScale=3;safe.GetComponent<SafeAreaFitter>().enabled=false;
                        safe.anchorMin=safe.anchorMax=new Vector2(.5f,.5f);safe.sizeDelta=points;
                        // Canvas units become physical pixels at scale3, while points are safe rect units.
                        typeof(StackHud).GetMethod("LayoutCampaign",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(hud,new object[]{new Rect(0,0,1,1)});
                        Canvas.ForceUpdateCanvases();
                        foreach(var button in canvas.GetComponentsInChildren<Button>())
                        {
                            if(!button.isActiveAndEnabled)continue;
                            var rect=UiMetrics.ScreenRect((RectTransform)button.transform);
                            Check(rect.width/3>=48-.01f && rect.height/3>=48-.01f,"Compact primary target too small "+points+" "+button.name+" "+rect);checks++;
                        }
                        foreach(var graphic in canvas.GetComponentsInChildren<Graphic>(true)){Check(graphic.GetComponent<CanvasRenderer>()!=null,"Missing graphic renderer "+graphic.name);checks++;}
                    }
                    finally{if(canvas!=null)UnityEngine.Object.DestroyImmediate(canvas.gameObject);UnityEngine.Object.DestroyImmediate(root);UiMetrics.QaPointScale=null;}
                }
                Check(errors.Count==0,"UI logged errors: "+string.Join("; ",errors));
                Debug.Log("[PerfectDrop] Measured menu targets/renderers PASS "+checks);
            }
            finally{Application.logMessageReceived-=handler;}
        }
        private static void Field(object obj,string name,object value)=>obj.GetType().GetField(name,BindingFlags.Instance|BindingFlags.NonPublic).SetValue(obj,value);
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
    }
}
#endif
