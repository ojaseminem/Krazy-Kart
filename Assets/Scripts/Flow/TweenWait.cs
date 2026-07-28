using System.Collections;
using DG.Tweening;

namespace Flow
{
    /// This project's DOTween build does not ship Tween.WaitForCompletion(), so coroutines wait
    /// on tween state directly. Null and already-dead tweens fall through immediately.
    public static class TweenWait
    {
        public static IEnumerator WaitDone(this Tween tween)
        {
            if (tween == null) yield break;

            while (tween.IsActive() && !tween.IsComplete())
                yield return null;
        }
    }
}
