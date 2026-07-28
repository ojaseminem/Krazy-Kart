using System.Collections;
using UnityEngine;

namespace Kart
{
    public class KartController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CameraController cameraController;
        [SerializeField] private KartInput inputHandler;
        [SerializeField] private Transform itemsParent;
        public GameObject wheelShape;

        [Header("Movement Settings")]
        public float maxAngle = 30f;
        public float maxTorque = 300f;
        public float defaultSpeedTorque = 1f;
        public float maxSpeedUpTorque = 3f;
        public float speedUpTime = 4f;
        public float boostForce = 100f;
        public ForceMode forceMode;
        private float _speedUpTorque = 1f;

        [Header("Wheel Settings")]
        private WheelCollider[] _wheels;
        private TrailRenderer[] _wheelTrails;
        private Rigidbody _rigid;
        private bool _isEmitting = false;

        [Header("Brake")]
        private bool _brake = true;
        private float _brakeForce = 5000f;

        [Header("State")]
        public bool canMove;
        
        private void Start()
        {
            _wheels = GetComponentsInChildren<WheelCollider>();
            _wheelTrails = GetComponentsInChildren<TrailRenderer>();
            _rigid = GetComponent<Rigidbody>();
            _rigid.centerOfMass = new Vector3(0, -0.9f, 0);

            GenerateWheels();
            Emitting(false);
            
            cameraController?.Initialize(transform);
        }

        #region Input Computed Properties
        private float Angle => maxAngle * inputHandler.moveInput.x;
        private float Torque => maxTorque * inputHandler.moveInput.y;
        #endregion
        
        private void Update()
        {
            if (!canMove) return;

            AdjustBrake();
            HandleWheelMovement();

            // Boost
            if (inputHandler.boostInput) SpeedUp();

            // Fire
            if (inputHandler.fireInput) FireCart();

            // Drift
            if (inputHandler.driftInput) ApplyDrift();

            // Trail emission
            if (Torque >= maxTorque) Emitting(true);
            else if (_isEmitting) Emitting(false);
        }

        /// Current planar speed in m/s. Used by scoring and camera-independent gameplay systems.
        public float SpeedKph => _rigid != null
            ? new Vector3(_rigid.linearVelocity.x, 0f, _rigid.linearVelocity.z).magnitude * 3.6f
            : 0f;

        public bool IsDrifting => inputHandler != null && inputHandler.driftInput;
        public bool IsBoosting => inputHandler != null && inputHandler.boostInput;
        
        #region Wheel and Trail Logic
        private void HandleWheelMovement()
        {
            foreach (var wheel in _wheels)
            {
                if (wheel.transform.localPosition.z > 0)
                {
                    // Front wheels
                    wheel.steerAngle = Angle;
                    wheel.motorTorque = Torque * 0.1f * _speedUpTorque;
                }
                else
                {
                    // Back wheels
                    wheel.motorTorque = _brake ? 0 : Torque * _speedUpTorque;
                }

                wheel.brakeTorque = _brake ? _brakeForce : 0;

                if (wheelShape)
                {
                    wheel.GetWorldPose(out var pos, out var rot);
                    var shape = wheel.transform.GetChild(0);
                    shape.position = pos;
                    shape.rotation = rot;
                }
            }
        }

        private void Emitting(bool enable)
        {
            _isEmitting = enable;
            foreach (var trail in _wheelTrails)
            {
                if (!enable) { trail.emitting = false; continue; }
                bool isLeft = Angle > 0;
                if (trail.transform.localPosition.x < 0 && isLeft) trail.emitting = true;
                else if (trail.transform.localPosition.x > 0 && !isLeft) trail.emitting = true;
                else trail.emitting = false;
            }
        }

        private void GenerateWheels()
        {
            foreach (var wheel in _wheels)
            {
                if (!wheelShape) continue;

                var ws = Instantiate(wheelShape, wheel.transform);
                if (wheel.transform.localPosition.x < 0)
                {
                    var scale = ws.transform.localScale;
                    ws.transform.localScale = new Vector3(-scale.x, scale.y, scale.z);
                }
            }
        }
        #endregion

        #region Boost / Drift / Fire
        public void SpeedUp()
        {
            _rigid.AddForce(transform.forward * boostForce, forceMode);
            StartCoroutine(SpeedUpCoroutine());
        }

        private IEnumerator SpeedUpCoroutine()
        {
            float elapsed = 0f;
            _speedUpTorque = maxSpeedUpTorque;

            while (elapsed < speedUpTime)
            {
                _speedUpTorque = Mathf.Lerp(maxSpeedUpTorque, defaultSpeedTorque, elapsed / speedUpTime);
                elapsed += Time.deltaTime;
                yield return null;
            }
            _speedUpTorque = defaultSpeedTorque;
        }

        private void ApplyDrift()
        {
            // Example: Reduce wheel friction or add sideways force
            foreach (var wheel in _wheels) { wheel.brakeTorque = _brakeForce * 0.5f; }
        }

        private void FireCart()
        {
            // Example: shoot a projectile / apply effect
            Debug.Log("Fire Action Triggered!");
        }
        #endregion

        #region Game Over Boost

        public void GameOverBoost()
        {
            SpeedUp();
            
            if(itemsParent.childCount > 0) itemsParent.GetChild(0).GetComponent<Rigidbody>().isKinematic = false;

            var hj = itemsParent.GetComponentsInChildren<HingeJoint>();
            if(hj != null) foreach (var joint in hj) Destroy(joint);
            
            var col = itemsParent.GetComponentsInChildren<BoxCollider>();
            if (col == null) return;
            foreach (var boxCollider in col) boxCollider.isTrigger = false;
        }

        #endregion

        #region Brake
        private void AdjustBrake()
        {
            _brake = Mathf.Abs(inputHandler.moveInput.y) < 0.1f;
        }
        #endregion

    }
}