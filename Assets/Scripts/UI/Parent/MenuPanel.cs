using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Parent
{
    /// Base class for every modular menu panel prefab (Settings, Co-op, Socials, ...).
    ///
    /// Panels live as their own prefabs under Resources/MenuPanels and are spawned on demand by
    /// MenuPanelRouter, so editing one never dirties the menu scene. Subclasses override
    /// OnOpened/OnClosing for their own content behaviour rather than re-implementing animation.
    [RequireComponent(typeof(CanvasGroup))]
    public class MenuPanel : MonoBehaviour
    {
        [Header("Layout")]
        [SerializeField] protected RectTransform content;
        [SerializeField] private Button closeButton;
        [Tooltip("Optional click-catcher behind the panel that closes it.")]
        [SerializeField] private Button backdrop;

        [Header("Animation")]
        [SerializeField] private float openDuration = 0.4f;
        [SerializeField] private float closeDuration = 0.25f;
        [SerializeField] private Vector2 openFromOffset = new Vector2(0f, -60f);
        [SerializeField] private Ease openEase = Ease.OutBack;
        [SerializeField] private Ease closeEase = Ease.InBack;
        [SerializeField] private float openFromScale = 0.9f;

        public event Action<MenuPanel> OnCloseRequested;

        private CanvasGroup _group;
        private Sequence _sequence;
        private Vector2 _restAnchoredPos;
        private bool _capturedRest;

        protected CanvasGroup Group
        {
            get
            {
                if (_group == null) _group = GetComponent<CanvasGroup>();
                return _group;
            }
        }

        protected virtual void Awake()
        {
            CaptureRestPose();

            if (closeButton != null) closeButton.onClick.AddListener(RequestClose);
            if (backdrop != null) backdrop.onClick.AddListener(RequestClose);
        }

        protected virtual void OnDestroy()
        {
            _sequence?.Kill();

            if (closeButton != null) closeButton.onClick.RemoveListener(RequestClose);
            if (backdrop != null) backdrop.onClick.RemoveListener(RequestClose);
        }

        public void Open()
        {
            CaptureRestPose();
            gameObject.SetActive(true);

            _sequence?.Kill();
            Group.alpha = 0f;
            Group.blocksRaycasts = true;
            Group.interactable = false;

            if (content != null)
            {
                content.anchoredPosition = _restAnchoredPos + openFromOffset;
                content.localScale = Vector3.one * openFromScale;
            }

            _sequence = DOTween.Sequence().SetUpdate(true);
            _sequence.Append(Group.DOFade(1f, openDuration * 0.6f).SetEase(Ease.OutQuad));

            if (content != null)
            {
                _sequence.Join(content.DOAnchorPos(_restAnchoredPos, openDuration).SetEase(openEase));
                _sequence.Join(content.DOScale(1f, openDuration).SetEase(openEase));
            }

            _sequence.OnComplete(() =>
            {
                Group.interactable = true;
                OnOpened();
            });
        }

        public void Close(Action onClosed = null)
        {
            OnClosing();

            _sequence?.Kill();
            Group.interactable = false;
            Group.blocksRaycasts = false;

            _sequence = DOTween.Sequence().SetUpdate(true);
            _sequence.Append(Group.DOFade(0f, closeDuration).SetEase(Ease.InQuad));

            if (content != null)
            {
                _sequence.Join(content.DOAnchorPos(_restAnchoredPos + openFromOffset, closeDuration).SetEase(closeEase));
                _sequence.Join(content.DOScale(openFromScale, closeDuration).SetEase(closeEase));
            }

            _sequence.OnComplete(() =>
            {
                gameObject.SetActive(false);
                onClosed?.Invoke();
            });
        }

        protected void RequestClose() => OnCloseRequested?.Invoke(this);

        /// Called once the open animation finishes.
        protected virtual void OnOpened() { }

        /// Called as the close animation begins — commit any pending state here.
        protected virtual void OnClosing() { }

        private void CaptureRestPose()
        {
            if (_capturedRest || content == null) return;

            _restAnchoredPos = content.anchoredPosition;
            _capturedRest = true;
        }
    }
}
