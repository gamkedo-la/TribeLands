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
    public float Health => health;

    [SerializeField] private float maxEnergy = 150f;
    private float energy;
    public float Energy => energy;
    
    // Broadcasts remaining health and max health after damage taken.
    public UnityEvent<float, float> OnDamaged;
    public UnityEvent<float, float> OnPowerAttack;

    void Start()
    {
        health = maxHealth;
        energy = maxEnergy;
        
        if (OnDamaged == null)
            OnDamaged = new UnityEvent<float, float>();
        if (OnPowerAttack == null)
            OnPowerAttack = new UnityEvent<float, float>();
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

    public void PowerAttackPerformed(float cost)
    {
        energy -= cost;
        OnPowerAttack?.Invoke(energy, maxEnergy);
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
