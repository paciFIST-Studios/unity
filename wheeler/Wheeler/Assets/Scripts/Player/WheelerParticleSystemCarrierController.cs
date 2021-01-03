using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelerParticleSystemCarrierController : MonoBehaviour
{
    private Vector3 offset;
    private Transform target;

    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    public void SetOffset(Vector3 offset)
    {
        this.offset = offset;
    }

    private void Update()
    {
        transform.position = target.position + offset;
    }
}
