using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Loading
{
    /// Pure view for the loading screen prefab. Owns no loading logic and no fake pacing —
    /// it only renders whatever progress it is told about. Drive it via LoadingScreenService.
    public class LoadingScreenView : MonoBehaviour
    {
        [Header("Root")]
        [SerializeField] private CanvasGroup canvasGroup;

        [Header("Background")]
        [SerializeField] private Image background;

        [Header("Bar")]
        [Tooltip("Full-width track the kart travels along.")]
        [SerializeField] private RectTransform barTrack;
        [Tooltip("Filled portion of the bar. Image type must be Filled / Horizontal.")]
        [SerializeField] private Image barFill;
        [Tooltip("Kart icon that rides the bar from left to right.")]
        [SerializeField] private RectTransform kart;

        [Header("Text")]
        [SerializeField] private TMP_Text stageLabel;
        [SerializeField] private TMP_Text percentLabel;

        [Header("Feel")]
        [Tooltip("How long the bar takes to catch up to a newly reported value.")]
        [SerializeField] private float barCatchUpDuration = 0.35f;
        [SerializeField] private Ease barEase = Ease.OutCubic;
        [Tooltip("Degrees of bob applied to the kart while it travels.")]
        [SerializeField] private float kartBobAngle = 6f;
        [SerializeField] private float kartBobDuration = 0.45f;
        [SerializeField] private float fadeDuration = 0.35f;

        private Tween _barTween;
        private Tween _kartTween;
        private Tween _bobTween;
        private Tween _fadeTween;
        private float _displayedProgress;

        private float TrackWidth => barTrack != null ? barTrack.rect.width : 0f;

        private void Awake()
        {
            if (canvasGroup != null) canvasGroup.alpha = 0f;
            ApplyProgressImmediate(0f);
        }

        private void OnDestroy()
        {
            KillTweens();
        }

        /// Fades the screen in and starts the kart's idle bob.
        public Tween Show()
        {
            gameObject.SetActive(true);
            ApplyProgressImmediate(0f);
            StartBob();

            _fadeTween?.Kill();
            _fadeTween = canvasGroup
                .DOFade(1f, fadeDuration)
                .SetEase(Ease.OutQuad)
                .SetUpdate(true);
            return _fadeTween;
        }

        /// Fades the screen out. Caller decides when to deactivate.
        public Tween Hide()
        {
            _fadeTween?.Kill();
            _fadeTween = canvasGroup
                .DOFade(0f, fadeDuration)
                .SetEase(Ease.InQuad)
                .SetUpdate(true)
                .OnComplete(() =>
                {
                    _bobTween?.Kill();
                    gameObject.SetActive(false);
                });
            return _fadeTween;
        }

        /// Animates the bar + kart toward a normalised progress value.
        public void SetProgress(float normalised)
        {
            normalised = Mathf.Clamp01(normalised);
            if (Mathf.Approximately(normalised, _displayedProgress)) return;

            _barTween?.Kill();
            _kartTween?.Kill();

            float from = _displayedProgress;
            _displayedProgress = normalised;

            _barTween = DOVirtual
                .Float(from, normalised, barCatchUpDuration, RenderProgress)
                .SetEase(barEase)
                .SetUpdate(true);
        }

        public void SetStageLabel(string label)
        {
            if (stageLabel == null || stageLabel.text == label) return;

            stageLabel.text = label;
            stageLabel.transform.DOKill();
            stageLabel.transform.localScale = Vector3.one;
            stageLabel.transform
                .DOPunchScale(Vector3.one * 0.08f, 0.25f, 6, 0.8f)
                .SetUpdate(true);
        }

        public void SetBackground(Sprite sprite)
        {
            if (background != null && sprite != null) background.sprite = sprite;
        }

        private void ApplyProgressImmediate(float normalised)
        {
            _displayedProgress = Mathf.Clamp01(normalised);
            RenderProgress(_displayedProgress);
        }

        private void RenderProgress(float value)
        {
            if (barFill != null) barFill.fillAmount = value;

            if (kart != null)
            {
                // Kart rides the leading edge of the fill, staying inside the track.
                float halfKart = kart.rect.width * 0.5f;
                float travel = Mathf.Max(0f, TrackWidth - kart.rect.width);
                kart.anchoredPosition = new Vector2(halfKart + travel * value, kart.anchoredPosition.y);
            }

            if (percentLabel != null)
                percentLabel.text = Mathf.RoundToInt(value * 100f) + "%";
        }

        private void StartBob()
        {
            if (kart == null) return;

            _bobTween?.Kill();
            kart.localRotation = Quaternion.identity;
            _bobTween = kart
                .DOLocalRotate(new Vector3(0f, 0f, kartBobAngle), kartBobDuration)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine)
                .SetUpdate(true);
        }

        private void KillTweens()
        {
            _barTween?.Kill();
            _kartTween?.Kill();
            _bobTween?.Kill();
            _fadeTween?.Kill();
        }
    }
}
