using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class EnemyHealth : NetworkBehaviour
{
    public float maxHealth = 100f;

    [SerializeField] private Image healthBar;
    [SerializeField] private Image healthBarBackground;
    private Transform healthBarCanvas;
    private Camera mainCamera;
    [SyncVar] public float health;

    [SerializeField] private List<Pickup> pickups;

    public UnityEvent<int> OnDeath;

    private void Start()
    {
        health = maxHealth;
        healthBarCanvas = healthBar.transform.parent;
        mainCamera = Camera.main;
    }

    public override void OnStartServer()
    {
        if (OnDeath == null)
            OnDeath = new UnityEvent<int>();
        
        OnDeath.AddListener(DestroySelf);
        OnDeath.AddListener(SpawnPickup);
    }

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
    private void RpcTakeDamage()
    {
        var percentRemaining = health/maxHealth;
        healthBar.fillAmount = percentRemaining;
        healthBarBackground.fillAmount = 1 - percentRemaining;
    }

    [Server]
    private void SpawnPickup(int objId)
    {
        if (pickups == null || pickups.Count == 0) return;
        
        var selection = Random.Range(0, pickups.Count);
        Instantiate(pickups[selection], transform.position, Quaternion.identity);
    }

    [Server]
    private void DestroySelf(int objId)
    {
        NetworkServer.Destroy(gameObject);
    }
}
