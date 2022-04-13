using System.Collections.Generic;
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
    [SerializeField] private NetworkAnimator networkAnimator;
    
    public AllyBaseBehavior followAI;

    public bool isControlled = false;

    public GameObject selectionIndicator;

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

    private List<NetworkAvatar> avatars;

    void Start()
    {
        health = 1;
        energy = maxEnergy;
        audioSource = GetComponent<AudioSource>();

        // Find other avatars in scene.
        avatars = new List<NetworkAvatar>();
        foreach (var avatar in FindObjectsOfType<NetworkAvatar>())
        {
            if (avatar != this) avatars.Add(avatar);
        }

        if (OnHealthChanged == null)
            OnHealthChanged = new UnityEvent<float, float>();
        if (OnEnergyChanged == null)
            OnEnergyChanged = new UnityEvent<float, float>();
        if (OnAttack == null)
            OnAttack = new UnityEvent();
        if (OnPowerAttack == null)
            OnPowerAttack = new UnityEvent();

        OnAttack.AddListener(PlayAttackAnimation);
        OnAttack.AddListener(PlayAttackSound);
        OnPowerAttack.AddListener(PlayPowerAttackSound);
        OnPowerAttack.AddListener(PlayPowerAttackAnimation);
    }

    private void TakeDamage(float damage)
    {
        health = Mathf.Max(health - damage, 0f);
        OnHealthChanged?.Invoke(health, maxHealth);
    }

    public void AddPlayerControl()
    {
        isControlled = true;
        followAI.enabled = false;
        navMeshAgent.ResetPath();
    }

    public void RemovePlayerControl(NetworkAvatar newFollowTarget)
    {
        isControlled = false;
        netIdentity.RemoveClientAuthority();
        followAI.enabled = true;
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

    [Command]
    private void PlayAttackAnimation()
    {
        networkAnimator.SetTrigger("Attack");
    }

    [Command]
    private void PlayPowerAttackAnimation()
    {
        networkAnimator.SetTrigger("Power Attack");
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
