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

public class PlayerBoatController : MonoBehaviour
{
    [Header("Movement Speeds")]
    [Tooltip("Movement speed of the player, per axis, to be applied in local space")]
    [SerializeField] Vector3 movementSpeedVector;

    private Vector3 currentRotationSpeed = Vector3.one;
    [SerializeField] private Vector3 controllerRotationSpeed = new Vector3(30f, 200f, 30f);
    [SerializeField] private Vector3 mouseRotationSpeed      = new Vector3(0.3f, 5f, 0.3f);

    [Header("Clamp Ranges")]
    [SerializeField] private ClampRange horizontalClampRange;
    [SerializeField] private ClampRange verticalClampRange;
    [SerializeField] private ClampRange depthClampRange;
    [SerializeField] private ClampRange pitchClampRange;
    [SerializeField] private ClampRange yawClampRange;
    [SerializeField] private ClampRange rollClampRange;
    
    bool isMoving = false;
    Vector2 moveVectorForThisTick = Vector2.zero;

    bool isRotating = false;
    Vector2 lookVectorForThisTick = Vector2.zero;

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

        lookVectorForThisTick = ctx.ReadValue<Vector2>();

        // check to see if input was from mouse delta
        var style = ctx.action.GetBindingDisplayString();
        matchRotationSpeedToInputStyle(style);
    }

    // Class Functions ------------------------------------------------------------------

    private void MoveCharacter(Vector2 input)
    {
        Vector3 vec;

        vec = buildRawMovementVector(input);
        vec = applyMovementCalculations(vec);

        transform.localPosition = clampMovementVector(vec);
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

    Vector3 applyMovementCalculations(Vector3 displacement)
    {
        var movementVector = transform.localPosition;

        // change current position, according to each component
        // each component, is increased by the displacement vector component
        //           , scaled by the corresponding movement vector component
        // all of these are also scaled by dt
        movementVector.x += Time.deltaTime * displacement.x * movementSpeedVector.x;
        movementVector.y += Time.deltaTime * displacement.y * movementSpeedVector.y;
        movementVector.z += Time.deltaTime * displacement.z * movementSpeedVector.z;

        return movementVector;
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
        vec.x = Mathf.Clamp(vec.x, pitchClampRange.min, pitchClampRange.max );
        vec.y = Mathf.Clamp(vec.y, yawClampRange.min,   yawClampRange.max   );
        vec.z = Mathf.Clamp(vec.z, rollClampRange.min,  rollClampRange.max  );

        return vec;
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

}
