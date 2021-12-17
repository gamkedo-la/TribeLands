using Cinemachine;
using Mirror;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class NetworkAvatar : NetworkBehaviour
{
    public CinemachineTargetGroup targetGroup;
    public CinemachineVirtualCamera virtualCamera;
    public NavMeshAgent navMeshAgent;
    public Animator animator;

    public bool isControlled = false;
    public Transform hostPlayer;
    
    public float navUpdateInterval = 0.3f;
    private float timeSinceLastNavUpdate = 0f;
    
    public GameObject selectionIndicator;
    public Collider followRadius;
    public LayerMask followMask;

    [SerializeField] private float maxHealth = 100f;
    private float health;
    
    // Broadcasts remaining health and max health after damage taken.
    public UnityEvent<float, float> OnDamaged;

    void Start()
    {
        health = maxHealth;
        
        if (OnDamaged == null)
            OnDamaged = new UnityEvent<float, float>();
    }

    private void Update()
    {
        if (isControlled) return;
        
        timeSinceLastNavUpdate += Time.deltaTime;

        if (timeSinceLastNavUpdate > navUpdateInterval)
        {
            timeSinceLastNavUpdate = 0f;
            FollowHost();
        }
    }

    private void FollowHost()
    {
        if (!hostPlayer) return;

        var ray = new Ray(transform.position, (hostPlayer.position - transform.position).normalized);
        
        if (Physics.Raycast(ray, out var hitInfo, 100f, followMask))
        {
            navMeshAgent.SetDestination(hitInfo.point);
        }
    }

    private void TakeDamage(float damage)
    {
        health = Mathf.Max(health - damage, 0f);
        OnDamaged?.Invoke(health, maxHealth);
    }
}
