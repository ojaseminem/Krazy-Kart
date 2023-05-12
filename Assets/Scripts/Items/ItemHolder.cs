using Managers;
using UnityEngine;

namespace Items
{
    public class ItemHolder : MonoBehaviour
    {
        public ItemType itemType;

        [HideInInspector] public Item item;
        [HideInInspector] public GameObject zone;
    }
}