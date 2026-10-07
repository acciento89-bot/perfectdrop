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
        private static Mesh _beveledBoxMesh;

        public static Material Platform => _platform != null ? _platform : (_platform = CreateMaterial("Platform", new Color(0.030f, 0.042f, 0.065f), 0.68f, 0.34f));
        public static Material PlatformTop => _platformTop != null ? _platformTop : (_platformTop = CreateMaterial("PlatformTop", new Color(0.22f, 0.26f, 0.34f), 0.22f, 0.50f));
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

            var material = new Material(shader)
            {
                name = "PerfectDropSkybox",
                hideFlags = HideFlags.HideAndDontSave
            };
            RenderSettings.skybox = material;
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.19f, 0.24f, 0.38f);
            RenderSettings.ambientEquatorColor = new Color(0.21f, 0.15f, 0.20f);
            RenderSettings.ambientGroundColor = new Color(0.055f, 0.045f, 0.065f);
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
            var body = AddBeveledBox(root.transform, "MetalDeck", Vector3.zero, size, Platform);
            var tint = new MaterialPropertyBlock();
            tint.SetColor("_Color", Color.Lerp(new Color(.07f,.12f,.23f), new Color(.25f,.16f,.32f), level/30f));
            body.GetComponent<Renderer>().SetPropertyBlock(tint);
            AddBeveledBox(root.transform, "TopPlate", new Vector3(0,size.y*.53f,0), new Vector3(size.x*.96f,size.y*.10f,size.z*.96f), PlatformTop);
            var strip = Mathf.Min(.028f, Mathf.Min(size.x,size.z)*.08f);
            AddCube(root.transform,"GoldFront",new Vector3(0,size.y*.59f,-size.z*.46f),new Vector3(size.x*.96f,.035f,strip),Gold);
            AddCube(root.transform,"GoldBack",new Vector3(0,size.y*.59f,size.z*.46f),new Vector3(size.x*.96f,.035f,strip),Gold);
            AddCube(root.transform,"GoldLeft",new Vector3(-size.x*.46f,size.y*.59f,0),new Vector3(strip,.035f,size.z*.96f),Gold);
            AddCube(root.transform,"GoldRight",new Vector3(size.x*.46f,size.y*.59f,0),new Vector3(strip,.035f,size.z*.96f),Gold);
            return root;
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
            const float x = 0.5f;
            const float z = 0.5f;
            const float y = 0.5f;
            const float c = 0.085f;
            var ring = new[]
            {
                new Vector2(-x + c, -z), new Vector2(x - c, -z),
                new Vector2(x, -z + c), new Vector2(x, z - c),
                new Vector2(x - c, z), new Vector2(-x + c, z),
                new Vector2(-x, z - c), new Vector2(-x, -z + c)
            };

            var vertices = new List<Vector3>(48);
            var triangles = new List<int>(84);
            var uvs = new List<Vector2>(48);

            // Top and bottom caps use separate vertices so the edge keeps a crisp authored silhouette.
            for (var i = 0; i < 8; i++)
            {
                vertices.Add(new Vector3(ring[i].x, y, ring[i].y));
                uvs.Add(new Vector2(ring[i].x + 0.5f, ring[i].y + 0.5f));
            }
            for (var i = 0; i < 8; i++)
            {
                vertices.Add(new Vector3(ring[i].x, -y, ring[i].y));
                uvs.Add(new Vector2(ring[i].x + 0.5f, ring[i].y + 0.5f));
            }

            for (var i = 1; i < 7; i++)
            {
                triangles.Add(0); triangles.Add(i + 1); triangles.Add(i);
                triangles.Add(8); triangles.Add(8 + i); triangles.Add(8 + i + 1);
            }

            for (var i = 0; i < 8; i++)
            {
                var next = (i + 1) % 8;
                var baseIndex = vertices.Count;
                vertices.Add(new Vector3(ring[i].x, y, ring[i].y));
                vertices.Add(new Vector3(ring[next].x, y, ring[next].y));
                vertices.Add(new Vector3(ring[next].x, -y, ring[next].y));
                vertices.Add(new Vector3(ring[i].x, -y, ring[i].y));
                uvs.Add(new Vector2(0f, 1f));
                uvs.Add(new Vector2(1f, 1f));
                uvs.Add(new Vector2(1f, 0f));
                uvs.Add(new Vector2(0f, 0f));
                triangles.Add(baseIndex); triangles.Add(baseIndex + 1); triangles.Add(baseIndex + 2);
                triangles.Add(baseIndex); triangles.Add(baseIndex + 2); triangles.Add(baseIndex + 3);
            }

            var mesh = new Mesh { name = "PerfectDropBeveledBox", hideFlags = HideFlags.HideAndDontSave };
            mesh.SetVertices(vertices);
            mesh.SetTriangles(triangles, 0);
            mesh.SetUVs(0, uvs);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
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
