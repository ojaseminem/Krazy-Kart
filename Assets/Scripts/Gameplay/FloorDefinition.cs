using UnityEngine;

namespace Gameplay
{
    /// Marks one floor's root object. The RunController activates these in order as the player
    /// ascends, so only the current floor (and its neighbour) pays rendering cost.
    public class FloorDefinition : MonoBehaviour
    {
        [Header("Identity")]
        [SerializeField] private Core.FloorId floorId = Core.FloorId.B1;
        [SerializeField] private string displayName = "Basement Parking";

        [Header("Spawn")]
        [Tooltip("Where the kart enters this floor.")]
        [SerializeField] private Transform entryPoint;

        [Header("Cops")]
        [SerializeField] private bool copsEnabled = true;
        [Tooltip("Props destroyed before the first cop joins.")]
        [SerializeField] private int firstCopAt = 3;
        [SerializeField] private int secondCopAt = 6;
        [SerializeField] private int formationAt = 10;
        [SerializeField] private Transform[] copSpawnPoints;

        public Core.FloorId FloorId => floorId;
        public string DisplayName => displayName;
        public Transform EntryPoint => entryPoint != null ? entryPoint : transform;
        public bool CopsEnabled => copsEnabled;
        public int FirstCopAt => firstCopAt;
        public int SecondCopAt => secondCopAt;
        public int FormationAt => formationAt;
        public Transform[] CopSpawnPoints => copSpawnPoints;

        private void OnDrawGizmos()
        {
            if (entryPoint == null) return;
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(entryPoint.position, 1.2f);
            Gizmos.DrawLine(entryPoint.position, entryPoint.position + entryPoint.forward * 3f);
        }
    }
}
