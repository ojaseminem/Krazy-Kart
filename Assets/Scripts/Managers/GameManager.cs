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

        public static GameManager instance;
        private void Awake() => instance = this;

        #endregion

        [Header("Managers")]
        public PlayerManager playerManager;
        public UiManager uiManager;
        public SentenceManager sentenceManager;
        public ShoppingManager shoppingManager;
        public TaskManager taskManager;
        public TimeManager timeManager;
        public NpcManager npcManager;
        public StoreManager storeManager;

        [Header("Data")] 
        public LevelData levelData;
        public ItemData itemData;

        private void Start()
        {
            //Application.targetFrameRate = 60;
            ChangeState(GameState.Briefing);
        }

        public void ChangeState(GameState state)
        {
            switch (state)
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
            itemData.spawnedItems.Clear();
            
            shoppingManager.InitShopping(levelData, itemData);

            playerManager.SetPlayerMove(false);
            
            uiManager.InitializeWindow(Windows.Briefing);
            
            StartCoroutine(Util.WaitForSecondsRoutine(3f, () =>
            {
                uiManager.CloseWindow(Windows.Briefing);
                ChangeState(GameState.Playing);
                timeManager.StartCountDown(120f);
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

            sentenceManager.SetCurrentSentence();
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
}

//Todo Win a prize bonus game
//Todo Vending machine bonus game
//Todo Hard level riddle based word generation