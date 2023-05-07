using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fade
{
    public class FadeBlocker : MonoBehaviour
    {
        [SerializeField] private LayerMask layerMask;
        [SerializeField] private Transform player;
        [SerializeField] private Camera cam;
        [SerializeField] private float fadedAlpha = .33f;
        [SerializeField] private FadeMode fadeMode;
        
        [SerializeField] private float checksPerSecond = 10;
        [SerializeField] private int fadeFps = 30;
        [SerializeField] private float fadeSpeed = 1;

        [Header("Read Only Data")] 
        [SerializeField] private List<FadingObject> objectsBlockingView = new List<FadingObject>();

        private Dictionary<FadingObject, Coroutine> _runningCoroutines = new Dictionary<FadingObject, Coroutine>();

        private RaycastHit[] _hits = new RaycastHit[10];
        
        private static readonly int SrcBlend = Shader.PropertyToID("_SrcBlend");
        private static readonly int DstBlend = Shader.PropertyToID("_DstBlend");
        private static readonly int ZWrite = Shader.PropertyToID("_ZWrite");

        private void Start() => StartCoroutine(CheckForObjects());

        private IEnumerator CheckForObjects()
        {
            var wait = new WaitForSeconds(1f / checksPerSecond);

            while (true)
            {
                var c = cam.transform.position;
                var p = player.transform.position;

                var raycastHit = Physics.RaycastNonAlloc(c, (p - c).normalized, _hits, Vector3.Distance(c, p), layerMask);
                
                if (raycastHit > 0)
                {
                    for (int i = 0; i < raycastHit; i++)
                    {
                        var fadingObject = GetFadingObjectFromHit(_hits[i]);
                        if (fadingObject == null || objectsBlockingView.Contains(fadingObject)) continue;
                        
                        if (_runningCoroutines.ContainsKey(fadingObject))
                        {
                            if (_runningCoroutines[fadingObject] != null)
                            {
                                StopCoroutine(_runningCoroutines[fadingObject]);
                            }

                            _runningCoroutines.Remove(fadingObject);
                        }
                        _runningCoroutines.Add(fadingObject, StartCoroutine(FadeObjectOut(fadingObject)));
                        objectsBlockingView.Add(fadingObject);
                    }
                }
                FadeObjectsNoLongerBeingHit();

                ClearHits();
                
                yield return wait;
            }
        }

        private void FadeObjectsNoLongerBeingHit()
        {
            
            for (int i = 0; i < objectsBlockingView.Count; i++)
            {
                var objectIsBeingHit = false;

                foreach (var hit in _hits)
                {
                    var fadingObject = GetFadingObjectFromHit(hit);
                    
                    if (fadingObject == null || fadingObject != objectsBlockingView[i]) continue;
                    
                    objectIsBeingHit = true;
                    break;
                }

                if (objectIsBeingHit) continue;
                
                if (_runningCoroutines.ContainsKey(objectsBlockingView[i]))
                {
                    if (_runningCoroutines[objectsBlockingView[i]] != null)
                    {
                        StopCoroutine(_runningCoroutines[objectsBlockingView[i]]);
                    }

                    _runningCoroutines.Remove(objectsBlockingView[i]);
                }
                _runningCoroutines.Add(objectsBlockingView[i], StartCoroutine(FadeObjectIn(objectsBlockingView[i])));
                objectsBlockingView.RemoveAt(i);
            }
        }

        private IEnumerator FadeObjectIn(FadingObject fadingObject)
        {
            var waitTime = 1f / fadeFps;
            var wait = new WaitForSeconds(waitTime);
            var ticks = 1;

            if (fadingObject.materials[0].HasProperty("_Color"))
            {
                while (fadingObject.materials[0].color.a < fadingObject.initialAlpha)
                {
                    foreach (var material in fadingObject.materials)
                    {
                        if (material.HasProperty("_Color"))
                        {
                            material.color = new Color(
                                material.color.r,
                                material.color.g,
                                material.color.b,
                                Mathf.Lerp(fadedAlpha, fadingObject.initialAlpha, waitTime * ticks * fadeSpeed));
                        }
                    }

                    ticks++;
                    yield return wait;
                }
            }

            foreach (var material in fadingObject.materials)
            {
                material.DisableKeyword(fadeMode == FadeMode.Fade ? "_ALPHABLEND_ON" : "_ALPHAPREMULTIPLY_ON");
                material.SetInt(SrcBlend, (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt(DstBlend, (int)UnityEngine.Rendering.BlendMode.Zero);
                material.SetInt(ZWrite, 1);
                material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry;
            }

            if (!_runningCoroutines.ContainsKey(fadingObject)) yield break;
            
            StopCoroutine(_runningCoroutines[fadingObject]);
            _runningCoroutines.Remove(fadingObject);
        }

        private IEnumerator FadeObjectOut(FadingObject fadingObject)
        {
            var waitTime = 1f / fadeFps;
            var wait = new WaitForSeconds(waitTime);
            var ticks = 1;

            foreach (var i in fadingObject.materials)
            {
                i.SetInt(SrcBlend, (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                i.SetInt(DstBlend, (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                i.SetInt(ZWrite, 0);
                i.EnableKeyword(fadeMode == FadeMode.Fade ? "_ALPHABLEND_ON" : "_ALPHAPREMULTIPLY_ON");
                i.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
            }

            if (fadingObject.materials[0].HasProperty("_Color"))
            {
                while (fadingObject.materials[0].color.a > fadedAlpha)
                {
                    foreach (var i in fadingObject.materials)
                    {
                        i.color = new Color(
                            i.color.r,
                            i.color.b,
                            i.color.g,
                            Mathf.Lerp(fadingObject.initialAlpha, fadedAlpha, waitTime * ticks * fadeSpeed));
                    }

                    ticks++;
                    yield return wait;
                }
            }

            if (!_runningCoroutines.ContainsKey(fadingObject)) yield break;
            
            StopCoroutine(_runningCoroutines[fadingObject]);
            _runningCoroutines.Remove(fadingObject);
        }
        
        private FadingObject GetFadingObjectFromHit(RaycastHit hit) => hit.collider != null ? hit.collider.GetComponent<FadingObject>() : null;

        private void ClearHits()
        {
            var hit = new RaycastHit();
            
            for (int i = 0; i < _hits.Length; i++)
            {
                _hits[i] = hit;
            }
        }

    }
    
    public enum FadeMode
    {
        Transparent,
        Fade
    }
}