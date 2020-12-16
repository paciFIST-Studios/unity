//adapted from example script available at
//https://docs.unity3d.com/ScriptReference/Input.GetAxis.html
using UnityEngine;
using System.Collections;

using UnityEngine.Networking;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;

	  [SerializeField] private float speed = 10.0F;
    [SerializeField] private float rotationSpeed = 100.0F;

    private void Start()
    {
        rb = this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void LateUpdate ()
    {
        float translation = Input.GetAxis("Vertical");
        float rotation = Input.GetAxis("Horizontal");

        // multiply by Time.deltaTime in order to make it framerate independant
        translation *= Time.deltaTime * speed;
        rotation    *= Time.deltaTime * rotationSpeed;

        // take our turn degrees in Y axis, and make it a Quaternion, buy supplying a turn vector
        Quaternion turn = Quaternion.Euler(0f, rotation, 0f);

        // apply using rigid body, so it ties into physics
        rb.MovePosition(rb.position + this.transform.forward * translation);
        rb.MoveRotation(rb.rotation * turn);
	}
}
