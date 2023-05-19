using Data;
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

        private void Start() => ChangeState(GameState.PreRequisites);

        public void ChangeState(GameState state)
        {
            switch (state)
            {
                case GameState.PreRequisites:
                    PreRequisites();
                    break;
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
                    //FindObjectOfType<PlayerController>().canMove = false;
                    //StartCoroutine(onScoreCalculation());
                    break;
                case GameState.GameOver:
                    GameOver();
                    break;
            }
        }

        private void PreRequisites()
        {
            itemData.spawnedItems.Clear();
            
            levelData.skyboxMat.SetFloat(levelData.CubemapTransition, 0f);
            
            ChangeState(GameState.Briefing);
        }

        private void Briefing()
        {
            shoppingManager.InitShopping(levelData, itemData);

            playerManager.SetPlayerMove(false);
            
            uiManager.InitializeWindow(Windows.Briefing);
            
            StartCoroutine(Util.WaitForSecondsRoutine(3f, () =>
            {
                uiManager.CloseWindow(Windows.Briefing);
                ChangeState(GameState.Playing);
                timeManager.StartCountDown(levelData.timerInSeconds);
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

        private void GameOver()
        {
            uiManager.CloseWindow(Windows.Task);
            uiManager.CloseWindow(Windows.GameUi);
            uiManager.InitializeWindow(Windows.GameOver);
            playerManager.SetPlayerMove(false);
            playerManager.GameOver();
        }
    }
}

//Todo Win a prize bonus game
//Todo Vending machine bonus game
//Todo Hard level riddle based word generation