using Mirror;
using UnityEngine;

namespace Client
{
    public class NetworkMagicAvatarController : NetworkAvatarController
    {
        [SerializeField] private GameObject projectile;
        [SerializeField] private Transform fireFrom;
        [SerializeField] private float healSpellStrength = 50f;
        [SerializeField] private GameObject healEffect;

        [Command]
        public override void CmdAttack()
        {
            // Spawn magic projectile
            var projectileDirection = Quaternion.LookRotation(transform.forward, Vector3.up);
            var magicProjectile = Instantiate(projectile, fireFrom.position, projectileDirection);
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
