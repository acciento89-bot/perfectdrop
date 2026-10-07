#if UNITY_EDITOR
using System;
using UnityEngine;
using Kamilunavo.PerfectDrop.Gameplay;
namespace Kamilunavo.PerfectDrop.Editor
{
    public static class StackValidation
    {
        public static void ValidatePlatform()
        { Validate(); DuoReadinessValidation.Validate(); }
        public static void Validate()
        {
            var size = new Vector2(3.6f, 3.6f);
            var perfect = StackRules.Evaluate(new Vector2(0.03f, 0), size, Vector2.zero, StackAxis.X);
            Assert(perfect.Grade == StackGrade.Perfect && perfect.Center == Vector2.zero && perfect.Size == size, "Perfect must snap and preserve area.");
            foreach (var axis in new[] { StackAxis.X, StackAxis.Z })
            foreach (var sign in new[] { -1f, 1f })
            {
                var center = axis == StackAxis.X ? new Vector2(sign * 0.6f, 0) : new Vector2(0, sign * 0.6f);
                var cut = StackRules.Evaluate(center, size, Vector2.zero, axis);
                Assert(cut.Grade == StackGrade.Good, "Partial overlap must be Good.");
                Near(axis == StackAxis.X ? cut.Size.x : cut.Size.y, 3f, "Retained width");
                Near(axis == StackAxis.X ? cut.Center.x : cut.Center.y, sign * 0.3f, "Retained center");
                Near(axis == StackAxis.X ? cut.CutSize.x : cut.CutSize.y, 0.6f, "Cut width");
                Near(axis == StackAxis.X ? cut.CutCenter.x : cut.CutCenter.y, sign * 2.1f, "Cut center");
                var miss = StackRules.Evaluate(center * 6f, size, Vector2.zero, axis);
                Assert(miss.Grade == StackGrade.Miss, "Touching edges with no area must fail.");
            }
            var small = StackRules.Evaluate(new Vector2(0.001f, 0), new Vector2(0.03f, 2f), Vector2.zero, StackAxis.X);
            Assert(small.Grade == StackGrade.Perfect && small.Size.x == 0.03f, "Small perfect block must not grow.");
            foreach (var axis in new[] { StackAxis.X, StackAxis.Z })
            {
                var narrow = axis == StackAxis.X ? new Vector2(3.6f,.1f) : new Vector2(.1f,3.6f);
                Assert(StackRules.MotionExtent(narrow,axis) > 3.6f,"Motion must clear overlap even after orthogonal shrink.");
            }
            var run = new StackRun();
            for (var i = 0; i < 30; i++)
            {
                Assert(run.Axis == (i % 2 == 0 ? StackAxis.X : StackAxis.Z), "Axes must alternate.");
                run.Place(0);
            }
            Assert(run.Completed && run.Layers.Count == 30 && run.Streak == 30 && run.EarnedCoins > 90, "Exactly 30 perfect placements must complete and reward streaks.");
            var rejected = false;
            try { run.Place(0); } catch (InvalidOperationException) { rejected = true; }
            Assert(rejected && run.Layers.Count == 30, "Layer 31 must be rejected.");
            var failed = new StackRun();
            failed.Place(4);
            Assert(failed.Failed && failed.Layers.Count == 0 && failed.EarnedCoins == 0, "A miss must fail without a reward.");
            rejected = false;
            try { failed.Place(0); } catch (InvalidOperationException) { rejected = true; }
            Assert(rejected, "Failed run must reject repeated taps.");
            var invalid = false;
            try { StackRules.Evaluate(new Vector2(float.NaN, 0), size, Vector2.zero, StackAxis.X); } catch (ArgumentException) { invalid = true; }
            Assert(invalid, "Non-finite input must be rejected.");
            var saved = new StackProfile { ResumeActive = true, Best = 1, Coins = 7, Layers = new System.Collections.Generic.List<StackLayer> { new StackLayer { Center = Vector2.zero, Size = Vector2.one*3.6f } } };
            var restored = StackSave.Parse(JsonUtility.ToJson(saved));
            Assert(restored.ResumeActive && restored.Layers.Count == 1 && restored.Coins == 7, "Stack save round trip failed.");
            saved.Layers[0] = new StackLayer { Size = new Vector2(float.NaN, 3) };
            Assert(!StackSave.Parse(JsonUtility.ToJson(saved)).ResumeActive, "Invalid dimensions must not restore a tower.");
            Assert(StackSave.Parse("broken-json").Layers.Count == 0, "Corrupt data must recover safely.");
            var artRoot = new GameObject("StackArtValidation");
            try
            {
                var block = Kamilunavo.PerfectDrop.Visuals.WorldArt.CreateStackBlock(artRoot.transform, "Block", Vector3.zero, new Vector3(3.6f,.34f,3.6f),1);
                var mesh = block.transform.Find("MetalDeck").GetComponent<MeshFilter>().sharedMesh;
                Assert(mesh.normals[0].y > .9f, "Visible top cap must face upward.");
            }
            finally { UnityEngine.Object.DestroyImmediate(artRoot); }
            Debug.Log("[PerfectDrop] Stack overlap, cut geometry, perfect and miss matrix passed.");
        }
        private static void Near(float a, float b, string label) => Assert(Mathf.Abs(a-b) < 0.0001f, label);
        private static void Assert(bool value, string message) { if (!value) throw new InvalidOperationException(message); }
    }
}
#endif
