using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OscillationController : MonoBehaviour
{
    private float hertz;
    private Vector3 startPosition;

    [Header("Movement")]
    [SerializeField] private Vector3 offsetVector;
    [SerializeField] [Range(0, 1)] private float movementFactor;
    [SerializeField] private float oscillationPeriod = 1f;

    private void Start()
    {
        startPosition = transform.position;

        if(float.IsNaN(oscillationPeriod))
        {
            hertz = 1;
        }
        else
        {
            hertz = 1 / oscillationPeriod;
        }
    }

    private void Update()
    {
        float cycles = Time.time * hertz;

        const float tau = Mathf.PI * 2;
        float rawSin = Mathf.Sin(cycles * tau);

        movementFactor = (rawSin * 0.5f) + 0.5f;

        Vector3 offset = offsetVector * movementFactor;
        transform.position = startPosition + offset * rawSin;
    }

}
