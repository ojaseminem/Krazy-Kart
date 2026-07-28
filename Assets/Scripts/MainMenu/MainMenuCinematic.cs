using DG.Tweening;
using UnityEngine;

namespace MainMenu
{
    /// The menu's 3D layer, staged as a short film rather than a single move.
    ///
    /// ENTER — four beats: the kart hurtles in wide-angle, clips the shelving (debris + shake),
    ///         drifts sideways into its hero pose, then the camera settles and breathes.
    /// EXIT  — on Play the kart squats, the lens widens, and it drives at and past the camera.
    public class MainMenuCinematic : MonoBehaviour
    {
        [Header("Kart")]
        [SerializeField] private Transform kart;
        [SerializeField] private Transform entryPoint;
        [SerializeField] private Transform heroPoint;
        [SerializeField] private Transform exitPoint;

        [Header("Camera")]
        [SerializeField] private Transform cameraRig;
        [SerializeField] private Transform cameraEntryPoint;
        [SerializeField] private Transform cameraHeroPoint;
        [Tooltip("The Camera itself, nested under the rig. Shake and drift go here so they " +
                 "never fight the rig's own positional tween.")]
        [SerializeField] private Transform cameraShaker;
        [SerializeField] private Camera cam;

        [Header("Lens")]
        [Tooltip("Wide during the charge-in — exaggerates speed.")]
        [SerializeField] private float entryFov = 62f;
        [SerializeField] private float heroFov = 41f;
        [SerializeField] private float exitFov = 72f;

        [Header("Set Dressing")]
        [SerializeField] private MenuDebrisBurst debris;
        [SerializeField] private ParticleSystem driftSmoke;
        [SerializeField] private ParticleSystem sparks;

        [Header("Enter Timing")]
        [Tooltip("The fast charge from off-screen to the impact point.")]
        [SerializeField] private float chargeDuration = 1.15f;
        [Tooltip("The sideways drift from impact into the hero pose.")]
        [SerializeField] private float settleDuration = 1.35f;
        [Tooltip("Yaw overshoot at the peak of the drift, in degrees.")]
        [SerializeField] private float driftYaw = 46f;

        [Header("Impact")]
        [SerializeField] private float shakeStrength = 0.42f;
        [SerializeField] private float shakeDuration = 0.7f;
        [SerializeField] private int shakeVibrato = 14;

        [Header("Exit Timing")]
        [SerializeField] private float launchWindUp = 0.32f;
        [SerializeField] private float driveByDuration = 0.85f;

        [Header("Idle")]
        [SerializeField] private float idleBobHeight = 0.07f;
        [SerializeField] private float idleBobDuration = 2.4f;
        [Tooltip("Slow lateral camera drift so the hero shot never sits perfectly still.")]
        [SerializeField] private float idleDriftX = 0.16f;
        [SerializeField] private float idleDriftDuration = 5f;

        private Sequence _sequence;
        private Tween _idleBob;
        private Tween _idleDrift;
        private Vector3 _shakerRest;

        private void Awake()
        {
            if (cam == null && cameraShaker != null) cam = cameraShaker.GetComponent<Camera>();
            if (cameraShaker != null) _shakerRest = cameraShaker.localPosition;
        }

        private void OnDestroy()
        {
            _sequence?.Kill();
            _idleBob?.Kill();
            _idleDrift?.Kill();
        }

        /// Completes when the kart reaches its hero pose — the UI intro rides off the back of this.
        public Tween PlayEnter()
        {
            KillAll();
            ResetPose();

            PlayEffect(driftSmoke);

            // Impact lands where the charge ends and the drift begins.
            float impactAt = chargeDuration;
            Vector3 impactPos = heroPoint != null && entryPoint != null
                ? Vector3.Lerp(entryPoint.position, heroPoint.position, 0.62f)
                : Vector3.zero;

            _sequence = DOTween.Sequence();

            if (kart != null && entryPoint != null && heroPoint != null)
            {
                // Beat 1 — the charge. Linear-ish so it reads as momentum, not easing in.
                _sequence.Append(kart.DOMove(impactPos, chargeDuration).SetEase(Ease.InOutSine));
                _sequence.Join(kart.DORotate(
                    heroPoint.eulerAngles + new Vector3(0f, driftYaw, 0f),
                    chargeDuration).SetEase(Ease.InQuad));

                // Beat 3 — the drift into frame, decelerating hard.
                _sequence.Append(kart.DOMove(heroPoint.position, settleDuration).SetEase(Ease.OutQuint));
                _sequence.Join(kart.DORotate(heroPoint.eulerAngles, settleDuration).SetEase(Ease.OutBack));
            }

            // Camera travels the whole time, arriving slightly after the kart does.
            float camDuration = chargeDuration + settleDuration + 0.45f;
            if (cameraRig != null && cameraHeroPoint != null)
            {
                _sequence.Insert(0f, cameraRig.DOMove(cameraHeroPoint.position, camDuration).SetEase(Ease.InOutQuart));
                _sequence.Insert(0f, cameraRig.DORotateQuaternion(cameraHeroPoint.rotation, camDuration).SetEase(Ease.InOutQuart));
            }

            if (cam != null)
            {
                cam.fieldOfView = entryFov;
                _sequence.Insert(0f, cam.DOFieldOfView(heroFov, camDuration).SetEase(Ease.InOutQuart));
                // Beat 2 — a short lens punch on contact.
                _sequence.Insert(impactAt, cam.DOFieldOfView(entryFov + 5f, 0.12f).SetEase(Ease.OutQuad));
                _sequence.Insert(impactAt + 0.12f, cam.DOFieldOfView(heroFov, camDuration - impactAt).SetEase(Ease.OutQuart));
            }

            // Beat 2 — the hit: debris topples, camera shakes, sparks fly.
            _sequence.InsertCallback(impactAt, () =>
            {
                if (debris != null) debris.Burst();
                PlayEffect(sparks);
                if (cameraShaker != null)
                {
                    cameraShaker.DOKill();
                    cameraShaker.localPosition = _shakerRest;
                    cameraShaker.DOShakePosition(shakeDuration, shakeStrength, shakeVibrato, 90f, false, true);
                }
            });

            // Beat 4 — settle.
            _sequence.OnComplete(() =>
            {
                StopEffect(driftSmoke);
                StartIdle();
            });

            return _sequence;
        }

        /// Exit: squat, lens widens, kart drives at and past the lens.
        public Tween PlayExit()
        {
            KillAll();
            PlayEffect(driftSmoke);

            _sequence = DOTween.Sequence();
            if (kart == null || exitPoint == null) return _sequence;

            // Rock back so the launch has weight.
            _sequence.Append(kart.DOMove(kart.position - kart.forward * 0.55f, launchWindUp).SetEase(Ease.OutQuad));
            if (heroPoint != null)
                _sequence.Join(kart.DORotate(heroPoint.eulerAngles, launchWindUp).SetEase(Ease.OutQuad));

            _sequence.Append(kart.DOMove(exitPoint.position, driveByDuration).SetEase(Ease.InCubic));
            _sequence.Join(kart.DORotate(exitPoint.eulerAngles, driveByDuration * 0.45f).SetEase(Ease.OutQuad));

            if (cam != null)
                _sequence.Join(cam.DOFieldOfView(exitFov, driveByDuration).SetEase(Ease.InQuad));

            if (cameraShaker != null)
                _sequence.Insert(launchWindUp, cameraShaker.DOShakePosition(driveByDuration, 0.22f, 12, 90f, false, true));

            return _sequence;
        }

        private void ResetPose()
        {
            if (kart != null && entryPoint != null)
                kart.SetPositionAndRotation(entryPoint.position, entryPoint.rotation);

            if (cameraRig != null && cameraEntryPoint != null)
                cameraRig.SetPositionAndRotation(cameraEntryPoint.position, cameraEntryPoint.rotation);

            if (cameraShaker != null) cameraShaker.localPosition = _shakerRest;
            if (cam != null) cam.fieldOfView = entryFov;
        }

        private void StartIdle()
        {
            if (kart != null)
            {
                _idleBob = kart
                    .DOMoveY(kart.position.y + idleBobHeight, idleBobDuration)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetEase(Ease.InOutSine);
            }

            if (cameraShaker != null)
            {
                cameraShaker.localPosition = _shakerRest;
                _idleDrift = cameraShaker
                    .DOLocalMoveX(_shakerRest.x + idleDriftX, idleDriftDuration)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetEase(Ease.InOutSine);
            }
        }

        private void KillAll()
        {
            _sequence?.Kill();
            _idleBob?.Kill();
            _idleDrift?.Kill();
            if (cameraShaker != null) cameraShaker.DOKill();
        }

        private static void PlayEffect(ParticleSystem ps)
        {
            if (ps != null) ps.Play();
        }

        private static void StopEffect(ParticleSystem ps)
        {
            if (ps != null) ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }
}
