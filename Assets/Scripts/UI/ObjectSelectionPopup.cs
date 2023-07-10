using System;
using Managers;
using UnityEngine;
using UnityEngine.UI;
using Popups = UI.Parent.Popups;

namespace UI
{
    public class ObjectSelectionPopup : Popups
    {
        [Header("Selected Item Details")]
        
        [HideInInspector] public int itemPrice;
        
        [SerializeField] private Button acceptBtn;
        [SerializeField] private Button rejectBtn;
        [SerializeField] private Button closeButton;

        [SerializeField] private GameObject blocker;
        
        private GameManager _gameManager;

        private bool _accepted;

        private void Awake()
        {
            AppearAction += Appear;
            DisappearAction += Disappear;
            
            _gameManager = GameManager.Instance;
        }

        private void Appear() => blocker.SetActive(false);

        private void Disappear() => blocker.SetActive(true);

        public void InitPopup(Action acceptCallback = null, Action rejectCallback = null)
        {
            OnOpen();

            acceptBtn.onClick.AddListener(CallAccept);
            acceptBtn.onClick.AddListener((() => acceptCallback?.Invoke()));
            
            rejectBtn.onClick.AddListener(CallReject);
            rejectBtn.onClick.AddListener((() => rejectCallback?.Invoke()));
            
            closeButton.onClick.AddListener(CallReject);
            closeButton.onClick.AddListener((() => rejectCallback?.Invoke()));
        }

        private void CallAccept()
        {
            //_gameManager.economyManager.Spend(EconomyType.Coin, itemPrice);
            _gameManager.taskManager.taskCompleted = true;
            _accepted = true;
            OnClose();
            ClearListeners();
        }

        private void CallReject()
        {
            _accepted = false;
            print("Reject Buy");
            OnClose();
            ClearListeners();
        }

        private void ClearListeners()
        {
            acceptBtn.onClick.RemoveAllListeners();
            rejectBtn.onClick.RemoveAllListeners();
            closeButton.onClick.RemoveAllListeners();
        }

        protected override void OnDisappear()
        {
            if (_accepted)
            {
                print("Confirm Buy");
                _gameManager.taskManager.ChangeTaskState(TaskState.PostTask);
            }
            base.OnDisappear();
        }
    }
}