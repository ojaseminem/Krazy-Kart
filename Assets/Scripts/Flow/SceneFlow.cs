using System;
using System.Collections;
using System.Collections.Generic;
using Loading;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Flow
{
    /// Persistent driver for scene-to-scene transitions: fade out → loading screen up →
    /// staged async load → loading screen down → fade in.
    ///
    /// The stage list is supplied by the caller (see GameplayLoadDirector) so that what is
    /// "being loaded" is owned by the destination's manager, not by this class or the view.
    public class SceneFlow : MonoBehaviour
    {
        private static SceneFlow _instance;

        public static SceneFlow Instance
        {
            get
            {
                if (_instance != null) return _instance;

                var host = new GameObject("[SceneFlow]");
                _instance = host.AddComponent<SceneFlow>();
                DontDestroyOnLoad(host);
                return _instance;
            }
        }

        [Header("Timing")]
        [SerializeField] private float fadeOutDuration = 0.45f;
        [SerializeField] private float fadeInDuration = 0.45f;
        [Tooltip("Held on a full bar before the new scene is revealed.")]
        [SerializeField] private float completionHold = 0.35f;

        public bool IsTransitioning { get; private set; }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void TransitionTo(string sceneName, IReadOnlyList<LoadStage> stages, Action onComplete = null)
        {
            if (IsTransitioning)
            {
                Debug.LogWarning("[SceneFlow] Transition already in progress; ignoring request.");
                return;
            }

            StartCoroutine(TransitionRoutine(sceneName, stages, onComplete));
        }

        private IEnumerator TransitionRoutine(string sceneName, IReadOnlyList<LoadStage> stages, Action onComplete)
        {
            IsTransitioning = true;

            // Menu fades to black before the loading screen appears.
            yield return ScreenFader.Instance.FadeOut(fadeOutDuration).WaitDone();

            var loading = LoadingScreenService.Instance;
            loading.Show(stages != null && stages.Count > 0 ? stages[0].label : "Loading");

            // Reveal the loading screen from behind the black overlay.
            yield return ScreenFader.Instance.FadeIn(fadeInDuration).WaitDone();

            var op = SceneManager.LoadSceneAsync(sceneName);
            op.allowSceneActivation = false;

            yield return RunStages(stages, op, loading);

            loading.ReportProgress(1f);
            yield return new WaitForSecondsRealtime(completionHold);

            // Hide the new scene's first frame behind black while it wakes up.
            yield return ScreenFader.Instance.FadeOut(fadeOutDuration).WaitDone();

            loading.Hide();
            op.allowSceneActivation = true;
            while (!op.isDone) yield return null;

            yield return null;

            yield return ScreenFader.Instance.FadeIn(fadeInDuration).WaitDone();

            IsTransitioning = false;
            onComplete?.Invoke();
        }

        /// Walks the advertised stages, blending real streaming progress with the dwell each
        /// stage asks for. Unity's async progress caps at 0.9 while activation is held back.
        private IEnumerator RunStages(IReadOnlyList<LoadStage> stages, AsyncOperation op, LoadingScreenService loading)
        {
            if (stages == null || stages.Count == 0)
            {
                while (op.progress < 0.9f)
                {
                    loading.ReportProgress(op.progress / 0.9f);
                    yield return null;
                }
                yield break;
            }

            float totalWeight = 0f;
            for (int i = 0; i < stages.Count; i++) totalWeight += Mathf.Max(0.01f, stages[i].weight);

            float consumed = 0f;

            for (int i = 0; i < stages.Count; i++)
            {
                var stage = stages[i];
                float share = Mathf.Max(0.01f, stage.weight) / totalWeight;
                float start = consumed;
                float end = consumed + share;

                loading.ReportStage(string.IsNullOrEmpty(stage.label) ? "Loading" : stage.label);

                float elapsed = 0f;
                bool isFinalStage = i == stages.Count - 1;

                while (true)
                {
                    elapsed += Time.unscaledDeltaTime;

                    float dwell = stage.minDuration <= 0f ? 1f : Mathf.Clamp01(elapsed / stage.minDuration);
                    float streamed = Mathf.Clamp01(op.progress / 0.9f);

                    // Non-final stages advance on their dwell; the final stage also waits for
                    // the real load so the bar can't finish ahead of the scene.
                    float t = isFinalStage ? Mathf.Min(dwell, streamed) : dwell;
                    loading.ReportProgress(Mathf.Lerp(start, end, t));

                    bool dwellDone = elapsed >= stage.minDuration;
                    bool streamDone = !isFinalStage || op.progress >= 0.9f;
                    if (dwellDone && streamDone) break;

                    yield return null;
                }

                consumed = end;
            }
        }
    }
}
