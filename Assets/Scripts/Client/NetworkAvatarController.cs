using System.Collections;
using Mirror;
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

        [SerializeField] private float dodgeDistance = 10.0f;
        [SerializeField] private float dodgeDuration = 0.5f;
        private bool isDodging = false;
        private Vector3 dodgeTarget;
        private Vector3 dodgeVelocity;

        private Vector3 inputVelocity;
        private Animator animator;

        [SyncVar]
        public float currentSpeed;

        [SerializeField]
        protected float timeBetweenAttacks = 0.333f;

        [SerializeField] protected float powerAttackEnergyCost = 100f;
        public float PowerAttackEnergyCost => powerAttackEnergyCost;

        public float TimeBetweenAttacks => timeBetweenAttacks;
        
        protected NetworkAvatar[] avatars;

        private void Start()
        {
            animator = GetComponentInChildren<Animator>();
            navAgent = GetComponent<NavMeshAgent>();
            avatars = FindObjectsOfType<NetworkAvatar>();
        }

        private void Update()
        {
            if (isDodging)
            {
                var position = transform.position;
                var newPosition = Vector3.SmoothDamp(position, dodgeTarget, ref dodgeVelocity, dodgeDuration);
                navAgent.Move(newPosition - position);
            }
            else if (inputVelocity != Vector3.zero)
            {
                if (inputVelocity.sqrMagnitude >= 1f) inputVelocity.Normalize();

                var movement = inputVelocity * movementSpeed * Time.deltaTime;
                navAgent.Move(movement);

                // Turn toward movement direction
                var targetAngle = Mathf.Atan2(movement.x, movement.z) * Mathf.Rad2Deg;
                var angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity,
                    turnSmoothTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                // Reset input
                inputVelocity = Vector3.zero;
            }
        }

        [Command]
        private void SetCurrentSpeed(float speed)
        {
            currentSpeed = speed;
        }

        public void LateUpdate()
        {
            animator.SetFloat("Speed", currentSpeed);
        }

        public virtual void Move(Vector3 movementDirection)
        {
            var speed = movementDirection.sqrMagnitude;

            if (NavMesh.SamplePosition(transform.position + movementDirection, out var navMeshHit, 100f,
                NavMesh.AllAreas))
            {
                inputVelocity = movementDirection;
            }
            
            SetCurrentSpeed(speed);
        }

        public virtual void Dodge(Vector3 dodgeDirection)
        {
            // todo: use inputVelocity to determine dodge direction.
            isDodging = true;
            dodgeTarget = transform.position + dodgeDistance * dodgeDirection.normalized;
            StartCoroutine(DodgeTimer());
        }

        public virtual void CmdAttack()
        {
            Debug.LogError("Attack not implemented");
        }

        public virtual void CmdPowerAttack()
        {
            Debug.LogError("Power attack not implemented");
        }

        IEnumerator DodgeTimer()
        {
            yield return new WaitForSeconds(dodgeDuration);
            isDodging = false;
        }
    }
}
