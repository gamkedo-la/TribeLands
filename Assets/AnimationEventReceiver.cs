using Client;
using Mirror;
using UnityEngine;

public class AnimationEventReceiver : MonoBehaviour
{
    [SerializeField] private NetworkAvatarController receiver;

    void Attack()
    {
        receiver.Attack();
    }
    
    void PowerAttack()
    {
        receiver.CmdPowerAttack();
    }
}
