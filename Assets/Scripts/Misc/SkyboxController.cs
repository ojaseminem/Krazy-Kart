using System;
using System.Collections;
using Managers;
using UnityEngine;

namespace Misc
{
    public class SkyboxController : MonoBehaviour
    {
        private static readonly int CubemapTransition = Shader.PropertyToID("_CubemapTransition");

        public static IEnumerator SkyboxCountDown(float startValue = 0f, float endValue = 0f, bool perform = false, float duration = 0, Action callback = null)
        {
            var levelData = GameManager.Instance.levelData;
            if(!perform) yield break;
            
            var elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                var transitionValue = Mathf.Lerp(
                    startValue,
                    endValue,
                    elapsedTime / duration);
                    
                levelData.skyboxMat.SetFloat(CubemapTransition, transitionValue);
                    
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            
            callback?.Invoke();
        }
    }
}