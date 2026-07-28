using System;
using UnityEngine;

namespace Core
{
    /// Sits on a child trigger collider inside a corridor. Forwards player-enter to its parent
    /// TransitionCorridor so multiple zones (entry / midpoint) can be distinguished.
    [RequireComponent(typeof(Collider))]
    public class TransitionZoneRelay : MonoBehaviour
    {
        [SerializeField] private string playerTag = "Player";

        public event Action<Collider> OnPlayerEntered;

        private void Reset()
        {
            var col = GetComponent<Collider>();
            col.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(playerTag)) return;
            OnPlayerEntered?.Invoke(other);
        }
    }
}
