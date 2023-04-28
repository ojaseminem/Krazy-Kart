using System;
using System.Collections;
using UnityEngine;

namespace Utils
{
    public static class Util
    {
        private static Matrix4x4 _isoMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0));

        public static Vector3 ToIso(this Vector3 input) => _isoMatrix.MultiplyPoint3x4(input);
    
        public static IEnumerator WaitForSecondsRoutine(float time, Action callback = null)
        {
            yield return new WaitForSeconds(time);
            callback?.Invoke();
        }

        public static IEnumerator WaitUntilRoutine(Func<bool> condition, Action callback = null)
        {
            yield return new WaitUntil(condition);
            callback?.Invoke();
        }
    }
}