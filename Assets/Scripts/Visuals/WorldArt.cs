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
        private static Mesh _beveledBoxMesh;
        private static Mesh _insetFrameMesh;
        private static Cubemap _studioReflection;
        private static Material _skyMaterial;
        public static int ActiveChapter { get; private set; }

        public static Material Platform => _platform != null ? _platform : (_platform = CreateMaterial("Platform", new Color(0.15f, 0.19f, 0.28f), 0.48f, 0.46f));
        public static Material PlatformTop => _platformTop != null ? _platformTop : (_platformTop = CreateMaterial("PlatformTop", new Color(0.55f, 0.62f, 0.73f), 0.38f, 0.53f));
        public static Material PlatformInset => _platformInset != null ? _platformInset : (_platformInset = CreateMaterial("PlatformInset", new Color(0.018f, 0.025f, 0.040f), 0.35f, 0.22f));
        public static Material Gold => _gold != null ? _gold : (_gold = CreateMaterial("SignalGold", new Color(1f, 0.48f, 0.035f), 0.24f, 0.76f, new Color(1.85f, 0.58f, 0.035f)));
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
            pulse.AddComponent<StackLandingPulse>().Initialize(StyleColor(style));
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
            var body = AddBeveledBox(root.transform, "MetalDeck", new Vector3(0,-size.y*.09f,0),new Vector3(size.x,size.y*.82f,size.z), Platform);
            var tint = new MaterialPropertyBlock();
            tint.SetColor("_Color", Color.Lerp(new Color(.28f,.34f,.46f), new Color(.38f,.28f,.40f), Mathf.Clamp01(level/30f)));
            body.GetComponent<Renderer>().SetPropertyBlock(tint);
            AddBeveledBox(root.transform,"Undercore",new Vector3(0,-size.y*.43f,0),new Vector3(size.x*.91f,size.y*.16f,size.z*.91f),PlatformInset);
            AddBeveledBox(root.transform, "TopPlate", new Vector3(0,size.y*.508f,0), new Vector3(size.x*.94f,size.y*.018f,size.z*.94f), PlatformTop);
            // A continuous band replaces the corresponding body slice. It stays
            // inside the logical footprint even after a very narrow overhang cut.
            AddBeveledBox(root.transform,"GoldBand",new Vector3(0,size.y*.36f,0),new Vector3(size.x,size.y*.105f,size.z),Gold);
            AddBeveledBox(root.transform,"DeckCrown",new Vector3(0,size.y*.455f,0),new Vector3(size.x,size.y*.09f,size.z),Platform);
            var inset=new GameObject("GoldInset",typeof(MeshFilter),typeof(MeshRenderer));
            inset.transform.SetParent(root.transform,false);inset.transform.localPosition=new Vector3(0,size.y*.538f,0);
            inset.transform.localScale=new Vector3(size.x,.012f,size.z);
            if(_insetFrameMesh==null)_insetFrameMesh=BuildInsetFrame();
            inset.GetComponent<MeshFilter>().sharedMesh=_insetFrameMesh;inset.GetComponent<MeshRenderer>().sharedMaterial=Gold;
            return root;
        }

        public static GameObject BuildPlayerCity(Transform parent,StackProfile profile)
        {
            var root=new GameObject("OwnedCity");root.transform.SetParent(parent,false);root.transform.localPosition=new Vector3(0,-4,42);
            for(var district=0;district<3;district++)
            {
                var group=new GameObject("CityDistrict"+district);group.transform.SetParent(root.transform,false);
                var cityParent=group.transform;
                var origin=new Vector3((district-1)*24,0,0);
                var platform=CreateStackBlock(cityParent,"District"+district,origin,new Vector3(19,.5f,28),1+district*10);
                StyleStackBlock(platform,district==0?0:district==1?1:2);
                AddBeveledBox(cityParent,"DistrictFoundation"+district,origin+Vector3.down*.75f,new Vector3(18.2f,1.2f,27.2f),Platform);
                AddCube(cityParent,"Walkway"+district,origin+new Vector3(0,.29f,0),new Vector3(1.4f,.025f,24),PlatformInset);
                for(var slot=0;slot<10;slot++)
                {
                    var id=district*10+slot;var point=origin+new Vector3((slot%2==0?-4:4),.35f,(slot/2-2)*5f);
                    AddBeveledBox(cityParent,"Plot"+(id+1),point,new Vector3(3.5f,.16f,3.5f),PlatformTop);
                    if(profile.LevelStars[id]==0)continue;
                    var building=new GameObject("CityTower"+(id+1));building.transform.SetParent(cityParent,false);building.transform.localPosition=point;
                    var floors=3+Kamilunavo.PerfectDrop.Gameplay.StackCampaign.Level(id+1).Target/5;
                    for(var floor=0;floor<floors;floor++)
                    {
                        var piece=CreateStackBlock(building.transform,"CityFloor"+floor,new Vector3(0,.45f+floor*.85f,0),new Vector3(2.7f,.8f,2.7f),id+1);
                        StyleStackBlock(piece,district==0?0:district==1?1:2);
                    }
                    AddBeveledBox(building.transform,"RoofCrown",new Vector3(0,floors*.85f+.18f,0),new Vector3(2.35f,.24f,2.35f),PlatformTop);
                    AddCube(building.transform,"FacadeSignal",new Vector3(-1.36f,floors*.425f,-.65f),new Vector3(.035f,floors*.69f,.16f),district==0?Gold:Window);
                    for(var star=0;star<profile.LevelStars[id];star++)
                        AddCube(building.transform,"StarAntenna"+star,new Vector3((star-1)*.5f,floors*.85f+.5f,0),new Vector3(.09f,.8f,.09f),Gold);
                }
            }
            return root;
        }
        public static void MarkSpecialBlock(GameObject block,Kamilunavo.PerfectDrop.Gameplay.StackBlockKind kind)
        {
            if(kind==Kamilunavo.PerfectDrop.Gameplay.StackBlockKind.Standard)return;
            var color=kind==Kamilunavo.PerfectDrop.Gameplay.StackBlockKind.Bonus?new Color(1f,.65f,.12f):
                kind==Kamilunavo.PerfectDrop.Gameplay.StackBlockKind.Fragile?new Color(.9f,.25f,.65f):
                kind==Kamilunavo.PerfectDrop.Gameplay.StackBlockKind.Drift?new Color(.15f,.75f,1f):new Color(.2f,.9f,.5f);
            var plate=block.transform.Find("TopPlate").GetComponent<Renderer>();
            var tint=new MaterialPropertyBlock();tint.SetColor("_Color",color*.5f);tint.SetColor("_EmissionColor",color*.15f);plate.SetPropertyBlock(tint);
        }
        private static Color StyleColor(int style) => style==1?new Color(.08f,.8f,1f):style==2?new Color(1f,.18f,.55f):style==3?new Color(.2f,1f,.5f):new Color(1f,.55f,.06f);
        public static void StyleStackBlock(GameObject block,int style)
        {
            var color=StyleColor(style);
            foreach(var renderer in block.GetComponentsInChildren<Renderer>())
            {
                if(!renderer.name.StartsWith("Gold")) continue;
                var tint=new MaterialPropertyBlock(); tint.SetColor("_Color",color); tint.SetColor("_EmissionColor",color*1.85f); renderer.SetPropertyBlock(tint);
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
            Vector2[] Ring(float extent) => new[] {
                new Vector2(-extent+.055f,-extent),new Vector2(extent-.055f,-extent),
                new Vector2(extent,-extent+.055f),new Vector2(extent,extent-.055f),
                new Vector2(extent-.055f,extent),new Vector2(-extent+.055f,extent),
                new Vector2(-extent,extent-.055f),new Vector2(-extent,-extent+.055f)};
            var outer=Ring(.5f); var inner=Ring(.465f);
            void Face(Vector3 a,Vector3 b,Vector3 c,Vector3 d)
            {
                var start=vertices.Count; vertices.AddRange(new[]{a,b,c,d});
                uvs.AddRange(new[]{new Vector2(0,1),new Vector2(1,1),new Vector2(1,0),new Vector2(0,0)});
                triangles.AddRange(new[]{start,start+1,start+2,start,start+2,start+3});
            }
            Vector3 Point(Vector2 v,float y) => new Vector3(v.x,y,v.y);
            for(var i=0;i<8;i++)
            {
                var n=(i+1)%8;
                Face(Point(inner[i],.5f),Point(inner[n],.5f),Point(outer[n],.38f),Point(outer[i],.38f));
                Face(Point(outer[i],.38f),Point(outer[n],.38f),Point(outer[n],-.38f),Point(outer[i],-.38f));
                Face(Point(outer[i],-.38f),Point(outer[n],-.38f),Point(inner[n],-.5f),Point(inner[i],-.5f));
            }
            for(var side=0;side<2;side++)
            {
                var start=vertices.Count;var y=side==0?.5f:-.5f;
                foreach(var v in inner){vertices.Add(Point(v,y));uvs.Add(v+Vector2.one*.5f);}
                for(var i=1;i<7;i++)triangles.AddRange(side==0?new[]{start,start+i+1,start+i}:new[]{start,start+i,start+i+1});
            }
            var mesh=new Mesh{name="PerfectDropBeveledBox",hideFlags=HideFlags.HideAndDontSave};
            mesh.SetVertices(vertices);mesh.SetTriangles(triangles,0);mesh.SetUVs(0,uvs);mesh.RecalculateNormals();mesh.RecalculateBounds();return mesh;
        }

        private static Mesh BuildInsetFrame()
        {
            var cube=Resources.GetBuiltinResource<Mesh>("Cube.fbx");
            var parts=new CombineInstance[4];
            var points=new[]{new Vector3(0,0,-.34f),new Vector3(0,0,.34f),new Vector3(-.34f,0,0),new Vector3(.34f,0,0)};
            for(var i=0;i<4;i++)parts[i]=new CombineInstance{mesh=cube,transform=Matrix4x4.TRS(points[i],Quaternion.identity,i<2?new Vector3(.68f,1,.012f):new Vector3(.012f,1,.68f))};
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
            if(name=="Platform" || name=="PlatformTop" || name=="Skyline")
            {
                var texture=Resources.Load<Texture2D>("Art/GunmetalPanels");
                if(texture!=null)material.SetTexture("_MainTex",texture);
            }

            if (material.HasProperty("_Color")) material.SetColor("_Color", color);
            if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", color);
            if (material.HasProperty("_Metallic")) material.SetFloat("_Metallic", metallic);
            if (material.HasProperty("_Detail")) material.SetFloat("_Detail", name == "PlatformTop" ? 1f : 0f);
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
