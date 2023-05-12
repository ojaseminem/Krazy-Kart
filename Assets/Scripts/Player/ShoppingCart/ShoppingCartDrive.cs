using System;
using UnityEngine;
using Utils;

namespace Player.ShoppingCart
{
    public class ShoppingCartDrive : MonoBehaviour
    {
        [SerializeField] private CameraController cam;
        public float rotateSpeed = 30;
        public float maxAngle = 30;
        public float maxTorque = 300;
        public GameObject wheelShape;

        public bool canMove;

        private TrailRenderer[] _wheelTrails;
        private WheelCollider[] _wheels;
        private bool _brake = true;
        private float _brakeForce = 5000;
        private Rigidbody _rigid;

        private int _leftOrRight;
        
        private float angle => maxAngle * (Input.GetAxis("Horizontal") + CustomInput.HorizontalInput);

        private float torque => maxTorque * (Input.GetAxis("Vertical") + CustomInput.VerticalInput);

        private bool _isEmitting = false;

        private void Start()
        {
            _wheels = GetComponentsInChildren<WheelCollider>();
            _wheelTrails = GetComponentsInChildren<TrailRenderer>();
            _rigid = GetComponent<Rigidbody>();
            _rigid.centerOfMass = new Vector3(0, -0.9f, 0);

            GenerateWheel();
            Emitting(false);
        }

        private void Emitting(bool enableEmitting)
        {
            foreach (var trail in _wheelTrails)
            {
                if (!enableEmitting)
                {
                    trail.emitting = false;
                    return;
                }

                if (!(trail.transform.localPosition.z > 0)) continue;

                bool isLeft = (angle > 0);
                // Front trail
                if (trail.transform.localPosition.x < 0 && isLeft)
                {
                    // Left
                    trail.emitting = true;
                }
                else if (trail.transform.localPosition.x > 0 && !isLeft)
                {
                    // Right
                    trail.emitting = true;
                }
                else
                {
                    trail.emitting = false;
                }
            }
        }

        private void GenerateWheel()
        {
            foreach (var wheel in _wheels)
            {
                if (wheelShape != null)
                {
                    var ws = GameObject.Instantiate(wheelShape, wheel.transform);

                    if (wheel.transform.localPosition.x < 0f)
                    {
                        var localScale = ws.transform.localScale;

                        localScale = new Vector3(localScale.x * -1f, localScale.y, localScale.z);
                        ws.transform.localScale = localScale;
                    }
                }
            }
        }

        private void Update()
        {
            AdjustBrake();

            if (!canMove) return;

            AdjustCamera();
            
            HandleMovement();

            WheelMovement();
            
            if (torque >= maxTorque)
            {
                Emitting(true);
                _isEmitting = true;
            }
            else if (_isEmitting)
            {
                Emitting(false);
                _isEmitting = false;
            }
        }

        private void HandleMovement()
        {
            if (_leftOrRight == 1)
            {
                if (CustomInput.HorizontalInput < 1f)
                    CustomInput.HorizontalInput += 2f * Time.fixedDeltaTime;
            }
            else if (_leftOrRight == -1)
            {
                if (CustomInput.HorizontalInput > -1f)
                    CustomInput.HorizontalInput -= 2f * Time.fixedDeltaTime;
            }
            else if (_leftOrRight == 0)
            {
                if (CustomInput.HorizontalInput > 0)
                {
                    CustomInput.HorizontalInput -= 2f * Time.fixedDeltaTime;
                }
                else if (CustomInput.HorizontalInput < 0)
                {
                    CustomInput.HorizontalInput += 2f * Time.fixedDeltaTime;
                }
            }
        }

        private void AdjustCamera()
        {
#if UNITY_EDITOR
            var x = Input.GetAxis("Horizontal");
            if (Mathf.Abs(x - 1) < .1f) cam.ToggleCameraDirection(1);
            else if (Mathf.Abs(x + 1) < .1f) cam.ToggleCameraDirection(2);
            else if (Mathf.Abs(x) < .1f) cam.ToggleCameraDirection(0);
#endif

#if UNITY_ANDROID && !UNITY_EDITOR
            //Camera Rotation X
            if (Mathf.Abs(CustomInput.HorizontalInput - 1) < .1f) cam.ToggleCameraDirection(true);
            else if (Mathf.Abs(CustomInput.HorizontalInput + 1) < .1f) cam.ToggleCameraDirection(false);
            else if (Mathf.Abs(CustomInput.HorizontalInput) < .1f) cam.ToggleCameraDirection(false);
#endif
        }

        private void AdjustBrake()
        {
            if (CustomInput.VerticalInput == 0)
            {
                _brake = true;
            }
            else if (Math.Abs(CustomInput.VerticalInput + 1) < .1f || Math.Abs(CustomInput.VerticalInput - 1) < .1f)
            {
                _brake = false;
            }
        }

        private void WheelMovement()
        {
            foreach (WheelCollider wheel in _wheels)
            {
                if (wheel.transform.localPosition.z > 0)
                {
                    // Front wheels
                    wheel.steerAngle = angle;
                    wheel.motorTorque = torque * 0.1f;
                }

                if (wheel.transform.localPosition.z < 0)
                {
                    // Back wheels
                    wheel.motorTorque = _brake ? 0 : torque;
                }

                wheel.brakeTorque = _brake ? _brakeForce : 0;
                if (wheelShape)
                {
                    wheel.GetWorldPose(out var p, out var q);

                    var shapeTransform = wheel.transform.GetChild(0);
                    shapeTransform.position = p;
                    shapeTransform.rotation = q;
                }
            }
        }

        public void BrakeCart()
        {
            foreach (var wheel in _wheels)
            {
                wheel.mass = 100f;
                wheel.brakeTorque = _brakeForce;
                StartCoroutine(Util.WaitForSecondsRoutine(1f, () => wheel.mass = 20f));
            }
        }
        
        public void SetLeftRight(int value) => _leftOrRight = value;

        private void OnCollisionEnter(Collision collision)
        {
        }
    }
}