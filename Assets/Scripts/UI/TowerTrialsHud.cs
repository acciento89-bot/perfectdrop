using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Kamilunavo.PerfectDrop.Trials;

namespace Kamilunavo.PerfectDrop.UI
{
    public sealed partial class StackHud
    {
        private RectTransform _trialPanel,_trialViewport,_trialContent;
        private ScrollRect _trialScroll;
        private Text _trialTitle,_trialBody;
        private Button _trialEntry,_trialPrimary,_trialAbandon,_trialHome,_trialReload;
        private Button[] _trialCards;
        private bool _trialResult,_trialError;
        public bool TrialMenuOpen => _trialPanel!=null && _trialPanel.gameObject.activeSelf;
        private void BuildTrialMenus()
        {
            _trialEntry=UiFactory.Button(_home,"TowerTrialsEntry",T("TURM-\nPRÜFUNGEN","TOWER\nTRIALS"),GalleryTheme.Card,GalleryTheme.Text,Vector2.zero,Vector2.one,ShowTrials);
            _trialPanel=UiFactory.Panel(_safe,"TowerTrials",GalleryTheme.Night,Vector2.zero,Vector2.one);
            var scrollObject=new GameObject("TowerTrialsScroll",typeof(RectTransform),typeof(ScrollRect));scrollObject.transform.SetParent(_trialPanel,false);
            Set((RectTransform)scrollObject.transform,.025f,.02f,.975f,.98f);
            _trialScroll=scrollObject.GetComponent<ScrollRect>();_trialScroll.horizontal=false;_trialScroll.movementType=ScrollRect.MovementType.Clamped;
            _trialViewport=UiFactory.Panel(scrollObject.transform,"Viewport",Color.clear,Vector2.zero,Vector2.one);_trialViewport.gameObject.AddComponent<RectMask2D>();
            _trialContent=new GameObject("TrialContent",typeof(RectTransform)).GetComponent<RectTransform>();_trialContent.SetParent(_trialViewport,false);
            _trialContent.anchorMin=new Vector2(0,1);_trialContent.anchorMax=Vector2.one;_trialContent.pivot=new Vector2(.5f,1);
            _trialScroll.viewport=_trialViewport;_trialScroll.content=_trialContent;
            _trialTitle=UiFactory.Label(_trialContent,"TrialTitle","",34,Vector2.zero,Vector2.one,TextAnchor.MiddleCenter,GalleryTheme.Gold,FontStyle.Bold);
            _trialBody=UiFactory.Label(_trialContent,"TrialDescription","",24,Vector2.zero,Vector2.one,TextAnchor.MiddleLeft,GalleryTheme.Text);
            _trialPrimary=UiFactory.Button(_trialContent,"TrialPrimary","",Gold,Navy,Vector2.zero,Vector2.one,()=>
            {
                if(MenuNavigationLocked)return;
                if(_trialError)Game.RetryTrialSave();else if(_trialResult)Game.RetryTrial();else Game.ResumeTrial();
            });
            _trialAbandon=UiFactory.Button(_trialContent,"TrialAbandon",T("LAUF BEENDEN · NEUE 3 VERSUCHE","END RUN · START A NEW 3 ATTEMPTS"),GalleryTheme.Card,GalleryTheme.Text,Vector2.zero,Vector2.one,Game.AbandonTrial);
            _trialHome=UiFactory.Button(_trialContent,"TrialHome",T("‹ MENÜ · SPIELSTAND BLEIBT","‹ HOME · PROGRESS KEPT"),GalleryTheme.Card,GalleryTheme.Text,Vector2.zero,Vector2.one,Game.GoHome);
            _trialReload=UiFactory.Button(_trialContent,"ReloadSavedTrials",T("GESPEICHERTEN STAND LADEN · NEUEN SCHRITT VERWERFEN","LOAD SAVED PROGRESS · DISCARD NEW STEP"),GalleryTheme.Card,GalleryTheme.Text,Vector2.zero,Vector2.one,Game.ReloadSavedTrials);
            _trialCards=new Button[12];
            for(int i=0;i<12;i++) {int id=i+1;_trialCards[i]=UiFactory.Button(_trialContent,"TowerTrial"+id,"",GalleryTheme.Card,GalleryTheme.Text,Vector2.zero,Vector2.one,()=>{if(!MenuNavigationLocked)Game.StartTrial(id);});}
            _trialPanel.gameObject.SetActive(false);_trialEntry.gameObject.SetActive(false);
        }
        public void HideTrialPanels() {if(_trialPanel!=null)_trialPanel.gameObject.SetActive(false);}
        public void ShowTrials()
        {
            if(MenuNavigationLocked)return;
            if(Game.TrialMode)Game.Save();
            if(Game.TrialSaveError!=null){ShowTrialSaveFailure();return;}
            HideMenus();_trialResult=false;_trialError=false;_trialPanel.gameObject.SetActive(true);
            _trialReload.gameObject.SetActive(false);
            var book=Game.Trials;var active=book.Active;
            bool running=active!=null && active.State==TrialAttemptState.Running;
            _trialTitle.text=T("TURM-PRÜFUNGEN","TOWER TRIALS");
            _trialBody.text=T("12 Prüfungen · eigene Medaillen\nJe Lauf 3 kostenlose Versuche. Bronze öffnet die nächste Prüfung. Keine Kräfte, Risiko oder Coin-Belohnungen.\nDer bisherige Level-Lauf bleibt erhalten.",
                "12 trials · independent medals\nEach run has 3 free attempts. Bronze opens the next trial. No powers, risk or coin rewards.\nYour existing level run is kept.");
            _trialPrimary.gameObject.SetActive(running);_trialAbandon.gameObject.SetActive(running);
            _trialHome.gameObject.SetActive(true);_trialHome.onClick.RemoveAllListeners();_trialHome.onClick.AddListener(Game.GoHome);SetLabel(_trialHome,T("‹ MENÜ · SPIELSTAND BLEIBT","‹ HOME · PROGRESS KEPT"));
            if(running)SetLabel(_trialPrimary,T("FORTSETZEN · PRÜFUNG ","RESUME · TRIAL ")+active.TrialId+" · "+T("VERSUCH ","ATTEMPT ")+active.AttemptNumber+" / 3");
            foreach(var card in _trialCards)
            {
                int id=System.Array.IndexOf(_trialCards,card)+1;var definition=TowerTrialCatalog.Get(id);var record=book.Records.FirstOrDefault(x=>x.TrialId==id);
                bool unlocked=TrialRules.Unlocked(book,id);card.gameObject.SetActive(true);card.interactable=unlocked&&!running;
                SetLabel(card,T("PRÜFUNG ","TRIAL ")+id+" · "+ModifierText(definition.Modifier)+" · "+definition.Target+T(" Blöcke"," blocks")+"\n"+GoalText(definition)+"\n"+
                    (unlocked?T("BESTE: ","BEST: ")+MedalText(record==null?TrialMedal.None:record.BestMedal):T("GESPERRT · VORHER BRONZE ERREICHEN","LOCKED · EARN BRONZE IN PREVIOUS TRIAL")));
            }
            Layout(true);_trialScroll.verticalNormalizedPosition=1;Refresh();
        }
        public void ShowTrialResult()
        {
            HideMenus();_trialResult=true;_trialError=false;_trialPanel.gameObject.SetActive(true);
            _trialReload.gameObject.SetActive(false);
            var active=Game.Trials.Active;var definition=Game.TrialDefinition;
            _trialTitle.text=active.State==TrialAttemptState.Cleared?MedalText(active.Medal):active.State==TrialAttemptState.Failed?T("DANEBEN","MISSED"):T("ZIEL NOCH NICHT ERREICHT","GOAL NOT REACHED");
            _trialBody.text=T("PRÜFUNG ","TRIAL ")+definition.Id+" · "+ModifierText(definition.Modifier)+"\n"+GoalText(definition)+"\n"+
                T("Gestapelt: ","Stacked: ")+Game.Run.Count+" / "+definition.Target+"\n"+T("Perfekt: ","Perfect: ")+Game.Run.PerfectDrops+T(" · Beste Serie: "," · Best streak: ")+Game.Run.MaxStreak+"\n"+
                T("Restfläche: ","Area retained: ")+Mathf.RoundToInt(Game.Run.Top.Size.x*Game.Run.Top.Size.y/(definition.Width*definition.Width)*100)+"%\n"+
                T("Versuch ","Attempt ")+active.AttemptNumber+" / 3 · "+T("Gespeichert","Saved");
            bool retry=active.State!=TrialAttemptState.Cleared && active.AttemptNumber<3;
            _trialPrimary.gameObject.SetActive(retry);SetLabel(_trialPrimary,T("NÄCHSTER VERSUCH ","NEXT ATTEMPT ")+(active.AttemptNumber+1)+" / 3");
            _trialAbandon.gameObject.SetActive(false);
            foreach(var card in _trialCards)card.gameObject.SetActive(false);
            // A terminal run can always return to the ladder and start a new free run.
            _trialHome.gameObject.SetActive(true);_trialHome.onClick.RemoveAllListeners();_trialHome.onClick.AddListener(ShowTrials);SetLabel(_trialHome,T("PRÜFUNGSÜBERSICHT · ERNEUT SPIELEN","TRIAL LADDER · PLAY AGAIN"));
            Layout(true);_trialScroll.verticalNormalizedPosition=1;Refresh();
        }
        public void ShowTrialSaveFailure()
        {
            HideMenus();_trialResult=false;_trialError=true;_trialPanel.gameObject.SetActive(true);
            _trialTitle.text=T("NICHT GESPEICHERT","NOT SAVED");
            _trialBody.text=T("Der neue Schritt wurde nicht übernommen. Vorhandene Prüfungsdaten bleiben erhalten. Prüfe den freien Speicher und versuche es erneut. Unbekannte oder beschädigte Daten werden nicht überschrieben.",
                "The new step was not applied. Existing trial data is kept. Check free storage and retry. Unknown or damaged data will not be overwritten.");
            _trialPrimary.gameObject.SetActive(true);SetLabel(_trialPrimary,T("SPEICHERN ERNEUT VERSUCHEN","RETRY SAVE"));
            _trialReload.gameObject.SetActive(true);
            _trialAbandon.gameObject.SetActive(false);foreach(var card in _trialCards)card.gameObject.SetActive(false);
            _trialHome.onClick.RemoveAllListeners();_trialHome.onClick.AddListener(Game.GoHome);SetLabel(_trialHome,T("‹ MENÜ","‹ HOME"));
            _trialHome.gameObject.SetActive(!Game.TrialMode);
            Layout(true);_trialScroll.verticalNormalizedPosition=1;Refresh();
        }
        private void RefreshTrialHud()
        {
            _stats[1].transform.parent.Find("Heading").GetComponent<Text>().text=Game.TrialMode?T("PRÜFUNG","TRIAL"):T("BESTE","BEST");
            _stats[3].transform.parent.Find("Heading").GetComponent<Text>().text=Game.TrialMode?T("VERSUCH","ATTEMPT"):"COINS";
            if(!Game.TrialMode)return;
            var definition=Game.TrialDefinition;var active=Game.Trials.Active;
            _stats[1].text=definition.Id.ToString();_stats[3].text=active.AttemptNumber+"/3";
            string goal=definition.Goal==TrialGoal.Perfects?T("Perfekt","Perfect"):definition.Goal==TrialGoal.Streak?T("Serie","Streak"):T("Fläche","Area");
            _hint.text=goal+" ≥ "+Mathf.RoundToInt(definition.Bronze*(definition.Goal==TrialGoal.Area?100:1))+(definition.Goal==TrialGoal.Area?"%":"")+T(" · Bronze · Turm fertigstellen"," · Bronze · finish tower");
            _drop.interactable=_drop.interactable && Game.TrialSaveError==null;
        }
        private static string ModifierText(TrialModifier modifier)
        {return modifier==TrialModifier.Steady?T("RUHIG","STEADY"):modifier==TrialModifier.Fragile?T("BRÜCHIG · JEDER 2. BLOCK","FRAGILE · EVERY 2ND BLOCK"):modifier==TrialModifier.Drift?"DRIFT":T("WIND","WIND");}
        private static string MedalText(TrialMedal medal)
        {return medal==TrialMedal.Gold?T("GOLD","GOLD"):medal==TrialMedal.Silver?T("SILBER","SILVER"):medal==TrialMedal.Bronze?T("BRONZE","BRONZE"):T("NOCH KEINE","NONE YET");}
        private static string GoalText(TrialDefinition definition)
        {
            string target=definition.Goal==TrialGoal.Perfects?T("Perfekte Blöcke","Perfect drops"):definition.Goal==TrialGoal.Streak?T("Beste Serie","Best streak"):T("Restfläche","Retained area");
            float factor=definition.Goal==TrialGoal.Area?100:1;string suffix=definition.Goal==TrialGoal.Area?"%":"";
            return target+": "+Mathf.RoundToInt(definition.Bronze*factor)+" / "+Mathf.RoundToInt(definition.Silver*factor)+" / "+Mathf.RoundToInt(definition.Gold*factor)+suffix+" · "+T("Bronze/Silber/Gold · Turm fertigstellen","Bronze/Silver/Gold · finish tower");
        }
        private void LayoutTrials(Rect pane)
        {
            if(_trialPanel==null)return;
            Place(_trialPanel,pane,new Rect(0,0,1,1));Canvas.ForceUpdateCanvases();
            float unit=UiMetrics.PointScale/Mathf.Max(.001f,_trialPanel.GetComponentInParent<Canvas>().scaleFactor);
            float width=_trialViewport.rect.width,row=UiMetrics.TargetSize(_trialPanel,54),gap=8*unit;
            float y=0;
            void Item(RectTransform rect,float height) {Top(rect,0,y,width,height);y+=height+gap;}
            Item((RectTransform)_trialTitle.transform,Mathf.Max(row,64*unit));
            Item((RectTransform)_trialBody.transform,(_trialResult?190:180)*unit*Mathf.Max(1,300*unit/Mathf.Max(width,1)));
            if(_trialPrimary.gameObject.activeSelf)Item((RectTransform)_trialPrimary.transform,row);
            if(_trialAbandon.gameObject.activeSelf)Item((RectTransform)_trialAbandon.transform,row*1.25f);
            if(_trialHome.gameObject.activeSelf)Item((RectTransform)_trialHome.transform,row);
            if(_trialReload.gameObject.activeSelf)Item((RectTransform)_trialReload.transform,row*1.5f);
            foreach(var card in _trialCards)if(card.gameObject.activeSelf)Item((RectTransform)card.transform,Mathf.Max(row*2.5f,130*unit)*Mathf.Max(1,300*unit/Mathf.Max(width,1)));
            _trialContent.sizeDelta=new Vector2(0,y);
            foreach(var text in _trialContent.GetComponentsInChildren<Text>(true))GalleryTheme.Readable(text,15);
        }
    }
}
