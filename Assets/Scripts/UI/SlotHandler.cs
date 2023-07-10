using DG.Tweening;
using Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class SlotHandler : MonoBehaviour
    {
        [SerializeField] private TaskSlotsController taskSlotsController;
        
        [SerializeField] private Image taskImage;
        [SerializeField] private TextMeshProUGUI taskPrice;

        private Vector3 _initialPos;
        private Vector3 _centerPos;

        private int _itemPrice;

        [SerializeField] private Button registerButton;

        public void InitializeSlot(TaskSlotsController tsc, Item item, Vector3 centerPos)
        {
            ClearSlot();
            
            taskSlotsController = tsc;
            _centerPos = centerPos;

            taskImage.sprite = item.itemSprite;
            _itemPrice = item.itemPrice;
            taskPrice.text = _itemPrice.ToString();
            
            registerButton.onClick.AddListener(RegisterSlot);
        }

        public void RegisterSlot()
        {
            _initialPos = transform.localPosition;

            transform.DOLocalMove(_centerPos, .5f)
                .OnComplete(() =>
                {
                    transform.DOScale(1.2f, .5f);
                    taskSlotsController.ShowObjectSelectionPopup(
                        () =>DeRegisterSlot(true),
                        () => DeRegisterSlot(false));
                });

            taskSlotsController.itemPrice = _itemPrice;
            
            registerButton.onClick.RemoveAllListeners();
        }

        private void DeRegisterSlot(bool accepted)
        {
            transform.DOScale(1f, .5f)
                .OnComplete(() => transform.DOLocalMove(_initialPos, .5f));
            
            registerButton.onClick.AddListener(RegisterSlot);
        }

        public void ClearSlot()
        {
            taskImage.sprite = null;
            taskPrice.text = "";
        }
    }
}