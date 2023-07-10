using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "EconomyData", menuName = "Data/EconomyData")]
    public class EconomyData : ScriptableObject
    {
        public int coinCount;
        public int tokenCount;
    }
}