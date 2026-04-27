using UnityEngine;

namespace FrameCoreU.Timing
{
    [System.Serializable]
    public class TickRateLimiter
    {
        public bool limitUpdateRate;
        public float updatesPerSecond = 30f;

        private float nextTickTime;

        public bool CanTick()
        {
            if (!limitUpdateRate)
                return true;

            if (Time.time < nextTickTime)
                return false;

            nextTickTime = Time.time + 1f / Mathf.Max(1f, updatesPerSecond);
            return true;
        }

        public void Reset()
        {
            nextTickTime = 0f;
        }
    }
}