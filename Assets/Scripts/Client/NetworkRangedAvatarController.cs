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
            var rangedProjectile = Instantiate(projectile, fireFrom.position, direction);
            NetworkServer.Spawn(rangedProjectile);
        }
    }
}
