using System;
using Data;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Managers
{
    public class UiManager : MonoBehaviour
    {
        #region Singleton

        public static UiManager Instance;
        private void Awake() => Instance = this;

        #endregion
        
        public BriefingWindow briefingWindow;
        public TaskWindow taskWindow;

        private BriefingData _briefingData;
        private TaskData _taskData;
        
        public void InitializeWindow(Windows window)
        {
            switch (window)
            {
                case Windows.Briefing:
                    InitBriefing();
                    break;
                case Windows.GameUi:
                    break;
                case Windows.Task:
                    InitTask();
                    break;
            }
        }

        public void CloseWindow(Windows window)
        {
            switch (window)
            {
                case Windows.Briefing:
                    briefingWindow.window.SetActive(false);
                    break;
                case Windows.Task:
                    taskWindow.window.SetActive(false);
                    break;
            }
        }

        private void InitBriefing()
        {
            _briefingData = GameManager.Instance.briefingData;
            
            briefingWindow.window.SetActive(true);
            
            briefingWindow.heading.text = _briefingData.briefingHeader;
            
            var taskText = Instantiate(briefingWindow.taskTextPrefab, briefingWindow.taskParent);
            taskText.text = _briefingData.briefingTasksEasy[Random.Range(0, _briefingData.briefingTasksEasy.Length)];
        }

        private void InitTask()
        {
            _taskData = GameManager.Instance.taskData;

            taskWindow.window.SetActive(true);

            taskWindow.heading.text = _taskData.taskHeader;
        }
    }
}

public enum Windows
{
    Briefing,
    GameUi,
    Task,
}

[Serializable]
public struct BriefingWindow
{
    public GameObject window;
    public TextMeshProUGUI heading;
    public Transform taskParent;
    public TextMeshProUGUI taskTextPrefab;
}

[Serializable]
public struct TaskWindow
{
    public GameObject window;
    public TextMeshProUGUI heading;
    public TextMeshProUGUI taskText;
}