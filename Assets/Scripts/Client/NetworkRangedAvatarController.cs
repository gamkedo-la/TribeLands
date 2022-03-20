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
            // animation is triggered elsewhere
        }
        
        [Command]
        public override void CmdAttack()
        {
            // Spawn magic projectile
            // var projectileRay = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, cam.nearClipPlane));
            //
            // RaycastHit hit;
            // if (Physics.Raycast(projectileRay, out hit, 1000f, attackMask))
            // {
            //     projectileTarget = hit.point;
            // }
            // else
            // {
            //     projectileTarget = cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, cam.farClipPlane));
            // }
            //     
            // var projectileDirection = (projectileTarget - fireFrom.position).normalized;
            // var projectileRotation = Quaternion.LookRotation(projectileDirection);
            
            var projectileInstance = Instantiate(projectile, fireFrom.position, attackDirection);
            NetworkServer.Spawn(projectileInstance);
        }
    }
}
