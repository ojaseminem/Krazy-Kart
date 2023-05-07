using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Items;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Managers
{
    public class ShoppingManager : MonoBehaviour
    {
        private LevelData _levelData;
        private ItemData _itemData;

        public void InitShopping(LevelData levelData, ItemData itemData)
        {
            _levelData = levelData;
            _itemData = itemData;

            SetLevelDifficulty();
        }

        //CHANGE THE FUNCTIONALITY
        private void SetLevelDifficulty()
        {
            var totalNumOfItemCategories = ItemData.TotalNumOfItems;

            switch (_levelData.levelDifficulty)
            {
                case LevelDifficulty.Easy:
                    for (int i = 0; i < _levelData.taskCountEasy; i++)
                    {
                        var rand = Random.Range(0, totalNumOfItemCategories);
                        SelectItemCategories(0);
                    }

                    break;
                case LevelDifficulty.Average:
                    for (int i = 0; i < _levelData.taskCountAverage; i++)
                    {
                        var rand = Random.Range(0, totalNumOfItemCategories);
                        SelectItemCategories(rand);
                    }

                    break;
                case LevelDifficulty.Hard:
                    for (int i = 0; i < _levelData.taskCountHard; i++)
                    {
                        var rand = Random.Range(0, totalNumOfItemCategories);
                        SelectItemCategories(rand);
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void SelectItemCategories(int item)
        {
            switch (item)
            {
                case 0: SpawnItemHolders(_itemData.groceries); break;
                case 1: SpawnItemHolders(_itemData.arcade); break;
                case 2: SpawnItemHolders(_itemData.music); break;
                case 3: SpawnItemHolders(_itemData.fastFood); break;
                case 4: SpawnItemHolders(_itemData.jewellery); break;
                case 5: SpawnItemHolders(_itemData.clothing); break;
                case 6: SpawnItemHolders(_itemData.bakery); break;
            }
        }

        private void SpawnItemHolders(List<Item> itemList)
        {
            var rand = Random.Range(0, itemList.Count);

            var currentItem = itemList[rand];

            if (HasCurrentItemSpawnedAlready(currentItem)) return;

            ItemSpawner.instance.SpawnItemHolders(currentItem);

            //Brief the player about the task
            GameManager.instance.taskManager.InitTaskBriefing(currentItem.itemType);

            _itemData.spawnedItems.Add(currentItem);
        }

        private bool HasCurrentItemSpawnedAlready(Item item) => _itemData.spawnedItems.Contains(item);
    }
}