using Mirror;
using UnityEngine;

namespace Client
{
    public class NetworkMeleeAvatarController : NetworkAvatarController
    {
        [SerializeField] private GameObject powerAttackEffect;
        
        public override void BeginAttack()
        {
            // nothing to do here
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
    }
}