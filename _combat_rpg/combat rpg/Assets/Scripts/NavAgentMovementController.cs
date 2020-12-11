using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavAgentMovementController : MonoBehaviour
{
    [SerializeField] Transform target;

    void Update()
    {
        this.GetComponent<NavMeshAgent>().destination = target.position;
    }
}
