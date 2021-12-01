using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : NetworkBehaviour
{
    public float maxHealth = 100f;

    [SerializeField] private Image healthBar;
    [SerializeField] private Image healthBarBackground;
    private Transform healthBarCanvas;
    private Camera mainCamera;
    
    [SyncVar]
    public float health;
    
    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        healthBarCanvas = healthBar.transform.parent;
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        healthBarCanvas.rotation = mainCamera.transform.rotation * Quaternion.AngleAxis(180f, Vector3.up);
    }

    public void TakeDamage()
    {
        health -= 20f;

        if (health <= 0f) DestroySelf();
        
        var percentRemaining = health/maxHealth;
        healthBar.fillAmount = percentRemaining;
        healthBarBackground.fillAmount = 1 - percentRemaining;
    }

    [Server]
    private void DestroySelf()
    {
        NetworkServer.Destroy(gameObject);
    }
}
