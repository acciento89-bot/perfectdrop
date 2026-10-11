using System;
using UnityEngine;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using GoogleMobileAds.Ump.Api;
using Kamilunavo.PerfectDrop.Gameplay;
namespace Kamilunavo.PerfectDrop.Monetization
{
    public sealed class RewardedVideos:MonoBehaviour
    {
        private StackGame _game;
        private MonetizationConfig _config;
        private RewardedAd _ad;
        private bool _prepared,_initialized,_loading;
        private bool _focused=true,_applicationPaused;
        private readonly AdLoadGeneration _loads=new();
        private double _loadedAt,_retryAt;
        private string _session="";
        private bool _rewarded;
        public bool IsPresenting {get;private set;}
        public bool AdsInitialized=>_initialized;
        public bool PrivacyRequired=>_prepared && ConsentInformation.PrivacyOptionsRequirementStatus==PrivacyOptionsRequirementStatus.Required;
        public event Action Changed;
        public string Status {get;private set;}="";
        public bool CanWatch=>Application.isMobilePlatform && !IsPresenting && !(_game?.Interstitials?.IsPresenting??false) && !(_game?.Purchases?.IsPresenting??false) && _ad!=null && _ad.CanShowAd() && Time.realtimeSinceStartupAsDouble-_loadedAt<3500 && ConsentInformation.CanRequestAds() && RewardRules.CanClaim(_game.Profile,DateTime.UtcNow);
        private static string T(string de,string en)=>UI.StackHud.T(de,en);
        private void Main(Action action)=>MobileAdsEventExecutor.ExecuteInUpdate(()=>{if(this!=null)action();});
        public void Initialize(StackGame game)
        {
            _game=game;_config=MonetizationConfig.Load();
            if(Application.isPlaying)_focused=Application.isFocused;
            Status=T("Video gerade nicht verfügbar","Video currently unavailable");
        }
        public void Prepare()
        {
            if(_game==null||_game.Hud==null||!(_game.Hud.ShopOpen||_game.Hud.HomeOpen||_game.Hud.ResultOpen)||!_game.Profile.TutorialComplete||_game.Hud.TutorialActive||!_focused||_applicationPaused)return;
            if(CommerceRules.AdsRemoved(_game.Profile)&&!_game.Hud.ShopOpen)return;
            if(!Application.isMobilePlatform || Time.realtimeSinceStartupAsDouble<_retryAt)return;
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if(!string.IsNullOrEmpty(StackSave.QaKey))return;
#endif
            if(_prepared){if(_initialized)Load();return;}
            _prepared=true;SetStatus(T("Video wird vorbereitet …","Preparing video …"));
            try{ConsentInformation.Update(new ConsentRequestParameters(),error=>Main(()=>
            {
                if(error!=null){_prepared=false;_retryAt=Time.realtimeSinceStartupAsDouble+60;AfterConsent();return;}
                if(!TryPresentConsent(()=>ConsentForm.LoadAndShowConsentFormIfRequired(formError=>Main(()=>{IsPresenting=false;AfterConsent();})))){_prepared=false;AfterConsent();}
            }));}catch(Exception){_prepared=false;_retryAt=Time.realtimeSinceStartupAsDouble+60;SetStatus(T("Video gerade nicht verfügbar","Video currently unavailable"));}
        }
        private bool TryPresentConsent(Action show)
        {
            // The asynchronous update may finish after the player has left the shop.
            if(IsPresenting || !_focused || _applicationPaused || _game==null || !_game.Profile.TutorialComplete || _game.Hud==null || _game.Hud.TutorialActive || !(_game.Hud.ShopOpen||_game.Hud.HomeOpen||_game.Hud.ResultOpen) || !_game.Hud.ModalOpen || _game.Purchases?.IsPresenting==true || _game.Interstitials?.IsPresenting==true)return false;
            _game.Save();IsPresenting=true;Changed?.Invoke();
            try{show();return true;}catch(Exception){IsPresenting=false;_prepared=false;Changed?.Invoke();return false;}
        }
        private void AfterConsent()
        {
            Changed?.Invoke();
            if(!ConsentInformation.CanRequestAds()){SetStatus(T("Video nicht verfügbar – Datenschutzeinstellungen prüfen","Video unavailable – check privacy settings"));return;}
            if(_initialized){Load();return;}
            try{MobileAds.Initialize(status=>Main(()=>
            {
                if(status==null){SetStatus(T("Video gerade nicht verfügbar","Video currently unavailable"));return;}
                _initialized=true;Changed?.Invoke();Load();
            }));}catch(Exception){_prepared=false;_retryAt=Time.realtimeSinceStartupAsDouble+60;SetStatus(T("Video gerade nicht verfügbar","Video currently unavailable"));}
        }
        private void Load()
        {
            if(!_initialized || _loading || IsPresenting || !ConsentInformation.CanRequestAds() || Time.realtimeSinceStartupAsDouble<_retryAt)return;
            if(_ad!=null && _ad.CanShowAd() && Time.realtimeSinceStartupAsDouble-_loadedAt<3500)return;
            _ad?.Destroy();_ad=null;
            var id=_config.RewardedId;
            if(string.IsNullOrWhiteSpace(id)){SetStatus(T("Video gerade nicht verfügbar","Video currently unavailable"));return;}
            _loading=true;var generation=_loads.Begin();
            try{RewardedAd.Load(id,new AdRequest(),(ad,error)=>
            {
                if(this==null){ad?.Destroy();return;}
                MobileAdsEventExecutor.ExecuteInUpdate(()=>
                {
                    if(this==null){ad?.Destroy();return;}
                    if(!_loads.IsCurrent(generation) || !ConsentInformation.CanRequestAds()){ad?.Destroy();return;}
                    _loading=false;
                    if(error!=null || ad==null){ad?.Destroy();_retryAt=Time.realtimeSinceStartupAsDouble+60;SetStatus(T("Kein Video verfügbar – später erneut versuchen","No video available – try later"));return;}
                    _ad=ad;_loadedAt=Time.realtimeSinceStartupAsDouble;
                    bool opened=false;
                    ad.OnAdFullScreenContentOpened+=()=>Main(()=>{if(_ad!=ad||!IsPresenting||opened)return;opened=true;_game.Interstitials?.RewardedOpened();});
                    ad.OnAdFullScreenContentClosed+=()=>Main(()=>Finish(ad,false));
                    ad.OnAdFullScreenContentFailed+=failure=>Main(()=>Finish(ad,true));
                    SetStatus(T("Freiwilliges Video: +50 Coins","Optional video: +50 Coins"));
                });
            });}catch(Exception){_loads.Invalidate();_loading=false;_retryAt=Time.realtimeSinceStartupAsDouble+60;SetStatus(T("Video gerade nicht verfügbar","Video currently unavailable"));}
        }
        public void Watch()
        {
            if(!CanWatch || !_game.Hud.ShopOpen || !_focused || _applicationPaused)return;
            _game.Save();IsPresenting=true;_session=Guid.NewGuid().ToString("N");_rewarded=false;
            var session=_session;var ad=_ad;Changed?.Invoke();
            try{ad.Show(reward=>Main(()=>
            {
                // Only this specific SDK completion callback can award this view, once.
                if(_session!=session || _rewarded)return;
                try
                {
                    if(RewardRules.Fulfill(_game.Profile,session,DateTime.UtcNow,StackSave.Save))
                    {_rewarded=true;SetStatus(T("+50 Coins erhalten","Received +50 Coins"));_game.Hud.Refresh();}
                }
                catch(Exception){SetStatus(T("Belohnung konnte nicht gespeichert werden","Reward could not be saved"));}
            }));}
            catch(Exception){Finish(ad,true);}
        }
        private void Finish(RewardedAd ad,bool failed)
        {
            if(_ad!=ad)return;
            IsPresenting=false;
            SetStatus(failed?T("Video konnte nicht geöffnet werden","Video could not open"):_rewarded?T("+50 Coins erhalten","Received +50 Coins"):T("Video beendet – keine Prämie erhalten","Video ended – no reward received"));
            // Keep the session until the next Watch; delayed SDK reward callbacks remain valid.
            _ad=null;ad.Destroy();_retryAt=Time.realtimeSinceStartupAsDouble+2;
        }
        public void ShowPrivacy()
        {
            if(!PrivacyRequired || IsPresenting || !_focused || _applicationPaused || !_game.Hud.ShopOpen || _game.Purchases?.IsPresenting==true || _game.Interstitials?.IsPresenting==true)return;
            _game.Save();IsPresenting=true;_game.Interstitials?.Invalidate();_loads.Invalidate();_loading=false;_retryAt=0;_ad?.Destroy();_ad=null;Changed?.Invoke();
            try{ConsentForm.ShowPrivacyOptionsForm(error=>Main(()=>{IsPresenting=false;AfterConsent();}));}catch(Exception){IsPresenting=false;_prepared=false;Changed?.Invoke();}
        }
        private void SetStatus(string status){Status=status;Changed?.Invoke();}
        private void OnApplicationFocus(bool focus){_focused=focus;}
        private void OnApplicationPause(bool paused){_applicationPaused=paused;}
        private void OnDestroy(){_loads.Invalidate();_ad?.Destroy();_ad=null;}
    }
}
