using System;
using System.Collections.Generic;
using Items;
using TMPro;
using Utils;
using UnityEngine;
using Random = UnityEngine.Random;

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

        public bool taskCompleted;
        
        private TaskWindow _taskWindow;
        private List<Item> _uniqueItemsList = new List<Item>();
        private List<Item> _randomItemsList = new List<Item>();
        private int _objectsLayer;

        public void ChangeTaskState(TaskState taskState)
        {
            switch (taskState)
            {
                case TaskState.PreTask:
                    PreTask();
                    break;
                case TaskState.PostTask:
                    PostTask();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(taskState), taskState, null);
            }
        }

        private void PreTask()
        {
            #region PreRequisites

            var itemData = GameManager.Instance.itemData;
            _taskWindow = GameManager.Instance.uiManager.taskWindow;
            
            #endregion
            
            switch (itemData.currStore)
            {
                case StoreType.Groceries:
                    _uniqueItemsList.AddRange(itemData.groceries);
                    break;
                case StoreType.Arcade:
                    _uniqueItemsList.AddRange(itemData.arcade);
                    break;
                case StoreType.Music:
                    _uniqueItemsList.AddRange(itemData.music);
                    break;
                case StoreType.FastFood:
                    _uniqueItemsList.AddRange(itemData.fastFood);
                    break;
                case StoreType.Jewelry:
                    _uniqueItemsList.AddRange(itemData.jewellery);
                    break;
                case StoreType.Clothing:
                    _uniqueItemsList.AddRange(itemData.clothing);
                    break;
                case StoreType.Bakery:
                    _uniqueItemsList.AddRange(itemData.bakery);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            var spawnOrder = Random.Range(1, 4);

            _randomItemsList = _uniqueItemsList.FindAll(item => item.itemType != itemData.currItem.itemType);
            
            switch (spawnOrder)
            {
                case 1:
                    _taskWindow.taskSlotsController.SpawnItem(itemData.currItem, 1);
                    _taskWindow.taskSlotsController.SpawnItem(RandomItem(), 2);
                    _taskWindow.taskSlotsController.SpawnItem(RandomItem(), 3);
                    break;
                case 2:
                    _taskWindow.taskSlotsController.SpawnItem(RandomItem(), 1);
                    _taskWindow.taskSlotsController.SpawnItem(itemData.currItem, 2);
                    _taskWindow.taskSlotsController.SpawnItem(RandomItem(), 3);
                    break;
                case 3:
                    _taskWindow.taskSlotsController.SpawnItem(RandomItem(), 1);
                    _taskWindow.taskSlotsController.SpawnItem(RandomItem(), 2);
                    _taskWindow.taskSlotsController.SpawnItem(itemData.currItem, 3);
                    break;
            }

            _uniqueItemsList.Clear();

            Item RandomItem()
            {
                if (_randomItemsList.Count == 0) return null;

                var randomItem = _randomItemsList[Random.Range(0, _randomItemsList.Count)];
                _randomItemsList.Remove(randomItem);
                
                return randomItem;
            }
        }
        
        private void PostTask()
        {
            Debug.Log("Post Task");
            GameManager.Instance.uiManager.CloseWindow(Windows.Task);
            GameManager.Instance.ChangeState(GameState.Playing);
        }

        public void MiniTaskAnimationCompleted()
        {
            //Instantiate Popup
        }
        
        
        
        
        
        
        
        
        public bool checkForInput;
        public TextMeshProUGUI sentenceOutput;
    
        [HideInInspector] public string currentSentence;
    
        private UiManager _uiManager;
        private string _remainingSentence;

        private void Start()
        {
            _uiManager = GameManager.Instance.uiManager;
            sentenceOutput = _uiManager.taskWindow.taskText;
        }

        private void Update()
        {
            if (!checkForInput) return;
            CheckInput();
        }

        public void SetCurrentSentence()
        {
            checkForInput = true;
            currentSentence = RandomSentence();
        
            _uiManager.InitializeWindow(Windows.Task);
        
            SetRemainingSentence(currentSentence);
        }

        private void SetRemainingSentence(string newString)
        {
            _remainingSentence = newString;
            sentenceOutput.text = _remainingSentence;
        }

        private void CheckInput()
        {
            if (!Input.anyKeyDown) return;
        
            string keysPressed = Input.inputString;
            if (keysPressed.Length == 1) EnterLetter(keysPressed);
        }
    
        private void EnterLetter(string typedLetter)
        {
            if (!IsCorrectLetter(typedLetter)) return;
            RemoveLetter();

            if (!IsSentenceComplete()) return;
            checkForInput = false;
        
            _uiManager.CloseWindow(Windows.Task);
            GameManager.Instance.ChangeState(GameState.Playing);
        }
    
        private bool IsCorrectLetter(string letter) => _remainingSentence.IndexOf(letter, StringComparison.Ordinal) == 0;

        private void RemoveLetter()
        {
            string newString = _remainingSentence.Remove(0, 1);
            SetRemainingSentence(newString);
        }

        private bool IsSentenceComplete() => _remainingSentence.Length == 0;

        private string RandomSentence()
        {
            int randomSentenceIndex = Random.Range(0, JsonReader.GetSentenceBank().Length);
            string randomSentence = JsonReader.GetSentenceBank()[randomSentenceIndex];

            return randomSentence;
        }
    }
}