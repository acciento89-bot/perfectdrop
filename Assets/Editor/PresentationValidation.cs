#if UNITY_EDITOR
using System;
using UnityEngine;
using Kamilunavo.PerfectDrop.UI;
namespace Kamilunavo.PerfectDrop.Editor
{
    public static class PresentationValidation
    {
        public static void Validate()
        {
            var checks=0;
            foreach(var aspect in new[]{.461f,.5625f,1.333f,2.171f})
            {
                var landscape=aspect>1.2f;var pane=StackPresentation.World(landscape);
                Check(!pane.Overlaps(StackPresentation.Powers(landscape)),"Powers obstruct gameplay reservation.");checks++;
                Check(!pane.Overlaps(StackPresentation.Drop(landscape)),"Drop obstructs gameplay reservation.");checks++;
                foreach(var yaw in new[]{-180f,-90f,-35f,0f,90f,180f})foreach(var height in new[]{0f,2f,15.6f,33.28f})
                {
                    var outward=new Vector3(Mathf.Sin(yaw*Mathf.Deg2Rad),.78f,-Mathf.Cos(yaw*Mathf.Deg2Rad)).normalized;
                    var rotation=Quaternion.LookRotation(-outward);
                    var focus=new Vector3(0,height-1.1f,0);var bounds=new Bounds(focus,new Vector3(12,3.4f,12));
                    var distance=StackPresentation.FitDistance(bounds,focus,rotation,43,aspect,pane);
                    var inverse=Quaternion.Inverse(rotation);var tangent=Mathf.Tan(43*.5f*Mathf.Deg2Rad);
                    for(var x=-1;x<=1;x+=2)for(var y=-1;y<=1;y+=2)for(var z=-1;z<=1;z+=2)
                    {
                        var point=inverse*(bounds.center+Vector3.Scale(bounds.extents,new Vector3(x,y,z))-focus-outward*distance);
                        var projected=pane.center+new Vector2(point.x/(point.z*tangent*aspect)*.5f,point.y/(point.z*tangent)*.5f);
                        Check(point.z>0 && pane.Contains(projected),"Motion corner leaves reserved gameplay area.");checks++;
                    }
                }
            }
            foreach(var usable in new[]{new Rect(0,0,.465f,1),new Rect(.535f,0,.465f,1),new Rect(0,0,1,.465f),new Rect(0,.535f,1,.465f)})
            {
                var city=StackPresentation.ToViewport(new Rect(0,0,932,430),usable,new Rect(.04f,.20f,.92f,.48f),new Vector2(932,430));
                Check(usable.Contains(city.min) && usable.Contains(city.max),"City crosses excluded division pane.");checks++;
            }
            var original=new StackProfile { Coins=357,OwnedStyles=255,UnlockedLevel=15,TutorialComplete=true };
            Kamilunavo.PerfectDrop.Monetization.CommerceRules.RestoreEntitlement(original,Kamilunavo.PerfectDrop.Monetization.CommerceRules.Starter);
            Kamilunavo.PerfectDrop.Monetization.CommerceRules.RestoreEntitlement(original,Kamilunavo.PerfectDrop.Monetization.CommerceRules.Collection);
            var restored=StackSave.Parse(JsonUtility.ToJson(original));
            Check(restored.TutorialComplete && restored.Coins==357 && restored.OwnedStyles==255 && restored.UnlockedLevel==15,"Tutorial profile migration loses saved rights.");checks++;
            Check(!StackSave.Parse("{\"Schema\":2,\"Coins\":100}").TutorialComplete,"Old saves must receive a replayable first-use guide.");checks++;
            Debug.Log("[PerfectDrop] Presentation reservations/perspective/migration PASS "+checks);
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
    }
}
#endif
