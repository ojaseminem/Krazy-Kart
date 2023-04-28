using UnityEngine;

namespace Player
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Transform player;

        private void LateUpdate()
        {
            var position = player.position;
        
            float posX = position.x;
            float posZ = position.z;

            transform.position = new Vector3(posX, position.y, posZ);

        }
    }
}
