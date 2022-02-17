using Mirror;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : NetworkBehaviour
{
    public GameObject target;
    public NavMeshAgent navAgent;
    public Animator m_animator;

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
        m_animator?.SetFloat("Speed", currentSpeed);
        
        if (!isServer) return;

        timeSinceLastCheck += Time.deltaTime;
        if (target == null && timeSinceLastCheck > avatarCheckInterval)
        {
            FindTarget();
        }

        if (target != null)
        {
            if (navAgent.hasPath)
            {
                // Check if target has gotten too far away.
                if (Vector3.Distance(transform.position, target.transform.position) >= breakFollowDistance)
                {
                    RemoveTarget();
                    return;
                }

                // ⚡: maybe don't do this every frame
                currentSpeed = navAgent.velocity.magnitude;
            }

            if (navAgent.remainingDistance <= attackRange)
            {
                // Maybe also stop moving, and face the target?
                if (timeSinceLastAttack >= timeBetweenAttacks)
                {
                    AttackTarget();
                }
            }
        }

        timeSinceLastAttack += Time.deltaTime;
    }

    [ClientRpc]
    private void AttackTarget()
    {
        if (target == null) return;
        
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
        
            var path = new NavMeshPath();
            navAgent.CalculatePath(target.transform.position, path);
            navAgent.SetPath(path);
        }
    }

    [Server]
    private void RemoveTarget()
    {
        target = null;
        navAgent.ResetPath();
    }
}
