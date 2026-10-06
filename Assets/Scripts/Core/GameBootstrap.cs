using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Kamilunavo.PerfectDrop.CameraSystem;
using Kamilunavo.PerfectDrop.Gameplay;
using Kamilunavo.PerfectDrop.Input;
using Kamilunavo.PerfectDrop.UI;

namespace Kamilunavo.PerfectDrop
{
    public sealed class GameBootstrap : MonoBehaviour
    {
        private static readonly Color Panel = new(0.035f, 0.075f, 0.13f, 0.96f);
        private static readonly Color Gold = new(1f, 0.64f, 0.08f, 1f);
        private static readonly Color TextColor = new(0.97f, 0.985f, 1f, 1f);

        private void Start()
        {
            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 0;
            RenderSettings.fog = true;
            RenderSettings.fogColor = new Color(0.22f, 0.35f, 0.55f);
            RenderSettings.fogDensity = 0.006f;
            EnsureEventSystem();
            CreateLighting();

            var canvas = UiFactory.CreateCanvas();
            var safe = UiFactory.Panel(canvas.transform, "SafeArea", Color.clear, Vector2.zero, Vector2.one);
            safe.gameObject.AddComponent<SafeAreaFitter>();

            var stats = new Text[4];
            var statRects = new RectTransform[4];
            var labels = new[] { "FLOOR\n1", "BEST\n1", "STREAK\nx0", "COINS\n0" };
            for (var i = 0; i < 4; i++)
            {
                var minX = 0.03f + i * 0.242f;
                var panel = UiFactory.Panel(safe, $"Stat_{i}", Panel, new Vector2(minX, 0.90f), new Vector2(minX + 0.215f, 0.975f));
                statRects[i] = panel;
                stats[i] = UiFactory.Label(panel, "Text", labels[i], 34, new Vector2(0.08f, 0.08f), new Vector2(0.92f, 0.92f), TextAnchor.MiddleLeft, TextColor, FontStyle.Bold);
            }

            var objective = UiFactory.Panel(safe, "Objective", Panel, new Vector2(0.03f, 0.79f), new Vector2(0.97f, 0.885f));
            UiFactory.Label(objective, "Title", "PRECISION JUMP", 36, new Vector2(0.04f, 0.50f), new Vector2(0.96f, 0.92f), TextAnchor.MiddleLeft, Gold, FontStyle.Bold);
            UiFactory.Label(objective, "Subtitle", "Land inside the marked bay.", 28, new Vector2(0.04f, 0.20f), new Vector2(0.96f, 0.54f), TextAnchor.MiddleLeft, TextColor);
            var progress = UiFactory.Progress(objective, new Vector2(0.04f, 0.08f), new Vector2(0.96f, 0.16f), new Color(0.18f, 0.22f, 0.30f), Gold);
            var feedback = UiFactory.Label(safe, "Feedback", "READY", 30, new Vector2(0.32f, 0.73f), new Vector2(0.68f, 0.78f), TextAnchor.MiddleCenter, Gold, FontStyle.Bold);
            var joystick = VirtualJoystick.Create(safe, new Vector2(0.03f, 0.035f), new Vector2(0.29f, 0.18f));
            var jump = HoldButton.Create(safe, "↑", new Vector2(0.78f, 0.035f), new Vector2(0.97f, 0.17f), new Color(0.03f, 0.06f, 0.11f, 0.92f));

            var player = CreatePlayer();
            var camera = CreateCamera(player.transform);
            var courseObject = new GameObject("PrecisionCourse");
            var course = courseObject.AddComponent<PrecisionCourse>();
            course.Player = player.transform;
            course.FloorText = stats[0];
            course.BestText = stats[1];
            course.StreakText = stats[2];
            course.CoinsText = stats[3];
            course.FeedbackText = feedback;
            course.ProgressFill = progress;

            var motor = player.GetComponent<PlayerMotor>();
            motor.Joystick = joystick;
            motor.JumpButton = jump;
            motor.CameraTransform = camera.transform;
            motor.Course = course;

            var boosts = UiFactory.Button(safe, "Boosts", "BOOSTS", Gold, new Color(0.08f, 0.07f, 0.03f), new Vector2(0.34f, 0.045f), new Vector2(0.68f, 0.125f), () => motor.ApplyBoost(1.22f, 5f));

            var responsive = safe.gameObject.AddComponent<ResponsiveHud>();
            responsive.Bind(
                statRects,
                objective,
                feedback.rectTransform,
                joystick.GetComponent<RectTransform>(),
                boosts.GetComponent<RectTransform>(),
                jump.GetComponent<RectTransform>());

            course.Build();
        }

        private static GameObject CreatePlayer()
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            go.name = "Runner";
            Destroy(go.GetComponent<Collider>());
            var controller = go.AddComponent<CharacterController>();
            controller.height = 2f;
            controller.radius = 0.42f;
            controller.center = new Vector3(0f, 1f, 0f);
            go.AddComponent<PlayerMotor>();
            go.GetComponent<Renderer>().material = new Material(Shader.Find("Standard")) { color = new Color(0.05f, 0.06f, 0.08f) };
            return go;
        }

        private static Camera CreateCamera(Transform target)
        {
            var go = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener), typeof(OrbitCamera));
            go.tag = "MainCamera";
            var camera = go.GetComponent<Camera>();
            camera.fieldOfView = 58f;
            var orbit = go.GetComponent<OrbitCamera>();
            orbit.Target = target;
            orbit.Distance = 7.5f;
            orbit.Height = 2.7f;
            go.transform.position = target.position + new Vector3(0f, 3f, -7f);
            return camera;
        }

        private static void CreateLighting()
        {
            var go = new GameObject("Sun", typeof(Light));
            var light = go.GetComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.2f;
            light.color = new Color(1f, 0.78f, 0.55f);
            go.transform.rotation = Quaternion.Euler(38f, -28f, 0f);
        }

        private static void EnsureEventSystem()
        {
            if (FindFirstObjectByType<EventSystem>() != null) return;
            new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        }
    }
}
