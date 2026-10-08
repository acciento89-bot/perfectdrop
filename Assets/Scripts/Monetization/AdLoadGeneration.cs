namespace Kamilunavo.PerfectDrop.Monetization
{
    public sealed class AdLoadGeneration
    {
        private long _current;
        public long Begin()=>++_current;
        public void Invalidate()=>++_current;
        public bool IsCurrent(long generation)=>generation==_current;
    }
}
