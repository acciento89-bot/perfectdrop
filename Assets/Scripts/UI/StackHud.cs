using UnityEngine;
using UnityEngine.UI;
using Kamilunavo.PerfectDrop.Gameplay;

namespace Kamilunavo.PerfectDrop.UI
{
    public sealed partial class StackHud : MonoBehaviour
    {
        public StackGame Game;
        private RectTransform _safe, _statsRoot, _objective, _dropRect, _popup;
        private Text[] _stats;
        private Text _status, _hint, _popupTitle, _popupBody;
        private Image _progress;
        private Button _drop, _retry, _close, _daily, _sound, _haptics, _motion, _settingsMap;
        private bool _settingsReturnsHome;
        private int _width, _height;
        private Rect _division, _area;
        private bool _hadDivision;
        public Rect WorldPane { get; private set; } = new Rect(0,0,1,1);
        public bool MapOrStyleOpen => CampaignMenuOpen;
        public bool ModalOpen => (_popup != null && _popup.gameObject.activeSelf) || CampaignMenuOpen || CityOpen;
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
                UiFactory.Label(card,"Heading",statNames[i],24,new Vector2(.34f,.63f),new Vector2(.95f,.94f),TextAnchor.MiddleLeft,new Color(.68f,.74f,.84f),FontStyle.Bold);
                _stats[i] = UiFactory.Label(card,"Value","",50,new Vector2(.34f,.12f),new Vector2(.95f,.66f),TextAnchor.MiddleLeft,Color.white,FontStyle.Bold);
            }
            _objective = UiFactory.Panel(_safe,"Objective",Navy,new Vector2(.04f,.765f),new Vector2(.96f,.85f));
            _status = UiFactory.Label(_objective,"Status","PERFECT DROP",35,new Vector2(.045f,.40f),new Vector2(.76f,.94f),TextAnchor.MiddleLeft,Gold,FontStyle.Bold);
            _hint = UiFactory.Label(_objective,"Hint",T("Stapele 30 Blöcke. Tippe zum Absetzen.","Stack 30 blocks. Tap to drop."),24,new Vector2(.045f,.13f),new Vector2(.74f,.44f),TextAnchor.MiddleLeft,Color.white);
            _progress = UiFactory.Progress(_objective,new Vector2(.045f,.04f),new Vector2(.955f,.10f),new Color(.22f,.27f,.35f),Gold);
            UiFactory.Button(_objective,"Menu",T("MENÜ","MENU"),new Color(.14f,.20f,.29f),Color.white,new Vector2(.79f,.15f),new Vector2(.965f,.93f),OpenSettings);
            _drop = UiFactory.Button(_safe,"Drop",T("ABSETZEN","DROP"),Gold,Navy,new Vector2(.10f,.055f),new Vector2(.90f,.15f),game.Drop);
            _dropRect = (RectTransform)_drop.transform;
            UiFactory.ApplyPillImage(_drop.GetComponent<Image>());
            _popup = UiFactory.Panel(_safe,"ContextPanel",Navy,new Vector2(.07f,.23f),new Vector2(.93f,.75f));
            _popupTitle = UiFactory.Label(_popup,"Title","",45,new Vector2(.07f,.79f),new Vector2(.93f,.96f),TextAnchor.MiddleCenter,Gold,FontStyle.Bold);
            _popupBody = UiFactory.Label(_popup,"Body","",29,new Vector2(.07f,.58f),new Vector2(.93f,.79f),TextAnchor.MiddleCenter,Color.white);
            _retry = UiFactory.Button(_popup,"Retry",T("NOCHMAL","PLAY AGAIN"),Gold,Navy,new Vector2(.10f,.13f),new Vector2(.90f,.31f),()=> { Close(); Game.NewRun(); });
            _close = UiFactory.Button(_popup,"Close",T("WEITER","RESUME"),Gold,Navy,new Vector2(.10f,.07f),new Vector2(.90f,.18f),Close);
            _sound = UiFactory.Button(_popup,"Sound","",new Color(.13f,.20f,.30f),Color.white,new Vector2(.10f,.63f),new Vector2(.90f,.74f),()=> { GamePreferences.AudioEnabled = !GamePreferences.AudioEnabled; UpdateSettings(); });
            _haptics = UiFactory.Button(_popup,"Haptics","",new Color(.13f,.20f,.30f),Color.white,new Vector2(.10f,.49f),new Vector2(.90f,.60f),()=> { GamePreferences.HapticsEnabled = !GamePreferences.HapticsEnabled; UpdateSettings(); });
            _motion = UiFactory.Button(_popup,"Motion","",new Color(.13f,.20f,.30f),Color.white,new Vector2(.10f,.35f),new Vector2(.90f,.46f),()=> { GamePreferences.ReducedMotion = !GamePreferences.ReducedMotion; UpdateSettings(); });
            _daily = UiFactory.Button(_popup,"Daily","",new Color(.13f,.20f,.30f),Color.white,new Vector2(.10f,.77f),new Vector2(.90f,.88f),()=> { if (StackSave.ClaimDaily(Game.Profile)) { Game.UiClick(); Refresh(); UpdateSettings(); } });
            _settingsMap=UiFactory.Button(_popup,"SettingsMap",T("LEVELÜBERSICHT","LEVEL MAP"),new Color(.13f,.2f,.3f),Color.white,new Vector2(.10f,.21f),new Vector2(.90f,.32f),game.GoHome);
            BuildCampaignMenus(); BuildArcadeMenus();
            foreach (var graphic in canvas.GetComponentsInChildren<Graphic>(true)) graphic.raycastTarget = graphic.GetComponent<Button>() != null;
            _popup.gameObject.SetActive(false);
            Layout(true); Refresh();
        }

        public void Refresh()
        {
            var gameplayVisible=!CampaignMenuOpen && !CityOpen;
            Game.SetStackVisible(gameplayVisible);
            _statsRoot.gameObject.SetActive(gameplayVisible);_objective.gameObject.SetActive(gameplayVisible);_drop.gameObject.SetActive(gameplayVisible);
            _stats[0].text = Game.Run.Count.ToString() + (Game.Run.Endless?"":"/"+Game.Run.Target);
            _stats[1].text = (Game.Run.Endless?Game.Profile.EndlessBest:Game.Profile.Best).ToString();
            _stats[2].text = "x" + Game.Run.Streak;
            _stats[3].text = Game.Profile.Coins.ToString();
            _progress.fillAmount = Game.Run.Endless?0:Game.Run.Count/(float)Game.Run.Target;
            _drop.interactable = !Game.Run.Failed && !Game.Run.Completed && !ModalOpen;
            UpdateArcadeHud();
        }
        public void Grade(StackGrade grade)
        {
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
            Controls(false);
            _retry.gameObject.SetActive(true);
            _popupTitle.text = won ? Game.Profile.RunChallenge?T("CHALLENGE GESCHAFFT!","CHALLENGE COMPLETE!"):T("LEVEL "+Game.Level.Id+" GESCHAFFT!","LEVEL "+Game.Level.Id+" COMPLETE!") : T("DANEBEN!","MISSED!");
            _popupBody.gameObject.SetActive(true);
            _popupBody.text = T("Gestapelt","Stacked") + " " + Game.Run.Count + (Game.Run.Endless?"":"/"+Game.Run.Target) + "\n" + (won?Game.LastStars+" "+T("STERNE","STARS")+" · "+Game.LastBonus+" "+T("Bonus-Coins","bonus coins")+"\n":"") + Game.Run.EarnedCoins + " " + T("Coins verdient","coins earned");
            ShowTerminalActions(won);
            _popup.gameObject.SetActive(true); Refresh();
        }
        public void OpenSettings()
        {
            if (!CampaignMenuOpen && (Game.Run.Failed || Game.Run.Completed)) return;
            _settingsReturnsHome=CampaignMenuOpen;
            HideMenus(); Game.UiClick(); Controls(true); _retry.gameObject.SetActive(false); _popupBody.gameObject.SetActive(false);
            _popupTitle.text = T("EINSTELLUNGEN","SETTINGS");
            // Settings title and daily control occupy separate rows.
            Set((RectTransform)_popupTitle.transform,.07f,.885f,.93f,.99f);
            UpdateSettings(); _popup.gameObject.SetActive(true); Refresh(); Game.Save();
        }
        public void Close()
        {
            Game.UiClick(); _popup.gameObject.SetActive(false); HideTerminalActions();
            Set((RectTransform)_popupTitle.transform,.07f,.79f,.93f,.96f);
            if(_settingsReturnsHome){_settingsReturnsHome=false;ShowHome();}
            Refresh();
        }
        private void Controls(bool settings)
        {
            foreach (var button in new[] { _close, _sound, _haptics, _motion, _daily, _settingsMap }) button.gameObject.SetActive(settings);
        }
        private void UpdateSettings()
        {
            SetLabel(_sound, T("TON","SOUND") + ": " + GameText.ToggleValue(GamePreferences.AudioEnabled));
            SetLabel(_haptics, T("HAPTIK","HAPTICS") + ": " + GameText.ToggleValue(GamePreferences.HapticsEnabled));
            SetLabel(_motion, T("WENIGER BEWEGUNG","REDUCED MOTION") + ": " + GameText.ToggleValue(GamePreferences.ReducedMotion));
            SetLabel(_daily, StackSave.CanClaim(Game.Profile) ? T("TÄGLICH +25 COINS HOLEN","CLAIM DAILY +25 COINS") : T("HEUTE BEREITS GEHOLT","DAILY CLAIMED"));
            _daily.interactable = StackSave.CanClaim(Game.Profile);
        }
        private static void SetLabel(Button button, string text) => button.GetComponentInChildren<Text>().text = text;
        private void Update() { Layout(false); UpdateArcadeHud(); }
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
            LayoutCampaign(pane); LayoutArcade(pane);
            var screenSafe = Screen.safeArea;
            WorldPane = new Rect((screenSafe.x+pane.x*screenSafe.width)/Screen.width, (screenSafe.y+pane.y*screenSafe.height)/Screen.height, pane.width*screenSafe.width/Screen.width, pane.height*screenSafe.height/Screen.height);
            Place(_statsRoot,pane,new Rect(.04f,.875f,.92f,.105f));
            Place(_objective,pane,new Rect(.04f,.76f,.92f,.09f));
            Place(_dropRect,pane,new Rect(.10f,.05f,.80f,.10f));
            Place(_popup,pane,new Rect(.06f,.16f,.88f,.70f));
            var aspect = Screen.width * pane.width / Mathf.Max(1, Screen.height*pane.height);
            if (aspect > 1.2f)
            {
                Place(_statsRoot,pane,new Rect(.025f,.80f,.55f,.17f));
                Place(_objective,pane,new Rect(.60f,.77f,.37f,.20f));
                Place(_dropRect,pane,new Rect(.69f,.09f,.27f,.24f));
                Place(_popup,pane,new Rect(.05f,.025f,.90f,.95f));
            }
        }
        private static void Place(RectTransform rect, Rect pane, Rect slot) => Set(rect,pane.x+slot.x*pane.width,pane.y+slot.y*pane.height,pane.x+slot.xMax*pane.width,pane.y+slot.yMax*pane.height);
        private static void Set(RectTransform rect,float x,float y,float xmax,float ymax)
        {
            rect.anchorMin = new Vector2(x,y); rect.anchorMax = new Vector2(xmax,ymax); rect.offsetMin = rect.offsetMax = Vector2.zero;
        }
    }
}
