using System;
using UnityEngine;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using GoogleMobileAds.Ump.Api;
using Kamilunavo.PerfectDrop.Gameplay;

namespace Kamilunavo.PerfectDrop.Monetization
{
    public sealed class InterstitialAds : MonoBehaviour
    {
        private StackGame _game;
        private MonetizationConfig _config;
        private InterstitialPolicy _policy;
        private InterstitialAd _ad;
        private readonly AdLoadGeneration _loads = new();
        private bool _loading, _focused = true, _applicationPaused, _listenerWasPaused, _opened;
        private double _loadedAt, _retryAt, _nextPrepare;
        public bool IsPresenting { get; private set; }
        private bool Disabled => !Application.isMobilePlatform || QaEnabled;
        private static bool QaEnabled
        {
            get
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                return !string.IsNullOrEmpty(StackSave.QaKey);
#else
                return false;
#endif
            }
        }
        public void Initialize(StackGame game)
        {
            _game = game; _config = MonetizationConfig.Load();
            _policy = new InterstitialPolicy(Time.realtimeSinceStartupAsDouble);
            _focused = Application.isFocused;
            _game.Videos.Changed += Prepare;
            _game.Purchases.Changed += Prepare;
        }
        // Preload only. This method can never present an ad or a consent form.
        public void Prepare()
        {
            if (_game == null || Disabled || IsPresenting || _game.Videos.IsPresenting || _game.Purchases.IsPresenting) return;
            if (CommerceRules.AdsRemoved(_game.Profile) || !ConsentInformation.CanRequestAds()) { Invalidate(); return; }
            if (!_game.Videos.AdsInitialized || _loading || Time.realtimeSinceStartupAsDouble < _retryAt) return;
            if (_ad != null && _ad.CanShowAd() && Time.realtimeSinceStartupAsDouble - _loadedAt < 3500) return;
            Invalidate();
            if (string.IsNullOrWhiteSpace(_config.InterstitialId)) return;
            _loading = true; var generation = _loads.Begin();
            try{InterstitialAd.Load(_config.InterstitialId, new AdRequest(), (ad, error) =>
            {
                if (this == null) { ad?.Destroy(); return; }
                MobileAdsEventExecutor.ExecuteInUpdate(() =>
                {
                    if (this == null) { ad?.Destroy(); return; }
                    if (!_loads.IsCurrent(generation) || CommerceRules.AdsRemoved(_game.Profile) || !ConsentInformation.CanRequestAds()) { ad?.Destroy(); return; }
                    _loading = false;
                    if (error != null || ad == null) { ad?.Destroy(); _retryAt = Time.realtimeSinceStartupAsDouble + 60; return; }
                    _ad = ad; _loadedAt = Time.realtimeSinceStartupAsDouble;
                    ad.OnAdFullScreenContentOpened += () => Main(() =>
                    {
                        if (_ad != ad || !IsPresenting || _opened) return;
                        _opened = true; _policy.Impression(Time.realtimeSinceStartupAsDouble);
                    });
                    ad.OnAdFullScreenContentClosed += () => Main(() => Finish(ad));
                    ad.OnAdFullScreenContentFailed += _ => Main(() => Finish(ad));
                });
            });}catch(Exception){_loads.Invalidate();_loading=false;_retryAt=Time.realtimeSinceStartupAsDouble+60;}
        }
        public void Completed(string runToken, bool tutorial, bool eligible)
        {
            if (Disabled || _game == null || !_policy.RecordCompletion(runToken, tutorial, eligible)) return;
            // This successful result callback is the only presentation boundary.
            TryShow();
        }
        public void RewardedOpened() => _policy?.RewardedOpened(Time.realtimeSinceStartupAsDouble);
        private bool TryShow()
        {
            double now = Time.realtimeSinceStartupAsDouble;
            bool available = _ad != null && _ad.CanShowAd() && now - _loadedAt < 3500 && ConsentInformation.CanRequestAds();
            bool busy = _game.Purchases.IsPresenting || _game.Purchases.Busy || _game.Videos.IsPresenting || IsPresenting;
            bool result = _game.Run.Completed && _game.Hud.ResultOpen;
            if (!_policy.TryReserve(now, CommerceRules.AdsRemoved(_game.Profile), available, busy, _focused && !_applicationPaused, result)) return false;
            var ad = _ad;
            try
            {
                _game.Save(); IsPresenting = true; _opened = false;
                _listenerWasPaused = AudioListener.pause; AudioListener.pause = true;
                _game.Hud.Refresh(); ad.Show(); return true;
            }
            catch (Exception) { Finish(ad); return false; }
        }
        private void Main(Action action) => MobileAdsEventExecutor.ExecuteInUpdate(() => { if (this != null) action(); });
        public void Invalidate()
        {
            if (IsPresenting) return;
            _loads.Invalidate(); _loading = false; _ad?.Destroy(); _ad = null;
        }
        private void Finish(InterstitialAd ad)
        {
            if (_ad != ad) return;
            if (IsPresenting) AudioListener.pause = _listenerWasPaused;
            IsPresenting = false; _policy.CancelReservation(); _ad = null; ad?.Destroy();
            _retryAt = Time.realtimeSinceStartupAsDouble + 2;
            if (_game != null) _game.Hud.Refresh();
        }
        private void Update() { if(Time.realtimeSinceStartupAsDouble<_nextPrepare)return;_nextPrepare=Time.realtimeSinceStartupAsDouble+1;Prepare(); }
        private void OnApplicationFocus(bool focus) { _focused = focus; }
        private void OnApplicationPause(bool paused) { _applicationPaused = paused; }
        private void OnDestroy()
        {
            if (_game != null) { _game.Videos.Changed -= Prepare; _game.Purchases.Changed -= Prepare; }
            _loads.Invalidate(); if (IsPresenting) AudioListener.pause = _listenerWasPaused;
            IsPresenting = false; _ad?.Destroy(); _ad = null;
        }
    }
}
