using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Gameplay
{
    /// The manager's office escape. Total MC earned across the run is converted into escape
    /// seconds, so a destructive run buys a forgiving timer and a speedrun buys a brutal one —
    /// the player picks their own difficulty by how they played.
    ///
    /// Four phases fire as the clock burns down, matching the GDD beats.
    public class BossSequence : MonoBehaviour
    {
        public static BossSequence Instance { get; private set; }

        [Header("Phase Objects")]
        [Tooltip("Shutters that slam down at phase 2.")]
        [SerializeField] private Transform[] shutters;
        [SerializeField] private float shutterDropHeight = 4.2f;
        [Tooltip("Bollards that rise at phase 2.")]
        [SerializeField] private Transform[] bollards;
        [SerializeField] private float bollardRiseHeight = 1.4f;
        [Tooltip("Cleaning robots enabled at phase 3.")]
        [SerializeField] private GameObject[] cleaningRobots;
        [Tooltip("Glass runway revealed at phase 4.")]
        [SerializeField] private GameObject glassRunway;

        [Header("Lighting")]
        [SerializeField] private Light[] moodLights;
        [SerializeField] private Color alarmColor = new Color(1f, 0.18f, 0.12f);

        [Header("Exit")]
        [SerializeField] private Transform exitTrigger;
        [SerializeField] private float slowMoScale = 0.25f;
        [SerializeField] private float slowMoHold = 1.6f;

        [Header("Phase Fractions")]
        [Tooltip("Fraction of remaining time at which each phase begins.")]
        [SerializeField] private float phase2At = 0.72f;
        [SerializeField] private float phase3At = 0.45f;
        [SerializeField] private float phase4At = 0.2f;

        public bool IsRunning { get; private set; }
        public float TimeRemaining { get; private set; }
        public float TotalTime { get; private set; }
        public int Phase { get; private set; }

        public event Action<float, float> OnTimerTick;
        public event Action<int> OnPhaseChanged;

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

        public void Begin()
        {
            if (IsRunning) return;

            TotalTime = McManager.Instance != null ? McManager.Instance.EscapeSeconds : 12f;
            TimeRemaining = TotalTime;
            IsRunning = true;
            Phase = 1;
            OnPhaseChanged?.Invoke(Phase);

            SetPhaseObjectsToStart();
            StartCoroutine(TimerRoutine());
        }

        private void SetPhaseObjectsToStart()
        {
            foreach (var robot in cleaningRobots)
                if (robot != null) robot.SetActive(false);

            if (glassRunway != null) glassRunway.SetActive(false);
        }

        private IEnumerator TimerRoutine()
        {
            while (TimeRemaining > 0f && IsRunning)
            {
                TimeRemaining -= Time.deltaTime;
                OnTimerTick?.Invoke(Mathf.Max(0f, TimeRemaining), TotalTime);

                float fraction = TimeRemaining / TotalTime;
                if (Phase < 2 && fraction <= phase2At) EnterPhase2();
                else if (Phase < 3 && fraction <= phase3At) EnterPhase3();
                else if (Phase < 4 && fraction <= phase4At) EnterPhase4();

                yield return null;
            }

            if (IsRunning) Fail();
        }

        private void EnterPhase2()
        {
            Phase = 2;
            OnPhaseChanged?.Invoke(Phase);

            foreach (var shutter in shutters)
            {
                if (shutter == null) continue;
                shutter.DOKill();
                shutter.DOMoveY(shutter.position.y - shutterDropHeight, 0.85f).SetEase(Ease.InQuad);
            }

            foreach (var bollard in bollards)
            {
                if (bollard == null) continue;
                bollard.DOKill();
                bollard.DOMoveY(bollard.position.y + bollardRiseHeight, 0.5f).SetEase(Ease.OutBack);
            }
        }

        private void EnterPhase3()
        {
            Phase = 3;
            OnPhaseChanged?.Invoke(Phase);

            foreach (var robot in cleaningRobots)
                if (robot != null) robot.SetActive(true);

            // Mood shift: the office turns hostile.
            foreach (var light in moodLights)
            {
                if (light == null) continue;
                light.DOKill();
                light.DOColor(alarmColor, 0.9f);
                light.DOIntensity(Mathf.Max(0.4f, light.intensity * 0.75f), 0.9f);
            }
        }

        private void EnterPhase4()
        {
            Phase = 4;
            OnPhaseChanged?.Invoke(Phase);

            if (glassRunway != null) glassRunway.SetActive(true);
        }

        /// Called by the glass wall trigger when the player punches through at speed.
        public void TriggerEscape()
        {
            if (!IsRunning) return;

            IsRunning = false;
            StartCoroutine(EscapeRoutine());
        }

        private IEnumerator EscapeRoutine()
        {
            Time.timeScale = slowMoScale;
            Time.fixedDeltaTime = 0.02f * slowMoScale;

            yield return new WaitForSecondsRealtime(slowMoHold);

            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f;

            RunController.Instance?.CompleteRun();
        }

        private void Fail()
        {
            IsRunning = false;
            RunController.Instance?.FailRun();
        }

        private void OnDisable()
        {
            // Never leave the game in slow motion if this object goes away mid-sequence.
            if (Time.timeScale == slowMoScale)
            {
                Time.timeScale = 1f;
                Time.fixedDeltaTime = 0.02f;
            }
        }
    }
}
