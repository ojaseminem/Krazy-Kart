using System;
using UnityEngine;

namespace Player
{
    public class KartInput : MonoBehaviour
    {
        private UserInputActions _input;

        [HideInInspector] public Vector2 moveInput;
        [HideInInspector] public Vector2 lookInput;

        [HideInInspector] public bool boostInput;
        [HideInInspector] public bool driftInput;
        [HideInInspector] public bool fireInput;

        private void Awake()
        {
            _input = new UserInputActions();

            // Move and Look
            _input.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
            _input.Player.Move.canceled += ctx => moveInput = Vector2.zero;

            _input.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
            _input.Player.Look.canceled += ctx => lookInput = Vector2.zero;

            // Boost
            _input.Player.Boost.performed += ctx => boostInput = true;
            _input.Player.Boost.canceled += ctx => boostInput = false;

            // Drift
            _input.Player.Drift.performed += ctx => driftInput = true;
            _input.Player.Drift.canceled += ctx => driftInput = false;

            // Fire
            _input.Player.Fire.performed += ctx => fireInput = true;
            _input.Player.Fire.canceled += ctx => fireInput = false;
        }

        private void OnEnable()
        {
            _input.Enable();
        }

        private void OnDisable()
        {
            _input.Disable();
        }
    }
}