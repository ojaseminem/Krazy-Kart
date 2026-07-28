using DG.Tweening;
using UnityEngine;

namespace Gameplay
{
    /// A breakable piece of mall furniture. Takes damage from fast impacts, pays MC, throws
    /// debris, and feeds the chain-destruction window.
    ///
    /// Chains propagate: when this breaks it nudges neighbours inside splashRadius, so one good
    /// hit can cascade through a cluster without any authored sequencing.
    [RequireComponent(typeof(Collider))]
    public class DestructibleProp : MonoBehaviour
    {
        [Header("Value")]
        [SerializeField] private int mcValue = 10;
        [Tooltip("Hits needed at full impact speed.")]
        [SerializeField] private float health = 1f;

        [Header("Break Behaviour")]
        [Tooltip("Renderers hidden on break. Left empty, every child renderer is used.")]
        [SerializeField] private Renderer[] visuals;
        [Tooltip("Loose chunks spawned on break. Left empty, the visual is scaled away instead.")]
        [SerializeField] private GameObject debrisPrefab;
        [SerializeField] private int debrisCount = 4;
        [SerializeField] private float debrisForce = 5.5f;
        [SerializeField] private float debrisLifetime = 4f;

        [Header("Chain")]
        [Tooltip("Neighbours within this radius are shoved when this breaks.")]
        [SerializeField] private float splashRadius = 2.4f;
        [SerializeField] private float splashForce = 4.5f;
        [Tooltip("Damage passed to neighbours caught in the splash.")]
        [SerializeField] private float splashDamage = 0.55f;

        [Header("VFX")]
        [SerializeField] private GameObject breakVfxPrefab;
        [SerializeField] private float vfxLifetime = 3f;

        private bool _broken;
        private Collider[] _colliders;
        private Rigidbody _body;
        private static readonly Collider[] SplashHits = new Collider[16];

        public bool IsBroken => _broken;
        public int McValue => mcValue;

        private void Awake()
        {
            _colliders = GetComponentsInChildren<Collider>(true);
            _body = GetComponent<Rigidbody>();
            if (visuals == null || visuals.Length == 0)
                visuals = GetComponentsInChildren<Renderer>(true);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (_broken) return;

            var driver = collision.collider.GetComponentInParent<Kart.KartController>();
            if (driver == null) return;

            var cfg = McManager.Instance != null ? McManager.Instance.Config : null;
            if (cfg == null) return;

            float speed = collision.relativeVelocity.magnitude * 3.6f;
            float damage = cfg.DamageFromSpeed(speed);
            if (damage <= 0f) return;

            ApplyDamage(damage, collision.GetContact(0).point);
        }

        /// Public so splash, cops and scripted events can all break props the same way.
        public void ApplyDamage(float damage, Vector3 impactPoint)
        {
            if (_broken || damage <= 0f) return;

            health -= damage;
            if (health > 0f)
            {
                Wobble();
                return;
            }

            Break(impactPoint);
        }

        public void Break(Vector3 impactPoint)
        {
            if (_broken) return;
            _broken = true;

            var mc = McManager.Instance;
            if (mc != null)
            {
                mc.Award(mcValue, McSource.PropDestroyed, transform.position);
                mc.RegisterDestruction(transform.position);
            }

            SpawnVfx(impactPoint);
            SpawnDebris(impactPoint);
            Splash();

            foreach (var col in _colliders)
                if (col != null) col.enabled = false;

            if (_body != null)
            {
                _body.isKinematic = true;
                _body.detectCollisions = false;
            }

            RunStats.NotifyPropDestroyed();
            HideVisuals();
        }

        private void Wobble()
        {
            transform.DOKill(true);
            transform.DOPunchRotation(new Vector3(0f, 0f, 6f), 0.25f, 8, 0.6f);
        }

        private void HideVisuals()
        {
            // With no debris prefab the mesh itself is the effect: crush it and vanish.
            if (debrisPrefab == null && visuals != null && visuals.Length > 0)
            {
                transform.DOKill(true);
                transform.DOScale(Vector3.zero, 0.28f)
                    .SetEase(Ease.InBack)
                    .OnComplete(() => gameObject.SetActive(false));
                return;
            }

            foreach (var r in visuals)
                if (r != null) r.enabled = false;

            gameObject.SetActive(false);
        }

        private void SpawnVfx(Vector3 point)
        {
            if (breakVfxPrefab == null) return;

            var vfx = Instantiate(breakVfxPrefab, point, Quaternion.identity);
            Destroy(vfx, vfxLifetime);
        }

        private void SpawnDebris(Vector3 impactPoint)
        {
            if (debrisPrefab == null) return;

            for (int i = 0; i < debrisCount; i++)
            {
                Vector3 pos = transform.position + Random.insideUnitSphere * 0.4f + Vector3.up * 0.3f;
                var chunk = Instantiate(debrisPrefab, pos, Random.rotation);

                var rb = chunk.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    Vector3 dir = (pos - impactPoint).normalized + Vector3.up * 0.6f;
                    rb.AddForce(dir * debrisForce, ForceMode.Impulse);
                    rb.AddTorque(Random.insideUnitSphere * debrisForce, ForceMode.Impulse);
                }

                Destroy(chunk, debrisLifetime);
            }
        }

        /// Shoves and partially damages neighbours so clusters cascade.
        private void Splash()
        {
            if (splashRadius <= 0f) return;

            int count = Physics.OverlapSphereNonAlloc(transform.position, splashRadius, SplashHits);
            for (int i = 0; i < count; i++)
            {
                var hit = SplashHits[i];
                if (hit == null) continue;

                var other = hit.GetComponentInParent<DestructibleProp>();
                if (other == null || other == this || other.IsBroken) continue;

                var rb = other.GetComponent<Rigidbody>();
                if (rb != null && !rb.isKinematic)
                {
                    Vector3 dir = (other.transform.position - transform.position).normalized + Vector3.up * 0.25f;
                    rb.AddForce(dir * splashForce, ForceMode.Impulse);
                }

                other.ApplyDamage(splashDamage, transform.position);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1f, 0.6f, 0.1f, 0.35f);
            Gizmos.DrawWireSphere(transform.position, splashRadius);
        }
    }
}
