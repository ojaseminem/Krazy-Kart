using System;
using UnityEngine;

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
    
        private float Angle => maxAngle * Input.GetAxis("Horizontal");
        private float Torque => maxTorque * Input.GetAxis("Vertical");
    
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
            
                bool isLeft = (Angle > 0);
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

            if(!canMove) return;
            //Rotate();
        
            WheelMovement();
            if (Torque >= maxTorque) {
                Emitting(true);
                _isEmitting = true;
            } else if (_isEmitting) {
                Emitting(false);
                _isEmitting = false;
            }
        }

        private void AdjustBrake()
        {
            if (Input.GetButtonUp("Vertical"))
            {
                _brake = true;
            }
            else if (Input.GetButtonDown("Vertical"))
            {
                _brake = false;
            }
        }

        /*private void Rotate()
        {
            if (_torque < 10)
            {
                transform.Rotate(0.0f, Input.GetAxis("Horizontal") * rotateSpeed, 0.0f);
            }
        }*/

        private void WheelMovement()
        {
            foreach (WheelCollider wheel in _wheels)
            {
                if (wheel.transform.localPosition.z > 0)
                {
                    // Front wheels
                    wheel.steerAngle = Angle;
                    wheel.motorTorque = Torque * 0.1f;
                }

                if (wheel.transform.localPosition.z < 0)
                {
                    // Back wheels
                    wheel.motorTorque = _brake ? 0 : Torque;
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
                StartCoroutine(Utils.Util.WaitForSecondsRoutine(1f, () => wheel.mass = 20f));
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            
        }
    }
}
