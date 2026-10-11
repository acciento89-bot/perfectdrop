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
        private int _chapter;private bool _campaignOpen;private Rect _campaignPane=new(0,0,1,1);private Button _campaignNavigation,_campaignBack;private RectTransform _heroGallery;private RawImage _heroArt;private Text _heroGoal;private RawImage[] _chapterArt;
        private bool CampaignMenuOpen => ShopOpen || (_home!=null && _home.gameObject.activeSelf) || (_styles!=null && _styles.gameObject.activeSelf);
        private void BuildCampaignMenus()
        {
            _home=UiFactory.Panel(_safe,"LevelMap",GalleryTheme.Night,Vector2.zero,Vector2.one);
            _heroGallery=UiFactory.Panel(_home,"SpecimenGallery",Navy,Vector2.zero,Vector2.one);_heroGallery.gameObject.AddComponent<RectMask2D>();
            _heroArt=GalleryTheme.Art(_heroGallery,"ArchitecturalScene",1,Vector2.zero,Vector2.one,true);
            UiFactory.Panel(_home,"MapHeader",GalleryTheme.Night,Vector2.zero,Vector2.one);
            TowerPreviewGraphic.AddStyle(_home,"HeroTower",0,Vector2.zero,Vector2.one,6);
            _campaignNavigation=UiFactory.Button(_home,"CampaignNavigation",T("KAPITEL & LEVELS","CHAPTERS & LEVELS"),GalleryTheme.Card,GalleryTheme.Text,Vector2.zero,Vector2.one,ShowCampaign);
            _campaignBack=UiFactory.Button(_home,"CampaignBack",T("‹ MENÜ","‹ HOME"),GalleryTheme.Card,GalleryTheme.Text,Vector2.zero,Vector2.one,ShowHome);
            UiFactory.Button(_home,"DirectShop",T("SHOP","SHOP"),GalleryTheme.Card,GalleryTheme.Text,Vector2.zero,Vector2.one,ShowShop);
            UiFactory.Label(_home,"Tagline",T("DEIN TIMING. DEINE SKYLINE.","YOUR TIMING. YOUR SKYLINE."),23,Vector2.zero,Vector2.one,TextAnchor.MiddleLeft,GalleryTheme.Text,FontStyle.Bold);
            UiFactory.Panel(_home,"MapFooter",GalleryTheme.Night,Vector2.zero,Vector2.one);
            UiFactory.Label(_home,"Title","PERFECT DROP",49,Vector2.zero,Vector2.one,TextAnchor.MiddleLeft,GalleryTheme.Text,FontStyle.Bold);
            _homeInfo=UiFactory.Label(_home,"Progress","",27,Vector2.zero,Vector2.one,TextAnchor.MiddleLeft,GalleryTheme.Text);
            _heroGoal=UiFactory.Label(_home,"HeroGoal","",25,Vector2.zero,Vector2.one,TextAnchor.MiddleCenter,Color.white,FontStyle.Bold);var goalBack=UiFactory.Panel(_home,"HeroGoalPlate",Navy,Vector2.zero,Vector2.one);goalBack.SetSiblingIndex(_heroGoal.transform.GetSiblingIndex());
            _chapters=new Button[3];_chapterArt=new RawImage[3];
            var names=new[]{T("Wolkenwerk","Cloud Works"),T("Abendturm","Sunset Spire"),T("Neonstadt","Neon City")};
            for(var i=0;i<3;i++)
            {
                var chapter=i;
                _chapters[i]=UiFactory.Button(_home,"Chapter"+i,names[i],GalleryTheme.Card,GalleryTheme.Text,Vector2.zero,Vector2.one,()=>{if(MenuNavigationLocked)return;_chapter=chapter;_levelScroll.verticalNormalizedPosition=1;RefreshHome();});
                _chapterArt[i]=GalleryTheme.Art(_chapters[i].transform,"ChapterArt",i,new Vector2(.03f,.35f),new Vector2(.97f,.98f));Set((RectTransform)_chapters[i].transform.Find("Label"),.03f,.03f,.97f,.34f);GalleryTheme.Style(_chapters[i],GalleryTheme.Card);
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
                _levels[i]=UiFactory.Button(_levelContent,"LevelSlot"+i,"",GalleryTheme.Card,GalleryTheme.Text,Vector2.zero,Vector2.one,()=>{if(!MenuNavigationLocked)Game.StartLevel(_chapter*10+slot+1);});
                var card=(RectTransform)_levels[i].transform;
                var label=(RectTransform)card.Find("Label"); Set(label,.24f,.12f,.95f,.90f);
                _levelIcons[i]=UiFactory.Icon(card,"LevelIcon",HudIconType.Floors,Gold,new Vector2(.035f,.22f),new Vector2(.23f,.83f));
                _levelTowers[i]=TowerPreviewGraphic.Add(card,"LevelTower",Gold,new Vector2(.01f,.19f),new Vector2(.255f,.94f),3+i%3);
                for(var star=0;star<3;star++)
                    _levelStars[i,star]=UiFactory.Icon(card,"Star"+star,HudIconType.Star,Gold,new Vector2(.035f+star*.062f,.07f),new Vector2(.093f+star*.062f,.29f));
            }
            _continue=UiFactory.Button(_home,"Continue",T("LAUF FORTSETZEN","CONTINUE RUN"),Gold,Navy,new Vector2(.07f,.22f),new Vector2(.93f,.29f),StartOrContinue);
            _endless=UiFactory.Button(_home,"Endless","",GalleryTheme.Card,GalleryTheme.Text,new Vector2(.07f,.13f),new Vector2(.93f,.20f),()=>{if(!MenuNavigationLocked)Game.StartEndless();});
            UiFactory.Button(_home,"Styles",T("TURM-DESIGNS","TOWER DESIGNS"),Navy,Color.white,new Vector2(.07f,.04f),new Vector2(.93f,.11f),ShowStyles);
            _styles=UiFactory.Panel(_safe,"StyleShop",GalleryTheme.Night,Vector2.zero,Vector2.one);
            GalleryTheme.Art(_styles,"DesignSkyline",2,Vector2.zero,Vector2.one,true);var designShade=UiFactory.Panel(_styles,"DesignShade",new Color(.035f,.065f,.12f,.75f),Vector2.zero,Vector2.one);designShade.GetComponent<Image>().material=null;
            UiFactory.Label(_styles,"Title",T("TURM-DESIGNS","TOWER DESIGNS"),50,new Vector2(.06f,.9f),new Vector2(.94f,.98f),TextAnchor.MiddleCenter,GalleryTheme.Text,FontStyle.Bold);
            _styleInfo=UiFactory.Label(_styles,"Wallet","",34,new Vector2(.06f,.82f),new Vector2(.94f,.89f),TextAnchor.MiddleCenter,GalleryTheme.Text);
            _styleButtons=new Button[4];
            for(var i=0;i<4;i++)
            {
                var style=i;
                _styleButtons[i]=UiFactory.Button(_styles,"Style"+i,"",new Color(.045f,.075f,.13f,.96f),Color.white,new Vector2(.08f,.66f-i*.155f),new Vector2(.92f,.79f-i*.155f),()=>{if(MenuNavigationLocked)return;if(Game.SelectStyle(style))Game.UiClick();RefreshStyles();});
                var styleRect=(RectTransform)_styleButtons[i].transform;
                Set((RectTransform)styleRect.Find("Label"),.25f,.05f,.96f,.95f);
                TowerPreviewGraphic.AddStyle(styleRect,"StyleTower",i,new Vector2(.06f,.24f),new Vector2(.94f,.98f));
                Set((RectTransform)styleRect.Find("Label"),.05f,.025f,.95f,.25f);
                styleRect.Find("Label").GetComponent<Text>().fontSize=27;
            }
            UiFactory.Button(_styles,"Back",T("ZURÜCK ZU LEVELS","BACK TO LEVELS"),GalleryTheme.Card,GalleryTheme.Text,new Vector2(.08f,.04f),new Vector2(.92f,.15f),ShowHome);
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
        public bool HomeOpen=>_home!=null&&_home.gameObject.activeSelf;
        public void ShowHome()
        {
            if(MenuNavigationLocked)return;_campaignOpen=false;_settingsReturnsHome=false;HideMenus();_home.gameObject.SetActive(true);RefreshHome();Refresh();LayoutCampaign(_campaignPane);Game.Videos?.Prepare();
        }
        private bool MenuNavigationLocked=>(Game.Purchases?.IsPresenting??false)||(Game.Videos?.IsPresenting??false)||(Game.Interstitials?.IsPresenting??false);
        private void ShowCampaign(){if(MenuNavigationLocked)return;_campaignOpen=true;_levelScroll.verticalNormalizedPosition=1;RefreshHome();LayoutCampaign(_campaignPane);}
        private void RefreshHome()
        {
            Kamilunavo.PerfectDrop.Visuals.WorldArt.SetChapter(_chapter);
            var stars=0;foreach(var score in Game.Profile.LevelStars)stars+=score;
            _home.Find("HeroTower").GetComponent<TowerPreviewGraphic>().ApplyStyle(Game.Profile.Style);_heroGoal.text="Level "+Game.Profile.UnlockedLevel+" · "+StackCampaign.Level(Game.Profile.UnlockedLevel).Target+T(" Blöcke"," blocks");
            _heroArt.GetComponent<TerracePreviewLease>().Apply(_chapter,true);GalleryTheme.Crop(_heroArt);
            _homeInfo.text=stars+" / 90 "+T("STERNE","STARS")+" · "+Game.Profile.Coins+" COINS";
            for(var i=0;i<3;i++)
            { _chapters[i].interactable=Game.Profile.UnlockedLevel>i*10;GalleryTheme.Style(_chapters[i],i==_chapter?Navy:GalleryTheme.Card); }
            for(var i=0;i<10;i++)
            {
                var id=_chapter*10+i+1; var unlocked=id<=Game.Profile.UnlockedLevel;
                var starsHere=Game.Profile.LevelStars[id-1];
                var current=unlocked && id==Game.Profile.UnlockedLevel;
                SetLabel(_levels[i],"LEVEL "+id+(current?"  ›":"")+"\n"+(unlocked?StackCampaign.Level(id).Target+" "+T("BLÖCKE","BLOCKS"):T("GESPERRT","LOCKED")));
                GalleryTheme.Style(_levels[i],current?Navy:GalleryTheme.Card);
                _levelIcons[i].IconType=unlocked?HudIconType.Floors:HudIconType.Lock;
                _levelIcons[i].color=unlocked?Gold:new Color(.46f,.54f,.66f); _levelIcons[i].SetVerticesDirty();
                _levelIcons[i].gameObject.SetActive(!unlocked);_levelTowers[i].gameObject.SetActive(unlocked);
                for(var star=0;star<3;star++)_levelStars[i,star].color=star<starsHere?(current?Gold:new Color(.49f,.31f,.05f)):new Color(.40f,.43f,.47f);
                _levels[i].interactable=unlocked;
            }
            var endless=StackCampaign.EndlessUnlocked(Game.Profile);
            SetLabel(_endless,endless?T("ENDLOS · BESTE ","ENDLESS · BEST ")+Game.Profile.EndlessBest:T("ENDLOS: LEVEL 5 ABSCHLIESSEN","ENDLESS: COMPLETE LEVEL 5"));
            _endless.interactable=endless;RefreshArcadeHome();
            _continue.gameObject.SetActive(true);
            SetLabel(_continue,Game.Profile.ResumeActive && !Game.Run.Failed && !Game.Run.Completed?T("FORTSETZEN","CONTINUE"):T("SPIELEN · LEVEL ","PLAY · LEVEL ")+Game.Profile.UnlockedLevel);
        }
        private void StartOrContinue() { if(MenuNavigationLocked)return;if(Game.Profile.ResumeActive && !Game.Run.Failed && !Game.Run.Completed)HideMenus();else Game.StartLevel(Game.Profile.UnlockedLevel); }
        private void ShowStyles() { if(MenuNavigationLocked)return;HideMenus();_styles.gameObject.SetActive(true);RefreshStyles();Refresh(); }
        private void RefreshStyles()
        {
            _styleInfo.text=Game.Profile.Coins+" COINS";
            var names=new[]{T("SIGNAL-GOLD","SIGNAL GOLD"),T("EISBLAU","ICE BLUE"),T("NEON-PINK","NEON PINK"),T("JADE","JADE")};
            for(var i=0;i<4;i++)
            {
                var owned=(Game.Profile.OwnedStyles&(1<<i))!=0;
                SetLabel(_styleButtons[i],names[i]+" · "+(Game.Profile.Style==i?T("AKTIV","ACTIVE"):owned?T("AUSWÄHLEN","SELECT"):StackCampaign.StyleCost(i)+" COINS"));
                GalleryTheme.Style(_styleButtons[i],Game.Profile.Style==i?Navy:GalleryTheme.Card);
                _styleButtons[i].interactable=owned || Game.Profile.Coins>=StackCampaign.StyleCost(i);
            }
        }
        private void ShowTerminalActions(bool won) { _map.gameObject.SetActive(true);_next.gameObject.SetActive(won && !Game.Profile.RunChallenge && Game.Level.Id<30); }
        private void HideTerminalActions() { if(_map!=null)_map.gameObject.SetActive(false);if(_next!=null)_next.gameObject.SetActive(false); }
        private void LayoutCampaign(Rect pane)
        {
            if(_home==null)return;_campaignPane=pane;
            Place(_home,pane,new Rect(.02f,.015f,.96f,.97f));Place(_styles,pane,new Rect(.02f,.015f,.96f,.97f));Canvas.ForceUpdateCanvases();
            bool landscape=_home.rect.width>_home.rect.height*1.2f;float unit=UiMetrics.PointScale/Mathf.Max(.001f,_home.GetComponentInParent<Canvas>().scaleFactor),w=_home.rect.width/unit,h=_home.rect.height/unit,row=UiMetrics.TargetSize(_home,49)/unit,play=UiMetrics.TargetSize(_home,54)/unit,margin=w<220?6:10,gap=w<220?4:8;
            void Position(RectTransform rect,float x,float y,float width,float height)=>Top(rect,x*unit,y*unit,width*unit,height*unit);
            var gallery=(RectTransform)_levelScroll.transform;var title=(RectTransform)_home.Find("Title");var progress=(RectTransform)_home.Find("Progress");var header=(RectTransform)_home.Find("MapHeader");var footer=(RectTransform)_home.Find("MapFooter");
            _home.GetComponent<Image>().color=GalleryTheme.Night;_home.GetComponent<Image>().material=null;header.GetComponent<Image>().material=null;footer.GetComponent<Image>().material=null;footer.SetAsFirstSibling();
            float playY=_campaignOpen?h-8:h-play-8;_continue.gameObject.SetActive(!_campaignOpen);Position((RectTransform)_continue.transform,margin,playY,w-margin*2,play);GalleryTheme.Style(_continue,Gold);
            _home.Find("Tagline").gameObject.SetActive(false);_heroGallery.gameObject.SetActive(!_campaignOpen);_home.Find("HeroTower").gameObject.SetActive(!_campaignOpen);_home.Find("HeroGoalPlate").gameObject.SetActive(!_campaignOpen);_heroGoal.gameObject.SetActive(!_campaignOpen);_campaignNavigation.gameObject.SetActive(!_campaignOpen);_campaignBack.gameObject.SetActive(_campaignOpen);gallery.gameObject.SetActive(_campaignOpen);
            foreach(var chapter in _chapters)chapter.gameObject.SetActive(_campaignOpen);_endless.gameObject.SetActive(_campaignOpen);_challenge.gameObject.SetActive(_campaignOpen);_home.Find("Styles").gameObject.SetActive(!_campaignOpen);_home.Find("City").gameObject.SetActive(!_campaignOpen);_home.Find("HomeSettings").gameObject.SetActive(!_campaignOpen);_home.Find("Learn").gameObject.SetActive(!_campaignOpen);_home.Find("DirectShop").gameObject.SetActive(!_campaignOpen);
            Position(header,0,0,w,70);Position(title,margin,8,w-margin*2,32);Position(progress,margin,43,w-margin*2,22);title.GetComponent<Text>().text=_campaignOpen?T("KAPITEL & LEVELS","CHAPTERS & LEVELS"):"PERFECT DROP";
            if(!_campaignOpen)
            {
                progress.gameObject.SetActive(true);
                float navX=landscape?w*.45f:margin,navW=w-navX-margin,navY=playY-row*2-gap-12,heroX=margin,heroY=74,heroW=landscape?navX-margin-gap:w-margin*2,heroH=landscape?playY-heroY-8:navY-heroY-8;
                Position(_heroGallery,heroX,heroY,heroW,Mathf.Max(1,heroH));Position((RectTransform)_home.Find("HeroTower"),heroX+heroW*.17f,heroY+5,heroW*.66f,Mathf.Max(1,heroH-48));
                float goalX=landscape?navX:heroX+8,goalY=landscape?84:heroY+heroH-37,goalW=landscape?navW:heroW-16;Position((RectTransform)_home.Find("HeroGoalPlate"),goalX,goalY,goalW,34);Position((RectTransform)_heroGoal.transform,goalX,goalY,goalW,34);
                if(h<240&&landscape){Position(title,margin,6,heroW,30);Position(progress,margin,38,heroW,24);_heroGoal.gameObject.SetActive(false);_home.Find("HeroGoalPlate").gameObject.SetActive(false);}
                Position(footer,navX,navY-5,navW,h-navY);float cell=(navW-gap*2)/3;var main=new[]{_campaignNavigation,_home.Find("Styles").GetComponent<Button>(),_home.Find("City").GetComponent<Button>()};
                for(int i=0;i<3;i++){Position((RectTransform)main[i].transform,navX+i*(cell+gap),navY,cell,row);GalleryTheme.Style(main[i],GalleryTheme.Card);}
                float secondary=(navW-gap*2)/3;var options=_home.Find("HomeSettings").GetComponent<Button>();var learn=_home.Find("Learn").GetComponent<Button>();var shop=_home.Find("DirectShop").GetComponent<Button>();Position((RectTransform)shop.transform,navX,navY+row+gap,secondary,row);Position((RectTransform)options.transform,navX+secondary+gap,navY+row+gap,secondary,row);Position((RectTransform)learn.transform,navX+2*(secondary+gap),navY+row+gap,secondary,row);GalleryTheme.Style(shop,GalleryTheme.Card);GalleryTheme.Style(options,GalleryTheme.Card);GalleryTheme.Style(learn,GalleryTheme.Card);SetLabel(learn,T("TUTORIAL","TUTORIAL"));
                SetLabel(_campaignNavigation,T("LEVELS","LEVELS"));SetLabel(main[1],T("DESIGNS","DESIGNS"));SetLabel(main[2],T("MEINE STADT","MY CITY"));
            }
            else
            {
                float chapterY=h<240?22:74,chapterH=Mathf.Max(row,landscape?49:72),levelsY=chapterY+chapterH+(h<240?4:gap),bottomY=playY-row,bodyW=w-margin*2;Position(footer,margin,bottomY-4,bodyW,h-bottomY);
                if(h<240){Position(title,margin,0,w-margin*2,20);progress.gameObject.SetActive(false);}else progress.gameObject.SetActive(true);float chapterW=(bodyW-gap*2)/3;for(int i=0;i<3;i++)Position((RectTransform)_chapters[i].transform,margin+i*(chapterW+gap),chapterY,chapterW,chapterH);
                Position(gallery,margin,levelsY,bodyW,Mathf.Max(row,bottomY-levelsY-(h<240?4:8)));float cell=(bodyW-gap*2)/3;var actions=new[]{_campaignBack,_endless,_challenge};for(int i=0;i<3;i++){Position((RectTransform)actions[i].transform,margin+i*(cell+gap),bottomY,cell,row);GalleryTheme.Style(actions[i],GalleryTheme.Card);}SetLabel(_endless,StackCampaign.EndlessUnlocked(Game.Profile)?T("ENDLOS","ENDLESS"):T("ENDLOS · LV 5","ENDLESS · LV 5"));SetLabel(_challenge,T("TAGESROUTE","DAILY ROUTE"));
            }
            Canvas.ForceUpdateCanvases();float galleryGap=8*unit,galleryRow=UiMetrics.TargetSize(_home,54);_levelContent.sizeDelta=new Vector2(0,5*galleryRow+4*galleryGap);_levelContent.offsetMin=new Vector2(0,_levelContent.offsetMin.y);_levelContent.offsetMax=new Vector2(0,_levelContent.offsetMax.y);
            for(int i=0;i<10;i++){var rect=(RectTransform)_levels[i].transform;rect.anchorMin=new Vector2(i%2*.51f,1);rect.anchorMax=new Vector2(i%2*.51f+.49f,1);rect.pivot=new Vector2(.5f,1);rect.sizeDelta=new Vector2(0,galleryRow);rect.anchoredPosition=new Vector2(0,-i/2*(galleryRow+galleryGap));}
            Set((RectTransform)_styles.Find("Title"),.06f,.905f,.94f,.985f);Set((RectTransform)_styles.Find("Wallet"),.06f,.85f,.94f,.90f);_styles.GetComponent<Image>().material=null;
            for(int i=0;i<4;i++)Set((RectTransform)_styleButtons[i].transform,.04f+i%2*.48f,.55f-i/2*.285f,.48f+i%2*.48f,.825f-i/2*.285f);
            float styleButtonH=UiMetrics.TargetSize(_styles,49)/Mathf.Max(1,_styles.rect.height);Set((RectTransform)_styles.Find("ExtraDesigns"),.04f,.025f+styleButtonH+.012f,.96f,.025f+styleButtonH*2+.012f);Set((RectTransform)_styles.Find("Back"),.04f,.025f,.96f,.025f+styleButtonH);
            if(landscape){for(int i=0;i<4;i++)Set((RectTransform)_styleButtons[i].transform,.025f+i*.245f,.25f,.255f+i*.245f,.82f);Set((RectTransform)_styles.Find("ExtraDesigns"),.025f,.03f,.49f,.03f+styleButtonH);Set((RectTransform)_styles.Find("Back"),.51f,.03f,.975f,.03f+styleButtonH);}
            foreach(var button in _styleButtons)GalleryTheme.Style(button,button==_styleButtons[Game.Profile.Style<4?Game.Profile.Style:0]&&Game.Profile.Style<4?Navy:GalleryTheme.Card);GalleryTheme.Style(_styles.Find("ExtraDesigns").GetComponent<Button>(),GalleryTheme.Card);GalleryTheme.Style(_styles.Find("Back").GetComponent<Button>(),GalleryTheme.Card);
            foreach(var button in _home.GetComponentsInChildren<Button>(true))GalleryTheme.Readable(button.GetComponentInChildren<Text>(),button==_continue?18:12);foreach(var button in _styleButtons)GalleryTheme.Readable(button.GetComponentInChildren<Text>(),13);GalleryTheme.Readable(title.GetComponent<Text>(),24);GalleryTheme.Readable(_homeInfo,13);GalleryTheme.Readable(_heroGoal,15);GalleryTheme.Readable(_styleInfo,13);GalleryTheme.Readable(_styles.Find("Title").GetComponent<Text>(),22);foreach(var image in _chapterArt)GalleryTheme.Crop(image);GalleryTheme.Crop(_heroArt);GalleryTheme.Crop(_styles.Find("DesignSkyline").GetComponent<RawImage>());
        }
        private static void Top(RectTransform rect,float x,float y,float w,float h){rect.anchorMin=rect.anchorMax=new Vector2(0,1);rect.pivot=new Vector2(0,1);rect.anchoredPosition=new Vector2(x,-y);rect.sizeDelta=new Vector2(w,h);}
        private static string GoalText(StackLevel level,bool endless)
        {
            if(endless)return T("Stapele weiter. Jeder Block zählt!","Keep stacking. Every block counts!");
            return level.Goal==1?T("Bonus: ","Bonus: ")+Mathf.CeilToInt(level.Target*.65f)+T(" Perfect in Folge."," Perfect drops in a row."):
                level.Goal==2?T("Bonus: 80 % Fläche behalten.","Bonus: retain 80% of the area."):
                T("Bonus: 80 % perfekte Platzierungen.","Bonus: 80% perfect placements.");
        }
    }
}
