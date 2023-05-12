using Items;
using Managers;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "LevelData", menuName = "Data/LevelData")]
    public class LevelData : ScriptableObject
    {
        [Header("Task Briefing")]
        public string briefingText;
        
        [Header("Level Difficulty")]
        public LevelDifficulty levelDifficulty;
        public int taskCountEasy;
        public int taskCountAverage;
        public int taskCountHard;

        [Header("Level Danger")] 
        public float colorChangeSpeed;
        public Color32 defaultColor;
        public Color32 dangerColor;
    }
}