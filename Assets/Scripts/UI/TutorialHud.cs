using UnityEngine;
using UnityEngine.UI;
using Kamilunavo.PerfectDrop.Gameplay;
namespace Kamilunavo.PerfectDrop.UI
{
    public sealed partial class StackHud
    {
        private bool _tutorialActive;
        private int _tutorialStep;
        private Button _tutorialSkip;
        public bool TutorialActive=>_tutorialActive;
        public int TutorialStep=>_tutorialStep;
        private void BuildTutorial()
        {
            UiFactory.Button(_home,"Learn",T("LERNEN","LEARN"),GalleryTheme.Paper,GalleryTheme.Ink,Vector2.zero,Vector2.one,()=>
            {
                // Replay observes the current live run; no reset or extra rewards.
                if(!Game.Profile.ResumeActive || Game.Run.Completed || Game.Run.Failed)Game.StartLevel(Game.Profile.UnlockedLevel);
                StartTutorial();
            });
            _tutorialSkip=UiFactory.Button(_objective,"SkipGuide",T("FERTIG","DONE"),new Color(.13f,.2f,.3f),Color.white,new Vector2(.75f,.15f),new Vector2(.965f,.93f),EndTutorial);
            _tutorialSkip.gameObject.SetActive(false);
        }
        public void StartTutorial()
        {
            HideMenus();_tutorialStep=0;_tutorialActive=true;UpdateTutorial();
        }
        private void EndTutorial()
        {
            _tutorialActive=false;Game.Profile.TutorialComplete=true;Game.Save();
            _tutorialSkip.gameObject.SetActive(false);_objective.Find("Menu").gameObject.SetActive(true);ResetMessage();
        }
        private void TutorialPlacement(StackGrade grade)
        {
            if(!_tutorialActive || grade==StackGrade.Miss)return;
            if(_tutorialStep==1 && grade!=StackGrade.Perfect)return;
            _tutorialStep++;
            if(_tutorialStep>=3){EndTutorial();ArcadeFeedback(T("BEREIT FÜR DEINEN TURM!","READY TO BUILD YOUR TOWER!"));}
        }
        private void UpdateTutorial()
        {
            if(_tutorialSkip==null)return;
            _tutorialSkip.gameObject.SetActive(_tutorialActive && !ModalOpen);
            _objective.Find("Menu").gameObject.SetActive(!_tutorialActive);
            if(!_tutorialActive || ModalOpen)return;
            _status.text=T("SO GEHT'S · ","HOW TO PLAY · ")+(_tutorialStep+1)+" / 3";
            _hint.text=_tutorialStep==0?T("Überlapp abwarten → ABSETZEN tippen.","Wait for overlap → tap DROP."):
                _tutorialStep==1?T("Mittig absetzen: PERFECT erhält die Fläche.","Drop at the center: PERFECT keeps the area."):
                T("Jetzt die neue Richtung treffen. Perfect lädt Energie.","Now match the new direction. Perfect charges energy.");
        }
    }
}
