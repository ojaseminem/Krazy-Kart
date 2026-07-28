using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
    /// Rides on the player kart and pays out the three "style" MC sources: sustained drifting,
    /// near-misses against nearby geometry, and airtime stunts.
    ///
    /// Kept entirely read-only with respect to the kart — it samples state, never drives it, so
    /// the finalised handling is untouched.
    [RequireComponent(typeof(Rigidbody))]
    public class StyleScorer : MonoBehaviour
    {
        [SerializeField] private ScoringConfig config;
        [SerializeField] private Kart.KartController kart;
        [Tooltip("Layers counted as ground for the airborne check.")]
        [SerializeField] private LayerMask groundMask = ~0;
        [Tooltip("Layers considered for near-miss scoring.")]
        [SerializeField] private LayerMask nearMissMask = ~0;
        [SerializeField] private float groundCheckDistance = 1.1f;

        private Rigidbody _body;
        private float _driftAccumulator;
        private float _airTime;
        private bool _wasAirborne;
        private readonly Dictionary<Collider, float> _nearMissCooldowns = new Dictionary<Collider, float>();
        private readonly List<Collider> _expired = new List<Collider>();
        private static readonly Collider[] NearHits = new Collider[24];

        public bool IsAirborne { get; private set; }
        public float CurrentAirTime => _airTime;

        private void Awake()
        {
            _body = GetComponent<Rigidbody>();
            if (kart == null) kart = GetComponent<Kart.KartController>();
            if (config == null && McManager.Instance != null) config = McManager.Instance.Config;
        }

        private void Update()
        {
            if (config == null)
            {
                if (McManager.Instance == null) return;
                config = McManager.Instance.Config;
                if (config == null) return;
            }

            TickCooldowns();
            TickAirborne();
            TickDrift();
        }

        private void TickCooldowns()
        {
            if (_nearMissCooldowns.Count == 0) return;

            _expired.Clear();
            var keys = new List<Collider>(_nearMissCooldowns.Keys);
            foreach (var key in keys)
            {
                float t = _nearMissCooldowns[key] - Time.deltaTime;
                if (t <= 0f || key == null) _expired.Add(key);
                else _nearMissCooldowns[key] = t;
            }

            foreach (var key in _expired) _nearMissCooldowns.Remove(key);
        }

        private void TickAirborne()
        {
            IsAirborne = !Physics.Raycast(transform.position + Vector3.up * 0.2f, Vector3.down,
                groundCheckDistance, groundMask, QueryTriggerInteraction.Ignore);

            if (IsAirborne)
            {
                _airTime += Time.deltaTime;
                _wasAirborne = true;
                return;
            }

            if (!_wasAirborne) return;
            _wasAirborne = false;

            // Landed — pay the stunt if the hang time earned it.
            if (_airTime >= config.minAirTime && McManager.Instance != null)
            {
                int amount = Mathf.Min(
                    config.stuntMcCap,
                    config.stuntBaseMc + Mathf.RoundToInt(_airTime * config.stuntMcPerSecond));
                McManager.Instance.Award(amount, McSource.Stunt, transform.position);
            }

            _airTime = 0f;
        }

        private void TickDrift()
        {
            if (kart == null || McManager.Instance == null) return;

            float speed = kart.SpeedKph;
            bool drifting = kart.IsDrifting && speed >= config.driftMinSpeedKph && !IsAirborne;
            if (!drifting)
            {
                _driftAccumulator = 0f;
                return;
            }

            // Pay drift MC in whole points as the accumulator crosses 1.
            _driftAccumulator += config.driftMcPerSecond * Time.deltaTime;
            if (_driftAccumulator >= 1f)
            {
                int whole = Mathf.FloorToInt(_driftAccumulator);
                _driftAccumulator -= whole;
                McManager.Instance.Award(whole, McSource.Drift, transform.position);
            }

            ScanNearMisses();
        }

        /// While drifting, brushing past intact geometry pays a one-off bonus per object.
        private void ScanNearMisses()
        {
            int count = Physics.OverlapSphereNonAlloc(
                transform.position, config.nearMissRadius, NearHits, nearMissMask,
                QueryTriggerInteraction.Ignore);

            for (int i = 0; i < count; i++)
            {
                var hit = NearHits[i];
                if (hit == null) continue;
                if (hit.transform.IsChildOf(transform)) continue;
                if (_nearMissCooldowns.ContainsKey(hit)) continue;

                var destructible = hit.GetComponentInParent<DestructibleProp>();
                if (destructible == null || destructible.IsBroken) continue;

                _nearMissCooldowns[hit] = config.nearMissCooldown;
                McManager.Instance.Award(config.nearMissBonus, McSource.NearMiss, hit.transform.position);
            }
        }
    }
}
