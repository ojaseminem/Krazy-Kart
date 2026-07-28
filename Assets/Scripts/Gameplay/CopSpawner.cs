using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
    /// Spawns security karts in response to how much the player has wrecked on the current floor.
    /// Thresholds come from the floor definition, so B1 stays cop-free as a tutorial while the
    /// upper floors escalate.
    public class CopSpawner : MonoBehaviour
    {
        public static CopSpawner Instance { get; private set; }

        [SerializeField] private GameObject copPrefab;
        [SerializeField] private Transform player;
        [Tooltip("Hard cap on simultaneous cops, per the GDD.")]
        [SerializeField] private int maxActiveCops = 4;
        [Tooltip("Spawn points are skipped if the player is closer than this, to avoid pop-in.")]
        [SerializeField] private float minSpawnDistance = 18f;

        private readonly List<CopKart> _active = new List<CopKart>();
        private FloorDefinition _floor;
        private bool _firstDone, _secondDone, _formationDone;

        public int ActiveCount => _active.Count;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private bool _subscribed;
        private bool _started;

        // Subscribed in Start, not OnEnable: RunController.Instance is only guaranteed to exist
        // once every Awake has run, and missing the floor-changed event leaves cops disabled.
        private void Start()
        {
            Subscribe();

            if (RunController.Instance != null)
                HandleFloorEntered(RunController.Instance.CurrentFloor, RunController.Instance.FloorIndex);
        }

        private void OnEnable()
        {
            if (_started) Subscribe();
        }

        private void OnDisable() => Unsubscribe();

        private void Subscribe()
        {
            if (_subscribed) return;
            _subscribed = true;
            _started = true;

            RunStats.OnPropDestroyed += HandlePropDestroyed;
            if (RunController.Instance != null)
                RunController.Instance.OnFloorEntered += HandleFloorEntered;
        }

        private void Unsubscribe()
        {
            if (!_subscribed) return;
            _subscribed = false;

            RunStats.OnPropDestroyed -= HandlePropDestroyed;
            if (RunController.Instance != null)
                RunController.Instance.OnFloorEntered -= HandleFloorEntered;
        }

        private void OnDestroy()
        {
            Unsubscribe();
            if (Instance == this) Instance = null;
        }

        private void HandleFloorEntered(FloorDefinition floor, int index)
        {
            _floor = floor;
            _firstDone = _secondDone = _formationDone = false;
            ClearAll();
        }

        private void HandlePropDestroyed(int floorCount)
        {
            if (_floor == null || !_floor.CopsEnabled || copPrefab == null) return;

            if (!_firstDone && floorCount >= _floor.FirstCopAt)
            {
                _firstDone = true;
                Spawn(1);
            }
            else if (!_secondDone && floorCount >= _floor.SecondCopAt)
            {
                _secondDone = true;
                Spawn(1);
            }
            else if (!_formationDone && floorCount >= _floor.FormationAt)
            {
                _formationDone = true;
                Spawn(3);
            }
        }

        private void Spawn(int count)
        {
            var points = _floor.CopSpawnPoints;
            if (points == null || points.Length == 0) return;

            for (int i = 0; i < count; i++)
            {
                if (_active.Count >= maxActiveCops) return;

                Transform point = PickSpawnPoint(points);
                if (point == null) return;

                var go = Instantiate(copPrefab, point.position, point.rotation);
                var cop = go.GetComponent<CopKart>();
                if (cop == null)
                {
                    Destroy(go);
                    continue;
                }

                cop.Initialise(player);
                _active.Add(cop);
            }
        }

        /// Prefers a point far enough away that the cop does not appear in front of the player.
        private Transform PickSpawnPoint(Transform[] points)
        {
            Transform best = null;
            float bestDistance = -1f;

            for (int i = 0; i < points.Length; i++)
            {
                if (points[i] == null) continue;

                float d = player != null
                    ? Vector3.Distance(points[i].position, player.position)
                    : float.MaxValue;

                if (d >= minSpawnDistance) return points[i];
                if (d > bestDistance)
                {
                    bestDistance = d;
                    best = points[i];
                }
            }

            return best;
        }

        public void NotifyDespawned(CopKart cop) => _active.Remove(cop);

        private void ClearAll()
        {
            for (int i = _active.Count - 1; i >= 0; i--)
                if (_active[i] != null) Destroy(_active[i].gameObject);

            _active.Clear();
        }
    }
}
