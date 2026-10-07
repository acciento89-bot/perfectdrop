using UnityEngine;

namespace Kamilunavo.PerfectDrop.Gameplay
{
    public sealed class PrecisionPlatform : MonoBehaviour
    {
        public int Index { get; set; }
        public float BayHalfWidth { get; set; } = 1.65f;
        public float BayHalfDepth { get; set; } = 1.10f;
        public GameObject LandingBayRoot { get; set; }

        public void SetTarget(bool active)
        {
            if (LandingBayRoot != null) LandingBayRoot.SetActive(active);
        }
    }
}
