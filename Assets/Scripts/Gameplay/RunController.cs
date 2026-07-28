using System;
using System.Collections;
using System.Collections.Generic;
using Flow;
using UnityEngine;

namespace Gameplay
{
    public enum RunState
    {
        Starting,
        Climbing,
        BossEscape,
        Finished,
        Failed
    }

    /// Drives one full run: B1 at the bottom, up through every floor, into the boss escape.
    ///
    /// Floors are stacked in a single scene rather than streamed additively — the mall is one
    /// continuous vertical space, so this keeps the ascent seamless and removes a whole class of
    /// load-timing bugs. Only floors near the player stay active.
    public class RunController : MonoBehaviour
    {
        public static RunController Instance { get; private set; }

        [Header("Floors, bottom to top")]
        [SerializeField] private List<FloorDefinition> floors = new List<FloorDefinition>();

        [Header("Player")]
        [SerializeField] private Transform playerRig;
        [SerializeField] private Kart.KartController kart;
        [Tooltip("Chase camera transform, moved alongside the kart on teleports.")]
        [SerializeField] private Transform playerCamera;

        [Header("Activation")]
        [Tooltip("Floors above the current one kept active so the next space is already warm.")]
        [SerializeField] private int lookAhead = 1;
        [Tooltip("Floors below kept active so the player can still see where they came from.")]
        [SerializeField] private int lookBehind = 1;

        [Header("Timing")]
        [SerializeField] private float startDelay = 0.6f;

        [Header("Safety Net")]
        [Tooltip("How far below the current floor the kart may fall before being recovered.")]
        [SerializeField] private float fallLimit = 14f;
        [Tooltip("Height of one floor, used to locate the current floor's plane.")]
        [SerializeField] private float floorHeight = 10f;

        public RunState State { get; private set; } = RunState.Starting;
        public int FloorIndex { get; private set; }
        public FloorDefinition CurrentFloor =>
            FloorIndex >= 0 && FloorIndex < floors.Count ? floors[FloorIndex] : null;

        public event Action<FloorDefinition, int> OnFloorEntered;
        public event Action<RunState> OnStateChanged;

        private bool _cleanCorridor = true;
        private float _lastAdvanceTime = -999f;
        private const float advanceCooldown = 1.5f;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        private IEnumerator Start()
        {
            RunStats.ResetRun();

            // Hold the kart still until the run formally begins.
            if (kart != null) kart.canMove = false;

            EnterFloor(0, teleport: true);

            yield return new WaitForSeconds(startDelay);

            SetState(RunState.Climbing);
            if (kart != null) kart.canMove = true;
        }

        private void Update()
        {
            if (State != RunState.Climbing && State != RunState.BossEscape) return;

            RunStats.RunTime += Time.deltaTime;
            CheckFall();
        }

        /// Driving off the open shaft edge should cost momentum, not end the run — put the
        /// player back at the current floor's entry rather than letting them fall forever.
        private void CheckFall()
        {
            if (kart == null || CurrentFloor == null) return;

            float floorPlane = FloorIndex * floorHeight;
            if (kart.transform.position.y > floorPlane - fallLimit) return;

            PlacePlayer(CurrentFloor.EntryPoint);
        }

        public void AdvanceFloor(bool awardCleanCorridor)
        {
            if (State != RunState.Climbing) return;

            // A respawn or a wide trigger can briefly overlap two volumes; without this a single
            // arrival could chain several floors at once.
            if (Time.time - _lastAdvanceTime < advanceCooldown) return;
            _lastAdvanceTime = Time.time;

            if (awardCleanCorridor && _cleanCorridor && McManager.Instance != null)
            {
                McManager.Instance.Award(
                    McManager.Instance.Config.cleanCorridorBonus,
                    McSource.CleanCorridor,
                    playerRig != null ? playerRig.position : transform.position);
            }

            int next = FloorIndex + 1;
            if (next >= floors.Count)
            {
                BeginBossEscape();
                return;
            }

            EnterFloor(next, teleport: false);
        }

        /// Moves the run to a floor. teleport is used for the initial spawn only; during play the
        /// player physically drives up the ramps, so their transform is left alone.
        public void EnterFloor(int index, bool teleport)
        {
            if (index < 0 || index >= floors.Count) return;

            FloorIndex = index;
            var floor = floors[index];

            RunStats.ResetFloor();
            McManager.Instance?.ResetFloorSubtotal();
            _cleanCorridor = true;

            RefreshActiveFloors();

            if (teleport && floor.EntryPoint != null && playerRig != null)
                PlacePlayer(floor.EntryPoint);

            // Re-arm this floor's exits so a replayed floor can still be completed.
            foreach (var exit in floor.GetComponentsInChildren<FloorExitTrigger>(true))
                exit.Rearm();

            OnFloorEntered?.Invoke(floor, index);
        }

        /// Teleports the kart. The Rigidbody must be moved directly: it is a child of the rig and
        /// owns its own position, so moving the rig root alone leaves the body behind and it
        /// falls out of the world.
        private void PlacePlayer(Transform point)
        {
            if (kart == null) return;

            var body = kart.GetComponent<Rigidbody>();
            if (body != null)
            {
                body.linearVelocity = Vector3.zero;
                body.angularVelocity = Vector3.zero;
                body.position = point.position;
                body.rotation = point.rotation;
            }

            kart.transform.SetPositionAndRotation(point.position, point.rotation);

            // Bring the chase camera with it so it does not have to sweep across the mall.
            if (playerCamera != null)
            {
                playerCamera.position = point.position - point.forward * 5f + Vector3.up * 2.2f;
                playerCamera.rotation = Quaternion.LookRotation(point.forward, Vector3.up);
            }

            Physics.SyncTransforms();
        }

        private void RefreshActiveFloors()
        {
            for (int i = 0; i < floors.Count; i++)
            {
                if (floors[i] == null) continue;

                bool active = i >= FloorIndex - lookBehind && i <= FloorIndex + lookAhead;
                if (floors[i].gameObject.activeSelf != active)
                    floors[i].gameObject.SetActive(active);
            }
        }

        /// Called when the player scrapes a wall — cancels the clean-corridor bonus.
        public void NotifyWallContact() => _cleanCorridor = false;

        private void BeginBossEscape()
        {
            SetState(RunState.BossEscape);
            BossSequence.Instance?.Begin();
        }

        public void CompleteRun()
        {
            if (State == RunState.Finished) return;

            SetState(RunState.Finished);
            if (kart != null) kart.canMove = false;
        }

        public void FailRun()
        {
            if (State == RunState.Failed) return;

            SetState(RunState.Failed);
            if (kart != null) kart.canMove = false;
        }

        public void ReturnToMenu()
        {
            SceneFlow.Instance.TransitionTo("MenuScene", null);
        }

        private void SetState(RunState state)
        {
            if (State == state) return;

            State = state;
            OnStateChanged?.Invoke(state);
        }
    }
}
