using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Gameplay
{
    /// A single floating "+MC" label. Pooled and driven by GameplayHud.
    public class McPopup : MonoBehaviour
    {
        [SerializeField] private TMP_Text label;
        [SerializeField] private CanvasGroup group;
        [SerializeField] private float riseDistance = 90f;
        [SerializeField] private float duration = 1.05f;

        private RectTransform _rect;
        private Sequence _sequence;

        private void Awake()
        {
            _rect = (RectTransform)transform;
            if (label == null) label = GetComponentInChildren<TMP_Text>();
            if (group == null) group = GetComponent<CanvasGroup>();
        }

        public void Play(string text, Color color, Vector2 anchoredPosition)
        {
            gameObject.SetActive(true);
            _sequence?.Kill();

            label.text = text;
            label.color = color;
            _rect.anchoredPosition = anchoredPosition;
            _rect.localScale = Vector3.one * 0.6f;
            group.alpha = 1f;

            _sequence = DOTween.Sequence();
            _sequence.Append(_rect.DOScale(1f, 0.22f).SetEase(Ease.OutBack));
            _sequence.Join(_rect.DOAnchorPosY(anchoredPosition.y + riseDistance, duration).SetEase(Ease.OutCubic));
            _sequence.Insert(duration * 0.55f, group.DOFade(0f, duration * 0.45f));
            _sequence.OnComplete(() => gameObject.SetActive(false));
        }

        private void OnDestroy() => _sequence?.Kill();
    }
}
