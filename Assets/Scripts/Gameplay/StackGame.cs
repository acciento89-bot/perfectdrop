using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Kamilunavo.PerfectDrop.UI;
using Kamilunavo.PerfectDrop.Visuals;
using Kamilunavo.PerfectDrop.Feedback;

namespace Kamilunavo.PerfectDrop.Gameplay
{
    public sealed class StackGame : MonoBehaviour
    {
        public const float LayerHeight = .34f;
        public StackRun Run { get; private set; }
        public StackProfile Profile { get; private set; }
        public StackHud Hud { get; private set; }
        public float MovingOffset { get; private set; }
        private Transform _tower, _moving;
        private Camera _camera;
        private FeedbackSystem _feedback;
        private readonly List<GameObject> _placed = new();
        private float _phase, _yaw = -35f;
        private float _cameraAspect;
        private Rect _cameraPane;
        private Vector2 _press, _lastPointer;
        private bool _dragging, _pointerActive, _startedOnUi;
        private int _width, _height;
        private bool _paused;
        private float _nextDrop;

        public void Initialize()
        {
            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 0;
            QualitySettings.antiAliasing = 4;
            WorldArt.ApplySkybox();
            RenderSettings.fog = true;
            RenderSettings.fogColor = new Color(.34f,.38f,.56f);
            RenderSettings.fogDensity = .005f;
            var key = new GameObject("SunsetKey").AddComponent<Light>();
            key.type = LightType.Directional; key.color = new Color(1f,.72f,.42f); key.intensity = 1.4f;
            key.transform.rotation = Quaternion.Euler(35,-32,0); key.shadows = LightShadows.Soft;
            var fill = new GameObject("SkyFill").AddComponent<Light>();
            fill.type = LightType.Directional; fill.color = new Color(.35f,.52f,1f); fill.intensity = .55f;
            fill.transform.rotation = Quaternion.Euler(25,145,0);
            var cameraObject = new GameObject("StackCamera"); cameraObject.tag = "MainCamera";
            _camera = cameraObject.AddComponent<Camera>(); _camera.fieldOfView = 43; _camera.nearClipPlane = .15f; _camera.farClipPlane = 230;
            cameraObject.AddComponent<AudioListener>(); cameraObject.AddComponent<CinematicGrade>();
            var feedback = new GameObject("StackFeedback",typeof(AudioSource),typeof(FeedbackSystem));
            _feedback = feedback.GetComponent<FeedbackSystem>();
            if (FindFirstObjectByType<EventSystem>() == null) new GameObject("EventSystem",typeof(EventSystem),typeof(StandaloneInputModule));
            var world = new GameObject("CloudCity").transform;
            WorldArt.BuildStackCloudSea(world); WorldArt.BuildSkyline(world);
            _tower = new GameObject("StackTower").transform;
            Profile = StackSave.Load();
            Run = new StackRun();
            if (Profile.ResumeActive) { Run.Restore(Profile.Layers,Profile.Streak,Profile.RunCoins); _phase = Profile.Phase; }
            else _phase = -Mathf.PI/2;
            CreateBlock("Pedestal",Vector2.zero,Vector2.one*4f,-LayerHeight,0);
            for (var i=0;i<Run.Layers.Count;i++) _placed.Add(CreateBlock("Placed_"+(i+1),Run.Layers[i].Center,Run.Layers[i].Size,i*LayerHeight,i+1));
            SpawnMoving();
            Hud = gameObject.AddComponent<StackHud>(); Hud.Build(this);
            UpdateCamera(true);
        }

        private void Update()
        {
            if (Run == null || _paused) return;
            if (!Hud.ModalOpen && !Run.Failed && !Run.Completed)
            {
                _phase += Mathf.Min(Time.deltaTime,.05f)*(1.05f + Run.Layers.Count*.032f);
                MovingOffset = Mathf.Sin(_phase) * StackRules.MotionExtent(Run.Top.Size,Run.Axis);
                var position = Run.Top.Center + (Run.Axis == StackAxis.X ? new Vector2(MovingOffset,0) : new Vector2(0,MovingOffset));
                _moving.localPosition = new Vector3(position.x,Run.Layers.Count*LayerHeight,position.y);
            }
            ReadInput(); UpdateCamera(false);
        }
        private void ReadInput()
        {
            if (_width != Screen.width || _height != Screen.height)
            { _pointerActive = false; _width = Screen.width; _height = Screen.height; }
            if (Hud.ModalOpen) { _pointerActive = false; return; }
            if (UnityEngine.Input.GetKeyDown(KeyCode.Space)) Drop();
            if (UnityEngine.Input.GetMouseButton(1)) _yaw += UnityEngine.Input.GetAxis("Mouse X")*3;
            if (UnityEngine.Input.touchCount > 0)
            {
                var touch = UnityEngine.Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began) BeginPointer(touch.position,EventSystem.current.IsPointerOverGameObject(touch.fingerId));
                if (touch.phase == TouchPhase.Moved) MovePointer(touch.position);
                if (touch.phase == TouchPhase.Ended) EndPointer(touch.position);
                if (touch.phase == TouchPhase.Canceled) _pointerActive = false;
                return;
            }
            if (UnityEngine.Input.GetMouseButtonDown(0)) BeginPointer(UnityEngine.Input.mousePosition,EventSystem.current.IsPointerOverGameObject());
            if (UnityEngine.Input.GetMouseButton(0)) MovePointer(UnityEngine.Input.mousePosition);
            if (UnityEngine.Input.GetMouseButtonUp(0)) EndPointer(UnityEngine.Input.mousePosition);
        }
        private void BeginPointer(Vector2 point,bool ui) { _press = _lastPointer = point; _startedOnUi = ui; _dragging = false; _pointerActive = true; }
        private void MovePointer(Vector2 point)
        {
            if (!_pointerActive || _startedOnUi) return;
            if (Vector2.Distance(point,_press) > Mathf.Min(Screen.width,Screen.height)*.02f) _dragging = true;
            if (_dragging) _yaw += (point.x-_lastPointer.x)*.16f;
            _lastPointer = point;
        }
        private void EndPointer(Vector2 point)
        {
            if (_pointerActive && !_startedOnUi && !_dragging && Vector2.Distance(point,_press) <= Mathf.Min(Screen.width,Screen.height)*.02f) Drop();
            _pointerActive = false;
        }

        public void Drop()
        {
            if (Run == null || Run.Failed || Run.Completed || Hud.ModalOpen || _paused || Time.unscaledTime < _nextDrop) return;
            // Suppress only duplicate events from a single tap; no blocking placement animation.
            _nextDrop = Time.unscaledTime+.06f;
            var oldCoins = Run.EarnedCoins;
            var result = Run.Place(MovingOffset);
            var height = (Run.Layers.Count-1)*LayerHeight;
            if (result.Grade == StackGrade.Miss)
            {
                _moving.gameObject.AddComponent<StackOffcut>(); _moving = null;
                _feedback.PlayRecovery(); Hud.Grade(result.Grade); Save(); Hud.Terminal(false); return;
            }
            Destroy(_moving.gameObject);
            _placed.Add(CreateBlock("Placed_"+Run.Layers.Count,result.Center,result.Size,height,Run.Layers.Count));
            if (result.Grade == StackGrade.Good)
                CreateBlock("Overhang",result.CutCenter,result.CutSize,height,Run.Layers.Count).AddComponent<StackOffcut>();
            Profile.Coins += Run.EarnedCoins-oldCoins;
            Profile.Best = Mathf.Max(Profile.Best,Run.Layers.Count);
            _feedback.PlayLanding(result.Grade == StackGrade.Perfect ? LandingGrade.Perfect : LandingGrade.Good);
            if (result.Grade == StackGrade.Perfect)
                WorldArt.SpawnLandingBurst(new Vector3(result.Center.x,height+LayerHeight*.5f,result.Center.y),LandingGrade.Good);
            Hud.Grade(result.Grade);
            if (Run.Completed)
            { Profile.Towers++; _feedback.PlayComplete(); Hud.Terminal(true); }
            else { _phase = -Mathf.PI/2; SpawnMoving(); }
            Save(); Hud.Refresh();
        }
        public void NewRun()
        {
            foreach (var block in _placed) if (block != null) Destroy(block);
            _placed.Clear();
            if (_moving != null) Destroy(_moving.gameObject);
            Run = new StackRun(); _phase = -Mathf.PI/2; MovingOffset = -StackRules.BaseWidth*1.24f; _nextDrop = 0;
            SpawnMoving(); Hud.ResetMessage(); Save(); UpdateCamera(true);
        }
        private void SpawnMoving()
        {
            var top = Run.Top;
            MovingOffset = Mathf.Sin(_phase)*StackRules.MotionExtent(top.Size,Run.Axis);
            var center = top.Center + (Run.Axis == StackAxis.X ? new Vector2(MovingOffset,0) : new Vector2(0,MovingOffset));
            _moving = CreateBlock("MovingBlock",center,top.Size,Run.Layers.Count*LayerHeight,Run.Layers.Count+1).transform;
        }
        private GameObject CreateBlock(string name,Vector2 center,Vector2 size,float height,int level)
        {
            var block = WorldArt.CreateStackBlock(_tower,name,new Vector3(center.x,height,center.y),new Vector3(size.x,LayerHeight,size.y),level);
            return block;
        }
        private void UpdateCamera(bool snap)
        {
            var top = Run.Top.Center;
            var height = Run.Layers.Count*LayerHeight;
            var focus = new Vector3(top.x,height-.7f,top.y);
            var yaw = _yaw*Mathf.Deg2Rad;
            var size = Run.Top.Size;
            var x = Mathf.Abs(Mathf.Cos(yaw)); var z = Mathf.Abs(Mathf.Sin(yaw));
            var horizontal = x*size.x*.5f + z*size.y*.5f + Mathf.Max(x*size.x,z*size.y)*1.24f;
            var depth = z*size.x*.5f + x*size.y*.5f + Mathf.Max(z*size.x,x*size.y)*1.24f;
            var tan = Mathf.Tan(_camera.fieldOfView*.5f*Mathf.Deg2Rad);
            var distance = Mathf.Max(17f, horizontal/(tan*_camera.aspect*.82f)+depth+2f);
            var desired = focus + new Vector3(Mathf.Sin(yaw),.65f,-Mathf.Cos(yaw)).normalized*distance;
            var paneNow = Hud != null ? Hud.WorldPane : new Rect(0,0,1,1);
            var layoutChanged = !Mathf.Approximately(_cameraAspect,_camera.aspect) || _cameraPane != paneNow;
            _cameraAspect = _camera.aspect; _cameraPane = paneNow;
            var needsOutwardFit = Vector3.Distance(_camera.transform.position,focus) < distance;
            var blend = snap || layoutChanged || needsOutwardFit || GamePreferences.ReducedMotion ? 1f : 1-Mathf.Exp(-7f*Time.deltaTime);
            _camera.transform.position = Vector3.Lerp(_camera.transform.position,desired,blend);
            _camera.transform.rotation = Quaternion.Slerp(_camera.transform.rotation,Quaternion.LookRotation(focus-_camera.transform.position),blend);
            // Keep the full world render while fitting the active stack inside the available pane.
            _camera.ResetProjectionMatrix();
            if (Hud != null)
            {
                var pane = Hud.WorldPane;
                var projection = _camera.projectionMatrix;
                var fit = Mathf.Min(pane.width,pane.height);
                projection.m00 *= fit; projection.m11 *= fit;
                projection.m02 = 1f - 2f*pane.center.x;
                projection.m12 = 1f - 2f*pane.center.y;
                _camera.projectionMatrix = projection;
            }
        }
        public void UiClick() => _feedback.PlayJump();
        public void Save()
        {
            if (Profile == null || Run == null) return;
            Profile.ResumeActive = !Run.Failed && !Run.Completed;
            Profile.Layers = new List<StackLayer>(Run.Layers);
            Profile.Streak = Run.Streak; Profile.RunCoins = Run.EarnedCoins; Profile.Phase = _phase;
            StackSave.Save(Profile);
        }
        private void OnApplicationPause(bool pause) { _paused = pause; _pointerActive = false; Save(); }
        private void OnApplicationFocus(bool focus) { _paused = !focus; _pointerActive = false; if (!focus) Save(); }
        private void OnApplicationQuit() => Save();
    }

    public sealed class StackOffcut : MonoBehaviour
    {
        private float _age, _velocity;
        private void Update()
        {
            _age += Time.deltaTime; _velocity -= 14f*Time.deltaTime;
            transform.position += Vector3.up*(_velocity*Time.deltaTime);
            if (!GamePreferences.ReducedMotion) transform.Rotate(new Vector3(25,14,32)*Time.deltaTime);
            if (_age > 2f) Destroy(gameObject);
        }
    }
}
