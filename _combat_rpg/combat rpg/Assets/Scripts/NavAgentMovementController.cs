using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEditor.IMGUI;

public class NavAgentMovementController : MonoBehaviour
{
    [Header("Navigation")]
    [SerializeField] GameObject navTargetPrefab;
    [SerializeField] Transform mainCamera;

    private GameObject navTarget;
    private NavMeshAgent navAgent;


    [Header("Debug Ray")]
    [SerializeField] bool drawRay = false;
    [SerializeField] float rayLength = 100f;

    Ray previousRay;

    private void Start()
    {
        navTarget = Instantiate(navTargetPrefab);
        navTarget.GetComponent<MeshRenderer>().enabled = false;

        navAgent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (drawRay)
        {
            Debug.DrawRay(previousRay.origin, previousRay.direction * rayLength, Color.cyan);
        }

        if(Vector3.Distance(transform.position, navAgent.destination) <= navAgent.stoppingDistance + 1)
        {
            navTarget.GetComponent<MeshRenderer>().enabled = false;
        }
    }


    private void OnGUI()
    {
        GUI.Box(new Rect(10, 10, 200, 60)
            ,
                  "dest=" + navAgent.destination.ToString()
                + "\npos=" + transform.position.ToString()
                + "\ndist=" + Vector3.Distance(transform.position, navAgent.destination).ToString()
            );
    }

    public void OnMoveTo(InputAction.CallbackContext ctx)
    {
        // only handle button down, not button up
        if(ctx.canceled) { return; }

        MoveToCursorPosition();
    }


    private void MoveToCursorPosition()
    {
        print("move cursor to position");

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo))
        {
            GetComponent<NavMeshAgent>().destination = hitInfo.point;
            previousRay = ray;

            navTarget.transform.position = hitInfo.point;
            navTarget.GetComponent<MeshRenderer>().enabled = true;
        }
    }
}
