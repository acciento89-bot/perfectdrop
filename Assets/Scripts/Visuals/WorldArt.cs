using System.Collections.Generic;
using UnityEngine;

namespace Kamilunavo.PerfectDrop.Visuals
{
    public static class WorldArt
    {
        private static Material _platform;
        private static Material _platformTop;
        private static Material _platformInset;
        private static Material _gold;
        private static Material _goldPlate;
        private static Material _cyan;
        private static Material _tower;
        private static Material _window;
        private static Material _runnerDark;
        private static Material _runnerSkin;
        private static Material _runnerHair;
        private static Material _runnerShoe;
        private static Material _cloud;
        private static Material _sun;
        private static Material _signal;
        private static Material _garden,_paving,_cityGlass,_warmWindow;
        private static Material Garden => _garden!=null?_garden:(_garden=CreateMaterial("CityGarden",new Color(.13f,.29f,.20f),.05f,.25f));
        private static Material Paving => _paving!=null?_paving:(_paving=CreateMaterial("CityPaving",new Color(.33f,.38f,.43f),.2f,.4f));
        private static Material CityGlass => _cityGlass!=null?_cityGlass:(_cityGlass=CreateMaterial("CityGlass",new Color(.08f,.20f,.31f),.52f,.82f,new Color(.015f,.075f,.12f)));
        private static Material WarmWindow => _warmWindow!=null?_warmWindow:(_warmWindow=CreateMaterial("WarmWindow",new Color(.73f,.48f,.22f),.22f,.66f,new Color(.50f,.26f,.075f)));
        // City-only finishes; the live slab/pedestal palette is independent.
        private static Material _cityLimestone,_cityMetal,_cityBronze,_cityLeaf;
        private static Material CityLimestone => _cityLimestone!=null?_cityLimestone:(_cityLimestone=CreateMaterial("CityLimestone",new Color(.76f,.70f,.59f),.08f,.37f));
        private static Material CityMetal => _cityMetal!=null?_cityMetal:(_cityMetal=CreateMaterial("CityArchitecturalMetal",new Color(.19f,.27f,.34f),.65f,.68f));
        private static Material CityBronze => _cityBronze!=null?_cityBronze:(_cityBronze=CreateMaterial("CityBronze",new Color(.43f,.29f,.17f),.54f,.59f));
        private static Material CityLeaf => _cityLeaf!=null?_cityLeaf:(_cityLeaf=CreateMaterial("CityLeafHighlights",new Color(.32f,.43f,.27f),.01f,.23f));
        private static Mesh _beveledBoxMesh;
        private static Mesh _insetFrameMesh;
        private static Mesh _daisInlayMesh;
        private static Material _daisStone;
        private static Cubemap _studioReflection;
        private static Material _skyMaterial;
        public static int ActiveChapter { get; private set; }

        public static Material Platform => _platform != null ? _platform : (_platform = CreateMaterial("Platform", new Color(0.15f, 0.19f, 0.28f), 0.48f, 0.46f));
        public static Material PlatformTop => _platformTop != null ? _platformTop : (_platformTop = CreateMaterial("PlatformTop", new Color(0.55f, 0.62f, 0.73f), 0.38f, 0.53f));
        public static Material PlatformInset => _platformInset != null ? _platformInset : (_platformInset = CreateMaterial("PlatformInset", new Color(0.018f, 0.025f, 0.040f), 0.35f, 0.22f));
        public static Material Gold => _gold != null ? _gold : (_gold = CreateMaterial("SignalGold", new Color(1f, 0.48f, 0.035f), 0.24f, 0.76f, new Color(1.85f, 0.58f, 0.035f)));
        private static Material GoldPlate => _goldPlate != null ? _goldPlate : (_goldPlate = CreateMaterial("GoldPlate", new Color(1f, .80f, .35f), .62f, .76f, new Color(.16f, .085f, .008f)));
        public static Material Cyan => _cyan != null ? _cyan : (_cyan = CreateMaterial("PrecisionCyan", new Color(0.05f, 0.72f, 0.95f), 0.18f, 0.82f, new Color(0.03f, 1.00f, 1.75f)));
        public static Material Tower => _tower != null ? _tower : (_tower = CreateMaterial("Skyline", new Color(0.07f, 0.09f, 0.15f), 0.74f, 0.28f));
        public static Material Window => _window != null ? _window : (_window = CreateMaterial("Window", new Color(0.19f, 0.52f, 0.72f), 0.12f, 0.78f, new Color(0.05f, 0.55f, 1.00f)));
        public static Material RunnerDark => _runnerDark != null ? _runnerDark : (_runnerDark = CreateMaterial("RunnerDark", new Color(0.022f, 0.025f, 0.035f), 0.18f, 0.48f));
        public static Material RunnerSkin => _runnerSkin != null ? _runnerSkin : (_runnerSkin = CreateMaterial("RunnerSkin", new Color(0.82f, 0.53f, 0.38f), 0.02f, 0.42f));
        public static Material RunnerHair => _runnerHair != null ? _runnerHair : (_runnerHair = CreateMaterial("RunnerHair", new Color(0.12f, 0.045f, 0.025f), 0.02f, 0.28f));
        public static Material RunnerShoe => _runnerShoe != null ? _runnerShoe : (_runnerShoe = CreateMaterial("RunnerShoe", new Color(0.86f, 0.89f, 0.94f), 0.16f, 0.58f));
        public static Material Cloud => _cloud != null ? _cloud : (_cloud = CreateMaterial("Cloud", new Color(0.68f, 0.68f, 0.78f), 0.0f, 0.16f, new Color(0.055f, 0.050f, 0.085f)));
        public static Material Sun => _sun != null ? _sun : (_sun = CreateMaterial("Sun", new Color(1.0f, 0.67f, 0.22f), 0f, 0.12f, new Color(3.0f, 1.30f, 0.35f)));

        public static void ApplySkybox()
        {
            var shader = Resources.Load<Shader>("PerfectDropSky");
            if (shader == null) shader = Shader.Find("Kamilunavo/PerfectDropSky");
            if (shader == null) return;

            if(_skyMaterial==null)_skyMaterial=new Material(shader){name="PerfectDropSkybox",hideFlags=HideFlags.HideAndDontSave};
            var material=_skyMaterial;
            RenderSettings.skybox = material;
            var portrait = Resources.Load<Texture2D>("Art/CloudCityPortrait");
            var landscape = Resources.Load<Texture2D>("Art/CloudCityLandscape");
            if (portrait != null)
            {
                material.SetTexture("_BackdropPortrait", portrait);
                material.SetTexture("_BackdropLandscape", landscape != null ? landscape : portrait);
                material.SetFloat("_UseBackdrop", 1);
            }
            // A small cached studio reflection gives live metal the warm/cool lighting
            // of the authored cloud city without a per-frame reflection probe.
            if (_studioReflection == null) _studioReflection = BuildStudioReflection();
            RenderSettings.defaultReflectionMode = UnityEngine.Rendering.DefaultReflectionMode.Custom;
            RenderSettings.customReflection = _studioReflection;
            RenderSettings.reflectionIntensity = .8f;
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.40f, 0.48f, 0.68f);
            RenderSettings.ambientEquatorColor = new Color(0.40f, 0.32f, 0.37f);
            RenderSettings.ambientGroundColor = new Color(0.15f, 0.17f, 0.25f);
        }

        public static void SetChapter(int chapter)
        {
            if(_skyMaterial==null)return;
            chapter=Mathf.Clamp(chapter,0,2);ActiveChapter=chapter;
            var stem=chapter==0?"CloudCityDay":chapter==2?"CloudCityNight":"CloudCity";
            var portrait=Resources.Load<Texture2D>("Art/"+stem+"Portrait")??Resources.Load<Texture2D>("Art/CloudCityPortrait");
            var landscape=Resources.Load<Texture2D>("Art/"+stem+"Landscape")??Resources.Load<Texture2D>("Art/CloudCityLandscape");
            if(portrait!=null)_skyMaterial.SetTexture("_BackdropPortrait",portrait);
            if(landscape!=null)_skyMaterial.SetTexture("_BackdropLandscape",landscape);
            RenderSettings.ambientSkyColor=chapter==2?new Color(.30f,.38f,.63f):chapter==0?new Color(.52f,.62f,.82f):new Color(.40f,.48f,.68f);
        }

        public static void DecoratePlatform(Transform platform, int index)
        {
            if (platform == null) return;

            var renderer = platform.GetComponent<Renderer>();
            if (renderer != null) renderer.enabled = false;

            AddBeveledBox(platform, "Deck", new Vector3(0f, -0.01f, 0f), new Vector3(0.985f, 0.86f, 0.985f), Platform);
            AddBeveledBox(platform, "Undercore", new Vector3(0f, -0.54f, 0f), new Vector3(0.82f, 0.42f, 0.80f), PlatformInset);
            AddBeveledBox(platform, "TopPlate", new Vector3(0f, 0.48f, 0f), new Vector3(0.94f, 0.075f, 0.92f), PlatformTop);

            AddCube(platform, "TrimFront", new Vector3(0f, 0.555f, -0.487f), new Vector3(0.97f, 0.060f, 0.024f), Gold);
            AddCube(platform, "TrimBack", new Vector3(0f, 0.555f, 0.487f), new Vector3(0.97f, 0.060f, 0.024f), Gold);
            AddCube(platform, "TrimLeft", new Vector3(-0.487f, 0.555f, 0f), new Vector3(0.024f, 0.060f, 0.94f), Gold);
            AddCube(platform, "TrimRight", new Vector3(0.487f, 0.555f, 0f), new Vector3(0.024f, 0.060f, 0.94f), Gold);

            // Surface panel seams keep the deck from reading as one untextured primitive.
            for (var i = -1; i <= 1; i++)
                AddCube(platform, $"DeckSeamZ_{i}", new Vector3(i * 0.22f, 0.526f, 0f), new Vector3(0.008f, 0.012f, 0.82f), PlatformInset);
            AddCube(platform, "DeckSeamX", new Vector3(0f, 0.527f, 0f), new Vector3(0.82f, 0.012f, 0.008f), PlatformInset);

            // A little authored silhouette at milestone platforms.
            if ((index + 1) % 5 == 0)
            {
                AddCube(platform, "MilestoneLeft", new Vector3(-0.44f, 0.92f, 0.34f), new Vector3(0.045f, 0.58f, 0.045f), Gold);
                AddCube(platform, "MilestoneRight", new Vector3(0.44f, 0.92f, 0.34f), new Vector3(0.045f, 0.58f, 0.045f), Gold);
                AddCube(platform, "MilestoneLeftCap", new Vector3(-0.44f, 1.22f, 0.34f), new Vector3(0.10f, 0.055f, 0.10f), Cyan);
                AddCube(platform, "MilestoneRightCap", new Vector3(0.44f, 1.22f, 0.34f), new Vector3(0.10f, 0.055f, 0.10f), Cyan);
            }
        }

        public static GameObject CreateLandingBay(Transform platform, float halfWidthWorld, float halfDepthWorld)
        {
            if (platform == null) return null;

            var size = platform.lossyScale;
            var halfX = Mathf.Clamp(halfWidthWorld / Mathf.Max(0.01f, size.x), 0.12f, 0.44f);
            var halfZ = Mathf.Clamp(halfDepthWorld / Mathf.Max(0.01f, size.z), 0.12f, 0.44f);
            const float thickness = 0.022f;
            const float y = 0.615f;

            var root = new GameObject("PrecisionBay");
            root.transform.SetParent(platform, false);
            AddCube(root.transform, "BayFront", new Vector3(0f, y, -halfZ), new Vector3(halfX * 2f, 0.045f, thickness), Gold);
            AddCube(root.transform, "BayBack", new Vector3(0f, y, halfZ), new Vector3(halfX * 2f, 0.045f, thickness), Gold);
            AddCube(root.transform, "BayLeft", new Vector3(-halfX, y, 0f), new Vector3(thickness, 0.045f, halfZ * 2f), Gold);
            AddCube(root.transform, "BayRight", new Vector3(halfX, y, 0f), new Vector3(thickness, 0.045f, halfZ * 2f), Gold);

            var innerScale = 0.72f;
            AddCube(root.transform, "BayInnerFront", new Vector3(0f, y + 0.012f, -halfZ * innerScale), new Vector3(halfX * 1.25f, 0.028f, thickness * 0.72f), Cyan);
            AddCube(root.transform, "BayInnerBack", new Vector3(0f, y + 0.012f, halfZ * innerScale), new Vector3(halfX * 1.25f, 0.028f, thickness * 0.72f), Cyan);
            AddCube(root.transform, "BayInnerLeft", new Vector3(-halfX * innerScale, y + 0.012f, 0f), new Vector3(thickness * 0.72f, 0.028f, halfZ * 1.20f), Cyan);
            AddCube(root.transform, "BayInnerRight", new Vector3(halfX * innerScale, y + 0.012f, 0f), new Vector3(thickness * 0.72f, 0.028f, halfZ * 1.20f), Cyan);

            var center = AddCube(root.transform, "BayCenter", new Vector3(0f, y + 0.018f, 0f), new Vector3(0.060f, 0.034f, 0.060f), Gold);
            center.transform.localRotation = Quaternion.Euler(0f, 45f, 0f);
            return root;
        }

        public static RunnerVisualAnimator CreateRunnerVisual(Transform root)
        {
            if (root == null) return null;

            var visual = new GameObject("RunnerVisual").transform;
            visual.SetParent(root, false);

            // Broad hoodie chest + tapered waist gives a readable stylized runner silhouette.
            var torso = AddCube(visual, "Torso", new Vector3(0f, 1.10f, 0f), new Vector3(0.78f, 0.66f, 0.44f), RunnerDark).transform;
            AddCube(visual, "Waist", new Vector3(0f, 0.79f, 0f), new Vector3(0.56f, 0.22f, 0.34f), RunnerDark);
            AddPrimitive(PrimitiveType.Capsule, visual, "Hood", new Vector3(0f, 1.46f, -0.10f), new Vector3(0.35f, 0.22f, 0.30f), RunnerDark);
            AddCube(visual, "Collar", new Vector3(0f, 1.38f, -0.215f), new Vector3(0.44f, 0.085f, 0.035f), Gold);
            AddCube(visual, "LeftCuff", new Vector3(-0.45f, 0.79f, -0.005f), new Vector3(0.15f, 0.060f, 0.15f), Gold);
            AddCube(visual, "RightCuff", new Vector3(0.45f, 0.79f, -0.005f), new Vector3(0.15f, 0.060f, 0.15f), Gold);

            // Skin + layered hair, tuned for the rear gameplay camera.
            AddPrimitive(PrimitiveType.Sphere, visual, "Head", new Vector3(0f, 1.70f, 0f), new Vector3(0.36f, 0.38f, 0.36f), RunnerSkin);
            AddPrimitive(PrimitiveType.Sphere, visual, "HairCap", new Vector3(0f, 1.86f, -0.02f), new Vector3(0.42f, 0.25f, 0.38f), RunnerHair);
            AddPrimitive(PrimitiveType.Sphere, visual, "LeftEar", new Vector3(-0.20f, 1.69f, -0.01f), new Vector3(0.075f, 0.095f, 0.060f), RunnerSkin);
            AddPrimitive(PrimitiveType.Sphere, visual, "RightEar", new Vector3(0.20f, 1.69f, -0.01f), new Vector3(0.075f, 0.095f, 0.060f), RunnerSkin);
            for (var i = 0; i < 9; i++)
            {
                var x = (i - 4) * 0.052f;
                var y = 1.93f + (i % 3) * 0.022f;
                var z = -0.16f - Mathf.Abs(i - 4) * 0.008f;
                var spike = AddCube(visual, $"HairSpike_{i}", new Vector3(x, y, z), new Vector3(0.050f, 0.20f + (i % 2) * 0.045f, 0.072f), RunnerHair);
                spike.transform.localRotation = Quaternion.Euler(-8f, 0f, (i - 4) * -7f);
            }
            AddCube(visual, "HairNape", new Vector3(0f, 1.70f, -0.185f), new Vector3(0.28f, 0.16f, 0.055f), RunnerHair);

            var leftArm = AddPrimitive(PrimitiveType.Capsule, visual, "LeftArm", new Vector3(-0.47f, 1.08f, 0f), new Vector3(0.15f, 0.34f, 0.15f), RunnerDark).transform;
            var rightArm = AddPrimitive(PrimitiveType.Capsule, visual, "RightArm", new Vector3(0.47f, 1.08f, 0f), new Vector3(0.15f, 0.34f, 0.15f), RunnerDark).transform;
            AddPrimitive(PrimitiveType.Sphere, visual, "LeftHand", new Vector3(-0.47f, 0.72f, 0.01f), new Vector3(0.15f, 0.15f, 0.15f), RunnerSkin);
            AddPrimitive(PrimitiveType.Sphere, visual, "RightHand", new Vector3(0.47f, 0.72f, 0.01f), new Vector3(0.15f, 0.15f, 0.15f), RunnerSkin);

            var leftLeg = AddPrimitive(PrimitiveType.Capsule, visual, "LeftLeg", new Vector3(-0.18f, 0.43f, 0f), new Vector3(0.16f, 0.35f, 0.16f), RunnerDark).transform;
            var rightLeg = AddPrimitive(PrimitiveType.Capsule, visual, "RightLeg", new Vector3(0.18f, 0.43f, 0f), new Vector3(0.16f, 0.35f, 0.16f), RunnerDark).transform;

            AddCube(visual, "LeftShoe", new Vector3(-0.18f, 0.08f, 0.12f), new Vector3(0.29f, 0.16f, 0.44f), RunnerShoe);
            AddCube(visual, "RightShoe", new Vector3(0.18f, 0.08f, 0.12f), new Vector3(0.29f, 0.16f, 0.44f), RunnerShoe);
            AddCube(visual, "LeftSoleGlow", new Vector3(-0.18f, 0.014f, 0.13f), new Vector3(0.23f, 0.032f, 0.37f), Gold);
            AddCube(visual, "RightSoleGlow", new Vector3(0.18f, 0.014f, 0.13f), new Vector3(0.23f, 0.032f, 0.37f), Gold);

            // Three bars form the concept's triangular back mark.
            var markTop = AddCube(visual, "BackMarkTop", new Vector3(0f, 1.16f, -0.214f), new Vector3(0.28f, 0.046f, 0.022f), Gold);
            markTop.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            var markLeft = AddCube(visual, "BackMarkLeft", new Vector3(-0.10f, 1.07f, -0.214f), new Vector3(0.22f, 0.046f, 0.022f), Gold);
            markLeft.transform.localRotation = Quaternion.Euler(0f, 0f, -58f);
            var markRight = AddCube(visual, "BackMarkRight", new Vector3(0.10f, 1.07f, -0.214f), new Vector3(0.22f, 0.046f, 0.022f), Gold);
            markRight.transform.localRotation = Quaternion.Euler(0f, 0f, 58f);

            var animator = root.gameObject.AddComponent<RunnerVisualAnimator>();
            animator.VisualRoot = visual;
            animator.LeftArm = leftArm;
            animator.RightArm = rightArm;
            animator.LeftLeg = leftLeg;
            animator.RightLeg = rightLeg;
            return animator;
        }

        public static void BuildAtmosphere(Transform parent, int seed = 260910)
        {
            if (parent == null) return;
            var previous = Random.state;
            Random.InitState(seed);

            var atmosphere = new GameObject("Atmosphere").transform;
            atmosphere.SetParent(parent, false);

            // Cloud sea: intentionally sits below the route so it creates depth without hiding landings.
            for (var i = 0; i < 28; i++)
            {
                var center = new Vector3(
                    Random.Range(-30f, 30f),
                    Random.Range(-10.5f, -7.0f) + i * 0.045f,
                    Random.Range(-12f, 165f));

                var cluster = new GameObject($"Cloud_{i:00}").transform;
                cluster.SetParent(atmosphere, false);
                cluster.localPosition = center;

                var pieces = Random.Range(3, 6);
                for (var p = 0; p < pieces; p++)
                {
                    var puff = AddPrimitive(
                        PrimitiveType.Sphere,
                        cluster,
                        $"Puff_{p}",
                        new Vector3(Random.Range(-2.8f, 2.8f), Random.Range(-0.8f, 0.8f), Random.Range(-1.6f, 1.6f)),
                        new Vector3(Random.Range(4.8f, 8.6f), Random.Range(1.45f, 2.85f), Random.Range(4.1f, 7.2f)),
                        Cloud);
                    puff.isStatic = true;
                }
            }

            var sun = AddPrimitive(PrimitiveType.Sphere, atmosphere, "HorizonSun", new Vector3(36f, 13f, 150f), new Vector3(8f, 8f, 8f), Sun);
            sun.isStatic = true;

            var animator = atmosphere.gameObject.AddComponent<AtmosphereAnimator>();
            animator.Bind(atmosphere);
            Random.state = previous;
        }

        public static void SpawnLandingBurst(Vector3 worldPosition, Gameplay.LandingGrade grade)
        {
            if (GamePreferences.ReducedMotion) return;
            var root = new GameObject($"LandingBurst_{grade}").transform;
            root.position = worldPosition + Vector3.up * 0.12f;

            var material = grade == Gameplay.LandingGrade.Perfect ? Cyan : Gold;
            const int count = 10;
            var pieces = new Transform[count];
            var velocity = new Vector3[count];

            for (var i = 0; i < count; i++)
            {
                var angle = i * Mathf.PI * 2f / count;
                var dir = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
                var piece = AddCube(root, $"Shard_{i:00}", dir * 0.22f, new Vector3(0.09f, 0.09f, 0.30f), material);
                piece.transform.rotation = Quaternion.LookRotation(dir + Vector3.up * 0.18f, Vector3.up);
                pieces[i] = piece.transform;
                velocity[i] = dir * (2.0f + (i % 3) * 0.22f) + Vector3.up * (1.0f + (i % 2) * 0.22f);
            }

            var burst = root.gameObject.AddComponent<LandingBurst>();
            burst.Initialize(pieces, velocity, grade == Gameplay.LandingGrade.Perfect ? 0.48f : 0.38f);
        }

        public static void SpawnStackLandingPulse(Vector3 position,Vector2 size,int style)
        {
            if(GamePreferences.ReducedMotion)return;
            if(_signal==null)
            {
                var shader=Resources.Load<Shader>("PerfectDropSignal");if(shader==null)return;
                _signal=new Material(shader){name="PerfectLandingSignal",hideFlags=HideFlags.HideAndDontSave,enableInstancing=true};
            }
            if(_insetFrameMesh==null)_insetFrameMesh=BuildInsetFrame();
            var pulse=new GameObject("PerfectLandingPulse",typeof(MeshFilter),typeof(MeshRenderer));
            pulse.transform.position=position+Vector3.up*.018f;pulse.transform.localScale=new Vector3(size.x*1.4f,.014f,size.y*1.4f);
            pulse.GetComponent<MeshFilter>().sharedMesh=_insetFrameMesh;pulse.GetComponent<MeshRenderer>().sharedMaterial=_signal;
            pulse.AddComponent<StackLandingPulse>().Initialize(StackStylePalette.Get(style).Accent);
        }

        public static void BuildGoalBeacon(Transform parent, Vector3 center)
        {
            if (parent == null) return;
            var beacon = new GameObject("GoalBeacon").transform;
            beacon.SetParent(parent, false);

            const int segments = 30;
            const float radius = 3.1f;
            for (var i = 0; i < segments; i++)
            {
                var angle = i * Mathf.PI * 2f / segments;
                var position = center + new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0f);
                var segment = AddCube(beacon, $"Ring_{i:00}", position, new Vector3(0.36f, 0.36f, 0.20f), Gold);
                segment.transform.localRotation = Quaternion.Euler(0f, 0f, -angle * Mathf.Rad2Deg);
            }

            AddCube(beacon, "CoreVertical", center, new Vector3(0.13f, radius * 1.15f, 0.10f), Cyan);
            AddCube(beacon, "CoreHorizontal", center, new Vector3(radius * 1.15f, 0.13f, 0.10f), Cyan);
            beacon.gameObject.AddComponent<GoalBeaconAnimator>();
        }

        public static void BuildSkyline(Transform parent, int seed = 260907)
        {
            if (parent == null) return;
            var previous = Random.state;
            Random.InitState(seed);

            var skyline = new GameObject("Skyline").transform;
            skyline.SetParent(parent, false);

            for (var i = 0; i < 40; i++)
            {
                var side = i % 2 == 0 ? -1f : 1f;
                var height = Random.Range(10f, 34f);
                var width = Random.Range(3.0f, 7.0f);
                var depth = Random.Range(3.0f, 6.0f);
                var x = side * Random.Range(18f, 34f);
                var z = i * 4.2f + Random.Range(-3f, 3f);
                var y = height * 0.5f - 11f + i * 0.10f;

                var tower = AddCube(skyline, $"Tower_{i:00}", new Vector3(x, y, z), new Vector3(width, height, depth), Tower);
                tower.isStatic = true;

                if (i % 2 == 0)
                {
                    var signal = i % 4 == 0 ? Gold : Window;
                    var strip = AddCube(skyline, $"Signal_{i:00}", new Vector3(x - side * (width * 0.505f), y + height * 0.10f, z), new Vector3(0.055f, Mathf.Min(7f, height * 0.35f), depth * 0.15f), signal);
                    strip.isStatic = true;
                }
            }

            Random.state = previous;
        }

        public static void BuildStackCloudSea(Transform parent)
        {
            var shader = Resources.Load<Shader>("PerfectDropCloudSea");
            var mesh = new Mesh { name = "CloudSeaSurface" };
            mesh.vertices = new[] { new Vector3(-200,-6,-200), new Vector3(-200,-6,200), new Vector3(200,-6,200), new Vector3(200,-6,-200) };
            mesh.triangles = new[] { 0,1,2,0,2,3 }; mesh.RecalculateNormals(); mesh.RecalculateBounds();
            var sea = new GameObject("CloudSea",typeof(MeshFilter),typeof(MeshRenderer));
            sea.transform.SetParent(parent,false);
            sea.GetComponent<MeshFilter>().sharedMesh = mesh;
            sea.GetComponent<MeshRenderer>().sharedMaterial = new Material(shader) { name = "SunsetCloudSea" };
        }

        public static GameObject CreateStackBlock(Transform parent, string name, Vector3 position, Vector3 size, int level)
        {
            var root = new GameObject(name);
            root.transform.SetParent(parent, false);
            root.transform.localPosition = position;
            var body = AddBeveledBox(root.transform, "MetalDeck", new Vector3(0,size.y*.025f,0),new Vector3(size.x,size.y*.79f,size.z), Platform);
            var tint = new MaterialPropertyBlock();
            tint.SetColor("_Color", StackStylePalette.Get(0).Body);
            tint.SetColor("_DeckBaseColor",tint.GetColor("_Color"));
            body.GetComponent<Renderer>().SetPropertyBlock(tint);
            AddBeveledBox(root.transform,"Undercore",new Vector3(0,-size.y*.44f,0),new Vector3(size.x*.91f,size.y*.12f,size.z*.91f),PlatformInset);
            // A metallic gold inset and a dark shoulder mirror the app icon.
            // All parts scale with the actual retained/cut footprint, including slivers.
            AddBeveledBox(root.transform, "TopPlate", new Vector3(0,size.y*.508f,0), new Vector3(size.x*.77f,size.y*.032f,size.z*.77f), GoldPlate);
            // A continuous band replaces the corresponding body slice. It stays
            // inside the logical footprint even after a very narrow overhang cut.
            AddBeveledBox(root.transform,"GoldBand",new Vector3(0,-size.y*.43f,0),new Vector3(size.x,size.y*.14f,size.z),Gold);
            AddBeveledBox(root.transform,"DeckCrown",new Vector3(0,size.y*.455f,0),new Vector3(size.x,size.y*.09f,size.z),Platform);
            var inset=new GameObject("GoldInset",typeof(MeshFilter),typeof(MeshRenderer));
            inset.transform.SetParent(root.transform,false);inset.transform.localPosition=new Vector3(0,size.y*.538f,0);
            inset.transform.localScale=new Vector3(size.x,.012f,size.z);
            if(_insetFrameMesh==null)_insetFrameMesh=BuildInsetFrame();
            inset.GetComponent<MeshFilter>().sharedMesh=_insetFrameMesh;inset.GetComponent<MeshRenderer>().sharedMaterial=Gold;
            return root;
        }

        public static GameObject BuildStackDais(Transform parent)
        {
            var root=new GameObject("ArchitecturalDais");root.transform.SetParent(parent,false);
            if(_daisStone==null)_daisStone=CreateMaterial("IvoryDais",new Color(.74f,.68f,.55f),.15f,.52f);
            AddPrimitive(PrimitiveType.Cylinder,root.transform,"StoneDrum",new Vector3(0,-1.03f,0),new Vector3(6.35f,.18f,6.35f),_daisStone);
            AddPrimitive(PrimitiveType.Cylinder,root.transform,"NavyDrum",new Vector3(0,-1.27f,0),new Vector3(5.85f,.13f,5.85f),PlatformInset);
            var inlay=new GameObject("CompassInlay",typeof(MeshFilter),typeof(MeshRenderer));inlay.transform.SetParent(root.transform,false);inlay.transform.localPosition=new Vector3(0,-.842f,0);
            if(_daisInlayMesh==null)_daisInlayMesh=BuildDaisInlay();
            inlay.GetComponent<MeshFilter>().sharedMesh=_daisInlayMesh;inlay.GetComponent<MeshRenderer>().sharedMaterial=GoldPlate;
            return root;
        }
        private static Mesh BuildDaisInlay()
        {
            var vertices=new List<Vector3>();var triangles=new List<int>();var uv=new List<Vector2>();
            void Quad(Vector3 a,Vector3 b,Vector3 c,Vector3 d)
            {
                var first=vertices.Count;vertices.AddRange(new[]{a,b,c,d});
                uv.AddRange(new[]{Vector2.zero,Vector2.right,Vector2.one,Vector2.up});
                if(Vector3.Cross(b-a,c-a).y>0)triangles.AddRange(new[]{first,first+1,first+2,first,first+2,first+3});
                else triangles.AddRange(new[]{first,first+2,first+1,first,first+3,first+2});
            }
            Vector3 Radial(float angle,float radius)=>new Vector3(Mathf.Sin(angle)*radius,0,Mathf.Cos(angle)*radius);
            foreach(var radius in new[]{2.18f,2.94f,3.10f})for(var segment=0;segment<64;segment++)
            {
                var a=segment*Mathf.PI/32;var b=(segment+1)*Mathf.PI/32;
                Quad(Radial(a,radius),Radial(b,radius),Radial(b,radius+.028f),Radial(a,radius+.028f));
            }
            for(var ray=0;ray<16;ray++)
            {
                var angle=ray*Mathf.PI/8;var along=Radial(angle,1);var cross=new Vector3(along.z,0,-along.x)*.015f;
                Quad(along*2.20f-cross,along*2.93f-cross,along*2.93f+cross,along*2.20f+cross);
            }
            var mesh=new Mesh{name="GoldCompassInlay",hideFlags=HideFlags.HideAndDontSave};mesh.SetVertices(vertices);mesh.SetTriangles(triangles,0);mesh.SetUVs(0,uv);mesh.RecalculateNormals();mesh.RecalculateBounds();return mesh;
        }

        public static GameObject BuildPlayerCity(Transform parent,StackProfile profile)
        {
            var root=new GameObject("OwnedCity");root.transform.SetParent(parent,false);root.transform.localPosition=new Vector3(0,-4,42);
            for(var district=0;district<3;district++)
            {
                var group=new GameObject("CityDistrict"+district);group.transform.SetParent(root.transform,false);
                var origin=new Vector3((district-1)*24,0,0);
                // Landscape has its own batch owner; towers remain separately named/selectable.
                var landscape=new GameObject("CityLandscape"+district);landscape.transform.SetParent(group.transform,false);
                var cityParent=landscape.transform;var accent=district==2?Cyan:Gold;
                AddBeveledBox(cityParent,"DistrictFoundation"+district,origin+Vector3.down*.7f,new Vector3(19,1.4f,28),PlatformInset);
                AddBeveledBox(cityParent,"CityPaving"+district,origin,new Vector3(18.6f,.12f,27.6f),Paving);
                AddCube(cityParent,"Boulevard"+district,origin+new Vector3(0,.085f,0),new Vector3(2.4f,.035f,27),PlatformInset);
                for(var line=0;line<10;line++)AddCube(cityParent,"RoadMark"+line,origin+new Vector3(0,.11f,(line-4.5f)*2.6f),new Vector3(.075f,.015f,1.10f),CityLimestone);
                for(var side=-1;side<=1;side+=2)
                {
                    AddCube(cityParent,"StonePromenade"+side,origin+new Vector3(side*1.64f,.15f,0),new Vector3(.78f,.16f,27),CityLimestone);
                    AddCube(cityParent,"EdgeLight"+side,origin+new Vector3(side*9.15f,-.22f,0),new Vector3(.035f,.09f,27),accent);
                    for(var lamp=0;lamp<5;lamp++)CityLamp(cityParent,origin+new Vector3(side*1.95f,.21f,-10.5f+lamp*5.2f),district);
                    for(var rail=0;rail<8;rail++)
                    {
                        var q=origin+new Vector3(side*9.05f,.22f,-12f+rail*3.4f);
                        AddCube(cityParent,"PromenadeRailPost",q+Vector3.up*.24f,new Vector3(.065f,.48f,.065f),CityMetal);
                        AddCube(cityParent,"PromenadeRail",q+new Vector3(0,.46f,1.5f),new Vector3(.04f,.055f,3.4f),CityBronze);
                    }
                }
                foreach(var z in new[]{-7.8f,-2.6f,2.6f,7.8f})
                {
                    AddCube(cityParent,"PedestrianPassage",origin+new Vector3(0,.18f,z),new Vector3(17.5f,.055f,.63f),CityLimestone);
                    for(var crossing=0;crossing<5;crossing++)AddCube(cityParent,"BoulevardCrossing",origin+new Vector3(-.9f+crossing*.45f,.22f,z),new Vector3(.24f,.012f,.72f),CityLimestone);
                }
                AddBeveledBox(cityParent,"ArrivalPlaza",origin+new Vector3(0,.15f,-13.0f),new Vector3(17.8f,.15f,1.25f),CityLimestone);
                for(var side=-1;side<=1;side+=2)
                {
                    CityBench(cityParent,origin+new Vector3(side*2.9f,.25f,-12.9f));
                    CityTree(cityParent,origin+new Vector3(side*7.9f,.20f,-12.7f),.68f,district+side);
                    CityFountain(cityParent,origin+new Vector3(side*8.0f,.17f,0),accent);
                }
                for(var slot=0;slot<10;slot++)
                {
                    var id=district*10+slot;var side=slot%2==0?-1:1;
                    var point=origin+new Vector3(side*(4.55f+(slot%3)*.25f),.20f,(slot/2-2)*5.2f+(slot%3-1)*.16f);
                    // Small gardens and forecourts, rather than ten identical flat green rectangles.
                    var garden=point+new Vector3(side*2.17f,.07f,.15f);
                    AddBeveledBox(cityParent,"GardenBorder"+(id+1),garden,new Vector3(.94f,.18f,3.1f),CityLimestone);
                    AddBeveledBox(cityParent,"PlantedVerge"+(id+1),garden+Vector3.up*.10f,new Vector3(.80f,.05f,2.94f),Garden);
                    AddBeveledBox(cityParent,"EntryForecourt"+(id+1),point+new Vector3(-side*1.45f,.07f,-.12f),new Vector3(1.05f,.13f,2.15f),CityLimestone);
                    for(var tree=0;tree<2;tree++)CityTree(cityParent,garden+new Vector3(0,.14f,(tree==0?-1:1)*1.1f),tree==0?.73f:.59f,id+tree);
                    if(slot%3==1)CityBench(cityParent,point+new Vector3(-side*1.5f,.18f,1.1f));
                    if(profile.LevelStars[id]==0)continue;
                    var building=new GameObject("CityTower"+(id+1));building.transform.SetParent(group.transform,false);building.transform.localPosition=point;
                    BuildCityArchitecture(building.transform,id,slot,district,profile.LevelStars[id]);
                    BakeBuilding(building);
                }
                BakeBuilding(landscape);
            }
            return root;
        }

        private static void BuildCityArchitecture(Transform parent,int id,int slot,int district,int stars)
        {
            var floors=4+Kamilunavo.PerfectDrop.Gameplay.StackCampaign.Level(id+1).Target/4;
            var height=.85f+floors*.63f;var kind=(slot+district*2)%6;
            var width=kind==1?2.45f:kind==2?3.6f:3.1f;var depth=kind==2?3.45f:kind==5?3.55f:2.9f;
            var stone=district==0||kind==2||kind==4?CityLimestone:district==1?CityBronze:CityMetal;
            var accent=district==2?Cyan:Gold;
            AddBeveledBox(parent,"ArchitecturalPlinth",new Vector3(0,.22f,0),new Vector3(width+.28f,.35f,depth+.25f),CityLimestone);
            AddBeveledBox(parent,"StoneLobby",new Vector3(0,.68f,0),new Vector3(width,1.0f,depth),stone);
            AddCube(parent,"LobbyGlass",new Vector3(0,.67f,-depth*.505f),new Vector3(width*.72f,.71f,.035f),CityGlass);
            AddCube(parent,"EntryDoorFrame",new Vector3(.05f,.65f,-depth*.525f),new Vector3(.63f,.79f,.04f),CityMetal);
            AddCube(parent,"EntryDoorGlass",new Vector3(.05f,.65f,-depth*.537f),new Vector3(.49f,.68f,.02f),CityGlass);
            AddBeveledBox(parent,"EntranceCanopy",new Vector3(0,1.20f,-depth*.53f-.25f),new Vector3(1.85f,.14f,.86f),CityMetal);
            AddCube(parent,"CanopyUndersideLight",new Vector3(0,1.115f,-depth*.53f-.25f),new Vector3(1.36f,.025f,.50f),WarmWindow);
            foreach(var x in new[]{-.73f,.73f})AddCube(parent,"EntryStoneColumn",new Vector3(x,.69f,-depth*.53f-.45f),new Vector3(.105f,1.01f,.105f),CityLimestone);
            var shoulder=height*.66f;var crownWidth=width;var crownDepth=depth;var roof=height;
            if(kind==0)
            {
                // Art Deco: continuous stone shaft, real successive setbacks and vertical buttresses.
                AddBeveledBox(parent,"StoneShaft",new Vector3(0,(shoulder+1.12f)*.5f,0),new Vector3(width,shoulder-1.12f,depth),stone);
                AddBeveledBox(parent,"FirstSetback",new Vector3(0,(height+shoulder)*.5f-.13f,.14f),new Vector3(width*.77f,height-shoulder,depth*.78f),stone);
                AddBeveledBox(parent,"SteppedLantern",new Vector3(0,height+.28f,.14f),new Vector3(width*.48f,.68f,depth*.5f),CityLimestone);
                for(var fin=0;fin<3;fin++)AddCube(parent,"DecoStoneButtress",new Vector3((fin-1)*width*.37f,shoulder*.55f,-depth*.52f),new Vector3(.12f,shoulder*.80f,.11f),CityLimestone);
                crownWidth*=.78f;crownDepth*=.78f;
            }
            else if(kind==1)
            {
                // Two slender metal volumes with a glazed recessed connection and asymmetric roof.
                for(var wing=-1;wing<=1;wing+=2)AddBeveledBox(parent,"PairedMetalTower"+wing,new Vector3(wing*width*.28f,(height+1.05f)*.5f,wing*.14f),new Vector3(width*.41f,height-1.05f,depth*.84f),stone);
                AddCube(parent,"RecessedSkybridge",new Vector3(0,height*.61f,.12f),new Vector3(width*.38f,.55f,depth*.65f),CityGlass);
                AddBeveledBox(parent,"CantileverRoof",new Vector3(.30f,height+.10f,.13f),new Vector3(width*.82f,.22f,depth*.86f),CityMetal);
                crownWidth*=.88f;crownDepth*=.84f;
            }
            else if(kind==2)
            {
                // Courtyard wings make an open silhouette, with a lower bridge and stone colonnade.
                for(var wing=-1;wing<=1;wing+=2)AddBeveledBox(parent,"CourtyardStoneWing"+wing,new Vector3(wing*width*.31f,(height+1.05f)*.5f,0),new Vector3(width*.30f,height-1.05f,depth),stone);
                AddBeveledBox(parent,"CourtyardRearBridge",new Vector3(0,height*.59f,depth*.32f),new Vector3(width*.52f,height*.27f,depth*.26f),stone);
                for(var column=0;column<4;column++)AddCube(parent,"CourtyardColonnade",new Vector3((column-1.5f)*.53f,1.20f,-depth*.40f),new Vector3(.13f,1.95f,.13f),CityLimestone);
                AddBeveledBox(parent,"CourtyardTerrace",new Vector3(0,2.21f,-depth*.35f),new Vector3(width*.64f,.17f,.68f),CityLimestone);
            }
            else if(kind==3)
            {
                // Glass/metal business tower: substantial lower mass and offset upper lantern.
                AddBeveledBox(parent,"GlazedMetalShaft",new Vector3(0,(height+1.05f)*.5f,0),new Vector3(width*.84f,height-1.05f,depth*.82f),CityMetal);
                for(var fin=-1;fin<=1;fin+=2)AddCube(parent,"ContinuousArchitecturalFin",new Vector3(fin*width*.42f,height*.51f,-depth*.43f),new Vector3(.10f,height*.82f,.09f),stone);
                AddBeveledBox(parent,"OffsetRoofLantern",new Vector3(-.34f,height+.28f,.18f),new Vector3(width*.48f,.65f,depth*.58f),CityGlass);
                crownWidth*=.88f;crownDepth*=.86f;
            }
            else if(kind==4)
            {
                // A round belvedere and lantern break the square tower skyline.
                AddBeveledBox(parent,"BelvedereStoneShaft",new Vector3(0,(height+1.05f)*.5f,0),new Vector3(width*.85f,height-1.05f,depth*.85f),stone);
                AddPrimitive(PrimitiveType.Cylinder,parent,"CircularBelvedere",new Vector3(0,height+.34f,0),new Vector3(width*.69f,.36f,depth*.69f),CityLimestone);
                AddPrimitive(PrimitiveType.Cylinder,parent,"BelvedereRoofDisc",new Vector3(0,height+.75f,0),new Vector3(width*.80f,.06f,depth*.80f),CityBronze);
                for(var post=0;post<8;post++){var angle=post*Mathf.PI/4;AddCube(parent,"BelvedereLanternPier",new Vector3(Mathf.Sin(angle)*width*.25f,height+.38f,Mathf.Cos(angle)*depth*.25f),new Vector3(.075f,.64f,.075f),CityBronze);}
                roof+=.76f;crownWidth*=.78f;crownDepth*=.78f;
            }
            else
            {
                // Broad stepped terraces provide a third street scale rather than another slab stack.
                AddBeveledBox(parent,"TerracedLowerWing",new Vector3(-.27f,height*.36f,0),new Vector3(width,height*.48f,depth),stone);
                AddBeveledBox(parent,"TerracedMiddleWing",new Vector3(.27f,height*.61f,.23f),new Vector3(width*.76f,height*.43f,depth*.74f),stone);
                AddBeveledBox(parent,"TerracedUpperWing",new Vector3(.50f,height*.84f,.33f),new Vector3(width*.53f,height*.34f,depth*.56f),stone);
                AddCube(parent,"TerraceFrontBalustrade",new Vector3(-.20f,height*.60f,-depth*.48f),new Vector3(width*.89f,.22f,.05f),CityMetal);
                crownWidth*=.65f;crownDepth*=.62f;
            }
            // Windows are physical recessed bays with stone framing; no repeated per-floor solid slabs.
            for(var floor=0;floor<floors;floor++)
            {
                var y=1.45f+floor*.61f;if(y>height-.20f)break;
                var setback=kind==0&&y>shoulder?.77f:kind==5?(y>height*.825f?.53f:y>height*.60f?.76f:1f):kind==3||kind==4?.84f:1f;
                var cx=kind==5?(y>height*.825f?.50f:y>height*.60f?.27f:-.27f):0;
                var cz=kind==5?(y>height*.825f?.33f:y>height*.60f?.23f:0):kind==0&&y>shoulder?.14f:0;
                CityWindowBays(parent,floor,slot,y,width*setback,depth*setback,cx,cz,kind);
            }
            AddBeveledBox(parent,"RoofCrown",new Vector3(kind==5?.5f:0,roof+.09f,kind==5?.33f:0),new Vector3(crownWidth,.17f,crownDepth),CityMetal);
            AddCube(parent,"RecessedCrownLight",new Vector3(kind==5?.5f:0,roof+.185f,kind==5?.33f:0),new Vector3(crownWidth*.78f,.025f,crownDepth*.78f),accent);
            AddBeveledBox(parent,"RoofMechanicalCore",new Vector3(.30f,roof+.33f,.32f),new Vector3(.62f,.41f,.68f),stone);
            AddCube(parent,"RooftopVent",new Vector3(.30f,roof+.56f,.32f),new Vector3(.48f,.06f,.53f),CityMetal);
            for(var star=0;star<stars;star++)AddCube(parent,"StarAntenna"+star,new Vector3((star-1)*.37f,roof+.94f,0),new Vector3(.045f,.56f,.045f),accent);
        }

        private static void CityWindowBays(Transform parent,int floor,int slot,float y,float width,float depth,float cx,float cz,int kind)
        {
            var bays=kind==1?4:3;
            for(var bay=0;bay<bays;bay++)
            {
                if(kind==2&&bay==1)continue; // Real courtyard opening is kept clear.
                if(kind==1&&(bay==1||bay==2))continue;
                var glass=(floor*3+bay+slot)%7==0?WarmWindow:CityGlass;
                var x=cx+((bay+.5f)/bays-.5f)*width*.82f;var z=cz+((bay+.5f)/bays-.5f)*depth*.82f;
                foreach(var side in new[]{-1,1})
                {
                    var face=cz+(kind==1?(bay==0?-.14f:.14f):0)+side*depth*(kind==1?.84f:1f)*.505f;
                    AddCube(parent,"FacadeBay"+floor+"_"+bay+"_"+side,new Vector3(x,y,face),new Vector3(width*.62f/bays,.39f,.028f),glass);
                    if(floor%2==0)AddCube(parent,"StoneWindowSill",new Vector3(x,y-.23f,face+side*.025f),new Vector3(width*.69f/bays,.050f,.065f),CityLimestone);
                    var sideFace=cx+side*width*(kind==1?.485f:kind==2?.46f:.505f);
                    AddCube(parent,"ReturnWindowBay",new Vector3(sideFace,y,z),new Vector3(.028f,.39f,depth*.62f/bays),glass);
                }
            }
        }
        private static void CityTree(Transform parent,Vector3 point,float size,int variant)
        {
            AddPrimitive(PrimitiveType.Cylinder,parent,"RoundedTreeTrunk",point+Vector3.up*size*.48f,new Vector3(size*.10f,size*.48f,size*.10f),RunnerHair);
            for(var crown=0;crown<3;crown++)
            {
                var offset=new Vector3((crown-1)*size*.22f,size*(1.04f+(crown%2)*.24f),(crown%2==0?.08f:-.13f)*size);
                AddPrimitive(PrimitiveType.Sphere,parent,"RoundedLeafCrown",point+offset,new Vector3(size*.79f,size*.92f,size*.77f),((variant+crown)&1)==0?Garden:CityLeaf);
            }
        }
        private static void CityLamp(Transform parent,Vector3 point,int district)
        {
            AddPrimitive(PrimitiveType.Cylinder,parent,"PromenadeLampBase",point+Vector3.up*.08f,new Vector3(.18f,.08f,.18f),CityBronze);
            AddCube(parent,"PromenadeLampStem",point+Vector3.up*.78f,new Vector3(.07f,1.55f,.07f),CityMetal);
            AddBeveledBox(parent,"LanternCase",point+Vector3.up*1.56f,new Vector3(.26f,.32f,.26f),CityBronze);
            AddCube(parent,"WarmLanternGlass",point+Vector3.up*1.57f,new Vector3(.19f,.22f,.28f),WarmWindow);
            AddBeveledBox(parent,"LanternRoof",point+Vector3.up*1.77f,new Vector3(.34f,.08f,.34f),CityMetal);
        }
        private static void CityBench(Transform parent,Vector3 point)
        {
            for(var slat=0;slat<3;slat++)AddCube(parent,"ParkBenchSlat",point+new Vector3(0,.31f,-.15f+slat*.15f),new Vector3(.85f,.045f,.11f),CityBronze);
            AddCube(parent,"ParkBenchBack",point+new Vector3(0,.50f,.21f),new Vector3(.86f,.28f,.045f),CityBronze);
            foreach(var leg in new[]{-.30f,.30f})AddCube(parent,"ParkBenchLeg",point+new Vector3(leg,.15f,0),new Vector3(.07f,.30f,.35f),CityMetal);
        }
        private static void CityFountain(Transform parent,Vector3 point,Material accent)
        {
            AddPrimitive(PrimitiveType.Cylinder,parent,"PocketPlazaFountain",point+Vector3.up*.12f,new Vector3(1.02f,.13f,1.02f),CityLimestone);
            AddPrimitive(PrimitiveType.Cylinder,parent,"FountainWater",point+Vector3.up*.25f,new Vector3(.84f,.012f,.84f),CityGlass);
            AddPrimitive(PrimitiveType.Cylinder,parent,"FountainCenter",point+Vector3.up*.37f,new Vector3(.15f,.14f,.15f),CityBronze);
        }
        private static void BakeBuilding(GameObject building)
        {
            // An already owned batch is never baked into itself or duplicated in a parent batch.
            if(building.GetComponent<CityMeshOwner>()!=null)return;
            var groups=new Dictionary<Material,List<CombineInstance>>();
            foreach(var renderer in building.GetComponentsInChildren<MeshRenderer>())
            {
                if(!renderer.enabled||!renderer.gameObject.activeSelf||renderer.GetComponentInParent<CityMeshOwner>()!=null)continue;
                var filter=renderer.GetComponent<MeshFilter>();if(filter==null||filter.sharedMesh==null||renderer.sharedMaterial==null)continue;
                var material=renderer.sharedMaterial;
                if(!groups.TryGetValue(material,out var instances))groups[material]=instances=new List<CombineInstance>();
                instances.Add(new CombineInstance{mesh=filter.sharedMesh,transform=building.transform.worldToLocalMatrix*filter.transform.localToWorldMatrix});renderer.enabled=false;
            }
            if(groups.Count==0)return;
            var owner=building.AddComponent<CityMeshOwner>();
            foreach(var pair in groups)
            {
                var mesh=new Mesh{name=building.name+"_"+pair.Key.name};int vertices=0;foreach(var instance in pair.Value)vertices+=instance.mesh.vertexCount;
                if(vertices>65000)mesh.indexFormat=UnityEngine.Rendering.IndexFormat.UInt32;
                mesh.CombineMeshes(pair.Value.ToArray(),true,true);owner.Meshes.Add(mesh);
                var go=new GameObject("BakedFacade_"+pair.Key.name,typeof(MeshFilter),typeof(MeshRenderer));go.transform.SetParent(building.transform,false);
                go.GetComponent<MeshFilter>().sharedMesh=mesh;go.GetComponent<MeshRenderer>().sharedMaterial=pair.Key;
            }
        }
        public static void MarkSpecialBlock(GameObject block,Kamilunavo.PerfectDrop.Gameplay.StackBlockKind kind)
        {
            if(kind==Kamilunavo.PerfectDrop.Gameplay.StackBlockKind.Standard)return;
            var color=kind==Kamilunavo.PerfectDrop.Gameplay.StackBlockKind.Bonus?new Color(1f,.65f,.12f):
                kind==Kamilunavo.PerfectDrop.Gameplay.StackBlockKind.Fragile?new Color(.9f,.25f,.65f):
                kind==Kamilunavo.PerfectDrop.Gameplay.StackBlockKind.Drift?new Color(.15f,.75f,1f):new Color(.2f,.9f,.5f);
            var plate=block.transform.Find("TopPlate").GetComponent<Renderer>();
            var tint=new MaterialPropertyBlock();plate.GetPropertyBlock(tint);tint.SetColor("_Color",color);tint.SetColor("_EmissionColor",color*.075f);tint.SetFloat("_GoldFinish",.04f);plate.SetPropertyBlock(tint);
        }
        public static void StyleStackBlock(GameObject block,int style)
        {
            var finish=StackStylePalette.Get(style);
            foreach(var renderer in block.GetComponentsInChildren<Renderer>())
            {
                var paint=new MaterialPropertyBlock();renderer.GetPropertyBlock(paint);
                if(renderer.name=="MetalDeck" || renderer.name=="DeckCrown")
                {
                    var body=style==0&&renderer.name=="MetalDeck"?paint.GetColor("_DeckBaseColor"):finish.Body;
                    paint.SetColor("_Color",body);paint.SetFloat("_Metallic",finish.Metallic);paint.SetFloat("_Glossiness",finish.Smoothness);
                    paint.SetFloat("_BodyFinish",style==3?1:style==5?2:0);
                }
                else if(renderer.name=="TopPlate")
                {
                    paint.SetColor("_Color",finish.Plate);paint.SetColor("_EmissionColor",finish.Plate*.075f);
                    paint.SetFloat("_GoldFinish",finish.GoldFinish);paint.SetFloat("_Metallic",.62f);paint.SetFloat("_Glossiness",.76f);
                }
                else if(renderer.name.StartsWith("Gold"))
                {
                    paint.SetColor("_Color",finish.Accent);paint.SetColor("_EmissionColor",finish.Accent*1.85f);
                }
                else continue;
                renderer.SetPropertyBlock(paint);
            }
        }
        private static GameObject AddCube(Transform parent, string name, Vector3 localPosition, Vector3 localScale, Material material)
        {
            return AddPrimitive(PrimitiveType.Cube, parent, name, localPosition, localScale, material);
        }

        private static GameObject AddBeveledBox(Transform parent, string name, Vector3 localPosition, Vector3 localScale, Material material)
        {
            var go = new GameObject(name, typeof(MeshFilter), typeof(MeshRenderer));
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPosition;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = localScale;
            go.GetComponent<MeshFilter>().sharedMesh = BeveledBoxMesh;
            go.GetComponent<MeshRenderer>().sharedMaterial = material;
            return go;
        }

        private static Mesh BeveledBoxMesh => _beveledBoxMesh != null ? _beveledBoxMesh : (_beveledBoxMesh = BuildBeveledBoxMesh());

        private static Mesh BuildBeveledBoxMesh()
        {
            var vertices = new List<Vector3>(); var triangles = new List<int>(); var uvs = new List<Vector2>();
            Vector2[] Ring(float extent)
            {
                const float radius=.065f;
                var ring=new Vector2[20];
                for(var corner=0;corner<4;corner++)
                {
                    var angle=-135f+corner*90f;
                    var center=new Vector2(corner==0||corner==3?-extent+radius:extent-radius,corner<2?-extent+radius:extent-radius);
                    for(var step=0;step<5;step++)
                    {
                        var a=(angle-45f+step*22.5f)*Mathf.Deg2Rad;
                        ring[corner*5+step]=center+new Vector2(Mathf.Cos(a),Mathf.Sin(a))*radius;
                    }
                }
                return ring;
            }
            var outer=Ring(.5f); var inner=Ring(.465f);
            void Face(Vector3 a,Vector3 b,Vector3 c,Vector3 d)
            {
                var start=vertices.Count; vertices.AddRange(new[]{a,b,c,d});
                uvs.AddRange(new[]{new Vector2(0,1),new Vector2(1,1),new Vector2(1,0),new Vector2(0,0)});
                triangles.AddRange(new[]{start,start+1,start+2,start,start+2,start+3});
            }
            Vector3 Point(Vector2 v,float y) => new Vector3(v.x,y,v.y);
            for(var i=0;i<outer.Length;i++)
            {
                var n=(i+1)%outer.Length;
                Face(Point(inner[i],.5f),Point(inner[n],.5f),Point(outer[n],.38f),Point(outer[i],.38f));
                Face(Point(outer[i],.38f),Point(outer[n],.38f),Point(outer[n],-.38f),Point(outer[i],-.38f));
                Face(Point(outer[i],-.38f),Point(outer[n],-.38f),Point(inner[n],-.5f),Point(inner[i],-.5f));
            }
            for(var side=0;side<2;side++)
            {
                var start=vertices.Count;var y=side==0?.5f:-.5f;
                foreach(var v in inner){vertices.Add(Point(v,y));uvs.Add(v+Vector2.one*.5f);}
                for(var i=1;i<inner.Length-1;i++)triangles.AddRange(side==0?new[]{start,start+i+1,start+i}:new[]{start,start+i,start+i+1});
            }
            var mesh=new Mesh{name="PerfectDropBeveledBox",hideFlags=HideFlags.HideAndDontSave};
            mesh.SetVertices(vertices);mesh.SetTriangles(triangles,0);mesh.SetUVs(0,uvs);mesh.RecalculateNormals();mesh.RecalculateBounds();return mesh;
        }

        private static Mesh BuildInsetFrame()
        {
            var cube=Resources.GetBuiltinResource<Mesh>("Cube.fbx");
            var parts=new CombineInstance[4];
            var points=new[]{new Vector3(0,0,-.392f),new Vector3(0,0,.392f),new Vector3(-.392f,0,0),new Vector3(.392f,0,0)};
            for(var i=0;i<4;i++)parts[i]=new CombineInstance{mesh=cube,transform=Matrix4x4.TRS(points[i],Quaternion.identity,i<2?new Vector3(.796f,1,.012f):new Vector3(.012f,1,.796f))};
            var mesh=new Mesh{name="DeckInsetFrame",hideFlags=HideFlags.HideAndDontSave};mesh.CombineMeshes(parts,true,true);return mesh;
        }

        private static Cubemap BuildStudioReflection()
        {
            const int size=32;
            var map=new Cubemap(size,TextureFormat.RGBAHalf,true){name="CloudCityMetalReflection",hideFlags=HideFlags.HideAndDontSave};
            var sun=new Vector3(-.55f,.4f,.7f).normalized;
            for(var face=0;face<6;face++)
            {
                var pixels=new Color[size*size];
                for(var y=0;y<size;y++)for(var x=0;x<size;x++)
                {
                    var u=(x+.5f)/size*2-1;var v=(y+.5f)/size*2-1;
                    var dir=face switch {0=>new Vector3(1,-v,-u),1=>new Vector3(-1,-v,u),2=>new Vector3(u,1,v),3=>new Vector3(u,-1,-v),4=>new Vector3(u,-v,1),_=>new Vector3(-u,-v,-1)};
                    dir.Normalize();
                    var sky=Color.Lerp(new Color(.24f,.19f,.29f),new Color(.53f,.65f,.9f),Mathf.Clamp01(dir.y*.6f+.45f));
                    var glow=Mathf.Pow(Mathf.Max(0,Vector3.Dot(dir,sun)),12);
                    pixels[y*size+x]=sky+new Color(1.1f,.64f,.26f)*glow;
                }
                map.SetPixels(pixels,(CubemapFace)face);
            }
            map.Apply(true,true);return map;
        }

        private static GameObject AddPrimitive(PrimitiveType type, Transform parent, string name, Vector3 localPosition, Vector3 localScale, Material material)
        {
            var go = new GameObject(name, typeof(MeshFilter), typeof(MeshRenderer));
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPosition;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = localScale;

            var mesh = Resources.GetBuiltinResource<Mesh>(BuiltinMeshName(type));
            if (mesh == null)
                throw new System.InvalidOperationException($"Built-in mesh for {type} is unavailable.");

            go.GetComponent<MeshFilter>().sharedMesh = mesh;
            go.GetComponent<MeshRenderer>().sharedMaterial = material;
            return go;
        }

        private static string BuiltinMeshName(PrimitiveType type)
        {
            return type switch
            {
                PrimitiveType.Sphere => "Sphere.fbx",
                PrimitiveType.Capsule => "Capsule.fbx",
                PrimitiveType.Cylinder => "Cylinder.fbx",
                PrimitiveType.Plane => "Plane.fbx",
                PrimitiveType.Quad => "Quad.fbx",
                _ => "Cube.fbx"
            };
        }

        private static Material CreateMaterial(string name, Color color, float metallic, float smoothness, Color? emission = null)
        {
            var shader = Resources.Load<Shader>("PerfectDropSurface");
            if (shader == null) shader = Shader.Find("Kamilunavo/PerfectDropSurface");
            if (shader == null) throw new System.InvalidOperationException("PerfectDropSurface shader is missing from Resources.");

            var material = new Material(shader);
            material.name = name;
            material.hideFlags = HideFlags.HideAndDontSave;
            material.enableInstancing = true;
            if(name=="Platform" || name=="PlatformTop" || name=="Skyline" || name=="GoldPlate")
            {
                var texture=Resources.Load<Texture2D>("Art/GunmetalPanels");
                if(texture!=null)material.SetTexture("_MainTex",texture);
            }

            if (material.HasProperty("_Color")) material.SetColor("_Color", color);
            if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", color);
            if (material.HasProperty("_Metallic")) material.SetFloat("_Metallic", metallic);
            if (material.HasProperty("_Detail")) material.SetFloat("_Detail", name == "PlatformTop" ? 1f : 0f);
            if (material.HasProperty("_BrushFinish")) material.SetFloat("_BrushFinish", name == "Platform" || name == "PlatformTop" || name == "GoldPlate" ? 1f : 0f);
            if (material.HasProperty("_GoldFinish")) material.SetFloat("_GoldFinish", name == "GoldPlate" ? 1f : 0f);
            if (material.HasProperty("_Glossiness")) material.SetFloat("_Glossiness", smoothness);
            if (material.HasProperty("_Smoothness")) material.SetFloat("_Smoothness", smoothness);

            if (emission.HasValue && material.HasProperty("_EmissionColor"))
            {
                material.EnableKeyword("_EMISSION");
                material.SetColor("_EmissionColor", emission.Value);
            }

            return material;
        }
    }
}
