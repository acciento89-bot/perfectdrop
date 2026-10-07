#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Kamilunavo.PerfectDrop.QA
{
    // Opt-in observation only: never synthesizes input or invokes game callbacks.
    public sealed class InputObservation : MonoBehaviour
    {
        private readonly List<RaycastResult> _hits = new();
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Bootstrap()
        {
            if (Array.IndexOf(Environment.GetCommandLineArgs(), "-qaObserveInput") < 0) return;
            new GameObject("InputObservation").AddComponent<InputObservation>();
        }
        private void Update()
        {
            if (!UnityEngine.Input.GetMouseButtonDown(0) && !UnityEngine.Input.GetMouseButtonUp(0) && !UnityEngine.Input.anyKeyDown) return;
            _hits.Clear();
            if (EventSystem.current != null)
                EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current) { position = UnityEngine.Input.mousePosition }, _hits);
            Debug.Log($"[InputObservation] focused={Application.isFocused} position={UnityEngine.Input.mousePosition} down={UnityEngine.Input.GetMouseButtonDown(0)} up={UnityEngine.Input.GetMouseButtonUp(0)} key={UnityEngine.Input.anyKeyDown} hit={(_hits.Count > 0 ? _hits[0].gameObject.name : "none")}");
        }
        private void OnGUI()
        {
            var current = Event.current;
            if (current.type == EventType.MouseDown || current.type == EventType.MouseUp || current.type == EventType.KeyDown || current.type == EventType.KeyUp)
                Debug.Log($"[InputObservation] GUI={current.type} position={current.mousePosition} key={current.keyCode}");
        }
    }
}
#endif
