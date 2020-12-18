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

[System.Serializable]
public struct PhysicsAnimationCache
{
    [System.NonSerialized] public float pitch;
    [System.NonSerialized] public float yaw;
    [System.NonSerialized] public float roll;

    [SerializeField] public float pitchFalloff;
    [SerializeField] public float yawFalloff;
    [SerializeField] public float rollFalloff;
}

public class OnRailsPlayerBoatController : MonoBehaviour
{
    [Header("Movement Speeds")]
    [Tooltip("Movement speed of the player, per axis, to be applied in local space")]
    [SerializeField] Vector3 movementSpeedVector;

    private Vector3 currentRotationSpeed = Vector3.one;
    [SerializeField] private Vector3 controllerRotationSpeed = new Vector3(30f, 200f, 30f);
    [SerializeField] private Vector3 mouseRotationSpeed      = new Vector3(0.3f, 5f, 0.3f);

    [Header("Clamp Ranges")]
    [SerializeField] private ClampRange horizontalClampRange;   // x-axis movement(along)
    [SerializeField] private ClampRange verticalClampRange;     // y-axis    "
    [SerializeField] private ClampRange depthClampRange;        // z-axis    "

    [SerializeField] private ClampRange pitchClampRange;        // z-axis rotation(about)
    [SerializeField] private ClampRange yawClampRange;          // y-axis    "
    [SerializeField] private ClampRange rollClampRange;         // x-axis    "
    
    bool isMoving = false;
    Vector2 moveVectorForThisTick = Vector2.zero;

    bool isRotating = false;
    Vector2 lookVectorForThisTick = Vector2.zero;

    [Header("Physical Animation")]
    [SerializeField] private PhysicsAnimationCache physAnimCache;


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
            MoveCharacter(moveVectorForThisTick);
        }

        if (isRotating)
        {
            RotateCharacter(lookVectorForThisTick);
        }

        //applyFinalPhysicsAnimation();
    }

    // Input Callbacks ------------------------------------------------------------------


    public void OnMove(InputAction.CallbackContext ctx)
    {
        //if (ctx.started || ctx.performed)
        //{
        //    isMoving = true;
        //}
        //else if (ctx.canceled)
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
        //if (ctx.started || ctx.performed)
        //{
        //    isRotating = true;
        //}
        //else if (ctx.canceled)
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

    private void MoveCharacter(Vector2 input)
    {
        Vector3 vec;

        vec = buildRawMovementVector(input);
        {
            cacheMovementForRotation(vec); // for physical animation
        }
        vec = applyMovementCalculations(vec);
        vec = clampMovementVector(vec);

        transform.localPosition = vec;
    }

    // takes the Vec2 input, and creates a Vec3 representation
    Vector3 buildRawMovementVector(Vector2 input)
    {
        var displacementVector = Vector3.zero;

        displacementVector += input.x * transform.right;
        displacementVector += input.y * transform.up;         // y as up-down
      //displacementVector += input.y * transform.forward;    // y as depth

        return displacementVector;
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
    
    private void RotateCharacter(Vector2 input)
    {
        Vector3 vec;

        vec = buildRawRotationEuler(input);
        vec = applyRotationCalculations(vec);
        vec = clampRotationEuler(vec);

        transform.localRotation = Quaternion.Euler(vec.x, vec.y, vec.z);
    }

    // takes vec2 as input, and creates a vec3 representation
    Vector3 buildRawRotationEuler(Vector2 input)
    {
        var rotation = Vector3.zero;

        rotation += input.x * Vector3.up;
      //rotation += input.y * Vector3.right;

        return rotation;
    }

    Vector3 applyRotationCalculations(Vector3 raw)
    {
        var rotation = transform.localEulerAngles;

        rotation.x += Time.deltaTime * raw.x * currentRotationSpeed.x;
        rotation.y += Time.deltaTime * raw.y * currentRotationSpeed.y;
        rotation.z += Time.deltaTime * raw.z * currentRotationSpeed.z;

        return rotation;
    }

    Vector3 clampRotationEuler(Vector3 rotation)
    {
        //rotation.x = Mathf.Clamp(rotation.x, pitchClampRange.min, pitchClampRange.max );
        ////rotation.y = Mathf.Clamp(rotation.y, yawClampRange.min,   yawClampRange.max   );
        //rotation.z = Mathf.Clamp(rotation.z, rollClampRange.min,  rollClampRange.max  );

        return rotation;
    }


    void applyFinalPhysicsAnimation()
    {
        var rotation = transform.eulerAngles;
        rotation.x += physAnimCache.pitch;
        rotation.y += physAnimCache.yaw;
        rotation.z += physAnimCache.roll;

        rotation = clampRotationEuler(rotation);

        transform.rotation = Quaternion.Euler(rotation.x, rotation.y, rotation.z);

        perfomPhysicsAnimationUpkeep();
    }

    private void perfomPhysicsAnimationUpkeep()
    {
        physAnimCache.pitch = calculateValueFalloff(physAnimCache.pitch);
        physAnimCache.yaw   = calculateValueFalloff(physAnimCache.yaw  );
        physAnimCache.roll  = calculateValueFalloff(physAnimCache.roll );


        moveVectorForThisTick = Vector3.zero;
        lookVectorForThisTick = Vector3.zero;
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

    private void cacheMovementForRotation(Vector3 vec)
    {
        // NOTE: The incoming vectors represent the translations applied to the ship
        // so pitch, (rotation about x-axis) is association with translation along y-axis
        // roll, (rotation about z-axis) is associated with translation along x-axis
        // and hypothetically, nothing comes in for yaw
        physAnimCache.pitch +=  vec.y;
        physAnimCache.yaw   +=  vec.z;
        physAnimCache.roll  += -vec.x;
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
