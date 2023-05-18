using UnityEngine;

namespace Managers
{
    public class TaskManager : MonoBehaviour
    {
        public void InitTaskBriefing(ItemType itemType)
        {
            var briefingText = GameManager.Instance.levelData.briefingText;
            
            FinalizeBriefing(briefingText, itemType);

            var briefingWindow = GameManager.Instance.uiManager.briefingWindow;
            
            var ld = GameManager.Instance.levelData;
            
            var taskText = Instantiate(briefingWindow.taskTextPrefab, briefingWindow.taskParent);
            taskText.text = FinalizeBriefing(ld.briefingText, itemType);

            /*switch (ld.levelDifficulty)
            {
                case LevelDifficulty.Easy:
                    for (int i = 0; i < ld.taskCountEasy; i++)
                    {
                        print("SpawnedText");
                        var taskText = Instantiate(briefingWindow.taskTextPrefab, briefingWindow.taskParent);
                        taskText.text = FinalizeBriefing(ld.briefingText, itemType);
                    }
                    break;
                case LevelDifficulty.Average:
                    for (int i = 0; i < ld.taskCountAverage; i++)
                    {
                        var taskText = Instantiate(briefingWindow.taskTextPrefab, briefingWindow.taskParent);
                        taskText.text = FinalizeBriefing(ld.briefingText, itemType);
                    }
                    break;
                case LevelDifficulty.Hard:
                    for (int i = 0; i < ld.taskCountHard; i++)
                    {
                        var taskText = Instantiate(briefingWindow.taskTextPrefab, briefingWindow.taskParent);
                        taskText.text = FinalizeBriefing(ld.briefingText, itemType);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }*/
        }

        private string FinalizeBriefing(string briefValue, ItemType itemName)
        {
            var finalBrief = "";

            finalBrief = string.Format(briefValue, $"{itemName}");

            return finalBrief;
        }
    }
}