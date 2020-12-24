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
    [SerializeField] PIDController pid;

    [SerializeField] float hoverForce = 10f;
    [SerializeField] float targetAltitude = 1f;

    [SerializeField] float groundMovementForce = 100f;

    private Rigidbody rb;

    private bool isMoving = false;
    private bool isLooking = false;

    private Vector2 movementInputThisTick;
    private Vector2 lookInputThisTick;

    // ---------------------------------------------------------------

    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        
    }

    void FixedUpdate()
    {
        float currentAltitude = transform.position.y;

        float error = targetAltitude - currentAltitude;
        var correction = pid.Update(error);
        rb.AddForce(Vector3.up * hoverForce * correction);

        if(isMoving)
        {
            MovePlayerCharacter(movementInputThisTick);
        }

        if(isLooking)
        { }

    }

    // Input System Callbacks ----------------------------------------

    public void OnLook(InputAction.CallbackContext ctx)
    {

    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        if(ctx.canceled || ctx.performed)
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

    void RotatePlayerCharacter()
    {

    }

    // ---------------------------------------------------------------
    // ---------------------------------------------------------------
    // ---------------------------------------------------------------



}
