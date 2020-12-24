using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


[System.Serializable]
public class PIDController
{
    // patron saint
    // http://luminaryapps.com/blog/use-a-pid-loop-to-control-unity-game-objects/

    [Tooltip("Proportional constant (counters error)")]
    public float Kp = 0.2f;

    [Tooltip("Integral constant (counters accumulated error)")]
    public float Ki = 0.05f;

    [Tooltip("Derivative constant (fights oscillation)")]
    public float Kd = 1f;

    [Tooltip("current control value")]
    public float value = 0f;

    private float lastError;
    private float integral;


    public float Update(float error)
    {
        return Update(error, Time.deltaTime);
    }

    public float Update(float error, float dt)
    {
        float derivative = (error - lastError) / dt;
        integral += error * dt;
        lastError = error;

        value = Kp * error + Ki * integral + Kd * derivative;
        return value;
    }
}


public class WheelerPlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] PIDController pid;

    [SerializeField] float hoverForce = 10f;
    [SerializeField] float targetAltitude = 1f;

    [SerializeField] float groundMovementForce = 100f;

    [Header("Firing")]
    [SerializeField] int objectPoolSize = 10;
    [SerializeField] GameObject shotPrefab;
    [SerializeField] List<GameObject> shots;


    private Rigidbody rb;

    private bool isMoving = false;
    private bool isRotating = false;

    private Vector2 movementInputThisTick;
    private Vector2 rotateInputThisTick;

    private float zAxisRotation = 0f;

    private float screenHalfWidth = Screen.width * 0.5f;
    private float screenHalfHeight = Screen.height * 0.5f;


    // ---------------------------------------------------------------

    void Start()
    {
        rb = this.GetComponent<Rigidbody>();

        shots = new List<GameObject>(objectPoolSize);

        for(int i = 0; i < objectPoolSize; i++)
        {
            shots[i] = Instantiate(shotPrefab, transform);
            shots[i].GetComponent<MeshRenderer>().enabled = false;
            shots[i].GetComponent<CapsuleCollider>().enabled = false;
        }
    }

    void FixedUpdate()
    {
        float currentAltitude = transform.position.y;

        float error = targetAltitude - currentAltitude;
        var correction = Vector3.up;
        correction *= pid.Update(error);
        correction *= hoverForce;
        //correction *= Time.deltaTime;
        
        rb.AddForce(correction);

        if(isMoving)
        {
            MovePlayerCharacter(movementInputThisTick);
        }

        if(isRotating)
        {
            RotatePlayerCharacter(rotateInputThisTick);
        }

    }

    // Input System Callbacks ----------------------------------------

    public void OnLook(InputAction.CallbackContext ctx)
    {
        if(ctx.canceled)
        {
            isRotating = false;
            return;
        }

        isRotating = true;
        rotateInputThisTick = ctx.ReadValue<Vector2>();
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        if(ctx.canceled)
        {
            isMoving = false;
            return;
        }

        isMoving = true;
        movementInputThisTick = ctx.ReadValue<Vector2>();
    }

    public void OnFire(InputAction.CallbackContext ctx)
    {

    }

    // ---------------------------------------------------------------

    void MovePlayerCharacter(Vector2 input)
    {
        var movement = Vector3.right;
        movement.x *= input.x * groundMovementForce;
        rb.AddForce(movement);
    }

    void AboutFacePlayerCharacter()
    {

    }

    void RotatePlayerCharacter(Vector2 input)
    {
        var mouseControl = Mouse.current.position;

        Vector2 screenCoordinate;
        screenCoordinate.x = mouseControl.x.ReadValue();
        screenCoordinate.y = mouseControl.y.ReadValue();

        // recenter to middle of screen
        screenCoordinate.x -= screenHalfWidth;
        screenCoordinate.y -= screenHalfHeight;
        
        float angle = Mathf.Atan2(screenCoordinate.y, screenCoordinate.x) * Mathf.Rad2Deg;

        var rotation = rb.rotation.eulerAngles;
        rotation.z = angle;
        rb.rotation = Quaternion.Euler(rotation);
    }

    // ---------------------------------------------------------------
    // ---------------------------------------------------------------
    // ---------------------------------------------------------------



}
