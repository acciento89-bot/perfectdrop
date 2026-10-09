#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using Kamilunavo.PerfectDrop.Gameplay;
using Kamilunavo.PerfectDrop.UI;
namespace Kamilunavo.PerfectDrop.QA
{
    public sealed class RuntimeSmoke : MonoBehaviour
    {
        private string _output;
        private bool _arcade,_uiOnly,_cityOnly,_presentation;
        private readonly List<string> _errors=new();
        private void Awake()=>Application.logMessageReceived+=RecordError;
        private void OnDestroy()=>Application.logMessageReceived-=RecordError;
        private void RecordError(string message,string trace,LogType type){if(type==LogType.Error || type==LogType.Exception || type==LogType.Assert)_errors.Add(message);}
        private static bool? _requestedLandscape;
        private static bool RotateNative => Application.isMobilePlatform && Environment.GetEnvironmentVariable("PERFECTDROP_QA_ROTATE") == "1";
        private StackGame _game;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Bootstrap()
        {
            var args = QaLaunch.Arguments();
            for (var i=0;i<args.Length-1;i++) if (args[i]=="-qaSmoke" || args[i]=="-qaArcade" || args[i]=="-qaArcadeUI" || args[i]=="-qaCity" || args[i]=="-qaPresentation")
            {
                StackSave.QaKey = args[i]!="-qaSmoke"?"perfectdrop.arcade.qa.v1":"perfectdrop.stack.qa.v1";
                PlayerPrefs.DeleteKey(StackSave.QaKey);
                var root = new GameObject("StackRuntimeSmoke"); DontDestroyOnLoad(root);
                var smoke=root.AddComponent<RuntimeSmoke>();smoke._output=args[i+1];smoke._arcade=args[i]!="-qaSmoke";smoke._uiOnly=args[i]=="-qaArcadeUI";smoke._cityOnly=args[i]=="-qaCity";smoke._presentation=args[i]=="-qaPresentation"; break;
            }
        }
        private IEnumerator Start()
        {
            Directory.CreateDirectory(_output);
            File.WriteAllText(Path.Combine(_output,"result.txt"),"RUNNING\n");
            Time.captureDeltaTime = 1f / 60f; // Functional input timing; this does not measure real frame rate.
            var run = _presentation?RunPresentation():_cityOnly?RunCity():_arcade?RunArcade():Run();
            var pending = new Stack<IEnumerator>();
            pending.Push(run);
            while (pending.Count > 0)
            {
                object next;
                try
                {
                    var current = pending.Peek();
                    if (!current.MoveNext()) { pending.Pop(); continue; }
                    next=current.Current;
                }
                catch (Exception error)
                {
                    Debug.LogException(error); File.WriteAllText(Path.Combine(_output,"result.txt"),"FAIL\n"+error);
                    ScreenCapture.CaptureScreenshot(QaLaunch.ScreenshotPath(_output,"failure")); yield break;
                }
                if (next is IEnumerator nested && !(next is CustomYieldInstruction)) pending.Push(nested);
                else yield return next;
            }
            if(_errors.Count>0){File.WriteAllText(Path.Combine(_output,"result.txt"),"FAIL: runtime logged errors\n"+string.Join("\n",_errors));yield break;}
            File.WriteAllText(Path.Combine(_output,"result.txt"),_presentation?"PASS: presentation orientations, actual tutorial placements, replay/save integrity, city and galleries.\n":_cityOnly?"PASS: city framing at portrait/landscape and opaque map UI.\n":_uiOnly?"PASS: campaign/home-settings/modal-visibility/city-view/portrait-landscape/style UI regression.\n":_arcade?"PASS: campaign map/clear/next, powers, special blocks, risk failure/reward, city, daily challenge, styles, beyond-30 endless and bounded geometry.\n":"PASS: actual moving-block/drop-button sequence through 30; cut, miss, retry, settings pause, resize/progress, daily idempotence and scene reload resume.\n");
            if (Application.isMobilePlatform) File.WriteAllText(Path.Combine(_output,"result.txt"),
                "PASS: native current pose; game callbacks and visible-button raycasts. " +
                (_presentation ? "Presentation/tutorial/frustum." : _cityOnly ? "City framing." : _uiOnly ? "Campaign/settings/city/styles." : _arcade ? "Campaign, powers, special blocks, risk, city, daily, styles, 66-layer endless/reload/retry." : "Stack/cut/miss/retry/save.") +
                (RotateNative ? "\nNative orientation requests exercised; OS touch, fold poses and native performance not verified.\n" : "\nDesktop resize requests skipped; OS touch, other device poses and native performance not verified.\n") +
                $"Final framebuffer: {Screen.width}x{Screen.height}; safe area: {Screen.safeArea}.\n");
            Debug.Log("[PerfectDrop][QA] Stack runtime matrix passed.");
        }
        private IEnumerator RunPresentation()
        {
            yield return new WaitForSecondsRealtime(1);
            _game=FindFirstObjectByType<StackGame>();Require(_game!=null,"Game did not boot.");
            yield return Capture("gallery-home-portrait");
            FindButton("Learn").onClick.Invoke();Require(_game.Hud.TutorialActive && _game.Run.Count==0,"Guide did not start a real fresh run.");
            yield return Capture("guide-overlap");
            for(var step=0;step<3;step++)
            {
                var deadline=Time.realtimeSinceStartup+12;
                while(Mathf.Abs(_game.MovingOffset)>.04f && Time.realtimeSinceStartup<deadline)yield return null;
                Require(Mathf.Abs(_game.MovingOffset)<=.04f,"Perfect tutorial timing deadline.");
                FindButton("Drop").onClick.Invoke();yield return new WaitForSecondsRealtime(.1f);
                Require(_game.Run.Count==step+1,"Actual tutorial drop did not place.");CheckFraming();yield return Capture("guide-drop-"+(step+1));
            }
            Require(_game.Profile.TutorialComplete && !_game.Hud.TutorialActive,"Guide did not complete after real placements.");
            var wallet=_game.Profile.Coins;var count=_game.Run.Count;var owned=_game.Profile.OwnedStyles;
            _game.Hud.ShowHome();FindButton("Learn").onClick.Invoke();Require(_game.Run.Count==count && _game.Profile.Coins==wallet,"Guide replay reset or credited the live run.");
            FindButton("SkipGuide").onClick.Invoke();Require(_game.Run.Count==count && StackSave.Load().Coins==wallet && StackSave.Load().OwnedStyles==owned,"Skip changed persisted wallet/ownership/run.");
            _game.Profile.UnlockedLevel=21;_game.Profile.Coins=357;_game.Profile.OwnedStyles=255;
            for(var i=0;i<5;i++)_game.Profile.LevelStars[i]=3;
            _game.StartLevel(21); // Rebuild earned city through ordinary completion later.
            foreach(var pose in new[]{new Vector2Int(430,932),new Vector2Int(932,430)})
            {
                SetReviewResolution(pose.x,pose.y,false);yield return new WaitForSecondsRealtime(.8f);
                _game.Hud.ShowHome();yield return Capture("gallery-home-"+pose.x);
                GameObject.Find("LevelGalleryScroll").GetComponent<ScrollRect>().verticalNormalizedPosition=0;yield return new WaitForSecondsRealtime(.2f);yield return Capture("gallery-last-levels-"+pose.x);
                FindButton("Styles").onClick.Invoke();yield return Capture("gallery-styles-"+pose.x);
                FindButton("ExtraDesigns").onClick.Invoke();yield return Capture("gallery-shop-"+pose.x);
                var scroll=GameObject.Find("ShopScroll").GetComponent<ScrollRect>();scroll.verticalNormalizedPosition=0;
                yield return new WaitForSecondsRealtime(.2f);yield return Capture("gallery-shop-bottom-"+pose.x);
                _game.Hud.ShowHome();FindButton("Continue").onClick.Invoke();
                yield return new WaitForSecondsRealtime(.3f);CheckFraming();yield return Capture("stack-clear-space-"+pose.x);
                // Sample both ends of the real moving slab, not only center timing.
                for(var frame=0;frame<160;frame++){CheckFraming();yield return null;}
                var yaw=typeof(StackGame).GetField("_yaw",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic);
                foreach(var angle in new[]{-160f,145f,-35f}){yaw.SetValue(_game,angle);yield return null;CheckFraming();}
                _game.Hud.OpenSettings();yield return Capture("settings-"+pose.x);_game.Hud.Close();
            }
            _game.StartLevel(6);
            for(var block=0;block<_game.Run.Target;block++)
            {
                var deadline=Time.realtimeSinceStartup+12;while(Mathf.Abs(_game.MovingOffset)>.04f && Time.realtimeSinceStartup<deadline)yield return null;
                FindButton("Drop").onClick.Invoke();yield return new WaitForSecondsRealtime(.1f);
            }
            _game.Hud.ShowHome();FindButton("City").onClick.Invoke();FindButton("CityDistrict0").onClick.Invoke();yield return new WaitForSecondsRealtime(.5f);yield return Capture("district-architectural-landscape");
            SetReviewResolution(430,932,false);yield return new WaitForSecondsRealtime(.8f);yield return Capture("district-architectural-portrait");
        }
        private IEnumerator Run()
        {
            yield return new WaitForSecondsRealtime(1);
            _game=FindFirstObjectByType<StackGame>();
            Require(_game!=null,"Stack game did not boot.");
            _game.Profile.TutorialComplete=true;_game.Profile.UnlockedLevel=30;
            _game.StartLevel(30);
            var initialWidth=_game.Level.Width;
            var drop=FindButton("Drop");
            yield return new WaitForSeconds(.2f);
            _game.Hud.OpenSettings();
            var paused=_game.MovingOffset;
            yield return new WaitForSeconds(.2f);
            Require(Mathf.Abs(_game.MovingOffset-paused)<.00001f,"Settings did not pause moving block.");
            var beforeDaily = _game.Profile.Coins;
            FindButton("Daily").onClick.Invoke();
            Require(_game.Profile.Coins == beforeDaily+25,"Daily did not award 25 coins.");
            FindButton("Daily").onClick.Invoke();
            Require(_game.Profile.Coins == beforeDaily+25,"Daily rewarded twice on the same day.");
            yield return Capture("settings"); _game.Hud.Close();
            for (var floor=1;floor<=30;floor++)
            {
                var target=floor==1?.60f:0f;
                var deadline=Time.realtimeSinceStartup+30;
                while (Mathf.Abs(_game.MovingOffset-target)>.07f && Time.realtimeSinceStartup<deadline)
                { CheckFraming(); yield return null; }
                Require(Mathf.Abs(_game.MovingOffset-target)<=.07f,$"Moving block never reached placement window: offset={_game.MovingOffset:F3}, focus={Application.isFocused}, modal={_game.Hud.ModalOpen}, time={Time.time:F2}, realtime={Time.realtimeSinceStartup:F2}.");
                drop.onClick.Invoke();
                Require(_game.Run.Layers.Count==floor,"Tap did not place layer "+floor);
                if (floor==1)
                {
                    Require(Mathf.Abs(_game.Run.Top.Size.x-(initialWidth-.6f))<.071f,"Overhang did not shrink the first block.");
                    Require(GameObject.Find("Overhang")!=null,"Cut piece was not rendered.");
                }
                else Require(_game.Run.Streak==floor-1,"Perfect streak did not advance.");
                Require(StackSave.Load().Best>=floor,"Best was not saved.");
                Debug.Log("[PerfectDrop][QA] Real tap placement "+floor+" passed.");
                if (floor==5)
                {
                    yield return Capture("portrait");
                    SetReviewResolution(800,600,false); yield return null; CheckFraming(); yield return new WaitForSecondsRealtime(.5f); yield return Capture("landscape");
                    SetReviewResolution(956,440,false); yield return new WaitForSecondsRealtime(.5f); yield return Capture("iphone16-pro-max-landscape");
                    Require(_game.Run.Layers.Count==5,"Resize reset progress.");
                    Kamilunavo.PerfectDrop.UI.ReservedRegionProvider.QaDivision = new Rect(.48f,0,.04f,1);
                    yield return new WaitForSecondsRealtime(.5f); yield return Capture("division-synthetic");
                    CheckFraming();
                    Kamilunavo.PerfectDrop.UI.ReservedRegionProvider.QaDivision = null;
                    SetReviewResolution(600,800,false); yield return null; CheckFraming(); yield return new WaitForSecondsRealtime(.5f); yield return Capture("wide-portrait");
                    Require(_game.Run.Layers.Count==5,"Second resize reset progress.");
                    SetReviewResolution(540,960,false); yield return null; CheckFraming(); yield return new WaitForSecondsRealtime(.5f);
                    var coins = _game.Profile.Coins;
                    _game.Save();
                    yield return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(0);
                    yield return new WaitForSecondsRealtime(.5f);
                    _game = FindFirstObjectByType<StackGame>(); drop = FindButton("Drop");
                    Require(_game.Run.Layers.Count == 5 && _game.Run.Streak == 4 && _game.Profile.Coins == coins,"Scene reload did not restore the saved tower/rewards.");
                    yield return Capture("resumed");
                }
                yield return new WaitForSeconds(.10f);
            }
            Require(_game.Run.Completed && _game.Hud.ModalOpen,"Exactly 30 blocks did not complete.");
            yield return Capture("complete");
            FindButton("Menu").onClick.Invoke();
            Require(FindButton("Retry").gameObject.activeInHierarchy,"Menu hid the terminal retry action.");
            drop.onClick.Invoke();
            Require(_game.Run.Layers.Count==30,"Terminal input added layer 31.");
            FindButton("Retry").onClick.Invoke();
            Require(_game.Run.Layers.Count==0 && !_game.Hud.ModalOpen,"Retry did not immediately restart.");
            yield return new WaitForSeconds(.1f);
            drop.onClick.Invoke();
            Require(_game.Run.Failed && _game.Hud.ModalOpen,"No-overlap tap did not end the run.");
            yield return Capture("miss");
        }
        private IEnumerator RunCity()
        {
            yield return new WaitForSecondsRealtime(1);_game=FindFirstObjectByType<StackGame>();
            _game.Profile.UnlockedLevel=21;_game.Profile.LevelStars[0]=_game.Profile.LevelStars[5]=3;_game.Save();
            yield return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(0);yield return new WaitForSecondsRealtime(.5f);
            _game=FindFirstObjectByType<StackGame>();yield return Capture("level-map");FindButton("City").onClick.Invoke();FindButton("CityDistrict0").onClick.Invoke();
            yield return new WaitForSecondsRealtime(.5f);yield return Capture("city-portrait");
            SetReviewResolution(800,600,false);yield return new WaitForSecondsRealtime(.5f);yield return Capture("city-landscape");
            var building=GameObject.Find("CityTower1");var bounds=new Bounds(building.transform.position,Vector3.zero);
            foreach(var renderer in building.GetComponentsInChildren<Renderer>())bounds.Encapsulate(renderer.bounds);
            for(var i=0;i<8;i++)
            {
                var corner=bounds.center+Vector3.Scale(bounds.extents,new Vector3((i&1)==0?-1:1,(i&2)==0?-1:1,(i&4)==0?-1:1));
                var point=Camera.main.WorldToViewportPoint(corner);Require(point.y>.16f && point.y<.70f,"Owned building overlapped city navigation.");
            }
        }
        private IEnumerator RunArcade()
        {
            yield return new WaitForSecondsRealtime(1);
            _game=FindFirstObjectByType<StackGame>();
            Require(GameObject.Find("LevelMap")!=null,"Campaign did not open its map.");
            Require(FindButton("LevelSlot0").interactable && !FindButton("LevelSlot1").interactable,"Fresh level locks incorrect.");
            yield return Capture("level-map");
            SetReviewResolution(800,600,false);yield return new WaitForSecondsRealtime(.5f);yield return Capture("map-landscape");
            SetReviewResolution(540,960,false);yield return new WaitForSecondsRealtime(.5f);
            FindButton("LevelSlot0").onClick.Invoke();
            for(var block=1;block<=6;block++)
            {
                var until=Time.realtimeSinceStartup+30;
                while(Mathf.Abs(_game.MovingOffset)>.06f && Time.realtimeSinceStartup<until)yield return null;
                Require(Mathf.Abs(_game.MovingOffset)<=.06f,"Tutorial placement window missed.");
                FindButton("Drop").onClick.Invoke();
                Require(_game.Run.Count==block,"Tutorial drop failed.");yield return new WaitForSeconds(.1f);
                if(block==1 && !GamePreferences.ReducedMotion)
                {
                    var burst=GameObject.Find("LandingBurst_Good");Require(burst!=null,"Perfect landing spark feedback missing.");
                    foreach(Transform spark in burst.transform)Require(spark.localScale.x<=.10f && spark.localScale.z<=.31f,"Landing spark lost its authored scale.");
                }
                if(block==3){yield return new WaitForSecondsRealtime(.6f);yield return Capture("tutorial-stack");}
            }
            Require(_game.Run.Completed && _game.Profile.UnlockedLevel==2 && _game.LastStars==3 && StackCampaign.Buildings(_game.Profile)==1,"Tutorial did not reward/unlock/build city.");
            yield return Capture("first-clear");
            FindButton("Next").onClick.Invoke();Require(_game.Level.Id==2 && _game.Run.Count==0,"Next level failed.");
            // Seed advanced unlocks only in this isolated QA profile to exercise later systems efficiently.
            _game.Profile.UnlockedLevel=21;_game.GoHome();FindButton("LevelSlot5").onClick.Invoke();
            for(var block=1;block<=_game.Run.Target;block++)
            {
                if(block==4)
                {
                    _game.GoHome();FindButton("Chapter2").onClick.Invoke();
                    Require(Kamilunavo.PerfectDrop.Visuals.WorldArt.ActiveChapter==2,"Chapter preview backdrop did not change.");
                    FindButton("Continue").onClick.Invoke();
                    Require(Kamilunavo.PerfectDrop.Visuals.WorldArt.ActiveChapter==(_game.Level.Id-1)/10 && _game.Run.Count==3,"Continue did not restore the active run's chapter.");
                    Require(_game.Run.Powers.Energy==3,"Combo energy did not charge.");
                    FindButton("PowerCenter").onClick.Invoke();
                    Require(_game.Run.Count==4,"Center power did not place exactly one block.");
                    yield return Capture("center-power");yield return new WaitForSeconds(.1f);continue;
                }
                if(block==6)
                {
                    FindButton("PowerSlow").onClick.Invoke();
                    Require(_game.Run.Powers.SlowSeconds>0,"Slow power did not activate.");
                    yield return Capture("slow-power");
                    _game.Hud.OpenSettings();var duration=_game.Run.Powers.SlowSeconds;
                    yield return new WaitForSecondsRealtime(.2f);Require(Mathf.Abs(_game.Run.Powers.SlowSeconds-duration)<.001f,"Settings consumed slow-time duration.");
                    _game.Hud.Close();_game.Save();yield return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(0);yield return new WaitForSecondsRealtime(.5f);
                    _game=FindFirstObjectByType<StackGame>();Require(_game.Run.Count==5 && _game.Run.Powers.SlowSeconds>0,"Active power did not survive reload.");
                }
                var until=Time.realtimeSinceStartup+30;
                while(Mathf.Abs(_game.MovingOffset)>.06f && Time.realtimeSinceStartup<until)yield return null;
                Require(Mathf.Abs(_game.MovingOffset)<=.06f,"Advanced placement window missed.");
                if(block==8)FindButton("Risk").onClick.Invoke();
                var coins=_game.Run.EarnedCoins;FindButton("Drop").onClick.Invoke();
                Require(_game.Run.Count==block,"Advanced drop failed.");
                if(block==8)Require(_game.Run.EarnedCoins-coins>=16,"Risk Perfect reward failed.");
                yield return new WaitForSeconds(.1f);
            }
            Require(_game.Run.Completed,"Advanced level failed to complete.");
            FindButton("LevelMapAction").onClick.Invoke();FindButton("HomeSettings").onClick.Invoke();
            Require(_game.Hud.ModalOpen && GameObject.Find("ContextPanel")!=null,"Map settings unavailable after clear.");
            FindButton("Close").onClick.Invoke();Require(GameObject.Find("LevelMap")!=null,"Map settings did not return to map.");
            FindButton("City").onClick.Invoke();
            yield return new WaitForSecondsRealtime(.5f);Require(_game.Hud.CityOpen,"City did not open.");
            FindButton("CityDistrict0").onClick.Invoke();yield return new WaitForSecondsRealtime(.5f);yield return Capture("owned-city");
            Require(GameObject.Find("Skyline")==null,"Legacy skyline occluded city view.");
            SetReviewResolution(800,600,false);yield return new WaitForSecondsRealtime(.5f);yield return Capture("city-landscape");
            SetReviewResolution(540,960,false);yield return new WaitForSecondsRealtime(.5f);
            Require(GameObject.Find("CityTower1")!=null,"Completed tower missing in city view.");
            FindButton("CityBack").onClick.Invoke();FindButton("Styles").onClick.Invoke();
            var wallet=_game.Profile.Coins;FindButton("Style1").onClick.Invoke();
            Require(_game.Profile.Style==1 && _game.Profile.Coins==wallet-75,"Style purchase failed.");
            FindButton("Style1").onClick.Invoke();Require(_game.Profile.Coins==wallet-75,"Style charged twice.");
            yield return Capture("styles");
            FindButton("ExtraDesigns").onClick.Invoke();yield return new WaitForSecondsRealtime(.5f);
            Require(_game.Hud.ShopOpen && _game.Hud.ModalOpen,"Shop does not pause gameplay.");
            var shopCoins=_game.Profile.Coins;
            Require(!_game.Purchases.CanBuy(Kamilunavo.PerfectDrop.Monetization.CommerceRules.Starter) && !_game.Videos.CanWatch,"Offline QA exposes a fake native purchase/video.");
            Require(!FindButton("BuyStarter").interactable && !FindButton("OptionalVideo").interactable,"Unavailable purchase/video buttons enabled.");
            Require(!_game.SelectStyle(4) && _game.Profile.Coins==shopCoins,"Locked premium style changed wallet.");
            yield return Capture("shop-offline-portrait");
            SetReviewResolution(956,440,false);yield return new WaitForSecondsRealtime(.5f);yield return Capture("shop-offline-landscape");
            var scroll=GameObject.Find("ShopScroll").GetComponent<ScrollRect>();scroll.verticalNormalizedPosition=0;
            yield return new WaitForSecondsRealtime(.5f);yield return Capture("shop-bottom-landscape");
            Kamilunavo.PerfectDrop.Monetization.CommerceRules.RestoreEntitlement(_game.Profile,Kamilunavo.PerfectDrop.Monetization.CommerceRules.Collection);
            _game.Hud.RefreshCommerce();Require(_game.SelectStyle(6) && _game.Profile.Coins==shopCoins,"Owned style selection changed coins.");
            _game.Save();Require(StackSave.Load().Style==6,"Premium style not saved.");
            Kamilunavo.PerfectDrop.Monetization.CommerceRules.ReconcileEntitlements(_game.Profile,new string[0]);
            _game.RefreshProfileStyle();
            Require(_game.Profile.Style==0,"Revoked premium style remains selected.");
            // This run is completed, so no moving block exists. Home/shop hides
            // the placed tower; inspect its renderers without activating it.
            var rims=Array.FindAll(FindObjectsByType<Renderer>(FindObjectsInactive.Include,FindObjectsSortMode.None),
                candidate=>candidate.name=="GoldBand" && candidate.transform.parent.name.StartsWith("Placed_"));
            Require(rims.Length>0,"Placed block rims missing from hidden tower.");
            foreach(var rim in rims)
            {
                var paint=new MaterialPropertyBlock();rim.GetPropertyBlock(paint);
                var restored=paint.GetColor("_Color");var expected=new Color(1f,.55f,.06f);
                Require(Mathf.Abs(restored.r-expected.r)<.01f && Mathf.Abs(restored.g-expected.g)<.01f && Mathf.Abs(restored.b-expected.b)<.01f,"Revoked style still displayed on placed tower.");
            }
            FindButton("ShopBack").onClick.Invoke();SetReviewResolution(540,960,false);yield return new WaitForSecondsRealtime(.5f);
            FindButton("Back").onClick.Invoke();if(_uiOnly)yield break;FindButton("Challenge").onClick.Invoke();
            for(var block=1;block<=_game.Run.Target;block++)
            {
                var until=Time.realtimeSinceStartup+30;
                while(Mathf.Abs(_game.MovingOffset)>.06f && Time.realtimeSinceStartup<until)yield return null;
                Require(Mathf.Abs(_game.MovingOffset)<=.06f,"Daily placement window missed.");
                FindButton("Drop").onClick.Invoke();Require(_game.Run.Count==block,"Daily drop failed.");yield return new WaitForSeconds(.1f);
            }
            Require(_game.Profile.ChallengeRewarded && _game.LastBonus==75,"Daily three-star reward failed.");yield return Capture("daily-clear");
            FindButton("LevelMapAction").onClick.Invoke();_game.StartLevel(9);
            for(var block=1;block<=4;block++)
            {
                var until=Time.realtimeSinceStartup+30;
                while(Mathf.Abs(_game.MovingOffset)>.06f && Time.realtimeSinceStartup<until)yield return null;
                Require(Mathf.Abs(_game.MovingOffset)<=.06f,"Repair charge placement missed.");FindButton("Drop").onClick.Invoke();
                Require(_game.Run.Count==block,"Repair charge drop failed.");yield return new WaitForSeconds(.1f);
            }
            var area=_game.Run.Top.Size;FindButton("PowerRepair").onClick.Invoke();Require(_game.Run.Powers.RepairReady,"Repair did not arm.");
            FindButton("Risk").onClick.Invoke();FindButton("Drop").onClick.Invoke();
            Require(_game.Run.Count==5 && !_game.Run.Failed && _game.Run.Top.Size==area && !_game.Run.Powers.RepairReady,"Repair did not save actual risky miss.");
            yield return Capture("repair-save");_game.GoHome();FindButton("Endless").onClick.Invoke();
            for(var block=1;block<=66;block++)
            {
                var until=Time.realtimeSinceStartup+30;
                while(Mathf.Abs(_game.MovingOffset)>.06f && Time.realtimeSinceStartup<until)yield return null;
                Require(Mathf.Abs(_game.MovingOffset)<=.06f,"Endless placement window missed.");
                FindButton("Drop").onClick.Invoke();Require(_game.Run.Count==block && !_game.Run.Completed,"Endless count/completion incorrect.");
                yield return new WaitForSeconds(.1f);
            }
            Require(_game.Run.Layers.Count==64,"Endless retained unbounded geometry.");yield return Capture("endless-66");
            _game.Save();yield return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(0);yield return new WaitForSecondsRealtime(.5f);
            _game=FindFirstObjectByType<StackGame>();Require(_game.Run.Endless && _game.Run.Count==66 && _game.Run.Layers.Count==64,"Endless scene resume failed.");
            _game.NewRun();Require(Mathf.Abs(_game.Run.Top.Size.x-3.6f)<.0001f,"Endless retry narrowed after reload.");
            Require(GameObject.Find("Status").GetComponent<Text>().text==Kamilunavo.PerfectDrop.UI.StackHud.T("ENDLOS","ENDLESS"),"Resume/reset mode text incorrect.");
            _game.StartLevel(6);FindButton("Risk").onClick.Invoke();yield return new WaitForSeconds(.1f);FindButton("Drop").onClick.Invoke();
            Require(_game.Run.Failed,"Risk bad placement did not fail.");yield return Capture("risk-fail");
            FindButton("Retry").onClick.Invoke();Require(_game.Run.Count==0 && !_game.Run.Failed,"Arcade retry failed.");
        }
        private void CheckFraming()
        {
            var block = GameObject.Find("MovingBlock"); if (block == null) return;
            var bounds = block.transform.Find("MetalDeck").GetComponent<Renderer>().bounds;
            var pane = _game.Hud.WorldPane;
            for (var i=0;i<8;i++)
            {
                var corner = bounds.center + Vector3.Scale(bounds.extents,new Vector3((i&1)==0?-1:1,(i&2)==0?-1:1,(i&4)==0?-1:1));
                var point = Camera.main.WorldToViewportPoint(corner);
                Require(point.z > 0 && point.x >= pane.xMin && point.x <= pane.xMax && point.y >= pane.yMin && point.y <= pane.yMax,"Moving slab clipped outside available pane.");
            }
        }
        private IEnumerator Capture(string name)
        {
            if (RotateNative && _requestedLandscape.HasValue)
            {
                var deadline=Time.realtimeSinceStartup+6;
                while((Screen.width>Screen.height)!=_requestedLandscape.Value && Time.realtimeSinceStartup<deadline)yield return null;
                Require((Screen.width>Screen.height)==_requestedLandscape.Value,"Native orientation did not settle before capture: "+name);
                yield return new WaitForSecondsRealtime(.2f);
            }
            if (RotateNative) name += "-native-"+Screen.width+"x"+Screen.height;
            else if (Application.isMobilePlatform) name = name.Replace("wide-portrait", "native-current-pose").Replace("landscape", "native-current-pose").Replace("portrait", "native-current-pose");
            yield return new WaitForEndOfFrame();
            CheckButtonRaycasts();
            File.AppendAllText(Path.Combine(_output,"captures.txt"),$"{name}: {Screen.width}x{Screen.height}; safe area: {Screen.safeArea}\n");
            ScreenCapture.CaptureScreenshot(QaLaunch.ScreenshotPath(_output,name));
            yield return new WaitForSecondsRealtime(.3f);
        }
        private static void CheckButtonRaycasts()
        {
            if (Array.IndexOf(QaLaunch.Arguments(), "-qaBlockedButtonProbe") >= 0 && GameObject.Find("QaRaycastBlocker") == null)
            {
                var blocker = new GameObject("QaRaycastBlocker", typeof(RectTransform), typeof(Image));
                var rect = (RectTransform)blocker.transform;
                rect.SetParent(FindFirstObjectByType<Canvas>().transform, false);
                rect.anchorMin = Vector2.zero; rect.anchorMax = Vector2.one;
                rect.offsetMin = rect.offsetMax = Vector2.zero;
                blocker.GetComponent<Image>().color = Color.clear;
            }
            Canvas.ForceUpdateCanvases();
            var drop = GameObject.Find("Drop")?.GetComponent<Button>();
            if (drop != null && drop.isActiveAndEnabled)
            {
                var corners = new Vector3[4];
                ((RectTransform)drop.transform).GetWorldCorners(corners);
                var dropBounds = new Rect(corners[0].x, corners[0].y, corners[2].x-corners[0].x, corners[2].y-corners[0].y);
                foreach (var name in new[] { "PowerSlow", "PowerCenter", "PowerRepair", "Risk" })
                {
                    var control = GameObject.Find(name)?.GetComponent<Button>();
                    if (control == null || !control.isActiveAndEnabled) continue;
                    ((RectTransform)control.transform).GetWorldCorners(corners);
                    var bounds = new Rect(corners[0].x, corners[0].y, corners[2].x-corners[0].x, corners[2].y-corners[0].y);
                    Require(!dropBounds.Overlaps(bounds), "Drop target overlaps another gameplay control: " + name);
                }
            }
            var hits = new List<RaycastResult>();
            foreach (var button in FindObjectsByType<Button>(FindObjectsSortMode.None))
            {
                if (!button.isActiveAndEnabled || !button.IsInteractable() || button.GetComponent<Graphic>().canvasRenderer.cull) continue;
                var rect = (RectTransform)button.transform;
                var point = RectTransformUtility.WorldToScreenPoint(null, rect.TransformPoint(rect.rect.center));
                var clipping=button.GetComponentInParent<RectMask2D>();
                if(clipping!=null && !UiMetrics.ScreenRect((RectTransform)clipping.transform).Contains(point))continue; // Scroll to expose this center before tapping.
                hits.Clear();
                EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current) { position = point }, hits);
                Require(hits.Count > 0 && hits[0].gameObject.GetComponentInParent<Button>() == button,
                    "Visible button center is blocked or outside the input area: " + button.name);
            }
        }
        private static void SetReviewResolution(int width, int height, bool fullscreen)
        {
            // Device QA requests real orientation rather than changing render resolution.
            // Ordinary native probes retain their existing current-pose behavior.
            if (RotateNative){_requestedLandscape=width>height;Screen.orientation=_requestedLandscape.Value?ScreenOrientation.LandscapeLeft:ScreenOrientation.Portrait;}
            else if (!Application.isMobilePlatform) Screen.SetResolution(width, height, fullscreen);
        }
        private static Button FindButton(string name)
        {
            foreach (var button in FindObjectsByType<Button>(FindObjectsInactive.Include,FindObjectsSortMode.None)) if (button.name==name) return button;
            throw new InvalidOperationException("Missing actual UI button "+name);
        }
        private static void Require(bool value,string message) { if (!value) throw new InvalidOperationException(message); }
    }
}
#endif
