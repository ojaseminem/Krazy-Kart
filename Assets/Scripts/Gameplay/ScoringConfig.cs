using UnityEngine;

namespace Gameplay
{
    /// Every tunable scoring number lives here so gameplay code carries no magic values.
    /// Values follow the MC tables in Assets/_AI/CONTEXT.md.
    [CreateAssetMenu(menuName = "KrazyKart/Scoring Config", fileName = "ScoringConfig")]
    public class ScoringConfig : ScriptableObject
    {
        [Header("Impact")]
        [Tooltip("Minimum impact speed (kph) that will break a destructible.")]
        public float minBreakSpeedKph = 14f;
        [Tooltip("Impact speed (kph) at which a prop takes full damage.")]
        public float fullDamageSpeedKph = 45f;

        [Header("Chain Destruction")]
        [Tooltip("Props destroyed within this window count toward one chain.")]
        public float chainWindow = 0.8f;
        [Tooltip("Chain must reach this length before bonus MC is paid.")]
        public int chainMinLength = 3;
        [Tooltip("MC per prop beyond the minimum chain length.")]
        public int chainBonusPerProp = 12;
        public int chainBonusCap = 80;

        [Header("Combo")]
        [Tooltip("Combo decays if nothing is scored for this long.")]
        public float comboTimeout = 2f;
        public float comboStep = 0.25f;
        public float comboMax = 4f;

        [Header("Drift")]
        [Tooltip("Minimum speed before drifting earns style MC.")]
        public float driftMinSpeedKph = 20f;
        public float driftMcPerSecond = 14f;
        [Tooltip("Radius checked for nearby geometry during a drift.")]
        public float nearMissRadius = 2.6f;
        public int nearMissBonus = 18;
        [Tooltip("Cooldown so one object cannot pay out repeatedly.")]
        public float nearMissCooldown = 0.6f;

        [Header("Stunt / Air")]
        [Tooltip("Airborne longer than this counts as a stunt.")]
        public float minAirTime = 0.5f;
        public int stuntBaseMc = 40;
        public int stuntMcPerSecond = 80;
        public int stuntMcCap = 120;

        [Header("Cops")]
        public int copEvadeMc = 90;
        public int copKnockIntoPropMc = 140;

        [Header("NPC")]
        public int npcHitMc = 25;

        [Header("Transition")]
        public int cleanCorridorBonus = 60;

        [Header("MC → Escape Time")]
        [Tooltip("X = total MC, Y = escape seconds on the boss floor. Table from the GDD.")]
        public AnimationCurve mcToEscapeSeconds = new AnimationCurve(
            new Keyframe(0f, 12f),
            new Keyframe(200f, 20f),
            new Keyframe(500f, 35f),
            new Keyframe(900f, 55f),
            new Keyframe(1400f, 85f));

        public float EscapeSecondsFor(int totalMc) => mcToEscapeSeconds.Evaluate(totalMc);

        /// Maps an impact speed onto 0..1 damage.
        public float DamageFromSpeed(float speedKph)
        {
            if (speedKph < minBreakSpeedKph) return 0f;
            return Mathf.Clamp01(Mathf.InverseLerp(minBreakSpeedKph, fullDamageSpeedKph, speedKph));
        }
    }
}
