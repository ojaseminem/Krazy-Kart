using System;
using Items;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class TaskSlotsController : MonoBehaviour
    {
        [HideInInspector] public int itemPrice;
        
        [SerializeField] private SlotHandler slot1, slot2, slot3;
        [SerializeField] private Transform centerTransform;

        [SerializeField] private ObjectSelectionPopup popup;

        public void SpawnItem(Item item, int slotNumber)
        {
            ClearSlots();
            switch (slotNumber)
            {
                case 1:
                    slot1.InitializeSlot(this, item, centerTransform.localPosition);
                    break;
                case 2:
                    slot2.InitializeSlot(this, item, centerTransform.localPosition);
                    break;
                case 3:
                    slot3.InitializeSlot(this, item, centerTransform.localPosition);
                    break;
            }
        }

        public void ShowObjectSelectionPopup(Action acceptCallback = null, Action rejectCallback = null)
        {
            popup.gameObject.SetActive(true);
            
            popup.itemPrice = itemPrice;
            popup.InitPopup(acceptCallback, rejectCallback);
        }

        public void ClearSlots()
        {
            slot1.ClearSlot();
            slot2.ClearSlot();
            slot3.ClearSlot();
        }
    }
}