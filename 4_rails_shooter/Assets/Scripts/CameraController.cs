using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform folllowTarget;
    [SerializeField] Vector3 offset;


    void Update()
    {
        setNewPosition();
    }

    private void setNewPosition()
    {
        transform.position = folllowTarget.position + offset;
    }

}
