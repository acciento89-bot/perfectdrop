using System.Runtime.InteropServices;
using UnityEngine;

namespace Kamilunavo.PerfectDrop.UI
{
    public static class ReservedRegionProvider
    {
        public static bool IsSupported
        {
            get
            {
#if UNITY_IOS && !UNITY_EDITOR
                return PDReservedRegionsSupported() != 0;
#else
                return false;
#endif
            }
        }

        public static bool TryGetDivisionRegion(out Rect region)
        {
#if UNITY_IOS && !UNITY_EDITOR
            if (PDGetPrimaryReservedRegion(1, out var x, out var y, out var width, out var height) != 0)
            {
                region = new Rect(
                    Mathf.Clamp01(x),
                    Mathf.Clamp01(y),
                    Mathf.Clamp01(width),
                    Mathf.Clamp01(height));
                return region.width > 0.0001f && region.height > 0.0001f;
            }
#endif
            region = default;
            return false;
        }

#if UNITY_IOS && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern int PDReservedRegionsSupported();

        [DllImport("__Internal")]
        private static extern int PDGetPrimaryReservedRegion(
            int kind,
            out float x,
            out float y,
            out float width,
            out float height);
#endif
    }
}
