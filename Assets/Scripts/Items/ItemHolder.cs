using Managers;
using UnityEngine;

namespace Items
{
    public class ItemHolder : MonoBehaviour
    {
        public ItemType itemType;
        public GameObject zone;
        
        [HideInInspector] public Item item;
    }
}