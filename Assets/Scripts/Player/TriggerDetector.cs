using System.Collections.Generic;
using Items;
using Managers;
using Player.ShoppingCart;
using Store;
using UnityEngine;
using Utils;

namespace Player
{
    public class TriggerDetector : MonoBehaviour
    {
        [SerializeField] private Transform itemsParent;

        private List<Item> _itemsInCart = new List<Item>();
        private ShoppingCartDrive _cart;

        private void Start() => _cart = GetComponent<ShoppingCartDrive>();

        private void SpawnTasksInCart(ItemHolder itemHolder)
        {
            //Spawn Item based on item type

            StartCoroutine(Util.WaitUntilRoutine(() => !GameManager.instance.sentenceManager.checkForInput, Spawn));

            void Spawn()
            {
                var currItem = Instantiate(itemHolder.item, itemsParent);
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
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            var canDetect = GameManager.instance.playerManager.player.canMove;
            
            if(!canDetect) return;
            
            if (other.CompareTag("Task"))
            {
                _cart.BrakeCart();
                GameManager.instance.ChangeState(GameState.Task);

                var itemHolder = (ItemHolder)other.GetComponentInParent(typeof(ItemHolder));
                
                itemHolder.zone.SetActive(false);
                
                SpawnTasksInCart(itemHolder);

                other.GetComponent<BoxCollider>().enabled = false;
            }
            
            if (other.CompareTag("Store")) PlayerManager.EnteredStore(other.GetComponent<StoreController>().currStoreType);
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Store")) PlayerManager.ExitedStore();
        }
    }
}