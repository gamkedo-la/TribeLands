using Client;
using UnityEngine;

public class AnimationEventReceiver : MonoBehaviour
{
    [SerializeField] private NetworkAvatarController receiver;

    void Attack()
    {
        receiver.CmdAttack();
    }
    
    void PowerAttack()
    {
        receiver.CmdPowerAttack();
    }
}
