using System;
using System.Collections;
using System.Collections.Generic;
using Items;
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
        private Item _currItem;
        private List<Item> _uniqueItemsList = new List<Item>();
        private TaskWindow _taskWindow;

        private int _randomItemCount;
        private List<Item> _randomItemsList = new List<Item>();

        private int _objectsLayer;

        public void InitMidTask()
        {
            GameManager.Instance.uiManager.InitializeWindow(Windows.Task);
            var itemData = GameManager.Instance.itemData;
            _taskWindow = GameManager.Instance.uiManager.taskWindow;

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
            
            _objectsLayer = LayerMask.NameToLayer("UIObjects");
            
            _randomItemCount = Mathf.Min(_uniqueItemsList.Count, 2);
            _randomItemsList = _uniqueItemsList.FindAll(item => item != _currItem);
            
            switch (spawnOrder)
            {
                case 1:
                    _currItem = Instantiate(itemData.currItem, _taskWindow.layoutGroup.transform);
                    SpawnRandomItem();
                    SpawnRandomItem();
                    break;
                case 2:
                    SpawnRandomItem();
                    SpawnRandomItem();
                    _currItem = Instantiate(itemData.currItem, _taskWindow.layoutGroup.transform);
                    break;
                case 3:
                    SpawnRandomItem();
                    _currItem = Instantiate(itemData.currItem, _taskWindow.layoutGroup.transform);
                    SpawnRandomItem();
                    break;
                default:
                    break;
            }

            _currItem.gameObject.layer = _objectsLayer;

            _taskWindow.layoutGroup.RebuildLayout();
            
            _currItem.SpawnedForSelection();
            
            _uniqueItemsList.Clear();
        }
        
        private void SpawnRandomItem()
        {
            if (_randomItemsList.Count == 0) return;

            var spawnedRandomItem = Instantiate(_randomItemsList[Random.Range(0, _randomItemsList.Count)],
                _taskWindow.layoutGroup.transform);

            spawnedRandomItem.SpawnedForSelection();
            spawnedRandomItem.gameObject.layer = _objectsLayer;

            _randomItemsList.Remove(spawnedRandomItem);
        }
    }
}