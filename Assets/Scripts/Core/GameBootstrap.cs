using UnityEngine;
using Kamilunavo.PerfectDrop.Gameplay;

namespace Kamilunavo.PerfectDrop
{
    // Keep the existing scene component/GUID; the confirmed product is now tap-to-stack.
    public sealed class GameBootstrap : MonoBehaviour
    {
        private void Start() => gameObject.AddComponent<StackGame>().Initialize();
    }
}
