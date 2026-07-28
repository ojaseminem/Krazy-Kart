using System;
using UnityEngine;

namespace Flow
{
    /// One advertised step of a load sequence. Weight decides how much of the bar the stage
    /// occupies; minDuration is the dwell that makes streaming feel staged rather than instant.
    [Serializable]
    public struct LoadStage
    {
        [Tooltip("Shown on the loading screen while this stage runs.")]
        public string label;

        [Tooltip("Relative share of the bar this stage covers.")]
        [Min(0.01f)] public float weight;

        [Tooltip("Minimum seconds this stage stays on screen.")]
        [Min(0f)] public float minDuration;

        public LoadStage(string label, float weight, float minDuration)
        {
            this.label = label;
            this.weight = weight;
            this.minDuration = minDuration;
        }
    }
}
