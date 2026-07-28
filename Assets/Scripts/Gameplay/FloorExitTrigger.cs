using UnityEngine;

namespace Gameplay
{
    /// Volume at the top of a floor's exit ramp. Crossing it advances the run.
    [RequireComponent(typeof(Collider))]
    public class FloorExitTrigger : MonoBehaviour
    {
        [Tooltip("Awards the clean-corridor bonus if the player took no wall hits on the way up.")]
        [SerializeField] private bool awardsCleanCorridorBonus = true;

        private bool _fired;

        private void Reset()
        {
            GetComponent<Collider>().isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_fired) return;
            if (other.GetComponentInParent<Kart.KartController>() == null) return;

            _fired = true;
            RunController.Instance?.AdvanceFloor(awardsCleanCorridorBonus);
        }

        /// Re-armed by the RunController when a floor is (re)activated.
        public void Rearm() => _fired = false;
    }
}
