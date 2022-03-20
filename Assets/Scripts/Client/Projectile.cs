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
            if (!isServer) return;
                
            other.gameObject.SendMessageUpwards("TakeDamage", SendMessageOptions.DontRequireReceiver);
            DestroySelf();
        }

        public static Quaternion AttackDirection(Camera cam, LayerMask attackMask, Vector3 fireFrom)
        {

            Vector3 projectileTarget;
            var projectileRay = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, cam.nearClipPlane));
            
            RaycastHit hit;
            if (Physics.Raycast(projectileRay, out hit, 1000f, attackMask))
            {
                projectileTarget = hit.point;
            }
            else
            {
                projectileTarget = cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, cam.farClipPlane));
            }
                
            var projectileDirection = (projectileTarget - fireFrom).normalized;
            return Quaternion.LookRotation(projectileDirection);
        }
    }
}