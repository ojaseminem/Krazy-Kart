using System.Collections.Generic;
using Items;
using Kart;
using Managers;
using UnityEngine;

namespace Utils
{
    public class TriggerDetector : MonoBehaviour
    {
        [SerializeField] private Transform itemsParent;

        private List<Item> _itemsInCart = new List<Item>();
        private KartController _cart;

        private void Start() => _cart = GetComponent<KartController>();

        private void SpawnTasksInCart()
        {
            //Spawn Item based on item type

            // StartCoroutine(Util.WaitUntilRoutine(() => RunManager.Instance.taskManager.taskCompleted, Spawn));
            Spawn();
            return;

            void Spawn()
            {
                var itemData = RunManager.Instance.itemData;
                var currItem = Instantiate(itemData.currItem, itemsParent);
                _itemsInCart.Add(currItem);

                var currIndex = _itemsInCart.FindIndex(item => item == currItem);
                
                //First
                if (currIndex == 0)
                {
                    var rb = currItem.gameObject.AddComponent<Rigidbody>();
                    StartCoroutine(Util.WaitForSecondsRoutine(.1f, () => rb.isKinematic = true));
                }
                //Rest
                else
                {
                    var previousItem = _itemsInCart[currIndex - 1];
                    var previousPos = previousItem.transform.position;
                    var boundsSize = previousItem.GetComponent<BoxCollider>().bounds.size;
                    
                    currItem.transform.position = new Vector3(previousPos.x, previousPos.y + boundsSize.y, previousPos.z);
                    
                    var hj = currItem.gameObject.AddComponent<HingeJoint>();
                    hj.connectedBody = _itemsInCart[currIndex - 1].GetComponent<Rigidbody>();
                    hj.useSpring = true;
                    var hingeSpring = hj.spring;
                    hingeSpring.spring = 80;
                    hingeSpring.damper = 7;
                    hingeSpring.targetPosition = 0;
                    hj.spring = hingeSpring;

                    hj.autoConfigureConnectedAnchor = false;

                    var previousHj = previousItem.GetComponent<HingeJoint>();

                    if (previousHj != null)
                    {
                        previousHj.autoConfigureConnectedAnchor = true;
                    }
                }
                
                RunManager.Instance.ChangeState(GameState.Playing);
            }
        }

        /*private void OnTriggerEnter(Collider other)
        {
            var canDetect = RunManager.Instance.playerManager.player.canMove;
            
            if(!canDetect) return;
            
            if (other.CompareTag("Task"))
            {
                _cart.BrakeCart();

                var itemHolder = (ItemHolder)other.GetComponentInParent(typeof(ItemHolder));

                RunManager.Instance.itemData.currItem = itemHolder.item;
                itemHolder.zone.SetActive(false);
                
                RunManager.Instance.ChangeState(GameState.Task);

                SpawnTasksInCart();

                other.GetComponent<BoxCollider>().enabled = false;
            }
            
            if (other.CompareTag("Store"))
            {
                var currStore = other.GetComponent<StoreController>().currStoreType;
                RunManager.Instance.itemData.currStore = currStore; 
                PlayerManager.EnteredStore(currStore);
            }

            if (other.CompareTag("SpeedUp"))
            {
                var dir = Vector3.Normalize(other.transform.position - transform.position);
                var dot = Vector3.Dot(other.transform.forward, dir);
                
                if(dot > .3f) _cart.SpeedUp();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Store"))
            {
                PlayerManager.ExitedStore();
            }
        }*/
    }
}