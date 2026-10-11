using System.Collections.Generic;using UnityEngine;
namespace Kamilunavo.PerfectDrop.Visuals {
 public static class TerraceArtGeometry {
  public const float NearLanternOffset=2.65f,NearLanternBaseY=-1.2f,WorldOffsetY=-18f;
  // A rounded solid perimeter, shared by every live slab and narrow cut.
  public static Mesh RoundedInset()=>RoundedInsetGeometry().Mesh("PerfectDropRoundedInset");
  public static Buffer RoundedInsetGeometry(){var buffer=new Buffer();const int steps=32;for(int i=0;i<steps;i++){float a=i*Mathf.PI*2/steps,b=(i+1)*Mathf.PI*2/steps;Vector3 A=Rounded(a,.425f),B=Rounded(b,.425f),C=Rounded(b,.412f),D=Rounded(a,.412f);buffer.Quad(A+Vector3.up*.5f,B+Vector3.up*.5f,C+Vector3.up*.5f,D+Vector3.up*.5f,Vector3.up);buffer.Quad(D-Vector3.up*.5f,C-Vector3.up*.5f,B-Vector3.up*.5f,A-Vector3.up*.5f,Vector3.down);buffer.Quad(A-Vector3.up*.5f,B-Vector3.up*.5f,B+Vector3.up*.5f,A+Vector3.up*.5f,new Vector3(A.x,0,A.z));buffer.Quad(C-Vector3.up*.5f,D-Vector3.up*.5f,D+Vector3.up*.5f,C+Vector3.up*.5f,new Vector3(-C.x,0,-C.z));}return buffer;}
  static Vector3 Rounded(float angle,float extent){float c=Mathf.Cos(angle),s=Mathf.Sin(angle);return new Vector3(Mathf.Sign(c)*Mathf.Pow(Mathf.Abs(c),.24f)*extent,0,Mathf.Sign(s)*Mathf.Pow(Mathf.Abs(s),.24f)*extent);}
  public static void Build(Transform dais){var owner=dais.gameObject.AddComponent<TerraceArtOwner>();
   Material stone=Material(owner,"TerraceIvoryStone",new Color(.76f,.68f,.54f),.06f,.34f),brass=Material(owner,"TerraceAgedBrass",new Color(.70f,.43f,.16f),.68f,.64f),leaf=Material(owner,"TerraceCypress",new Color(.10f,.24f,.115f),.0f,.18f),dark=Material(owner,"TerraceArchShadow",new Color(.075f,.11f,.17f),.15f,.43f);
   var foundation=new Buffer();foundation.Lathe(new Vector3(0,0,0),new[]{-3.35f,-3.14f,-2.98f,-1.38f,-1.13f,-1.08f},new[]{3.35f,4.12f,4.23f,4.09f,4.20f,4.20f},48);foundation.Upload(dais,"TerraceFoundation",stone,owner);
   var foreLeaf=new Buffer();var paving=new Buffer();paving.Lathe(Vector3.zero,new[]{-3.47f,-3.32f,-3.27f},new[]{4.70f,4.70f,4.62f},48);
   // A broad grounded forecourt and masonry steps give the dais structural scale.
   for(int step=0;step<5;step++)paving.Box(new Vector3(0,-1.30f-step*.42f,-3.15f-step*.41f),new Vector3(2.7f,.26f,.84f));
   var detailing=new Buffer();detailing.Ring(Vector3.up*-1.12f,4.20f,.04f,48);detailing.Ring(Vector3.up*-2.83f,4.18f,.035f,48);detailing.Ring(Vector3.up*-3.26f,4.55f,.045f,48);
   for(int side=-1;side<=1;side+=2)for(int end=-1;end<=1;end+=2){var point=new Vector3(side*NearLanternOffset,NearLanternBaseY,end*NearLanternOffset);Lantern(paving,detailing,point);Cypress(foreLeaf,point+new Vector3(side*.25f,0,0));}
   for(int side=-1;side<=1;side+=2)
   {
    Marker(dais,"TerraceBalustrade");
    Vector3 RailPoint(int post){float z=-3.15f+post*.7875f;return new Vector3(side*Mathf.Sqrt(4.45f*4.45f-z*z),-3.2f,z);}
    for(int post=0;post<9;post++)paving.Lathe(RailPoint(post),new[]{0f,.15f,.24f,.54f,.77f,.88f},new[]{.14f,.14f,.08f,.13f,.07f,.13f},8);
    for(int segment=0;segment<8;segment++){var a=RailPoint(segment);var b=RailPoint(segment+1);var cap=new Buffer();cap.Box(Vector3.zero,new Vector3(.30f,.18f,Vector3.Distance(a,b)+.04f));var direction=b-a;paving.Append(cap,(a+b)*.5f+Vector3.up*.95f,Mathf.Atan2(direction.x,direction.z)*Mathf.Rad2Deg);}
   }

   // Low sculpted planters and radial molding frame the foreground without raising roofs.
   for(int side=-1;side<=1;side+=2)for(int end=-1;end<=1;end+=2){var at=new Vector3(side*3.5f,-1.2f,end*1.6f);paving.Lathe(at,new[]{0f,.12f,.25f,.72f,.82f},new[]{.38f,.38f,.28f,.53f,.55f},10);FoliagePlanes(foreLeaf,at+Vector3.up*.82f,1.35f,1.30f);}
   for(int edge=0;edge<3;edge++){float y=-3.25f-edge*.12f;detailing.Ring(Vector3.up*y,4.49f+edge*.06f,.035f,64);}
   var foliageMaterial=new Material(Resources.Load<Shader>("PerfectDropFoliage")){name="TerraceOriginalCypressCutout",color=Color.white};foliageMaterial.mainTexture=Resources.Load<Texture2D>("Art/AuthoredCypressCutout");owner.Own(foliageMaterial);
   paving.Upload(dais,"TerraceForecourt",stone,owner);foreLeaf.Upload(dais,"TerraceForecourtGarden",foliageMaterial,owner);detailing.Upload(dais,"TerracePedestalBrass",brass,owner);
   var world=new GameObject("TerraceWorld").transform;world.SetParent(dais,false);world.localPosition=Vector3.up*WorldOffsetY;var themes=new Transform[3];
   for(int chapter=0;chapter<3;chapter++){var theme=new GameObject("Theme"+chapter).transform;theme.SetParent(world,false);themes[chapter]=theme;
    var masonry=Material(owner,"TerraceThemeStone"+chapter,chapter==0?new Color(.80f,.75f,.65f):chapter==1?new Color(.71f,.52f,.32f):new Color(.19f,.26f,.36f),.08f,.36f);
    var dome=Material(owner,"TerraceThemeRoof"+chapter,chapter==0?new Color(.14f,.25f,.34f):chapter==1?new Color(.24f,.28f,.32f):new Color(.055f,.20f,.31f),.56f,.65f);
    var water=Material(owner,"TerraceCanalWater"+chapter,chapter==2?new Color(.06f,.23f,.38f):new Color(.16f,.42f,.44f),.22f,.86f);
    var lamps=Material(owner,"TerraceWindowLight"+chapter,chapter==2?new Color(.31f,.72f,.91f):new Color(.96f,.66f,.25f),.08f,.44f);lamps.SetColor("_EmissionColor",lamps.color*(chapter==2?.62f:.28f));
    var stoneBuffer=new Buffer();var roofBuffer=new Buffer();var brassBuffer=new Buffer();var foliage=new Buffer();var lightBuffer=new Buffer();var waterBuffer=new Buffer();
    TerraceCityGeometry.Build(theme,chapter,stoneBuffer,roofBuffer,brassBuffer,foliage,lightBuffer,waterBuffer);
    DistantCityBackdrop.Build(theme,chapter,owner);
    CloudCardGeometry.Build(theme,chapter,owner);
    stoneBuffer.Upload(theme,"TerraceMasonry"+chapter,masonry,owner);roofBuffer.Upload(theme,"TerraceDomes"+chapter,dome,owner);brassBuffer.Upload(theme,"TerraceArchitecturalBrass"+chapter,brass,owner);foliage.Upload(theme,"TerraceGardens"+chapter,leaf,owner);lightBuffer.Upload(theme,"TerraceWindowBays"+chapter,lamps,owner);waterBuffer.Upload(theme,"TerraceCanal"+chapter,water,owner);
   }
   owner.Bind(themes);
  }
  static Material Material(TerraceArtOwner owner,string name,Color color,float metallic,float gloss){var shader=Resources.Load<Shader>("PerfectDropSurface");var material=new Material(shader){name=name,color=color,enableInstancing=true};material.SetFloat("_Metallic",metallic);material.SetFloat("_Glossiness",gloss);if(name.Contains("Stone")){material.SetFloat("_StoneFinish",1);material.SetTexture("_StoneAlbedo",Resources.Load<Texture2D>("Art/AuthoredIvoryLimestone"));}if(name.Contains("Water"))material.SetFloat("_WaterFinish",1);owner.Own(material);return material;}
  static void Marker(Transform parent,string name){new GameObject(name).transform.SetParent(parent,false);}
  static void Lantern(Buffer stone,Buffer gold,Vector3 at){stone.Box(at+Vector3.up*.5f,new Vector3(.63f,1,.63f));gold.Box(at+Vector3.up*1.70f,new Vector3(.06f,1.4f,.06f));gold.Box(at+Vector3.up*2.00f,new Vector3(.45f,.08f,.45f));gold.Box(at+Vector3.up*2.48f,new Vector3(.45f,.08f,.45f));for(int x=-1;x<=1;x+=2)for(int z=-1;z<=1;z+=2)gold.Box(at+new Vector3(x*.20f,2.24f,z*.20f),new Vector3(.055f,.48f,.055f));gold.Lathe(at,new[]{2.55f,2.82f},new[]{.39f,.04f},8);}
  static void Cypress(Buffer b,Vector3 p)
  {
   FoliagePlanes(b,p,1.65f,3.8f);
  }
  static void FoliagePlanes(Buffer b,Vector3 p,float width,float height)
  {
   // Three intersecting leaf cards preserve the botanical silhouette during
   // camera orbit. One owned alpha-tested batch replaces opaque foliage blobs.
   for(int plane=0;plane<3;plane++)
   {
    float angle=plane*Mathf.PI/3;var side=new Vector3(Mathf.Cos(angle),0,Mathf.Sin(angle))*width*.5f;
    b.Quad(p-side,p+side,p+side+Vector3.up*height,p-side+Vector3.up*height,Vector3.Cross(side,Vector3.up));
   }
  }

  // Buffers assemble authored curved structural surfaces directly; one output
  // renderer per material replaces hundreds of independently drawn ornaments.
  public sealed class Buffer {
   readonly List<Vector3> vertices=new();readonly List<int> triangles=new();readonly List<Vector2> uv=new();
   public int VertexCount=>vertices.Count;
   // Bake each authored local facade into its chapter's six material batches.
   public void Append(Buffer source,Vector3 position,float yaw){int first=vertices.Count;var rotation=Quaternion.Euler(0,yaw,0);foreach(var vertex in source.vertices)vertices.Add(position+rotation*vertex);uv.AddRange(source.uv);foreach(var index in source.triangles)triangles.Add(first+index);}
   public void SurfaceGrid(Vector3[] points,int columns,int rows){int first=vertices.Count;vertices.AddRange(points);for(int row=0;row<rows;row++)for(int col=0;col<columns;col++)uv.Add(new Vector2((float)col/(columns-1),(float)row/(rows-1)));for(int row=0;row<rows-1;row++)for(int col=0;col<columns-1;col++){int a=first+row*columns+col;triangles.AddRange(new[]{a,a+1,a+columns,a+1,a+columns+1,a+columns});}}
   public void Triangle(Vector3 a,Vector3 b,Vector3 c,Vector3 outward){int first=vertices.Count;vertices.AddRange(new[]{a,b,c});uv.AddRange(new[]{Vector2.zero,Vector2.right,Vector2.up});if(Vector3.Dot(Vector3.Cross(b-a,c-a),outward)>=0)triangles.AddRange(new[]{first,first+1,first+2});else triangles.AddRange(new[]{first,first+2,first+1});}
   public void Quad(Vector3 a,Vector3 b,Vector3 c,Vector3 d,Vector3 outward){int first=vertices.Count;vertices.AddRange(new[]{a,b,c,d});uv.AddRange(new[]{Vector2.zero,Vector2.right,Vector2.one,Vector2.up});if(Vector3.Dot(Vector3.Cross(b-a,c-a),outward)>=0)triangles.AddRange(new[]{first,first+1,first+2,first,first+2,first+3});else triangles.AddRange(new[]{first,first+2,first+1,first,first+3,first+2});}
   public void OpenBeam(Vector3 at,Vector3 size){var h=size*.5f;Vector3 P(float x,float y,float z)=>at+Vector3.Scale(h,new Vector3(x,y,z));int axis=size.x>size.y&&size.x>size.z?0:size.z>size.y?2:1;if(axis!=2){Quad(P(-1,-1,1),P(1,-1,1),P(1,1,1),P(-1,1,1),Vector3.forward);Quad(P(1,-1,-1),P(-1,-1,-1),P(-1,1,-1),P(1,1,-1),Vector3.back);}if(axis!=0){Quad(P(1,-1,1),P(1,-1,-1),P(1,1,-1),P(1,1,1),Vector3.right);Quad(P(-1,-1,-1),P(-1,-1,1),P(-1,1,1),P(-1,1,-1),Vector3.left);}if(axis!=1){Quad(P(-1,1,1),P(1,1,1),P(1,1,-1),P(-1,1,-1),Vector3.up);Quad(P(-1,-1,-1),P(1,-1,-1),P(1,-1,1),P(-1,-1,1),Vector3.down);}}

   public void SolidAbove(Vector3 at,Vector3 size)=>Box(at,size,false);
   public void Box(Vector3 at,Vector3 size,bool bottom=true){var h=size*.5f;Vector3 P(float x,float y,float z)=>at+Vector3.Scale(h,new Vector3(x,y,z));Quad(P(-1,-1,1),P(1,-1,1),P(1,1,1),P(-1,1,1),Vector3.forward);Quad(P(1,-1,-1),P(-1,-1,-1),P(-1,1,-1),P(1,1,-1),Vector3.back);Quad(P(1,-1,1),P(1,-1,-1),P(1,1,-1),P(1,1,1),Vector3.right);Quad(P(-1,-1,-1),P(-1,-1,1),P(-1,1,1),P(-1,1,-1),Vector3.left);Quad(P(-1,1,1),P(1,1,1),P(1,1,-1),P(-1,1,-1),Vector3.up);if(bottom)Quad(P(-1,-1,-1),P(1,-1,-1),P(1,-1,1),P(-1,-1,1),Vector3.down);}
   public void Ring(Vector3 at,float radius,float width,int sides){for(int i=0;i<sides;i++){float a=i*Mathf.PI*2/sides,b=(i+1)*Mathf.PI*2/sides;Vector3 R(float angle,float r)=>at+new Vector3(Mathf.Cos(angle)*r,0,Mathf.Sin(angle)*r);Quad(R(a,radius),R(b,radius),R(b,radius-width),R(a,radius-width),Vector3.up);}}
   public void Lathe(Vector3 at,float[] heights,float[] radii,int sides){int first=vertices.Count;for(int ring=0;ring<heights.Length;ring++)for(int side=0;side<=sides;side++){float a=side*Mathf.PI*2/sides;vertices.Add(at+new Vector3(Mathf.Cos(a)*radii[ring],heights[ring],Mathf.Sin(a)*radii[ring]));uv.Add(new Vector2((float)side/sides,(float)ring/(heights.Length-1)));}for(int ring=0;ring<heights.Length-1;ring++)for(int side=0;side<sides;side++){int a=first+ring*(sides+1)+side,b=a+sides+1;triangles.AddRange(new[]{a,b,a+1,a+1,b,b+1});}foreach(int end in new[]{0,heights.Length-1}){int center=vertices.Count;vertices.Add(at+Vector3.up*heights[end]);uv.Add(new Vector2(.5f,.5f));int start=first+end*(sides+1);for(int side=0;side<sides;side++)triangles.AddRange(end==0?new[]{start+side,start+side+1,center}:new[]{start+side,center,start+side+1});}}
   public void Ellipsoid(Vector3 at,Vector3 size,int sides,int rings){var y=new float[rings+1];var r=new float[rings+1];for(int i=0;i<=rings;i++){float a=-Mathf.PI*.5f+i*Mathf.PI/rings;y[i]=Mathf.Sin(a)*size.y;r[i]=Mathf.Max(.015f,Mathf.Cos(a)*size.x);}int start=vertices.Count;Lathe(at,y,r,sides);for(int i=start;i<vertices.Count;i++){var v=vertices[i];v.z=at.z+(v.z-at.z)*size.z/size.x;vertices[i]=v;}}
   public void Dome(Vector3 at,float radius,float height,int sides){var y=new float[7];var r=new float[7];for(int ring=0;ring<7;ring++){float a=ring*Mathf.PI/12;y[ring]=Mathf.Sin(a)*height;r[ring]=Mathf.Max(.02f,Mathf.Cos(a)*radius);}Lathe(at,y,r,sides);}
   public void Arch(Vector3 at,float radius,float rise,float spring,float thickness,float depth,int segments=8){for(int i=0;i<segments;i++){float a=i*Mathf.PI/segments,b=(i+1)*Mathf.PI/segments;Vector3 P(float angle,bool outer,float z)=>at+new Vector3(Mathf.Cos(angle)*(radius+(outer?thickness:0)),spring+Mathf.Sin(angle)*(rise+(outer?thickness:0)),z);float front=depth*.5f,back=-front;Quad(P(a,true,front),P(b,true,front),P(b,false,front),P(a,false,front),Vector3.forward);Quad(P(a,false,back),P(b,false,back),P(b,true,back),P(a,true,back),Vector3.back);Quad(P(a,true,back),P(b,true,back),P(b,true,front),P(a,true,front),new Vector3(Mathf.Cos((a+b)*.5f),Mathf.Sin((a+b)*.5f),0));Quad(P(a,false,front),P(b,false,front),P(b,false,back),P(a,false,back),new Vector3(-Mathf.Cos((a+b)*.5f),-Mathf.Sin((a+b)*.5f),0));}foreach(int side in new[]{-1,1})Box(at+new Vector3(side*(radius+thickness*.5f),spring*.5f,0),new Vector3(thickness,spring,depth));}
   public Mesh Mesh(string name){var mesh=new Mesh{name=name};if(vertices.Count>65535)mesh.indexFormat=UnityEngine.Rendering.IndexFormat.UInt32;mesh.SetVertices(vertices);mesh.SetTriangles(triangles,0);mesh.SetUVs(0,uv);mesh.RecalculateNormals();mesh.RecalculateBounds();return mesh;}
   public void Upload(Transform parent,string name,Material material,TerraceArtOwner owner){if(vertices.Count==0)return;var mesh=Mesh(name);var go=new GameObject(name,typeof(MeshFilter),typeof(MeshRenderer));go.transform.SetParent(parent,false);go.GetComponent<MeshFilter>().sharedMesh=mesh;go.GetComponent<MeshRenderer>().sharedMaterial=material;owner.Own(mesh);}
  }
 }
}
