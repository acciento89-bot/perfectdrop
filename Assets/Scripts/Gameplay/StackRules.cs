using System;
using System.Collections.Generic;
using UnityEngine;

namespace Kamilunavo.PerfectDrop.Gameplay
{
    public enum StackAxis { X, Z }
    public enum StackGrade { Miss, Good, Perfect }
    [Serializable] public struct StackLayer { public Vector2 Center, Size; }
    public struct StackDrop
    {
        public StackGrade Grade;
        public Vector2 Center, Size, CutCenter, CutSize;
    }

    public static class StackRules
    {
        public const int Target = 30;
        public const float BaseWidth = 3.6f;
        public static float MotionExtent(Vector2 size, StackAxis axis) => (axis == StackAxis.X ? size.x : size.y) * 1.24f;
        public static StackDrop Evaluate(Vector2 center, Vector2 size, Vector2 previousCenter, StackAxis axis)
        {
            if (!Finite(center) || !Finite(previousCenter) || !Finite(size) || size.x <= 0 || size.y <= 0)
                throw new ArgumentException("Stack dimensions and centers must be finite and dimensions positive.");
            var x = axis == StackAxis.X;
            var width = x ? size.x : size.y;
            var offset = x ? center.x - previousCenter.x : center.y - previousCenter.y;
            var result = new StackDrop { Center = center, Size = size };
            if (Mathf.Abs(offset) >= width)
            {
                result.Grade = StackGrade.Miss;
                return result;
            }
            var tolerance = Mathf.Min(0.085f, width * 0.035f);
            if (Mathf.Abs(offset) <= tolerance)
            {
                result.Grade = StackGrade.Perfect;
                result.Center = previousCenter;
                return result;
            }
            result.Grade = StackGrade.Good;
            var kept = width - Mathf.Abs(offset);
            var previous = x ? previousCenter.x : previousCenter.y;
            var middle = previous + offset * 0.5f;
            var cutMiddle = previous + Mathf.Sign(offset) * (width + Mathf.Abs(offset)) * 0.5f;
            result.CutCenter = center;
            result.CutSize = size;
            if (x)
            {
                result.Center.x = middle; result.Size.x = kept;
                result.CutCenter.x = cutMiddle; result.CutSize.x = Mathf.Abs(offset);
            }
            else
            {
                result.Center.y = middle; result.Size.y = kept;
                result.CutCenter.y = cutMiddle; result.CutSize.y = Mathf.Abs(offset);
            }
            return result;
        }
        private static bool Finite(Vector2 v) => !float.IsNaN(v.x) && !float.IsInfinity(v.x) && !float.IsNaN(v.y) && !float.IsInfinity(v.y);
    }

    public sealed class StackRun
    {
        public readonly List<StackLayer> Layers = new();
        public int Streak { get; private set; }
        public int EarnedCoins { get; private set; }
        public bool Failed { get; private set; }
        public bool Completed => Layers.Count == StackRules.Target;
        public StackAxis Axis => Layers.Count % 2 == 0 ? StackAxis.X : StackAxis.Z;
        public StackLayer Top => Layers.Count == 0 ? new StackLayer { Size = Vector2.one * StackRules.BaseWidth } : Layers[Layers.Count - 1];
        public StackDrop Place(float offset)
        {
            if (Failed || Completed) throw new InvalidOperationException("A terminal run cannot accept another block.");
            var center = Top.Center + (Axis == StackAxis.X ? new Vector2(offset, 0) : new Vector2(0, offset));
            var drop = StackRules.Evaluate(center, Top.Size, Top.Center, Axis);
            if (drop.Grade == StackGrade.Miss) { Failed = true; Streak = 0; return drop; }
            Streak = drop.Grade == StackGrade.Perfect ? Streak + 1 : 0;
            EarnedCoins += drop.Grade == StackGrade.Perfect ? 3 + Mathf.Min(Streak, 7) : 1;
            Layers.Add(new StackLayer { Center = drop.Center, Size = drop.Size });
            return drop;
        }
        public void Restore(IEnumerable<StackLayer> layers, int streak, int earnedCoins)
        {
            Layers.Clear(); Layers.AddRange(layers);
            Streak = Mathf.Clamp(streak, 0, Layers.Count);
            EarnedCoins = Mathf.Max(0, earnedCoins);
            Failed = false;
        }
    }
}
