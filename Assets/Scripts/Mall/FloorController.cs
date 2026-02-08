using System.Collections.Generic;
using Items;
using UnityEngine;

namespace Mall
{
    public class FloorController : MonoBehaviour
    {
        public float floorHeight = 11f;
        
        [SerializeField] private FloorTriggerController entryFloorTrigger;
        [SerializeField] private FloorTriggerController exitFloorTrigger;
        

        // [SerializeField] private List<Item> itemsToSpawn;
        
        
        public void InitializeFloor()
        {
            // Random with a Seed passed in - Spawn Procedural assets - Actually let's do this one later
            
            
        }

        private void InitializeItemsToSpawn()
        {
            // Randomly enable Items placed in world for collecting
            // Call UIManager and trigger UI call to show Items on Player HUD
        }

        private void SetCanExitFloor()
        {
            exitFloorTrigger.SetIsColliderTrigger(true);
        }
        
        private void EnableTriggerInteraction()
        {
            entryFloorTrigger.enabled = true;
        }
        private void EnableExitInteraction()
        {
            exitFloorTrigger.enabled = true;
        }
        private void DisableTriggerInteraction()
        {
            entryFloorTrigger.enabled = false;
            exitFloorTrigger.enabled = false;
        }
    }
}