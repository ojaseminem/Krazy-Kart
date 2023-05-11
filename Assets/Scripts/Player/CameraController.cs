using Cinemachine;
using UnityEngine;

namespace Player
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera camLeft, camRight;

        [SerializeField] private float minFov, maxFov, defaultFov;
        
        public void ToggleCameraDirection(bool left)
        {
            if (left)
            {
                camRight.Priority = 2;
                camLeft.Priority = 1;
            }
            else
            {
                camRight.Priority = 1;
                camLeft.Priority = 2;
            }
        }

        public void SetFov(float torque, float maxTorque)
        {
            var normalizedSpeed = Mathf.Clamp01(torque / maxTorque);
            var fov = torque < .1f ? defaultFov : Mathf.SmoothStep(minFov, maxFov, normalizedSpeed);

            camLeft.m_Lens.FieldOfView = fov;
            camRight.m_Lens.FieldOfView = fov;
        }
    }
}
