using UnityEngine;
using UnityEngine.UI;
using Kamilunavo.PerfectDrop.Gameplay;

namespace Kamilunavo.PerfectDrop.UI
{
    public sealed partial class StackHud : MonoBehaviour
    {
        public StackGame Game;
        private RectTransform _safe, _statsRoot, _objective, _dropRect, _popup, _menuRect;
        private Text[] _stats;
        private Text _status, _hint, _popupTitle, _popupBody;
        private Image _progress;
        private Button _drop, _retry, _close, _daily, _sound, _music, _haptics, _motion, _settingsMap;
        private bool _settingsReturnsHome;
        private int _width, _height;
        private Rect _division, _area;
        private bool _hadDivision;
        public Rect CityPane { get; private set; } = new Rect(0,0,1,1);
        public Rect WorldPane { get; private set; } = new Rect(0,0,1,1);
        public bool MapOrStyleOpen => CampaignMenuOpen;
        public bool ResultOpen=>_popup!=null&&_popup.gameObject.activeSelf&&Game.Run.Completed;
        public bool ModalOpen => (_popup != null && _popup.gameObject.activeSelf) || CampaignMenuOpen || CityOpen || (Game.Purchases?.IsPresenting??false) || (Game.Videos?.IsPresenting??false) || (Game.Interstitials?.IsPresenting??false);
        private static Color Navy => new(0.035f, 0.065f, 0.12f, 1f);
        private static Color Gold => new(1f, 0.79f, 0.16f);
        public static string T(string de, string en) => GameText.German ? de : en;

        public void Build(StackGame game)
        {
            Game = game;
            var canvas = UiFactory.CreateCanvas();
            _safe = UiFactory.Panel(canvas.transform, "StackSafeArea", Color.clear, Vector2.zero, Vector2.one);
            _safe.gameObject.AddComponent<SafeAreaFitter>();
            _statsRoot = UiFactory.Panel(_safe, "Stats", Color.clear, new Vector2(.04f,.87f), new Vector2(.96f,.98f));
            _stats = new Text[4];
            var types = new[] { HudIconType.Floors, HudIconType.Crown, HudIconType.Flame, HudIconType.Diamond };
            var statNames=new[]{T("STAPEL","STACK"),T("BESTE","BEST"),T("SERIE","STREAK"),"COINS"};
            for (var i = 0; i < 4; i++)
            {
                var min = i * .25f;
                var card = UiFactory.Panel(_statsRoot, "Stat"+i, Navy, new Vector2(min,0), new Vector2(min+.235f,1));
                UiFactory.Icon(card, "Icon", types[i], Gold, new Vector2(.09f,.32f), new Vector2(.29f,.68f));
                UiFactory.Label(card,"Heading",statNames[i],24,new Vector2(.34f,.70f),new Vector2(.95f,.94f),TextAnchor.MiddleLeft,new Color(.68f,.74f,.84f),FontStyle.Bold);
                _stats[i] = UiFactory.Label(card,"Value","",50,new Vector2(.34f,.12f),new Vector2(.95f,.60f),TextAnchor.MiddleLeft,Color.white,FontStyle.Bold);
            }
            _objective = UiFactory.Panel(_safe,"Objective",Navy,new Vector2(.04f,.765f),new Vector2(.96f,.85f));
            _status = UiFactory.Label(_objective,"Status","PERFECT DROP",35,new Vector2(.045f,.48f),new Vector2(.71f,.94f),TextAnchor.MiddleLeft,Gold,FontStyle.Bold);
            _hint = UiFactory.Label(_objective,"Hint",T("Stapele 30 Blöcke. Tippe zum Absetzen.","Stack 30 blocks. Tap to drop."),24,new Vector2(.045f,.15f),new Vector2(.71f,.38f),TextAnchor.MiddleLeft,Color.white);
            _progress = UiFactory.Progress(_objective,new Vector2(.045f,.04f),new Vector2(.955f,.10f),new Color(.22f,.27f,.35f),Gold);
            _menuRect=(RectTransform)UiFactory.Button(_statsRoot,"Menu","Ⅱ",Navy,Gold,Vector2.zero,Vector2.one,OpenSettings).transform;
            UiFactory.ApplyCircularImage(_menuRect.GetComponent<Image>());
            _objective.GetComponent<Image>().color=Color.clear;_objective.GetComponent<Image>().material=null;
            _drop = UiFactory.Button(_safe,"Drop",T("ABSETZEN","DROP"),Gold,Navy,new Vector2(.10f,.055f),new Vector2(.90f,.15f),game.Drop);
            _dropRect = (RectTransform)_drop.transform;_drop.GetComponentInChildren<Text>().font=UiFactory.SerifFont;_drop.GetComponentInChildren<Text>().fontStyle=FontStyle.Normal;_status.font=UiFactory.SerifFont;_status.fontStyle=FontStyle.Normal;UiFactory.AddOutline(_status,new Color(.02f,.015f,.01f,.65f),1);
            UiFactory.ApplyPillImage(_drop.GetComponent<Image>());
            _popup = UiFactory.Panel(_safe,"ContextPanel",Navy,new Vector2(.07f,.23f),new Vector2(.93f,.75f));
            _popupTitle = UiFactory.Label(_popup,"Title","",45,new Vector2(.07f,.79f),new Vector2(.93f,.96f),TextAnchor.MiddleCenter,Gold,FontStyle.Bold);
            _popupBody = UiFactory.Label(_popup,"Body","",29,new Vector2(.07f,.58f),new Vector2(.93f,.79f),TextAnchor.MiddleCenter,Color.white);
            _retry = UiFactory.Button(_popup,"Retry",T("NOCHMAL","PLAY AGAIN"),Gold,Navy,new Vector2(.10f,.13f),new Vector2(.90f,.31f),()=> { if(MenuNavigationLocked)return;Close(); Game.NewRun(); });
            _close = UiFactory.Button(_popup,"Close",T("WEITER","RESUME"),Gold,Navy,new Vector2(.10f,.07f),new Vector2(.90f,.18f),Close);
            _sound = UiFactory.Button(_popup,"Sound","",new Color(.13f,.20f,.30f),Color.white,new Vector2(.10f,.63f),new Vector2(.90f,.74f),()=> { if(MenuNavigationLocked)return;GamePreferences.AudioEnabled = !GamePreferences.AudioEnabled; UpdateSettings(); });
            _music = UiFactory.Button(_popup,"Music","",new Color(.13f,.20f,.30f),Color.white,new Vector2(.10f,.63f),new Vector2(.90f,.74f),()=> { if(MenuNavigationLocked)return;GamePreferences.MusicEnabled = !GamePreferences.MusicEnabled; UpdateSettings(); });
            _haptics = UiFactory.Button(_popup,"Haptics","",new Color(.13f,.20f,.30f),Color.white,new Vector2(.10f,.49f),new Vector2(.90f,.60f),()=> { if(MenuNavigationLocked)return;GamePreferences.HapticsEnabled = !GamePreferences.HapticsEnabled; UpdateSettings(); });
            _motion = UiFactory.Button(_popup,"Motion","",new Color(.13f,.20f,.30f),Color.white,new Vector2(.10f,.35f),new Vector2(.90f,.46f),()=> { if(MenuNavigationLocked)return;GamePreferences.ReducedMotion = !GamePreferences.ReducedMotion; UpdateSettings(); });
            _daily = UiFactory.Button(_popup,"Daily","",new Color(.13f,.20f,.30f),Color.white,new Vector2(.10f,.77f),new Vector2(.90f,.88f),()=> { if (StackSave.ClaimDaily(Game.Profile)) { Game.UiClick(); Refresh(); UpdateSettings(); } });
            _settingsMap=UiFactory.Button(_popup,"SettingsMap",T("LEVELÜBERSICHT","LEVEL MAP"),new Color(.13f,.2f,.3f),Color.white,new Vector2(.10f,.21f),new Vector2(.90f,.32f),game.GoHome);
            BuildCampaignMenus(); BuildArcadeMenus(); BuildCommerceMenu(); BuildTutorial();
            foreach (var graphic in canvas.GetComponentsInChildren<Graphic>(true)) graphic.raycastTarget = graphic.GetComponent<Button>() != null || graphic.GetComponent<RectMask2D>() != null;
            _popup.gameObject.SetActive(false);
            Layout(true); Refresh();
        }

        public void Refresh()
        {
            var gameplayVisible=!CampaignMenuOpen && !CityOpen;
            Game.SetStackVisible(gameplayVisible);
            _statsRoot.gameObject.SetActive(gameplayVisible);_objective.gameObject.SetActive(gameplayVisible);_drop.gameObject.SetActive(gameplayVisible);
            _stats[0].text = T("STAPEL ","STACK ")+Game.Run.Count.ToString() + (Game.Run.Endless?"":" / "+Game.Run.Target);
            _stats[1].text = (Game.Run.Endless?Game.Profile.EndlessBest:Game.Profile.Best).ToString();
            _stats[2].text = "x" + Game.Run.Streak;
            _stats[3].text = Game.Profile.Coins.ToString();
            _progress.fillAmount = Game.Run.Endless?0:Game.Run.Count/(float)Game.Run.Target;
            _drop.interactable = !Game.Run.Failed && !Game.Run.Completed && !ModalOpen;
            UpdateArcadeHud();
        }
        public void Grade(StackGrade grade)
        {
            TutorialPlacement(grade);
            _status.text = grade == StackGrade.Perfect ? T("PERFEKT!","PERFECT!") : grade == StackGrade.Good ? T("GUT GESTAPELT","GOOD DROP") : T("DANEBEN","MISSED");
            _hint.text = grade == StackGrade.Perfect ? T("Fläche bleibt erhalten · Serienbonus","Area preserved · streak bonus") : grade == StackGrade.Good ? T("Überstand fällt herunter.","Overhang falls away.") : T("Kein Überlapp. Versuch es nochmal.","No overlap. Try again.");
        }
        public void ResetMessage()
        {
            _status.text = Game.Run.Endless?T("ENDLOS","ENDLESS"):Game.Profile.RunChallenge?T("TAGES-CHALLENGE","DAILY CHALLENGE"):"LEVEL "+Game.Level.Id;
            _hint.text = GoalText(Game.Level,Game.Run.Endless);
            Refresh();
        }
        public void Terminal(bool won)
        {
            Controls(false); Layout(true);
            _popupTitle.fontSize=_popupTitle.resizeTextMaxSize=45;
            _retry.gameObject.SetActive(true);
            _popupTitle.text = won ? Game.Profile.RunChallenge?T("CHALLENGE GESCHAFFT!","CHALLENGE COMPLETE!"):T("LEVEL "+Game.Level.Id+" GESCHAFFT!","LEVEL "+Game.Level.Id+" COMPLETE!") : T("DANEBEN!","MISSED!");
            _popupBody.gameObject.SetActive(true);
            _popupBody.text = T("Gestapelt","Stacked") + " " + Game.Run.Count + (Game.Run.Endless?"":"/"+Game.Run.Target) + "\n" + (won?Game.LastStars+" "+T("STERNE","STARS")+" · "+Game.LastBonus+" "+T("Bonus-Coins","bonus coins")+"\n":"") + Game.Run.EarnedCoins + " " + T("Coins verdient","coins earned");
            ShowTerminalActions(won);
            _popup.gameObject.SetActive(true); Refresh();
        }
        public void OpenSettings()
        {
            if(MenuNavigationLocked)return;
            if (!CampaignMenuOpen && (Game.Run.Failed || Game.Run.Completed)) return;
            _settingsReturnsHome=CampaignMenuOpen;
            HideMenus(); Game.UiClick(); Controls(true); _retry.gameObject.SetActive(false); _popupBody.gameObject.SetActive(false);
            _popupTitle.text = T("EINSTELLUNGEN","SETTINGS");
            // Settings title and daily control occupy separate rows.
            Set((RectTransform)_popupTitle.transform,.07f,.885f,.93f,.99f);
            UpdateSettings(); _popup.gameObject.SetActive(true); Layout(true); Refresh(); Game.Save();
        }
        public void Close()
        {
            if(MenuNavigationLocked)return;
            Game.UiClick(); _popup.gameObject.SetActive(false); HideTerminalActions();
            Set((RectTransform)_popupTitle.transform,.07f,.79f,.93f,.96f);
            if(_settingsReturnsHome){_settingsReturnsHome=false;ShowHome();}
            Refresh();
        }
        private void Controls(bool settings)
        {
            foreach (var button in new[] { _close, _sound, _music, _haptics, _motion, _daily, _settingsMap }) button.gameObject.SetActive(settings);
        }
        private void UpdateSettings()
        {
            SetLabel(_sound, T("TON","SOUND") + ": " + GameText.ToggleValue(GamePreferences.AudioEnabled));
            SetLabel(_music, T("MUSIK","MUSIC") + ": " + GameText.ToggleValue(GamePreferences.MusicEnabled));
            SetLabel(_haptics, T("HAPTIK","HAPTICS") + ": " + GameText.ToggleValue(GamePreferences.HapticsEnabled));
            SetLabel(_motion, T("WENIGER BEWEGUNG","REDUCED MOTION") + ": " + GameText.ToggleValue(GamePreferences.ReducedMotion));
            SetLabel(_daily, StackSave.CanClaim(Game.Profile) ? T("TÄGLICH +25 COINS HOLEN","CLAIM DAILY +25 COINS") : T("HEUTE BEREITS GEHOLT","DAILY CLAIMED"));
            _daily.interactable = StackSave.CanClaim(Game.Profile);
        }
        private static void SetLabel(Button button, string text) => button.GetComponentInChildren<Text>().text = text;
        private void Update() { Layout(false); UpdateArcadeHud(); UpdateCommerce(); UpdateTutorial(); }
        private void Layout(bool force)
        {
            var hasDivision = ReservedRegionProvider.TryGetDivisionRegion(out var division);
            if (!force && _width == Screen.width && _height == Screen.height && _area == Screen.safeArea && _hadDivision == hasDivision && division == _division) return;
            _width = Screen.width; _height = Screen.height; _area = Screen.safeArea; _division = division; _hadDivision = hasDivision;
            var pane = new Rect(0,0,1,1);
            if (hasDivision)
            {
                var safe = Screen.safeArea;
                var d = new Rect((division.x*Screen.width-safe.x)/safe.width,(division.y*Screen.height-safe.y)/safe.height,division.width*Screen.width/safe.width,division.height*Screen.height/safe.height);
                if (d.height >= d.width)
                    pane = d.x >= 1-d.xMax ? new Rect(0,0,Mathf.Max(.05f,d.x-.015f),1) : new Rect(d.xMax+.015f,0,Mathf.Max(.05f,1-d.xMax-.015f),1);
                else
                    pane = d.y >= 1-d.yMax ? new Rect(0,0,1,Mathf.Max(.05f,d.y-.015f)) : new Rect(0,d.yMax+.015f,1,Mathf.Max(.05f,1-d.yMax-.015f));
            }
            LayoutCampaign(pane); LayoutArcade(pane); LayoutCommerce(pane);
            var screenSafe = Screen.safeArea;
            var landscape = Screen.width*pane.width > Screen.height*pane.height*1.2f;
            var free = StackPresentation.World(landscape);
            WorldPane=StackPresentation.ToViewport(screenSafe,pane,free,new Vector2(Screen.width,Screen.height));
            CityPane=StackPresentation.ToViewport(screenSafe,pane,new Rect(.04f,.20f,.92f,.48f),new Vector2(Screen.width,Screen.height));
            Place(_dropRect,pane,new Rect(.075f,.045f,.735f,.10f));
            Place(_popup,pane,new Rect(.06f,.16f,.88f,.70f));
            Set((RectTransform)_daily.transform,.10f,.77f,.90f,.88f);
            Set((RectTransform)_sound.transform,.10f,.63f,.90f,.74f);Set((RectTransform)_haptics.transform,.10f,.49f,.90f,.60f);
            Set((RectTransform)_motion.transform,.10f,.35f,.90f,.46f);Set((RectTransform)_settingsMap.transform,.10f,.21f,.90f,.32f);Set((RectTransform)_close.transform,.10f,.07f,.90f,.18f);
            var aspect = Screen.width * pane.width / Mathf.Max(1, Screen.height*pane.height);
            if (aspect > 1.2f)
            {
                Place(_dropRect,pane,StackPresentation.Drop(true));
                Place(_popup,pane,new Rect(.23f,.035f,.54f,.93f));
                Set((RectTransform)_daily.transform,.06f,.68f,.94f,.83f);
                Set((RectTransform)_sound.transform,.06f,.505f,.48f,.655f);Set((RectTransform)_haptics.transform,.52f,.505f,.94f,.655f);
                Set((RectTransform)_motion.transform,.06f,.33f,.94f,.48f);
                Set((RectTransform)_settingsMap.transform,.06f,.155f,.48f,.305f);Set((RectTransform)_close.transform,.52f,.155f,.94f,.305f);
            }
            LayoutGameplayHeader(pane,landscape);
            if(_sound.gameObject.activeSelf)LayoutMusicSettings(pane);
        }
        private void LayoutGameplayHeader(Rect pane,bool landscape)
        {
            Place(_statsRoot,pane,landscape?new Rect(.025f,.76f,.95f,.20f):new Rect(.035f,.91f,.93f,.065f));
            Place(_objective,pane,landscape?new Rect(.43f,.66f,.54f,.12f):new Rect(.37f,.75f,.58f,.12f));
            _objective.GetComponent<Image>().color=Color.clear;
            float target=UiMetrics.TargetSize(_safe,49);
            _menuRect.anchorMin=_menuRect.anchorMax=new Vector2(0,.5f);_menuRect.pivot=new Vector2(0,.5f);
            _menuRect.sizeDelta=new Vector2(target,target);_menuRect.anchoredPosition=Vector2.zero;
            var pause=_menuRect.GetComponent<Button>();GalleryTheme.Style(pause,Navy);
            pause.GetComponentInChildren<Text>().color=Gold;GalleryTheme.Readable(pause.GetComponentInChildren<Text>(),25);
            for(int i=0;i<4;i++)_stats[i].transform.parent.gameObject.SetActive(i==0||i==3);
            var stage=(RectTransform)_stats[0].transform.parent;var wallet=(RectTransform)_stats[3].transform.parent;
            Set(stage,.30f,.10f,.70f,.91f);Set(wallet,.76f,.16f,1,.88f);
            foreach(var card in new[]{stage,wallet}){card.GetComponent<Image>().material=null;UiFactory.ApplyPillImage(card.GetComponent<Image>());GalleryFrameGraphic.Apply(card,Gold);card.GetComponent<Image>().color=Navy;}
            stage.Find("Icon").gameObject.SetActive(false);stage.Find("Heading").gameObject.SetActive(false);
            wallet.Find("Heading").gameObject.SetActive(false);
            Set((RectTransform)_stats[0].transform,.04f,.04f,.96f,.96f);_stats[0].alignment=TextAnchor.MiddleCenter;_stats[0].font=UiFactory.SerifFont;_stats[0].fontStyle=FontStyle.Normal;GalleryTheme.Readable(_stats[0],16);
            Set((RectTransform)_stats[3].transform,.28f,.08f,.96f,.92f);GalleryTheme.Readable(_stats[3],18);
            Set((RectTransform)wallet.Find("Icon"),.06f,.16f,.27f,.84f);
            Set((RectTransform)_status.transform,.02f,.52f,.98f,.98f);_status.alignment=TextAnchor.MiddleRight;_status.color=new Color(.98f,.85f,.45f);GalleryTheme.Readable(_status,30);
            Set((RectTransform)_hint.transform,.02f,.05f,.98f,.48f);_hint.alignment=TextAnchor.MiddleRight;GalleryTheme.Readable(_hint,12);
            var track=(RectTransform)_progress.transform.parent;
            track.SetParent(stage,false);Set(track,.10f,.025f,.90f,.075f);
            // Reserve Risk at its final size before assigning the remaining drop width.
            float usedW=_safe.rect.width*pane.width,usedH=_safe.rect.height*pane.height;
            var risk=(RectTransform)_risk.transform;float bottom=landscape?.045f:.045f;
            float actionH=Mathf.Max(UiMetrics.TargetSize(_safe,54),usedH*(landscape?.27f:.10f));
            float center=pane.y+bottom*pane.height+actionH/_safe.rect.height*.5f;
            risk.anchorMin=risk.anchorMax=new Vector2(pane.x+pane.width*.965f,center);risk.pivot=new Vector2(1,.5f);risk.sizeDelta=new Vector2(target,actionH);risk.anchoredPosition=Vector2.zero;
            float dropX=landscape?.33f:.075f;
            _dropRect.anchorMin=_dropRect.anchorMax=new Vector2(pane.x+pane.width*dropX,center);_dropRect.pivot=new Vector2(0,.5f);_dropRect.sizeDelta=new Vector2(Mathf.Max(target,usedW*(.965f-dropX)-target-4*UiMetrics.PointScale/Mathf.Max(.001f,_safe.GetComponentInParent<Canvas>().scaleFactor)),actionH);_dropRect.anchoredPosition=Vector2.zero;
            GalleryTheme.Style(_drop,Gold);GalleryTheme.Readable(_drop.GetComponentInChildren<Text>(),24);
            GalleryTheme.Style(_risk,Navy);GalleryTheme.Readable(_risk.GetComponentInChildren<Text>(),10);


        }
        // A separate music control adds one row; short panes use pairs to retain full logical targets.
        private void LayoutMusicSettings(Rect pane)
        {
            Canvas.ForceUpdateCanvases();float availableW=_safe.rect.width*pane.width,availableH=_safe.rect.height*pane.height,target=UiMetrics.TargetSize(_safe,49),gap=6,pad=12,header=50;
            bool pairs=availableH<7*(target+gap)+header+pad*2&&availableW>=target*2+pad*2+gap;int rows=pairs?4:7;float w=Mathf.Min(540,availableW-24),h=Mathf.Min(availableH-24,header+rows*(target+gap)+pad);
            _popup.anchorMin=_popup.anchorMax=new Vector2(pane.x,pane.y+pane.height);_popup.pivot=new Vector2(0,1);_popup.anchoredPosition=new Vector2((availableW-w)/2,-(availableH-h)/2);_popup.sizeDelta=new Vector2(w,h);
            void At(RectTransform r,float x,float y,float width,float height){r.anchorMin=r.anchorMax=new Vector2(0,1);r.pivot=new Vector2(0,1);r.anchoredPosition=new Vector2(x,-y);r.sizeDelta=new Vector2(width,height);}
            At((RectTransform)_popupTitle.transform,pad,8,w-pad*2,34);_popupTitle.fontSize=_popupTitle.resizeTextMaxSize=24;
            var controls=new[]{_daily,_sound,_music,_haptics,_motion,_settingsMap,_close};float inner=w-pad*2;
            for(int i=0;i<controls.Length;i++){int row=pairs?(i==0?0:(i+1)/2):i,col=pairs&&i>0?(i-1)%2:0;float bw=pairs&&i>0?(inner-gap)/2:inner;At((RectTransform)controls[i].transform,pad+col*(bw+gap),header+row*(target+gap),bw,target);var label=controls[i].GetComponentInChildren<Text>();label.fontSize=label.resizeTextMaxSize=18;}
        }
        private static void Place(RectTransform rect, Rect pane, Rect slot) => Set(rect,pane.x+slot.x*pane.width,pane.y+slot.y*pane.height,pane.x+slot.xMax*pane.width,pane.y+slot.yMax*pane.height);
        private static void Set(RectTransform rect,float x,float y,float xmax,float ymax)
        {
            rect.anchorMin = new Vector2(x,y); rect.anchorMax = new Vector2(xmax,ymax); rect.offsetMin = rect.offsetMax = Vector2.zero;
        }
    }
}
