using UnityEngine;

namespace Gameplay
{
    public enum CopState
    {
        Idle,
        Chase,
        Ram,
        Stunned
    }

    /// Pursuit AI for a security kart. Deliberately simple steering — it drives toward a lead
    /// point ahead of the player rather than pathfinding, which reads as aggressive without
    /// needing a NavMesh baked into every floor.
    ///
    /// Counterplay: outrun it (it gives up past loseDistance and pays evade MC), or ram it into
    /// props, which damages them and stuns the cop.
    [RequireComponent(typeof(Rigidbody))]
    public class CopKart : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float acceleration = 22f;
        [SerializeField] private float maxSpeed = 17f;
        [SerializeField] private float turnSpeed = 3.2f;
        [Tooltip("How far ahead of the player the cop aims, to cut corners.")]
        [SerializeField] private float leadDistance = 3.5f;

        [Header("Engagement")]
        [SerializeField] private float chaseDistance = 45f;
        [Tooltip("Beyond this the cop gives up and the player is paid evade MC.")]
        [SerializeField] private float loseDistance = 62f;
        [SerializeField] private float ramDistance = 6f;
        [SerializeField] private float ramForce = 9f;

        [Header("Stun")]
        [SerializeField] private float stunDuration = 2.2f;

        [Header("Feedback")]
        [SerializeField] private Light sirenLight;
        [SerializeField] private float sirenFlashRate = 6f;

        private Rigidbody _body;
        private Transform _target;
        private Rigidbody _targetBody;
        private CopState _state = CopState.Idle;
        private float _stunTimer;
        private bool _evadePaid;

        public CopState State => _state;

        private void Awake()
        {
            _body = GetComponent<Rigidbody>();
        }

        public void Initialise(Transform target)
        {
            _target = target;
            _targetBody = target != null ? target.GetComponent<Rigidbody>() : null;
            _state = CopState.Chase;
            _evadePaid = false;
        }

        private void Update()
        {
            if (sirenLight != null && _state != CopState.Stunned)
                sirenLight.intensity = Mathf.PingPong(Time.time * sirenFlashRate, 3f);

            if (_state != CopState.Stunned) return;

            _stunTimer -= Time.deltaTime;
            if (_stunTimer <= 0f) _state = CopState.Chase;
        }

        private void FixedUpdate()
        {
            if (_target == null || _state == CopState.Idle || _state == CopState.Stunned) return;

            float distance = Vector3.Distance(transform.position, _target.position);

            if (distance > loseDistance)
            {
                Disengage();
                return;
            }

            if (distance > chaseDistance) return;

            _state = distance <= ramDistance ? CopState.Ram : CopState.Chase;
            Drive(distance);
        }

        private void Drive(float distance)
        {
            // Aim ahead of the player so the cop cuts the corner instead of trailing exactly.
            Vector3 lead = _target.position;
            if (_targetBody != null)
                lead += _targetBody.linearVelocity.normalized * leadDistance;

            Vector3 toTarget = lead - transform.position;
            toTarget.y = 0f;
            if (toTarget.sqrMagnitude < 0.01f) return;

            Quaternion desired = Quaternion.LookRotation(toTarget.normalized, Vector3.up);
            _body.MoveRotation(Quaternion.Slerp(transform.rotation, desired, turnSpeed * Time.fixedDeltaTime));

            float push = _state == CopState.Ram ? acceleration * 1.35f : acceleration;
            if (_body.linearVelocity.magnitude < maxSpeed)
                _body.AddForce(transform.forward * push, ForceMode.Acceleration);
        }

        private void Disengage()
        {
            _state = CopState.Idle;

            if (!_evadePaid && McManager.Instance != null)
            {
                _evadePaid = true;
                McManager.Instance.Award(
                    McManager.Instance.Config.copEvadeMc, McSource.CopEvaded, transform.position);
                RunStats.NotifyCopEvaded();
            }

            CopSpawner.Instance?.NotifyDespawned(this);
            Destroy(gameObject, 0.5f);
        }

        private void OnCollisionEnter(Collision collision)
        {
            // Shoved into scenery by the player: wreck the prop, stun the cop, pay the player.
            var destructible = collision.collider.GetComponentInParent<DestructibleProp>();
            if (destructible != null && !destructible.IsBroken && _body.linearVelocity.magnitude > 5f)
            {
                destructible.Break(collision.GetContact(0).point);
                Stun();

                McManager.Instance?.Award(
                    McManager.Instance.Config.copKnockIntoPropMc,
                    McSource.CopKnocked,
                    transform.position);
                return;
            }

            // Cop-on-cop pileup: both stunned.
            var otherCop = collision.collider.GetComponentInParent<CopKart>();
            if (otherCop != null && otherCop != this)
            {
                Stun();
                otherCop.Stun();
            }
        }

        public void Stun()
        {
            _state = CopState.Stunned;
            _stunTimer = stunDuration;
            if (sirenLight != null) sirenLight.intensity = 0.2f;
        }
    }
}
