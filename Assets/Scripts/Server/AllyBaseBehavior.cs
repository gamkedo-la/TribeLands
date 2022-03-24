using Client;
using Mirror;
using UnityEngine;
using UnityEngine.AI;

public class AllyBaseBehavior : NetworkBehaviour
{
    public GameObject target;
    public NetworkAvatar targetAvatar;
    private Vector3 targetPosition;
    public NavMeshAgent navAgent;
    public Animator m_animator;
    public NetworkAnimator m_NetworkAnimator;
    public NetworkAvatarController networkAvatarController;

    public bool alliedFollow;
    public bool hostileFollow;
    public bool hostileAttack;

    [SerializeField] private float followRadius = 1.0f;
    [SerializeField] private LayerMask followLayerMask;
    [SerializeField] [Range(0, 1)] private float rotationSmoothing = 0.2f;
    private Collider[] targetPool;
    private int targetPoolSize = 2;

    [SerializeField] private LayerMask enemyLayerMask;
    [SerializeField] private float attackRange;
    [SerializeField] private Transform fireFrom;
    private Collider[] enemyTargetPool;
    [SerializeField] private float enemyDetectionRadius = 30f;
    public GameObject targetEnemy;
    private int enemyTargetPoolSize = 4;
    private float timeSinceLastAttack = 0f;

    [SerializeField] private float targetCheckInterval = 0.5f;
    [SerializeField] private float targetDetectionRadius = 100f;
    private float timeSinceLastCheck = 0f;

    enum AIState
    {
        Attacking,
        Following,
        Searching,
        Standby,
    };

    private AIState currentState = AIState.Searching;
    
    // Start is called before the first frame update
    public override void OnStartServer()
    {
        targetPool = new Collider[targetPoolSize];
        enemyTargetPool = new Collider[enemyTargetPoolSize];
    }

    // Update is called once per frame
    void Update()
    {
        if (!isServer) return;

        timeSinceLastCheck += Time.deltaTime;
        timeSinceLastAttack += Time.deltaTime;
        
        switch (currentState)
        {
            case AIState.Attacking:
                Attacking();
                break;
            case AIState.Following:
                Following();
                break;
            case AIState.Searching:
                Searching();
                break;
            case AIState.Standby:
                Standby();
                break;
        }
    }

    [Server]
    protected virtual void Searching()
    {
        if (timeSinceLastCheck > targetCheckInterval)
        {
            // Try first to find nearby enemies.
            if (FindEnemyTarget())
            {
                currentState = AIState.Attacking;
            }
            // Otherwise try to find a player to follow.
            // else if (FindTarget())
            // {
                // currentState = AIState.Following;
            // }
        }
    }

    [Server]
    protected virtual void Following()
    {
        m_animator?.SetFloat("Speed", navAgent.velocity.sqrMagnitude);
        
        var distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
        var distanceTargetMoved = Vector3.Distance(target.transform.position, targetPosition);
        
        // If target has moved out of range, repath.
        if (distanceTargetMoved > followRadius) UpdatePath();

        if (distanceToTarget <= followRadius)
        {
            navAgent.ResetPath();
            currentState = AIState.Standby;
        }
        
        // Avatar we're following is no longer player-controlled. Find a new target!
        if (targetAvatar != null && !targetAvatar.isControlled)
        {
            currentState = AIState.Searching;
        }
    }
    
    [Server]
    protected virtual void Attacking()
    {
        if (targetEnemy == null)
        {
            currentState = AIState.Searching;
            return;
        }

        var targetDirection = targetEnemy.transform.position - fireFrom.position;
        var targetDistance = targetDirection.magnitude;
        if (targetDistance < attackRange)
        {
            navAgent.ResetPath();
            
            if (timeSinceLastAttack >= networkAvatarController.TimeBetweenAttacks)
            {
                networkAvatarController.attackDirection = Quaternion.LookRotation(targetDirection.normalized);
                networkAvatarController.ServerAttack();
                m_NetworkAnimator.SetTrigger("Attack");
                timeSinceLastAttack = 0;
            }
        }

        else
        {
            UpdatePath(targetEnemy.transform);
        }
    }

    protected virtual void Standby()
    {
        if (target == null)
        {
            currentState = AIState.Searching;
            return;
        }
        
        var targetVector = (target.transform.position - transform.position);
        var distance = targetVector.magnitude;

        if (distance > 0.01f)
        {
            var direction = targetVector.normalized;
            var targetRotation = Quaternion.LookRotation(direction, transform.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSmoothing);
        }

        if (distance > followRadius)
        {
            currentState = AIState.Following;
        }
    }
    
    [Server]
    private bool FindEnemyTarget()
    {
        var numColliders =
            Physics.OverlapSphereNonAlloc(transform.position, enemyDetectionRadius, enemyTargetPool, enemyLayerMask);

        if (numColliders > 0)
        {
            int nearestIndex = 0;
            if (numColliders > 1)
            {
                float nearestDistance = (transform.position - enemyTargetPool[0].transform.position).sqrMagnitude;
                for (int i = 1; i < numColliders; i++)
                {
                    var distance = (transform.position - enemyTargetPool[0].transform.position).sqrMagnitude;
                    if (distance < nearestDistance) nearestIndex = i;
                }
            }

            targetEnemy = enemyTargetPool[nearestIndex].gameObject;
            return true;
        }

        return false;
    }

    [Server]
    private bool FindTarget()
    {
        var numColliders =
            Physics.OverlapSphereNonAlloc(transform.position, targetDetectionRadius, targetPool, followLayerMask);

        for (int i = 0; i < numColliders; i++)
        {
            var networkAvatar = targetPool[i].GetComponent<NetworkAvatar>();
            if (networkAvatar != null && networkAvatar.isControlled)
            {
                targetAvatar = networkAvatar;
                target = targetPool[i].gameObject;
                UpdatePath();
                return true;
            }
        }

        return false;
    }

    [Server]
    private void UpdatePath()
    {
        if (target == null) return;

        targetPosition = target.transform.position;
        navAgent.SetDestination(targetPosition);
        navAgent.updateRotation = true;
    }

    [Server]
    private void UpdatePath(Transform pathTarget)
    {
        if (pathTarget == null) return;

        targetPosition = pathTarget.transform.position;
        navAgent.SetDestination(targetPosition);
        navAgent.updateRotation = true;
    }

    [Server]
    private void RemoveTarget()
    {
        target = null;
        navAgent.ResetPath();
    }
}
