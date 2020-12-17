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
    [SerializeField] private bool showOffsetEndpoint;
    [SerializeField] private GameObject offsetEndpointMarkerPrefab;
    private GameObject offsetEndpointMarker;

    private void Start()
    {
        startPosition = transform.position;

        offsetEndpointMarker = Instantiate(offsetEndpointMarkerPrefab);
        offsetEndpointMarker.GetComponent<MeshRenderer>().enabled = false;


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
        if(offsetEndpointMarker)
        {
            offsetEndpointMarker.GetComponent<MeshRenderer>().enabled = showOffsetEndpoint;
            offsetEndpointMarker.transform.position = startPosition + offsetVector;
        }

        float cycles = Time.time * hertz;

        const float tau = Mathf.PI * 2;
        float rawSin = Mathf.Sin(cycles * tau);

        movementFactor = (rawSin * 0.5f) + 0.5f;

        Vector3 offset = offsetVector * movementFactor;
        transform.position = startPosition + offset * rawSin;
    }

}
