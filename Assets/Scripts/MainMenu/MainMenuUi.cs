using System;
using System.Collections.Generic;
using DG.Tweening;
using Flow;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu
{
    /// The menu's 2D layer: a left action column, a right social/extras column, and the
    /// Play → (Normal | Free Run) expansion. The kart sits in 3D between the two columns, so
    /// neither column is allowed to occupy the centre.
    public class MainMenuUi : MonoBehaviour
    {
        [Header("Root")]
        [SerializeField] private CanvasGroup root;

        [Header("Columns")]
        [SerializeField] private RectTransform leftColumn;
        [SerializeField] private RectTransform rightColumn;
        [Tooltip("Buttons in the left column, top to bottom. Animated in sequence.")]
        [SerializeField] private List<RectTransform> leftItems = new List<RectTransform>();
        [Tooltip("Buttons in the right column, top to bottom. Animated in sequence.")]
        [SerializeField] private List<RectTransform> rightItems = new List<RectTransform>();

        [Header("Primary Buttons")]
        [SerializeField] private Button playButton;
        [SerializeField] private Button coopButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button quitButton;

        [Header("Play Mode Expansion")]
        [SerializeField] private RectTransform playModeGroup;
        [SerializeField] private CanvasGroup playModeGroupCanvas;
        [SerializeField] private Button normalModeButton;
        [SerializeField] private Button freeRunModeButton;

        [Header("Panels")]
        [SerializeField] private MenuPanelRouter panelRouter;

        [Header("Animation")]
        [SerializeField] private float itemStagger = 0.07f;
        [SerializeField] private float itemDuration = 0.5f;
        [Tooltip("How far off-column items start. Large enough to clear the scrim entirely.")]
        [SerializeField] private float slideDistance = 620f;
        [SerializeField] private Ease itemEase = Ease.OutBack;

        public event Action<GameMode> OnPlayRequested;

        private readonly Dictionary<RectTransform, Vector2> _restPositions = new Dictionary<RectTransform, Vector2>();
        private Sequence _introSequence;
        private Sequence _playModeSequence;
        private bool _playModeExpanded;

        private void Awake()
        {
            CaptureRestPositions(leftItems);
            CaptureRestPositions(rightItems);

            if (playButton != null) playButton.onClick.AddListener(TogglePlayModes);
            if (coopButton != null) coopButton.onClick.AddListener(() => OpenPanel(MenuPanelType.Coop));
            if (settingsButton != null) settingsButton.onClick.AddListener(() => OpenPanel(MenuPanelType.Settings));
            if (quitButton != null) quitButton.onClick.AddListener(() => OpenPanel(MenuPanelType.Quit));

            if (normalModeButton != null) normalModeButton.onClick.AddListener(() => RequestPlay(GameMode.Normal));
            if (freeRunModeButton != null) freeRunModeButton.onClick.AddListener(() => RequestPlay(GameMode.FreeRun));

            SetHiddenImmediate();
        }

        private void OnDestroy()
        {
            _introSequence?.Kill();
            _playModeSequence?.Kill();
        }

        /// Pops the UI in after the intro cinematic settles. Columns fly in from their own
        /// screen edges so the centred kart is never covered mid-animation.
        public Tween PlayIntro()
        {
            _introSequence?.Kill();

            root.alpha = 0f;
            root.gameObject.SetActive(true);
            SetHiddenImmediate();

            _introSequence = DOTween.Sequence();
            _introSequence.Append(root.DOFade(1f, 0.3f).SetEase(Ease.OutQuad));

            float cursor = 0f;
            cursor = AppendColumn(leftItems, -slideDistance, cursor);
            AppendColumn(rightItems, slideDistance, cursor * 0.5f);

            _introSequence.OnComplete(() =>
            {
                root.interactable = true;
                root.blocksRaycasts = true;
            });

            return _introSequence;
        }

        /// Slides the UI back out when Play is committed, clearing the frame for the kart exit.
        public Tween PlayOutro()
        {
            _introSequence?.Kill();
            CollapsePlayModes(true);

            root.interactable = false;
            root.blocksRaycasts = false;

            var outro = DOTween.Sequence();

            for (int i = 0; i < leftItems.Count; i++)
                AppendOutroItem(outro, leftItems[i], -slideDistance, i * itemStagger * 0.5f);

            for (int i = 0; i < rightItems.Count; i++)
                AppendOutroItem(outro, rightItems[i], slideDistance, i * itemStagger * 0.5f);

            outro.Insert(0.15f, root.DOFade(0f, 0.35f).SetEase(Ease.InQuad));
            return outro;
        }

        private float AppendColumn(List<RectTransform> items, float fromOffsetX, float startAt)
        {
            float cursor = startAt;

            foreach (var item in items)
            {
                if (item == null) continue;

                Vector2 rest = _restPositions[item];
                item.anchoredPosition = rest + new Vector2(fromOffsetX, 0f);

                _introSequence.Insert(cursor, item.DOAnchorPos(rest, itemDuration).SetEase(itemEase));
                cursor += itemStagger;
            }

            return cursor;
        }

        private void AppendOutroItem(Sequence sequence, RectTransform item, float toOffsetX, float at)
        {
            if (item == null) return;

            Vector2 rest = _restPositions[item];
            sequence.Insert(at, item.DOAnchorPos(rest + new Vector2(toOffsetX, 0f), 0.3f).SetEase(Ease.InBack));
        }

        private void TogglePlayModes()
        {
            if (_playModeExpanded) CollapsePlayModes(false);
            else ExpandPlayModes();
        }

        private void ExpandPlayModes()
        {
            if (playModeGroup == null) return;

            _playModeExpanded = true;
            _playModeSequence?.Kill();

            playModeGroup.gameObject.SetActive(true);
            playModeGroup.localScale = new Vector3(1f, 0f, 1f);

            _playModeSequence = DOTween.Sequence();
            _playModeSequence.Append(playModeGroup.DOScaleY(1f, 0.35f).SetEase(Ease.OutBack));

            if (playModeGroupCanvas != null)
            {
                playModeGroupCanvas.alpha = 0f;
                playModeGroupCanvas.blocksRaycasts = true;
                _playModeSequence.Join(playModeGroupCanvas.DOFade(1f, 0.25f));
            }
        }

        private void CollapsePlayModes(bool instant)
        {
            if (playModeGroup == null || !_playModeExpanded) return;

            _playModeExpanded = false;
            _playModeSequence?.Kill();

            if (playModeGroupCanvas != null) playModeGroupCanvas.blocksRaycasts = false;

            if (instant)
            {
                playModeGroup.gameObject.SetActive(false);
                return;
            }

            _playModeSequence = DOTween.Sequence();
            _playModeSequence.Append(playModeGroup.DOScaleY(0f, 0.22f).SetEase(Ease.InBack));

            if (playModeGroupCanvas != null)
                _playModeSequence.Join(playModeGroupCanvas.DOFade(0f, 0.18f));

            _playModeSequence.OnComplete(() => playModeGroup.gameObject.SetActive(false));
        }

        private void OpenPanel(MenuPanelType type)
        {
            if (panelRouter == null)
            {
                Debug.LogWarning("[MainMenuUi] No MenuPanelRouter assigned.");
                return;
            }

            CollapsePlayModes(false);
            panelRouter.Open(type);
        }

        private void RequestPlay(GameMode mode)
        {
            root.interactable = false;
            OnPlayRequested?.Invoke(mode);
        }

        private void CaptureRestPositions(List<RectTransform> items)
        {
            foreach (var item in items)
            {
                if (item == null || _restPositions.ContainsKey(item)) continue;
                _restPositions[item] = item.anchoredPosition;
            }
        }

        private void SetHiddenImmediate()
        {
            // Must be fully transparent from the first frame: the intro does not run until the
            // cinematic finishes, and anything visible before then reads as a pop-in glitch.
            root.alpha = 0f;
            root.interactable = false;
            root.blocksRaycasts = false;

            foreach (var kvp in _restPositions)
            {
                bool isLeft = leftItems.Contains(kvp.Key);
                float offset = isLeft ? -slideDistance : slideDistance;
                kvp.Key.anchoredPosition = kvp.Value + new Vector2(offset, 0f);
            }

            if (playModeGroup != null) playModeGroup.gameObject.SetActive(false);
            _playModeExpanded = false;
        }
    }
}
