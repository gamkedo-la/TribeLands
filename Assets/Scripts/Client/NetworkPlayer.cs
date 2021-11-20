using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Mirror;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class NetworkPlayer : NetworkBehaviour
{
    private Vector2 movementInput;
    private Animator animator;
    private NavMeshAgent navAgent;
    private Camera cam;

    [SyncVar] private float currentSpeed;

    [SerializeField]
    private GameObject rightHandSlot;

    [SerializeField]
    private CinemachineTargetGroup targetGroup;
    [SerializeField]
    private CinemachineVirtualCamera virtualCamera;
    private bool cameraChanged;

    [SerializeField] private Transform avatarSlot;
    private NetworkAvatar avatar;

    private NetworkAvatar[] avatars;
    
    private void Start()
    {
        // cam = GetComponent<PlayerInput>().camera ?? Camera.main;
        // if (!cam)
        cam = Camera.main;
        // animator = GetComponentInChildren<Animator>();
        // navAgent = GetComponent<NavMeshAgent>();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        
        // var avatarObjs = GameObject.FindGameObjectsWithTag("Avatar");
        // avatars = new NetworkAvatar[avatarObjs.Length];
        // for (var i = 0; i < avatarObjs.Length; i++)
        // {
        //     avatars[i] = avatarObjs[i].GetComponent<NetworkAvatar>();
        //     avatars[i].isControlled = false;
        // }
        //
        // if (avatars.Length > 0)
        // {
        //     BecomeHost(avatars[0]);
        // }
    }

    private void Awake()
    {
        if (!isLocalPlayer) return;
        
        // cam = Camera.main;
        // GetComponentInChildren<CinemachineTargetGroup>().enabled = true;
        // GetComponentInChildren<CinemachineVirtualCamera>().enabled = true;
    }

    private void Update()
    {
        if (!isLocalPlayer) return;
        
        if (movementInput != Vector2.zero)
            Move();

        if (navAgent)
        {
            SetMoveSpeed(navAgent.velocity.magnitude);
        }
    }
    
    private void LateUpdate()
    {
        if (animator != null)
            animator.SetFloat("Speed", currentSpeed);
    }

    private void Move()
    {
        if (avatar == null) return;
        
        var cameraTransform = cam.transform;
        var right = cameraTransform.right;
        var forward = cameraTransform.forward;
        var hMovement = new Vector3(right.x, 0f, right.z) * movementInput.x;
        var vMovement = new Vector3(forward.x, 0f, forward.z) * movementInput.y;
        var moveDir = (hMovement + vMovement).normalized;

        if (NavMesh.SamplePosition(avatar.transform.position + moveDir, out var navMeshHit, 100f, NavMesh.AllAreas))
        {
            navAgent.SetDestination(navMeshHit.position);
        }
    }

    [Command]
    private void SetMoveSpeed(float speed)
    {
        currentSpeed = speed;
    }

    public void OnMovePerformed(InputAction.CallbackContext ctx)
    {
        movementInput = ctx.ReadValue<Vector2>();
    }

    public void OnAttackPerformed(InputAction.CallbackContext ctx)
    {
        // Only do this on press, not release.
        if (ctx.started)
        {
            CmdAttack();
        }
    }

    private void OnSwapAvatar(InputAction.CallbackContext ctx, int avatarSlot)
    {
        if (ctx.performed && avatarSlot >= 0 && avatarSlot < avatars.Length)
        {
            var a = avatars[avatarSlot];
            if (a.gameObject != avatar.gameObject)
            {
                // BecomeHost(a);
            }
        }
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
        Debug.Log($"{name} becoming host to {a.gameObject.name}");
        if (avatar)
        {
            DeactivateCurrentAvatar();
        }

        // Tell other avatars to follow the new target avatar
        // UpdateAvatarAI(a);
        
        // Assign ourselves authority over this avatar (?!)
        // avatar.netIdentity.AssignClientAuthority(netIdentity.connectionToServer);

        // Tell avatar that it's being controlled externally
        a.isControlled = true;
        
        // Save reference
        avatar = a;
        
        // Change navmeshagent reference
        navAgent = avatar.navMeshAgent;
        
        // Activate virtual camera and target group
        avatar.targetGroup.gameObject.SetActive(true);
        avatar.virtualCamera.gameObject.SetActive(true);
        
        // Activate selection indicator
        avatar.selectionIndicator.SetActive(true);
        
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
        avatar.targetGroup.gameObject.SetActive(false);
        avatar.virtualCamera.gameObject.SetActive(false);
        avatar.selectionIndicator.SetActive(false);
    }
}
