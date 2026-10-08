namespace Kamilunavo.PerfectDrop.Monetization
{
    // A failed SDK callback must not start a second connection while its await is pending.
    public sealed class StoreReconnectGate
    {
        private bool _connecting;private double _retryAt;
        public bool CanBegin(double now)=>!_connecting&&now>=_retryAt;
        public bool TryBegin(double now){if(!CanBegin(now))return false;_connecting=true;return true;}
        public void EndAttempt()=>_connecting=false;
        public void Failed(double now)=>_retryAt=now+5;
    }
}
