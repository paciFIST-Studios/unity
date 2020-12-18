using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBoatController : MonoBehaviour
{
    [Header("Player Stats")]
    [SerializeField] float lateralMoveSpeed = 1f;
    [SerializeField] float rotationSpeed = 1f;
    [SerializeField] float debugSprintMultiplier = 20f;

    [SerializeField] private float controllerRotationSpeed = 200f;
    [SerializeField] private float mouseRotationSpeed = 5f;

    [SerializeField] private float xAxisClampMag;
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
        if (ctx.performed)
        {
            isMoving = true;
        }
        else if (ctx.canceled)
        {
            isMoving = false;
            return;
        }

        print("OnMove() : " + ctx.action.name);

        moveVectorForThisTick = ctx.ReadValue<Vector2>();
    }


    public void OnLook(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
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


        if (ctx.action.GetBindingDisplayString() == "Delta")
        {
            rotationSpeed = mouseRotationSpeed;
        }
        else
        {
            rotationSpeed = controllerRotationSpeed;
        }
    }


    private void MoveCharacter(Vector2 vec)
    {
        Vector3 moveVector = transform.right * vec.x;

        // moveVector += transform.up * vec.y       // y as up-down
        // moveVector += transform.forward * vec.y  // y as depth

        if (Keyboard.current.leftShiftKey.isPressed)
        {
            moveVector *= debugSprintMultiplier;
        }

        var currentPos = transform.localPosition;
        currentPos += Time.deltaTime * moveVector * lateralMoveSpeed;
        currentPos.x = Mathf.Clamp(currentPos.x, -xAxisClampMag, xAxisClampMag);
        currentPos.z = fixedPlayerDepth;
        transform.localPosition = currentPos;
    }


    private void RotateCharacter(float degrees)
    {
        var ro = transform.localRotation.eulerAngles;
        ro.y += degrees * rotationSpeed * Time.deltaTime;
        transform.localRotation = Quaternion.Euler(ro.x, ro.y, ro.z);
    }

}
