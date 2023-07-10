using System;
using Data;
using TMPro;
using UI;
using UnityEngine;

namespace Managers
{
    public class UiManager : MonoBehaviour
    {
        [Header("Windows Data")]
        public BriefingWindow briefingWindow;
        public GameUiWindow gameUiWindow;
        public TaskWindow taskWindow;
        public GameOverWindow gameOverWindow;

        public void InitializeWindow(Windows window)
        {
            switch (window)
            {
                case Windows.Briefing:
                    briefingWindow.window.SetActive(true);
                    break;
                case Windows.GameUi:
                    gameUiWindow.window.SetActive(true);
                    break;
                case Windows.Task:
                    taskWindow.window.SetActive(true);
                    break;
                case Windows.GameOver:
                    gameOverWindow.window.SetActive(true);
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
                case Windows.GameUi:
                    gameUiWindow.window.SetActive(false);
                    break;
                case Windows.Task:
                    if(!GameManager.Instance.taskManager.taskCompleted) return;
                    taskWindow.window.SetActive(false);
                    break;
                case Windows.GameOver:
                    gameOverWindow.window.SetActive(false);
                    break;
            }
        }
    }
}

[Serializable]
public struct BriefingWindow
{
    public GameObject window;
    public Transform taskParent;
    public TextMeshProUGUI taskTextPrefab;
}

[Serializable]
public struct GameUiWindow
{
    public GameObject window;
    public TextMeshProUGUI countDownText;
}

[Serializable]
public struct TaskWindow
{
    public GameObject window;
    public TextMeshProUGUI taskText;
    public TaskSlotsController taskSlotsController;
}

[Serializable]
public struct GameOverWindow
{
    public GameObject window;
}