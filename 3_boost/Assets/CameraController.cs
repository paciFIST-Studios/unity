using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    Transform followTarget;

    [SerializeField]
    Vector3 offset;

	void Start ()
    {
		
	}
	
	void Update ()
    {
        this.transform.position = followTarget.position + offset;
	}
}
