using UnityEngine;
namespace Kamilunavo.PerfectDrop.UI
{
    /// <summary>Geometry reservations are independent of camera motion and stack height.</summary>
    public static class StackPresentation
    {
        public const float GameplayElevation=.62f;
        public static Rect World(bool landscape) => landscape ? new Rect(.025f,.07f,.68f,.785f) : new Rect(.04f,.255f,.92f,.615f);
        public static Rect Powers(bool landscape) => landscape ? new Rect(.75f,.40f,.225f,.28f) : new Rect(.07f,.165f,.86f,.085f);
        public static Rect Drop(bool landscape) => landscape ? new Rect(.75f,.065f,.225f,.15f) : new Rect(.10f,.045f,.80f,.10f);
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
