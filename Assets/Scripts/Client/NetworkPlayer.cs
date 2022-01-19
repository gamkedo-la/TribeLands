using Cinemachine;
using Client;
using Mirror;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class NetworkPlayer : NetworkBehaviour
{
    private Vector2 movementInput;
    private Vector2 lookInput;
    private NavMeshAgent navAgent;
    private Camera cam;

    [SerializeField] private float lookSpeed = 5f;

    [SerializeField]
    private GameObject rightHandSlot;

    [SerializeField]
    private CinemachineTargetGroup targetGroup;
    [SerializeField]
    private CinemachineVirtualCamera virtualCamera;
    private bool cameraChanged;

    private NetworkAvatar avatar;
    private NetworkAvatarController avatarController;
    
    private Animator animator;

    private NetworkAvatar[] avatars;

    // Attack speed is tracked on the avatars.
    private float timeSinceAttack;
    
    private void Start()
    {
        cam = Camera.main;
        
        // Find all NetworkAvatars in the scene.
        avatars = FindObjectsOfType<NetworkAvatar>();
    }

    private void Update()
    {
        if (!isLocalPlayer) return;

        timeSinceAttack += Time.deltaTime;
        
        Look();
        Move();
    }

    private void Look()
    {
        if (avatar == null || targetGroup == null) return;

        var xRotation = lookInput.x;
        // avatar.targetGroup.transform.Rotate(Vector3.up, xRotation);
        targetGroup.transform.Rotate(Vector3.up, xRotation);
    }

    private void Move()
    {
        if (avatarController == null || targetGroup == null) return;

        var cameraTransform = targetGroup.transform;
        var right = cameraTransform.right;
        var forward = cameraTransform.forward;
        var hMovement = new Vector3(right.x, 0f, right.z) * movementInput.x;
        var vMovement = new Vector3(forward.x, 0f, forward.z) * movementInput.y;
        var moveDir = hMovement + vMovement;

        avatarController.Move(moveDir);
    }

    public void OnControlsChanged(PlayerInput input)
    {
        Debug.Log(input.currentControlScheme);
    }

    public void OnLookPerformed(InputAction.CallbackContext ctx)
    {
        lookInput = ctx.ReadValue<Vector2>() * lookSpeed;
    }

    public void OnMouseLookPerformed(InputAction.CallbackContext ctx)
    {
        lookInput = ctx.ReadValue<Vector2>();
    }

    public void OnMovePerformed(InputAction.CallbackContext ctx)
    {
        movementInput = ctx.ReadValue<Vector2>();
    }

    public void OnAttackPerformed(InputAction.CallbackContext ctx)
    {
        if (!isLocalPlayer ||
            !ctx.started ||
            timeSinceAttack < avatarController.TimeBetweenAttacks) return;

        timeSinceAttack = 0f;
        avatarController.CmdAttack();
    }

    public void OnPowerAttackPerformed(InputAction.CallbackContext ctx)
    {
        if (!isLocalPlayer || !ctx.started) return;
        
        if (avatar.Energy >= avatarController.PowerAttackEnergyCost)
        {
            avatar.GainEnergy(-avatarController.PowerAttackEnergyCost);
            avatarController.CmdPowerAttack();
        }
        else
        {
            Debug.LogWarning($"not enough energy: {avatar.Energy}");
        }
    }

    public void OnDodgePerformed(InputAction.CallbackContext ctx)
    {
        if (!isLocalPlayer || !ctx.started) return;
        
        // todo: stamina check (see power attack energy check for implementation)
        avatarController.Dodge();
    }

    private void OnSwapAvatar(InputAction.CallbackContext ctx, int avatarSlot)
    {
        if (ctx.performed)
        {
            AskForAvatar(avatarSlot);
        }
    }

    [Command]
    void AskForAvatar(int slot)
    {
        GroupNetworkManager.gnmSingleton.AskForAvatar(connectionToClient, slot);
    }

    public void OnSelectFirstAvatar(InputAction.CallbackContext ctx) => OnSwapAvatar(ctx, 0);
    public void OnSelectSecondAvatar(InputAction.CallbackContext ctx) => OnSwapAvatar(ctx, 1);
    public void OnSelectThirdAvatar(InputAction.CallbackContext ctx) => OnSwapAvatar(ctx, 2);


    [Command]
    private void CmdAttack()
    {
        // TODO: do server-specific attacky things here, if necessary
        RpcAttack();
    }

    [ClientRpc]
    private void RpcAttack()
    {
        if (animator)
        {
            animator.SetTrigger("Melee Attack");
        }
    }

    [ClientRpc]
    private void BecomeHost(NetworkAvatar a)
    {
        if (!isLocalPlayer) return;
        
        Debug.Log($"{name} becoming host to {a.gameObject.name}");
        if (avatar)
        {
            DeactivateCurrentAvatar();
        }

        // Tell other avatars to follow the new target avatar
        // UpdateAvatarAI(a);

        // Tell avatar that it's being controlled externally
        a.isControlled = true;
        
        // Save reference
        avatar = a;
        avatarController = a.GetComponent<NetworkAvatarController>();
        
        // Change navmeshagent reference
        navAgent = avatar.navMeshAgent;
        
        targetGroup = FindObjectOfType<CinemachineTargetGroup>();
        if (targetGroup != null)
        {
            var target = new CinemachineTargetGroup.Target();
            target.target = avatar.animator.transform;
            target.weight = 1;
            target.radius = 0;

            targetGroup.m_Targets = new[] { target };
        }
        
        // Activate virtual camera and target group
        // avatar.targetGroup.gameObject.SetActive(true);
        // avatar.virtualCamera.gameObject.SetActive(true);
        
        // Activate selection indicator
        // commented out while experimenting with 3rd person camera (rather than top-down camera)
        // avatar.selectionIndicator.SetActive(true);
        
        // Change animator reference
        animator = avatar.animator;
        
        Debug.Log($"hosting avatar {a.gameObject.name}");
    }
    
    private void UpdateAvatarAI(NetworkAvatar avatar)
    {
        foreach (var networkAvatar in avatars)
        {
            networkAvatar.isControlled = false;
            networkAvatar.hostPlayer = avatar.transform;
        }
    }

    private void DeactivateCurrentAvatar()
    {
        Debug.Log($"Deactivate current avatar for {name}");
        
        avatar.targetGroup.gameObject.SetActive(false);
        avatar.virtualCamera.gameObject.SetActive(false);
        avatar.selectionIndicator.SetActive(false);
        avatar.hostPlayer = avatar.transform;
        avatar.isControlled = false;
        avatar.netIdentity.RemoveClientAuthority();
    }
}
