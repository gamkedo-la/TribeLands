using Mirror;
using UnityEngine;

namespace Client
{
    public class NetworkRangedAvatarController : NetworkAvatarController
    {
        [SerializeField] private GameObject projectile;
        [SerializeField] private Transform fireFrom;
        
        [Command]
        public override void CmdAttack(Quaternion direction)
        {
            // Spawn projectile
            var projectileDirection = Quaternion.LookRotation(transform.forward, Vector3.up);
            // var rangedProjectile = Instantiate(projectile, fireFrom.position, projectileDirection);
            var rangedProjectile = Instantiate(projectile, fireFrom.position, direction);
            NetworkServer.Spawn(rangedProjectile);
        }
    }
}
