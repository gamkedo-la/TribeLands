using System;
using Mirror;
using UnityEngine;
using UnityEngine.AI;

namespace Client
{
    public abstract class NetworkAvatarController : NetworkBehaviour
    {
        private NavMeshAgent navAgent;
        
        private void Start()
        {
            navAgent = GetComponent<NavMeshAgent>();
        }

        public virtual void Move(Vector3 movementDirection)
        {
            if (NavMesh.SamplePosition(transform.position + movementDirection, out var navMeshHit, 100f, NavMesh.AllAreas))
            {
                navAgent.SetDestination(navMeshHit.position);
            }
        }

        public virtual void Dodge(Vector2 direction)
        {
            Debug.LogError("Dodge not implemented");
        }

        public virtual void Attack()
        {
            Debug.LogError("Attack not implemented");
        }

        public virtual void PowerAttack()
        {
            Debug.LogError("Power attack not implemented");
        }
    }
}
