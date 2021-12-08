using System;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

namespace Client
{
    public abstract class NetworkAvatarController : NetworkBehaviour
    {
        private NavMeshAgent navAgent;

        [SerializeField] private float movementSpeed = 4.0f;
        [SerializeField] private float turnSmoothTime = 0.05f;
        private float turnSmoothVelocity;

        private Vector3 inputVelocity;
        private Camera mainCamera;

        private Animator animator;

        [SyncVar]
        public float currentSpeed;

        private void Start()
        {
            animator = GetComponentInChildren<Animator>();
            navAgent = GetComponent<NavMeshAgent>();
            mainCamera = Camera.main;
        }

        private void Update()
        {
            currentSpeed = 0f;

            if (inputVelocity.sqrMagnitude >= 0.1f)
            {
                if (inputVelocity.sqrMagnitude >= 1f) inputVelocity.Normalize();

                var movement = inputVelocity * movementSpeed * Time.deltaTime;
                navAgent.Move(movement);

                // Turn toward movement direction
                var targetAngle = Mathf.Atan2(movement.x, movement.z) * Mathf.Rad2Deg;
                var angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity,
                    turnSmoothTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                // Update animator
                currentSpeed = inputVelocity.sqrMagnitude;

                // Reset input
                inputVelocity = Vector3.zero;
            }
            
            animator.SetFloat("Speed", currentSpeed);
        }

        public void LateUpdate()
        {
            // Debug.Log($"SetFloat({currentSpeed}) on {gameObject.name} animator");
            // animator.SetFloat("Speed", currentSpeed);
        }

        public virtual void Move(Vector3 movementDirection)
        {
            if (NavMesh.SamplePosition(transform.position + movementDirection, out var navMeshHit, 100f, NavMesh.AllAreas))
            {
                inputVelocity = movementDirection;
            }
        }

        public virtual void Dodge(Vector2 direction)
        {
            Debug.LogError("Dodge not implemented");
        }

        public virtual void CmdAttack()
        {
            Debug.LogError("Attack not implemented");
        }

        public virtual void PowerAttack()
        {
            Debug.LogError("Power attack not implemented");
        }
    }
}
