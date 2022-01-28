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
    [SerializeField] private float mouseLookSpeed = 0.2f;
    [SerializeField] private float minVerticalAngle = 40f;
    [SerializeField] private float maxVerticalAngle = 340f;
    [SerializeField] private bool invertGamePadVertical = true;
    [SerializeField] private bool invertMouseVertical = false;

    [SerializeField]
    private GameObject rightHandSlot;

    [SerializeField]
    private CinemachineTargetGroup targetGroup;
    [SerializeField]
    private CinemachineVirtualCamera virtualCamera;
    private bool cameraChanged;
    private Quaternion nextRotation;

    private NetworkAvatar avatar;
    private NetworkAvatarController avatarController;
    
    private Animator animator;

    private NetworkAvatar[] avatars;

    // Attack speed is tracked on the avatars.
    private bool isAttacking = false;
    private float timeSinceAttack = 100;
    
    private void Start()
    {
        cam = Camera.main;
        
        // Find all NetworkAvatars in the scene.
        avatars = FindObjectsOfType<NetworkAvatar>();

        nextRotation = transform.rotation;
    }

    private void Update()
    {
        if (!isLocalPlayer) return;

        timeSinceAttack += Time.deltaTime;

        if (isAttacking) Attack();
        Look();
        Move();
    }

    private void Attack()
    {
        if (!isLocalPlayer || timeSinceAttack < avatarController.TimeBetweenAttacks) return;

        timeSinceAttack = 0f;
        avatarController.CmdAttack(nextRotation);
    }

    private void Look()
    {
        if (avatar == null || targetGroup == null) return;
        
        // Rotation around axes individually based on input
        var xRotation = lookInput.x;
        var yRotation = lookInput.y;
        var targetRotation = targetGroup.transform.rotation;
        targetRotation *= Quaternion.AngleAxis(xRotation, Vector3.up);
        targetRotation *= Quaternion.AngleAxis(yRotation, Vector3.right);

        // Clamp vertical angle, ignore z
        var angles = targetRotation.eulerAngles;
        angles.z = 0;
        var angle = angles.x;
        if (angle > 180 && angle < maxVerticalAngle)
        {
            angles.x = maxVerticalAngle;
        }
        else if (angle < 180 && angle > minVerticalAngle)
        {
            angles.x = minVerticalAngle;
        }

        // smoov
        targetRotation.eulerAngles = angles;
        nextRotation = Quaternion.Lerp(targetGroup.transform.rotation, targetRotation, lookInput.sqrMagnitude);
        targetGroup.transform.rotation = nextRotation;
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

        // Rotate the avatar only if we're moving. That way players can rotate the camera around
        // to see the front of the avatar.
        if (movementInput.x != 0 || movementInput.y != 0)
        {
            var targetRotation = Quaternion.Euler(0, nextRotation.eulerAngles.y, 0);
            avatarController.transform.localRotation =
                Quaternion.Lerp(avatarController.transform.localRotation, targetRotation, 0.1f);
        }
    }

    public void OnControlsChanged(PlayerInput input)
    {
        Debug.Log(input.currentControlScheme);
    }

    public void OnLookPerformed(InputAction.CallbackContext ctx)
    {
        lookInput = ctx.ReadValue<Vector2>() * lookSpeed;
        if (invertGamePadVertical)
            lookInput.y = -lookInput.y;
    }

    public void OnMouseLookPerformed(InputAction.CallbackContext ctx)
    {
        lookInput = ctx.ReadValue<Vector2>() * mouseLookSpeed;
        if (invertMouseVertical)
            lookInput.y = -lookInput.y;
    }

    public void OnMovePerformed(InputAction.CallbackContext ctx)
    {
        movementInput = ctx.ReadValue<Vector2>();
    }

    public void OnAttackPerformed(InputAction.CallbackContext ctx)
    {
        if (!isLocalPlayer) return;

        if (ctx.started) isAttacking = true;
        else if (ctx.canceled) isAttacking = false;
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
        var dodgeDir = avatarController.transform.forward;
        avatarController.Dodge(dodgeDir);
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
