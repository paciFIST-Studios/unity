using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketController : MonoBehaviour
{
    private Rigidbody rb;

    float force = 0.2f;

    private void Start()
    {
        rb = this.GetComponent<Rigidbody>();        

    }

    void Update ()
    {
        ProcessInput();
	}
	
    private void ProcessInput()
    {
        if(Input.GetKey(KeyCode.Space))
        {
            // using rocket's coordinate system
            rb.AddRelativeForce(Vector3.up * force, ForceMode.Impulse);
        }

        // left rotate has precedence 
        if (Input.GetKey(KeyCode.A))
        {
            print("Rotate Left");
        }
        else if (Input.GetKey(KeyCode.D))
        {
            print("Rotate Right");
        }

        if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            force += 0.1f;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            force -= 0.1f;
        }

    }

}
