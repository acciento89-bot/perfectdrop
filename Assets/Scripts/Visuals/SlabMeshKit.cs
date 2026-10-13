using System.Collections.Generic;
using UnityEngine;

namespace Kamilunavo.PerfectDrop.Visuals
{
    /// <summary>Original turned shoulder profiles, with a continuous rounded perimeter.
    /// The normalized envelope is shared by full slabs and actual overhang cuts.</summary>
    public static class SlabMeshKit
    {
        private static Mesh _body, _rim;
        public static Mesh Body => _body != null ? _body : (_body = Build("AuthoredLacquerShoulder",
            new[] {-.5f,-.44f,-.38f,.36f,.44f,.5f},
            new[] {.482f,.498f,.5f,.5f,.498f,.482f}));
        public static Mesh Rim => _rim != null ? _rim : (_rim = Build("AuthoredTurnedGoldRim",
            new[] {-.5f,-.30f,.0f,.30f,.5f},
            new[] {.465f,.495f,.5f,.495f,.465f}));

        private static Mesh Build(string name,float[] heights,float[] extents)
        {
            const int steps=8, count=4*(steps+1);
            var vertices=new List<Vector3>();var normals=new List<Vector3>();
            var uv=new List<Vector2>();var triangles=new List<int>();
            Vector2 Point(int index,float extent)
            {
                int corner=index/(steps+1), step=index%(steps+1);
                float a=(corner*90+step*90f/steps)*Mathf.Deg2Rad;
                const float radius=.055f;
                var center=new Vector2(corner==0||corner==3?extent-radius:-extent+radius,
                    corner<2?extent-radius:-extent+radius);
                return center+new Vector2(Mathf.Cos(a),Mathf.Sin(a))*radius;
            }
            Vector3 Position(int index,int ring){var p=Point(index,extents[ring]);return new Vector3(p.x,heights[ring],p.y);}
            var perimeter=new float[count+1];
            for(int i=0;i<count;i++)perimeter[i+1]=perimeter[i]+Vector2.Distance(Point(i,.5f),Point((i+1)%count,.5f));
            for(int ring=0;ring<heights.Length-1;ring++)
            {
                for(int i=0;i<count;i++)
                {
                    int next=(i+1)%count, first=vertices.Count;
                    var a=Position(i,ring);var b=Position(next,ring);
                    var c=Position(next,ring+1);var d=Position(i,ring+1);
                    // Counterclockwise in X/Z; face winding points out of the solid.
                    var normal=Vector3.Cross(d-a,b-a).normalized;
                    vertices.AddRange(new[]{a,b,c,d});normals.AddRange(new[]{normal,normal,normal,normal});
                    float u=perimeter[i]/perimeter[count],un=perimeter[i+1]/perimeter[count];
                    uv.AddRange(new[]{new Vector2(u,heights[ring]+.5f),new Vector2(un,heights[ring]+.5f),new Vector2(un,heights[ring+1]+.5f),new Vector2(u,heights[ring+1]+.5f)});
                    triangles.AddRange(new[]{first,first+2,first+1,first,first+3,first+2});
                }
            }
            foreach(int ring in new[]{0,heights.Length-1})
            {
                int first=vertices.Count;bool top=ring>0;var normal=top?Vector3.up:Vector3.down;
                vertices.Add(Vector3.up*heights[ring]);normals.Add(normal);uv.Add(new Vector2(.5f,.5f));
                for(int i=0;i<count;i++){var p=Position(i,ring);vertices.Add(p);normals.Add(normal);uv.Add(new Vector2(p.x+.5f,p.z+.5f));}
                for(int i=0;i<count;i++){int a=first+1+i,b=first+1+(i+1)%count;triangles.AddRange(top?new[]{first,b,a}:new[]{first,a,b});}
            }
            var mesh=new Mesh{name=name,hideFlags=HideFlags.HideAndDontSave};mesh.SetVertices(vertices);
            mesh.SetNormals(normals);mesh.SetUVs(0,uv);mesh.SetTriangles(triangles,0);mesh.RecalculateTangents();mesh.RecalculateBounds();return mesh;
        }
    }
}
