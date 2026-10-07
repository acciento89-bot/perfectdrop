#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;
using Kamilunavo.PerfectDrop.Gameplay;
using Kamilunavo.PerfectDrop.UI;

namespace Kamilunavo.PerfectDrop.QA
{
    // Real-time rendering/stability evidence, separate from deterministic functional QA.
    public sealed class RenderSoak : MonoBehaviour
    {
        private string _output;
        private int _seconds=1800;
        private readonly List<string> _errors=new();
        private readonly Dictionary<string,List<float>> _frames=new();
        private StackGame _game;
        private string _mode="boot";
        private ProfilerRecorder _batches;
        private StreamWriter _csv;
        private long _firstMemory,_maxMemory;
        private int _frameCount,_focusedFrames;
        private bool _complete;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Bootstrap()
        {
            var args=Environment.GetCommandLineArgs();
            for(var i=0;i<args.Length-1;i++)if(args[i]=="-qaRenderSoak")
            {
                StackSave.QaKey="perfectdrop.render-soak.qa.v1";
                var profile=new StackProfile{UnlockedLevel=30,RunEndless=true,ResumeActive=true,TotalPlaced=96,PerfectDrops=96,MaxStreak=96,Streak=96,EndlessBest=96,Coins=1200,OwnedStyles=15,Phase=-1.57f};
                for(var level=0;level<30;level++)profile.LevelStars[level]=3;
                for(var layer=0;layer<64;layer++)profile.Layers.Add(new StackLayer{Center=Vector2.zero,Size=Vector2.one*3.6f});
                StackSave.Save(profile);
                var go=new GameObject("RenderSoak");DontDestroyOnLoad(go);var soak=go.AddComponent<RenderSoak>();soak._output=args[i+1];
                for(var j=0;j<args.Length-1;j++)if(args[j]=="-qaSoakSeconds" && int.TryParse(args[j+1],out var duration))soak._seconds=Mathf.Max(30,duration);
                break;
            }
        }
        private void Awake() => Application.logMessageReceived+=Log;
        private void Log(string message,string trace,LogType type)
        {
            if((type==LogType.Error || type==LogType.Exception || type==LogType.Assert) && _errors.Count<20)_errors.Add(message);
        }
        private IEnumerator Start()
        {
            Directory.CreateDirectory(_output); Time.captureDeltaTime=0;
            _csv=new StreamWriter(Path.Combine(_output,"render-samples.csv"));_csv.WriteLine("elapsed_seconds,mode,width,height,focused,frame_ms,batches,allocated_mib,active_renderers");
            _batches=ProfilerRecorder.StartNew(ProfilerCategory.Render,"Batches Count",1);
            yield return new WaitForSecondsRealtime(3);
            _game=FindFirstObjectByType<StackGame>();
            foreach(var name in new[]{"PerfectDropSky","PerfectDropSurface","PerfectDropPanel","PerfectDropGrade","PerfectDropBloom","PerfectDropSignal"})
            {
                var shader=Resources.Load<Shader>(name);if(shader==null || !shader.isSupported)_errors.Add("Missing/unsupported shader: "+name);
            }
            if(_game==null || _game.Run.Count!=96 || _game.Run.Layers.Count!=64)_errors.Add("Seeded 64-layer endless scene did not restore.");
            var start=Time.realtimeSinceStartup;var nextPhase=0f;var nextSample=0f;var phase=0;
            while(Time.realtimeSinceStartup-start<_seconds)
            {
                var elapsed=Time.realtimeSinceStartup-start;
                if(elapsed>=nextPhase)
                {
                    nextPhase=elapsed+60; _mode="warmup"; yield return Phase(phase++%6);
                    yield return new WaitForSecondsRealtime(2);
                    ScreenCapture.CaptureScreenshot(Path.Combine(_output,_mode+".png"));
                }
                if(!_frames.TryGetValue(_mode,out var frames)){frames=new List<float>();_frames[_mode]=frames;}
                frames.Add(Time.unscaledDeltaTime*1000);_frameCount++;if(Application.isFocused)_focusedFrames++;
                if(elapsed>=nextSample)
                {
                    nextSample=elapsed+1;var memory=Profiler.GetTotalAllocatedMemoryLong();
                    if(_firstMemory==0)_firstMemory=memory;_maxMemory=Math.Max(_maxMemory,memory);
                    var count=0;foreach(var renderer in FindObjectsByType<Renderer>(FindObjectsSortMode.None))if(renderer.enabled && renderer.gameObject.activeInHierarchy)count++;
                    _csv.WriteLine(string.Join(",",elapsed.ToString("F1",CultureInfo.InvariantCulture),_mode,Screen.width,Screen.height,Application.isFocused?1:0,(Time.unscaledDeltaTime*1000).ToString("F2",CultureInfo.InvariantCulture),_batches.Valid?_batches.LastValue:-1,(memory/1048576d).ToString("F1",CultureInfo.InvariantCulture),count));_csv.Flush();
                }
                yield return null;
            }
            var summary=new List<string>{"Desktop real-time rendering soak; not native performance or OS touch acceptance.","Duration seconds: "+_seconds,"Focused frame share: "+(_focusedFrames/(double)Math.Max(1,_frameCount)).ToString("P1",CultureInfo.InvariantCulture),"Unity allocated memory first/max MiB: "+(_firstMemory/1048576d).ToString("F1",CultureInfo.InvariantCulture)+" / "+(_maxMemory/1048576d).ToString("F1",CultureInfo.InvariantCulture)};
            foreach(var pair in _frames){pair.Value.Sort();summary.Add(pair.Key+" frame-time median/p95 ms: "+pair.Value[pair.Value.Count/2].ToString("F2",CultureInfo.InvariantCulture)+" / "+pair.Value[Mathf.Min(pair.Value.Count-1,Mathf.FloorToInt(pair.Value.Count*.95f))].ToString("F2",CultureInfo.InvariantCulture));}
            if(_focusedFrames/(double)Math.Max(1,_frameCount)<.95)_errors.Add("Too much unfocused time for moving-scene stability evidence.");
            summary.AddRange(_errors);File.WriteAllLines(Path.Combine(_output,"result.txt"),summary);File.WriteAllText(Path.Combine(_output,"status.txt"),_errors.Count==0?"PASS":"FAIL");
            _complete=true;_csv.Dispose();_csv=null;_batches.Dispose();Debug.Log("[PerfectDrop][QA] Render soak "+(_errors.Count==0?"passed":"failed"));
        }
        private IEnumerator Phase(int phase)
        {
            ReservedRegionProvider.QaDivision=null;
            _game.Hud.HideMenus();
            if(phase==0 || phase==5)
            {
                Screen.SetResolution(phase==5?600:540,phase==5?800:960,false);
                if(phase==5)ReservedRegionProvider.QaDivision=new Rect(.47f,0,.07f,1);
                _mode=phase==5?"endless-division-synthetic":"endless-64-portrait";
            }
            else if(phase==1)
            {Screen.SetResolution(800,600,false);_mode="endless-64-landscape";}
            else if(phase==2 || phase==3)
            {
                Screen.SetResolution(phase==2?540:800,phase==2?960:600,false);
                Button("City").onClick.Invoke();Button("CityDistrict2").onClick.Invoke();_mode=phase==2?"city-max-portrait":"city-max-landscape";
            }
            else
            {Screen.SetResolution(540,960,false);_game.Hud.ShowHome();Button("Chapter2").onClick.Invoke();_mode="chapter3-map";}
            yield return null;
        }
        private static Button Button(string name)
        {
            foreach(var button in FindObjectsByType<Button>(FindObjectsInactive.Include,FindObjectsSortMode.None))if(button.name==name)return button;
            throw new InvalidOperationException("Missing soak UI control: "+name);
        }
        private void OnDestroy()
        {
            Application.logMessageReceived-=Log;_csv?.Dispose();
            if(!_complete && _batches.Valid)_batches.Dispose();
        }
    }
}
#endif
