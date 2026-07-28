using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay
{
    /// Run HUD: MC total, combo multiplier, floor banner, and the boss countdown that takes over
    /// the screen during the escape. All motion is DOTween so the feedback stays punchy.
    public class GameplayHud : MonoBehaviour
    {
        [Header("MC")]
        [SerializeField] private TMP_Text mcLabel;
        [SerializeField] private RectTransform mcRoot;

        [Header("Combo")]
        [SerializeField] private CanvasGroup comboGroup;
        [SerializeField] private TMP_Text comboLabel;
        [SerializeField] private Image comboFill;

        [Header("Floor Banner")]
        [SerializeField] private CanvasGroup bannerGroup;
        [SerializeField] private TMP_Text bannerLabel;
        [SerializeField] private RectTransform bannerRoot;

        [Header("Boss Countdown")]
        [SerializeField] private CanvasGroup bossGroup;
        [SerializeField] private TMP_Text bossTimerLabel;
        [SerializeField] private Image bossFill;
        [SerializeField] private Color bossSafeColor = new Color(1f, 0.76f, 0.24f);
        [SerializeField] private Color bossDangerColor = new Color(1f, 0.24f, 0.2f);

        [Header("Popups")]
        [SerializeField] private RectTransform popupLayer;
        [SerializeField] private McPopup popupPrefab;
        [SerializeField] private int popupPoolSize = 12;

        [Header("Result")]
        [SerializeField] private CanvasGroup resultGroup;
        [SerializeField] private TMP_Text resultTitle;
        [SerializeField] private TMP_Text resultBody;

        private readonly List<McPopup> _pool = new List<McPopup>();
        private int _displayedMc;
        private Tween _mcTween;
        private Camera _camera;
        private bool _bossSubscribed;
        private bool _subscribed;
        private bool _started;

        private static readonly Dictionary<McSource, Color> SourceColors = new Dictionary<McSource, Color>
        {
            { McSource.PropDestroyed, new Color(1f, 0.82f, 0.35f) },
            { McSource.ChainBonus,    new Color(1f, 0.45f, 0.15f) },
            { McSource.Drift,         new Color(0.45f, 0.85f, 1f) },
            { McSource.NearMiss,      new Color(0.65f, 1f, 0.7f) },
            { McSource.Stunt,         new Color(0.85f, 0.55f, 1f) },
            { McSource.NpcHit,        new Color(1f, 0.7f, 0.8f) },
            { McSource.CopEvaded,     new Color(0.5f, 1f, 0.85f) },
            { McSource.CopKnocked,    new Color(1f, 0.35f, 0.4f) },
            { McSource.CleanCorridor, new Color(1f, 1f, 0.7f) }
        };

        private void Awake()
        {
            _camera = Camera.main;
            BuildPool();

            if (bossGroup != null) bossGroup.alpha = 0f;
            if (bannerGroup != null) bannerGroup.alpha = 0f;
            if (comboGroup != null) comboGroup.alpha = 0f;
            if (resultGroup != null)
            {
                resultGroup.alpha = 0f;
                resultGroup.blocksRaycasts = false;
            }
        }

        // Subscribing in Start rather than OnEnable: every manager's Awake has run by then, so
        // the singletons are guaranteed to exist regardless of scene ordering.
        private void Start() => Subscribe();

        private void OnEnable()
        {
            if (_started) Subscribe();
        }

        private void Subscribe()
        {
            if (_subscribed) return;
            _subscribed = true;
            _started = true;

            if (McManager.Instance != null)
            {
                McManager.Instance.OnMcAwarded += HandleMcAwarded;
                McManager.Instance.OnTotalChanged += HandleTotalChanged;
                McManager.Instance.OnMultiplierChanged += HandleMultiplierChanged;
            }

            if (RunController.Instance != null)
            {
                RunController.Instance.OnFloorEntered += HandleFloorEntered;
                RunController.Instance.OnStateChanged += HandleStateChanged;
            }

            TrySubscribeBoss();
        }

        /// The boss floor starts deactivated, so BossSequence.Instance does not exist when the
        /// HUD wakes. Subscription is retried once the escape actually begins.
        private void TrySubscribeBoss()
        {
            if (_bossSubscribed || BossSequence.Instance == null) return;

            BossSequence.Instance.OnTimerTick += HandleBossTick;
            _bossSubscribed = true;
        }

        private void OnDisable() => Unsubscribe();

        private void Unsubscribe()
        {
            if (!_subscribed) return;
            _subscribed = false;

            if (McManager.Instance != null)
            {
                McManager.Instance.OnMcAwarded -= HandleMcAwarded;
                McManager.Instance.OnTotalChanged -= HandleTotalChanged;
                McManager.Instance.OnMultiplierChanged -= HandleMultiplierChanged;
            }

            if (RunController.Instance != null)
            {
                RunController.Instance.OnFloorEntered -= HandleFloorEntered;
                RunController.Instance.OnStateChanged -= HandleStateChanged;
            }

            if (BossSequence.Instance != null && _bossSubscribed)
            {
                BossSequence.Instance.OnTimerTick -= HandleBossTick;
                _bossSubscribed = false;
            }
        }

        private void BuildPool()
        {
            if (popupPrefab == null || popupLayer == null) return;

            for (int i = 0; i < popupPoolSize; i++)
            {
                var popup = Instantiate(popupPrefab, popupLayer);
                popup.gameObject.SetActive(false);
                _pool.Add(popup);
            }
        }

        private McPopup GetPopup()
        {
            for (int i = 0; i < _pool.Count; i++)
                if (!_pool[i].gameObject.activeSelf) return _pool[i];

            return _pool.Count > 0 ? _pool[0] : null;
        }

        private void HandleMcAwarded(int amount, McSource source, Vector3 worldPos)
        {
            var popup = GetPopup();
            if (popup == null || _camera == null) return;

            Vector3 screen = _camera.WorldToScreenPoint(worldPos);
            if (screen.z < 0f) return;

            Vector2 local;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                popupLayer, screen, null, out local);

            Color color = SourceColors.TryGetValue(source, out var c) ? c : Color.white;
            string prefix = source == McSource.ChainBonus ? "CHAIN +" : "+";
            popup.Play(prefix + amount, color, local);
        }

        private void HandleTotalChanged(int total)
        {
            _mcTween?.Kill();

            // This DOTween build has no DOVirtual.Int, so count up in float and round for display.
            float from = _displayedMc;
            _mcTween = DOVirtual.Float(from, total, 0.35f, v =>
            {
                _displayedMc = Mathf.RoundToInt(v);
                if (mcLabel != null) mcLabel.text = _displayedMc.ToString("N0");
            }).SetEase(Ease.OutCubic);

            if (mcRoot != null)
            {
                mcRoot.DOKill(true);
                mcRoot.DOPunchScale(Vector3.one * 0.07f, 0.22f, 6, 0.7f);
            }
        }

        private void HandleMultiplierChanged(float multiplier)
        {
            if (comboGroup == null) return;

            bool visible = multiplier > 1f;
            comboGroup.DOKill();
            comboGroup.DOFade(visible ? 1f : 0f, 0.2f);

            if (comboLabel != null) comboLabel.text = "x" + multiplier.ToString("0.00");

            var cfg = McManager.Instance != null ? McManager.Instance.Config : null;
            if (comboFill != null && cfg != null)
                comboFill.fillAmount = Mathf.InverseLerp(1f, cfg.comboMax, multiplier);

            if (visible && comboLabel != null)
            {
                comboLabel.transform.DOKill(true);
                comboLabel.transform.DOPunchScale(Vector3.one * 0.12f, 0.2f, 6, 0.8f);
            }
        }

        private void HandleFloorEntered(FloorDefinition floor, int index)
        {
            if (bannerGroup == null || floor == null) return;

            if (bannerLabel != null) bannerLabel.text = floor.DisplayName.ToUpperInvariant();

            bannerGroup.DOKill();
            bannerRoot.DOKill();

            bannerRoot.anchoredPosition = new Vector2(-60f, bannerRoot.anchoredPosition.y);
            bannerGroup.alpha = 0f;

            var seq = DOTween.Sequence();
            seq.Append(bannerGroup.DOFade(1f, 0.3f));
            seq.Join(bannerRoot.DOAnchorPosX(0f, 0.45f).SetEase(Ease.OutBack));
            seq.AppendInterval(1.9f);
            seq.Append(bannerGroup.DOFade(0f, 0.4f));
        }

        private void HandleStateChanged(RunState state)
        {
            if (state == RunState.BossEscape)
            {
                TrySubscribeBoss();

                if (bossGroup != null)
                {
                    bossGroup.DOKill();
                    bossGroup.DOFade(1f, 0.4f);
                }
            }

            if (state == RunState.Finished) ShowResult("ESCAPED!", true);
            else if (state == RunState.Failed) ShowResult("CAUGHT!", false);
        }

        private void HandleBossTick(float remaining, float total)
        {
            if (bossTimerLabel != null)
                bossTimerLabel.text = remaining.ToString("00.00");

            float fraction = total > 0f ? remaining / total : 0f;

            if (bossFill != null)
            {
                bossFill.fillAmount = fraction;
                bossFill.color = Color.Lerp(bossDangerColor, bossSafeColor, fraction);
            }

            // Pulse the clock once things get genuinely tight.
            if (fraction < 0.25f && bossTimerLabel != null && !DOTween.IsTweening(bossTimerLabel.transform))
            {
                bossTimerLabel.transform
                    .DOPunchScale(Vector3.one * 0.1f, 0.4f, 4, 0.6f)
                    .SetLoops(2);
            }
        }

        private void ShowResult(string title, bool success)
        {
            if (resultGroup == null) return;

            if (bossGroup != null) bossGroup.DOFade(0f, 0.3f);

            if (resultTitle != null)
            {
                resultTitle.text = title;
                resultTitle.color = success ? bossSafeColor : bossDangerColor;
            }

            if (resultBody != null)
            {
                int mc = McManager.Instance != null ? McManager.Instance.TotalMc : 0;
                resultBody.text =
                    $"MALL CREDIT   {mc:N0}\n" +
                    $"PROPS WRECKED   {RunStats.PropsDestroyed}\n" +
                    $"COPS EVADED   {RunStats.CopsEvaded}\n" +
                    $"TIME   {RunStats.RunTime:0.00}s";
            }

            resultGroup.blocksRaycasts = true;
            resultGroup.DOKill();
            resultGroup.DOFade(1f, 0.5f).SetUpdate(true);
        }
    }
}
