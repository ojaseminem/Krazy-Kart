using Player;
using Player.ShoppingCart;
using UnityEngine;

namespace Managers
{
    public class PlayerManager : MonoBehaviour
    {
        public ShoppingCartDrive player;

        public void SetPlayerMove(bool toggle)
        {
            player.canMove = toggle;
        }
        
        //TODO add a functionality to add items as hinge joints in cart
    }
}