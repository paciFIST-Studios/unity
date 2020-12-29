using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfaceLanderController : MonoBehaviour
{
    [SerializeField] private Transform doorHinge;
    [SerializeField] [Range(0, 1)] private float doorActivation;

    private Vector3 doorRotationAxis;

    private void Start()
    {
        // take transform of door, and then provide an offset from its centerpoint, to a point on its rotation axis
        // 

    }


}
