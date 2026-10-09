using System;
using UnityEngine;
using UnityEngine.UI;
using Kamilunavo.PerfectDrop.Gameplay;
namespace Kamilunavo.PerfectDrop.UI
{
    public sealed partial class StackHud
    {
        private RectTransform _powerRow,_cityPanel;
        private Text _powerInfo,_cityInfo;
        private Button[] _powerButtons;
        private Button _risk,_challenge;
        private Button[] _cityTabs;
        private int _powerSignature=int.MinValue;
        public bool CityOpen => _cityPanel!=null && _cityPanel.gameObject.activeSelf;
        private void BuildArcadeMenus()
        {
            _powerRow=UiFactory.Panel(_safe,"Powers",Color.clear,Vector2.zero,Vector2.one);
            var powerInfoCard=UiFactory.Panel(_powerRow,"PowerInfoCard",Navy,new Vector2(0,.74f),new Vector2(.985f,1));
            _powerInfo=UiFactory.Label(powerInfoCard,"Energy","",28,new Vector2(.02f,0),new Vector2(.98f,1),TextAnchor.MiddleCenter,Gold,FontStyle.Bold);
            _powerButtons=new Button[3];
            for(var i=0;i<3;i++)
            {
                var power=(StackPower)i;
                _powerButtons[i]=UiFactory.Button(_powerRow,"Power"+power,"",new Color(.13f,.2f,.3f),Color.white,new Vector2(i*.25f,.03f),new Vector2(i*.25f+.235f,.70f),()=>Game.UsePower(power));
            }
            _risk=UiFactory.Button(_powerRow,"Risk","",new Color(.13f,.2f,.3f),Color.white,new Vector2(.75f,.03f),new Vector2(.985f,.70f),Game.ToggleRisk);
            UiFactory.Button(_home,"HomeSettings",T("TON / OPTIONEN","SOUND / OPTIONS"),new Color(.13f,.2f,.3f),Color.white,new Vector2(.07f,.22f),new Vector2(.49f,.29f),OpenSettings);
            Set((RectTransform)_continue.transform,.51f,.22f,.93f,.29f);
            Set((RectTransform)_endless.transform,.07f,.13f,.49f,.20f);
            Set((RectTransform)_home.Find("Styles"),.07f,.04f,.49f,.11f);
            _challenge=UiFactory.Button(_home,"Challenge","",new Color(.13f,.2f,.3f),Color.white,new Vector2(.51f,.13f),new Vector2(.93f,.20f),Game.StartChallenge);
            UiFactory.Button(_home,"City",T("MEINE STADT","MY CITY"),new Color(.13f,.2f,.3f),Color.white,new Vector2(.51f,.04f),new Vector2(.93f,.11f),ShowCity);
            _cityPanel=UiFactory.Panel(_safe,"CityView",Color.clear,Vector2.zero,Vector2.one);
            var header=UiFactory.Panel(_cityPanel,"CityHeader",Navy,new Vector2(.03f,.81f),new Vector2(.97f,.98f));
            UiFactory.Label(header,"Title",T("DEINE STADT WÄCHST","YOUR CITY IS GROWING"),44,new Vector2(.05f,.53f),new Vector2(.95f,.95f),TextAnchor.MiddleCenter,Gold,FontStyle.Bold);
            _cityInfo=UiFactory.Label(header,"Progress","",29,new Vector2(.05f,.03f),new Vector2(.95f,.52f),TextAnchor.MiddleCenter,Color.white);
            UiFactory.Button(_cityPanel,"CityBack",T("ZURÜCK ZU LEVELS","BACK TO LEVELS"),Gold,Navy,new Vector2(.08f,.04f),new Vector2(.92f,.15f),ShowHome);
            _cityTabs=new Button[3];
            for(var i=0;i<3;i++)
            {
                var district=i;
                _cityTabs[i]=UiFactory.Button(_cityPanel,"CityDistrict"+i,T("BEZIRK ","DISTRICT ")+(i+1),new Color(.13f,.2f,.3f),Color.white,new Vector2(.04f+i*.31f,.72f),new Vector2(.34f+i*.31f,.80f),()=>{Game.SelectCityDistrict(district);RefreshCityTabs();});
            }
            _cityPanel.gameObject.SetActive(false);
        }
        private void HideArcadeMenus() { if(_cityPanel!=null)_cityPanel.gameObject.SetActive(false);Game.SetCityView(false); }
        private void ShowCity()
        {
            Game.Save();HideMenus();_cityPanel.gameObject.SetActive(true);Game.SetCityView(true);
            _cityInfo.text=StackCampaign.Buildings(Game.Profile)+" / 30 "+T("TÜRME","TOWERS")+"\n"+T("Neue Bezirke: Level 11 und 21","New districts: levels 11 and 21");
            for(var i=0;i<3;i++)_cityTabs[i].interactable=Game.Profile.UnlockedLevel>=i*10+1;
            RefreshCityTabs();
            Refresh();
        }
        private void RefreshCityTabs()
        {
            for(var i=0;i<3;i++)
            {
                var selected=Game.CityDistrict==i;
                _cityTabs[i].GetComponent<Image>().color=selected?Gold:new Color(.045f,.075f,.13f,.98f);
                _cityTabs[i].GetComponentInChildren<Text>().color=selected?Navy:Color.white;
                SetLabel(_cityTabs[i],T("BEZIRK ","DISTRICT ")+(i+1)+(selected?"  ›":""));
            }
        }
        private void RefreshArcadeHome()
        {
            if(_challenge==null)return;
            var today=DateTime.UtcNow.ToString("yyyy-MM-dd");
            SetLabel(_challenge,Game.Profile.ChallengeDate==today && Game.Profile.ChallengeRewarded?T("CHALLENGE ERLEDIGT","CHALLENGE COMPLETE"):T("TAGES-CHALLENGE\n3 STERNE = +75","DAILY CHALLENGE\n3 STARS = +75"));
        }
        private void LayoutArcade(Rect pane)
        {
            if(_powerRow==null)return;
            var landscape=Screen.width*pane.width>Screen.height*pane.height*1.2f;
            Place(_powerRow,pane,StackPresentation.Powers(landscape));
            var info=(RectTransform)_powerInfo.transform.parent;
            Set(info,0,landscape?.87f:.74f,1,1);
            for(var i=0;i<4;i++)
            {
                var button=i<3?_powerButtons[i]:_risk;
                if(landscape)Set((RectTransform)button.transform,0,.65f-i*.215f,1,.85f-i*.215f);
                else Set((RectTransform)button.transform,i*.25f,.03f,i*.25f+.235f,.70f);
                button.GetComponentInChildren<Text>().fontSize=landscape?26:29;
            }
            Place(_cityPanel,pane,new Rect(0,0,1,1));
            Canvas.ForceUpdateCanvases();
            var cityHeight=Mathf.Max(1,_cityPanel.rect.height);
            var tabHeight=UiMetrics.TargetSize(_cityPanel,49)/cityHeight;
            var backHeight=UiMetrics.TargetSize(_cityPanel,54)/cityHeight;
            Set((RectTransform)_cityPanel.Find("CityHeader"),.03f,.85f,.97f,.98f);
            for(var i=0;i<3;i++)Set((RectTransform)_cityTabs[i].transform,.04f+i*.31f,.83f-tabHeight,.34f+i*.31f,.83f);
            Set((RectTransform)_cityPanel.Find("CityBack"),.08f,.045f,.92f,.045f+backHeight);
            var cityTitle=(RectTransform)_cityPanel.Find("CityHeader/Title");
            if(landscape){Set(cityTitle,.04f,.12f,.60f,.90f);Set((RectTransform)_cityInfo.transform,.63f,.06f,.97f,.94f);}
            else{Set(cityTitle,.05f,.54f,.95f,.94f);Set((RectTransform)_cityInfo.transform,.05f,.06f,.95f,.49f);}
        }
        private void UpdateArcadeHud()
        {
            if(_powerRow==null || Game.Run==null)return;
            _powerRow.gameObject.SetActive(!ModalOpen && !Game.Run.Completed && !Game.Run.Failed);
            var powers=Game.Run.Powers;
            var signature=powers.Energy+Mathf.CeilToInt(powers.SlowSeconds)*10+(powers.RepairReady?100:0)+(powers.Risk?200:0)+(int)Game.CurrentKind*1000+Game.Profile.UnlockedLevel*10000+(ModalOpen?1000000:0);
            if(signature==_powerSignature)return;_powerSignature=signature;
            var kind=Game.CurrentKind;
            var label=kind==StackBlockKind.Bonus?T("BONUS: PERFECT +4 COINS","BONUS: PERFECT +4 COINS"):
                kind==StackBlockKind.Fragile?T("BRÜCHIG: ÜBERSTAND KOSTET FLÄCHE","FRAGILE: OVERHANG COSTS AREA"):
                kind==StackBlockKind.Drift?T("DRIFT: TEMPO WECHSELT","DRIFT: SPEED CHANGES"):
                kind==StackBlockKind.Wind?T("WIND: ACHTE AUF DIE BEWEGUNG","WIND: WATCH THE MOVEMENT"):T("PERFECT LÄDT ENERGIE","PERFECT CHARGES ENERGY");
            _powerInfo.text=T("ENERGIE ","ENERGY ")+powers.Energy+" / 6";
            if(kind!=StackBlockKind.Standard && !_tutorialActive)_hint.text=label;
            var names=new[]{T("ZEITLUPE","SLOW TIME"),T("ZENTRIEREN","CENTER"),T("RETTEN","SAVE")};
            for(var i=0;i<3;i++)
            {
                var power=(StackPower)i;var unlocked=StackCampaign.PowerUnlocked(Game.Profile,power);
                SetLabel(_powerButtons[i],names[i]+"\n"+(unlocked?power==StackPower.Slow && powers.SlowSeconds>0?Mathf.CeilToInt(powers.SlowSeconds)+" s":StackPowers.Cost(power)+" ◆":T("LEVEL ","LEVEL ")+(i==0?3:i==1?5:9)));
                _powerButtons[i].interactable=unlocked && !ModalOpen && !Game.Run.Failed && !Game.Run.Completed && powers.Energy>=StackPowers.Cost(power) && !(power==StackPower.Slow && powers.SlowSeconds>0) && !(power==StackPower.Repair && powers.RepairReady);
            }
            SetLabel(_risk,T("RISIKO","RISK")+"\n"+(Game.Profile.UnlockedLevel<6?T("AB LEVEL 6","FROM LEVEL 6"):powers.Risk?T("PERFECT ODER ENDE","PERFECT OR FAIL"):T("2× COINS","2× COINS")));
            _risk.interactable=Game.Profile.UnlockedLevel>=6 && !ModalOpen && !Game.Run.Failed && !Game.Run.Completed;
            _risk.GetComponent<Image>().color=powers.Risk?new Color(.65f,.15f,.1f):new Color(.13f,.2f,.3f);
        }
        public void ArcadeFeedback(string text) => _status.text=text;
    }
}
