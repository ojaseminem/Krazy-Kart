using System.Collections.Generic;
using Items;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "ItemData", menuName = "Data/ItemData")]
    public class ItemData : ScriptableObject
    {
        [Header("Item Data")] 
        public const int TotalNumOfItems = 7;
        
        [Header("ItemHolders")]
        public ItemHolder[] itemHolders;

        [Header("Items")]
        public List<Item> groceries;
        public List<Item> arcade;
        public List<Item> music;
        public List<Item> fastFood;
        public List<Item> jewellery;
        public List<Item> clothing;
        public List<Item> bakery;

        public List<Item> spawnedItems = new List<Item>();
    }
}