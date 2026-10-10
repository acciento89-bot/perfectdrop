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
        private int _chapter;private RectTransform _heroGallery;private RawImage _heroArt;private Text _heroGoal;private RawImage[] _chapterArt;
        private bool CampaignMenuOpen => ShopOpen || (_home!=null && _home.gameObject.activeSelf) || (_styles!=null && _styles.gameObject.activeSelf);
        private void BuildCampaignMenus()
        {
            _home=UiFactory.Panel(_safe,"LevelMap",GalleryTheme.Ivory,Vector2.zero,Vector2.one);
            _heroGallery=UiFactory.Panel(_home,"SpecimenGallery",Navy,Vector2.zero,Vector2.one);_heroGallery.gameObject.AddComponent<RectMask2D>();
            _heroArt=GalleryTheme.Art(_heroGallery,"ArchitecturalScene",1,Vector2.zero,Vector2.one,true);
            UiFactory.Panel(_home,"MapHeader",GalleryTheme.Ivory,Vector2.zero,Vector2.one);
            TowerPreviewGraphic.AddStyle(_home,"HeroTower",0,Vector2.zero,Vector2.one,6);
            UiFactory.Label(_home,"Tagline",T("DEIN TIMING. DEINE SKYLINE.","YOUR TIMING. YOUR SKYLINE."),23,Vector2.zero,Vector2.one,TextAnchor.MiddleLeft,GalleryTheme.Ink,FontStyle.Bold);
            UiFactory.Panel(_home,"MapFooter",GalleryTheme.Ivory,Vector2.zero,Vector2.one);
            UiFactory.Label(_home,"Title","PERFECT DROP",49,Vector2.zero,Vector2.one,TextAnchor.MiddleLeft,GalleryTheme.Ink,FontStyle.Bold);
            _homeInfo=UiFactory.Label(_home,"Progress","",27,Vector2.zero,Vector2.one,TextAnchor.MiddleLeft,GalleryTheme.Ink);
            _heroGoal=UiFactory.Label(_home,"HeroGoal","",25,Vector2.zero,Vector2.one,TextAnchor.MiddleCenter,Color.white,FontStyle.Bold);var goalBack=UiFactory.Panel(_home,"HeroGoalPlate",Navy,Vector2.zero,Vector2.one);goalBack.SetSiblingIndex(_heroGoal.transform.GetSiblingIndex());
            _chapters=new Button[3];_chapterArt=new RawImage[3];
            var names=new[]{T("Wolkenwerk","Cloud Works"),T("Abendturm","Sunset Spire"),T("Neonstadt","Neon City")};
            for(var i=0;i<3;i++)
            {
                var chapter=i;
                _chapters[i]=UiFactory.Button(_home,"Chapter"+i,names[i],GalleryTheme.Paper,GalleryTheme.Ink,Vector2.zero,Vector2.one,()=>{_chapter=chapter;_levelScroll.verticalNormalizedPosition=1;RefreshHome();});
                _chapterArt[i]=GalleryTheme.Art(_chapters[i].transform,"ChapterArt",i,new Vector2(.03f,.35f),new Vector2(.97f,.98f));Set((RectTransform)_chapters[i].transform.Find("Label"),.03f,.03f,.97f,.34f);GalleryTheme.Style(_chapters[i],GalleryTheme.Paper);
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
                _levels[i]=UiFactory.Button(_levelContent,"LevelSlot"+i,"",GalleryTheme.Paper,GalleryTheme.Ink,Vector2.zero,Vector2.one,()=>Game.StartLevel(_chapter*10+slot+1));
                var card=(RectTransform)_levels[i].transform;
                var label=(RectTransform)card.Find("Label"); Set(label,.24f,.12f,.95f,.90f);
                _levelIcons[i]=UiFactory.Icon(card,"LevelIcon",HudIconType.Floors,Gold,new Vector2(.035f,.22f),new Vector2(.23f,.83f));
                _levelTowers[i]=TowerPreviewGraphic.Add(card,"LevelTower",Gold,new Vector2(.01f,.19f),new Vector2(.255f,.94f),3+i%3);
                for(var star=0;star<3;star++)
                    _levelStars[i,star]=UiFactory.Icon(card,"Star"+star,HudIconType.Star,Gold,new Vector2(.035f+star*.062f,.07f),new Vector2(.093f+star*.062f,.29f));
            }
            _continue=UiFactory.Button(_home,"Continue",T("LAUF FORTSETZEN","CONTINUE RUN"),Gold,Navy,new Vector2(.07f,.22f),new Vector2(.93f,.29f),StartOrContinue);
            _endless=UiFactory.Button(_home,"Endless","",GalleryTheme.Paper,GalleryTheme.Ink,new Vector2(.07f,.13f),new Vector2(.93f,.20f),Game.StartEndless);
            UiFactory.Button(_home,"Styles",T("TURM-DESIGNS","TOWER DESIGNS"),Navy,Color.white,new Vector2(.07f,.04f),new Vector2(.93f,.11f),ShowStyles);
            _styles=UiFactory.Panel(_safe,"StyleShop",GalleryTheme.Ivory,Vector2.zero,Vector2.one);
            UiFactory.Label(_styles,"Title",T("TURM-DESIGNS","TOWER DESIGNS"),50,new Vector2(.06f,.9f),new Vector2(.94f,.98f),TextAnchor.MiddleCenter,GalleryTheme.Ink,FontStyle.Bold);
            _styleInfo=UiFactory.Label(_styles,"Wallet","",34,new Vector2(.06f,.82f),new Vector2(.94f,.89f),TextAnchor.MiddleCenter,GalleryTheme.Ink);
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
            UiFactory.Button(_styles,"Back",T("ZURÜCK ZU LEVELS","BACK TO LEVELS"),GalleryTheme.Paper,GalleryTheme.Ink,new Vector2(.08f,.04f),new Vector2(.92f,.15f),ShowHome);
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
            _home.Find("HeroTower").GetComponent<TowerPreviewGraphic>().ApplyStyle(Game.Profile.Style);_heroGoal.text="Level "+Game.Profile.UnlockedLevel+" · "+StackCampaign.Level(Game.Profile.UnlockedLevel).Target+T(" Blöcke"," blocks");
            var family=_chapter==0?"CloudCityDay":_chapter==2?"CloudCityNight":"CloudCity";_heroArt.texture=Resources.Load<Texture2D>("Art/"+family+"Portrait");GalleryTheme.Crop(_heroArt);
            _homeInfo.text=stars+" / 90 "+T("STERNE","STARS")+" · "+Game.Profile.Coins+" COINS";
            for(var i=0;i<3;i++)
            { _chapters[i].interactable=Game.Profile.UnlockedLevel>i*10;GalleryTheme.Style(_chapters[i],i==_chapter?Navy:GalleryTheme.Paper); }
            for(var i=0;i<10;i++)
            {
                var id=_chapter*10+i+1; var unlocked=id<=Game.Profile.UnlockedLevel;
                var starsHere=Game.Profile.LevelStars[id-1];
                var current=unlocked && id==Game.Profile.UnlockedLevel;
                SetLabel(_levels[i],"LEVEL "+id+(current?"  ›":"")+"\n"+(unlocked?StackCampaign.Level(id).Target+" "+T("BLÖCKE","BLOCKS"):T("GESPERRT","LOCKED")));
                GalleryTheme.Style(_levels[i],current?Navy:GalleryTheme.Paper);
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
                GalleryTheme.Style(_styleButtons[i],Game.Profile.Style==i?Navy:GalleryTheme.Paper);
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
            float unit=UiMetrics.PointScale/Mathf.Max(.001f,_home.GetComponentInParent<Canvas>().scaleFactor);float w=_home.rect.width/unit,h=_home.rect.height/unit,row=UiMetrics.TargetSize(_home,49)/unit,gap=8,play=UiMetrics.TargetSize(_home,54)/unit;
            void Position(RectTransform rect,float x,float y,float width,float height)=>Top(rect,x*unit,y*unit,width*unit,height*unit);
            var gallery=(RectTransform)_levelScroll.transform;
            var utilities=new[]{_home.Find("Styles").GetComponent<Button>(),_home.Find("City").GetComponent<Button>(),_endless,_challenge,_home.Find("HomeSettings").GetComponent<Button>(),_home.Find("Learn").GetComponent<Button>()};
            float contentX=landscape?w*.42f:8,contentW=landscape?w-contentX-8:w-16;
            float playY=h-play-8,utilityY=playY-gap-row*2-gap,chapterY=landscape?8:Mathf.Max(84,h*.39f),chapterH=landscape?Mathf.Max(row+28,h*.26f):Mathf.Max(row+26,h*.14f),levelsY=chapterY+chapterH+8,levelBottom=utilityY-8;
            if(!landscape){levelsY=utilityY-Mathf.Max(row+12,h*.13f);chapterY=levelsY-chapterH-8;}
            Position((RectTransform)_home.Find("MapHeader"),8,8,landscape?contentX-16:w-16,62);
            Position((RectTransform)_home.Find("Title"),16,8,landscape?contentX-32:w-32,34);
            Position((RectTransform)_home.Find("Progress"),16,42,landscape?contentX-32:w-32,23);
            float heroX=8,heroY=74,heroW=landscape?contentX-16:w-16,heroH=landscape?h-play-90:chapterY-heroY-8;
            Position(_heroGallery,heroX,heroY,heroW,Mathf.Max(60,heroH));Position((RectTransform)_home.Find("HeroTower"),heroX+heroW*.19f,heroY+4,heroW*.62f,Mathf.Max(50,heroH-40));
            Position((RectTransform)_home.Find("HeroGoalPlate"),heroX+8,heroY+heroH-34,heroW-16,30);Position((RectTransform)_heroGoal.transform,heroX+8,heroY+heroH-34,heroW-16,30);_home.Find("Tagline").gameObject.SetActive(false);
            Position((RectTransform)_home.Find("MapFooter"),contentX,utilityY-5,contentW,h-utilityY);
            for(var i=0;i<3;i++){float cell=(contentW-12)/3;Position((RectTransform)_chapters[i].transform,contentX+i*(cell+6),chapterY,cell,chapterH);string[] titles=cell<80?new[]{T("Wolken\nwerk","Cloud\nWorks"),T("Abend\nturm","Sunset\nSpire"),T("Neon\nstadt","Neon\nCity")}:new[]{T("Wolkenwerk","Cloud Works"),T("Abendturm","Sunset Spire"),T("Neonstadt","Neon City")};SetLabel(_chapters[i],titles[i]);}
            if(landscape)levelsY=chapterY+chapterH+8;
            Position(gallery,contentX,levelsY,contentW,Mathf.Max(30,levelBottom-levelsY));
            for(var i=0;i<6;i++){float cell=(contentW-12)/3;Position((RectTransform)utilities[i].transform,contentX+i%3*(cell+6),utilityY+i/3*(row+gap),cell,row);GalleryTheme.Style(utilities[i],i<2?Navy:GalleryTheme.Paper);}
            Position((RectTransform)_continue.transform,8,playY,w-16,play);GalleryTheme.Style(_continue,Gold);
            Canvas.ForceUpdateCanvases();
            var galleryGap=8f;var galleryRow=Mathf.Max(UiMetrics.TargetSize(_home,54f),(_levelViewport.rect.height-4*galleryGap)/5f);
            var contentHeight=Mathf.Max(_levelViewport.rect.height,5*galleryRow+4*galleryGap);
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
            foreach(var button in _styleButtons)GalleryTheme.Style(button,button==_styleButtons[Game.Profile.Style<4?Game.Profile.Style:0]&&Game.Profile.Style<4?Navy:GalleryTheme.Paper);
            foreach(var button in _home.GetComponentsInChildren<Button>(true))GalleryTheme.Readable(button.GetComponentInChildren<Text>(),button==_continue?18:13);foreach(var button in _styleButtons)GalleryTheme.Readable(button.GetComponentInChildren<Text>(),13);GalleryTheme.Readable(_home.Find("Title").GetComponent<Text>(),26);GalleryTheme.Readable(_homeInfo,13);GalleryTheme.Readable(_heroGoal,13);GalleryTheme.Readable(_styleInfo,13);GalleryTheme.Readable(_styles.Find("Title").GetComponent<Text>(),22);foreach(var image in _chapterArt)GalleryTheme.Crop(image);GalleryTheme.Crop(_heroArt);
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
