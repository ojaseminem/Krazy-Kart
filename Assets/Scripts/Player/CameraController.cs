using Cinemachine;
using UnityEngine;

namespace Player
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera camLeft, camRight, camMiddle;

        public void ToggleCameraDirection(int middleLeftRight)
        {
            switch (middleLeftRight)
            {
                case 0:
                    camMiddle.Priority = 2;
                    camLeft.Priority = 1;
                    camRight.Priority = 1;
                    break;
                case 1:
                    camMiddle.Priority = 1;
                    camLeft.Priority = 2;
                    camRight.Priority = 1;
                    break;
                case 2:
                    camMiddle.Priority = 1;
                    camLeft.Priority = 1;
                    camRight.Priority = 2;
                    break;
            }
        }
    }
}
