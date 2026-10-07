using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Kamilunavo.PerfectDrop.CameraSystem;
using Kamilunavo.PerfectDrop.Gameplay;
using Kamilunavo.PerfectDrop.Feedback;
using Kamilunavo.PerfectDrop.Input;
using Kamilunavo.PerfectDrop.UI;
using Kamilunavo.PerfectDrop.Visuals;

namespace Kamilunavo.PerfectDrop
{
    public sealed class GameBootstrap : MonoBehaviour
    {
        private static readonly Color Panel = new(0.035f, 0.075f, 0.13f, 0.96f);
        private static readonly Color Gold = new(1f, 0.78f, 0.12f, 1f);
        private static readonly Color TextColor = new(0.97f, 0.985f, 1f, 1f);

        private void Start()
        {
            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 0;
            RenderSettings.fog = true;
            RenderSettings.fogColor = new Color(0.22f, 0.35f, 0.55f);
            RenderSettings.fogDensity = 0.006f;
            RenderSettings.ambientLight = new Color(0.12f, 0.16f, 0.24f);
            WorldArt.ApplySkybox();
            EnsureEventSystem();
            CreateLighting();

            var canvas = UiFactory.CreateCanvas();
            var safe = UiFactory.Panel(canvas.transform, "SafeArea", Color.clear, Vector2.zero, Vector2.one);
            safe.gameObject.AddComponent<SafeAreaFitter>();

            var stats = new Text[4];
            var statRects = new RectTransform[4];
            var labels = new[] { GameText.Floor(1), GameText.Best(1), GameText.Streak(0), GameText.Coins(0) };
            var icons = new[] { HudIconType.Floors, HudIconType.Crown, HudIconType.Flame, HudIconType.Diamond };
            for (var i = 0; i < 4; i++)
            {
                var minX = 0.03f + i * 0.242f;
                var panel = UiFactory.Panel(safe, $"Stat_{i}", Panel, new Vector2(minX, 0.90f), new Vector2(minX + 0.215f, 0.975f));
                statRects[i] = panel;
                UiFactory.Icon(panel, "Icon", icons[i], Gold, new Vector2(0.075f, 0.22f), new Vector2(0.31f, 0.78f));
                stats[i] = UiFactory.Label(panel, "Text", labels[i], 34, new Vector2(0.34f, 0.08f), new Vector2(0.94f, 0.92f), TextAnchor.MiddleLeft, TextColor, FontStyle.Bold);
            }

            var objective = UiFactory.Panel(safe, "Objective", Panel, new Vector2(0.03f, 0.79f), new Vector2(0.97f, 0.885f));
            UiFactory.Label(objective, "Title", GameText.ObjectiveTitle, 36, new Vector2(0.04f, 0.50f), new Vector2(0.77f, 0.92f), TextAnchor.MiddleLeft, Gold, FontStyle.Bold);
            UiFactory.Label(objective, "Subtitle", GameText.ObjectiveSubtitle, 28, new Vector2(0.04f, 0.20f), new Vector2(0.77f, 0.54f), TextAnchor.MiddleLeft, TextColor);
            var progress = UiFactory.Progress(objective, new Vector2(0.04f, 0.08f), new Vector2(0.96f, 0.16f), new Color(0.18f, 0.22f, 0.30f), Gold);
            var feedback = UiFactory.Label(safe, "Feedback", GameText.Ready, 30, new Vector2(0.32f, 0.73f), new Vector2(0.68f, 0.78f), TextAnchor.MiddleCenter, Gold, FontStyle.Bold);
            var joystick = VirtualJoystick.Create(safe, new Vector2(0.03f, 0.035f), new Vector2(0.29f, 0.18f));
            var jump = HoldButton.Create(safe, string.Empty, new Vector2(0.78f, 0.035f), new Vector2(0.97f, 0.17f), new Color(0.03f, 0.06f, 0.11f, 0.92f));
            UiFactory.ApplyCircularImage(jump.GetComponent<Image>());
            UiFactory.AddOutline(jump.GetComponent<Image>(), Gold, 3f);
            UiFactory.Icon(jump.transform, "JumpIcon", HudIconType.ArrowUp, TextColor, new Vector2(0.27f, 0.24f), new Vector2(0.73f, 0.76f));

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

            var boosts = UiFactory.Button(safe, "Boosts", GameText.Boosts, Gold, new Color(0.08f, 0.07f, 0.03f), new Vector2(0.34f, 0.045f), new Vector2(0.68f, 0.125f), () => motor.ApplyBoost(1.22f, 5f));
            UiFactory.ApplyPillImage(boosts.GetComponent<Image>());
            UiFactory.Icon(boosts.transform, "BoostIcon", HudIconType.Bolt, new Color(0.06f, 0.05f, 0.015f, 1f), new Vector2(0.11f, 0.20f), new Vector2(0.29f, 0.80f));
            var boostLabel = boosts.transform.Find("Label")?.GetComponent<Text>();
            if (boostLabel != null)
            {
                boostLabel.rectTransform.anchorMin = new Vector2(0.30f, 0f);
                boostLabel.rectTransform.anchorMax = new Vector2(0.94f, 1f);
                boostLabel.rectTransform.offsetMin = Vector2.zero;
                boostLabel.rectTransform.offsetMax = Vector2.zero;
            }

            var completion = UiFactory.Panel(safe, "Completion", new Color(0.02f, 0.045f, 0.085f, 0.985f), new Vector2(0.09f, 0.33f), new Vector2(0.91f, 0.67f));
            UiFactory.Label(completion, "Title", GameText.TowerCleared, 52, new Vector2(0.07f, 0.67f), new Vector2(0.93f, 0.91f), TextAnchor.MiddleCenter, Gold, FontStyle.Bold);
            var completionText = UiFactory.Label(completion, "Summary", GameText.Completion(1, 0, PlayerProfileStore.Level), 31, new Vector2(0.08f, 0.38f), new Vector2(0.92f, 0.66f), TextAnchor.MiddleCenter, TextColor, FontStyle.Bold);
            UiFactory.Button(completion, "RunAgain", GameText.RunAgain, Gold, new Color(0.08f, 0.07f, 0.03f), new Vector2(0.18f, 0.10f), new Vector2(0.82f, 0.31f), course.RestartRun);
            completion.gameObject.SetActive(false);
            course.CompletionPanel = completion.gameObject;
            course.CompletionText = completionText;

            var settingsRoot = UiFactory.Panel(safe, "Settings", new Color(0.02f, 0.045f, 0.085f, 0.99f), new Vector2(0.12f, 0.25f), new Vector2(0.88f, 0.75f));
            UiFactory.Label(settingsRoot, "Title", GameText.SettingsTitle, 44, new Vector2(0.07f, 0.81f), new Vector2(0.58f, 0.96f), TextAnchor.MiddleLeft, Gold, FontStyle.Bold);
            UiFactory.Label(settingsRoot, "SoundLabel", GameText.Sound, 30, new Vector2(0.08f, 0.61f), new Vector2(0.62f, 0.76f), TextAnchor.MiddleLeft, TextColor, FontStyle.Bold);
            UiFactory.Label(settingsRoot, "HapticsLabel", GameText.Haptics, 30, new Vector2(0.08f, 0.43f), new Vector2(0.62f, 0.58f), TextAnchor.MiddleLeft, TextColor, FontStyle.Bold);
            UiFactory.Label(settingsRoot, "MotionLabel", GameText.ReducedMotion, 27, new Vector2(0.08f, 0.25f), new Vector2(0.62f, 0.40f), TextAnchor.MiddleLeft, TextColor, FontStyle.Bold);

            var settings = safe.gameObject.AddComponent<SettingsPanel>();
            settings.Root = settingsRoot.gameObject;
            settings.Motor = motor;
            settings.Course = course;

            var soundToggle = UiFactory.Button(settingsRoot, "SoundToggle", GameText.ToggleValue(GamePreferences.AudioEnabled), Gold, new Color(0.08f, 0.07f, 0.03f), new Vector2(0.68f, 0.61f), new Vector2(0.92f, 0.76f), settings.ToggleSound);
            var hapticsToggle = UiFactory.Button(settingsRoot, "HapticsToggle", GameText.ToggleValue(GamePreferences.HapticsEnabled), Gold, new Color(0.08f, 0.07f, 0.03f), new Vector2(0.68f, 0.43f), new Vector2(0.92f, 0.58f), settings.ToggleHaptics);
            var motionToggle = UiFactory.Button(settingsRoot, "MotionToggle", GameText.ToggleValue(GamePreferences.ReducedMotion), Gold, new Color(0.08f, 0.07f, 0.03f), new Vector2(0.68f, 0.25f), new Vector2(0.92f, 0.40f), settings.ToggleReducedMotion);
            settings.SoundValue = soundToggle.transform.Find("Label").GetComponent<Text>();
            settings.HapticsValue = hapticsToggle.transform.Find("Label").GetComponent<Text>();
            settings.MotionValue = motionToggle.transform.Find("Label").GetComponent<Text>();
            UiFactory.Button(settingsRoot, "Close", GameText.Close, Gold, new Color(0.08f, 0.07f, 0.03f), new Vector2(0.26f, 0.06f), new Vector2(0.74f, 0.18f), settings.Close);

            var progressionRoot = UiFactory.Panel(safe, "Progression", new Color(0.02f, 0.045f, 0.085f, 0.995f), new Vector2(0.08f, 0.13f), new Vector2(0.92f, 0.86f));
            UiFactory.Label(progressionRoot, "Title", GameText.ProfileTitle, 44, new Vector2(0.06f, 0.87f), new Vector2(0.94f, 0.97f), TextAnchor.MiddleLeft, Gold, FontStyle.Bold);
            var levelText = UiFactory.Label(progressionRoot, "Level", GameText.ProfileLevel(PlayerProfileStore.Level), 34, new Vector2(0.06f, 0.76f), new Vector2(0.45f, 0.86f), TextAnchor.MiddleLeft, TextColor, FontStyle.Bold);
            var xpText = UiFactory.Label(progressionRoot, "XP", GameText.ProfileXp(PlayerProfileStore.XpIntoLevel, PlayerProfileStore.XpForNextLevel), 26, new Vector2(0.48f, 0.76f), new Vector2(0.94f, 0.86f), TextAnchor.MiddleRight, TextColor);

            UiFactory.Label(progressionRoot, "DailyTitle", GameText.DailyReward, 29, new Vector2(0.06f, 0.64f), new Vector2(0.66f, 0.72f), TextAnchor.MiddleLeft, Gold, FontStyle.Bold);
            var dailyText = UiFactory.Label(progressionRoot, "DailyStatus", GameText.DailyRewardStatus(PlayerProfileStore.CanClaimDailyReward), 23, new Vector2(0.06f, 0.54f), new Vector2(0.66f, 0.64f), TextAnchor.MiddleLeft, TextColor);

            UiFactory.Label(progressionRoot, "ChallengeTitle", GameText.DailyChallenge, 29, new Vector2(0.06f, 0.43f), new Vector2(0.66f, 0.51f), TextAnchor.MiddleLeft, Gold, FontStyle.Bold);
            var challengeText = UiFactory.Label(progressionRoot, "ChallengeStatus", GameText.DailyChallengeStatus(PlayerProfileStore.DailyChallengeProgress, PlayerProfileStore.DailyChallengeTarget, PlayerProfileStore.DailyChallengeClaimed), 23, new Vector2(0.06f, 0.33f), new Vector2(0.66f, 0.43f), TextAnchor.MiddleLeft, TextColor);

            UiFactory.Label(progressionRoot, "StyleTitle", GameText.RunnerStyle, 29, new Vector2(0.06f, 0.22f), new Vector2(0.66f, 0.30f), TextAnchor.MiddleLeft, Gold, FontStyle.Bold);
            var styleText = UiFactory.Label(progressionRoot, "StyleStatus", GameText.StyleStatus(PlayerProfileStore.StyleName(PlayerProfileStore.SelectedStyle, GameText.German), PlayerProfileStore.Level), 23, new Vector2(0.06f, 0.12f), new Vector2(0.66f, 0.22f), TextAnchor.MiddleLeft, TextColor);

            var progression = safe.gameObject.AddComponent<ProgressionPanel>();
            progression.Root = progressionRoot.gameObject;
            progression.Motor = motor;
            progression.Course = course;
            progression.StyleController = player.GetComponent<RunnerStyleController>();
            progression.LevelText = levelText;
            progression.XpText = xpText;
            progression.DailyText = dailyText;
            progression.ChallengeText = challengeText;
            progression.StyleText = styleText;
            progression.DailyButton = UiFactory.Button(progressionRoot, "DailyClaim", GameText.Claim, Gold, new Color(0.08f, 0.07f, 0.03f), new Vector2(0.70f, 0.54f), new Vector2(0.94f, 0.70f), progression.ClaimDaily);
            progression.ChallengeButton = UiFactory.Button(progressionRoot, "ChallengeClaim", GameText.Claim, Gold, new Color(0.08f, 0.07f, 0.03f), new Vector2(0.70f, 0.33f), new Vector2(0.94f, 0.49f), progression.ClaimChallenge);
            progression.StyleButton = UiFactory.Button(progressionRoot, "StyleNext", GameText.NextStyle, Gold, new Color(0.08f, 0.07f, 0.03f), new Vector2(0.70f, 0.12f), new Vector2(0.94f, 0.28f), progression.NextStyle);
            UiFactory.Button(progressionRoot, "Close", GameText.Close, new Color(0.12f, 0.17f, 0.24f, 1f), TextColor, new Vector2(0.34f, 0.025f), new Vector2(0.66f, 0.105f), progression.Close);
            progressionRoot.gameObject.SetActive(false);

            UiFactory.Button(settingsRoot, "Profile", GameText.Progression, new Color(0.12f, 0.17f, 0.24f, 1f), TextColor, new Vector2(0.64f, 0.82f), new Vector2(0.92f, 0.95f), progression.Open);
            settingsRoot.gameObject.SetActive(false);

            UiFactory.Button(objective, "Menu", GameText.Menu, new Color(0.12f, 0.17f, 0.24f, 1f), TextColor, new Vector2(0.82f, 0.38f), new Vector2(0.96f, 0.84f), settings.Open);

            var responsive = safe.gameObject.AddComponent<ResponsiveHud>();
            responsive.Bind(
                statRects,
                objective,
                feedback.rectTransform,
                joystick.GetComponent<RectTransform>(),
                boosts.GetComponent<RectTransform>(),
                jump.GetComponent<RectTransform>(),
                completion,
                settingsRoot,
                progressionRoot);

            course.Build();
        }

        private static GameObject CreatePlayer()
        {
            var go = new GameObject("Runner");
            var controller = go.AddComponent<CharacterController>();
            controller.height = 2f;
            controller.radius = 0.42f;
            controller.center = new Vector3(0f, 1f, 0f);
            go.AddComponent<AudioSource>();
            go.AddComponent<FeedbackSystem>();
            go.AddComponent<PlayerMotor>();
            WorldArt.CreateRunnerVisual(go.transform);
            go.AddComponent<RunnerStyleController>();
            return go;
        }

        private static Camera CreateCamera(Transform target)
        {
            var go = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener), typeof(OrbitCamera));
            go.tag = "MainCamera";
            var camera = go.GetComponent<Camera>();
            go.AddComponent<CinematicGrade>();
            camera.fieldOfView = 58f;
            camera.clearFlags = CameraClearFlags.Skybox;
            camera.backgroundColor = new Color(0.025f, 0.055f, 0.105f);
            camera.allowHDR = true;
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
            if (FindAnyObjectByType<EventSystem>() != null) return;
            new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        }
    }
}
