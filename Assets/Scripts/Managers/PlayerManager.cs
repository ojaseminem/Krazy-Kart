using Player;
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
    }
}