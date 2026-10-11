using UnityEngine;
using Buffer = Kamilunavo.PerfectDrop.Visuals.TerraceArtGeometry.Buffer;

namespace Kamilunavo.PerfectDrop.Visuals
{
    // Original local-space city models, baked into six owned chapter meshes.
    // Heights recede outward: no nearby roof can mask a live placement.
    public static class TerraceCityGeometry
    {
        public static void Build(Transform theme, int chapter, Buffer stone, Buffer roof,
            Buffer gold, Buffer leaf, Buffer light, Buffer water)
        {
            Canal(theme, chapter, stone, gold, leaf, light, water);
            for (int ring = 0; ring < 2; ring++)
            {
                const int count = 16;
                float radius = ring == 0 ? 31 : 44;
                for (int i = 0; i < count; i++)
                {
                    float angle = (i + ring * .47f) * 360 / count;
                    float a = angle * Mathf.Deg2Rad;
                    float r = radius + Mathf.Sin(i * 2.13f) * 1.1f;
                    var at = new Vector3(Mathf.Cos(a) * r, -7.5f, Mathf.Sin(a) * r);
                    float height = ring == 0 ? 3.6f + (i % 4) * .65f : 7.8f + (i % 5) * 1.3f;
                    if(chapter == 1) height *= ring == 0 ? .94f : .90f;
                    House(theme, chapter, stone, roof, gold, light, at, 270 - angle,
                        height, (ring == 0 ? 6.8f : chapter == 1 ? 8.6f : 7.8f) * (.84f + i % 3 * .07f), i + ring * 31, ring == 0);
                }
            }
            for (int i = 0; i < 6; i++)
            {
                float angle = i * 60 + 15;
                float a = angle * Mathf.Deg2Rad;
                var at = new Vector3(Mathf.Cos(a) * 57, -7.8f, Mathf.Sin(a) * 57);
                Landmark(theme, chapter, stone, roof, gold, light, at, 270 - angle,
                    (18 + (i % 3) * 4.1f) * (chapter == 1 ? .57f : .63f), i);
            }
            // A third, deliberately inexpensive skyline band gives the city
            // depth behind the detailed arcades instead of six isolated towers.
            // All faces remain in the existing chapter batches.
            for(int i=0;i<12;i++)
            {
                Marker(theme,"DistantCityHouse");
                float angle=i*30+8, a=angle*Mathf.Deg2Rad;
                float height=12+(i%5)*2.8f, width=4.6f+(i%3)*.7f;
                var s=new Buffer();var r=new Buffer();var l=new Buffer();
                s.SolidAbove(new Vector3(0,height*.5f,0),new Vector3(width,height,width*.8f));
                s.SolidAbove(new Vector3(0,height+.15f,0),new Vector3(width+.38f,.3f,width*.8f+.38f));
                if(chapter==2)
                    r.Lathe(new Vector3(0,height+.3f,0),new[]{0f,1.1f,3.3f,4.8f},new[]{width*.5f,width*.43f,width*.22f,.035f},4);
                else
                    r.Dome(new Vector3(0,height+.3f,0),width*.50f,width*(chapter==0?.57f:.39f),8);
                for(int side=-1;side<=1;side+=2)
                {
                    var p=new Vector3(side*width*.24f,height*.71f,width*.401f);
                    l.Quad(p+new Vector3(-.23f,-.9f,0),p+new Vector3(.23f,-.9f,0),p+new Vector3(.23f,.9f,0),p+new Vector3(-.23f,.9f,0),Vector3.forward);
                }
                var at=new Vector3(Mathf.Cos(a)*74,-10.8f,Mathf.Sin(a)*74);
                stone.Append(s,at,270-angle);roof.Append(r,at,270-angle);light.Append(l,at,270-angle);
            }
            // Real amber glass inside the already-owned near terrace lanterns.
            for (int side = -1; side <= 1; side += 2)
                for (int end = -1; end <= 1; end += 2)
                    light.Box(new Vector3(side * 2.55f, -.96f, end * 2.55f),
                        new Vector3(.2465f, .37f, .2465f));
        }

        static void Marker(Transform parent, string name)
        {
            new GameObject(name).transform.SetParent(parent, false);
        }

        static void Canal(Transform theme, int chapter, Buffer stone, Buffer gold,
            Buffer leaf, Buffer light, Buffer water)
        {
            Marker(theme, "CanalArcade");
            const int bays = 16;
            // A broad modeled canal annulus, never crossing the 12m play radius.
            for (int i = 0; i < 64; i++)
            {
                float a = i * Mathf.PI / 32, b = (i + 1) * Mathf.PI / 32;
                Vector3 P(float angle, float radius) => new Vector3(Mathf.Cos(angle) * radius,
                    -7.38f, Mathf.Sin(angle) * radius);
                water.Quad(P(a, 16), P(b, 16), P(b, 28), P(a, 28), Vector3.up);
            }
            for (int i = 0; i < bays; i++)
            {
                float angle = i * 360f / bays;
                float a = angle * Mathf.Deg2Rad;
                var at = new Vector3(Mathf.Cos(a) * 22, -7.3f, Mathf.Sin(a) * 22);
                var s = new Buffer(); var g = new Buffer(); var l = new Buffer(); var f = new Buffer();
                s.Arch(Vector3.zero, 3.72f, 2.45f, 1.18f, .57f, 1.48f);
                s.SolidAbove(new Vector3(0,3.55f,-.84f),new Vector3(.38f,.66f,.20f));
                s.SolidAbove(new Vector3(0, 4.0f, 0), new Vector3(8.65f, .55f, 2.35f));
                s.SolidAbove(new Vector3(0, 4.28f, -.75f), new Vector3(8.65f, .12f, .3f));
                s.SolidAbove(new Vector3(0, 4.99f, -.93f), new Vector3(8.65f, .14f, .28f));
                for (int post = -3; post <= 3; post++)
                {
                    s.Lathe(new Vector3(post * 1.32f, 4.3f, -.93f),
                        new[] { 0f, .14f, .46f, .66f },
                        new[] { .12f, .075f, .075f, .12f }, 6);
                }
                // Heavy pier caps and warm practical fixtures alternate with urn gardens.
                s.SolidAbove(new Vector3(4.18f, 4.52f, 0), new Vector3(.58f, .76f, .62f));
                s.SolidAbove(new Vector3(4.18f, 4.96f, 0), new Vector3(.78f, .13f, .81f));
                if (i % 2 == 0)
                {
                    Lantern(s, g, l, new Vector3(4.18f, 5.0f, 0), .75f);
                    Cypress(f, new Vector3(-3.4f, 4.28f, .38f), 2.45f, i);
                }
                else
                {
                    s.Lathe(new Vector3(4.18f, 5.03f, 0), new[] { 0f, .22f, .6f, .66f },
                        new[] { .17f, .19f, .4f, .43f }, 8);
                    Shrub(f, new Vector3(4.18f, 5.8f, 0), .65f, i);
                }
                stone.Append(s, at, 270 - angle); gold.Append(g, at, 270 - angle);
                light.Append(l, at, 270 - angle); leaf.Append(f, at, 270 - angle);
            }
        }

        static void House(Transform theme, int chapter, Buffer stone, Buffer roof,
            Buffer gold, Buffer light, Vector3 at, float yaw, float height, float width, int index, bool low)
        {
            var s = new Buffer(); var r = new Buffer(); var g = new Buffer(); var l = new Buffer();
            float depth = width * .70f;
            s.SolidAbove(new Vector3(0, height * .5f, 0), new Vector3(width, height, depth));
            s.SolidAbove(new Vector3(0, .17f, 0), new Vector3(width + .45f, .34f, depth + .5f));
            Cornice(s, height, width, depth);
            Facades(s, r, l, height, width, depth, index, chapter == 2);
            if(chapter != 2) ColumnOrder(s, new Vector3(0, 0, depth * .5f + .25f), width, height);
            if (chapter == 2)
            {
                // Cool, angular city: stepped shoulders and blade crowns instead of historic domes.
                r.Box(new Vector3(width * .12f, height + .5f, 0), new Vector3(width * .78f, 1, depth * .77f));
                if (!low)
                {
                    r.Lathe(new Vector3(width * .18f, 0, 0),
                        new[] { height + 1, height + 2, height + 3.6f },
                        new[] { width * .35f, width * .2f, .02f }, 4);
                    l.Box(new Vector3(-width * .4f, height * .52f, depth * .502f),
                        new Vector3(.09f, height * .91f, .045f));
                    l.Box(new Vector3(width * .4f, height * .52f, depth * .502f),
                        new Vector3(.09f, height * .91f, .045f));
                }
                g.Box(new Vector3(0, height + .07f, 0), new Vector3(width + .35f, .075f, depth + .36f));
            }
            else
            {
                Marker(theme, chapter == 0 ? "DayBellTower" : "SunsetPalazzo");
                if (!low && index % 4 == 0)
                {
                    Marker(theme, "ArchitecturalDome");
                    s.Lathe(new Vector3(0, height + .15f, 0), new[] { 0f, .24f, .92f, 1.02f },
                        new[] { width * .40f, width * .39f, width * .32f, width * .34f }, 12);
                    RibbedDome(r, g, new Vector3(0, height + 1.17f, 0), width * .38f, width * .34f);
                }
                else if (index % 4 == 1)
                {
                    HipRoof(r, new Vector3(0, height + .22f, 0), width + .48f, depth + .50f,
                        low ? 1.0f : 1.8f);
                    g.Box(new Vector3(0, height + (low ? 1.24f : 2.04f), 0), new Vector3(width * .46f, .07f, .09f));
                }
                else
                {
                    // A lower mansard and stair-stepped parapet varies the roofline between domes.
                    HipRoof(r, new Vector3(0, height + .2f, 0), width + .36f, depth + .4f, low ? .55f : 1.1f);
                    // Slender fluted chimneys and pedimented dormers replace huge bare roof boxes.
                    for(int side=-1;side<=1;side+=2)
                    {
                        s.Lathe(new Vector3(side * width * .31f, height + .28f, 0),
                            new[]{0f,.18f,.88f,1.0f},new[]{.20f,.16f,.16f,.23f},6);
                        var dormer = new Vector3(side * width * .22f, height + .28f, depth * .21f);
                        s.Box(dormer + Vector3.up * .35f,new Vector3(1.05f,.7f,.7f));
                        Window(s,r,dormer + new Vector3(0,.04f,.36f),.24f,.6f,false);
                        Pediment(s,dormer + new Vector3(0,.78f,.42f),1.32f,.44f);
                    }
                }
                if(!low && index % 4 == 2)
                {
                    // A projecting open roof loggia makes palazzo crowns dimensional.
                    s.Arch(new Vector3(0,height + .18f,0),1.03f,.74f,.95f,.20f,.48f);
                    Cornice(s,height + 1.97f,2.8f,1.7f);
                    HipRoof(r,new Vector3(0,height + 2.24f,0),3.0f,2.0f,.73f);
                }
                if (!low && index % 3 == 0)
                    Portico(s, g, l, new Vector3(0, 0, depth * .5f + .5f), width * .8f, 2.85f, index);
                if (!low && index % 3 != 0)
                    Balcony(s, g, new Vector3(0, height * .54f, depth * .5f), width * .65f);
            }
            stone.Append(s, at, yaw); roof.Append(r, at, yaw);
            gold.Append(g, at, yaw); light.Append(l, at, yaw);
        }

        static void Landmark(Transform theme, int chapter, Buffer stone, Buffer roof,
            Buffer gold, Buffer light, Vector3 at, float yaw, float height, int index)
        {
            if (chapter == 2)
            {
                Marker(theme, "NightCrystalSpire");
                House(theme, chapter, stone, roof, gold, light, at, yaw, height * .73f, 5.8f, index, false);
                var r = new Buffer(); var l = new Buffer();
                float upper = height * .73f;
                r.Lathe(Vector3.zero, new[] { upper, height * .95f, height * 1.20f }, new[] { 2.1f, 1.4f, .025f }, 4);
                l.Box(new Vector3(0, height * 1.1f, 0), new Vector3(.055f, height * .25f, .055f));
                roof.Append(r, at, yaw); light.Append(l, at, yaw);
                return;
            }
            Marker(theme, chapter == 0 ? "DayBellTower" : "SunsetPalazzo");
            var s = new Buffer(); var r0 = new Buffer(); var g = new Buffer(); var l0 = new Buffer();
            float shaft = height * .68f, width = 4.7f;
            s.SolidAbove(new Vector3(0, shaft * .5f, 0), new Vector3(width, shaft, width));
            // Tall pilasters, broad belt courses and a genuinely open four-sided belfry.
            for (int x = -1; x <= 1; x += 2)
                for (int z = -1; z <= 1; z += 2)
                    s.SolidAbove(new Vector3(x * width * .47f, shaft * .51f, z * width * .47f),
                        new Vector3(.33f, shaft, .33f));
            Facades(s, r0, l0, shaft, width, width, index, false);
            ColumnOrder(s,new Vector3(0,0,width * .5f + .22f),width,shaft);
            Cornice(s, shaft, width, width);
            for (int face = 0; face < 4; face++)
            {
                var arch = new Buffer();
                arch.Arch(Vector3.zero, 1.32f, 1.5f, 2.0f, .36f, .5f);
                s.Append(arch, new Vector3(0, shaft + .2f, 0) +
                    Quaternion.Euler(0, face * 90, 0) * new Vector3(0, 0, 1.94f), face * 90);
            }
            float crown = shaft + 4.15f;
            Cornice(s, crown, 5.0f, 5.0f);
            // Bell hangs inside the open arch volume, rather than a solid lit rectangle.
            g.Lathe(new Vector3(0, shaft + 1.7f, 0), new[] { 0f, .18f, .8f, .93f },
                new[] { .66f, .52f, .24f, .14f }, 10);
            if (index % 3 == 0)
            {
                Marker(theme, "ArchitecturalDome");
                RibbedDome(r0, g, new Vector3(0, crown + .2f, 0), 2.7f, 2.3f);
                s.Lathe(new Vector3(0, crown + 2.5f, 0), new[] { 0f, .65f, .78f }, new[] { .52f, .40f, .5f }, 8);
                RibbedDome(r0, g, new Vector3(0, crown + 3.3f, 0), .75f, .7f);
            }
            else if (index % 3 == 1)
            {
                r0.Lathe(new Vector3(0, crown + .2f, 0), new[] { 0f, .6f, 3.8f }, new[] { 2.8f, 2.1f, .025f }, 8);
                g.Lathe(new Vector3(0, crown + 3.9f, 0), new[] { 0f, .7f }, new[] { .08f, .015f }, 6);
            }
            else
            {
                HipRoof(r0, new Vector3(0, crown + .2f, 0), 5.3f, 5.3f, 2.0f);
                for (int merlon = -2; merlon <= 2; merlon++)
                    s.SolidAbove(new Vector3(merlon * .93f, crown + .48f, 2.6f), new Vector3(.43f, .75f, .43f));
            }
            Portico(s, g, l0, new Vector3(0, 0, 3.0f), 8.0f, 3.2f, index);
            // Paired lower domed wings give large landmarks a city silhouette.
            for (int side = -1; side <= 1; side += 2)
            {
                var wing = new Vector3(side * 4.0f, 0, 0);
                s.SolidAbove(wing + Vector3.up * 3.3f, new Vector3(3.9f, 6.6f, 4.8f));
                var ws=new Buffer();var wd=new Buffer();var wl=new Buffer();
                Facades(ws,wd,wl,6.6f,3.9f,4.8f,index + side,false);
                Cornice(ws,6.6f,3.9f,4.8f);
                s.Append(ws,wing,0);r0.Append(wd,wing,0);l0.Append(wl,wing,0);
                RibbedDome(r0, g, wing + Vector3.up * 6.8f, 2.15f, 1.65f);
            }
            stone.Append(s, at, yaw); roof.Append(r0, at, yaw);
            gold.Append(g, at, yaw); light.Append(l0, at, yaw);
        }

        static void Cornice(Buffer stone, float height, float width, float depth)
        {
            stone.Box(new Vector3(0, height + .08f, 0), new Vector3(width + .18f, .16f, depth + .18f));
            stone.Box(new Vector3(0, height + .24f, 0), new Vector3(width + .42f, .12f, depth + .42f));
        }

        static void Facades(Buffer stone, Buffer shadow, Buffer light, float height, float width, float depth, int seed, bool modern)
        {
            // Four modeled elevations stay coherent through gameplay and menu orbits.
            int rows = Mathf.Clamp(Mathf.CeilToInt(height / 2.9f), 2, 5);
            
            for (int face = 0; face < 4; face++)
            {
                var s = new Buffer(); var l = new Buffer(); var dark = new Buffer();
                float span = face % 2 == 0 ? width : depth;
                float inset = face % 2 == 0 ? depth : width;
                int bays = face == 0 && span > 6 ? 3 : 2;
                // The inward facade is the view from any terrace orbit; side bays
                // remain authored, while the far rear wall needs only its lower service tier.
                int faceRows=face == 0 ? rows : face == 2 ? 1 : Mathf.Min(rows,2);
                float levelHeight=height / faceRows;
                for (int floor = 0; floor < faceRows; floor++)
                {
                    float y = .36f + floor * levelHeight;
                    for (int bay = 0; bay < bays; bay++)
                    {
                        float x = (bay - (bays - 1) * .5f) * span / (bays + .55f);
                        bool lit = modern || (bay + floor + face + seed) % 3 != 0;
                        Window(s, lit ? l : dark, new Vector3(x, y, inset * .5f + .018f),
                            .38f, Mathf.Min(1.9f,levelHeight * .67f), modern,face == 0);
                    }
                    s.Quad(new Vector3(-span * .5f - .05f,y + levelHeight - .27f,inset * .5f + .08f),new Vector3(span * .5f + .05f,y + levelHeight - .27f,inset * .5f + .08f),new Vector3(span * .5f + .05f,y + levelHeight - .17f,inset * .5f + .08f),new Vector3(-span * .5f - .05f,y + levelHeight - .17f,inset * .5f + .08f),Vector3.forward);
                }
                stone.Append(s, Vector3.zero, face * 90); light.Append(l, Vector3.zero, face * 90); shadow.Append(dark, Vector3.zero, face * 90);
            }
        }

        static void Window(Buffer stone, Buffer light, Vector3 at, float radius, float height, bool modern,bool sculpted=true)
        {
            // Slender projecting jambs, a broad sill and arched glass avoid painted square bays.
            float spring = modern ? height : height - radius;
            if(sculpted)for(int side=-1;side<=1;side+=2)
            {
                float x=side*radius;
                stone.Quad(at + new Vector3(x,0,.035f),at + new Vector3(x,0,.16f),
                    at + new Vector3(x,spring,.16f),at + new Vector3(x,spring,.035f),Vector3.right * -side);
                stone.Quad(at + new Vector3(x,0,.16f),at + new Vector3(x + side*.11f,0,.16f),
                    at + new Vector3(x + side*.11f,spring,.16f),at + new Vector3(x,spring,.16f),Vector3.forward);
            }
            stone.Quad(at + new Vector3(-radius - .16f, -.14f, .14f), at + new Vector3(radius + .16f, -.14f, .14f), at + new Vector3(radius + .16f, 0, .14f), at + new Vector3(-radius - .16f, 0, .14f), Vector3.forward);
            if (light != null)
                light.Quad(at + new Vector3(-radius, 0, .035f), at + new Vector3(radius, 0, .035f),
                    at + new Vector3(radius, spring, .035f), at + new Vector3(-radius, spring, .035f), Vector3.forward);
            int steps = modern ? 1 : sculpted ? 4 : 2;
            for (int i = 0; i < steps; i++)
            {
                float a = i * Mathf.PI / steps, b = (i + 1) * Mathf.PI / steps;
                Vector3 P(float angle, float r, float z) => at +
                    new Vector3(Mathf.Cos(angle) * r, spring + Mathf.Sin(angle) * r, z);
                if (modern)
                    stone.Quad(at + new Vector3(-radius-.11f,height-.05f,.10f),at + new Vector3(radius+.11f,height-.05f,.10f),at + new Vector3(radius+.11f,height+.06f,.10f),at + new Vector3(-radius-.11f,height+.06f,.10f),Vector3.forward);
                else
                {
                    stone.Quad(P(a, radius, .11f), P(b, radius, .11f), P(b, radius + .105f, .11f), P(a, radius + .105f, .11f), Vector3.forward);
                    if (light != null)
                        light.Triangle(at + new Vector3(0, spring, .035f), P(a, radius, .035f),
                            P(b, radius, .035f), Vector3.forward);
                }
            }
            if (light != null)
                stone.Quad(at + new Vector3(-.025f, 0, .075f), at + new Vector3(.025f, 0, .075f),
                    at + new Vector3(.025f, height - .07f, .075f), at + new Vector3(-.025f, height - .07f, .075f), Vector3.forward);
        }

        static void ColumnOrder(Buffer stone,Vector3 at,float width,float height)
        {
            for(int column=0;column<4;column++)
            {
                float x=(column / 3f - .5f) * (width - .45f);
                stone.Lathe(at + Vector3.right * x,
                    new[]{0f,.16f,.30f,height-.40f,height-.18f,height},
                    new[]{.25f,.25f,.14f,.14f,.22f,.27f},6);
            }
            stone.Box(at + Vector3.up * (height + .10f),new Vector3(width + .20f,.20f,.54f));
            // Dentils belong to the visible projecting entablature, rather than the wall texture.
            for(int tooth=0;tooth<5;tooth++)
                stone.SolidAbove(at + new Vector3((tooth / 4f - .5f) * width,height-.07f,.22f),new Vector3(.19f,.19f,.18f));
        }

        static void Pediment(Buffer stone,Vector3 at,float width,float height)
        {
            var left=at + Vector3.left * width*.5f;
            var right=at + Vector3.right * width*.5f;
            var peak=at + Vector3.up * height;
            stone.Triangle(left,right,peak,Vector3.forward);
            stone.Box(at + Vector3.down * .04f,new Vector3(width+.18f,.12f,.22f));
        }

        static void Portico(Buffer stone, Buffer gold, Buffer light, Vector3 at, float width, float height, int seed)
        {
            float spacing = width / 3;
            for (int bay = -1; bay <= 1; bay++)
                stone.Arch(at + new Vector3(bay * spacing, 0, 0), spacing * .43f,
                    .72f, height - .72f, .17f, .52f);
            stone.Box(at + Vector3.up * (height + .3f), new Vector3(width + .5f, .36f, 1.75f));
            Balcony(stone, gold, at + Vector3.up * (height + .5f), width + .1f);
            for (int side = -1; side <= 1; side += 2)
                Lantern(stone, gold, light, at + new Vector3(side * width * .52f, .3f, .25f), .7f);
        }

        static void Balcony(Buffer stone, Buffer gold, Vector3 at, float width)
        {
            stone.Box(at + new Vector3(0, 0, .4f), new Vector3(width, .20f, 1.1f));
            for (int side = -1; side <= 1; side += 2)
                stone.Box(at + new Vector3(side * width * .37f, -.32f, .2f), new Vector3(.26f, .6f, .64f));
            for (int i = 0; i <= 4; i++)
                gold.OpenBeam(at + new Vector3((i / 4f - .5f) * (width - .20f), .46f, .92f), new Vector3(.055f, .82f, .055f));
            gold.OpenBeam(at + new Vector3(0, .88f, .92f), new Vector3(width, .07f, .09f));
        }

        static void Lantern(Buffer stone, Buffer gold, Buffer light, Vector3 at, float scale)
        {
            Vector3 P(float x, float y, float z) => at + new Vector3(x, y, z) * scale;
            gold.OpenBeam(P(0, .52f, 0), new Vector3(.07f, 1.0f, .07f) * scale);
            gold.OpenBeam(P(0, 1.08f, 0), new Vector3(.49f, .09f, .49f) * scale);
            gold.OpenBeam(P(0, 1.62f, 0), new Vector3(.49f, .09f, .49f) * scale);
            light.Box(P(0, 1.35f, 0), new Vector3(.33f, .45f, .33f) * scale);
            for (int x = -1; x <= 1; x += 2)
                gold.OpenBeam(P(x * .22f, 1.35f, x * .22f), new Vector3(.055f, .55f, .055f) * scale);
            gold.Lathe(at, new[] { 1.66f * scale, 1.92f * scale }, new[] { .37f * scale, .025f * scale }, 8);
        }

        static void HipRoof(Buffer roof, Vector3 at, float width, float depth, float height)
        {
            var a = at + new Vector3(-width * .5f, 0, -depth * .5f);
            var b = at + new Vector3(width * .5f, 0, -depth * .5f);
            var c = at + new Vector3(width * .5f, 0, depth * .5f);
            var d = at + new Vector3(-width * .5f, 0, depth * .5f);
            var e = at + new Vector3(-width * .24f, height, 0);
            var f = at + new Vector3(width * .24f, height, 0);
            roof.Quad(a, b, f, e, Vector3.back + Vector3.up);
            roof.Triangle(b, c, f, Vector3.right + Vector3.up);
            roof.Quad(c, d, e, f, Vector3.forward + Vector3.up);
            roof.Triangle(d, a, e, Vector3.left + Vector3.up);
        }

        static void RibbedDome(Buffer roof, Buffer gold, Vector3 at, float radius, float height)
        {
            roof.Dome(at, radius, height, 16);
            gold.Ring(at + Vector3.up * .025f, radius + .035f, .07f, 24);
            for (int rib = 0; rib < 6; rib++)
            {
                float a = rib * Mathf.PI / 3;
                for (int segment = 0; segment < 5; segment++)
                {
                    float t0 = segment * Mathf.PI / 10, t1 = (segment + 1) * Mathf.PI / 10;
                    Vector3 P(float t, float side) => at + new Vector3(Mathf.Cos(a + side) * Mathf.Cos(t) * (radius + .025f),
                        Mathf.Sin(t) * height + .025f, Mathf.Sin(a + side) * Mathf.Cos(t) * (radius + .025f));
                    var outward = new Vector3(Mathf.Cos(a), .7f, Mathf.Sin(a));
                    // The final rib converges to one apex. A four-corner face would
                    // leave a zero-area triangle and an unlit vertex after baking.
                    if (segment == 4)
                        gold.Triangle(P(t0, -.009f), P(t0, .009f),
                            at + Vector3.up * (height + .025f), outward);
                    else
                        gold.Quad(P(t0, -.009f), P(t0, .009f), P(t1, .009f), P(t1, -.009f), outward);
                }
            }
            gold.Lathe(at, new[] { height, height + .32f, height + .92f }, new[] { .13f, .08f, .015f }, 6);
        }

        public static void Cypress(Buffer leaf, Vector3 at, float height, int seed)
        {
            // A scalloped broad silhouette made from overlapping uneven foliage lobes.
            // No cone trunk: each cluster has radial leaf ridges and a separate apex.
            for (int level = 0; level < 5; level++)
            {
                float y = height * (.17f + level * .155f);
                float width = height * (.17f - level * .020f);
                var center = at + new Vector3(Mathf.Sin(seed + level * 1.7f) * width * .15f, y, 0);
                FoliageCluster(leaf, center, new Vector3(width, height * .24f, width * .86f), seed + level * 5);
            }
        }

        public static void Shrub(Buffer leaf, Vector3 at, float size, int seed)
        {
            FoliageCluster(leaf, at, new Vector3(size, size * .70f, size * .8f), seed);
            FoliageCluster(leaf, at + new Vector3(size * .58f, -.10f, size * .18f), new Vector3(size * .65f, size * .55f, size * .65f), seed + 3);
        }

        static void FoliageCluster(Buffer leaf, Vector3 at, Vector3 size, int seed)
        {
            const int sides = 9, rings = 5;
            Vector3 P(int ring, int side)
            {
                float a = side * Mathf.PI * 2 / sides;
                float v = ring * Mathf.PI / rings;
                float scallop = 1 + .23f * Mathf.Sin(side * 2.7f + ring * 3.1f + seed);
                return at + new Vector3(Mathf.Cos(a) * Mathf.Sin(v) * size.x * scallop,
                    Mathf.Cos(v) * size.y, Mathf.Sin(a) * Mathf.Sin(v) * size.z * scallop);
            }
            // Shared ring vertices retain the irregular silhouette without five times
            // as many duplicate leaf-face vertices.
            var points = new Vector3[(rings + 1) * (sides + 1)];
            for (int ring = 0; ring <= rings; ring++)
                for (int side = 0; side <= sides; side++)
                {
                    var point = P(ring, side % sides);
                    if (ring == 0 || ring == rings)
                    {
                        float a = side * Mathf.PI * 2 / sides;
                        point += new Vector3(Mathf.Cos(a), 0, Mathf.Sin(a)) * .007f;
                    }
                    points[ring * (sides + 1) + side] = point;
                }
            leaf.SurfaceGrid(points, sides + 1, rings + 1);
        }
    }
}
