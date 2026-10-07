#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System.Collections;
using System.IO;
using UnityEngine;

namespace Kamilunavo.PerfectDrop.QA
{
    public sealed class VisualQaCapture : MonoBehaviour
    {
        private string _path;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Bootstrap()
        {
            var path = GetArgument("-qaScreenshot");
            if (string.IsNullOrWhiteSpace(path)) return;

            var go = new GameObject("VisualQaCapture");
            DontDestroyOnLoad(go);
            var capture = go.AddComponent<VisualQaCapture>();
            capture._path = path;
        }

        private IEnumerator Start()
        {
            yield return new WaitForSecondsRealtime(2.5f);
            yield return new WaitForEndOfFrame();

            var directory = Path.GetDirectoryName(_path);
            if (!string.IsNullOrEmpty(directory))
                Directory.CreateDirectory(directory);

            ScreenCapture.CaptureScreenshot(_path);
            Debug.Log($"[PerfectDrop][QA] Screenshot requested: {_path}");

            var timeout = Time.realtimeSinceStartup + 5f;
            while (!File.Exists(_path) && Time.realtimeSinceStartup < timeout)
                yield return null;

            if (File.Exists(_path))
                Debug.Log($"[PerfectDrop][QA] Screenshot saved ({new FileInfo(_path).Length} bytes).");
            else
                Debug.LogError($"[PerfectDrop][QA] Screenshot was not created: {_path}");
        }

        private static string GetArgument(string key)
        {
            var args = System.Environment.GetCommandLineArgs();
            for (var i = 0; i < args.Length - 1; i++)
            {
                if (args[i] == key)
                    return args[i + 1];
            }

            return null;
        }
    }
}

#endif
