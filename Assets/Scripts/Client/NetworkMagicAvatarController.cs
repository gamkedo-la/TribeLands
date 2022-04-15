using System;
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
