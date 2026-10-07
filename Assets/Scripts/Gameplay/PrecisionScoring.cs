using UnityEngine;

namespace Kamilunavo.PerfectDrop.Gameplay
{
    public enum LandingGrade { Safe, Good, Perfect }

    public static class PrecisionScoring
    {
        public static LandingGrade Grade(float localX, float localZ, float halfWidth, float halfDepth)
        {
            var normalizedX = Mathf.Abs(localX) / Mathf.Max(0.01f, halfWidth);
            var normalizedZ = Mathf.Abs(localZ) / Mathf.Max(0.01f, halfDepth);
            var ratio = Mathf.Max(normalizedX, normalizedZ);

            if (ratio <= 0.18f) return LandingGrade.Perfect;
            if (ratio <= 0.45f) return LandingGrade.Good;
            return LandingGrade.Safe;
        }

        public static int CoinReward(LandingGrade grade, int streak)
        {
            var baseValue = grade == LandingGrade.Perfect ? 8 : grade == LandingGrade.Good ? 4 : 2;
            return baseValue + Mathf.Clamp(streak / 3, 0, 8);
        }
    }
}
