using Cinemachine;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.Serialization;

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
    public float MaxHealth => maxHealth;

    [SerializeField] private float maxEnergy = 150f;
    private float energy;
    public float Energy => energy;
    public float MaxEnergy => maxEnergy;

    private AudioSource audioSource;
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip powerAttackSound;
    
    // Broadcasts remaining health and max health after damage taken.
    public UnityEvent<float, float> OnHealthChanged;
    public UnityEvent<float, float> OnEnergyChanged;
    public UnityEvent OnAttack;
    public UnityEvent OnPowerAttack;

    void Start()
    {
        health = 1;
        energy = maxEnergy;
        audioSource = GetComponent<AudioSource>();
        
        if (OnHealthChanged == null)
            OnHealthChanged = new UnityEvent<float, float>();
        if (OnEnergyChanged == null)
            OnEnergyChanged = new UnityEvent<float, float>();
        if (OnAttack == null)
            OnAttack = new UnityEvent();
        if (OnPowerAttack == null)
            OnPowerAttack = new UnityEvent();
        
        OnAttack.AddListener(PlayAttackSound);
        OnPowerAttack.AddListener(PlayPowerAttackSound);
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
        OnHealthChanged?.Invoke(health, maxHealth);
    }

    public void GainHealth(float amount)
    {
        health = Mathf.Min((health + amount), maxHealth);
        OnHealthChanged?.Invoke(health, maxHealth);
    }

    [Command]
    public void GainEnergy(float amount)
    {
        GainEnergyRpc(amount);
    }

    [ClientRpc]
    private void GainEnergyRpc(float amount)
    {
        energy = Mathf.Min((energy + amount), maxEnergy);
        OnEnergyChanged?.Invoke(energy, maxEnergy);
    }

    private void PlayAttackSound()
    {
        audioSource?.PlayOneShot(attackSound);
    }

    private void PlayPowerAttackSound()
    {
        audioSource?.PlayOneShot(powerAttackSound);
    }
    
}
