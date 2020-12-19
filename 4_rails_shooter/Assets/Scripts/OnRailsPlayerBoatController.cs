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
    [SerializeField] Vector3 movementSpeedVector;

    [SerializeField] private Vector3 controllerRotationSpeed = new Vector3(30f, 200f, 30f);
    [SerializeField] private Vector3 mouseRotationSpeed      = new Vector3(0.3f, 5f, 0.3f);

    [Header("Clamp Ranges")]
    [SerializeField] private ClampRange horizontalClampRange;   // x-axis movement(along)
    [SerializeField] private ClampRange verticalClampRange;     // y-axis    "
    [SerializeField] private ClampRange depthClampRange;        // z-axis    "

    [SerializeField] private ClampRange pitchClampRange;        // z-axis rotation(about)
    [SerializeField] private ClampRange yawClampRange;          // y-axis    "
    [SerializeField] private ClampRange rollClampRange;         // x-axis    "

    
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

        applyFinalPhysicsAnimation();
    }

    // Input Callbacks ------------------------------------------------------------------


    public void OnMove(InputAction.CallbackContext ctx)
    {
        if (ctx.canceled)
        {
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

    private void MovePlayerBoat(Vector2 input)
    {
        Vector3 vec;

        vec = buildRawMovementVector(input);
        vec = applyMovementCalculations(vec);
        vec = clampMovementVector(vec);

        transform.localPosition = vec;
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

    Vector3 applyMovementCalculations(Vector3 raw)
    {
        var movement = transform.localPosition;

        // change current position, according to each component
        // each component, is increased by the displacement vector component
        //           , scaled by the corresponding movement vector component
        // all of these are also scaled by dt
        movement.x += Time.deltaTime * raw.x * movementSpeedVector.x;
        movement.y += Time.deltaTime * raw.y * movementSpeedVector.y;
        movement.z += Time.deltaTime * raw.z * movementSpeedVector.z;

        return movement;
    }

    Vector3 clampMovementVector(Vector3 movement)
    {    
        movement.x = Mathf.Clamp(movement.x, horizontalClampRange.min, horizontalClampRange.max );
        movement.y = Mathf.Clamp(movement.y, verticalClampRange.min  , verticalClampRange.max   );
        movement.z = Mathf.Clamp(movement.z, depthClampRange.min     , depthClampRange.max      );

        return movement;
    }
    
    private void RotatePlayerBoat(Vector2 input)
    {
        Vector3 vec;
        vec = buildRawRotationEuler(input);
        vec = calculateRotation(vec);
        vec = clampRotationEuler(vec);
        applyLocalRotation(vec);
    }

    // takes vec2 as input, and creates a vec3 representation
    Vector3 buildRawRotationEuler(Vector2 input)
    {
        var rotation = Vector3.zero;

        rotation += input.x * Vector3.up;
        rotation += input.y * Vector3.right;

        return rotation;
    }

    Vector3 calculateRotation(Vector3 raw)
    {
        raw.x *= Time.deltaTime * currentRotationSpeed.x;
        raw.y *= Time.deltaTime * currentRotationSpeed.y;
        raw.z *= Time.deltaTime * currentRotationSpeed.z;

        return raw;
    }

    Vector3 clampRotationEuler(Vector3 rotation)
    {
        //rotation.x = clampRotationDegrees(rotation.x, pitchClampRange.min, pitchClampRange.max);
        //rotation.y = clampRotationDegrees(rotation.y, yawClampRange.min, yawClampRange.max);
        //rotation.z = clampRotationDegrees(rotation.z, rollClampRange.min, rollClampRange.max);

        return rotation;
    }

    // Applies axis angles rotation to local transform
    void applyLocalRotation(Vector3 v)
    {
        currentRotationVector = v;

        var rotation = transform.localRotation;
        rotation *= Quaternion.AngleAxis(currentRotationVector.x, Vector3.right);
        rotation *= Quaternion.AngleAxis(currentRotationVector.y, Vector3.up);
        rotation *= Quaternion.AngleAxis(currentRotationVector.z, Vector3.forward);
        transform.localRotation = rotation;
    }

    void applyFinalPhysicsAnimation()
    {
        var xPos = transform.localPosition.x;
        var calculatedRotation = xPos * -0.1f;

        var rotation = transform.rotation;
        rotation *= Quaternion.AngleAxis(calculatedRotation, Vector3.forward);
        transform.rotation = rotation;
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
