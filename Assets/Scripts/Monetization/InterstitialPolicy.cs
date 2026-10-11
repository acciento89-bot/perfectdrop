using System;
using System.Collections.Generic;

namespace Kamilunavo.PerfectDrop.Monetization
{
    // A result boundary is the only caller allowed to reserve an ad. Loading
    // or the clock becoming eligible never creates a future presentation.
    public sealed class InterstitialPolicy
    {
        private readonly HashSet<string> _completed = new HashSet<string>();
        private double _lastImpression;
        private int _successes;
        private bool _reserved;
        public InterstitialPolicy(double sessionStart) { _lastImpression = sessionStart; }
        public bool RecordCompletion(string token, bool tutorial, bool eligible)
        {
            if (tutorial || !eligible || string.IsNullOrWhiteSpace(token) || !_completed.Add(token)) return false;
            if (_successes < 3) _successes++;
            return true;
        }
        public bool CanShow(double now, bool owned, bool available, bool busy, bool focused, bool resultVisible)
        {
            return !_reserved && !owned && available && !busy && focused && resultVisible && _successes >= 3
                && !double.IsNaN(now) && !double.IsInfinity(now) && now - _lastImpression >= 180;
        }
        public bool TryReserve(double now, bool owned, bool available, bool busy, bool focused, bool resultVisible)
        {
            if (!CanShow(now, owned, available, busy, focused, resultVisible)) return false;
            _reserved = true;
            return true;
        }
        public void Impression(double now)
        {
            if (!_reserved || double.IsNaN(now) || double.IsInfinity(now)) return;
            _lastImpression = Math.Max(_lastImpression, now);
            _successes = 0;
        }
        public void CancelReservation() { _reserved = false; }
        public void RewardedOpened(double now)
        {
            if (!double.IsNaN(now) && !double.IsInfinity(now)) _lastImpression = Math.Max(_lastImpression, now);
        }
    }
}
