using DG.Tweening;
using UnityEngine;

namespace Core
{
    /// C-shaped drift-spiral corridor between two floors. Entry zone unloads the floor behind;
    /// midpoint zone streams the next floor in additively so there's no load hitch on arrival.
    public class TransitionCorridor : MonoBehaviour
    {
        [Header("Scenes")]
        [SerializeField] private string previousFloorSceneName;
        [SerializeField] private string nextFloorSceneName;

        [Header("Zones (child trigger colliders)")]
        [SerializeField] private TransitionZoneRelay entryZone;
        [SerializeField] private TransitionZoneRelay midpointZone;

        [Header("Feedback")]
        [Tooltip("Optional lights pulsed as the kart streams into the corridor.")]
        [SerializeField] private Light[] atmosphereLights;
        [SerializeField] private float pulseDuration = 0.4f;

        private bool _entryFired;
        private bool _midpointFired;

        private void OnEnable()
        {
            if (entryZone != null) entryZone.OnPlayerEntered += HandleEntry;
            if (midpointZone != null) midpointZone.OnPlayerEntered += HandleMidpoint;
        }

        private void OnDisable()
        {
            if (entryZone != null) entryZone.OnPlayerEntered -= HandleEntry;
            if (midpointZone != null) midpointZone.OnPlayerEntered -= HandleMidpoint;
        }

        private void HandleEntry(Collider player)
        {
            if (_entryFired) return;
            _entryFired = true;

            FloorManager.Instance?.UnloadFloorAdditive(previousFloorSceneName);
            PulseLights();
        }

        private void HandleMidpoint(Collider player)
        {
            if (_midpointFired) return;
            _midpointFired = true;

            FloorManager.Instance?.LoadFloorAdditive(nextFloorSceneName);
        }

        private void PulseLights()
        {
            if (atmosphereLights == null) return;

            foreach (var light in atmosphereLights)
            {
                if (light == null) continue;
                float baseIntensity = light.intensity;
                light.DOIntensity(baseIntensity * 1.6f, pulseDuration * 0.5f)
                    .SetLoops(2, LoopType.Yoyo)
                    .SetEase(Ease.InOutSine);
            }
        }
    }
}
