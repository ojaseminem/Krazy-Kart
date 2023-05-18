using Player.ShoppingCart;
using UnityEngine;

namespace Managers
{
    public class PlayerManager : MonoBehaviour
    {
        public ShoppingCartDrive player;

        public void SetPlayerMove(bool toggle) => player.canMove = toggle;

        public static void EnteredStore(StoreType storeType) => GameManager.Instance.storeManager.CheckStore(storeType);

        public static void ExitedStore() => GameManager.Instance.storeManager.indicate = false;
    }
}