using System;
using Managers;
using UnityEngine;
using Utils;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        public bool canMove;

        [SerializeField] private float speed = 5;
        [SerializeField] private float turnSpeed = 360;
        private Vector3 _input;

        private Rigidbody _rb;

        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            if (!canMove) return;

            GatherInput();
            Look();
        }

        private void FixedUpdate()
        {
            if (canMove) Move();
        }

        private void GatherInput()
        {
            _input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        }

        private void Look()
        {
            if (_input == Vector3.zero) return;
        
            var relative = (transform.position + _input.ToIso()) - transform.position;
            var rot = Quaternion.LookRotation(relative, Vector3.up);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, turnSpeed * Time.deltaTime);
        }

        private void Move()
        {
            _rb.MovePosition(
                transform.position + (transform.forward * _input.normalized.magnitude) * speed * Time.deltaTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Task"))
            {
                GameManager.Instance.ChangeState(GameState.Task);
                other.GetComponent<BoxCollider>().enabled = false;
            }

            if (other.CompareTag("LevelCompleted"))
            {
                GameManager.Instance.ChangeState(GameState.ScoreCalculation);
            }
        }
    }
}