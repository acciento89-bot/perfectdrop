#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Kamilunavo.PerfectDrop.Gameplay;
namespace Kamilunavo.PerfectDrop.QA
{
    public sealed class RuntimeSmoke : MonoBehaviour
    {
        private string _output;
        private StackGame _game;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Bootstrap()
        {
            var args = Environment.GetCommandLineArgs();
            for (var i=0;i<args.Length-1;i++) if (args[i]=="-qaSmoke")
            {
                StackSave.QaKey = "perfectdrop.stack.qa.v1";
                PlayerPrefs.DeleteKey(StackSave.QaKey);
                var root = new GameObject("StackRuntimeSmoke"); DontDestroyOnLoad(root);
                root.AddComponent<RuntimeSmoke>()._output=args[i+1]; break;
            }
        }
        private IEnumerator Start()
        {
            Directory.CreateDirectory(_output);
            Time.captureDeltaTime = 1f / 60f; // Functional input timing; this does not measure real frame rate.
            var run = Run();
            while (true)
            {
                object next;
                try { if (!run.MoveNext()) break; next=run.Current; }
                catch (Exception error)
                {
                    Debug.LogException(error); File.WriteAllText(Path.Combine(_output,"result.txt"),"FAIL\n"+error);
                    ScreenCapture.CaptureScreenshot(Path.Combine(_output,"failure.png")); yield break;
                }
                yield return next;
            }
            File.WriteAllText(Path.Combine(_output,"result.txt"),"PASS: actual moving-block/drop-button sequence through 30; cut, miss, retry, settings pause, resize/progress, daily idempotence and scene reload resume.\n");
            Debug.Log("[PerfectDrop][QA] Stack runtime matrix passed.");
        }
        private IEnumerator Run()
        {
            yield return new WaitForSecondsRealtime(1);
            _game=FindFirstObjectByType<StackGame>();
            Require(_game!=null,"Stack game did not boot.");
            _game.NewRun();
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
                    Require(_game.Run.Top.Size.x<3.1f && _game.Run.Top.Size.x>2.9f,"Overhang did not shrink the first block.");
                    Require(GameObject.Find("Overhang")!=null,"Cut piece was not rendered.");
                }
                else Require(_game.Run.Streak==floor-1,"Perfect streak did not advance.");
                Require(StackSave.Load().Best>=floor,"Best was not saved.");
                Debug.Log("[PerfectDrop][QA] Real tap placement "+floor+" passed.");
                if (floor==5)
                {
                    yield return Capture("portrait");
                    Screen.SetResolution(800,600,false); yield return null; CheckFraming(); yield return new WaitForSecondsRealtime(.5f); yield return Capture("landscape");
                    Require(_game.Run.Layers.Count==5,"Resize reset progress.");
                    Kamilunavo.PerfectDrop.UI.ReservedRegionProvider.QaDivision = new Rect(.48f,0,.04f,1);
                    yield return new WaitForSecondsRealtime(.5f); yield return Capture("division-synthetic");
                    CheckFraming();
                    Kamilunavo.PerfectDrop.UI.ReservedRegionProvider.QaDivision = null;
                    Screen.SetResolution(600,800,false); yield return null; CheckFraming(); yield return new WaitForSecondsRealtime(.5f); yield return Capture("wide-portrait");
                    Require(_game.Run.Layers.Count==5,"Second resize reset progress.");
                    Screen.SetResolution(540,960,false); yield return null; CheckFraming(); yield return new WaitForSecondsRealtime(.5f);
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
        private void CheckFraming()
        {
            var block = GameObject.Find("MovingBlock"); if (block == null) return;
            var bounds = block.transform.Find("MetalDeck").GetComponent<Renderer>().bounds;
            var pane = _game.Hud.WorldPane;
            for (var i=0;i<8;i++)
            {
                var corner = bounds.center + Vector3.Scale(bounds.extents,new Vector3((i&1)==0?-1:1,(i&2)==0?-1:1,(i&4)==0?-1:1));
                var point = Camera.main.WorldToViewportPoint(corner);
                Require(point.z > 0 && point.x >= pane.xMin && point.x <= pane.xMax,"Moving slab clipped outside available pane.");
            }
        }
        private IEnumerator Capture(string name)
        {
            yield return new WaitForEndOfFrame();
            ScreenCapture.CaptureScreenshot(Path.Combine(_output,name+".png"));
            yield return new WaitForSecondsRealtime(.3f);
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
