#if UNITY_EDITOR
using System;
using UnityEditor;
using Kamilunavo.PerfectDrop.UI;

namespace Kamilunavo.PerfectDrop.Editor
{
    public static class DuoReadinessValidation
    {
        public static void Validate()
        {
            AssertLayout(1080f, 1920f, MobileLayoutClass.CompactPortrait, "classic 9:16 portrait");
            AssertLayout(1536f, 2048f, MobileLayoutClass.WidePortrait, "iPhone Duo / 3:4 portrait");
            AssertLayout(2048f, 1536f, MobileLayoutClass.Landscape, "iPhone Duo / 4:3 landscape");
            AssertLayout(2556f, 1179f, MobileLayoutClass.Landscape, "wide resizable landscape");
            UnityEngine.Debug.Log("[PerfectDrop] Adaptive display matrix passed.");
        }

        private static void AssertLayout(float width, float height, MobileLayoutClass expected, string label)
        {
            var actual = ResponsiveHud.Classify(width, height);
            if (actual != expected)
                throw new InvalidOperationException($"{label}: expected {expected}, got {actual} ({width}x{height}).");
        }
    }
}
#endif
