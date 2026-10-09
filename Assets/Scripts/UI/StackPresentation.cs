using UnityEngine;
namespace Kamilunavo.PerfectDrop.UI
{
    /// <summary>Geometry reservations are independent of camera motion and stack height.</summary>
    public static class StackPresentation
    {
        public static Rect World(bool landscape) => landscape ? new Rect(.22f,.245f,.735f,.49f) : new Rect(.045f,.32f,.91f,.41f);
        public static Rect Powers(bool landscape) => landscape ? new Rect(.025f,.055f,.165f,.695f) : new Rect(.04f,.165f,.92f,.135f);
        public static Rect Drop(bool landscape) => landscape ? new Rect(.25f,.035f,.63f,.165f) : new Rect(.10f,.045f,.80f,.10f);
        public static Rect ToViewport(Rect safe,Rect usable,Rect slot,Vector2 resolution) => new Rect((safe.x+(usable.x+slot.x*usable.width)*safe.width)/resolution.x,(safe.y+(usable.y+slot.y*usable.height)*safe.height)/resolution.y,slot.width*usable.width*safe.width/resolution.x,slot.height*usable.height*safe.height/resolution.y);
        public static float FitDistance(Bounds bounds, Vector3 focus, Quaternion rotation, float fov, float aspect, Rect pane)
        {
            var inverse=Quaternion.Inverse(rotation);
            var tangent=Mathf.Tan(fov*.5f*Mathf.Deg2Rad);
            var horizontal=tangent*Mathf.Max(.01f,aspect)*pane.width*.91f;
            var vertical=tangent*pane.height*.91f;
            var result=1f;
            for(var x=-1;x<=1;x+=2)for(var y=-1;y<=1;y+=2)for(var z=-1;z<=1;z+=2)
            {
                var offset=inverse*(bounds.center+Vector3.Scale(bounds.extents,new Vector3(x,y,z))-focus);
                result=Mathf.Max(result,Mathf.Abs(offset.x)/horizontal-offset.z,Mathf.Abs(offset.y)/vertical-offset.z,.4f-offset.z);
            }
            return result;
        }
    }
}
