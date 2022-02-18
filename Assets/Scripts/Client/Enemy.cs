using System;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : NetworkBehaviour
{
    public GameObject target;
    public NavMeshAgent navAgent;
    public Animator m_animator;
    public NetworkAnimator m_NetworkAnimator;
    private Vector3 targetPosition;

    [SerializeField] private float detectionRadius = 10f;
    [SerializeField] private float breakFollowDistance = 15f;
    [SerializeField] private float avatarCheckInterval = 0.5f;
    [SerializeField] private float attackRange = 1.0f;
    [SerializeField] private LayerMask avatarLayerMask;
    private float timeSinceLastCheck = 0f;
    private Collider[] nearbyAvatars;

    [SerializeField] private float attackDamage = 20f;
    [SerializeField] private float timeBetweenAttacks = 3.0f;
    private float timeSinceLastAttack = 0f;

    [SyncVar] private float currentSpeed;

    // Start is called before the first frame update
    void Start()
    {
        navAgent = GetComponentInChildren<NavMeshAgent>();
    }

    public override void OnStartServer()
    {
        nearbyAvatars = new Collider[3];
    }

    private void Update()
    {
        if (!isServer) return;
        
        m_animator?.SetFloat("Speed", currentSpeed);
        
        // ⚡: maybe don't do this every frame
        currentSpeed = navAgent.velocity.magnitude;

        timeSinceLastCheck += Time.deltaTime;
        timeSinceLastAttack += Time.deltaTime;
        
        if (target == null && timeSinceLastCheck > avatarCheckInterval)
        {
            FindTarget();
        }

        if (target != null)
        {
            var distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
            var distanceTargetMoved = Vector3.Distance(target.transform.position, targetPosition);
            
            if (navAgent.hasPath)
            {
                // Check if target has gotten too far away.
                if (distanceToTarget >= breakFollowDistance)
                {
                    RemoveTarget();
                    return;
                }
            }
            
            // If target has moved out of attack range of original position, repath.
            if (distanceTargetMoved > attackRange)
            {
                UpdatePath();
            }

            if (distanceToTarget <= attackRange)
            {
                // Bail on the rest of our path.
                navAgent.ResetPath();
                
                // TODO: Face target
                Debug.Log("within attack distance, attack target");
                
                if (timeSinceLastAttack >= timeBetweenAttacks)
                {
                    AttackTarget();
                }
            }
        }
    }

    [ClientRpc]
    private void AttackTarget()
    {
        if (target == null) return;
        
        m_NetworkAnimator?.SetTrigger("Attack");
        target.SendMessage("TakeDamage", attackDamage);
        timeSinceLastAttack = 0f;
    }

    [Server]
    private void FindTarget()
    {
        var numColliders = Physics.OverlapSphereNonAlloc(transform.position, detectionRadius, nearbyAvatars, avatarLayerMask);

        // Just get the first, if there is one.
        if (numColliders > 0)
        {
            target = nearbyAvatars[0].gameObject;
            UpdatePath();
        }
    }

    [Server]
    private void UpdatePath()
    {
        if (target == null) return;
        
        targetPosition = target.transform.position;
        var path = new NavMeshPath();
        navAgent.CalculatePath(targetPosition, path);
        navAgent.SetPath(path);
        navAgent.updateRotation = true;
    }

    [Server]
    private void RemoveTarget()
    {
        target = null;
        navAgent.ResetPath();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
