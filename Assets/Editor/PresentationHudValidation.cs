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
                foreach(var points in new[]{new Vector2(375,627),new Vector2(430,850),new Vector2(627,355),new Vector2(850,394),new Vector2(190,627),new Vector2(250,627),new Vector2(299,627),new Vector2(300,627),new Vector2(627,190),new Vector2(850,190),new Vector2(627,333),new Vector2(627,349),new Vector2(627,226)})
                foreach(float pixelDensity in new[]{3f,1f})
                {
                    var root=new GameObject("PresentationHudProbe");Canvas canvas=null;
                    try
                    {
                        var game=root.AddComponent<StackGame>();
                        var profile=new StackProfile{Coins=357,UnlockedLevel=21,TutorialComplete=true};
                        Field(game,"<Profile>k__BackingField",profile);Field(game,"<Run>k__BackingField",new StackRun());Field(game,"<Level>k__BackingField",StackCampaign.Level(1));
                        var hud=root.AddComponent<StackHud>();Field(game,"<Hud>k__BackingField",hud);hud.Build(game);hud.ShowHome();
                        var safe=(RectTransform)typeof(StackHud).GetField("_safe",BindingFlags.Instance|BindingFlags.NonPublic).GetValue(hud);canvas=safe.GetComponentInParent<Canvas>();
                        canvas.GetComponent<CanvasScaler>().enabled=false;canvas.scaleFactor=pixelDensity==3?3:.375f;UiMetrics.QaPointScale=pixelDensity;safe.GetComponent<SafeAreaFitter>().enabled=false;
                        safe.anchorMin=safe.anchorMax=new Vector2(.5f,.5f);safe.sizeDelta=points*(pixelDensity/canvas.scaleFactor);
                        // Also test a fractional desktop Canvas scale: logical text rows must
                        // be converted to Canvas units, just like touch target dimensions.
                        typeof(StackHud).GetMethod("LayoutCampaign",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(hud,new object[]{new Rect(0,0,1,1)});
                        typeof(StackHud).GetMethod("LayoutArcade",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(hud,new object[]{new Rect(0,0,1,1)});
                        Canvas.ForceUpdateCanvases();
                        foreach(var button in canvas.GetComponentsInChildren<Button>())
                        {
                            if(!button.isActiveAndEnabled)continue;
                            var rect=UiMetrics.ScreenRect((RectTransform)button.transform);
                            Check(rect.width/pixelDensity>=48-.01f && rect.height/pixelDensity>=48-.01f,"Compact primary target too small "+points+" "+button.name+" "+rect);checks++;
                        }
                        foreach(var graphic in canvas.GetComponentsInChildren<Graphic>(true)){Check(graphic.GetComponent<CanvasRenderer>()!=null,"Missing graphic renderer "+graphic.name);checks++;}
                        foreach(var button in canvas.GetComponentsInChildren<Button>(true))
                        {
                            if(!button.name.StartsWith("CityDistrict") && button.name!="CityBack")continue;
                            var rect=UiMetrics.ScreenRect((RectTransform)button.transform);
                            Check(rect.width/pixelDensity>=48-.01f && rect.height/pixelDensity>=48-.01f,"City target too small "+points+" "+button.name+" "+rect);checks++;
                        }
                        foreach(var specimen in canvas.GetComponentsInChildren<TowerPreviewGraphic>(true))
                        {
                            using(var vertices=new VertexHelper())
                            {
                                typeof(TowerPreviewGraphic).GetMethod("OnPopulateMesh",BindingFlags.Instance|BindingFlags.NonPublic,null,new[]{typeof(VertexHelper)},null).Invoke(specimen,new object[]{vertices});
                                var vertex=new UIVertex();
                                for(var v=0;v<vertices.currentVertCount;v++)
                                {
                                    vertices.PopulateUIVertex(ref vertex,v);
                                    Check(specimen.rectTransform.rect.Contains(vertex.position),"Specimen leaves card bounds: "+specimen.name);checks++;
                                }
                            }
                        }
                        hud.HideMenus();
                        typeof(StackHud).GetMethod("LayoutGameplayHeader",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(hud,new object[]{new Rect(0,0,1,1),points.x>points.y*1.2f});
                        Canvas.ForceUpdateCanvases();
                        foreach(var button in canvas.GetComponentsInChildren<Button>())
                        {
                            if(!button.isActiveAndEnabled||!(button.name=="Menu"||button.name=="Drop"||button.name=="Risk"||button.name.StartsWith("Power")))continue;
                            var rect=UiMetrics.ScreenRect((RectTransform)button.transform);
                            Check(rect.width/pixelDensity>=48-.01f&&rect.height/pixelDensity>=48-.01f,"Compact gameplay target too small "+points+" "+button.name+" "+rect);checks++;
                            var safeBounds=UiMetrics.ScreenRect(safe);Check(rect.xMin>=safeBounds.xMin-.5f&&rect.xMax<=safeBounds.xMax+.5f&&rect.yMin>=safeBounds.yMin-.5f&&rect.yMax<=safeBounds.yMax+.5f,"Gameplay target outside safe pane "+points+" "+button.name);checks++;
                            foreach(var other in canvas.GetComponentsInChildren<Button>()){if(other==button||!other.isActiveAndEnabled||!(other.name=="Menu"||other.name=="Drop"||other.name=="Risk"||other.name.StartsWith("Power")))continue;var b=UiMetrics.ScreenRect((RectTransform)other.transform);Check(Mathf.Min(rect.xMax,b.xMax)-Mathf.Max(rect.xMin,b.xMin)<=.5f||Mathf.Min(rect.yMax,b.yMax)-Mathf.Max(rect.yMin,b.yMin)<=.5f,"Gameplay targets overlap "+points+" "+button.name+" / "+other.name);checks++;}
                        }
                        foreach(string sentence in new[]{"PERFEKT!","LEVEL30","Fläche bleibt erhalten · Serienbonus","WIND: ACHTE AUF DIE BEWEGUNG","Area preserved · streak bonus","WIND: WATCH THE MOVEMENT"})
                        {
                            bool feedback=sentence=="PERFEKT!"||sentence=="LEVEL30";
                            var label=safe.Find(feedback?"Objective/Status":"Objective/Hint").GetComponent<Text>();if(!label.gameObject.activeInHierarchy)continue;
                            label.text=sentence;var generator=new TextGenerator();generator.Populate(label.text,label.GetGenerationSettings(label.rectTransform.rect.size));
                            Check(generator.characterCountVisible>=label.text.Length,"Actual gameplay feedback clipped "+points+" "+sentence+" visible="+generator.characterCountVisible+" rect="+label.rectTransform.rect.size);checks++;
                            if(points.x==190 && sentence=="Fläche bleibt erhalten · Serienbonus")
                                Debug.Log("[PerfectDrop] Narrow gameplay hint complete: density="+pixelDensity+" font="+label.font.name+" visible="+generator.characterCountVisible+" required="+label.text.Length+" rect="+label.rectTransform.rect.size+" logicalFont="+generator.fontSizeUsedForBestFit/pixelDensity);
                        }
                        if(points.x==190)
                        {
                            // Recreate the actual pre-fix 58%-width objective. The same
                            // font, text generator and strict assertion must detect it.
                            var objective=(RectTransform)safe.Find("Objective");
                            var hint=objective.Find("Hint").GetComponent<Text>();
                            var min=objective.anchorMin;var max=objective.anchorMax;var text=hint.text;
                            try
                            {
                                objective.anchorMin=new Vector2(.37f,min.y);objective.anchorMax=new Vector2(.95f,max.y);
                                hint.text="Fläche bleibt erhalten · Serienbonus";Canvas.ForceUpdateCanvases();
                                var clipped=new TextGenerator();clipped.Populate(hint.text,hint.GetGenerationSettings(hint.rectTransform.rect.size));
                                Check(clipped.characterCountVisible<hint.text.Length,"Negative control failed to detect the original narrow hint clipping");checks++;
                                Debug.Log("[PerfectDrop] Narrow hint negative control detected: density="+pixelDensity+" visible="+clipped.characterCountVisible+" required="+hint.text.Length+" rect="+hint.rectTransform.rect.size);
                            }
                            finally{objective.anchorMin=min;objective.anchorMax=max;hint.text=text;Canvas.ForceUpdateCanvases();}
                        }
                        hud.ShowHome();
                        // A locked card must stay legible over the sunlit cloud background.
                        // Unity's default disabled tint halves alpha as well as brightness.
                        foreach(var button in canvas.GetComponentsInChildren<Button>())
                        {
                            var colors=button.colors;colors.fadeDuration=0;button.colors=colors;
                            button.interactable=false;
                            var image=button.targetGraphic as Image;
                            if(image==null || image.color.a<.9f)continue;
                            Check(image.color.a*image.canvasRenderer.GetColor().a>=.9f,"Disabled card becomes transparent: "+button.name);checks++;
                        }
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
