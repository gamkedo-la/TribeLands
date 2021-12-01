using System;
using Mirror;
using UnityEngine;

namespace Client
{
    public class Projectile : NetworkBehaviour
    {
        [SerializeField] private float destroyAfter = 5f;
        [SerializeField] private Rigidbody rb;
        [SerializeField] private float force = 1000f;

        private void Start()
        {
            Invoke(nameof(DestroySelf), destroyAfter);
            rb.AddForce(transform.forward * force);
        }

        [Server]
        private void DestroySelf()
        {
            NetworkServer.Destroy(gameObject);
        }

        private void OnCollisionEnter(Collision other)
        {
            other.gameObject.SendMessageUpwards("TakeDamage", SendMessageOptions.DontRequireReceiver);
            DestroySelf();
        }
    }
}