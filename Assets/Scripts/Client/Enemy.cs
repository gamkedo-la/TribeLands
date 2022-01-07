using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Enemy : NetworkBehaviour
{
    public float maxHealth = 100f;

    [SerializeField] private Image healthBar;
    [SerializeField] private Image healthBarBackground;
    private Transform healthBarCanvas;
    private Camera mainCamera;
    
    [SyncVar]
    public float health;

    public GameObject target;
    public NavMeshAgent navAgent;

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

    [SerializeField] private List<Pickup> pickups;

    public UnityEvent<int> OnDeath;
    
    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        healthBarCanvas = healthBar.transform.parent;
        mainCamera = Camera.main;
        navAgent = GetComponentInChildren<NavMeshAgent>();
    }

    public override void OnStartServer()
    {
        nearbyAvatars = new Collider[3];
        
        if (OnDeath == null)
            OnDeath = new UnityEvent<int>();
        
        OnDeath.AddListener(DestroySelf);
        OnDeath.AddListener(SpawnPickup);
    }

    private void Update()
    {
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
                
                Debug.Log($"{gameObject.name} navAgent path status: {navAgent.path.status.ToString()}");
            }
            else
            {
                Debug.LogError($"navAgent has target {target.gameObject.name}, but no valid path was found");
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

    // Update is called once per frame
    void LateUpdate()
    {
        healthBarCanvas.rotation = mainCamera.transform.rotation * Quaternion.AngleAxis(180f, Vector3.up);
    }

    [Server]
    public void TakeDamage()
    {
        health -= 20f;

        if (health <= 0f)
        {
            OnDeath?.Invoke(gameObject.GetInstanceID());
        }
        else
        {
            RpcTakeDamage();
        }
    }

    [ClientRpc]
    private void AttackTarget()
    {
        if (target == null) return;
        
        target.SendMessage("TakeDamage", attackDamage);
        timeSinceLastAttack = 0f;
    }

    [ClientRpc]
    private void RpcTakeDamage()
    {
        var percentRemaining = health/maxHealth;
        healthBar.fillAmount = percentRemaining;
        healthBarBackground.fillAmount = 1 - percentRemaining;
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
    private void DestroySelf(int objId)
    {
        NetworkServer.Destroy(gameObject);
    }

    [Server]
    private void SpawnPickup(int objId)
    {
        if (pickups == null || pickups.Count == 0) return;
        
        var selection = Random.Range(0, pickups.Count);
        Instantiate(pickups[selection], transform.position, Quaternion.identity);
    }

    [Server]
    private void RemoveTarget()
    {
        target = null;
        navAgent.ResetPath();
    }
}
