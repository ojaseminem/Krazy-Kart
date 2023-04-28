using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "TaskData", menuName = "Data/TaskData")]
    public class TaskData : ScriptableObject
    {
        public string taskHeader;
    }
}