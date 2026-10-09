using System;
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
        public const float LayerHeight = .52f;
        public StackRun Run { get; private set; }
        public StackProfile Profile { get; private set; }
        public StackHud Hud { get; private set; }
        public Monetization.StorePurchases Purchases {get;private set;}
        public Monetization.RewardedVideos Videos {get;private set;}
        public float MovingOffset { get; private set; }
        public StackLevel Level { get; private set; }
        public int LastStars { get; private set; }
        public int LastBonus { get; private set; }
        private bool _hasStarted;
        private Transform _tower, _moving;
        private Transform _world;
        private GameObject _city;
        public int CityDistrict { get; private set; }
        public StackBlockKind CurrentKind => StackCampaign.Kind(Profile.RunEndless || Profile.RunChallenge?Profile.UnlockedLevel:Level.Id,Run.Count);
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
            _camera = cameraObject.AddComponent<Camera>(); _camera.allowHDR = true; _camera.fieldOfView = 43; _camera.nearClipPlane = .15f; _camera.farClipPlane = 230;
            cameraObject.AddComponent<AudioListener>(); cameraObject.AddComponent<CinematicGrade>();
            var feedback = new GameObject("StackFeedback",typeof(AudioSource),typeof(FeedbackSystem));
            _feedback = feedback.GetComponent<FeedbackSystem>();
            if (FindFirstObjectByType<EventSystem>() == null) new GameObject("EventSystem",typeof(EventSystem),typeof(StandaloneInputModule));
            var world = new GameObject("CloudCity").transform; _world=world;
            // Authored cloud-city environment is rendered by the skybox; keep the
            // foreground stack and earned city as live geometry.
            _tower = new GameObject("StackTower").transform;
            Profile = StackSave.Load();
            Level = Profile.RunEndless?StackCampaign.Level(1):Profile.RunChallenge?StackCampaign.Daily(DateTime.ParseExact(Profile.RunChallengeDate,"yyyy-MM-dd",System.Globalization.CultureInfo.InvariantCulture)):StackCampaign.Level(Profile.RunLevel);
            WorldArt.SetChapter(Level.Chapter);
            RebuildCity();
            Run = new StackRun(Level.Target,Profile.RunEndless,Vector2.one*Level.Width);
            _hasStarted = Profile.ResumeActive;
            if (Profile.ResumeActive) { Run.Restore(Profile.Layers,Profile.Streak,Profile.RunCoins,Profile.TotalPlaced,Profile.PerfectDrops,Profile.MaxStreak); _phase = Profile.Phase; }
            else _phase = -Mathf.PI/2;
            if(Profile.ResumeActive)Run.RestorePowers(Profile.Powers);
            CreateBlock("Pedestal",Vector2.zero,Vector2.one*4f,-LayerHeight,0);
            for (var i=0;i<Run.Layers.Count;i++) _placed.Add(CreateBlock("Placed_"+(i+1),Run.Layers[i].Center,Run.Layers[i].Size,i*LayerHeight,Run.Count-Run.Layers.Count+i+1));
            SpawnMoving();
            Hud = gameObject.AddComponent<StackHud>(); Hud.Build(this); Hud.ResetMessage();
            if(!Profile.ResumeActive) Hud.ShowHome();
            UpdateCamera(true);
            Purchases=gameObject.AddComponent<Monetization.StorePurchases>();
            Purchases.Changed+=Hud.RefreshCommerce;Purchases.Initialize(this);
            Videos=gameObject.AddComponent<Monetization.RewardedVideos>();Videos.Changed+=Hud.RefreshCommerce;Videos.Initialize(this);
        }

        private void Update()
        {
            if (Run == null || _paused) return;
            if (!Hud.ModalOpen && !Run.Failed && !Run.Completed)
            {
                var delta=Mathf.Min(Time.deltaTime,.05f);
                Run.Powers.Tick(delta);
                var speed=(Run.Endless?1.05f:Level.Speed)+Mathf.Min(Run.Count,80)*.020f;
                if(CurrentKind==StackBlockKind.Drift)speed*=1+.30f*Mathf.Sin(_phase*2);
                _phase += delta*speed*(Run.Powers.SlowSeconds>0?.35f:1f);
                MovingOffset = Mathf.Sin(_phase) * StackRules.MotionExtent(Run.Top.Size,Run.Axis);
                if(CurrentKind==StackBlockKind.Wind)MovingOffset+=Mathf.Sin(_phase*2.3f)*(Run.Axis==StackAxis.X?Run.Top.Size.x:Run.Top.Size.y)*.12f;
                var position = Run.Top.Center + (Run.Axis == StackAxis.X ? new Vector2(MovingOffset,0) : new Vector2(0,MovingOffset));
                _moving.localPosition = new Vector3(position.x,Run.Layers.Count*LayerHeight,position.y);
            }
            ReadInput(); UpdateCamera(false);
        }
        private void ReadInput()
        {
            if (_width != Screen.width || _height != Screen.height)
            { _pointerActive = false; _width = Screen.width; _height = Screen.height; }
            if (Hud.ModalOpen && !Hud.CityOpen) { _pointerActive = false; return; }
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
            var kind=CurrentKind;
            var result = Run.Place(MovingOffset,kind);
            var height = (Run.Layers.Count-1)*LayerHeight;
            if (result.Grade == StackGrade.Miss)
            {
                _moving.gameObject.AddComponent<StackOffcut>(); _moving = null;
                _feedback.PlayRecovery(); Hud.Grade(result.Grade); Save(); Hud.Terminal(false); return;
            }
            Destroy(_moving.gameObject);
            if(Run.Endless && Run.Count>StackRun.RetainedLayers)
            {
                Destroy(_placed[0]); _placed.RemoveAt(0);
                foreach(var block in _placed) block.transform.localPosition-=Vector3.up*LayerHeight;
            }
            _placed.Add(CreateBlock("Placed_"+Run.Count,result.Center,result.Size,height,Run.Count));
            if (result.Grade == StackGrade.Good && result.CutSize.x>0 && result.CutSize.y>0)
                CreateBlock("Overhang",result.CutCenter,result.CutSize,height,Run.Layers.Count).AddComponent<StackOffcut>();
            Profile.Coins += Run.EarnedCoins-oldCoins;
            if(Run.Endless) Profile.EndlessBest=Mathf.Max(Profile.EndlessBest,Run.Count);
            else Profile.Best = Mathf.Max(Profile.Best,Run.Count);
            _feedback.PlayLanding(result.Grade == StackGrade.Perfect ? LandingGrade.Perfect : LandingGrade.Good);
            if (result.Grade == StackGrade.Perfect)
            {
                var landing=new Vector3(result.Center.x,height+LayerHeight*.55f,result.Center.y);
                WorldArt.SpawnLandingBurst(landing,LandingGrade.Good);
                WorldArt.SpawnStackLandingPulse(landing,result.Size,Profile.Style);
            }
            Hud.Grade(result.Grade);
            if(result.Rescued)Hud.ArcadeFeedback(StackHud.T("GERETTET!","SAVED!"));
            if (Run.Completed)
            { Profile.Towers++; LastStars=StackCampaign.Stars(Level,Run); LastBonus=Profile.RunChallenge?StackCampaign.RecordDaily(Profile,Level,Run,Profile.RunChallengeDate):StackCampaign.Record(Profile,Level,Run);
                if(!Profile.RunChallenge && LastBonus>0)RebuildCity(); _feedback.PlayComplete(); Hud.Terminal(true); }
            else { _phase = -Mathf.PI/2; SpawnMoving(); }
            Save(); Hud.Refresh();
        }
        public void NewRun()
        {
            foreach (var block in _placed) if (block != null) Destroy(block);
            _placed.Clear();
            if (_moving != null) Destroy(_moving.gameObject);
            if(Profile.RunChallenge){Profile.RunChallengeDate=DateTime.UtcNow.ToString("yyyy-MM-dd");Level=StackCampaign.Daily(DateTime.UtcNow);}
            Run = new StackRun(Level.Target,Profile.RunEndless,Vector2.one*Level.Width); _hasStarted=true; LastStars=LastBonus=0; _phase = -Mathf.PI/2; MovingOffset = -Level.Width*1.24f; _nextDrop = 0;
            Hud.HideMenus(); WorldArt.SetChapter(Level.Chapter);
            SpawnMoving(); Hud.ResetMessage(); Save(); UpdateCamera(true); if(!Profile.TutorialComplete)Hud.StartTutorial();
        }
        public void StartLevel(int id)
        {
            if(id<1 || id>Profile.UnlockedLevel) return;
            Profile.RunChallenge=false; Profile.RunEndless=false; Profile.RunLevel=id; Level=StackCampaign.Level(id); NewRun();
        }
        public void StartEndless()
        {
            if(!StackCampaign.EndlessUnlocked(Profile)) return;
            Profile.RunChallenge=false; Profile.RunEndless=true; Level=StackCampaign.Level(1); NewRun();
        }
        public void StartChallenge()
        {
            Profile.RunChallenge=true;Profile.RunEndless=false;
            Profile.RunChallengeDate=DateTime.UtcNow.ToString("yyyy-MM-dd");Level=StackCampaign.Daily(DateTime.UtcNow);NewRun();
        }
        public void UsePower(StackPower power)
        {
            if(Hud.ModalOpen || _paused || Run.Completed || Run.Failed || !StackCampaign.PowerUnlocked(Profile,power) || !Run.Powers.Activate(power))return;
            UiClick();
            if(power==StackPower.Center)
            {
                _phase=0;MovingOffset=0;
                _moving.localPosition=new Vector3(Run.Top.Center.x,Run.Layers.Count*LayerHeight,Run.Top.Center.y);
                _nextDrop=0;Drop();
            }
            Save();Hud.Refresh();
        }
        public void ToggleRisk()
        {
            if(Hud.ModalOpen || _paused || Run.Completed || Run.Failed || Profile.UnlockedLevel<6)return;
            Run.Powers.Risk=!Run.Powers.Risk;Save();Hud.Refresh();
        }
        private void RebuildCity()
        {
            if(_city!=null)Destroy(_city);
            _city=WorldArt.BuildPlayerCity(_world,Profile);_city.SetActive(false);
        }
        public void SetStackVisible(bool active) { if(_tower!=null)_tower.gameObject.SetActive(active); }
        public void SetCityView(bool active)
        {
            if(_city!=null)_city.SetActive(active);if(_tower!=null)_tower.gameObject.SetActive(!active);
            var skyline=_world!=null?_world.Find("Skyline"):null;if(skyline!=null)skyline.gameObject.SetActive(!active);
            if(active)SelectCityDistrict(Mathf.Min(2,(Profile.UnlockedLevel-1)/10));
            else WorldArt.SetChapter(Level.Chapter);
        }
        public void SelectCityDistrict(int district)
        {
            if(district<0 || district>2 || Profile.UnlockedLevel<district*10+1)return;
            CityDistrict=district;WorldArt.SetChapter(district);
            for(var i=0;i<3;i++)_city.transform.Find("CityDistrict"+i).gameObject.SetActive(i==district);
        }
        public void GoHome() { Save(); Hud.ShowHome(); }
        public bool SelectStyle(int style)
        {
            if(!StackCampaign.SelectStyle(Profile,style)) return false;
            RefreshProfileStyle();
            Save(); return true;
        }
        public void RefreshProfileStyle()
        {
            foreach(var block in _placed)WorldArt.StyleStackBlock(block,Profile.Style);
            if(_moving!=null)WorldArt.StyleStackBlock(_moving.gameObject,Profile.Style);
        }
        private void SpawnMoving()
        {
            var top = Run.Top;
            MovingOffset = Mathf.Sin(_phase)*StackRules.MotionExtent(top.Size,Run.Axis);
            var center = top.Center + (Run.Axis == StackAxis.X ? new Vector2(MovingOffset,0) : new Vector2(0,MovingOffset));
            _moving = CreateBlock("MovingBlock",center,top.Size,Run.Layers.Count*LayerHeight,Run.Layers.Count+1).transform;
            WorldArt.MarkSpecialBlock(_moving.gameObject,CurrentKind);
        }
        private GameObject CreateBlock(string name,Vector2 center,Vector2 size,float height,int level)
        {
            var block = WorldArt.CreateStackBlock(_tower,name,new Vector3(center.x,height,center.y),new Vector3(size.x,LayerHeight,size.y),level);
            WorldArt.StyleStackBlock(block,Profile.Style);
            return block;
        }
        private void UpdateCamera(bool snap)
        {
            var top = Run.Top.Center;
            var height = Run.Layers.Count*LayerHeight;
            var cityView=Hud!=null && Hud.CityOpen;
            var focus = new Vector3(top.x,height-1.1f,top.y);
            var paneNow=Hud!=null?Hud.WorldPane:new Rect(0,0,1,1);
            var yaw=_yaw*Mathf.Deg2Rad;
            var outward=new Vector3(Mathf.Sin(yaw),cityView?1.05f:.78f,-Mathf.Cos(yaw)).normalized;
            var rotation=Quaternion.LookRotation(-outward);
            var size=Run.Top.Size;
            var extent=StackRules.MotionExtent(size,Run.Axis);
            var bounds=new Bounds(new Vector3(top.x,height-1.1f,top.y),new Vector3(size.x+(Run.Axis==StackAxis.X?extent*2f:0),3.4f,size.y+(Run.Axis==StackAxis.Z?extent*2f:0)));
            if(cityView)
            {
                focus=new Vector3((CityDistrict-1)*24,0,42);
                bounds=new Bounds(focus+Vector3.up*1.5f,new Vector3(19,12,28));
                paneNow=Hud.CityPane;
                focus=bounds.center;
            }
            var distance=StackPresentation.FitDistance(bounds,focus,rotation,_camera.fieldOfView,_camera.aspect,paneNow);
            var desired=focus+outward*distance;
            var layoutChanged=!Mathf.Approximately(_cameraAspect,_camera.aspect) || _cameraPane!=paneNow;
            _cameraAspect=_camera.aspect;_cameraPane=paneNow;
            var needsOutwardFit=Vector3.Distance(_camera.transform.position,focus)<distance;
            var blend=snap || layoutChanged || needsOutwardFit || GamePreferences.ReducedMotion?1f:1-Mathf.Exp(-7f*Time.deltaTime);
            _camera.transform.position=Vector3.Lerp(_camera.transform.position,desired,blend);
            _camera.transform.rotation=rotation;
            // Shift only the principal point: preserve the metal slabs' proportions.
            _camera.ResetProjectionMatrix();
            var projection=_camera.projectionMatrix;
            projection.m02=1f-2f*paneNow.center.x;projection.m12=1f-2f*paneNow.center.y;
            _camera.projectionMatrix=projection;
            // A rapid orbit or a newly placed layer must not ease through a pose
            // that puts the moving overlap surface behind controls.
            for(var i=0;i<8;i++)
            {
                var corner=bounds.center+Vector3.Scale(bounds.extents,new Vector3((i&1)==0?-1:1,(i&2)==0?-1:1,(i&4)==0?-1:1));
                var point=_camera.WorldToViewportPoint(corner);
                if(point.z<=0 || !paneNow.Contains(new Vector2(point.x,point.y)))
                { _camera.transform.position=desired;break; }
            }
        }
        public void UiClick() => _feedback.PlayJump();
        public void Save()
        {
            if (Profile == null || Run == null) return;
            Profile.ResumeActive = _hasStarted && !Run.Failed && !Run.Completed;
            Profile.Powers=Run.Powers;
            Profile.TotalPlaced=Run.Count; Profile.PerfectDrops=Run.PerfectDrops; Profile.MaxStreak=Run.MaxStreak;
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
