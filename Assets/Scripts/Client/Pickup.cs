using UnityEngine;
using UnityEngine.Events;

public class Pickup : MonoBehaviour
{
    [SerializeField] [Tooltip("Radius to start floating toward target")]
    private float floatRange = 5f;

    [SerializeField] [Tooltip("Radius to be picked up by target")]
    private float pickupRange = 1f;

    [SerializeField] [Tooltip("Amount of time for the orb to reach its target (lower number means faster movement.")]
    private float timeToTarget = 0.25f;
    
    [SerializeField] private LayerMask playerMask;

    [SerializeField] private GameObject pickupEffect;

    private Transform target;
    private Vector3 currentSpeed = Vector3.zero;

    [SerializeField] private string pickupMessage;
    [SerializeField] private float pickupValue;
    public UnityEvent OnPickup;

    private void Start()
    {
        if (OnPickup == null) OnPickup = new UnityEvent();

        OnPickup.AddListener(RestorePlayerAttribute);
        OnPickup.AddListener(SpawnPickupEffect);
        OnPickup.AddListener(DestroySelf);
    }

    void Update()
    {
        if (target)
        {
            FloatTowardTarget();

            if ((target.position - transform.position).sqrMagnitude <= pickupRange)
            {
                OnPickup?.Invoke();
            }
        }
        else
        {
            FindTarget();
        }
    }

    void FindTarget()
    {
        var nearbyPlayers = Physics.OverlapSphere(transform.position, floatRange, playerMask);
        if (nearbyPlayers.Length > 0)
        {
            target = nearbyPlayers[0].transform;
        }
    }

    void RestorePlayerAttribute()
    {
        target.gameObject.SendMessage(pickupMessage, pickupValue, SendMessageOptions.DontRequireReceiver);
    }

    void FloatTowardTarget()
    {
        transform.position = Vector3.SmoothDamp(transform.position, target.position, ref currentSpeed, timeToTarget);
    }

    void SpawnPickupEffect()
    {
        var effect = Instantiate(pickupEffect, target);
        Destroy(effect, 1f);
    }

    void DestroySelf()
    {
        Destroy(gameObject);
    }
}
