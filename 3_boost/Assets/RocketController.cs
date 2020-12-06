using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketController : MonoBehaviour
{
    private Rigidbody rb;

    float boostForce = 500f;
    float rotationForce = 50f;

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
            rb.AddRelativeForce(Vector3.up * boostForce * Time.deltaTime);
        }

        // left rotate has precedence 
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward *  rotationForce * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward *  rotationForce * Time.deltaTime);
        }

        if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            boostForce += 50f;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            boostForce -= 50f;
        }

    }

}
