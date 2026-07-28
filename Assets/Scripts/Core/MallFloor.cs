using UnityEngine;

namespace Core
{
    /// Identifies a floor scene and its MC/cop tuning. Lives on a root object in each Floor_* scene.
    public class MallFloor : MonoBehaviour
    {
        [Header("Identity")]
        [SerializeField] private FloorId floorId;
        [SerializeField] private string sceneName;

        [Header("MC Budget")]
        [SerializeField] private int mcBudgetMin;
        [SerializeField] private int mcBudgetMax;

        [Header("Cop Thresholds")]
        [SerializeField] private bool copsEnabled = true;
        [Tooltip("Props destroyed to spawn the 1st cop / 2nd cop / 3-kart formation.")]
        [SerializeField] private int copThreshold1 = 3;
        [SerializeField] private int copThreshold2 = 6;
        [SerializeField] private int copThreshold3 = 10;

        public FloorId FloorId => floorId;
        public string SceneName => sceneName;
        public int McBudgetMin => mcBudgetMin;
        public int McBudgetMax => mcBudgetMax;
        public bool CopsEnabled => copsEnabled;
        public int CopThreshold1 => copThreshold1;
        public int CopThreshold2 => copThreshold2;
        public int CopThreshold3 => copThreshold3;

        private void Start()
        {
            FloorManager.Instance?.SetCurrentFloor(this);
        }
    }
}
