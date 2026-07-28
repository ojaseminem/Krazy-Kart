using System;

namespace Gameplay
{
    /// Lightweight static counters for the current run. Kept separate from McManager so systems
    /// that only care about "how much has been wrecked" (cop spawning, summary screen) do not
    /// need a reference to the scoring object.
    public static class RunStats
    {
        public static int PropsDestroyed { get; private set; }
        public static int PropsDestroyedThisFloor { get; private set; }
        public static int NpcsHit { get; private set; }
        public static int CopsEvaded { get; private set; }
        public static float RunTime { get; set; }

        /// Raised every time a prop breaks, carrying the running floor count.
        public static event Action<int> OnPropDestroyed;

        public static void NotifyPropDestroyed()
        {
            PropsDestroyed++;
            PropsDestroyedThisFloor++;
            OnPropDestroyed?.Invoke(PropsDestroyedThisFloor);
        }

        public static void NotifyNpcHit() => NpcsHit++;
        public static void NotifyCopEvaded() => CopsEvaded++;

        public static void ResetFloor() => PropsDestroyedThisFloor = 0;

        public static void ResetRun()
        {
            PropsDestroyed = 0;
            PropsDestroyedThisFloor = 0;
            NpcsHit = 0;
            CopsEvaded = 0;
            RunTime = 0f;
        }
    }
}
