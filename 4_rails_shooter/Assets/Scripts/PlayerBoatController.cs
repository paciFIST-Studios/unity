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
    [Tooltip("Movement speed of the player, to be applied in local space")]
    [SerializeField] Vector3 movementSpeedVector;
    [SerializeField] float debugSprintMultiplier = 20f;

    [Tooltip("DO NOT USE.  This is the current value used for rotation.  To change rotation, change either controller rotation, or mouse rotation")]
    [SerializeField] private float currentRotationSpeed = 1f;
    [SerializeField] private float controllerRotationSpeed = 200f;
    [SerializeField] private float mouseRotationSpeed = 5f;

    [SerializeField] private float xAxisClampMag;
    [SerializeField] private ClampVec2 yAxisClampHeight;
    [SerializeField] private float fixedPlayerDepth = 20f;

    bool isMoving = false;
    Vector2 moveVectorForThisTick = Vector2.zero;

    bool isRotating = false;
    float rotationThisTick = 0f;

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
            RotateCharacter(rotationThisTick);
        }
    }

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
        rotationThisTick = lookVector.x;

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


    private void MoveCharacter(Vector2 vec)
    {
        var displacementVector = Vector3.zero;
        displacementVector += vec.x * transform.right;
        displacementVector += vec.y * transform.up;         // y as up-down
      //displacementVector += vec.y * transform.forward;    // y as depth


        if (Keyboard.current.leftShiftKey.isPressed)
        {
            displacementVector *= debugSprintMultiplier;
        }

        Vector3 currentPos = transform.localPosition;

        // change current position, according to each component
        // each component, is increased by the displacement vector component
        //           , scaled by the corresponding movement vector component
        // all of these are also scaled by dt
        currentPos.x += Time.deltaTime * displacementVector.x * movementSpeedVector.x;
        currentPos.y += Time.deltaTime * displacementVector.y * movementSpeedVector.y;
        currentPos.z += Time.deltaTime * displacementVector.z * movementSpeedVector.z;

        // clamp resulting change to acceptable boundary
        currentPos.x = Mathf.Clamp(currentPos.x, -xAxisClampMag, xAxisClampMag);
        currentPos.y = Mathf.Clamp(currentPos.y, yAxisClampHeight.min, yAxisClampHeight.max);
        currentPos.z = fixedPlayerDepth;

        transform.localPosition = currentPos;
    }


    private void RotateCharacter(float degrees)
    {
        var ro = transform.localRotation.eulerAngles;
        ro.y += degrees * currentRotationSpeed * Time.deltaTime;
        transform.localRotation = Quaternion.Euler(ro.x, ro.y, ro.z);
    }

}
