using UnityEngine;

namespace Player
{
    public class CameraController : MonoBehaviour
    {
        public bool followPlayer = true;
        
        [SerializeField] private Transform player;

        private void LateUpdate()
        {
            if(!followPlayer) return;
            
            var position = player.position;
        
            float posX = position.x;
            float posZ = position.z;

            transform.position = new Vector3(posX, position.y, posZ);

        }
    }
}
