#!/usr/bin/env python3
"""Execute the production skin repaint/selection bodies against small renderer test doubles.
This checks material-property routing, not GPU rendering; Unity QA remains required.
"""
from pathlib import Path
import subprocess,tempfile
root=Path(__file__).resolve().parents[1]
world=(root/'Assets/Scripts/Visuals/WorldArt.cs').read_text()
game=(root/'Assets/Scripts/Gameplay/StackGame.cs').read_text()
campaign=(root/'Assets/Scripts/Gameplay/StackCampaign.cs').read_text()
def method(source,signature):
 start=source.index(signature); brace=source.index('{',start);depth=1;end=brace+1
 while depth:
  depth+=(source[end]=='{')-(source[end]=='}');end+=1
 return source[start:end]
refresh=method(game,'public void RefreshProfileStyle(')
style=method(world,'public static void StyleStackBlock(')
# The old implementation owns a private color switch; the fixed implementation uses the shared palette.
old_color=world[world.index('private static Color StyleColor('):world.index('public static void StyleStackBlock(')] if 'private static Color StyleColor(' in world else ''
selection=method(campaign,'public static bool SelectStyle(')
cost=campaign[campaign.index('public static int StyleCost('):campaign.index('public static bool SelectStyle(')]
stubs='''using System;using System.Collections.Generic;using UnityEngine;using Kamilunavo.PerfectDrop;using Kamilunavo.PerfectDrop.Visuals;using Monetization=Kamilunavo.PerfectDrop.Monetization;
namespace UnityEngine {
 public struct Color {public float r,g,b,a;public Color(float r,float g,float b,float a=1){this.r=r;this.g=g;this.b=b;this.a=a;}public static Color white=>new Color(1,1,1);public static Color black=>new Color(0,0,0);public static Color operator*(Color c,float k)=>new Color(c.r*k,c.g*k,c.b*k,c.a*k);public static Color Lerp(Color a,Color b,float t)=>new Color(a.r+(b.r-a.r)*t,a.g+(b.g-a.g)*t,a.b+(b.b-a.b)*t,a.a+(b.a-a.a)*t);}
 public static class Mathf {public static int Clamp(int v,int lo,int hi)=>Math.Max(lo,Math.Min(hi,v));}
 public class Material {public string name;public Color color;public float GetFloat(string s)=>s=="_Metallic"?.48f:.46f;public Color GetColor(string s)=>color;public bool HasProperty(string s)=>true;}
 public class MaterialPropertyBlock {public Dictionary<string,Color> C=new();public Dictionary<string,float> F=new();public void SetColor(string n,Color c)=>C[n]=c;public Color GetColor(string n)=>C.TryGetValue(n,out var c)?c:default;public void SetFloat(string n,float f)=>F[n]=f;public float GetFloat(string n)=>F.TryGetValue(n,out var f)?f:0;public void Clear(){C.Clear();F.Clear();}}
 public class Renderer {public string name;public Material sharedMaterial;public MaterialPropertyBlock P=new();public void GetPropertyBlock(MaterialPropertyBlock p){foreach(var c in P.C)p.C[c.Key]=c.Value;foreach(var f in P.F)p.F[f.Key]=f.Value;}public void SetPropertyBlock(MaterialPropertyBlock p){P=new();GetCopy(p,P);}static void GetCopy(MaterialPropertyBlock from,MaterialPropertyBlock to){foreach(var c in from.C)to.C[c.Key]=c.Value;foreach(var f in from.F)to.F[f.Key]=f.Value;}}
 public class Transform {public GameObject gameObject;}
 public class GameObject {public Renderer[] Parts;public T[] GetComponentsInChildren<T>()=>Parts as T[];}
}
namespace Kamilunavo.PerfectDrop.Monetization {public static class CommerceRules {public static void Normalize(Kamilunavo.PerfectDrop.StackProfile p){}}}
namespace Kamilunavo.PerfectDrop.Visuals {}
namespace Kamilunavo.PerfectDrop {public class StackProfile {public int Coins=1000,OwnedStyles=255,Style;}}
'''
main='''public static class Test {static string RGB(Color c)=>$"{c.r:F3}/{c.g:F3}/{c.b:F3}";static Color Effective(Renderer r)=>r.P.C.TryGetValue("_Color",out var c)?c:r.sharedMaterial.color;
 public static int Main(){var parts=new List<Renderer>();foreach(var n in new[]{"MetalDeck","TopPlate","GoldBand","GoldInset","DeckCrown","Undercore"}){var r=new Renderer{name=n,sharedMaterial=new Material{name=n,color=n=="TopPlate"?new Color(1,.8f,.35f):new Color(.3f,.35f,.44f)}};r.P.SetColor("_DeckBaseColor",new Color(.3f,.35f,.44f));parts.Add(r);}var block=new GameObject{Parts=parts.ToArray()};var profile=new Kamilunavo.PerfectDrop.StackProfile();var dominant=new HashSet<string>();int wallet=profile.Coins;
 for(int i=0;i<8;i++){if(!StackCampaign.SelectStyle(profile,i)||profile.Style!=i)throw new Exception("production selection failed");WorldArt.StyleStackBlock(block,profile.Style);string signature=RGB(Effective(parts[0]))+":"+RGB(Effective(parts[1]));Console.WriteLine("style "+i+" body/plate "+signature);if(!dominant.Add(signature))throw new Exception("STYLE_RENDER_FAIL style "+i+" keeps the same dominant body/plate as another design");if(i>0&&!parts[1].P.C.ContainsKey("_Color"))throw new Exception("STYLE_RENDER_FAIL selected style never reaches TopPlate shader property");if(!StackCampaign.SelectStyle(profile,i)||profile.Coins!=wallet)throw new Exception("repeat selection recharged wallet");}
 var pedestal=new GameObject{Parts=new[]{new Renderer{name="TopPlate",sharedMaterial=new Material{color=new Color(1,.8f,.35f)}}}};var owner=new StackGame{_pedestal=pedestal};owner.Profile.Style=2;WorldArt.StyleStackBlock(block,2);owner.RefreshProfileStyle();if(RGB(Effective(pedestal.Parts[0]))!=RGB(Effective(parts[1])))throw new Exception("STYLE_RENDER_FAIL actual RefreshProfileStyle leaves owned pedestal in previous color");Console.WriteLine("STYLE_RENDER_PATH_PASS 8 distinct dominant finishes + production selection/reselect + owned pedestal refresh");return 0;}}
'''
refresh_fixture='public class StackGame {public Kamilunavo.PerfectDrop.StackProfile Profile=new();public List<GameObject> _placed=new();public Transform _moving;public GameObject _pedestal;public int CurrentKind;'+refresh+"}"
source=stubs+'\npublic static class WorldArt {\n'+old_color+style+' public static void MarkSpecialBlock(GameObject block,int kind){}\n}\npublic static class StackCampaign {\n'+cost+selection+'\n}\n'+refresh_fixture+main
palette=root/'Assets/Scripts/Visuals/StackStylePalette.cs'
if palette.exists():source+=palette.read_text().replace('using UnityEngine;','')
with tempfile.TemporaryDirectory(prefix='pd-style-path-') as d:
 d=Path(d);src=d/'test.cs';src.write_text(source);exe=d/'test.exe'
 unity=Path('/Applications/Unity/Hub/Editor/6000.6.4f1/Unity.app/Contents/Resources/Scripting')
 args=[str(unity/'DotNetSdk/dotnet'),str(unity/'DotNetSdk/sdk/8.0.318/Roslyn/bincore/csc.dll'),'-nologo','-langversion:latest','-target:exe','-nostdlib+',f'-out:{exe}']
 for name in ['mscorlib','System','System.Core']:args.append('-r:'+str(unity/f'MonoBleedingEdge/lib/mono/4.5/{name}.dll'))
 subprocess.run(args+[str(src)],check=True)
 subprocess.run([str(unity/'MonoBleedingEdge/bin/mono'),str(exe)],check=True)
