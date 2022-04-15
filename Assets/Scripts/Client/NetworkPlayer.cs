using System.Collections.Generic;
using Cinemachine;
using Client;
using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
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
    private GameManager gameManager;
    private PlayerInput playerInput;
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
    // ------- UI --------
    private GraphicRaycaster uiRaycaster;
    PointerEventData clickData;
    List<RaycastResult> clickResults;

    private void Awake(){
        gameManager = FindObjectOfType<GameManager>();
        clickData = new PointerEventData(EventSystem.current);
        clickResults = new List<RaycastResult>();
        playerInput = GetComponent<PlayerInput>();
    }
    private void Start()
    {
        cam = Camera.main;
        // Find all NetworkAvatars in the scene.
        avatars = FindObjectsOfType<NetworkAvatar>();
        nextRotation = transform.rotation;
        Cursor.lockState = CursorLockMode.Locked;
    }    
    private void Update(){
        if (!isLocalPlayer) return;
        timeSinceAttack += Time.deltaTime;
        if (isAttacking) Attack();
        Look();
        Move();
    }
    // private void OnEnable(){
    //     //Actions that will change the action map
    //     playerInput.actions["Pause"].performed += Pause;
    //     playerInput.actions["Cancel"].performed += Resume;
    // }
    private void Attack()
    {
        if (!isLocalPlayer || timeSinceAttack < avatarController.TimeBetweenAttacks) return;
        timeSinceAttack = 0f;
        avatarController.BeginAttack();
        avatar.OnAttack?.Invoke();
    }
    private void Look()
    {
        // Mouse sensitivity - Storing saved profile 
        // set a slider 0 - 3 [SerializeField] [range(0, 3)] double mouseSensitivity = 1;   
        if (avatar == null || targetGroup == null) return;
        
        // Rotation around axes individually based on input
        var xRotation = lookInput.x;
        var yRotation = lookInput.y;
        var targetRotation = targetGroup.transform.rotation; // * sensitivity
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

    public void OnPowerAttackPerformed(InputAction.CallbackContext ctx){
        if (!isLocalPlayer || !ctx.started) return;
        
        if (avatar.Energy >= avatarController.PowerAttackEnergyCost)
        {
            avatar.GainEnergy(-avatarController.PowerAttackEnergyCost);
            avatar.OnPowerAttack?.Invoke();
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
    public void OnSwapAvatar(InputAction.CallbackContext ctx, int avatarSlot)
    {
        if (ctx.performed)
        {
            AskForAvatar(avatarSlot);
        }
    }
    public void Pause(InputAction.CallbackContext ctx){
        if(ctx.performed){
            Debug.Log("Game Paused");
            gameManager.OpenPauseMenu();
            Cursor.lockState = CursorLockMode.None;
            playerInput.SwitchCurrentActionMap("UI");
        }
        //playerInput.actions.FindActionMap("UI").Enable();
        //playerInput.actions.FindActionMap("Player").Disable();
    }

    public void ToggleMute(InputAction.CallbackContext ctx)
    {
        if(ctx.performed)
        {
            gameManager.ToggleMute();
        }
    }

    //-----------------------------UI--------------------------
    public void UIClick(){
        clickData.position = Mouse.current.position.ReadValue();
        clickResults.Clear();
        foreach (RaycastResult result in clickResults){
            GameObject uiElement = result.gameObject;
        }
    }
    public void Resume(){
        Debug.Log("Resume");
        gameManager.ClosePauseMenu();
        Cursor.lockState = CursorLockMode.Locked;
        playerInput.SwitchCurrentActionMap("Player");        
    }
    [Command]
    void AskForAvatar(int slot)
    {
        GroupNetworkManager.gnmSingleton.AskForAvatar(connectionToClient, slot);
    }

    public void OnSelectFirstAvatar(InputAction.CallbackContext ctx) => OnSwapAvatar(ctx, 0);
    public void OnSelectSecondAvatar(InputAction.CallbackContext ctx) => OnSwapAvatar(ctx, 1);
    public void OnSelectThirdAvatar(InputAction.CallbackContext ctx) => OnSwapAvatar(ctx, 2);

    public void OnToggleMute(InputAction.CallbackContext ctx) => ToggleMute(ctx);

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
            // avatarController.CmdAttack();
        }
    }

    [ClientRpc]
    private void BecomeHost(NetworkAvatar a)
    {
        if (!isLocalPlayer) return;
        
        // Tell other avatars to follow the new target avatar
        if (avatar)
        {
            avatar.RemovePlayerControl(a);
        }
        
        // Tell avatar that it's being controlled externally.
        a.AddPlayerControl();
        
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
        
        // Change animator reference
        animator = avatar.animator;
        
        Debug.Log($"hosting avatar {a.gameObject.name}");
    }
}
