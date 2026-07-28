using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Flow
{
    /// Persistent full-screen black overlay used between flow steps. Built at runtime so no
    /// scene needs to own it, and so it always renders above every other canvas.
    public class ScreenFader : MonoBehaviour
    {
        private const int SortingOrder = 32000;

        private static ScreenFader _instance;

        public static ScreenFader Instance
        {
            get
            {
                if (_instance != null) return _instance;

                var host = new GameObject("[ScreenFader]");
                _instance = host.AddComponent<ScreenFader>();
                DontDestroyOnLoad(host);
                _instance.Build();
                return _instance;
            }
        }

        private CanvasGroup _group;
        private Tween _tween;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
            Build();
        }

        public Tween FadeOut(float duration = 0.4f) => FadeTo(1f, duration);

        public Tween FadeIn(float duration = 0.4f) => FadeTo(0f, duration);

        private Tween FadeTo(float target, float duration)
        {
            Build();
            _tween?.Kill();
            _group.blocksRaycasts = target > 0.01f;
            _tween = _group
                .DOFade(target, duration)
                .SetEase(Ease.InOutQuad)
                .SetUpdate(true);
            return _tween;
        }

        private void Build()
        {
            if (_group != null) return;

            var canvasGo = new GameObject("FadeCanvas", typeof(Canvas), typeof(CanvasGroup));
            canvasGo.transform.SetParent(transform, false);

            var canvas = canvasGo.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = SortingOrder;

            _group = canvasGo.GetComponent<CanvasGroup>();
            _group.alpha = 0f;
            _group.blocksRaycasts = false;

            var imageGo = new GameObject("Black", typeof(Image));
            imageGo.transform.SetParent(canvasGo.transform, false);

            var image = imageGo.GetComponent<Image>();
            image.color = Color.black;

            var rect = image.rectTransform;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }
    }
}
