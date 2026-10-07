using UnityEngine;
using UnityEngine.UI;
using Kamilunavo.PerfectDrop.Gameplay;
namespace Kamilunavo.PerfectDrop.UI
{
    public sealed partial class StackHud
    {
        private RectTransform _home,_styles;
        private Text _homeInfo,_styleInfo;
        private Button[] _levels,_chapters,_styleButtons;
        private Button _endless,_continue,_next,_map;
        private int _chapter;
        private bool CampaignMenuOpen => (_home!=null && _home.gameObject.activeSelf) || (_styles!=null && _styles.gameObject.activeSelf);
        private void BuildCampaignMenus()
        {
            _home=UiFactory.Panel(_safe,"LevelMap",Navy,Vector2.zero,Vector2.one);
            UiFactory.Label(_home,"Title","PERFECT DROP",54,new Vector2(.06f,.90f),new Vector2(.94f,.98f),TextAnchor.MiddleCenter,Gold,FontStyle.Bold);
            _homeInfo=UiFactory.Label(_home,"Progress","",30,new Vector2(.06f,.83f),new Vector2(.94f,.9f),TextAnchor.MiddleCenter,Color.white);
            _chapters=new Button[3];
            var names=new[]{T("WOLKENWERK","CLOUD WORKS"),T("ABENDTURM","SUNSET SPIRE"),T("NEONSTADT","NEON CITY")};
            for(var i=0;i<3;i++)
            {
                var chapter=i;
                _chapters[i]=UiFactory.Button(_home,"Chapter"+i,names[i],new Color(.14f,.2f,.3f),Color.white,new Vector2(.05f+i*.30f,.745f),new Vector2(.335f+i*.30f,.82f),()=>{_chapter=chapter;RefreshHome();});
            }
            _levels=new Button[10];
            for(var i=0;i<10;i++)
            {
                var slot=i;
                _levels[i]=UiFactory.Button(_home,"LevelSlot"+i,"",Gold,Navy,Vector2.zero,Vector2.one,()=>Game.StartLevel(_chapter*10+slot+1));
            }
            _continue=UiFactory.Button(_home,"Continue",T("LAUF FORTSETZEN","CONTINUE RUN"),Gold,Navy,new Vector2(.07f,.22f),new Vector2(.93f,.29f),HideMenus);
            _endless=UiFactory.Button(_home,"Endless","",new Color(.14f,.2f,.3f),Color.white,new Vector2(.07f,.13f),new Vector2(.93f,.20f),Game.StartEndless);
            UiFactory.Button(_home,"Styles",T("TURM-DESIGNS","TOWER DESIGNS"),new Color(.14f,.2f,.3f),Color.white,new Vector2(.07f,.04f),new Vector2(.93f,.11f),ShowStyles);
            _styles=UiFactory.Panel(_safe,"StyleShop",Navy,Vector2.zero,Vector2.one);
            UiFactory.Label(_styles,"Title",T("TURM-DESIGNS","TOWER DESIGNS"),50,new Vector2(.06f,.9f),new Vector2(.94f,.98f),TextAnchor.MiddleCenter,Gold,FontStyle.Bold);
            _styleInfo=UiFactory.Label(_styles,"Wallet","",34,new Vector2(.06f,.82f),new Vector2(.94f,.89f),TextAnchor.MiddleCenter,Color.white);
            _styleButtons=new Button[4];
            for(var i=0;i<4;i++)
            {
                var style=i;
                _styleButtons[i]=UiFactory.Button(_styles,"Style"+i,"",new Color(.14f,.2f,.3f),Color.white,new Vector2(.08f,.66f-i*.155f),new Vector2(.92f,.79f-i*.155f),()=>{if(Game.SelectStyle(style))Game.UiClick();RefreshStyles();});
            }
            UiFactory.Button(_styles,"Back",T("ZURÜCK ZU LEVELS","BACK TO LEVELS"),Gold,Navy,new Vector2(.08f,.04f),new Vector2(.92f,.15f),ShowHome);
            _next=UiFactory.Button(_popup,"Next",T("NÄCHSTES LEVEL","NEXT LEVEL"),Gold,Navy,new Vector2(.10f,.43f),new Vector2(.90f,.56f),()=>Game.StartLevel(Game.Level.Id+1));
            _map=UiFactory.Button(_popup,"LevelMapAction",T("LEVELÜBERSICHT","LEVEL MAP"),new Color(.14f,.2f,.3f),Color.white,new Vector2(.10f,.32f),new Vector2(.90f,.42f),Game.GoHome);
            _home.gameObject.SetActive(false); _styles.gameObject.SetActive(false); HideTerminalActions();
        }
        public void HideMenus()
        {
            HideArcadeMenus();
            _home.gameObject.SetActive(false);_styles.gameObject.SetActive(false);_popup.gameObject.SetActive(false);HideTerminalActions();
            Set((RectTransform)_popupTitle.transform,.07f,.79f,.93f,.96f); Refresh();
        }
        public void ShowHome()
        {
            _settingsReturnsHome=false;HideMenus();_home.gameObject.SetActive(true);RefreshHome();Refresh();
        }
        private void RefreshHome()
        {
            var stars=0;foreach(var score in Game.Profile.LevelStars)stars+=score;
            _homeInfo.text=stars+" / 90 "+T("STERNE","STARS")+" · "+Game.Profile.Coins+" COINS";
            for(var i=0;i<3;i++)
            { _chapters[i].interactable=Game.Profile.UnlockedLevel>i*10;_chapters[i].GetComponent<Image>().color=i==_chapter?Gold:new Color(.14f,.2f,.3f); }
            for(var i=0;i<10;i++)
            {
                var id=_chapter*10+i+1; var unlocked=id<=Game.Profile.UnlockedLevel;
                var starsHere=Game.Profile.LevelStars[id-1];
                SetLabel(_levels[i],"LEVEL "+id+"\n"+(unlocked?StackCampaign.Level(id).Target+" "+T("BLÖCKE","BLOCKS")+" · "+starsHere+" ★":T("GESPERRT","LOCKED")));
                _levels[i].interactable=unlocked;
            }
            var endless=StackCampaign.EndlessUnlocked(Game.Profile);
            SetLabel(_endless,endless?T("ENDLOS · BESTE ","ENDLESS · BEST ")+Game.Profile.EndlessBest:T("ENDLOS: LEVEL 5 ABSCHLIESSEN","ENDLESS: COMPLETE LEVEL 5"));
            _endless.interactable=endless;RefreshArcadeHome();
            _continue.gameObject.SetActive(Game.Profile.ResumeActive && !Game.Run.Failed && !Game.Run.Completed);
        }
        private void ShowStyles() { HideMenus();_styles.gameObject.SetActive(true);RefreshStyles();Refresh(); }
        private void RefreshStyles()
        {
            _styleInfo.text=Game.Profile.Coins+" COINS";
            var names=new[]{T("SIGNAL-GOLD","SIGNAL GOLD"),T("EISBLAU","ICE BLUE"),T("NEON-PINK","NEON PINK"),T("JADE","JADE")};
            for(var i=0;i<4;i++)
            {
                var owned=(Game.Profile.OwnedStyles&(1<<i))!=0;
                SetLabel(_styleButtons[i],names[i]+" · "+(Game.Profile.Style==i?T("AKTIV","ACTIVE"):owned?T("AUSWÄHLEN","SELECT"):StackCampaign.StyleCost(i)+" COINS"));
                _styleButtons[i].interactable=owned || Game.Profile.Coins>=StackCampaign.StyleCost(i);
            }
        }
        private void ShowTerminalActions(bool won) { _map.gameObject.SetActive(true);_next.gameObject.SetActive(won && !Game.Profile.RunChallenge && Game.Level.Id<30); }
        private void HideTerminalActions() { if(_map!=null)_map.gameObject.SetActive(false);if(_next!=null)_next.gameObject.SetActive(false); }
        private void LayoutCampaign(Rect pane)
        {
            if(_home==null)return;
            Place(_home,pane,new Rect(.04f,.015f,.92f,.97f));Place(_styles,pane,new Rect(.04f,.015f,.92f,.97f));
            var landscape=Screen.width*pane.width > Screen.height*pane.height*1.2f;
            for(var i=0;i<10;i++)
            {
                var cols=landscape?5:2;var rows=landscape?2:5;
                var width=.9f/cols;var height=.43f/rows;
                Set((RectTransform)_levels[i].transform,.05f+(i%cols)*width,.73f-(i/cols+1)*height,.05f+(i%cols+1)*width-.018f,.73f-(i/cols)*height-.012f);
            }
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
