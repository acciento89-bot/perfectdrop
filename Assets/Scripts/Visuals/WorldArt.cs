using UnityEngine;

namespace Kamilunavo.PerfectDrop.Visuals
{
    public static class WorldArt
    {
        private static Material _platform;
        private static Material _platformTop;
        private static Material _gold;
        private static Material _cyan;
        private static Material _tower;
        private static Material _window;
        private static Material _runnerDark;
        private static Material _runnerLight;

        public static Material Platform => _platform != null ? _platform : (_platform = CreateMaterial("Platform", new Color(0.045f, 0.07f, 0.105f), 0.72f, 0.36f));
        public static Material PlatformTop => _platformTop != null ? _platformTop : (_platformTop = CreateMaterial("PlatformTop", new Color(0.085f, 0.12f, 0.16f), 0.52f, 0.48f));
        public static Material Gold => _gold != null ? _gold : (_gold = CreateMaterial("SignalGold", new Color(1f, 0.54f, 0.055f), 0.35f, 0.78f, new Color(1.7f, 0.55f, 0.035f)));
        public static Material Cyan => _cyan != null ? _cyan : (_cyan = CreateMaterial("PrecisionCyan", new Color(0.08f, 0.78f, 0.95f), 0.22f, 0.76f, new Color(0.03f, 0.9f, 1.45f)));
        public static Material Tower => _tower != null ? _tower : (_tower = CreateMaterial("Skyline", new Color(0.025f, 0.04f, 0.075f), 0.82f, 0.23f));
        public static Material Window => _window != null ? _window : (_window = CreateMaterial("Window", new Color(0.18f, 0.58f, 0.74f), 0.15f, 0.82f, new Color(0.05f, 0.55f, 0.9f)));
        public static Material RunnerDark => _runnerDark != null ? _runnerDark : (_runnerDark = CreateMaterial("RunnerDark", new Color(0.035f, 0.045f, 0.065f), 0.68f, 0.42f));
        public static Material RunnerLight => _runnerLight != null ? _runnerLight : (_runnerLight = CreateMaterial("RunnerLight", new Color(0.79f, 0.86f, 0.94f), 0.28f, 0.52f));

        public static void DecoratePlatform(Transform platform, int index)
        {
            if (platform == null) return;
            var renderer = platform.GetComponent<Renderer>();
            if (renderer != null) renderer.sharedMaterial = Platform;

            AddCube(platform, "TopPlate", new Vector3(0f, 0.53f, 0f), new Vector3(0.96f, 0.10f, 0.95f), PlatformTop);
            AddCube(platform, "TrimFront", new Vector3(0f, 0.57f, -0.49f), new Vector3(0.97f, 0.075f, 0.025f), Gold);
            AddCube(platform, "TrimBack", new Vector3(0f, 0.57f, 0.49f), new Vector3(0.97f, 0.075f, 0.025f), Gold);
            AddCube(platform, "TrimLeft", new Vector3(-0.49f, 0.57f, 0f), new Vector3(0.025f, 0.075f, 0.94f), Gold);
            AddCube(platform, "TrimRight", new Vector3(0.49f, 0.57f, 0f), new Vector3(0.025f, 0.075f, 0.94f), Gold);

            if ((index + 1) % 5 == 0)
            {
                AddCube(platform, "MilestoneLeft", new Vector3(-0.43f, 1.25f, 0.38f), new Vector3(0.055f, 1.25f, 0.055f), Gold);
                AddCube(platform, "MilestoneRight", new Vector3(0.43f, 1.25f, 0.38f), new Vector3(0.055f, 1.25f, 0.055f), Gold);
            }
        }

        public static void CreateLandingBay(Transform platform, float halfWidthWorld, float halfDepthWorld)
        {
            if (platform == null) return;

            var size = platform.lossyScale;
            var halfX = Mathf.Clamp(halfWidthWorld / Mathf.Max(0.01f, size.x), 0.12f, 0.44f);
            var halfZ = Mathf.Clamp(halfDepthWorld / Mathf.Max(0.01f, size.z), 0.12f, 0.44f);
            const float thickness = 0.022f;
            const float y = 0.615f;

            AddCube(platform, "BayFront", new Vector3(0f, y, -halfZ), new Vector3(halfX * 2f, 0.045f, thickness), Cyan);
            AddCube(platform, "BayBack", new Vector3(0f, y, halfZ), new Vector3(halfX * 2f, 0.045f, thickness), Cyan);
            AddCube(platform, "BayLeft", new Vector3(-halfX, y, 0f), new Vector3(thickness, 0.045f, halfZ * 2f), Cyan);
            AddCube(platform, "BayRight", new Vector3(halfX, y, 0f), new Vector3(thickness, 0.045f, halfZ * 2f), Cyan);

            var center = AddCube(platform, "BayCenter", new Vector3(0f, y + 0.01f, 0f), new Vector3(0.055f, 0.035f, 0.055f), Gold);
            center.transform.localRotation = Quaternion.Euler(0f, 45f, 0f);
        }

        public static RunnerVisualAnimator CreateRunnerVisual(Transform root)
        {
            var oldRenderer = root != null ? root.GetComponent<Renderer>() : null;
            if (oldRenderer != null) oldRenderer.enabled = false;

            var visual = new GameObject("RunnerVisual").transform;
            visual.SetParent(root, false);

            var torso = AddCube(visual, "Torso", new Vector3(0f, 1.12f, 0f), new Vector3(0.72f, 0.72f, 0.38f), RunnerDark).transform;
            AddCube(torso, "ChestSignal", new Vector3(0f, 0.06f, 0.52f), new Vector3(0.78f, 0.15f, 0.045f), Gold);
            AddCube(torso, "BackSignal", new Vector3(0f, 0.06f, -0.52f), new Vector3(0.78f, 0.15f, 0.045f), Cyan);

            var head = AddPrimitive(PrimitiveType.Sphere, visual, "Head", new Vector3(0f, 1.72f, 0f), new Vector3(0.54f, 0.54f, 0.54f), RunnerLight).transform;
            AddCube(head, "Visor", new Vector3(0f, 0.02f, 0.49f), new Vector3(0.72f, 0.20f, 0.06f), Cyan);
            AddCube(head, "RearHelmetSignal", new Vector3(0f, 0.02f, -0.49f), new Vector3(0.54f, 0.12f, 0.05f), Gold);

            var leftArm = AddCube(visual, "LeftArm", new Vector3(-0.48f, 1.10f, 0f), new Vector3(0.18f, 0.68f, 0.20f), RunnerDark).transform;
            var rightArm = AddCube(visual, "RightArm", new Vector3(0.48f, 1.10f, 0f), new Vector3(0.18f, 0.68f, 0.20f), RunnerDark).transform;
            var leftLeg = AddCube(visual, "LeftLeg", new Vector3(-0.20f, 0.42f, 0f), new Vector3(0.25f, 0.72f, 0.28f), RunnerDark).transform;
            var rightLeg = AddCube(visual, "RightLeg", new Vector3(0.20f, 0.42f, 0f), new Vector3(0.25f, 0.72f, 0.28f), RunnerDark).transform;

            var animator = root.gameObject.AddComponent<RunnerVisualAnimator>();
            animator.VisualRoot = visual;
            animator.LeftArm = leftArm;
            animator.RightArm = rightArm;
            animator.LeftLeg = leftLeg;
            animator.RightLeg = rightLeg;
            return animator;
        }

        public static void BuildGoalBeacon(Transform parent, Vector3 center)
        {
            if (parent == null) return;
            var beacon = new GameObject("GoalBeacon").transform;
            beacon.SetParent(parent, false);

            const int segments = 28;
            const float radius = 3.1f;
            for (var i = 0; i < segments; i++)
            {
                var angle = i * Mathf.PI * 2f / segments;
                var position = center + new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0f);
                var segment = AddCube(beacon, $"Ring_{i:00}", position, new Vector3(0.42f, 0.42f, 0.24f), Gold);
                segment.transform.localRotation = Quaternion.Euler(0f, 0f, -angle * Mathf.Rad2Deg);
            }

            AddCube(beacon, "CoreVertical", center, new Vector3(0.16f, radius * 1.15f, 0.12f), Cyan);
            AddCube(beacon, "CoreHorizontal", center, new Vector3(radius * 1.15f, 0.16f, 0.12f), Cyan);
        }

        public static void BuildSkyline(Transform parent, int seed = 260907)
        {
            if (parent == null) return;
            var previous = Random.state;
            Random.InitState(seed);

            var skyline = new GameObject("Skyline").transform;
            skyline.SetParent(parent, false);

            for (var i = 0; i < 48; i++)
            {
                var side = i % 2 == 0 ? -1f : 1f;
                var height = Random.Range(9f, 31f);
                var width = Random.Range(2.8f, 6.4f);
                var depth = Random.Range(2.6f, 5.5f);
                var x = side * Random.Range(12f, 22f);
                var z = i * 3.6f + Random.Range(-2f, 2f);
                var y = height * 0.5f - 9f + i * 0.11f;

                var tower = AddCube(skyline, $"Tower_{i:00}", new Vector3(x, y, z), new Vector3(width, height, depth), Tower);
                tower.isStatic = true;

                if (i % 3 == 0)
                {
                    var window = AddCube(skyline, $"Window_{i:00}", new Vector3(x - side * (width * 0.505f), y + height * 0.12f, z), new Vector3(0.05f, Mathf.Min(5f, height * 0.28f), depth * 0.55f), Window);
                    window.isStatic = true;
                }
            }

            Random.state = previous;
        }

        private static GameObject AddCube(Transform parent, string name, Vector3 localPosition, Vector3 localScale, Material material)
        {
            return AddPrimitive(PrimitiveType.Cube, parent, name, localPosition, localScale, material);
        }

        private static GameObject AddPrimitive(PrimitiveType type, Transform parent, string name, Vector3 localPosition, Vector3 localScale, Material material)
        {
            var go = GameObject.CreatePrimitive(type);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPosition;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = localScale;

            var collider = go.GetComponent<Collider>();
            if (collider != null) Object.Destroy(collider);
            var renderer = go.GetComponent<Renderer>();
            if (renderer != null) renderer.sharedMaterial = material;
            return go;
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
