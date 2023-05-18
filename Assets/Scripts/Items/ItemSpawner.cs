using Data;
using Managers;
using UnityEngine;

namespace Items
{
    public class ItemSpawner : MonoBehaviour
    {
        public static ItemSpawner instance;
        private void Awake() => instance = this;

        private ItemData _data;

        private void Start() => _data = GameManager.Instance.itemData;

        public void SpawnItemHolders(Item item)
        {
            foreach (var holder in _data.itemHolders)
            {
                if (holder.itemType == item.itemType)
                {
                    var itemHolder = Instantiate(holder);
                    itemHolder.item = item;
                }
            }
        }
    }
}