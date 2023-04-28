using System.Collections;
using Data;
using Player;
using TMPro;
using UnityEngine;
using Utils;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        #region Singleton

        public static GameManager Instance;
        private void Awake() => Instance = this;

        #endregion

        [Header("Managers")]
        [SerializeField] private PlayerManager playerManager;
        [SerializeField] private UiManager uiManager;
        [SerializeField] private TaskManager taskManager;

        [Header("Data")]
        public BriefingData briefingData;
        public TaskData taskData;
        
        private void Start()
        {
            ChangeState(GameState.Briefing);
        }

        public void ChangeState(GameState newState)
        {
            switch (newState)
            {
                case GameState.Briefing:
                    Briefing();
                    break;
                case GameState.Playing:
                    Playing();
                    break;
                case GameState.Task:
                    Task();
                    break;
                case GameState.ScoreCalculation:
                    FindObjectOfType<PlayerController>().canMove = false;
                    //StartCoroutine(onScoreCalculation());
                    break;
            }
        }
    
        private void Briefing()
        {
            playerManager.SetPlayerMove(false);
            
            uiManager.InitializeWindow(Windows.Briefing);
            
            StartCoroutine(Util.WaitForSecondsRoutine(3f, () =>
            {
                uiManager.CloseWindow(Windows.Briefing);
                ChangeState(GameState.Playing);
            }));
        }

        private void Playing()
        {
            playerManager.SetPlayerMove(true);

            uiManager.InitializeWindow(Windows.GameUi);
        }
    
        private void Task()
        {
            playerManager.SetPlayerMove(false);

            taskManager.SetCurrentSentence();
        }
        
        /*IEnumerator onScoreCalculation()
        {
            scoreCalculationWindow.gameObject.SetActive(true);

            Transform completedText = scoreCalculationWindow.GetChild(1);
            completedText.GetComponent<TextMeshProUGUI>().text = "Level Complete";
            scoreText.text = "";

            yield return new WaitForSeconds(0.3f);

            calculateScore = true;

            yield return new WaitUntil(() => scoreCalculated == true);

            calculateScore = false;
            if (ScoreManager.currentScore == ScoreManager.totalScore) ChangeState(GameState.Win);
        }*/
    

    }

    public enum GameState
    {
        Briefing,
        Playing,
        Task,
        ScoreCalculation
    }
}

//Todo Win a prize bonus game
//Todo Vending machine bonus game
//Todo Hard level riddle based word generation