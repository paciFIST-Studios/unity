using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class OscillationController : MonoBehaviour
{
    private Vector3 startPosition;

    [SerializeField] private Vector3 offsetVector;
    [SerializeField] [Range(0, 1)] private float movementFactor;
    [SerializeField] private float oscillationPeriod; // time it takes to complete 1 cycle

    private float hertz;

    private void Start()
    {
        startPosition = this.transform.position;

        if (float.IsNaN(oscillationPeriod))
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
        // update movement factor in code
        float cycles = Time.time * hertz; // grows continually

        const float tau = Mathf.PI * 2; //  about 6.28
        float rawSin = Mathf.Sin(cycles * tau);

        movementFactor = (rawSin / 2f) + 0.5f;

        Vector3 offset = offsetVector * movementFactor;
        transform.position = startPosition + offset;
    }

}
