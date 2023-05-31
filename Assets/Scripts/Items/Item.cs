using DG.Tweening;
using Managers;
using UnityEngine;

namespace Items
{
    public class Item : MonoBehaviour
    {
        [Header("Item Details")]
        public ItemType itemType;

        [Header("Item Selection Details")]
        public bool canSelect;
        public Vector3 rotationSpeed;
        [HideInInspector] public Tween RotationTween;
        
        public void SpawnedForSelection()
        {
            Debug.Log($"Item Spawned :: {itemType}");
            
            Rotate();
        }

        private void Rotate()
        {
            RotationTween = transform
                .DORotate(transform.rotation.eulerAngles + rotationSpeed, 1f, RotateMode.LocalAxisAdd)
                .SetLoops(-1, LoopType.Yoyo);
        }
    }
}