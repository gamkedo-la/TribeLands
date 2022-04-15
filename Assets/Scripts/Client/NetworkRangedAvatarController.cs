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
        }

        public override void Attack()
        {
            if (isServer)
            {
                ServerAttack(fireFrom.position, attackDirection);
            }
            else
            {
                CmdAttack(fireFrom.position, attackDirection);
            }
        } 
        
        [Command]
        public override void CmdAttack(Vector3 attackPosition, Quaternion direction)
        {
            // this is just a proxy so that clients can fire projectiles
            ServerAttack(attackPosition, direction);
        }
        
        [Server]
        public override void ServerAttack(Vector3 attackPosition, Quaternion direction)
        {
            var projectileInstance = Instantiate(projectile, attackPosition, direction);
            NetworkServer.Spawn(projectileInstance);
        }
    }
}
