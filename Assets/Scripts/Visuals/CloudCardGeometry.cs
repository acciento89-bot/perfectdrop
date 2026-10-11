using UnityEngine;
using UnityEngine.Rendering;
namespace Kamilunavo.PerfectDrop.Visuals
{
    // Original soft cloud albedo on world-anchored cards, facing each rendering
    // camera. The cards retain real parallax and occlusion during city orbit.
    public static class CloudCardGeometry
    {
        public static void Build(Transform theme,int chapter,TerraceArtOwner owner)
        {
            const int count=12;
            var vertices=new Vector3[count*4];var normals=new Vector3[vertices.Length];var uv=new Vector2[vertices.Length];var size=new Vector2[vertices.Length];var indices=new int[count*6];
            var corners=new[]{Vector2.zero,Vector2.right,Vector2.one,Vector2.up};
            for(int i=0;i<count;i++)
            {
                float a=i*Mathf.PI/6,r=i%2==0?14:26;
                var center=new Vector3(Mathf.Cos(a)*r,-15.5f-TerraceArtGeometry.WorldOffsetY-i%3*.8f,Mathf.Sin(a)*r);
                var dimensions=new Vector2(11+i%3*1.8f,5.5f+i%2*.7f);
                for(int corner=0;corner<4;corner++){int v=i*4+corner;uv[v]=corners[corner];size[v]=dimensions;normals[v]=Vector3.forward;vertices[v]=center+new Vector3((corners[corner].x-.5f)*dimensions.x,(corners[corner].y-.5f)*dimensions.y,0);}
                int first=i*4,t=i*6;indices[t]=first;indices[t+1]=first+1;indices[t+2]=first+2;indices[t+3]=first;indices[t+4]=first+2;indices[t+5]=first+3;
            }
            var mesh=new Mesh{name="TerraceOriginalCloudCards"+chapter};mesh.vertices=vertices;mesh.normals=normals;mesh.uv=uv;mesh.uv2=size;mesh.triangles=indices;mesh.RecalculateBounds();mesh.bounds=new Bounds(new Vector3(0,2,0),new Vector3(90,32,90));owner.Own(mesh);
            var material=new Material(Resources.Load<Shader>("PerfectDropCloudCard")){name="TerraceOriginalCloudCardMaterial"+chapter,color=chapter==2?new Color(.42f,.34f,.72f):chapter==1?new Color(1,.86f,.74f):new Color(.98f,1,1)};
            material.mainTexture=Resources.Load<Texture2D>("Art/AuthoredCloudCutout");if(material.mainTexture==null)throw new System.InvalidOperationException("Original cloud alpha asset is missing.");owner.Own(material);
            var node=new GameObject("TerraceCloudCards",typeof(MeshFilter),typeof(MeshRenderer));node.transform.SetParent(theme,false);node.GetComponent<MeshFilter>().sharedMesh=mesh;
            var renderer=node.GetComponent<MeshRenderer>();renderer.sharedMaterial=material;renderer.shadowCastingMode=ShadowCastingMode.Off;renderer.receiveShadows=false;
        }
    }
}
