using UnityEngine;
using UnityEngine.Rendering;

namespace Kamilunavo.PerfectDrop.Visuals
{
    // A world-space cyclorama carries original distant-city matte art. Near
    // arcades, terrace, slab silhouettes and all gameplay stay real geometry.
    // This is an environment asset, never a replacement gameplay framebuffer.
    public static class DistantCityBackdrop
    {
        public static void Build(Transform theme, int chapter, TerraceArtOwner owner)
        {
            var stem=chapter==0?"Day":chapter==1?"Sunset":"Night";
            var texture=Resources.Load<Texture2D>("Art/AuthoredDistantCity"+stem);
            var shader=Resources.Load<Shader>("PerfectDropDistantCity");
            if(texture==null || shader==null)
                throw new System.InvalidOperationException("Original distant city asset missing: "+stem);
            const int segments=64;
            const float radius=60, bottom=-140, top=100;
            var vertices=new Vector3[segments*4+2];var uv=new Vector2[vertices.Length];
            var triangles=new int[segments*12];
            float U(float angle)
            {
                float phase=angle/Mathf.PI-35f/180f+.5f;
                return 1-Mathf.Abs(Mathf.Repeat(phase*.5f,1)*2-1);
            }
            for(int i=0;i<segments;i++)
            {
                float a=i*Mathf.PI*2/segments,b=(i+1)*Mathf.PI*2/segments;
                var p=new Vector3(Mathf.Sin(a)*radius,bottom,Mathf.Cos(a)*radius);
                var q=new Vector3(Mathf.Sin(b)*radius,bottom,Mathf.Cos(b)*radius);
                int v=i*4,t=i*6;
                vertices[v]=p;vertices[v+1]=q;vertices[v+2]=q+Vector3.up*(top-bottom);vertices[v+3]=p+Vector3.up*(top-bottom);
                uv[v]=new Vector2(U(a),0);uv[v+1]=new Vector2(U(b),0);uv[v+2]=new Vector2(U(b),1);uv[v+3]=new Vector2(U(a),1);
                // Clockwise x/z parameterization winds toward the cylinder interior.
                triangles[t]=v;triangles[t+1]=v+2;triangles[t+2]=v+1;
                triangles[t+3]=v;triangles[t+4]=v+3;triangles[t+5]=v+2;
            }
            // Close the environment above and below for steep City orbit views.
            int lower=segments*4,upper=lower+1;
            vertices[lower]=Vector3.up*bottom;vertices[upper]=Vector3.up*top;
            uv[lower]=new Vector2(.5f,0);uv[upper]=new Vector2(.5f,1);
            for(int i=0;i<segments;i++)
            {
                int v=i*4,t=segments*6+i*6;
                triangles[t]=v;triangles[t+1]=v+1;triangles[t+2]=lower;
                triangles[t+3]=v+3;triangles[t+4]=upper;triangles[t+5]=v+2;
            }
            var mesh=new Mesh{name="TerraceDistantCyclorama"+chapter};
            mesh.vertices=vertices;mesh.uv=uv;mesh.triangles=triangles;mesh.RecalculateNormals();
            // Vertices follow the active rendering camera in the shader. This
            // conservative culling bound also covers the supported endless run.
            mesh.bounds=new Bounds(Vector3.zero,Vector3.one*100000);owner.Own(mesh);
            var material=new Material(shader){name="TerraceDistantMatte"+chapter};material.mainTexture=texture;material.SetTexture("_CloudTex",Resources.Load<Texture2D>("Art/AuthoredGoldenCloudPanorama"));material.SetFloat("_Night",chapter==2?1:0);material.SetFloat("_Day",chapter==0?1:0);owner.Own(material);
            material.SetVector("_CloudOrigin",theme.position-Vector3.up*TerraceArtGeometry.WorldOffsetY);
            var node=new GameObject("DistantCityCyclorama",typeof(MeshFilter),typeof(MeshRenderer));node.transform.SetParent(theme,false);
            node.GetComponent<MeshFilter>().sharedMesh=mesh;
            var renderer=node.GetComponent<MeshRenderer>();renderer.sharedMaterial=material;renderer.shadowCastingMode=ShadowCastingMode.Off;renderer.receiveShadows=false;
        }
    }
}
