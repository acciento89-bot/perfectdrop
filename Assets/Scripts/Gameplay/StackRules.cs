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
        public bool Rescued;
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
        public const int RetainedLayers = 64;
        public readonly List<StackLayer> Layers = new();
        public StackPowers Powers { get; private set; } = new();
        public int Count { get; private set; }
        public int PerfectDrops { get; private set; }
        public int MaxStreak { get; private set; }
        public int Target { get; }
        public bool Endless { get; }
        private readonly Vector2 _startSize;
        public StackRun(int target=StackRules.Target,bool endless=false,Vector2? startSize=null)
        {
            if(!endless && (target<1 || target>30)) throw new ArgumentOutOfRangeException(nameof(target));
            Target=target; Endless=endless; _startSize=startSize ?? Vector2.one*StackRules.BaseWidth;
        }
        public int Streak { get; private set; }
        public int EarnedCoins { get; private set; }
        public bool Failed { get; private set; }
        public bool Completed => !Endless && Count >= Target;
        public StackAxis Axis => Count % 2 == 0 ? StackAxis.X : StackAxis.Z;
        public StackLayer Top => Layers.Count == 0 ? new StackLayer { Size = _startSize } : Layers[Layers.Count - 1];
        public StackDrop Place(float offset,StackBlockKind kind=StackBlockKind.Standard)
        {
            if (Failed || Completed) throw new InvalidOperationException("A terminal run cannot accept another block.");
            var center = Top.Center + (Axis == StackAxis.X ? new Vector2(offset, 0) : new Vector2(0, offset));
            var drop = StackRules.Evaluate(center, Top.Size, Top.Center, Axis);
            var risk=Powers.Risk; Powers.Risk=false;
            if(risk && drop.Grade!=StackGrade.Perfect)drop.Grade=StackGrade.Miss;
            if(Powers.RepairReady && drop.Grade!=StackGrade.Perfect)
            {
                Powers.RepairReady=false;
                drop=new StackDrop { Grade=StackGrade.Good,Center=Top.Center,Size=Top.Size,Rescued=true };
            }
            if(kind==StackBlockKind.Fragile && drop.Grade==StackGrade.Good && !drop.Rescued)
            { if(Axis==StackAxis.X)drop.Size.x*=.9f;else drop.Size.y*=.9f; }
            if (drop.Grade == StackGrade.Miss) { Failed = true; Streak = 0; return drop; }
            Streak = drop.Grade == StackGrade.Perfect ? Streak + 1 : 0;
            if(drop.Grade==StackGrade.Perfect) PerfectDrops++;
            MaxStreak=Mathf.Max(MaxStreak,Streak);
            var reward=drop.Grade==StackGrade.Perfect?3+Mathf.Min(Streak,7)+(kind==StackBlockKind.Bonus?4:0):1;
            EarnedCoins+=drop.Grade==StackGrade.Perfect && risk?reward*2:reward;
            if(drop.Grade==StackGrade.Perfect)Powers.Charge(risk);
            Layers.Add(new StackLayer { Center = drop.Center, Size = drop.Size });
            Count++;
            if(Endless && Layers.Count>RetainedLayers) Layers.RemoveAt(0);
            return drop;
        }
        public void Restore(IEnumerable<StackLayer> layers, int streak, int earnedCoins,int total=0,int perfect=0,int maxStreak=0)
        {
            Layers.Clear(); Layers.AddRange(layers);
            Count=Mathf.Max(total,Layers.Count);
            Streak = Mathf.Clamp(streak, 0, Count);
            PerfectDrops=Mathf.Clamp(perfect,0,Count); MaxStreak=Mathf.Clamp(Mathf.Max(maxStreak,Streak),0,Count);
            EarnedCoins = Mathf.Max(0, earnedCoins);
            Failed = false;
        }
        public void RestorePowers(StackPowers powers) { Powers=powers ?? new StackPowers(); }
    }
}
