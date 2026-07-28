using UnityEngine;

namespace Gameplay
{
    /// The final glass wall. Only breaks if the player arrives fast enough — arriving slowly
    /// bounces them off, so the cinematic exit always reads as a launch.
    [RequireComponent(typeof(Collider))]
    public class GlassExitTrigger : MonoBehaviour
    {
        [SerializeField] private float requiredSpeedKph = 30f;
        [SerializeField] private GameObject shatterVfx;
        [SerializeField] private Renderer[] glassPanels;

        private bool _fired;

        private void Reset()
        {
            GetComponent<Collider>().isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_fired) return;

            var kart = other.GetComponentInParent<Kart.KartController>();
            if (kart == null) return;
            if (kart.SpeedKph < requiredSpeedKph) return;

            _fired = true;

            if (shatterVfx != null)
            {
                var vfx = Instantiate(shatterVfx, transform.position, Quaternion.identity);
                Destroy(vfx, 4f);
            }

            foreach (var panel in glassPanels)
                if (panel != null) panel.enabled = false;

            BossSequence.Instance?.TriggerEscape();
        }
    }
}
