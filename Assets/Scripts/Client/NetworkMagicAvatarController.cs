using Mirror;
using UnityEngine;

namespace Client
{
    public class NetworkMagicAvatarController : NetworkAvatarController
    {
        [SerializeField] private GameObject projectile;
        [SerializeField] private Transform fireFrom;
        
        public override void Attack()
        {
            // Spawn magic projectile
            var projectileDirection = Quaternion.LookRotation(transform.forward, Vector3.up);
            var magicProjectile = Instantiate(projectile, fireFrom.position, projectileDirection);
            NetworkServer.Spawn(magicProjectile);
        }
    }
}
