using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


[System.Serializable]
public struct ClampRange
{
    [SerializeField] public float min;
    [SerializeField] public float max;
}

public class OnRailsPlayerBoatController : MonoBehaviour
{
    [Header("Movement Speeds")]
    [Tooltip("Movement speed of the player, per axis, to be applied in local space")]
    [SerializeField] Vector3 movementSpeedsPerVector;

    [SerializeField] private Vector3 controllerRotationSpeed = new Vector3(30f, 200f, 1.0f);
    [SerializeField] private Vector3 mouseRotationSpeed      = new Vector3(0.3f, 5f, 0.3f);

    [Header("Clamp Ranges")]
    [SerializeField] private ClampRange horizontalClampRange;   // x-axis movement(along)
    [SerializeField] private ClampRange verticalClampRange;     // y-axis    "
    [SerializeField] private ClampRange depthClampRange;        // z-axis    "

    [SerializeField] private ClampRange pitchClampRange;        // z-axis rotation(about)
    [SerializeField] private ClampRange yawClampRange;          // y-axis    "
    [SerializeField] private ClampRange rollClampRange;         // x-axis    "

    [SerializeField] private float rollSelfRightingSpeed = 1f;
    
    // these are used in LateUpdate
    private bool isMoving   = false;
    private bool isRotating = false;
    private Vector2 moveVectorForThisTick = Vector2.zero;
    private Vector2 lookVectorForThisTick = Vector2.zero;

    private Vector3 currentRotationVector = Vector3.zero;
    private Vector3 currentRotationSpeed  = Vector3.one;


    // Unity Functions ------------------------------------------------------------------

    private void Start()
    {
        var pos = transform.localPosition;
        pos.z = depthClampRange.min;
        transform.localPosition = pos;
    }

    void FixedUpdate()
    {
        if (isMoving)
        {
            MovePlayerBoat(moveVectorForThisTick);
        }

        if (isRotating)
        {
            RotatePlayerBoat(lookVectorForThisTick);
        }

        applyPhysicalAnimation();
    }

    // Input Callbacks ------------------------------------------------------------------


    public void OnMove(InputAction.CallbackContext ctx)
    {
        if (ctx.canceled)
        {
            moveVectorForThisTick = Vector2.zero;
            isMoving = false;
            return;
        }
        isMoving = true;

        moveVectorForThisTick = ctx.ReadValue<Vector2>();
    }


    public void OnLook(InputAction.CallbackContext ctx)
    {
        if (ctx.canceled)
        {
            lookVectorForThisTick = Vector2.zero;
            isRotating = false;
            return;
        }
        isRotating = true;

        lookVectorForThisTick = ctx.ReadValue<Vector2>();

        // we use different values for mouse vs controller rotation
        var style = ctx.action.GetBindingDisplayString();
        matchRotationSpeedToInputStyle(style);
    }

    // Class Functions ------------------------------------------------------------------

    // Movement ----
    private void MovePlayerBoat(Vector2 input)
    {
        Vector3 vec;
        vec = buildRawMovementVector(input);
        vec = refineMovementVector(vec);
        applyMovementVector(vec);
    }

    // takes the Vec2 input, and creates a Vec3 representation
    Vector3 buildRawMovementVector(Vector2 input)
    {
        var raw = Vector3.zero;

        raw += input.x * transform.right;
        raw += input.y * transform.up;         // y as up-down
      //raw += input.y * transform.forward;    // y as depth

        return raw;
    }

    Vector3 refineMovementVector(Vector3 raw)
    {
        raw.x *= Time.deltaTime * movementSpeedsPerVector.x;
        raw.y *= Time.deltaTime * movementSpeedsPerVector.y;
        raw.z *= Time.deltaTime * movementSpeedsPerVector.z;

        return raw;
    }

    Vector3 clampMovementVector(Vector3 movement)
    {    
        movement.x = Mathf.Clamp(movement.x, horizontalClampRange.min, horizontalClampRange.max );
        movement.y = Mathf.Clamp(movement.y, verticalClampRange.min  , verticalClampRange.max   );
        movement.z = Mathf.Clamp(movement.z, depthClampRange.min     , depthClampRange.max      );

        return movement;
    }
    
    private void applyMovementVector(Vector3 vec)
    {
        // clamping the position isn't merely clamping the incoming
        // movement, but the incoming movement plus the current position
        // so we have to sample the current position (by adding it in)
        // before we perform the clamp
        vec += transform.localPosition;
        vec = clampMovementVector(vec);
        transform.localPosition = vec;
    }


    // Rotation ----
    private void RotatePlayerBoat(Vector2 input)
    {
        Vector3 vec;
        vec = buildRawRotationEuler(input);
        vec = refineRotationEuler(vec);
        applyRotationEuler(vec);
    }

    // takes vec2 as input, and creates a vec3 representation
    Vector3 buildRawRotationEuler(Vector2 input)
    {
        var rotation = Vector3.zero;

        rotation += input.x * Vector3.up;
        rotation += input.y * Vector3.right;

        return rotation;
    }

    Vector3 refineRotationEuler(Vector3 raw)
    {
        raw.x *= Time.deltaTime * currentRotationSpeed.x;
        raw.y *= Time.deltaTime * currentRotationSpeed.y;
        raw.z *= Time.deltaTime * currentRotationSpeed.z;

        return raw;
    }

    Vector3 clampRotationEuler(Vector3 euler)
    {
        //euler.x = clampSingleAxisRotation(euler.x, pitchClampRange.min, pitchClampRange.max);
        //euler.y = clampSingleAxisRotation(euler.y, yawClampRange.min, yawClampRange.max);
        //euler.z = clampSingleAxisRotation(euler.z, rollClampRange.min, rollClampRange.max);

        return euler;
    }

    float clampSingleAxisRotation(float value, float min, float max)
    {
        value += 360f;
        min += 360f;
        max += 360f;
        Mathf.Clamp(value, min, max);
        value -= 360f;   
        return value;
    }    

    // Applies axis angles rotation to local transform
    void applyRotationEuler(Vector3 euler)
    {
        var rotation = transform.localRotation;

        // sample local rotation so we can perform a good clamp
        //euler += rotation.eulerAngles;
        //euler = clampRotationEuler(euler);

        rotation *= Quaternion.AngleAxis(euler.x, Vector3.right);
        rotation *= Quaternion.AngleAxis(euler.y, Vector3.up);
        rotation *= Quaternion.AngleAxis(euler.z, Vector3.forward);
        transform.localRotation = rotation;
    }

    void applyPhysicalAnimation()
    {
        performRollDueToUserInput();
        performIncrementalSelfRighting();
    }

    void performRollDueToUserInput()
    {
        var xInput = moveVectorForThisTick.x * -1f;
        var roll = xInput * currentRotationSpeed.z;

        roll = clampRollDueToUserInput(roll);

        var rotation = transform.localRotation;
        rotation *= Quaternion.AngleAxis(roll, Vector3.forward);
        transform.localRotation = rotation;
    }

    private float clampRollDueToUserInput(float roll)
    {
        var currentRoll = transform.localEulerAngles.z;

        // clamp roll too far left
        if (currentRoll < 180f && currentRoll >= 0f)
        {
            if (currentRoll > rollClampRange.max)
            {
                // if we're greater than the limit and we would
                // continue on in that direction
                if (roll > 0f)
                {
                    // prevent further roll
                    roll = 0f;
                }
            }
        }
        // clamp roll too far right
        else if (currentRoll >= 180f && currentRoll <= 360f)
        {
            if (currentRoll <= rollClampRange.min  + 360f)
            {
                // if we're lower than the max, but we would
                // keep getting lower
                if(roll < 0f)
                {
                    // prevent further roll
                    roll = 0f;
                }
            }
        }
        else
        {
            print("Rotation error: OnRailsPlayerBoatController::ClampRoll !");
        }

        return roll;
    }

    void performIncrementalSelfRighting()
    {
        var rotation = transform.localRotation;
        var currentRoll = rotation.eulerAngles.z;

        // set roll to zero, if we're close to zero
        if(Mathf.Abs(currentRoll) < float.Epsilon)
        {
            transform.localRotation = Quaternion.Euler(
                  transform.localEulerAngles.x
                , transform.localEulerAngles.y
                , 0f
                );
            return;
        }


        float roll = 0f;
        if(currentRoll >= 180f && currentRoll <= 360f)
        {
            roll = Time.deltaTime * (360f - currentRoll) * rollSelfRightingSpeed;
        }
        else if (currentRoll < 180f && currentRoll > 0f)
        {
            roll = Time.deltaTime * -currentRoll * rollSelfRightingSpeed;
        }
        else
        {
            print("Rotation error: OnRailsPlayerBoatController::SelfRighting !");
        }

        rotation *= Quaternion.AngleAxis(roll, Vector3.forward);
        transform.localRotation = rotation;
    }


    // Utilities ---------------------------------------------------------------------------

    private void matchRotationSpeedToInputStyle(string style)
    {
        // check to see if input was from mouse delta, vs game pad input
        if (style == "Delta")
        {
            currentRotationSpeed = mouseRotationSpeed;
        }
        else
        {
            currentRotationSpeed = controllerRotationSpeed;
        }
    }

    private float calculateValueFalloff(float value, float fallRate = 0.5f, float fallTo = 0.0f, float threshhold = 0.001f)
    {
        var result = value * fallRate;
        if (Mathf.Abs(result) < Mathf.Abs(fallTo + threshhold))
        {
            result = fallTo;
        }

        return result;
    }

}
