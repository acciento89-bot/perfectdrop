using UnityEngine;

namespace Kamilunavo.PerfectDrop.Gameplay
{
    public enum LandingGrade { Safe, Good, Perfect }

    public static class PrecisionScoring
    {
        public static LandingGrade Grade(float distanceFromCenter, float halfWidth)
        {
            var ratio = distanceFromCenter / Mathf.Max(0.01f, halfWidth);
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
