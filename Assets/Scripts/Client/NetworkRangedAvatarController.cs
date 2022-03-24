using Mirror;
using UnityEngine;

namespace Client
{
    public class NetworkRangedAvatarController : NetworkAvatarController
    {
        [SerializeField] private GameObject projectile;
        [SerializeField] private Transform fireFrom;
        
        private Vector3 projectileTarget;

        public override void BeginAttack()
        {
            attackDirection = Projectile.AttackDirection(cam, attackMask, fireFrom.position);
            // animation is triggered elsewhere..should it happen here instead?
        }
        
        [Command]
        public override void CmdAttack()
        {
            ServerAttack();
        }
        
        [Server]
        public override void ServerAttack()
        {
            var projectileInstance = Instantiate(projectile, fireFrom.position, attackDirection);
            NetworkServer.Spawn(projectileInstance);
        }
    }
}
