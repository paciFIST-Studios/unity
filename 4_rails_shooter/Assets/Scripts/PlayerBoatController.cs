using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


[System.Serializable]
public struct ClampVec2
{
    [SerializeField] public float min;
    [SerializeField] public float max;
}

public class PlayerBoatController : MonoBehaviour
{
    [Header("Player Stats")]
    [Tooltip("Movement speed of the player, per axis, to be applied in local space")]
    [SerializeField] Vector3 movementSpeedVector;
    [Tooltip("Rotation speed of the player, per axis, to be applied in local space")]
    [SerializeField] Vector3 rotationSpeedVector;
    [SerializeField] float debugSprintMultiplier = 20f;

    //[SerializeField] private float currentRotationSpeed = 1f;
    //[SerializeField] private float controllerRotationSpeed = 200f;
    //[SerializeField] private float mouseRotationSpeed = 5f;
    [Tooltip("DO NOT USE.  This is the current value used for rotation.  To change rotation, change either controller rotation, or mouse rotation")]
    [SerializeField] private Vector3 currentRotationSpeed    = Vector3.one;
    [SerializeField] private Vector3 controllerRotationSpeed = new Vector3(30f, 200f, 30f);
    [SerializeField] private Vector3 mouseRotationSpeed      = new Vector3(0.3f, 5f, 0.3f);

    [SerializeField] private float xAxisClampMag;
    [SerializeField] private ClampVec2 yAxisClampHeight;
    [SerializeField] private float fixedPlayerDepth = 20f;

    bool isMoving = false;
    Vector2 moveVectorForThisTick = Vector2.zero;

    bool isRotating = false;
    Vector2 lookVectorForThisTick = Vector2.zero;

    // Unity Functions ------------------------------------------------------------------

    private void Start()
    {
        var pos = transform.localPosition;
        pos.z = fixedPlayerDepth;
        transform.localPosition = pos;
    }

    void FixedUpdate()
    {
        if (isMoving)
        {
            MoveCharacter(moveVectorForThisTick);
        }

        if (isRotating)
        {
            RotateCharacter(lookVectorForThisTick);
        }
    }

    // Input Callbacks ------------------------------------------------------------------


    public void OnMove(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            isMoving = true;
        }
        else if (ctx.canceled)
        {
            isMoving = false;
            return;
        }
        // else if started

        print("OnMove() : " + ctx.action.name);

        moveVectorForThisTick = ctx.ReadValue<Vector2>();
    }


    public void OnLook(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            isRotating = true;
        }
        else if (ctx.canceled)
        {
            isRotating = false;
            return;
        }

        var lookVector = ctx.ReadValue<Vector2>();
        lookVectorForThisTick = lookVector;

        // check to see if input was from mouse delta
        if (ctx.action.GetBindingDisplayString() == "Delta")
        {
            currentRotationSpeed = mouseRotationSpeed;
        }
        else
        {
            currentRotationSpeed = controllerRotationSpeed;
        }
    }

    // Class Functions ------------------------------------------------------------------

    private void MoveCharacter(Vector2 input)
    {
        if (Keyboard.current.leftShiftKey.isPressed)
        {
            input *= debugSprintMultiplier;
        }

        var vec = buildRawMovementVector(input);
        vec = applyMovementCalculations(vec);
        transform.localPosition = clampMovementVector(vec);
    }

    // takes the Vec2 input, and creates a Vec3 representation of it
    Vector3 buildRawMovementVector(Vector2 input)
    {
        var displacementVector = Vector3.zero;
        displacementVector += input.x * transform.right;
        displacementVector += input.y * transform.up;         // y as up-down
      //displacementVector += input.y * transform.forward;    // y as depth

        return displacementVector;
    }

    // takes the Vec3 input, and applies all movement calculations
    Vector3 applyMovementCalculations(Vector3 displacement)
    {
        Vector3 movementVector = transform.localPosition;

        // change current position, according to each component
        // each component, is increased by the displacement vector component
        //           , scaled by the corresponding movement vector component
        // all of these are also scaled by dt
        movementVector.x += Time.deltaTime * displacement.x * movementSpeedVector.x;
        movementVector.y += Time.deltaTime * displacement.y * movementSpeedVector.y;
        movementVector.z += Time.deltaTime * displacement.z * movementSpeedVector.z;

        return movementVector;
    }

    // clamps movement vector to acceptable ranges
    Vector3 clampMovementVector(Vector3 movement)
    {    
        movement.x = Mathf.Clamp(movement.x, -xAxisClampMag, xAxisClampMag);
        movement.y = Mathf.Clamp(movement.y, yAxisClampHeight.min, yAxisClampHeight.max);
        movement.z = fixedPlayerDepth;

        return movement;
    }


    private void RotateCharacter(Vector2 input)
    {
        var vec = buildRawRotationEuler(input);
        vec = applyRotationCalculations(vec);
        vec = clampRotationEuler(vec);
        transform.localRotation = Quaternion.Euler(vec.x, vec.y, vec.z);

        //var ro = transform.localRotation.eulerAngles;
        //ro.y += degrees * currentRotationSpeed * Time.deltaTime;
        //transform.localRotation = Quaternion.Euler(ro.x, ro.y, ro.z);
    }


    Vector3 buildRawRotationEuler(Vector2 input)
    {
        var rotation = Vector3.zero;

        rotation += input.x * Vector3.up;
        //rotation += input.y * Vector3.right;

        return rotation;
    }

    Vector3 applyRotationCalculations(Vector3 vec)
    {
        var rotation = transform.localEulerAngles;

        rotation.x += Time.deltaTime * vec.x * currentRotationSpeed.x;
        rotation.y += Time.deltaTime * vec.y * currentRotationSpeed.y;
        rotation.z += Time.deltaTime * vec.z * currentRotationSpeed.z;

        return rotation;
    }

    Vector3 clampRotationEuler(Vector3 vec)
    {
        return vec;
    }



}
