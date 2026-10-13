using UnityEngine;
using UnityEngine.UI;
using Kamilunavo.PerfectDrop.Gameplay;
namespace Kamilunavo.PerfectDrop.UI
{
    public sealed partial class StackHud
    {
        private RectTransform _home,_styles,_levelViewport,_levelContent;
        private ScrollRect _levelScroll,_styleScroll;private RectTransform _styleViewport,_styleContent;private RawImage _titleArt;
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
            UiFactory.Panel(_home,"MapHeader",Color.clear,Vector2.zero,Vector2.one);
            var logo=new GameObject("AuthoredTitle",typeof(RectTransform),typeof(CanvasRenderer),typeof(RawImage));logo.transform.SetParent(_home,false);_titleArt=logo.GetComponent<RawImage>();_titleArt.texture=Resources.Load<Texture2D>("Art/AuthoredPerfectDropTitle");_titleArt.raycastTarget=false;
            TowerPreviewGraphic.AddStyle(_home,"HeroTower",0,Vector2.zero,Vector2.one,6);
            _campaignNavigation=UiFactory.Button(_home,"CampaignNavigation",T("KAPITEL & LEVELS","CHAPTERS & LEVELS"),GalleryTheme.Card,GalleryTheme.Text,Vector2.zero,Vector2.one,ShowCampaign);
            _campaignBack=UiFactory.Button(_home,"CampaignBack",T("‹ MENÜ","‹ HOME"),GalleryTheme.Card,GalleryTheme.Text,Vector2.zero,Vector2.one,ShowHome);
            UiFactory.Button(_home,"DirectShop",T("SHOP","SHOP"),GalleryTheme.Card,GalleryTheme.Text,Vector2.zero,Vector2.one,ShowShop);
            UiFactory.Label(_home,"Tagline",T("DEIN TIMING. DEINE SKYLINE.","YOUR TIMING. YOUR SKYLINE."),23,Vector2.zero,Vector2.one,TextAnchor.MiddleLeft,GalleryTheme.Text,FontStyle.Bold);
            UiFactory.Panel(_home,"MapFooter",GalleryTheme.Ivory,Vector2.zero,Vector2.one);
            UiFactory.Label(_home,"Title","PERFECT DROP",49,Vector2.zero,Vector2.one,TextAnchor.MiddleLeft,GalleryTheme.Text,FontStyle.Bold);
            _homeInfo=UiFactory.Label(_home,"Progress","",27,Vector2.zero,Vector2.one,TextAnchor.MiddleLeft,GalleryTheme.Text);
            _heroGoal=UiFactory.Label(_home,"HeroGoal","",25,Vector2.zero,Vector2.one,TextAnchor.MiddleCenter,Color.white,FontStyle.Bold);var goalBack=UiFactory.Panel(_home,"HeroGoalPlate",Navy,Vector2.zero,Vector2.one);goalBack.SetSiblingIndex(_heroGoal.transform.GetSiblingIndex());
            _chapters=new Button[3];_chapterArt=new RawImage[3];
            var names=new[]{T("Wolkenwerk","Cloud Works"),T("Abendturm","Sunset Spire"),T("Neonstadt","Neon City")};
            for(var i=0;i<3;i++)
            {
                var chapter=i;
                _chapters[i]=UiFactory.Button(_home,"Chapter"+i,names[i],GalleryTheme.Card,GalleryTheme.Text,Vector2.zero,Vector2.one,()=>{if(MenuNavigationLocked)return;_chapter=chapter;_levelScroll.verticalNormalizedPosition=1;RefreshHome();});
                _chapterArt[i]=GalleryTheme.Art(_chapters[i].transform,"ChapterArt",i,new Vector2(.03f,.35f),new Vector2(.97f,.98f),false,true);Set((RectTransform)_chapters[i].transform.Find("Label"),.03f,.03f,.97f,.34f);GalleryTheme.Style(_chapters[i],GalleryTheme.Card);
                var chapterName=_chapters[i].transform.Find("Label").GetComponent<Text>();chapterName.font=UiFactory.SerifFont;chapterName.fontStyle=FontStyle.Normal;
                UiFactory.Label(_chapters[i].transform,"ChapterProgress","",20,new Vector2(.03f,.015f),new Vector2(.97f,.16f),TextAnchor.MiddleCenter,GalleryTheme.Text);
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
            _continue.GetComponentInChildren<Text>().font=UiFactory.SerifFont;_continue.GetComponentInChildren<Text>().fontStyle=FontStyle.Normal;
            _endless=UiFactory.Button(_home,"Endless","",GalleryTheme.Card,GalleryTheme.Text,new Vector2(.07f,.13f),new Vector2(.93f,.20f),()=>{if(!MenuNavigationLocked)Game.StartEndless();});
            UiFactory.Button(_home,"Styles",T("TURM-DESIGNS","TOWER DESIGNS"),Navy,Color.white,new Vector2(.07f,.04f),new Vector2(.93f,.11f),ShowStyles);
            _styles=UiFactory.Panel(_safe,"StyleShop",GalleryTheme.Night,Vector2.zero,Vector2.one);
            GalleryTheme.Art(_styles,"DesignSkyline",1,Vector2.zero,Vector2.one,true);var designShade=UiFactory.Panel(_styles,"DesignShade",new Color(.035f,.065f,.12f,.30f),Vector2.zero,Vector2.one);designShade.GetComponent<Image>().material=null;
            UiFactory.Label(_styles,"Title",T("TURM-DESIGNS","TOWER DESIGNS"),50,new Vector2(.06f,.9f),new Vector2(.94f,.98f),TextAnchor.MiddleCenter,GalleryTheme.Text,FontStyle.Bold).font=UiFactory.SerifFont;
            _styleInfo=UiFactory.Label(_styles,"Wallet","",34,new Vector2(.06f,.82f),new Vector2(.94f,.89f),TextAnchor.MiddleCenter,GalleryTheme.Text);
            var stylesObject=new GameObject("DesignGalleryScroll",typeof(RectTransform),typeof(ScrollRect));stylesObject.transform.SetParent(_styles,false);
            _styleScroll=stylesObject.GetComponent<ScrollRect>();_styleScroll.vertical=false;_styleScroll.movementType=ScrollRect.MovementType.Clamped;
            _styleViewport=UiFactory.Panel(stylesObject.transform,"DesignViewport",Color.clear,Vector2.zero,Vector2.one);_styleViewport.gameObject.AddComponent<RectMask2D>();
            _styleContent=new GameObject("DesignContent",typeof(RectTransform)).GetComponent<RectTransform>();_styleContent.SetParent(_styleViewport,false);_styleContent.anchorMin=new Vector2(0,0);_styleContent.anchorMax=new Vector2(0,1);_styleContent.pivot=new Vector2(0,.5f);_styleScroll.viewport=_styleViewport;_styleScroll.content=_styleContent;
            _styleButtons=new Button[8];
            for(var i=0;i<8;i++)
            {
                var style=i;
                _styleButtons[i]=UiFactory.Button(_styleContent,"Style"+i,"",new Color(.045f,.075f,.13f,.96f),Color.white,new Vector2(.08f,.66f-i*.155f),new Vector2(.92f,.79f-i*.155f),()=>{if(MenuNavigationLocked)return;if(style>=4&&(Game.Profile.OwnedStyles&(1<<style))==0){ShowShop();return;}if(Game.SelectStyle(style))Game.UiClick();RefreshStyles();LayoutCampaign(_campaignPane);});
                var styleRect=(RectTransform)_styleButtons[i].transform;
                Set((RectTransform)styleRect.Find("Label"),.25f,.05f,.96f,.95f);
                TowerPreviewGraphic.AddStyle(styleRect,"StyleTower",i,new Vector2(.06f,.24f),new Vector2(.94f,.98f),7).AlignTop=true;
                Set((RectTransform)styleRect.Find("Label"),.05f,.025f,.95f,.25f);
                styleRect.Find("Label").GetComponent<Text>().fontSize=27;
                var actionPlate=UiFactory.Panel(styleRect,"SelectionPlate",Gold,new Vector2(.06f,.02f),new Vector2(.94f,.09f));
                var actionImage=actionPlate.GetComponent<Image>();actionImage.raycastTarget=false;actionImage.material=UiFactory.GoldActionMaterial;UiFactory.ApplyPillImage(actionImage);GalleryFrameGraphic.Apply(actionPlate,GalleryTheme.Ivory);
                styleRect.Find("Label").SetAsLastSibling();
                var designName=UiFactory.Label(styleRect,"DesignName","",27,new Vector2(.03f,.85f),new Vector2(.97f,.97f),TextAnchor.MiddleCenter,GalleryTheme.Text);designName.font=UiFactory.SerifFont;
                var materialNames=new[]{T("Metall · Gold","Metal · Gold"),T("Eis · Cyan","Ice · Cyan"),T("Lack · Pink","Lacquer · Pink"),T("Jade · Gold","Jade · Gold"),T("Kupfer · Bronze","Copper · Bronze"),T("Keramik · Gold","Ceramic · Gold"),T("Lack · Violett","Lacquer · Violet"),T("Metall · Solar","Metal · Solar")};UiFactory.Label(styleRect,"MaterialName",materialNames[i],22,new Vector2(.025f,.78f),new Vector2(.975f,.85f),TextAnchor.MiddleCenter,GalleryTheme.Text);
            }
            UiFactory.Button(_styles,"Back",T("‹ MENÜ","‹ HOME"),GalleryTheme.Card,GalleryTheme.Text,new Vector2(.08f,.04f),new Vector2(.92f,.15f),ShowHome);
            var designWallet=UiFactory.Button(_styles,"StyleWallet","",Navy,GalleryTheme.Text,Vector2.zero,Vector2.one,ShowShop);
            UiFactory.Icon(designWallet.transform,"WalletCoin",HudIconType.Coin,GalleryTheme.Gold,new Vector2(.04f,.18f),new Vector2(.30f,.82f));Set((RectTransform)designWallet.transform.Find("Label"),.31f,.05f,.94f,.95f);
            _next=UiFactory.Button(_popup,"Next",T("NÄCHSTES LEVEL","NEXT LEVEL"),Gold,Navy,new Vector2(.10f,.43f),new Vector2(.90f,.56f),()=>Game.StartLevel(Game.Level.Id+1));
            _map=UiFactory.Button(_popup,"LevelMapAction",T("LEVELÜBERSICHT","LEVEL MAP"),new Color(.14f,.2f,.3f),Color.white,new Vector2(.10f,.32f),new Vector2(.90f,.42f),Game.GoHome);
            _home.gameObject.SetActive(false); _styles.gameObject.SetActive(false); HideTerminalActions();
        }
        public void HideMenus()
        {
            _settingsOpen=false;_settingsReturn=null;if(_settingsBackdrop!=null)_settingsBackdrop.gameObject.SetActive(false);
            HideArcadeMenus();if(_shop!=null)_shop.gameObject.SetActive(false);
            _home.gameObject.SetActive(false);_styles.gameObject.SetActive(false);_popup.gameObject.SetActive(false);HideTerminalActions();
            Set((RectTransform)_popupTitle.transform,.07f,.79f,.93f,.96f); Refresh();
        }
        public bool HomeOpen=>_home!=null&&_home.gameObject.activeSelf;
        public void ShowHome()
        {
            if(MenuNavigationLocked)return;_campaignOpen=false;_settingsReturn=null;_settingsOpen=false;HideMenus();_home.gameObject.SetActive(true);Place(_home,_campaignPane,new Rect(0,0,1,1));Canvas.ForceUpdateCanvases();RefreshHome();Refresh();LayoutCampaign(_campaignPane);Game.Videos?.Prepare();
        }
        private bool MenuNavigationLocked=>(Game.Purchases?.IsPresenting??false)||(Game.Videos?.IsPresenting??false)||(Game.Interstitials?.IsPresenting??false);
        private void ShowCampaign(){if(MenuNavigationLocked)return;_campaignOpen=true;_levelScroll.verticalNormalizedPosition=1;RefreshHome();LayoutCampaign(_campaignPane);}
        private void RefreshHome()
        {
            Kamilunavo.PerfectDrop.Visuals.WorldArt.SetChapter(_chapter);
            var stars=0;foreach(var score in Game.Profile.LevelStars)stars+=score;
            _home.Find("HeroTower").GetComponent<TowerPreviewGraphic>().ApplyStyle(Game.Profile.Style);_heroGoal.text="Level "+Game.Profile.UnlockedLevel+" · "+StackCampaign.Level(Game.Profile.UnlockedLevel).Target+T(" Blöcke"," blocks");
            _heroArt.GetComponent<TerracePreviewLease>().ApplyHero(_chapter,Game.Profile.Style,_home.rect.width<=_home.rect.height*1.2f);_home.Find("HeroTower").GetComponent<TowerPreviewGraphic>().enabled=false;GalleryTheme.Crop(_heroArt);
            _homeInfo.text=stars+" / 90 "+T("STERNE","STARS")+" · "+Game.Profile.Coins+" COINS";
            for(var i=0;i<3;i++)
            { _chapters[i].interactable=Game.Profile.UnlockedLevel>i*10;GalleryTheme.Style(_chapters[i],i==_chapter?Navy:GalleryTheme.Ivory);ConceptCardSkin.Select(_chapters[i].GetComponent<Image>(),i==_chapter);int chapterStars=0;for(int slot=0;slot<10;slot++)chapterStars+=Game.Profile.LevelStars[i*10+slot];_chapters[i].transform.Find("ChapterProgress").GetComponent<Text>().text="★ "+chapterStars+" / 30"; }
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
        private void UpdateDesignAction(int style)
        {
            var plate=_styleButtons[style].transform.Find("SelectionPlate");
            if(plate==null)return;
            bool owned=(Game.Profile.OwnedStyles&(1<<style))!=0 || style==0;
            var surface=plate.GetComponent<Image>();surface.color=owned?Gold:GalleryTheme.Card;
            if(owned){if(!ConceptGoldSkin.Apply(surface)){surface.material=UiFactory.GoldActionMaterial;UiFactory.ApplyPillImage(surface);}}else{var skin=surface.GetComponent<ConceptGoldSkin>();if(skin!=null)skin.enabled=false;surface.material=null;UiFactory.ApplyPillImage(surface);}
            GalleryFrameGraphic.Apply(plate,owned?GalleryTheme.Ivory:GalleryTheme.Gold);
            var label=_styleButtons[style].transform.Find("Label").GetComponent<Text>();label.color=owned?GalleryTheme.Ink:GalleryTheme.Text;
            var shadow=label.GetComponent<Shadow>();if(shadow!=null)shadow.enabled=!owned;
            label.font=UiFactory.SerifFont;
            GalleryFrameGraphic.Apply(_styleButtons[style].transform,Game.Profile.Style==style?GalleryTheme.Ivory:Color.Lerp(GalleryTheme.Gold,GalleryTheme.Ink,.62f));
            ConceptCardSkin.Select(_styleButtons[style].GetComponent<Image>(),Game.Profile.Style==style);
        }
        private static Color DesignSurface(int style)=>style==3?new Color(.025f,.17f,.14f,.92f):style==5?new Color(.22f,.24f,.28f,.94f):new Color(.025f,.05f,.10f,.92f);
        private void StartOrContinue() { if(MenuNavigationLocked)return;if(Game.Profile.ResumeActive && !Game.Run.Failed && !Game.Run.Completed)HideMenus();else Game.StartLevel(Game.Profile.UnlockedLevel); }
        private void ShowStyles() { if(MenuNavigationLocked)return;HideMenus();_styles.gameObject.SetActive(true);_styleScroll.horizontalNormalizedPosition=0;RefreshStyles();Refresh();LayoutCampaign(_campaignPane); }
        private void RefreshStyles()
        {
            _styleInfo.text=Game.Profile.Coins+" COINS";
            SetLabel(_styles.Find("StyleWallet").GetComponent<Button>(),Game.Profile.Coins+" +");
            var names=new[]{T("SIGNAL-GOLD","SIGNAL GOLD"),T("EISBLAU","ICE BLUE"),T("NEON-PINK","NEON PINK"),T("JADE","JADE"),T("KUPFER","COPPER"),T("PERLE","PEARL"),T("VIOLETT","VIOLET"),T("SOLAR","SOLAR")};
            for(var i=0;i<8;i++)
            {
                var owned=(Game.Profile.OwnedStyles&(1<<i))!=0;
                _styleButtons[i].transform.Find("DesignName").GetComponent<Text>().text=names[i];SetLabel(_styleButtons[i],(Game.Profile.Style==i?T("AKTIV","ACTIVE"):owned?T("AUSWÄHLEN","SELECT"):i>=4?T("IM SHOP","IN SHOP"):StackCampaign.StyleCost(i)+" COINS"));
                GalleryTheme.Style(_styleButtons[i],DesignSurface(i));UpdateDesignAction(i);
                _styleButtons[i].interactable=i>=4 || owned || Game.Profile.Coins>=StackCampaign.StyleCost(i);
            }
        }
        private void ShowTerminalActions(bool won) { _map.gameObject.SetActive(true);_next.gameObject.SetActive(won && !Game.Profile.RunChallenge && Game.Level.Id<30); }
        private void HideTerminalActions() { if(_map!=null)_map.gameObject.SetActive(false);if(_next!=null)_next.gameObject.SetActive(false); }
        private void LayoutCampaign(Rect pane)
        {
            if(_home==null)return;_campaignPane=pane;
            Place(_home,pane,new Rect(0,0,1,1));Place(_styles,pane,new Rect(.02f,.015f,.96f,.97f));Canvas.ForceUpdateCanvases();
            bool landscape=_home.rect.width>_home.rect.height*1.2f;float unit=UiMetrics.PointScale/Mathf.Max(.001f,_home.GetComponentInParent<Canvas>().scaleFactor),w=_home.rect.width/unit,h=_home.rect.height/unit,row=UiMetrics.TargetSize(_home,49)/unit,play=UiMetrics.TargetSize(_home,54)/unit,margin=w<220?6:10,gap=w<220?4:8;
            if(!landscape&&h>=550){play=Mathf.Max(play,84);row=Mathf.Max(row,80);}
            void Position(RectTransform rect,float x,float y,float width,float height)=>Top(rect,x*unit,y*unit,width*unit,height*unit);
            var gallery=(RectTransform)_levelScroll.transform;var title=(RectTransform)_home.Find("Title");var progress=(RectTransform)_home.Find("Progress");var header=(RectTransform)_home.Find("MapHeader");var footer=(RectTransform)_home.Find("MapFooter");
            _home.GetComponent<Image>().color=GalleryTheme.Night;_home.GetComponent<Image>().material=null;header.GetComponent<Image>().material=null;footer.GetComponent<Image>().material=null;footer.SetSiblingIndex(_heroGallery.GetSiblingIndex()+1);
            float playY=_campaignOpen?h-8:h-play-8;_continue.gameObject.SetActive(!_campaignOpen);Position((RectTransform)_continue.transform,margin,playY,w-margin*2,play);GalleryTheme.Style(_continue,Gold);
            _home.Find("Tagline").gameObject.SetActive(false);_heroGallery.gameObject.SetActive(!_campaignOpen);_home.Find("HeroTower").gameObject.SetActive(!_campaignOpen);_home.Find("HeroGoalPlate").gameObject.SetActive(!_campaignOpen);_heroGoal.gameObject.SetActive(!_campaignOpen);_campaignNavigation.gameObject.SetActive(!_campaignOpen);_campaignBack.gameObject.SetActive(_campaignOpen);gallery.gameObject.SetActive(_campaignOpen);
            foreach(var chapter in _chapters)chapter.gameObject.SetActive(true);_endless.gameObject.SetActive(_campaignOpen);_challenge.gameObject.SetActive(_campaignOpen);_home.Find("Styles").gameObject.SetActive(!_campaignOpen);_home.Find("City").gameObject.SetActive(!_campaignOpen);_home.Find("HomeSettings").gameObject.SetActive(!_campaignOpen);_home.Find("Learn").gameObject.SetActive(!_campaignOpen);_home.Find("DirectShop").gameObject.SetActive(!_campaignOpen);
            _titleArt.gameObject.SetActive(!_campaignOpen);Position(header,0,0,w,70);Position(title,margin,8,w-margin*2,32);title.GetComponent<Text>().color=GalleryTheme.Gold;Position(progress,margin,43,w-margin*2,22);title.GetComponent<Text>().text=_campaignOpen?T("KAPITEL & LEVELS","CHAPTERS & LEVELS"):"PERFECT DROP";
            if(!_campaignOpen)
            {
                progress.gameObject.SetActive(false);title.gameObject.SetActive(false);
                float navX=landscape?w*.46f:margin,navW=w-navX-margin,navY=playY-row-gap;
                if(landscape)Position((RectTransform)_continue.transform,navX,playY,navW,play);
                float chapterH=landscape?row:Mathf.Clamp(h*.26f,94,250),chapterY=navY-chapterH-(landscape?gap:Mathf.Max(gap,16));
                float heroX=margin,heroY=landscape?75:Mathf.Max(82,h*.145f),heroW=landscape?navX-margin-gap:w-margin*2,heroH=landscape?h-heroY-12:chapterY-heroY-8;
                Position(_heroGallery,0,0,w,h);Position((RectTransform)_home.Find("HeroTower"),heroX+heroW*.09f,heroY,heroW*.82f,Mathf.Max(95,heroH-row-2));
                float logoW=Mathf.Min(landscape?heroW:w*.58f,landscape?235:190),logoH=logoW*.61f;
                float logoTop=w<450?UiMetrics.TargetSize(_home,49)/unit+2:Mathf.Clamp(h*.02f,8,22);
                Position(_titleArt.rectTransform,landscape?heroX+(heroW-logoW)/2:(w-logoW)/2,landscape?5:logoTop,logoW,logoH);
                float goalHeight=UiMetrics.TargetSize(_home,49)/unit,goalY=landscape?heroY+heroH-goalHeight:chapterY-goalHeight-5;
                float plaqueW=landscape?heroW-16:Mathf.Min(w*.65f,310);
                Position((RectTransform)_campaignNavigation.transform,heroX+(heroW-plaqueW)*.5f,goalY,plaqueW,goalHeight);GalleryTheme.Style(_campaignNavigation,Navy);UiFactory.ApplyPillImage(_campaignNavigation.GetComponent<Image>());GalleryFrameGraphic.Apply(_campaignNavigation.transform,GalleryTheme.Ivory);SetLabel(_campaignNavigation,_heroGoal.text+"  ›");_heroGoal.gameObject.SetActive(false);_home.Find("HeroGoalPlate").gameObject.SetActive(false);
                Position(footer,navX-6,chapterY-6,navW+12,h-chapterY+6);footer.GetComponent<Image>().color=GalleryTheme.Ivory;
                float chapterW=(navW-gap*2)/3;for(int i=0;i<3;i++)Position((RectTransform)_chapters[i].transform,navX+i*(chapterW+gap),chapterY,chapterW,chapterH);
                var designs=_home.Find("Styles").GetComponent<Button>();var city=_home.Find("City").GetComponent<Button>();float cell=(navW-gap)/2;
                Position((RectTransform)designs.transform,navX,navY,cell,row);Position((RectTransform)city.transform,navX+cell+gap,navY,cell,row);GalleryTheme.Style(designs,Navy);GalleryTheme.Style(city,Navy);SetLabel(designs,T("Designs  ›","Designs  ›"));SetLabel(city,T("Meine Stadt  ›","My City  ›"));
                if(designs.transform.Find("NavigationArt")==null){var art=new GameObject("NavigationArt",typeof(RectTransform),typeof(CanvasRenderer),typeof(RawImage));art.transform.SetParent(designs.transform,false);var thumbnail=art.GetComponent<RawImage>();thumbnail.texture=Resources.Load<Texture2D>("Art/ConceptV3/DesignNavigation");thumbnail.raycastTarget=false;Set((RectTransform)art.transform,.02f,.05f,.27f,.95f);}
                if(city.transform.Find("NavigationArt")==null)GalleryTheme.Art(city.transform,"NavigationArt",0,new Vector2(.02f,.05f),new Vector2(.27f,.95f));
                foreach(var item in new[]{designs,city}){Set((RectTransform)item.transform.Find("Label"),.28f,.06f,.98f,.94f);item.transform.Find("Label").GetComponent<Text>().font=UiFactory.SerifFont;item.transform.Find("Label").GetComponent<Text>().fontStyle=FontStyle.Normal;}
                var options=_home.Find("HomeSettings").GetComponent<Button>();var learn=_home.Find("Learn").GetComponent<Button>();var shop=_home.Find("DirectShop").GetComponent<Button>();
                float headerTarget=UiMetrics.TargetSize(_home,49)/unit,walletWidth=Mathf.Max(w>=250?116:84,headerTarget);
                Position((RectTransform)options.transform,margin,8,headerTarget,headerTarget);Position((RectTransform)learn.transform,margin,8+headerTarget+gap,headerTarget,headerTarget);Position((RectTransform)shop.transform,w-margin-walletWidth,8,walletWidth,headerTarget);
                GalleryTheme.Style(options,Navy);GalleryTheme.Style(learn,Navy);GalleryTheme.Style(shop,Navy);UiFactory.ApplyCircularImage(options.GetComponent<Image>());UiFactory.ApplyCircularImage(learn.GetComponent<Image>());UiFactory.ApplyPillImage(shop.GetComponent<Image>());
                SetLabel(options,"");if(options.transform.Find("SettingsIcon")==null)UiFactory.Icon(options.transform,"SettingsIcon",HudIconType.Gear,Gold,new Vector2(.23f,.23f),new Vector2(.77f,.77f));SetLabel(learn,"?");SetLabel(shop,Game.Profile.Coins+"  +");Set((RectTransform)shop.transform.Find("Label"),.30f,.06f,.94f,.94f);shop.transform.Find("Label").GetComponent<Text>().font=UiFactory.DefaultFont;
                if(shop.transform.Find("WalletCoin")==null)UiFactory.Icon(shop.transform,"WalletCoin",HudIconType.Coin,GalleryTheme.Gold,new Vector2(.04f,.18f),new Vector2(.31f,.82f));
                // Three compact rows retain every action in a short divided landscape pane.
                if(landscape && h<240)
                {
                    float compactGap=4,compactCell=(navW-compactGap*3)/4,compactTop=(h-row*3-compactGap*2)/2;
                    Position(_heroGallery,0,0,w,h);Position((RectTransform)_home.Find("HeroTower"),heroX,heroY,heroW,Mathf.Max(95,h-heroY-margin));Position(_titleArt.rectTransform,heroX+25,2,Mathf.Max(65,heroW-40),72);
                    Position(footer,navX-4,compactTop-4,navW+8,h-compactTop+4);
                    for(int i=0;i<3;i++)Position((RectTransform)_chapters[i].transform,navX+i*(compactCell+compactGap),compactTop,compactCell,row);
                    Position((RectTransform)_campaignNavigation.transform,navX+3*(compactCell+compactGap),compactTop,compactCell,row);SetLabel(_campaignNavigation,"LEVELS");
                    var middle=new[]{designs,city,shop,options};for(int i=0;i<4;i++)Position((RectTransform)middle[i].transform,navX+i*(compactCell+compactGap),compactTop+row+compactGap,compactCell,row);
                    Position((RectTransform)learn.transform,navX,compactTop+2*(row+compactGap),compactCell,row);SetLabel(learn,"TUTORIAL");
                    Position((RectTransform)_continue.transform,navX+compactCell+compactGap,compactTop+2*(row+compactGap),compactCell*3+compactGap*2,row);
                }
            }
            else
            {
                title.gameObject.SetActive(true);footer.GetComponent<Image>().color=GalleryTheme.Night;float chapterY=h<240?22:74,chapterH=Mathf.Max(row,landscape?49:72),levelsY=chapterY+chapterH+(h<240?4:gap),bottomY=playY-row,bodyW=w-margin*2;Position(footer,margin,bottomY-4,bodyW,h-bottomY);
                if(h<240){Position(title,margin,0,w-margin*2,20);progress.gameObject.SetActive(false);}else progress.gameObject.SetActive(true);float chapterW=(bodyW-gap*2)/3;for(int i=0;i<3;i++)Position((RectTransform)_chapters[i].transform,margin+i*(chapterW+gap),chapterY,chapterW,chapterH);
                Position(gallery,margin,levelsY,bodyW,Mathf.Max(row,bottomY-levelsY-(h<240?4:8)));float cell=(bodyW-gap*2)/3;var actions=new[]{_campaignBack,_endless,_challenge};for(int i=0;i<3;i++){Position((RectTransform)actions[i].transform,margin+i*(cell+gap),bottomY,cell,row);GalleryTheme.Style(actions[i],GalleryTheme.Card);}SetLabel(_endless,StackCampaign.EndlessUnlocked(Game.Profile)?T("ENDLOS","ENDLESS"):T("ENDLOS · LV 5","ENDLESS · LV 5"));SetLabel(_challenge,T("TAGESROUTE","DAILY ROUTE"));
            }
            if(!_campaignOpen){_heroArt.GetComponent<TerracePreviewLease>().ApplyHero(_chapter,Game.Profile.Style,!landscape);GalleryTheme.Crop(_heroArt);}
            Canvas.ForceUpdateCanvases();float galleryGap=8*unit,galleryRow=UiMetrics.TargetSize(_home,54);_levelContent.sizeDelta=new Vector2(0,5*galleryRow+4*galleryGap);_levelContent.offsetMin=new Vector2(0,_levelContent.offsetMin.y);_levelContent.offsetMax=new Vector2(0,_levelContent.offsetMax.y);
            for(int i=0;i<10;i++){var rect=(RectTransform)_levels[i].transform;rect.anchorMin=new Vector2(i%2*.51f,1);rect.anchorMax=new Vector2(i%2*.51f+.49f,1);rect.pivot=new Vector2(.5f,1);rect.sizeDelta=new Vector2(0,galleryRow);rect.anchoredPosition=new Vector2(0,-i/2*(galleryRow+galleryGap));}
            Set((RectTransform)_styles.Find("Title"),.06f,.905f,.94f,.985f);Set((RectTransform)_styles.Find("Wallet"),.06f,.85f,.94f,.90f);_styles.GetComponent<Image>().material=null;
            float styleButtonH=UiMetrics.TargetSize(_styles,49)/Mathf.Max(1,_styles.rect.height);
            float styleHeader=(UiMetrics.TargetSize(_styles,49)+14*unit)/Mathf.Max(1,_styles.rect.height);
            Set((RectTransform)_styleScroll.transform,.025f,.15f,.975f,Mathf.Min(.915f,1-styleHeader));
            Set((RectTransform)_styles.Find("ExtraDesigns"),.52f,.03f,.975f,.03f+styleButtonH);Set((RectTransform)_styles.Find("Back"),.025f,.03f,.48f,.03f+styleButtonH);
            if(landscape)Set((RectTransform)_styleScroll.transform,.025f,.05f+styleButtonH,.975f,.83f);
            Canvas.ForceUpdateCanvases();float cardGap=8*unit,cardW=(_styleViewport.rect.width-cardGap*2)/3,cardH=_styleViewport.rect.height;
            _styleContent.sizeDelta=new Vector2(cardW*8+cardGap*7,0);_styleContent.anchoredPosition=new Vector2(_styleContent.anchoredPosition.x,0);
            var order=new[]{0,3,5,1,2,4,6,7};
            for(int position=0;position<8;position++)
            {
                int i=order[position];var rect=(RectTransform)_styleButtons[i].transform;
                float inset=landscape||Game.Profile.Style==i?0:Mathf.Min(24*unit,cardH*.06f),height=cardH-inset;
                rect.anchorMin=rect.anchorMax=new Vector2(0,1);rect.pivot=new Vector2(0,1);rect.sizeDelta=new Vector2(cardW,height);rect.anchoredPosition=new Vector2(position*(cardW+cardGap),-inset);
                GalleryTheme.Style(_styleButtons[i],DesignSurface(i));float labelHeight=Mathf.Min(.43f,49*unit/Mathf.Max(1,height));
                Set((RectTransform)rect.Find("SelectionPlate"),.06f,.02f,.94f,labelHeight+.005f);Set((RectTransform)rect.Find("Label"),.08f,.02f,.92f,labelHeight);UpdateDesignAction(i);
                float nameTop=landscape?.97f:.94f,nameBand=Mathf.Min(.40f,(w<250&&!landscape?35:22)*unit/Mathf.Max(1,height)),nameBottom=nameTop-nameBand;
                Set((RectTransform)rect.Find("DesignName"),.03f,nameBottom,.97f,nameTop);Set((RectTransform)rect.Find("MaterialName"),.03f,nameBottom-.08f,.97f,nameBottom-.01f);
                Set((RectTransform)rect.Find("StyleTower"),.02f,labelHeight+.02f,.98f,nameBottom-(landscape?.025f:.10f));GalleryTheme.Readable(rect.Find("DesignName").GetComponent<Text>(),13);GalleryTheme.Readable(rect.Find("MaterialName").GetComponent<Text>(),10.5f);rect.Find("MaterialName").gameObject.SetActive(!landscape);
            }
            _styles.Find("Title").gameObject.SetActive(landscape);
            GalleryTheme.Style(_styles.Find("ExtraDesigns").GetComponent<Button>(),GalleryTheme.Card);GalleryTheme.Style(_styles.Find("Back").GetComponent<Button>(),GalleryTheme.Card);
            var designBack=_styles.Find("Back").GetComponent<Button>();var designWallet=_styles.Find("StyleWallet").GetComponent<Button>();float styleUnit=UiMetrics.PointScale/Mathf.Max(.001f,_styles.GetComponentInParent<Canvas>().scaleFactor),styleWidth=_styles.rect.width/styleUnit,headerSize=UiMetrics.TargetSize(_styles,49)/styleUnit;
            if(!landscape){Top((RectTransform)designBack.transform,8*styleUnit,8*styleUnit,headerSize*styleUnit,headerSize*styleUnit);GalleryTheme.Style(designBack,Navy);UiFactory.ApplyCircularImage(designBack.GetComponent<Image>());SetLabel(designBack,"‹");Set((RectTransform)_styles.Find("ExtraDesigns"),.16f,.03f,.84f,.03f+styleButtonH);}
            Top((RectTransform)designWallet.transform,(styleWidth-Mathf.Min(116,styleWidth*.48f)-8)*styleUnit,8*styleUnit,Mathf.Min(116,styleWidth*.48f)*styleUnit,headerSize*styleUnit);GalleryTheme.Style(designWallet,Navy);UiFactory.ApplyPillImage(designWallet.GetComponent<Image>());GalleryTheme.Readable(designWallet.GetComponentInChildren<Text>(),13);_styleInfo.gameObject.SetActive(false);Set((RectTransform)_styles.Find("Title"),.08f,.84f,.92f,.90f);
            foreach(var button in _home.GetComponentsInChildren<Button>(true))GalleryTheme.Readable(button.GetComponentInChildren<Text>(),button==_continue?18:12);foreach(var button in _styleButtons)GalleryTheme.Readable(button.GetComponentInChildren<Text>(),13);GalleryTheme.Readable(title.GetComponent<Text>(),24);GalleryTheme.Readable(_homeInfo,13);GalleryTheme.Readable(_heroGoal,15);GalleryTheme.Readable(_styleInfo,13);GalleryTheme.Readable(_styles.Find("Title").GetComponent<Text>(),22);
            if(!_campaignOpen){GalleryTheme.Readable(_continue.GetComponentInChildren<Text>(),22);GalleryTheme.Readable(_campaignNavigation.GetComponentInChildren<Text>(),16);_campaignNavigation.GetComponentInChildren<Text>().font=UiFactory.SerifFont;foreach(var name in new[]{"Styles","City"})GalleryTheme.Readable(_home.Find(name).Find("Label").GetComponent<Text>(),14);if(w>=250&&(!landscape||h>=240))GalleryTheme.Readable(_home.Find("DirectShop").Find("Label").GetComponent<Text>(),14);}
            for(int chapter=0;chapter<3;chapter++){Set((RectTransform)_chapters[chapter].transform.Find("Label"),.02f,.17f,.98f,.37f);Set((RectTransform)_chapterArt[chapter].transform,.025f,.40f,.975f,.975f);GalleryTheme.Readable(_chapters[chapter].transform.Find("Label").GetComponent<Text>(),14);GalleryTheme.Readable(_chapters[chapter].transform.Find("ChapterProgress").GetComponent<Text>(),11);_chapters[chapter].transform.Find("ChapterProgress").gameObject.SetActive(!landscape);if(landscape)Set((RectTransform)_chapters[chapter].transform.Find("Label"),.02f,.03f,.98f,.37f);else if(w<250){Set((RectTransform)_chapters[chapter].transform.Find("Label"),.02f,.16f,.98f,.48f);Set((RectTransform)_chapterArt[chapter].transform,.025f,.51f,.975f,.975f);}}
            foreach(var image in _chapterArt)GalleryTheme.Crop(image);GalleryTheme.Crop(_heroArt);GalleryTheme.Crop(_styles.Find("DesignSkyline").GetComponent<RawImage>());
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
