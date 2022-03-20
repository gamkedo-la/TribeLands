using Mirror;
using Unity.VisualScripting;
using UnityEngine;

namespace Client
{
    public class NetworkMagicAvatarController : NetworkAvatarController
    {
        [SerializeField] private GameObject projectile;
        [SerializeField] private Transform fireFrom;
        [SerializeField] private float healSpellStrength = 50f;
        [SerializeField] private GameObject healEffect;

        private Vector3 projectileTarget;

        public override void BeginAttack()
        {
            attackDirection = Projectile.AttackDirection(cam, attackMask, fireFrom.position);
        }

        [Command]
        public override void CmdAttack()
        {
            // // Spawn magic projectile
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
            
            var magicProjectile = Instantiate(projectile, fireFrom.position, attackDirection);
            NetworkServer.Spawn(magicProjectile);
        }

        [Command]
        public override void CmdPowerAttack()
        {
            
            if (avatars != null)
            {
                foreach (var avatar in avatars)
                {
                    HealTarget(avatar);
                }
            }
            else
            {
                Debug.LogWarning("avatars is null, unable to perform Tua power attack");
            }
        }

        [ClientRpc]
        private void HealTarget(NetworkAvatar target)
        {
            target.GainHealth(healSpellStrength);
            var effect = Instantiate(healEffect, target.transform);
            NetworkServer.Spawn(effect);
            Destroy(effect, 1f);
        }
    }
}
