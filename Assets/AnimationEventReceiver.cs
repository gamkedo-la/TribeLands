using Client;
using Mirror;
using UnityEngine;

public class AnimationEventReceiver : NetworkBehaviour
{
    [SerializeField] private NetworkAvatarController receiver;

    [Server]
    void Attack()
    {
        receiver.CmdAttack();
    }
    
    [Server]
    void PowerAttack()
    {
        receiver.CmdPowerAttack();
    }
}
