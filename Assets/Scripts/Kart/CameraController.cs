using Cinemachine;
using UnityEngine;

namespace Kart
{
    [RequireComponent(typeof(CinemachineVirtualCamera))]
    public class CameraController : MonoBehaviour
    {
        [Header("Cameras")]
        [SerializeField] private CinemachineVirtualCamera mainCam;

        [Header("Camera Settings")]
        [SerializeField] private float followSpeed = 5f;
        [SerializeField] private float lookSpeed = 3f;
        [SerializeField] private float tiltAngle = 10f;
        [SerializeField] private float maxDistance = 6f;
        [SerializeField] private float minDistance = 3f;
        [SerializeField] private float speedDistanceFactor = 0.5f;

        [Header("Camera Effects")]
        [SerializeField] private float boostFOV = 75f;
        [SerializeField] private float normalFOV = 60f;
        [SerializeField] private float fovTransitionSpeed = 5f;
        [SerializeField] private float boostShakeAmount = 0.2f;
        [SerializeField] private float driftTiltAmount = 15f;

        private Transform _target;
        private Rigidbody _targetRb;
        private KartInput _inputHandler;
        private Vector3 _currentOffset;
        private Vector3 _desiredOffset;
        private CinemachineComponentBase _composer;

        private void Awake()
        {
            if (mainCam != null)
                _composer = mainCam.GetCinemachineComponent(CinemachineCore.Stage.Body);
        }

        public void Initialize(Transform kartTransform)
        {
            _target = kartTransform;
            _targetRb = _target.GetComponent<Rigidbody>();
            _inputHandler = _target.GetComponent<KartInput>();

            _currentOffset = mainCam.transform.position - _target.position;
            _desiredOffset = _currentOffset;
        }

        private void LateUpdate()
        {
            if (!_target || _inputHandler == null) return;

            HandleDynamicCamera();
            ApplyBoostAndDriftEffects();
            // ApplyLookInput(); - Removed Mouse Input for Cam
        }

        #region Follow & Tilt
        private void HandleDynamicCamera()
        {
            // Adjust distance based on speed
            float speed = _targetRb ? _targetRb.linearVelocity.magnitude : 0f;
            float dynamicDistance = Mathf.Lerp(minDistance, maxDistance, speed * speedDistanceFactor / 20f);
            _desiredOffset = -_target.forward * dynamicDistance + Vector3.up * 2f;

            // Smooth follow
            _currentOffset = Vector3.Lerp(_currentOffset, _desiredOffset, followSpeed * Time.deltaTime);
            mainCam.transform.position = Vector3.Lerp(mainCam.transform.position, _target.position + _currentOffset, followSpeed * Time.deltaTime);

            // Look at kart
            Vector3 lookTarget = _target.position + Vector3.up * 1.5f;
            mainCam.transform.rotation = Quaternion.Slerp(mainCam.transform.rotation,
                Quaternion.LookRotation(lookTarget - mainCam.transform.position), lookSpeed * Time.deltaTime);

            // Base tilt on horizontal movement
            float tilt = -_inputHandler.moveInput.x * tiltAngle;
            mainCam.transform.Rotate(Vector3.forward, tilt * Time.deltaTime, Space.Self);
        }
        #endregion

        #region Look Input - Not needed anymore. Removing Mouse input for Cam control
        /*private void ApplyLookInput()
        {
            // Rotate camera around kart based on look input
            Vector3 right = _target.right;
            Vector3 up = Vector3.up;
            mainCam.transform.position += right * _inputHandler.lookInput.x * 0.5f + up * _inputHandler.lookInput.y * 0.2f;
        }*/
        #endregion

        #region Boost and Drift Effects
        private void ApplyBoostAndDriftEffects()
        {
            CinemachineVirtualCamera vcam = mainCam;
            if (!vcam) return;

            // Adjust FOV for Boost
            var lens = vcam.m_Lens;
            float targetFOV = _inputHandler.boostInput ? boostFOV : normalFOV;
            lens.FieldOfView = Mathf.Lerp(lens.FieldOfView, targetFOV, fovTransitionSpeed * Time.deltaTime);
            vcam.m_Lens = lens;

            // Camera shake for Boost
            if (_inputHandler.boostInput)
            {
                Vector3 shake = new Vector3(
                    Random.Range(-boostShakeAmount, boostShakeAmount),
                    Random.Range(-boostShakeAmount, boostShakeAmount),
                    0);
                mainCam.transform.position += shake;
            }

            // Drift tilt
            if (_inputHandler.driftInput)
            {
                float driftTilt = _inputHandler.moveInput.x * driftTiltAmount;
                mainCam.transform.Rotate(Vector3.forward, driftTilt * Time.deltaTime, Space.Self);
            }
        }
        #endregion
    }
}
