using System;
using Mirror;
using UnityEngine;

namespace Client
{
    public class NetworkMeleeAvatarController : NetworkAvatarController
    {
        [SerializeField] private GameObject powerAttackEffect;
        [SerializeField] private Transform fireFrom;
        [SerializeField] private Vector3 meleeHurtbox;
        
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
            foreach (var enemy in Physics.OverlapBox(attackPosition, meleeHurtbox, Quaternion.identity, attackMask))
            {
                enemy.gameObject.SendMessageUpwards("TakeDamage", attackDamage, SendMessageOptions.DontRequireReceiver);
            }
        }

        [Command]
        public override void CmdPowerAttack()
        {
            Debug.Log("melee power attack");
            if (powerAttackEffect == null) return;
            
            var effectRotation = Quaternion.identity;
            var effect = Instantiate(powerAttackEffect, transform.position, effectRotation);
            NetworkServer.Spawn(effect);
            Destroy(effect, 1f);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(fireFrom.position, meleeHurtbox*2);
        }
    }
}