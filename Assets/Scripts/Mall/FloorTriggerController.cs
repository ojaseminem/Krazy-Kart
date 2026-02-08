using System;
using Managers;
using UnityEngine;

namespace Mall
{
    [RequireComponent(typeof(BoxCollider))]
    public class FloorTriggerController : MonoBehaviour
    {
        public FloorTriggerType floorTriggerType;

        private void Start()
        {
            Debug.Log("Floor Trigger type for GameObject - " + gameObject.name + " is set to - " + floorTriggerType);
        }

        public void SetIsColliderTrigger(bool isTrigger)
        {
            var col = GetComponent<BoxCollider>();
            col.isTrigger = isTrigger;
        }
    }
}