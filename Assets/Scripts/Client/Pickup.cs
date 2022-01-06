using UnityEngine;

public class Pickup : MonoBehaviour
{
    [SerializeField] [Tooltip("Radius to start floating toward target")]
    private float floatRange = 5f;

    [SerializeField] [Tooltip("Radius to be picked up by target")]
    private float pickupRange = 1f;

    [SerializeField] [Tooltip("Amount of time for the orb to reach its target (lower number means faster movement.")]
    private float timeToTarget = 0.25f;
    
    [SerializeField] private LayerMask playerMask;

    private Transform target;
    private Vector3 currentSpeed = Vector3.zero;

    void Update()
    {
        if (target)
        {
            FloatTowardTarget();

            if ((target.position - transform.position).sqrMagnitude <= pickupRange)
            {
                OnPickup();
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

    void FloatTowardTarget()
    {
        transform.position = Vector3.SmoothDamp(transform.position, target.position, ref currentSpeed, timeToTarget);
    }

    void OnPickup()
    {
        Destroy(gameObject);
    }
}
