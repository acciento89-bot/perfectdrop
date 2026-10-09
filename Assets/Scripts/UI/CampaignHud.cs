using UnityEngine;
using UnityEngine.UI;
using Kamilunavo.PerfectDrop.Gameplay;
namespace Kamilunavo.PerfectDrop.UI
{
    public sealed partial class StackHud
    {
        private RectTransform _home,_styles,_levelViewport,_levelContent;
        private ScrollRect _levelScroll;
        private Text _homeInfo,_styleInfo;
        private Button[] _levels,_chapters,_styleButtons;
        private HudIconGraphic[] _levelIcons;
        private TowerPreviewGraphic[] _levelTowers;
        private HudIconGraphic[,] _levelStars;
        private Button _endless,_continue,_next,_map;
        private int _chapter;
        private bool CampaignMenuOpen => ShopOpen || (_home!=null && _home.gameObject.activeSelf) || (_styles!=null && _styles.gameObject.activeSelf);
        private void BuildCampaignMenus()
        {
            _home=UiFactory.Panel(_safe,"LevelMap",Color.clear,Vector2.zero,Vector2.one);
            UiFactory.Panel(_home,"MapHeader",new Color(.025f,.045f,.08f,.92f),new Vector2(.025f,.80f),new Vector2(.975f,.99f));
            TowerPreviewGraphic.Add(_home,"HeroTower",Gold,new Vector2(.65f,.81f),new Vector2(.94f,.99f),5);
            UiFactory.Label(_home,"Tagline",T("DEIN TIMING. DEINE SKYLINE.","YOUR TIMING. YOUR SKYLINE."),23,new Vector2(.06f,.835f),new Vector2(.65f,.875f),TextAnchor.MiddleLeft,new Color(.7f,.77f,.85f),FontStyle.Bold);
            UiFactory.Panel(_home,"MapFooter",new Color(.025f,.045f,.08f,.80f),new Vector2(.025f,.012f),new Vector2(.975f,.335f));
            UiFactory.Label(_home,"Title","PERFECT DROP",49,new Vector2(.06f,.92f),new Vector2(.68f,.985f),TextAnchor.MiddleLeft,Gold,FontStyle.Bold);
            _homeInfo=UiFactory.Label(_home,"Progress","",27,new Vector2(.06f,.88f),new Vector2(.68f,.925f),TextAnchor.MiddleLeft,Color.white);
            _chapters=new Button[3];
            var names=new[]{T("WOLKENWERK","CLOUD WORKS"),T("ABENDTURM","SUNSET SPIRE"),T("NEONSTADT","NEON CITY")};
            for(var i=0;i<3;i++)
            {
                var chapter=i;
                _chapters[i]=UiFactory.Button(_home,"Chapter"+i,names[i],new Color(.14f,.2f,.3f),Color.white,new Vector2(.05f+i*.30f,.745f),new Vector2(.335f+i*.30f,.82f),()=>{_chapter=chapter;RefreshHome();});
            }
            var levelScrollObject=new GameObject("LevelGalleryScroll",typeof(RectTransform),typeof(ScrollRect));levelScrollObject.transform.SetParent(_home,false);
            _levelScroll=levelScrollObject.GetComponent<ScrollRect>();_levelScroll.horizontal=false;_levelScroll.movementType=ScrollRect.MovementType.Clamped;
            _levelViewport=UiFactory.Panel(levelScrollObject.transform,"LevelGalleryViewport",Color.clear,Vector2.zero,Vector2.one);_levelViewport.gameObject.AddComponent<RectMask2D>();
            _levelContent=new GameObject("LevelGalleryContent",typeof(RectTransform)).GetComponent<RectTransform>();_levelContent.SetParent(_levelViewport,false);_levelContent.anchorMin=new Vector2(0,1);_levelContent.anchorMax=Vector2.one;_levelContent.pivot=new Vector2(.5f,1);
            _levelScroll.viewport=_levelViewport;_levelScroll.content=_levelContent;
            _levels=new Button[10]; _levelIcons=new HudIconGraphic[10]; _levelTowers=new TowerPreviewGraphic[10]; _levelStars=new HudIconGraphic[10,3];
            for(var i=0;i<10;i++)
            {
                var slot=i;
                _levels[i]=UiFactory.Button(_levelContent,"LevelSlot"+i,"",new Color(.045f,.075f,.13f,.96f),Color.white,Vector2.zero,Vector2.one,()=>Game.StartLevel(_chapter*10+slot+1));
                var card=(RectTransform)_levels[i].transform;
                var label=(RectTransform)card.Find("Label"); Set(label,.24f,.12f,.95f,.90f);
                _levelIcons[i]=UiFactory.Icon(card,"LevelIcon",HudIconType.Floors,Gold,new Vector2(.035f,.22f),new Vector2(.23f,.83f));
                _levelTowers[i]=TowerPreviewGraphic.Add(card,"LevelTower",Gold,new Vector2(.01f,.19f),new Vector2(.255f,.94f),3+i%3);
                for(var star=0;star<3;star++)
                    _levelStars[i,star]=UiFactory.Icon(card,"Star"+star,HudIconType.Star,Gold,new Vector2(.035f+star*.062f,.07f),new Vector2(.093f+star*.062f,.29f));
            }
            _continue=UiFactory.Button(_home,"Continue",T("LAUF FORTSETZEN","CONTINUE RUN"),Gold,Navy,new Vector2(.07f,.22f),new Vector2(.93f,.29f),StartOrContinue);
            _endless=UiFactory.Button(_home,"Endless","",new Color(.14f,.2f,.3f),Color.white,new Vector2(.07f,.13f),new Vector2(.93f,.20f),Game.StartEndless);
            UiFactory.Button(_home,"Styles",T("TURM-DESIGNS","TOWER DESIGNS"),new Color(.14f,.2f,.3f),Color.white,new Vector2(.07f,.04f),new Vector2(.93f,.11f),ShowStyles);
            _styles=UiFactory.Panel(_safe,"StyleShop",new Color(.025f,.045f,.08f,.78f),Vector2.zero,Vector2.one);
            UiFactory.Label(_styles,"Title",T("TURM-DESIGNS","TOWER DESIGNS"),50,new Vector2(.06f,.9f),new Vector2(.94f,.98f),TextAnchor.MiddleCenter,Gold,FontStyle.Bold);
            _styleInfo=UiFactory.Label(_styles,"Wallet","",34,new Vector2(.06f,.82f),new Vector2(.94f,.89f),TextAnchor.MiddleCenter,Color.white);
            _styleButtons=new Button[4];
            for(var i=0;i<4;i++)
            {
                var style=i;
                _styleButtons[i]=UiFactory.Button(_styles,"Style"+i,"",new Color(.045f,.075f,.13f,.96f),Color.white,new Vector2(.08f,.66f-i*.155f),new Vector2(.92f,.79f-i*.155f),()=>{if(Game.SelectStyle(style))Game.UiClick();RefreshStyles();});
                var styleRect=(RectTransform)_styleButtons[i].transform;
                Set((RectTransform)styleRect.Find("Label"),.25f,.05f,.96f,.95f);
                TowerPreviewGraphic.AddStyle(styleRect,"StyleTower",i,new Vector2(.06f,.24f),new Vector2(.94f,.98f));
                Set((RectTransform)styleRect.Find("Label"),.05f,.025f,.95f,.25f);
                styleRect.Find("Label").GetComponent<Text>().fontSize=27;
            }
            UiFactory.Button(_styles,"Back",T("ZURÜCK ZU LEVELS","BACK TO LEVELS"),Gold,Navy,new Vector2(.08f,.04f),new Vector2(.92f,.15f),ShowHome);
            _next=UiFactory.Button(_popup,"Next",T("NÄCHSTES LEVEL","NEXT LEVEL"),Gold,Navy,new Vector2(.10f,.43f),new Vector2(.90f,.56f),()=>Game.StartLevel(Game.Level.Id+1));
            _map=UiFactory.Button(_popup,"LevelMapAction",T("LEVELÜBERSICHT","LEVEL MAP"),new Color(.14f,.2f,.3f),Color.white,new Vector2(.10f,.32f),new Vector2(.90f,.42f),Game.GoHome);
            _home.gameObject.SetActive(false); _styles.gameObject.SetActive(false); HideTerminalActions();
        }
        public void HideMenus()
        {
            HideArcadeMenus();if(_shop!=null)_shop.gameObject.SetActive(false);
            _home.gameObject.SetActive(false);_styles.gameObject.SetActive(false);_popup.gameObject.SetActive(false);HideTerminalActions();
            Set((RectTransform)_popupTitle.transform,.07f,.79f,.93f,.96f); Refresh();
        }
        public void ShowHome()
        {
            _settingsReturnsHome=false;HideMenus();_home.gameObject.SetActive(true);RefreshHome();Refresh();
        }
        private void RefreshHome()
        {
            Kamilunavo.PerfectDrop.Visuals.WorldArt.SetChapter(_chapter);
            var stars=0;foreach(var score in Game.Profile.LevelStars)stars+=score;
            _homeInfo.text=stars+" / 90 "+T("STERNE","STARS")+" · "+Game.Profile.Coins+" COINS";
            for(var i=0;i<3;i++)
            { _chapters[i].interactable=Game.Profile.UnlockedLevel>i*10;_chapters[i].GetComponent<Image>().color=i==_chapter?Gold:new Color(.045f,.075f,.13f,.96f); _chapters[i].GetComponentInChildren<Text>().color=i==_chapter?Navy:Color.white; }
            for(var i=0;i<10;i++)
            {
                var id=_chapter*10+i+1; var unlocked=id<=Game.Profile.UnlockedLevel;
                var starsHere=Game.Profile.LevelStars[id-1];
                var current=unlocked && id==Game.Profile.UnlockedLevel;
                SetLabel(_levels[i],"LEVEL "+id+(current?"  ›":"")+"\n"+(unlocked?StackCampaign.Level(id).Target+" "+T("BLÖCKE","BLOCKS"):T("GESPERRT","LOCKED")));
                _levels[i].GetComponent<Image>().color=current?new Color(.30f,.20f,.055f,.98f):new Color(.045f,.075f,.13f,.96f);
                _levelIcons[i].IconType=unlocked?HudIconType.Floors:HudIconType.Lock;
                _levelIcons[i].color=unlocked?Gold:new Color(.46f,.54f,.66f); _levelIcons[i].SetVerticesDirty();
                _levelIcons[i].gameObject.SetActive(!unlocked);_levelTowers[i].gameObject.SetActive(unlocked);
                for(var star=0;star<3;star++)_levelStars[i,star].color=star<starsHere?Gold:new Color(.20f,.27f,.36f);
                _levels[i].interactable=unlocked;
            }
            var endless=StackCampaign.EndlessUnlocked(Game.Profile);
            SetLabel(_endless,endless?T("ENDLOS · BESTE ","ENDLESS · BEST ")+Game.Profile.EndlessBest:T("ENDLOS: LEVEL 5 ABSCHLIESSEN","ENDLESS: COMPLETE LEVEL 5"));
            _endless.interactable=endless;RefreshArcadeHome();
            _continue.gameObject.SetActive(true);
            SetLabel(_continue,Game.Profile.ResumeActive && !Game.Run.Failed && !Game.Run.Completed?T("FORTSETZEN","CONTINUE"):T("SPIELEN · LEVEL ","PLAY · LEVEL ")+Game.Profile.UnlockedLevel);
        }
        private void StartOrContinue() { if(Game.Profile.ResumeActive && !Game.Run.Failed && !Game.Run.Completed)HideMenus();else Game.StartLevel(Game.Profile.UnlockedLevel); }
        private void ShowStyles() { HideMenus();_styles.gameObject.SetActive(true);RefreshStyles();Refresh(); }
        private void RefreshStyles()
        {
            _styleInfo.text=Game.Profile.Coins+" COINS";
            var names=new[]{T("SIGNAL-GOLD","SIGNAL GOLD"),T("EISBLAU","ICE BLUE"),T("NEON-PINK","NEON PINK"),T("JADE","JADE")};
            for(var i=0;i<4;i++)
            {
                var owned=(Game.Profile.OwnedStyles&(1<<i))!=0;
                SetLabel(_styleButtons[i],names[i]+" · "+(Game.Profile.Style==i?T("AKTIV","ACTIVE"):owned?T("AUSWÄHLEN","SELECT"):StackCampaign.StyleCost(i)+" COINS"));
                _styleButtons[i].GetComponent<Image>().color=Game.Profile.Style==i?new Color(.24f,.18f,.09f):new Color(.045f,.075f,.13f,.98f);
                _styleButtons[i].interactable=owned || Game.Profile.Coins>=StackCampaign.StyleCost(i);
            }
        }
        private void ShowTerminalActions(bool won) { _map.gameObject.SetActive(true);_next.gameObject.SetActive(won && !Game.Profile.RunChallenge && Game.Level.Id<30); }
        private void HideTerminalActions() { if(_map!=null)_map.gameObject.SetActive(false);if(_next!=null)_next.gameObject.SetActive(false); }
        private void LayoutCampaign(Rect pane)
        {
            if(_home==null)return;
            Place(_home,pane,new Rect(.04f,.015f,.92f,.97f));Place(_styles,pane,new Rect(.04f,.015f,.92f,.97f));
            Canvas.ForceUpdateCanvases();
            var landscape=_home.rect.width > _home.rect.height*1.2f;
            var gallery=(RectTransform)_levelScroll.transform;
            if(landscape)
            {
                Set((RectTransform)_home.Find("MapHeader"),.015f,.03f,.28f,.97f);
                Set((RectTransform)_home.Find("MapFooter"),.74f,.03f,.985f,.97f);
                Set((RectTransform)_home.Find("Title"),.035f,.84f,.265f,.95f);
                Set((RectTransform)_home.Find("Progress"),.035f,.75f,.265f,.84f);
                Set((RectTransform)_home.Find("Tagline"),.035f,.64f,.265f,.73f);
                Set((RectTransform)_home.Find("HeroTower"),.04f,.21f,.265f,.63f);
                var chapterHeight=UiMetrics.TargetSize(_home,49f)/Mathf.Max(1,_home.rect.height);
                for(var i=0;i<3;i++)Set((RectTransform)_chapters[i].transform,.30f+i*.143f,.96f-chapterHeight,.435f+i*.143f,.96f);
                Set(gallery,.30f,.07f,.715f,.80f);
                var actions=new[]{_continue,_endless,_challenge,_home.Find("Styles").GetComponent<Button>(),_home.Find("City").GetComponent<Button>()};
                for(var i=0;i<actions.Length;i++)Set((RectTransform)actions[i].transform,.755f,.77f-i*.147f,.97f,.91f-i*.147f);
                Set((RectTransform)_home.Find("HomeSettings"),.035f,.04f,.145f,.19f);
                Set((RectTransform)_home.Find("Learn"),.155f,.04f,.265f,.19f);
            }
            else
            {
                Set((RectTransform)_home.Find("MapHeader"),.025f,.845f,.975f,.99f);Set((RectTransform)_home.Find("MapFooter"),.025f,.012f,.975f,.335f);
                Set((RectTransform)_home.Find("Title"),.06f,.94f,.68f,.985f);Set((RectTransform)_home.Find("Progress"),.06f,.903f,.68f,.938f);
                Set((RectTransform)_home.Find("Tagline"),.06f,.853f,.65f,.893f);Set((RectTransform)_home.Find("HeroTower"),.65f,.85f,.94f,.99f);
                var chapterHeight=UiMetrics.TargetSize(_home,49f)/Mathf.Max(1,_home.rect.height);
                for(var i=0;i<3;i++)Set((RectTransform)_chapters[i].transform,.04f+i*.313f,.83f-chapterHeight,.34f+i*.313f,.83f);
                Set(gallery,.04f,.355f,.96f,.705f);
                var row=UiMetrics.TargetSize(_home,49f)/Mathf.Max(1,_home.rect.height);var gap=8f/Mathf.Max(1,_home.rect.height);
                var bottom=.018f;
                Set((RectTransform)_continue.transform,.05f,bottom+3*(row+gap),.95f,bottom+3*(row+gap)+row);
                Set((RectTransform)_endless.transform,.05f,bottom+2*(row+gap),.485f,bottom+2*(row+gap)+row);Set((RectTransform)_challenge.transform,.515f,bottom+2*(row+gap),.95f,bottom+2*(row+gap)+row);
                Set((RectTransform)_home.Find("Styles"),.05f,bottom+row+gap,.485f,bottom+2*row+gap);Set((RectTransform)_home.Find("City"),.515f,bottom+row+gap,.95f,bottom+2*row+gap);
                Set((RectTransform)_home.Find("HomeSettings"),.05f,bottom,.485f,bottom+row);Set((RectTransform)_home.Find("Learn"),.515f,bottom,.95f,bottom+row);
                var footer=bottom+4*row+3*gap+.01f;Set((RectTransform)_home.Find("MapFooter"),.025f,.01f,.975f,footer);
                Set(gallery,.04f,footer+.016f,.96f,.82f-chapterHeight);
            }
            Canvas.ForceUpdateCanvases();
            var galleryGap=12f;var galleryRow=Mathf.Max(UiMetrics.TargetSize(_home,49f),(_levelViewport.rect.height-4*galleryGap)/5f);
            var contentHeight=Mathf.Max(_levelViewport.rect.height,5*(galleryRow+galleryGap));
            _levelContent.sizeDelta=new Vector2(0,contentHeight);_levelContent.offsetMin=new Vector2(0,_levelContent.offsetMin.y);_levelContent.offsetMax=new Vector2(0,_levelContent.offsetMax.y);
            for(var i=0;i<10;i++)
            {
                var rect=(RectTransform)_levels[i].transform;rect.anchorMin=new Vector2((i%2)*.51f,1);rect.anchorMax=new Vector2((i%2)*.51f+.49f,1);rect.pivot=new Vector2(.5f,1);
                rect.sizeDelta=new Vector2(0,galleryRow);rect.anchoredPosition=new Vector2(0,-i/2*(galleryRow+galleryGap));
            }
            Set((RectTransform)_styles.Find("Title"),.06f,.895f,.94f,.985f);Set((RectTransform)_styles.Find("Wallet"),.06f,.83f,.94f,.89f);
            for(var i=0;i<4;i++)Set((RectTransform)_styleButtons[i].transform,.05f+(i%2)*.47f,.54f-(i/2)*.285f,.48f+(i%2)*.47f,.815f-(i/2)*.285f);
            Set((RectTransform)_styles.Find("ExtraDesigns"),.05f,.135f,.95f,.235f);Set((RectTransform)_styles.Find("Back"),.05f,.02f,.95f,.12f);
            if(landscape)
            {
                for(var i=0;i<4;i++)Set((RectTransform)_styleButtons[i].transform,.035f+i*.24f,.23f,.26f+i*.24f,.81f);
                Set((RectTransform)_styles.Find("ExtraDesigns"),.035f,.045f,.49f,.20f);Set((RectTransform)_styles.Find("Back"),.51f,.045f,.965f,.20f);
            }
            foreach(var button in _home.GetComponentsInChildren<Button>(true))button.GetComponentInChildren<Text>().fontSize=button==_continue?34:27;
        }
        private static string GoalText(StackLevel level,bool endless)
        {
            if(endless)return T("Stapele weiter. Jeder Block zählt!","Keep stacking. Every block counts!");
            return level.Goal==1?T("Bonus: ","Bonus: ")+Mathf.CeilToInt(level.Target*.65f)+T(" Perfect in Folge."," Perfect drops in a row."):
                level.Goal==2?T("Bonus: 80 % Fläche behalten.","Bonus: retain 80% of the area."):
                T("Bonus: 80 % perfekte Platzierungen.","Bonus: 80% perfect placements.");
        }
    }
}
