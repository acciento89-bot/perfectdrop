using System;
using UnityEngine;
namespace Kamilunavo.PerfectDrop.Gameplay
{
    public enum StackBlockKind { Standard, Bonus, Fragile, Drift, Wind }
    public enum StackPower { Slow, Center, Repair }
    [Serializable]
    public sealed class StackPowers
    {
        public int Energy;
        public float SlowSeconds;
        public bool RepairReady, Risk;
        public static int Cost(StackPower power) => power==StackPower.Slow?2:power==StackPower.Center?3:4;
        public bool Activate(StackPower power)
        {
            if(Energy<Cost(power) || (power==StackPower.Slow && SlowSeconds>0) || (power==StackPower.Repair && RepairReady))return false;
            Energy-=Cost(power);
            if(power==StackPower.Slow)SlowSeconds=3;
            if(power==StackPower.Repair)RepairReady=true;
            return true;
        }
        public void Tick(float delta) => SlowSeconds=Mathf.Max(0,SlowSeconds-Mathf.Max(0,delta));
        public void Charge(bool risk) => Energy=Mathf.Min(6,Energy+(risk?2:1));
    }
}
