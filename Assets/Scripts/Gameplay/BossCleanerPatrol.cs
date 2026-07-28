using UnityEngine;

namespace Gameplay
{
    /// Phase-3 cleaning robot. Sweeps side to side across the office and shoves the kart off
    /// line on contact — an obstacle to read and time, not a threat that can kill.
    public class BossCleanerPatrol : MonoBehaviour
    {
        [SerializeField] private float patrolWidth = 14f;
        [SerializeField] private float speed = 4.5f;
        [Tooltip("Sideways shove applied to the kart on contact.")]
        [SerializeField] private float shoveForce = 6f;
        [SerializeField] private float spinSpeed = 140f;

        private Vector3 _origin;
        private float _phase;

        private void OnEnable()
        {
            _origin = transform.localPosition;
            _phase = Random.Range(0f, Mathf.PI * 2f);
        }

        private void Update()
        {
            _phase += Time.deltaTime * (speed / Mathf.Max(1f, patrolWidth)) * 2f;

            Vector3 pos = _origin;
            pos.x += Mathf.Sin(_phase) * patrolWidth * 0.5f;
            transform.localPosition = pos;

            transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime, Space.Self);
        }

        private void OnTriggerEnter(Collider other) => Shove(other);

        private void OnCollisionEnter(Collision collision) => Shove(collision.collider);

        private void Shove(Collider other)
        {
            var kart = other.GetComponentInParent<Kart.KartController>();
            if (kart == null) return;

            var body = kart.GetComponent<Rigidbody>();
            if (body == null) return;

            Vector3 dir = (kart.transform.position - transform.position).normalized;
            dir.y = 0.15f;
            body.AddForce(dir * shoveForce, ForceMode.VelocityChange);
        }
    }
}
