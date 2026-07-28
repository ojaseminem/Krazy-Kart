using System;
using UnityEngine;

namespace Gameplay
{
    /// Owns the run's Mall Credit total, the combo multiplier, and the chain-destruction window.
    /// Every scoring source funnels through Award() so the multiplier and HUD stay consistent.
    public class McManager : MonoBehaviour
    {
        public static McManager Instance { get; private set; }

        [SerializeField] private ScoringConfig config;

        public ScoringConfig Config => config;

        public int TotalMc { get; private set; }
        public int FloorMc { get; private set; }
        public float Multiplier { get; private set; } = 1f;
        public int ChainLength { get; private set; }

        /// (amount after multiplier, source, world position for the popup)
        public event Action<int, McSource, Vector3> OnMcAwarded;
        public event Action<float> OnMultiplierChanged;
        public event Action<int> OnTotalChanged;
        /// Fired when a chain closes with enough links to pay a bonus.
        public event Action<int, Vector3> OnChainCompleted;

        private float _comboTimer;
        private float _chainTimer;
        private Vector3 _lastChainPos;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            if (config == null)
                Debug.LogError("[McManager] No ScoringConfig assigned.");
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        private void Update()
        {
            if (config == null) return;

            TickCombo();
            TickChain();
        }

        /// Adds MC from a source. Base amount is scaled by the live combo multiplier.
        public void Award(int baseAmount, McSource source, Vector3 worldPos)
        {
            if (baseAmount <= 0) return;

            int amount = Mathf.RoundToInt(baseAmount * Multiplier);
            TotalMc += amount;
            FloorMc += amount;

            BumpCombo();

            OnMcAwarded?.Invoke(amount, source, worldPos);
            OnTotalChanged?.Invoke(TotalMc);
        }

        /// Called by each destructible as it breaks so chains can be detected.
        public void RegisterDestruction(Vector3 worldPos)
        {
            ChainLength++;
            _chainTimer = config != null ? config.chainWindow : 0.8f;
            _lastChainPos = worldPos;
        }

        public void ResetFloorSubtotal() => FloorMc = 0;

        public float EscapeSeconds => config != null ? config.EscapeSecondsFor(TotalMc) : 12f;

        private void BumpCombo()
        {
            _comboTimer = config.comboTimeout;
            Multiplier = Mathf.Min(config.comboMax, Multiplier + config.comboStep);
            OnMultiplierChanged?.Invoke(Multiplier);
        }

        private void TickCombo()
        {
            if (Multiplier <= 1f) return;

            _comboTimer -= Time.deltaTime;
            if (_comboTimer > 0f) return;

            Multiplier = 1f;
            OnMultiplierChanged?.Invoke(Multiplier);
        }

        private void TickChain()
        {
            if (ChainLength <= 0) return;

            _chainTimer -= Time.deltaTime;
            if (_chainTimer > 0f) return;

            // Window closed — pay out if the chain was long enough, then reset.
            if (ChainLength >= config.chainMinLength)
            {
                int extra = ChainLength - (config.chainMinLength - 1);
                int bonus = Mathf.Min(config.chainBonusCap, extra * config.chainBonusPerProp);
                Award(bonus, McSource.ChainBonus, _lastChainPos);
                OnChainCompleted?.Invoke(ChainLength, _lastChainPos);
            }

            ChainLength = 0;
        }
    }
}
