using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "BriefingData", menuName = "Data/BriefingData")]
    public class BriefingData : ScriptableObject
    {
        public string briefingHeader;

        public string[] briefingTasksEasy;
        public string[] briefingTasksAverage;
        public string[] briefingTasksHard;
    }
}